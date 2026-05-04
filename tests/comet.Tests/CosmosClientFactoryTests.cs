using Comet.Cosmos;

namespace Comet.Tests;

public sealed class CosmosClientFactoryTests
{
    [Fact]
    public void LooksLikeConnectionString_DetectsCosmosConnectionString()
    {
        var result = CosmosClientFactory.LooksLikeConnectionString(
            "AccountEndpoint=https://example.documents.azure.com:443/;AccountKey=fake;");

        Assert.True(result);
    }

    [Fact]
    public void LooksLikeConnectionString_DoesNotTreatEndpointAsConnectionString()
    {
        var result = CosmosClientFactory.LooksLikeConnectionString("https://example.documents.azure.com:443/");

        Assert.False(result);
    }
}

