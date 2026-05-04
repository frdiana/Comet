using System.Text.Json.Serialization;

namespace Comet.Recipes;

public sealed class CometRecipe
{
    [JsonPropertyName("create")]
    public List<CreateRecipeItem> Create { get; init; } = [];

    [JsonPropertyName("clone")]
    public List<CloneRecipeItem> Clone { get; init; } = [];
}

public sealed class CreateRecipeItem
{
    [JsonPropertyName("target")]
    public string? Target { get; init; }

    [JsonPropertyName("auth")]
    public string? Auth { get; init; }

    [JsonPropertyName("database")]
    public string? Database { get; init; }

    [JsonPropertyName("containers")]
    public List<string> Containers { get; init; } = [];

    [JsonPropertyName("throughput")]
    public int? Throughput { get; init; }
}

public sealed class CloneRecipeItem
{
    [JsonPropertyName("source")]
    public string? Source { get; init; }

    [JsonPropertyName("target")]
    public string? Target { get; init; }

    [JsonPropertyName("auth")]
    public string? Auth { get; init; }

    [JsonPropertyName("sourceAuth")]
    public string? SourceAuth { get; init; }

    [JsonPropertyName("targetAuth")]
    public string? TargetAuth { get; init; }

    [JsonPropertyName("database")]
    public string? Database { get; init; }

    [JsonPropertyName("to")]
    public string? To { get; init; }

    [JsonPropertyName("exclude")]
    public List<string> Exclude { get; init; } = [];

    [JsonPropertyName("schemaOnly")]
    public bool SchemaOnly { get; init; }

    [JsonPropertyName("maxItems")]
    public int? MaxItems { get; init; }

    [JsonPropertyName("batchSize")]
    public int BatchSize { get; init; } = 100;
}

