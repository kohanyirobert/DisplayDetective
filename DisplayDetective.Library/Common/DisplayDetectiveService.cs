using System.Diagnostics;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DisplayDetective.Library.Common;

public class DisplayDetectiveService : IDisplayDetectiveService, IDisposable
{
    private readonly ILogger<IDisplayDetectiveService> _logger;
    private readonly IDisplayMonitorService _monitorService;
    private readonly ICommandRunnerService _runnerService;
    private readonly string _deviceID;
    private readonly string _createCommandFileName;
    private readonly IEnumerable<string> _createCommandArguments;
    private readonly string _deleteCommandFileName;
    private readonly IEnumerable<string> _deleteCommandArguments;
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

        var appSection = configuration.GetRequiredSection("DisplayDetective");
        var watchesSection = appSection.GetRequiredSection("Watches");
        var watches = watchesSection.GetChildren().ToList();
        if (watches.Count == 0) throw new InvalidOperationException($"No watches configured in {watchesSection}");
        else if (watches.Count > 1) throw new InvalidOperationException($"Multiple watches configured in {watchesSection}");

        var deviceSection = watches[0];
        _deviceID = deviceSection.Key;
        if (string.IsNullOrWhiteSpace(_deviceID))
        {
            throw new InvalidOperationException($"Device ID is unset or empty ({_deviceID})");
        }

        _createCommandFileName = deviceSection["CreateCommand"]
            ?? throw new InvalidOperationException($"No CreateCommand configured in {watchesSection.Path}");
        _createCommandArguments = deviceSection.GetRequiredSection("CreateArguments").Get<string[]>()
            ?? throw new InvalidOperationException($"No CreateArguments configured in {watchesSection.Path}");
        _deleteCommandFileName = deviceSection["DeleteCommand"]
            ?? throw new InvalidOperationException($"No CreateCommand configured in {watchesSection.Path}");
        _deleteCommandArguments = deviceSection.GetRequiredSection("DeleteArguments").Get<string[]>()
            ?? throw new InvalidOperationException($"No CreateArguments configured in {watchesSection.Path}");
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
                _logger.LogDebug("⌛ Keeping service alive until cancellation: iteration {i}", i);
                break;
            }
            try
            {
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
        HandleDisplayCreatedOrDeletedAsync(display, true);
    }

    private void OnDisplayDeleted(object? sender, IDisplay display)
    {
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
            var type = created ? "Create" : "Delete";
            var command = created ? _createCommandFileName : _deleteCommandFileName;
            var arguments = created ? _createCommandArguments : _deleteCommandArguments;
            _logger.LogInformation("{emoji} Running {type} command: {cmd}", emoji, label, command);
            try
            {
                var process = _runnerService.Run(command, arguments, _token);
                if (created) _createProcess = process;
                else _deleteProcess = process;
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
        _createProcess?.Dispose();
        _deleteProcess?.Dispose();
    }
}