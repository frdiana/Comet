namespace Comet.Models;

public sealed record ContainerInspection(string Name, string PartitionKeyPath, int? Throughput);

