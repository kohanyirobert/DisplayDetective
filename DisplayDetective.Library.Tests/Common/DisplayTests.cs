using DisplayDetective.Library.Common;

namespace DisplayDetective.Library.Tests.Common;

[Trait("Category", "Unit")]
public class DisplayTests
{
    [Fact]
    public void Display_Properties_SetCorrectly()
    {
        var display = Display.Create("test-device-id", "test-name", "test-manufacturer", "test-description");
        Assert.Equal("test-device-id", display.DeviceID);
        Assert.Equal("test-name", display.Name);
        Assert.Equal("test-manufacturer", display.Manufacturer);
        Assert.Equal("test-description", display.Description);
    }

    [Fact]
    public void ToString_ReturnsCorrectFormat()
    {
        var display = Display.Create("test-device-id", "test-name", "test-manufacturer", "test-description");
        var expected = "Name=test-name Manufacturer=test-manufacturer Description=test-description DeviceID=test-device-id";
        Assert.Equal(expected, display.ToString());
    }

    [Fact]
    public void ToMultilineString_ReturnsCorrectFormat()
    {
        var display = Display.Create("test-device-id", "test-name", "test-manufacturer", "test-description");
        var expected = """
                Name = test-name
        Manufacturer = test-manufacturer
         Description = test-description
            DeviceID = test-device-id
        """;
        Assert.Equal(expected, display.ToMultilineString());
    }
}