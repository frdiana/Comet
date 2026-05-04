using CosmosDbComet.Cosmos;
using CosmosDbComet.Models;
using CosmosDbComet.Parsing;
using Spectre.Console.Cli;

namespace CosmosDbComet.Commands;

public sealed class CloneDbCommand : AsyncCommand<CloneDbSettings>
{
    protected override async Task<int> ExecuteAsync(CommandContext context, CloneDbSettings settings, CancellationToken cancellationToken)
    {
        var request = new CloneDatabaseRequest(
            new CosmosConnection(settings.Source!, settings.SourceAuth ?? settings.Auth),
            new CosmosConnection(settings.Target!, settings.TargetAuth ?? settings.Auth),
            settings.Database!,
            settings.To ?? settings.Database!,
            ListOptionParser.Parse(settings.ExcludeContainers),
            settings.SchemaOnly,
            settings.MaxItems,
            settings.BatchSize);

        if (settings.DryRun)
        {
            PlanRenderer.RenderClonePlan(request);
            return 0;
        }

        var cloner = new DatabaseCloner(new CosmosClientFactory(), new DocumentCopier());
        var result = await cloner.CloneAsync(request, cancellationToken);
        PlanRenderer.RenderCloneResult(result);
        return 0;
    }
}