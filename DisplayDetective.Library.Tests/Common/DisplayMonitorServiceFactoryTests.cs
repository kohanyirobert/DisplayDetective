using DisplayDetective.Library.Common;
using DisplayDetective.Library.Windows;

namespace DisplayDetective.Library.Tests.Windows;

[Trait("Category", "Unit")]
public class DisplayMonitorServiceFactoryTests
{
    [Fact]
    public void Create_ReturnsServiceOnWindows_OtherwiseThrows()
    {
        if (OperatingSystem.IsWindows())
        {
            var service = DisplayMonitorServiceFactory.Create();
            Assert.NotNull(service);
            Assert.IsType<WindowsDisplayMonitorService>(service);
        }
        else
        {
            Assert.Throws<PlatformNotSupportedException>(() => DisplayMonitorServiceFactory.Create());
        }
    }
}