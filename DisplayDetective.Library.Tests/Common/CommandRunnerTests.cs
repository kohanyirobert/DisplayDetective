using System.Diagnostics;

using DisplayDetective.Library.Common;

using Microsoft.Extensions.Logging;

namespace DisplayDetective.Library.Tests.Common;

[Trait("Category", "Unit")]
public class CommandRunnerTests
{
    private const string SleepExe = "DisplayDetective.TestSleep.exe";

    [Fact]
    public void RunAsync_ShortLivedProcess_Works()
    {
        var service = new CommandRunnerService(Mock.Of<ILogger<ICommandRunnerService>>());

        using var process = service.Run("cmd.exe", ["/c", "echo", "START"], TestContext.Current.CancellationToken);
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

        using var process = service.Run("cmd.exe", ["/c", SleepExe, "2"], TestContext.Current.CancellationToken);
        Assert.NotNull(process);
        Assert.False(process.HasExited);

        var exited = process.WaitForExit(1000);
        Assert.False(exited);

        exited = process.WaitForExit(1500);
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

        using var process = service.Run("cmd.exe", ["/c", SleepExe, "3"], token);
        Assert.NotNull(process);
        Assert.False(process.HasExited);

        var exited = process.WaitForExit(1000);
        Assert.False(exited);

        source.Cancel();
        Assert.True(process.HasExited);
        Assert.NotEqual(0, process.ExitCode);
    }
}