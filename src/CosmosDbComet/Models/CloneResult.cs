namespace CosmosDbComet.Models;

public sealed record CloneResult(
    string SourceDatabaseName,
    string TargetDatabaseName,
    IReadOnlyList<string> ClonedContainers,
    IReadOnlyList<string> ExcludedContainers,
    int CopiedItems,
    bool ReachedMaxItems,
    bool SchemaOnly);