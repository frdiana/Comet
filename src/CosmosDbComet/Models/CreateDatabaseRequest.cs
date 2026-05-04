namespace CosmosDbComet.Models;

public sealed record CreateDatabaseRequest(
    CosmosConnection Target,
    string DatabaseName,
    IReadOnlyList<ContainerSpec> Containers,
    int? Throughput);