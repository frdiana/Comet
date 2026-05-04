namespace Comet.Models;

public sealed record CloneDatabaseRequest(
    CosmosConnection Source,
    CosmosConnection Target,
    string SourceDatabaseName,
    string TargetDatabaseName,
    IReadOnlySet<string> ExcludedContainers,
    bool SchemaOnly,
    int? MaxItems,
    int BatchSize);

