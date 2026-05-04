using CosmosDbComet.Models;
using Microsoft.Azure.Cosmos;

namespace CosmosDbComet.Cosmos;

public sealed class DatabaseInspector(CosmosClientFactory clientFactory)
{
    public async Task<IReadOnlyList<ContainerInspection>> InspectAsync(CosmosConnection connection, string databaseName, CancellationToken cancellationToken)
    {
        using var client = clientFactory.Create(connection);
        var database = client.GetDatabase(databaseName);
        var iterator = database.GetContainerQueryIterator<ContainerProperties>();
        var containers = new List<ContainerInspection>();

        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync(cancellationToken);
            foreach (var properties in page)
            {
                var container = database.GetContainer(properties.Id);
                var throughput = await ReadThroughputOrNullAsync(container, cancellationToken);
                containers.Add(new ContainerInspection(properties.Id, properties.PartitionKeyPath, throughput));
            }
        }

        return containers;
    }

    private static async Task<int?> ReadThroughputOrNullAsync(Container container, CancellationToken cancellationToken)
    {
        try
        {
            return await container.ReadThroughputAsync(cancellationToken: cancellationToken);
        }
        catch (CosmosException)
        {
            return null;
        }
    }
}