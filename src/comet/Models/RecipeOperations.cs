namespace Comet.Models;

public sealed record RecipeOperations(
    IReadOnlyList<CreateDatabaseRequest> Create,
    IReadOnlyList<CloneDatabaseRequest> Clones);

