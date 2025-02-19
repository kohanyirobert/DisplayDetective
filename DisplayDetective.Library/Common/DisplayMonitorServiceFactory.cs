using DisplayDetective.Library.Windows;

namespace DisplayDetective.Library.Common;

public static class DisplayMonitorServiceFactory
{
    public static IDisplayMonitorService Create()
    {
        if (OperatingSystem.IsWindows())
        {
            return new WindowsDisplayMonitorService();
        }
        throw new PlatformNotSupportedException();
    }
}