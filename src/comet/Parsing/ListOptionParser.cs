using System.Text.Json;

namespace Comet.Parsing;

public static class ListOptionParser
{
    public static IReadOnlySet<string> Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        var items = ParseItems(value);
        return items.ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public static IReadOnlyList<string> ParseItems(string value)
    {
        var trimmed = value.Trim();
        if (trimmed.Length == 0)
        {
            return [];
        }

        if (trimmed.StartsWith('['))
        {
            try
            {
                return JsonSerializer.Deserialize<List<string>>(trimmed) ?? [];
            }
            catch (JsonException ex)
            {
                throw new ArgumentException("List options must be a JSON array of strings or a comma-separated list.", ex);
            }
        }

        return trimmed
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(item => item.Length > 0)
            .ToArray();
    }
}

