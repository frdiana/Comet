namespace CosmosDbComet.Models;

public sealed record RecipeOperations(
    IReadOnlyList<CreateDatabaseRequest> Create,
    IReadOnlyList<CloneDatabaseRequest> Clones);