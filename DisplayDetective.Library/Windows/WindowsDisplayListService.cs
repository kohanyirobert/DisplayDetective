using System.Management;
using System.Runtime.Versioning;

using DisplayDetective.Library.Common;

using Microsoft.Extensions.Logging;

namespace DisplayDetective.Library.Windows;

[SupportedOSPlatform("windows")]
public sealed class WindowsDisplayListService : IDisplayListService
{
    private static readonly ManagementScope Scope = new(ManagementPath.DefaultPath);

    private static readonly ObjectQuery Query = new("SELECT * FROM Win32_PnPEntity WHERE PNPClass = 'Monitor'");

    private readonly ILogger<WindowsDisplayListService> _logger;

    internal WindowsDisplayListService(ILogger<WindowsDisplayListService> logger)
    {
        _logger = logger;
    }

    public void ListDisplays()
    {
        _logger.LogInformation("🔍 Scanning displays");
        using var searcher = new ManagementObjectSearcher(Scope, Query);
        IList<IDisplay> displays = [.. searcher.Get().Cast<ManagementObject>().Select(Display.Create)];
        _logger.LogInformation("✨ Found {count} display(s)", displays.Count);
        for (int i = 0; i < displays.Count; i++)
        {
            var display = displays[i];
            _logger.LogInformation("\n📺 Display {i}\n{display}", i + 1, display.ToMultilineString());
        }
    }
}