using System.Runtime.Versioning;

using DisplayDetective.Library.Windows;

using Microsoft.Extensions.Logging;

namespace DisplayDetective.Library.Tests.Windows;

[Trait("Category", "Acceptance")]
[SupportedOSPlatform("windows")]
public class WindowsDisplayListServiceTests
{
    [Fact]
    public void GetDisplays_ReturnsNonEmptyList()
    {
        var loggerMock = new Mock<ILogger<WindowsDisplayListService>>();
        var service = new WindowsDisplayListService(loggerMock.Object);
        service.ListDisplays();

        loggerMock.VerifyLogMatch(LogLevel.Information, Times.Once(), @"(?i).+scanning");
        loggerMock.VerifyLogMatch(LogLevel.Information, Times.Once(), @"(?i).+found \d+ display");
        loggerMock.VerifyLogMatch(LogLevel.Information, Times.Once(), @"(?i).+display 1");
        loggerMock.VerifyLog(LogLevel.Error, Times.Never());
    }
}