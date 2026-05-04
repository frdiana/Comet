using System.Text.Json;

namespace Comet.Recipes;

public static class CometRecipeReader
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    public static async Task<CometRecipe> ReadAsync(string path, CancellationToken cancellationToken)
    {
        await using var stream = File.OpenRead(path);
        var recipe = await JsonSerializer.DeserializeAsync<CometRecipe>(stream, JsonOptions, cancellationToken);
        return recipe ?? throw new InvalidOperationException("Recipe file is empty or invalid.");
    }
}

