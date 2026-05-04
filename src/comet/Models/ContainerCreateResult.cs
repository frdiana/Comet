namespace Comet.Models;

public sealed record ContainerCreateResult(string Name, string PartitionKeyPath, int? Throughput, string Status);

