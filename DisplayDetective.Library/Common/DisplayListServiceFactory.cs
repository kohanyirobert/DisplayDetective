using DisplayDetective.Library.Windows;

namespace DisplayDetective.Library.Common;

public static class DisplayListServiceFactory
{
    public static IDisplayListService Create()
    {
        if (OperatingSystem.IsWindows())
        {
            return new WindowsDisplayListService();
        }
        throw new PlatformNotSupportedException();
    }
}