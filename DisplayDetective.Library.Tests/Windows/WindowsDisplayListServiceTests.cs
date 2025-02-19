using System.Runtime.Versioning;

using DisplayDetective.Library.Windows;

namespace DisplayDetective.Library.Tests.Windows;

[Trait("Category", "Acceptance")]
[SupportedOSPlatform("windows")]
public class WindowsDisplayListServiceTests
{
    [Fact]
    public void GetDisplays_ReturnsNonEmptyList()
    {
        var service = new WindowsDisplayListService();
        var displays = service.GetDisplays();

        Assert.NotNull(displays);
        Assert.NotEmpty(displays);

        var display = displays.First();
        Assert.NotEmpty(display.DeviceID);
        Assert.NotEmpty(display.Name);
        Assert.NotEmpty(display.Manufacturer);
        Assert.NotEmpty(display.Description);
    }
}