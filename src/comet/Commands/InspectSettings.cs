using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace Comet.Commands;

public sealed class InspectSettings : CommandSettings
{
    [CommandOption("--source <VALUE>")]
    [Description("Source Cosmos DB endpoint or connection string.")]
    public string? Source { get; init; }

    [CommandOption("--auth <MODE>")]
    [Description("Authentication mode for endpoint auth: az-cli or default.")]
    public string? Auth { get; init; }

    [CommandOption("--database <NAME>")]
    [Description("Database name to inspect.")]
    public string? Database { get; init; }

    public override ValidationResult Validate()
    {
        if (string.IsNullOrWhiteSpace(Source))
        {
            return ValidationResult.Error("Specify --source.");
        }

        if (string.IsNullOrWhiteSpace(Database))
        {
            return ValidationResult.Error("Specify --database.");
        }

        return ValidationResult.Success();
    }
}

