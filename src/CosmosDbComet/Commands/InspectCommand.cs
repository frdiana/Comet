using CosmosDbComet.Cosmos;
using CosmosDbComet.Models;
using Spectre.Console.Cli;

namespace CosmosDbComet.Commands;

public sealed class InspectCommand : AsyncCommand<InspectSettings>
{
    protected override async Task<int> ExecuteAsync(CommandContext context, InspectSettings settings, CancellationToken cancellationToken)
    {
        var inspector = new DatabaseInspector(new CosmosClientFactory());
        var containers = await inspector.InspectAsync(
            new CosmosConnection(settings.Source!, settings.Auth),
            settings.Database!,
            cancellationToken);

        PlanRenderer.RenderInspectResult(settings.Database!, containers);
        return 0;
    }
}