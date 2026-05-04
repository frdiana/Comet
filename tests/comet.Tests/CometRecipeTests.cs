using Comet.Recipes;
using System.Text.Json;

namespace Comet.Tests;

public sealed class CometRecipeTests
{
    [Fact]
    public void ToOperations_MapsCompactCreateAndCloneRecipe()
    {
        var recipe = JsonSerializer.Deserialize<CometRecipe>("""
        {
          "create": [
            {
              "target": "https://target.documents.azure.com:443/",
              "auth": "az-cli",
              "database": "sales",
              "containers": ["orders/tenantId", "customers/customerId"]
            }
          ],
          "clone": [
            {
              "source": "https://source.documents.azure.com:443/",
              "target": "https://target.documents.azure.com:443/",
              "database": "sales",
              "exclude": ["logs"]
            }
          ]
        }
        """, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        var operations = recipe!.ToOperations();

        Assert.Single(operations.Create);
        Assert.Single(operations.Clones);
        Assert.Equal("sales", operations.Create[0].DatabaseName);
        Assert.Equal("/tenantId", operations.Create[0].Containers[0].PartitionKeyPath);
        Assert.Equal("sales", operations.Clones[0].TargetDatabaseName);
        Assert.Contains("logs", operations.Clones[0].ExcludedContainers);
    }

    [Fact]
    public void ToOperations_RejectsEmptyRecipe()
    {
        var recipe = new CometRecipe();

        var error = Assert.Throws<InvalidOperationException>(recipe.ToOperations);

        Assert.Contains("at least one", error.Message);
    }
}

