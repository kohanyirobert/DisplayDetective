using System.Diagnostics;

using Microsoft.Extensions.Logging;

namespace DisplayDetective.Library.Common;

public sealed class CommandRunnerService : ICommandRunnerService
{
    private readonly ILogger<ICommandRunnerService> _logger;

    public CommandRunnerService(ILogger<ICommandRunnerService> logger)
    {
        _logger = logger;
    }

    public Process Run(
        string commandFileName,
        IEnumerable<string> commandArguments,
        CancellationToken cancellationToken)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo(commandFileName, commandArguments)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                _logger.LogInformation("☝️ {Data}", e.Data);
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                _logger.LogError("❌ {Data}", e.Data);
            }
        };

        process.EnableRaisingEvents = true;
        process.Exited += (sender, args) =>
        {
            _logger.LogDebug("✅ Process exited with code {ExitCode}", process.ExitCode);
        };

        cancellationToken.Register(() =>
        {
            if (!process.HasExited)
            {
                process.Kill();
            }
        });

        if (!process.Start())
        {
            throw new InvalidOperationException("Failed to start process.");
        }
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        return process;
    }
}