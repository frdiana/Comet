namespace Comet.Models;

public sealed record CosmosConnection(string Value, string? Auth)
{
    public string EffectiveAuth => string.IsNullOrWhiteSpace(Auth) ? "az-cli" : Auth.Trim();
}

