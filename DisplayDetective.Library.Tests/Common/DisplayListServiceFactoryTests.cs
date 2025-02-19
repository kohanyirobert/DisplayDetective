using DisplayDetective.Library.Common;
using DisplayDetective.Library.Windows;

namespace DisplayDetective.Library.Tests.Windows;

[Trait("Category", "Unit")]
public class DisplayListServiceFactoryTests
{
    [Fact]
    public void Create_ReturnsServiceOnWindows_OtherwiseThrows()
    {
        if (OperatingSystem.IsWindows())
        {
            var service = DisplayListServiceFactory.Create();
            Assert.NotNull(service);
            Assert.IsType<WindowsDisplayListService>(service);
        }
        else
        {
            Assert.Throws<PlatformNotSupportedException>(() => DisplayMonitorServiceFactory.Create());
        }
    }
}