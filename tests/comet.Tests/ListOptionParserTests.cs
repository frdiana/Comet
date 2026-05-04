using Comet.Parsing;

namespace Comet.Tests;

public sealed class ListOptionParserTests
{
    [Fact]
    public void Parse_ReturnsEmptySetForMissingValue()
    {
        var result = ListOptionParser.Parse(null);

        Assert.Empty(result);
    }

    [Fact]
    public void Parse_ReadsJsonArrayCaseInsensitively()
    {
        var result = ListOptionParser.Parse("[\"logs\",\"audit\"]");

        Assert.Contains("LOGS", result);
        Assert.Contains("audit", result);
    }

    [Fact]
    public void Parse_ReadsCommaSeparatedList()
    {
        var result = ListOptionParser.Parse("logs, audit");

        Assert.Equal(2, result.Count);
        Assert.Contains("logs", result);
        Assert.Contains("audit", result);
    }
}

