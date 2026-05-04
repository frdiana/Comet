using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace CosmosDbComet.Commands;

public sealed class CloneDbSettings : CommandSettings
{
    [CommandOption("--source <VALUE>")]
    [Description("Source Cosmos DB endpoint or connection string.")]
    public string? Source { get; init; }

    [CommandOption("--target <VALUE>")]
    [Description("Target Cosmos DB endpoint or connection string.")]
    public string? Target { get; init; }

    [CommandOption("--auth <MODE>")]
    [Description("Authentication mode for both endpoints: az-cli or default.")]
    public string? Auth { get; init; }

    [CommandOption("--source-auth <MODE>")]
    [Description("Authentication mode for the source endpoint.")]
    public string? SourceAuth { get; init; }

    [CommandOption("--target-auth <MODE>")]
    [Description("Authentication mode for the target endpoint.")]
    public string? TargetAuth { get; init; }

    [CommandOption("--database <NAME>")]
    [Description("Source database name.")]
    public string? Database { get; init; }

    [CommandOption("--to <NAME>")]
    [Description("Target database name. Defaults to --database.")]
    public string? To { get; init; }

    [CommandOption("--exclude-containers <JSON_OR_CSV>")]
    [Description("Container names to exclude as JSON array or comma-separated list.")]
    public string? ExcludeContainers { get; init; }

    [CommandOption("--schema-only")]
    [Description("Clone only database and container schema.")]
    public bool SchemaOnly { get; init; }

    [CommandOption("--max-items <COUNT>")]
    [Description("Maximum number of documents to copy across all containers.")]
    public int? MaxItems { get; init; }

    [CommandOption("--batch-size <COUNT>")]
    [Description("Query page size for document copy.")]
    [DefaultValue(100)]
    public int BatchSize { get; init; } = 100;

    [CommandOption("--dry-run")]
    [Description("Show the plan without creating or copying resources.")]
    public bool DryRun { get; init; }

    public override ValidationResult Validate()
    {
        if (string.IsNullOrWhiteSpace(Source))
        {
            return ValidationResult.Error("Specify --source.");
        }

        if (string.IsNullOrWhiteSpace(Target))
        {
            return ValidationResult.Error("Specify --target.");
        }

        if (string.IsNullOrWhiteSpace(Database))
        {
            return ValidationResult.Error("Specify --database.");
        }

        if (MaxItems is <= 0)
        {
            return ValidationResult.Error("--max-items must be greater than zero.");
        }

        if (BatchSize <= 0)
        {
            return ValidationResult.Error("--batch-size must be greater than zero.");
        }

        return ValidationResult.Success();
    }
}