using System.Runtime.Versioning;

using DisplayDetective.Library.Common;
using DisplayDetective.Library.Windows;

using Microsoft.Extensions.Logging;

namespace DisplayDetective.Library.Tests.Windows;

[Trait("Category", "Unit")]
public class DisplayListServiceFactoryTests
{
    [SupportedOSPlatform("windows")]
    [Fact]
    public void Create_OnWindows_Works()
    {
        var provider = new Mock<IServiceProvider>();
        provider.Setup(m => m.GetService(typeof(ILogger<WindowsDisplayListService>)))
            .Returns(Mock.Of<ILogger<WindowsDisplayListService>>());

        bool isWindows = true;

        var factory = new DisplayListServiceFactory(provider.Object, isWindows);
        var service = factory.Create();

        Assert.NotNull(service);
        Assert.IsType<WindowsDisplayListService>(service);

        provider.Verify(m => m.GetService(typeof(ILogger<WindowsDisplayListService>)), Times.Once());
        provider.VerifyNoOtherCalls();
    }

    [UnsupportedOSPlatform("windows")]
    [Fact]
    public void Create_NotOnWindows_DoesNotWork()
    {
        var provider = new Mock<IServiceProvider>();
        bool isWindows = false;
        var factory = new DisplayListServiceFactory(provider.Object, isWindows);
        Assert.Throws<PlatformNotSupportedException>(factory.Create);
        provider.VerifyNoOtherCalls();
    }
}