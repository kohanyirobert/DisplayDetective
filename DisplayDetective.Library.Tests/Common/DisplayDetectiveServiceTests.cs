using System.Text;

using DisplayDetective.Library.Common;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DisplayDetective.Library.Tests.Common;

[Trait("Category", "Unit")]
public class DisplayDetectiveServiceTests
{
    const string DeviceID = "TestDeviceID";
    static readonly IDisplay TestDisplay = Display.Create(DeviceID, "TestName", "TestManufacturer", "TestDescription");

    [Fact]
    public async Task RunAsync_Works_WhenEverythingIsInPlace()
    {
        var logger = new Mock<ILogger<DisplayDetectiveService>>();

        var configuration = new ConfigurationBuilder()
            .AddJsonFile($"testsettings.{nameof(DisplayDetectiveServiceTests)}.json")
            .Build();

        var monitorMock = new Mock<IDisplayMonitorService>();
        var runnerMock = new Mock<ICommandRunnerService>();

        var detectiveService = new DisplayDetectiveService(
            logger.Object,
            configuration,
            monitorMock.Object,
            runnerMock.Object);

        var source = new CancellationTokenSource();
        var token = source.Token;

        var detectiveTask = detectiveService.RunAsync(token);

        monitorMock.VerifyAdd(m => m.OnDisplayCreated += It.IsAny<EventHandler<IDisplay>>(), Times.Once);
        monitorMock.VerifyAdd(m => m.OnDisplayDeleted += It.IsAny<EventHandler<IDisplay>>(), Times.Once);
        monitorMock.VerifyNoOtherCalls();
        monitorMock.Raise(m => m.OnDisplayCreated += null, this, TestDisplay);

        runnerMock.Verify(m => m.Run(
            It.Is<string>(s => !string.IsNullOrWhiteSpace(s)),
            It.Is<string[]>(x => x != null && x.Length > 0),
            token), Times.Once);
        runnerMock.VerifyNoOtherCalls();

        source.Cancel();
        await detectiveTask;
    }
}