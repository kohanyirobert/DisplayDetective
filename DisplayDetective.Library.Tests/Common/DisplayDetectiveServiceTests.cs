using DisplayDetective.Library.Common;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DisplayDetective.Library.Tests.Common;

[Trait("Category", "Unit")]
public class DisplayDetectiveServiceTests
{
    private static IEnumerable<TheoryDataRow<string>> GetConfigFiles(string subDirectory) {
        var directory = Path.Combine("testsettings", subDirectory);
        return Directory.GetFiles(directory, "*.json").Select(f => new TheoryDataRow<string>(f));
    }

    public static IEnumerable<TheoryDataRow<string>> GetGoodConfigFiles() => GetConfigFiles("good");
    public static IEnumerable<TheoryDataRow<string>> GetBadConfigFiles() => GetConfigFiles("bad");

    private const string DeviceID = "TestDeviceID";
    private static readonly IDisplay TestDisplay = Display.Create(DeviceID, "TestName", "TestManufacturer", "TestDescription");

    private static readonly IConfiguration Configuration = new ConfigurationBuilder()
        .AddJsonFile(Path.Combine("testsettings", "good", "both-commands-with-args.json"))
        .Build();

    [Fact]
    public void Ctor_InvalidConfig_Fails()
    {
        Assert.Throws<InvalidDataException>(() => new DisplayDetectiveService(
            Mock.Of<ILogger<DisplayDetectiveService>>(),
            new ConfigurationBuilder()
                .AddJsonFile(Path.Combine("testsettings", "invalid.json"))
                .Build(),
            Mock.Of<IDisplayMonitorService>(),
            Mock.Of<ICommandRunnerService>()));
    }

    [Theory]
    [MemberData(nameof(GetBadConfigFiles))]
    public void Ctor_BadConfig_Fails(string file)
    {
        Assert.Throws<InvalidOperationException>(() => new DisplayDetectiveService(
            Mock.Of<ILogger<DisplayDetectiveService>>(),
            new ConfigurationBuilder()
                .AddJsonFile(file)
                .Build(),
            Mock.Of<IDisplayMonitorService>(),
            Mock.Of<ICommandRunnerService>()));
    }

    [Theory]
    [MemberData(nameof(GetGoodConfigFiles))]
    public void Ctor_GoodConfig_Works(string file)
    {
        Assert.NotNull(new DisplayDetectiveService(
            Mock.Of<ILogger<DisplayDetectiveService>>(),
            new ConfigurationBuilder()
                .AddJsonFile(file)
                .Build(),
            Mock.Of<IDisplayMonitorService>(),
            Mock.Of<ICommandRunnerService>()));
    }

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
        monitorMock.Raise(m => m.OnDisplayDeleted += null, this, TestDisplay);

        runnerMock.Verify(m => m.Run("test1.exe", new string[] { "argX", "argY" }, TestContext.Current.CancellationToken), Times.Once);
        runnerMock.Verify(m => m.Run("test2.exe", new string[] { "argZ" }, TestContext.Current.CancellationToken), Times.Once);
        runnerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void RunAsync_AfterCancelling_NoMoreEventsFire()
    {
        var monitorMock = new Mock<IDisplayMonitorService>();
        var runnerMock = new Mock<ICommandRunnerService>();

        var detectiveService = new DisplayDetectiveService(
            Mock.Of<ILogger<DisplayDetectiveService>>(),
            Configuration,
            monitorMock.Object,
            runnerMock.Object);

        var source = new CancellationTokenSource();

        var detectiveTask = detectiveService.RunAsync(source.Token);

        monitorMock.VerifyAdd(m => m.OnDisplayCreated += It.IsAny<EventHandler<IDisplay>>(), Times.Once);
        monitorMock.VerifyAdd(m => m.OnDisplayDeleted += It.IsAny<EventHandler<IDisplay>>(), Times.Once);
        monitorMock.VerifyNoOtherCalls();

        source.Cancel();

        monitorMock.Raise(m => m.OnDisplayCreated += null, this, TestDisplay);
        monitorMock.Raise(m => m.OnDisplayDeleted += null, this, TestDisplay);

        runnerMock.VerifyNoOtherCalls();
    }
}