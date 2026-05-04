using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;

namespace Comet.Cosmos;

public sealed class DocumentCopier
{
    public async Task<DocumentCopyResult> CopyAsync(
        Container source,
        Container target,
        int batchSize,
        int? maxItems,
        Action<string>? log,
        CancellationToken cancellationToken)
    {
        if (maxItems is <= 0)
        {
            return new DocumentCopyResult(0, true);
        }

        var copied = 0;
        var iterator = source.GetItemQueryIterator<JObject>(
            new QueryDefinition("SELECT * FROM c"),
            requestOptions: new QueryRequestOptions { MaxItemCount = batchSize });

        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync(cancellationToken);
            var pageCount = 0;
            foreach (var item in page)
            {
                if (maxItems is not null && copied >= maxItems.Value)
                {
                    log?.Invoke($"  Batch: +{pageCount} items | Total: {copied} (max-items reached)");
                    return new DocumentCopyResult(copied, true);
                }

                await target.UpsertItemAsync(item, cancellationToken: cancellationToken);
                copied++;
                pageCount++;
            }

            log?.Invoke($"  Batch: +{pageCount} items | Total: {copied}");
        }

        return new DocumentCopyResult(copied, false);
    }
}

public sealed record DocumentCopyResult(int CopiedItems, bool ReachedMaxItems);

