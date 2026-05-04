using Comet.Models;
using Spectre.Console;

namespace Comet;

public static class PlanRenderer
{
    public static void RenderCreatePlan(CreateDatabaseRequest request)
    {
        AnsiConsole.MarkupLineInterpolated($"[yellow]Dry run:[/] create database [bold]{request.DatabaseName}[/]");
        RenderContainers(request.Containers.Select(item => new ContainerCreateResult(item.Name, item.PartitionKeyPath, request.Throughput, "planned")));
    }

    public static void RenderCreateResults(string databaseName, IReadOnlyList<ContainerCreateResult> results)
    {
        AnsiConsole.MarkupLineInterpolated($"[green]Database ready:[/] [bold]{databaseName}[/]");
        RenderContainers(results);
    }

    public static void RenderClonePlan(CloneDatabaseRequest request)
    {
        var table = new Table().Title("Clone plan");
        table.AddColumn("Source");
        table.AddColumn("Target");
        table.AddColumn("Exclude");
        table.AddColumn("Mode");
        table.AddRow(
            request.SourceDatabaseName,
            request.TargetDatabaseName,
            request.ExcludedContainers.Count == 0 ? "none" : string.Join(", ", request.ExcludedContainers),
            request.SchemaOnly ? "schema only" : "schema + data");
        AnsiConsole.Write(table);
    }

    public static void RenderCloneResult(CloneResult result)
    {
        var table = new Table().Title("Clone result");
        table.AddColumn("Source");
        table.AddColumn("Target");
        table.AddColumn("Containers");
        table.AddColumn("Excluded");
        table.AddColumn("Copied items");
        table.AddRow(
            result.SourceDatabaseName,
            result.TargetDatabaseName,
            result.ClonedContainers.Count.ToString(),
            result.ExcludedContainers.Count == 0 ? "none" : string.Join(", ", result.ExcludedContainers),
            result.SchemaOnly ? "schema only" : result.CopiedItems.ToString());
        AnsiConsole.Write(table);

        if (result.ReachedMaxItems)
        {
            AnsiConsole.MarkupLine("[yellow]Stopped because --max-items was reached.[/]");
        }
    }

    public static void RenderInspectResult(string databaseName, IReadOnlyList<ContainerInspection> containers)
    {
        var table = new Table().Title($"Database: {databaseName}");
        table.AddColumn("Container");
        table.AddColumn("Partition key");
        table.AddColumn("Throughput");

        foreach (var container in containers)
        {
            table.AddRow(container.Name, container.PartitionKeyPath, container.Throughput?.ToString() ?? "shared/unknown");
        }

        AnsiConsole.Write(table);
    }

    public static void RenderRecipePlan(string path, RecipeOperations operations)
    {
        AnsiConsole.MarkupLineInterpolated($"[yellow]Dry run:[/] recipe [bold]{path}[/]");

        foreach (var create in operations.Create)
        {
            RenderCreatePlan(create);
        }

        foreach (var clone in operations.Clones)
        {
            RenderClonePlan(clone);
        }
    }

    private static void RenderContainers(IEnumerable<ContainerCreateResult> containers)
    {
        var table = new Table();
        table.AddColumn("Container");
        table.AddColumn("Partition key");
        table.AddColumn("Throughput");
        table.AddColumn("Status");

        foreach (var container in containers)
        {
            table.AddRow(container.Name, container.PartitionKeyPath, container.Throughput?.ToString() ?? "default", container.Status);
        }

        AnsiConsole.Write(table);
    }
}

