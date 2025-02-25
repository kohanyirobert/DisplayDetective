using System.Runtime.Versioning;

using DisplayDetective.Library.Windows;

namespace DisplayDetective.Library.Common;

public sealed class DisplayMonitorServiceFactory
{
    [SupportedOSPlatformGuard("windows")]
    private readonly bool _isWindows;

    public DisplayMonitorServiceFactory(bool isWindows)
    {
        _isWindows = isWindows;
    }

    public IDisplayMonitorService Create()
    {
        if (_isWindows)
        {
            return new WindowsDisplayMonitorService();
        }
        throw new PlatformNotSupportedException();
    }
}