using DisplayDetective.Library.Common;

using Microsoft.Extensions.Logging;

namespace DisplayDetective.Library.Tests.Common;

[Trait("Category", "Acceptance")]
public class CommandRunnerServiceTests
{
    private const string SleepExe = "DisplayDetective.TestSleep.exe";

    [Fact]
    public void RunAsync_ShortLivedProcess_Works()
    {
        var service = new CommandRunnerService(Mock.Of<ILogger<ICommandRunnerService>>());

        using var process = service.Run(SleepExe, [], TestContext.Current.CancellationToken);
        Assert.NotNull(process);
        Assert.False(process.HasExited);

        var exited = process.WaitForExit(100);
        Assert.True(exited);
        Assert.True(process.HasExited);
        Assert.Equal(0, process.ExitCode);
    }

    [Fact]
    public void RunAsync_LongLivedProcess_Works()
    {
        var service = new CommandRunnerService(Mock.Of<ILogger<ICommandRunnerService>>());

        using var process = service.Run(SleepExe, ["500"], TestContext.Current.CancellationToken);
        Assert.NotNull(process);
        Assert.False(process.HasExited);

        var exited = process.WaitForExit(250);
        Assert.False(exited);

        exited = process.WaitForExit(500);
        Assert.True(exited);
        Assert.True(process.HasExited);
        Assert.Equal(0, process.ExitCode);
    }

    [Fact]
    public void RunAsync_LongLivedProcess_CanBeCancelled()
    {
        var service = new CommandRunnerService(Mock.Of<ILogger<ICommandRunnerService>>());
        var source = new CancellationTokenSource();
        var token = source.Token;

        using var process = service.Run(SleepExe, ["500"], token);
        Assert.NotNull(process);
        Assert.False(process.HasExited);

        var exited = process.WaitForExit(250);
        Assert.False(exited);

        source.Cancel();
        Assert.True(process.HasExited);
        Assert.NotEqual(0, process.ExitCode);
    }
}