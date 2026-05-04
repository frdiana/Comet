using CosmosDbComet.Models;
using CosmosDbComet.Parsing;

namespace CosmosDbComet.Recipes;

public static class CometRecipeExtensions
{
    public static RecipeOperations ToOperations(this CometRecipe recipe)
    {
        var create = recipe.Create.Select(ToCreateRequest).ToArray();
        var clone = recipe.Clone.Select(ToCloneRequest).ToArray();

        if (create.Length == 0 && clone.Length == 0)
        {
            throw new InvalidOperationException("Recipe must contain at least one create or clone operation.");
        }

        return new RecipeOperations(create, clone);
    }

    private static CreateDatabaseRequest ToCreateRequest(CreateRecipeItem item)
    {
        Require(item.Target, "create.target");
        Require(item.Database, "create.database");

        if (item.Containers.Count == 0)
        {
            throw new InvalidOperationException("create.containers must contain at least one entry.");
        }

        if (item.Throughput is <= 0)
        {
            throw new InvalidOperationException("create.throughput must be greater than zero.");
        }

        var containers = item.Containers.Select(ContainerSpecParser.Parse).ToArray();
        return new CreateDatabaseRequest(new CosmosConnection(item.Target!, item.Auth), item.Database!, containers, item.Throughput);
    }

    private static CloneDatabaseRequest ToCloneRequest(CloneRecipeItem item)
    {
        Require(item.Source, "clone.source");
        Require(item.Target, "clone.target");
        Require(item.Database, "clone.database");

        if (item.MaxItems is <= 0)
        {
            throw new InvalidOperationException("clone.maxItems must be greater than zero.");
        }

        if (item.BatchSize <= 0)
        {
            throw new InvalidOperationException("clone.batchSize must be greater than zero.");
        }

        return new CloneDatabaseRequest(
            new CosmosConnection(item.Source!, item.SourceAuth ?? item.Auth),
            new CosmosConnection(item.Target!, item.TargetAuth ?? item.Auth),
            item.Database!,
            item.To ?? item.Database!,
            item.Exclude.ToHashSet(StringComparer.OrdinalIgnoreCase),
            item.SchemaOnly,
            item.MaxItems,
            item.BatchSize);
    }

    private static void Require(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{fieldName} is required.");
        }
    }
}