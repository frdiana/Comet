using CosmosDbComet.Cosmos;
using CosmosDbComet.Recipes;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CosmosDbComet.Commands;

public sealed class RunCommand : AsyncCommand<RunSettings>
{
    protected override async Task<int> ExecuteAsync(CommandContext context, RunSettings settings, CancellationToken cancellationToken)
    {
        var recipe = await CometRecipeReader.ReadAsync(settings.FromFile!, cancellationToken);
        var operations = recipe.ToOperations();

        if (settings.DryRun)
        {
            PlanRenderer.RenderRecipePlan(settings.FromFile!, operations);
            return 0;
        }

        var creator = new DatabaseCreator(new CosmosClientFactory());
        var cloner = new DatabaseCloner(new CosmosClientFactory(), new DocumentCopier());

        foreach (var create in operations.Create)
        {
            try
            {
                var results = await creator.CreateAsync(create, cancellationToken);
                PlanRenderer.RenderCreateResults(create.DatabaseName, results);
            }
            catch (Exception ex) when (settings.ContinueOnError)
            {
                AnsiConsole.MarkupLineInterpolated($"[red]create failed:[/] {ex.Message}");
            }
        }

        foreach (var clone in operations.Clones)
        {
            try
            {
                var result = await cloner.CloneAsync(clone, cancellationToken);
                PlanRenderer.RenderCloneResult(result);
            }
            catch (Exception ex) when (settings.ContinueOnError)
            {
                AnsiConsole.MarkupLineInterpolated($"[red]clone failed:[/] {ex.Message}");
            }
        }

        return 0;
    }
}