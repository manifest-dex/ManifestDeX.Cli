using ManifestDeX.Cli.Presentation.Parsing;

namespace ManifestDeX.Cli.UnitTests;

public class CommandParserTests
{
    [Fact]
    public void Parse_ShouldReadAuthSetKey()
    {
        var parsed = CommandParser.Parse(["auth", "set-key", "abc"]);

        Assert.Equal("auth:set-key", parsed.Name);
        Assert.Single(parsed.Arguments);
        Assert.Equal("abc", parsed.Arguments[0]);
    }

    [Fact]
    public void Parse_ShouldUseJsonOutput_WhenJsonFlagProvided()
    {
        var parsed = CommandParser.Parse(["usage", "--json"]);

        Assert.Equal("usage", parsed.Name);
        Assert.Equal(OutputMode.Json, parsed.OutputMode);
    }
}
