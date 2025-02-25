using System.Runtime.Versioning;

using DisplayDetective.Library.Common;
using DisplayDetective.Library.Windows;

namespace DisplayDetective.Library.Tests.Windows;

[SupportedOSPlatform("windows")]
public class WindowsDisplayMonitorServiceTests
{
    [Fact]
    public void DisplayCreatedAndDeletedEvent_NoErrors()
    {
        var service = new WindowsDisplayMonitorService();
        var handlerMock = new Mock<EventHandler<IDisplay>>();

        service.OnDisplayCreated += handlerMock.Object;
        service.OnDisplayDeleted += handlerMock.Object;

        handlerMock.Verify(m => m(It.IsAny<object>(), It.IsAny<IDisplay>()), Times.Never());
    }
}