using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace Comet.Commands;

public sealed class CreateDbSettings : CommandSettings
{
    [CommandOption("--target <VALUE>")]
    [Description("Target Cosmos DB endpoint or connection string.")]
    public string? Target { get; init; }

    [CommandOption("--auth <MODE>")]
    [Description("Authentication mode for endpoint auth: az-cli or default.")]
    public string? Auth { get; init; }

    [CommandOption("--database <NAME>")]
    [Description("Database name to create.")]
    public string? Database { get; init; }

    [CommandOption("--containers <JSON_OR_CSV>")]
    [Description("Container specs as JSON array or comma-separated list, for example [\"orders/tenantId\"].")]
    public string? Containers { get; init; }

    [CommandOption("--throughput <RU>")]
    [Description("Optional database/container throughput.")]
    public int? Throughput { get; init; }

    [CommandOption("--dry-run")]
    [Description("Show the plan without creating resources.")]
    public bool DryRun { get; init; }

    public override ValidationResult Validate()
    {
        if (string.IsNullOrWhiteSpace(Target))
        {
            return ValidationResult.Error("Specify --target.");
        }

        if (string.IsNullOrWhiteSpace(Database))
        {
            return ValidationResult.Error("Specify --database.");
        }

        if (string.IsNullOrWhiteSpace(Containers))
        {
            return ValidationResult.Error("Specify --containers.");
        }

        if (Throughput is <= 0)
        {
            return ValidationResult.Error("--throughput must be greater than zero.");
        }

        return ValidationResult.Success();
    }
}

