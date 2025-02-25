using System.Diagnostics;
using System.Reflection.Emit;

using DisplayDetective.Library.Common;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DisplayDetective.Library.Tests.Common;

[Trait("Category", "Unit")]
public class DisplayDetectiveServiceTests
{
    private static IEnumerable<TheoryDataRow<string>> GetConfigFiles(string subDirectory)
    {
        var directory = Path.Combine("TestSettings", subDirectory);
        return Directory.GetFiles(directory, "*.json").Select(f => new TheoryDataRow<string>(f));
    }

    public static IEnumerable<TheoryDataRow<string>> GetGoodConfigFiles() => GetConfigFiles("good");
    public static IEnumerable<TheoryDataRow<string>> GetBadConfigFiles() => GetConfigFiles("bad");

    private static IConfiguration GetGoodConfig(string fileName) => new ConfigurationBuilder()
        .AddJsonFile(Path.Combine("TestSettings", "good", fileName + ".json"))
        .Build();

    private static IConfiguration GoodConfiguration => GetGoodConfig("both-commands-with-args");

    private static void VerifyLog<T>(Mock<ILogger<T>> loggerMock, LogLevel level, Times times) where T : class
    {
        loggerMock.Verify(m => m.Log(
            level,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), times);
    }

    private const string DeviceID = "TestDeviceID";
    private static readonly IDisplay TestDisplay = Display.Create(DeviceID, "TestName", "TestManufacturer", "TestDescription");

    [Fact]
    public void Ctor_InvalidConfig_Fails()
    {
        Assert.Throws<InvalidDataException>(() => new DisplayDetectiveService(
            Mock.Of<ILogger<DisplayDetectiveService>>(),
            new ConfigurationBuilder()
                .AddJsonFile(Path.Combine("TestSettings", "invalid.json"))
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
    public void RunAsync_Works_BothCommandAndArgs()
    {
        var loggerMock = new Mock<ILogger<DisplayDetectiveService>>();
        var monitorMock = new Mock<IDisplayMonitorService>();
        var runnerMock = new Mock<ICommandRunnerService>();

        var detectiveService = new DisplayDetectiveService(
            loggerMock.Object,
            GoodConfiguration,
            monitorMock.Object,
            runnerMock.Object);

        var detectiveTask = detectiveService.RunAsync(TestContext.Current.CancellationToken);

        monitorMock.VerifyAdd(m => m.OnDisplayCreated += It.IsAny<EventHandler<IDisplay>>(), Times.Once());
        monitorMock.VerifyAdd(m => m.OnDisplayDeleted += It.IsAny<EventHandler<IDisplay>>(), Times.Once());
        monitorMock.VerifyNoOtherCalls();
        monitorMock.Raise(m => m.OnDisplayCreated += null, this, TestDisplay);
        monitorMock.Raise(m => m.OnDisplayDeleted += null, this, TestDisplay);

        runnerMock.Verify(m => m.Run("test1.exe", new string[] { "argX", "argY" }, TestContext.Current.CancellationToken), Times.Once());
        runnerMock.Verify(m => m.Run("test2.exe", new string[] { "argZ" }, TestContext.Current.CancellationToken), Times.Once());
        runnerMock.VerifyNoOtherCalls();

        VerifyLog(loggerMock, LogLevel.Error, Times.Never());
        VerifyLog(loggerMock, LogLevel.Warning, Times.Never());
    }

    [Fact]
    public void RunAsync_Works_OnlyCreateCommandWithArgs()
    {
        var loggerMock = new Mock<ILogger<DisplayDetectiveService>>();
        var monitorMock = new Mock<IDisplayMonitorService>();
        var runnerMock = new Mock<ICommandRunnerService>();

        var detectiveService = new DisplayDetectiveService(
            loggerMock.Object,
            GetGoodConfig("only-create-command-with-args"),
            monitorMock.Object,
            runnerMock.Object);

        var detectiveTask = detectiveService.RunAsync(TestContext.Current.CancellationToken);

        monitorMock.VerifyAdd(m => m.OnDisplayCreated += It.IsAny<EventHandler<IDisplay>>(), Times.Once());
        monitorMock.VerifyAdd(m => m.OnDisplayDeleted += It.IsAny<EventHandler<IDisplay>>(), Times.Once());
        monitorMock.VerifyNoOtherCalls();
        monitorMock.Raise(m => m.OnDisplayCreated += null, this, TestDisplay);
        monitorMock.Raise(m => m.OnDisplayDeleted += null, this, TestDisplay);

        runnerMock.Verify(m => m.Run("test1.exe", new string[] { "argX", "argY" }, TestContext.Current.CancellationToken), Times.Once());
        runnerMock.VerifyNoOtherCalls();

        VerifyLog(loggerMock, LogLevel.Error, Times.Never());
        VerifyLog(loggerMock, LogLevel.Warning, Times.Never());
    }

    [Fact]
    public void RunAsync_AfterCancelling_NoMoreEventsFire()
    {
        var loggerMock = new Mock<ILogger<DisplayDetectiveService>>();
        var monitorMock = new Mock<IDisplayMonitorService>();
        var runnerMock = new Mock<ICommandRunnerService>();

        var detectiveService = new DisplayDetectiveService(
            loggerMock.Object,
            GoodConfiguration,
            monitorMock.Object,
            runnerMock.Object);

        var source = new CancellationTokenSource();

        var detectiveTask = detectiveService.RunAsync(source.Token);

        monitorMock.VerifyAdd(m => m.OnDisplayCreated += It.IsAny<EventHandler<IDisplay>>(), Times.Once());
        monitorMock.VerifyAdd(m => m.OnDisplayDeleted += It.IsAny<EventHandler<IDisplay>>(), Times.Once());
        monitorMock.VerifyNoOtherCalls();

        source.Cancel();

        monitorMock.Raise(m => m.OnDisplayCreated += null, this, TestDisplay);
        monitorMock.Raise(m => m.OnDisplayDeleted += null, this, TestDisplay);

        runnerMock.VerifyNoOtherCalls();

        VerifyLog(loggerMock, LogLevel.Error, Times.Never());
        VerifyLog(loggerMock, LogLevel.Warning, Times.Never());
    }

    [Fact]
    public void Dispose_Should_Unsubscribe()
    {
        var loggerMock = new Mock<ILogger<DisplayDetectiveService>>();
        var monitorMock = new Mock<IDisplayMonitorService>();
        var runnerMock = new Mock<ICommandRunnerService>();

        runnerMock.SetupSequence(m => m.Run(
            It.IsAny<string>(),
            It.IsAny<IList<string>>(),
            TestContext.Current.CancellationToken))
            .Returns(Mock.Of<Process>())
            .Returns(Mock.Of<Process>());

        var detectiveService = new DisplayDetectiveService(
            loggerMock.Object,
            GoodConfiguration,
            monitorMock.Object,
            runnerMock.Object);

        var detectiveTask = detectiveService.RunAsync(TestContext.Current.CancellationToken);

        monitorMock.VerifyAdd(m => m.OnDisplayCreated += It.IsAny<EventHandler<IDisplay>>(), Times.Once());
        monitorMock.VerifyAdd(m => m.OnDisplayDeleted += It.IsAny<EventHandler<IDisplay>>(), Times.Once());

        detectiveService.Dispose();

        monitorMock.VerifyRemove(m => m.OnDisplayCreated -= It.IsAny<EventHandler<IDisplay>>(), Times.Once());
        monitorMock.VerifyRemove(m => m.OnDisplayDeleted -= It.IsAny<EventHandler<IDisplay>>(), Times.Once());
        monitorMock.VerifyNoOtherCalls();
    }
}