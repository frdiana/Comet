using CosmosDbComet.Models;

namespace CosmosDbComet.Parsing;

public static class ContainerSpecParser
{
    public static IReadOnlyList<ContainerSpec> ParseMany(string value)
    {
        var items = ListOptionParser.ParseItems(value);
        if (items.Count == 0)
        {
            throw new ArgumentException("At least one container spec is required.");
        }

        return items.Select(Parse).ToArray();
    }

    public static ContainerSpec Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Container spec cannot be empty.");
        }

        var separatorIndex = value.IndexOf('/');
        if (separatorIndex <= 0 || separatorIndex == value.Length - 1)
        {
            throw new ArgumentException($"Container spec '{value}' must use containerName/partitionKey.");
        }

        var name = value[..separatorIndex].Trim();
        var partitionKey = value[(separatorIndex + 1)..].Trim().TrimStart('/');

        if (name.Length == 0 || partitionKey.Length == 0)
        {
            throw new ArgumentException($"Container spec '{value}' must use containerName/partitionKey.");
        }

        return new ContainerSpec(name, "/" + partitionKey);
    }
}