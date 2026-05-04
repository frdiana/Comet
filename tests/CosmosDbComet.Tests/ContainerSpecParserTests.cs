using CosmosDbComet.Parsing;

namespace CosmosDbComet.Tests;

public sealed class ContainerSpecParserTests
{
    [Fact]
    public void Parse_CreatesPartitionKeyPath()
    {
        var spec = ContainerSpecParser.Parse("orders/tenantId");

        Assert.Equal("orders", spec.Name);
        Assert.Equal("/tenantId", spec.PartitionKeyPath);
    }

    [Fact]
    public void Parse_KeepsNestedPartitionKeyPath()
    {
        var spec = ContainerSpecParser.Parse("events/context/userId");

        Assert.Equal("events", spec.Name);
        Assert.Equal("/context/userId", spec.PartitionKeyPath);
    }

    [Fact]
    public void Parse_RejectsMissingPartitionKey()
    {
        var error = Assert.Throws<ArgumentException>(() => ContainerSpecParser.Parse("orders"));

        Assert.Contains("containerName/partitionKey", error.Message);
    }

    [Fact]
    public void ParseMany_ReadsJsonArray()
    {
        var specs = ContainerSpecParser.ParseMany("[\"orders/tenantId\",\"customers/customerId\"]");

        Assert.Collection(
            specs,
            first => Assert.Equal("orders", first.Name),
            second => Assert.Equal("customers", second.Name));
    }

    [Fact]
    public void ParseMany_ReadsCommaSeparatedList()
    {
        var specs = ContainerSpecParser.ParseMany("orders/tenantId, customers/customerId");

        Assert.Equal(2, specs.Count);
    }
}