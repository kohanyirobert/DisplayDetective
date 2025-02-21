using System.Diagnostics;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DisplayDetective.Library.Common;

public class DisplayDetectiveService : IDisplayDetectiveService, IDisposable
{
    private readonly ILogger<IDisplayDetectiveService> _logger;
    private readonly IDisplayMonitorService _monitorService;
    private readonly ICommandRunnerService _runnerService;
    private readonly string? _deviceID;
    private readonly string? _createCommandFileName;
    private readonly IList<string> _createCommandArguments;
    private readonly string? _deleteCommandFileName;
    private readonly IList<string> _deleteCommandArguments;
    private CancellationToken _token;
    private Process? _createProcess;
    private Process? _deleteProcess;

    public DisplayDetectiveService(
        ILogger<IDisplayDetectiveService> logger,
        IConfiguration configuration,
        IDisplayMonitorService monitorService,
        ICommandRunnerService runnerService)
    {
        _logger = logger;
        _monitorService = monitorService;
        _runnerService = runnerService;

        var watchesSection = configuration.GetRequiredSection("DisplayDetective:Watches");
        var watches = watchesSection.GetChildren().ToList();
        if (watches.Count == 0) throw new InvalidOperationException($"No watches configured");
        else if (watches.Count > 1) throw new InvalidOperationException($"Multiple watches configured");

        var deviceSection = watches[0];
        _deviceID = deviceSection["DeviceID"];
        if (string.IsNullOrWhiteSpace(_deviceID))
        {
            throw new InvalidOperationException($"Device ID is unset or empty ({_deviceID})");
        }

        _createCommandFileName = deviceSection["CreateCommand"];
        _createCommandArguments = deviceSection.GetSection("CreateArguments").Get<string[]>() ?? [];
        _deleteCommandFileName = deviceSection["DeleteCommand"];
        _deleteCommandArguments = deviceSection.GetSection("DeleteArguments").Get<string[]>() ?? [];

        if (_createCommandFileName == null && _deleteCommandFileName == null)
        {
            throw new InvalidOperationException("CreateCommand and DeleteCommand are both missing");
        }
        else if (_createCommandFileName != null && _createCommandFileName.All(char.IsWhiteSpace))
        {
            throw new InvalidOperationException("CreateCommand is empty");
        }
        else if (_deleteCommandFileName != null && _deleteCommandFileName.All(char.IsWhiteSpace))
        {
            throw new InvalidOperationException("DeleteCommand is empty");
        }
        else if (_createCommandFileName == null && _createCommandArguments.Count > 0)
        {
            throw new InvalidOperationException("CreateArguments is missing");
        }
        else if (_deleteCommandFileName == null && _deleteCommandArguments.Count > 0)
        {
            throw new InvalidOperationException("DeleteArguments is missing");
        }
    }

    public async Task RunAsync(CancellationToken token)
    {
        _token = token;
        _logger.LogInformation("👀 Monitoring display: {deviceID}", _deviceID);
        _monitorService.OnDisplayCreated += OnDisplayCreated;
        _monitorService.OnDisplayDeleted += OnDisplayDeleted;
        for (int i = 0; ; i++)
        {
            if (_token.IsCancellationRequested)
            {
                _logger.LogDebug("🛑 Cancellation requested at iteration {i}", i);
                break;
            }
            try
            {
                _logger.LogDebug("⌛ Keeping service alive until cancellation, currently at iteration {i}", i);
                await Task.Delay(1000, _token);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogDebug("☝️ Monitoring cancelled: {ex}", ex);
                return;
            }
        }
    }

    private void OnDisplayCreated(object? sender, IDisplay display)
    {
        if (_token.IsCancellationRequested) return;
        HandleDisplayCreatedOrDeletedAsync(display, true);
    }

    private void OnDisplayDeleted(object? sender, IDisplay display)
    {
        if (_token.IsCancellationRequested) return;
        HandleDisplayCreatedOrDeletedAsync(display, false);
    }

    private void HandleDisplayCreatedOrDeletedAsync(IDisplay display, bool created)
    {
        {
            var emoji = created ? "✨" : "🔥";
            var label = created ? "connected" : "disconnected";
            _logger.LogInformation("{emoji} Display {label}: {DeviceID}", emoji, label, display.DeviceID);
        }
        if (_deviceID == display.DeviceID)
        {
            _logger.LogDebug("☝️ Matched monitored device ID ({_deviceID}), running command", _deviceID);
            var emoji = created ? "👟" : "🛑";
            var label = created ? "create" : "delete";
            var command = created ? _createCommandFileName : _deleteCommandFileName;
            var arguments = created ? _createCommandArguments : _deleteCommandArguments;
            _logger.LogInformation("{emoji} Running {type} command: {cmd}", emoji, label, command);
            try
            {
                if (string.IsNullOrEmpty(command))
                {
                    _logger.LogInformation(
                        "ℹ️ Empty {label} command, doing nothing (arguments were {arguments})",
                        label,
                        arguments.Count == 0
                            ? "none"
                            : string.Join(' ', arguments));
                }
                else
                {
                    var process = _runnerService.Run(command, arguments, _token);
                    if (created) _createProcess = process;
                    else _deleteProcess = process;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while trying to run the command: {command}", command);
            }
        }
        else
        {
            _logger.LogDebug("💤 Not matching monitored device ID ({_deviceID}), doing nothing", _deviceID);
        }
    }

    public void Dispose()
    {
        _monitorService.OnDisplayCreated -= OnDisplayCreated;
        _monitorService.OnDisplayDeleted -= OnDisplayDeleted;
        if (_createProcess != null && !_createProcess.HasExited)
        {
            _createProcess.Dispose();
            _createProcess = null;
        }
        if (_deleteProcess != null && !_deleteProcess.HasExited)
        {
            _deleteProcess.Dispose();
            _deleteProcess = null;
        }
    }
}