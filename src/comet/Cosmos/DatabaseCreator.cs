using Comet.Models;
using Microsoft.Azure.Cosmos;

namespace Comet.Cosmos;

public sealed class DatabaseCreator(CosmosClientFactory clientFactory)
{
    public async Task<IReadOnlyList<ContainerCreateResult>> CreateAsync(CreateDatabaseRequest request, CancellationToken cancellationToken)
    {
        using var client = clientFactory.Create(request.Target);
        var databaseResponse = await client.CreateDatabaseIfNotExistsAsync(request.DatabaseName, request.Throughput, cancellationToken: cancellationToken);
        var database = databaseResponse.Database;

        var results = new List<ContainerCreateResult>();
        foreach (var container in request.Containers)
        {
            var properties = new ContainerProperties(container.Name, container.PartitionKeyPath);
            var response = await database.CreateContainerIfNotExistsAsync(properties, request.Throughput, cancellationToken: cancellationToken);
            results.Add(new ContainerCreateResult(
                container.Name,
                container.PartitionKeyPath,
                request.Throughput,
                response.StatusCode.ToString()));
        }

        return results;
    }
}

