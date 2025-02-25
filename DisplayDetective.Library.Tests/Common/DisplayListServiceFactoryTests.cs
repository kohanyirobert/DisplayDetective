using System.Runtime.Versioning;

using DisplayDetective.Library.Common;
using DisplayDetective.Library.Windows;

namespace DisplayDetective.Library.Tests.Windows;

[Trait("Category", "Unit")]
public class DisplayListServiceFactoryTests
{
    [SupportedOSPlatform("windows")]
    [Fact]
    public void Create_OnWindows_Works()
    {
        bool isWindows = true;
        var factory = new DisplayListServiceFactory(isWindows);
        var service = factory.Create();
        Assert.NotNull(service);
        Assert.IsType<WindowsDisplayListService>(service);
    }

    [UnsupportedOSPlatform("windows")]
    [Fact]
    public void Create_NotOnWindows_DoesNotWork()
    {
        bool isWindows = false;
        var factory = new DisplayListServiceFactory(isWindows);
        Assert.Throws<PlatformNotSupportedException>(() => factory.Create());
    }
}