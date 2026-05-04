using CosmosDbComet.Cosmos;
using CosmosDbComet.Models;
using CosmosDbComet.Parsing;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CosmosDbComet.Commands;

public sealed class CreateDbCommand : AsyncCommand<CreateDbSettings>
{
    protected override async Task<int> ExecuteAsync(CommandContext context, CreateDbSettings settings, CancellationToken cancellationToken)
    {
        var containers = ContainerSpecParser.ParseMany(settings.Containers!);
        var request = new CreateDatabaseRequest(
            new CosmosConnection(settings.Target!, settings.Auth),
            settings.Database!,
            containers,
            settings.Throughput);

        if (settings.DryRun)
        {
            PlanRenderer.RenderCreatePlan(request);
            return 0;
        }

        var creator = new DatabaseCreator(new CosmosClientFactory());
        var results = await creator.CreateAsync(request, cancellationToken);
        PlanRenderer.RenderCreateResults(settings.Database!, results);
        return 0;
    }
}