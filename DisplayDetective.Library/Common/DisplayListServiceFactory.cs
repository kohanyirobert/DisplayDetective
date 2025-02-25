using System.Runtime.Versioning;

using DisplayDetective.Library.Windows;

namespace DisplayDetective.Library.Common;

public sealed class DisplayListServiceFactory
{
    [SupportedOSPlatformGuard("windows")]
    private readonly bool _isWindows;

    public DisplayListServiceFactory(bool isWindows)
    {
        _isWindows = isWindows;
    }

    public IDisplayListService Create()
    {
        if (_isWindows)
        {
            return new WindowsDisplayListService();
        }
        throw new PlatformNotSupportedException();
    }
}