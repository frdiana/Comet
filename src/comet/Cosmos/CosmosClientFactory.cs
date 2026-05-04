using Azure.Core;
using Azure.Identity;
using Comet.Models;
using Microsoft.Azure.Cosmos;

namespace Comet.Cosmos;

public sealed class CosmosClientFactory
{
    public CosmosClient Create(CosmosConnection connection)
    {
        if (LooksLikeConnectionString(connection.Value))
        {
            return new CosmosClient(connection.Value);
        }

        if (!Uri.TryCreate(connection.Value, UriKind.Absolute, out var endpoint) || endpoint.Scheme != Uri.UriSchemeHttps)
        {
            throw new InvalidOperationException("Cosmos connection must be a connection string or HTTPS endpoint.");
        }

        return new CosmosClient(endpoint.ToString(), CreateCredential(connection.EffectiveAuth));
    }

    public static bool LooksLikeConnectionString(string value)
    {
        return value.Contains("AccountEndpoint=", StringComparison.OrdinalIgnoreCase)
            || value.Contains("AccountKey=", StringComparison.OrdinalIgnoreCase)
            || value.Contains("ResourceId=", StringComparison.OrdinalIgnoreCase);
    }

    private static TokenCredential CreateCredential(string auth)
    {
        return auth.Trim().ToLowerInvariant() switch
        {
            "az-cli" => new AzureCliCredential(),
            "default" => new DefaultAzureCredential(),
            _ => throw new InvalidOperationException("Auth must be 'az-cli' or 'default'.")
        };
    }
}

