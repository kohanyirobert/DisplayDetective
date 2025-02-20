using System.Text;

using DisplayDetective.Library.Common;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DisplayDetective.Library.Tests.Common;

[Trait("Category", "Unit")]
public class DisplayDetectiveServiceTests
{
    private const string DeviceID = "TestDeviceID";
    private static readonly IDisplay TestDisplay = Display.Create(DeviceID, "TestName", "TestManufacturer", "TestDescription");

    private static readonly IConfiguration Configuration = new ConfigurationBuilder()
        .AddJsonFile($"testsettings.{nameof(DisplayDetectiveServiceTests)}.json")
        .Build();

    [Fact]
    public void RunAsync_Works_WhenEverythingIsInPlace()
    {
        var monitorMock = new Mock<IDisplayMonitorService>();
        var runnerMock = new Mock<ICommandRunnerService>();

        var detectiveService = new DisplayDetectiveService(
            Mock.Of<ILogger<DisplayDetectiveService>>(),
            Configuration,
            monitorMock.Object,
            runnerMock.Object);

        var detectiveTask = detectiveService.RunAsync(TestContext.Current.CancellationToken);

        monitorMock.VerifyAdd(m => m.OnDisplayCreated += It.IsAny<EventHandler<IDisplay>>(), Times.Once);
        monitorMock.VerifyAdd(m => m.OnDisplayDeleted += It.IsAny<EventHandler<IDisplay>>(), Times.Once);
        monitorMock.VerifyNoOtherCalls();
        monitorMock.Raise(m => m.OnDisplayCreated += null, this, TestDisplay);

        runnerMock.Verify(m => m.Run(
            It.Is<string>(s => !string.IsNullOrWhiteSpace(s)),
            It.Is<string[]>(x => x != null && x.Length > 0),
            TestContext.Current.CancellationToken), Times.Once);
        runnerMock.VerifyNoOtherCalls();
    }
}