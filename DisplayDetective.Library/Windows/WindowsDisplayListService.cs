using System.Management;
using System.Runtime.Versioning;

using DisplayDetective.Library.Common;

namespace DisplayDetective.Library.Windows;

[SupportedOSPlatform("windows")]
internal sealed class WindowsDisplayListService : IDisplayListService
{
    private static readonly ManagementScope Scope = new(ManagementPath.DefaultPath);

    private static readonly ObjectQuery Query = new("SELECT * FROM Win32_PnPEntity WHERE PNPClass = 'Monitor'");

    public IList<IDisplay> GetDisplays()
    {
        using var searcher = new ManagementObjectSearcher(Scope, Query);
        return [.. searcher.Get().Cast<ManagementObject>().Select(Display.Create)];
    }
}