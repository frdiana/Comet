using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace CosmosDbComet.Commands;

public sealed class RunSettings : CommandSettings
{
    [CommandOption("--from-file <PATH>")]
    [Description("Path to a compact comet JSON file.")]
    public string? FromFile { get; init; }

    [CommandOption("--dry-run")]
    [Description("Show the plan without creating or copying resources.")]
    public bool DryRun { get; init; }

    [CommandOption("--continue-on-error")]
    [Description("Continue with remaining operations after a failure.")]
    public bool ContinueOnError { get; init; }

    public override ValidationResult Validate()
    {
        if (string.IsNullOrWhiteSpace(FromFile))
        {
            return ValidationResult.Error("Specify --from-file.");
        }

        if (!File.Exists(FromFile))
        {
            return ValidationResult.Error($"Recipe file not found: {FromFile}");
        }

        return ValidationResult.Success();
    }
}