using CosmosDbComet.Models;
using Microsoft.Azure.Cosmos;

namespace CosmosDbComet.Cosmos;

public sealed class DatabaseCloner(CosmosClientFactory clientFactory, DocumentCopier documentCopier)
{
    public async Task<CloneResult> CloneAsync(CloneDatabaseRequest request, CancellationToken cancellationToken)
    {
        using var sourceClient = clientFactory.Create(request.Source);
        using var targetClient = clientFactory.Create(request.Target);

        var sourceDatabase = sourceClient.GetDatabase(request.SourceDatabaseName);
        var targetDatabaseResponse = await targetClient.CreateDatabaseIfNotExistsAsync(request.TargetDatabaseName, cancellationToken: cancellationToken);
        var targetDatabase = targetDatabaseResponse.Database;

        var clonedContainers = new List<string>();
        var copiedItems = 0;
        var reachedMaxItems = false;
        var sourceContainers = await ReadContainerPropertiesAsync(sourceDatabase, cancellationToken);

        foreach (var sourceProperties in sourceContainers)
        {
            if (request.ExcludedContainers.Contains(sourceProperties.Id))
            {
                continue;
            }

            var targetProperties = CopyContainerProperties(sourceProperties);
            var sourceContainer = sourceDatabase.GetContainer(sourceProperties.Id);
            var targetThroughput = await ReadThroughputOrNullAsync(sourceContainer, cancellationToken);
            await targetDatabase.CreateContainerIfNotExistsAsync(targetProperties, targetThroughput, cancellationToken: cancellationToken);
            clonedContainers.Add(sourceProperties.Id);

            if (!request.SchemaOnly && !reachedMaxItems)
            {
                var copyResult = await documentCopier.CopyAsync(
                    sourceContainer,
                    targetDatabase.GetContainer(sourceProperties.Id),
                    request.BatchSize,
                    request.MaxItems is null ? null : request.MaxItems - copiedItems,
                    cancellationToken);

                copiedItems += copyResult.CopiedItems;
                reachedMaxItems = copyResult.ReachedMaxItems;
            }
        }

        return new CloneResult(
            request.SourceDatabaseName,
            request.TargetDatabaseName,
            clonedContainers,
            request.ExcludedContainers.Order(StringComparer.OrdinalIgnoreCase).ToArray(),
            copiedItems,
            reachedMaxItems,
            request.SchemaOnly);
    }

    private static async Task<IReadOnlyList<ContainerProperties>> ReadContainerPropertiesAsync(Database database, CancellationToken cancellationToken)
    {
        var iterator = database.GetContainerQueryIterator<ContainerProperties>();
        var containers = new List<ContainerProperties>();

        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync(cancellationToken);
            containers.AddRange(page);
        }

        return containers;
    }

    private static ContainerProperties CopyContainerProperties(ContainerProperties source)
    {
        return new ContainerProperties(source.Id, source.PartitionKeyPath)
        {
            IndexingPolicy = source.IndexingPolicy,
            UniqueKeyPolicy = source.UniqueKeyPolicy,
            DefaultTimeToLive = source.DefaultTimeToLive,
            ConflictResolutionPolicy = source.ConflictResolutionPolicy
        };
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