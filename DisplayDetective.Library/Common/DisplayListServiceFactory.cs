using System.Runtime.Versioning;

using DisplayDetective.Library.Windows;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DisplayDetective.Library.Common;

public sealed class DisplayListServiceFactory
{
    private readonly IServiceProvider _provider;

    [SupportedOSPlatformGuard("windows")]
    private readonly bool _isWindows;

    public DisplayListServiceFactory(IServiceProvider provider, bool isWindows)
    {
        _provider = provider;
        _isWindows = isWindows;
    }

    public IDisplayListService Create()
    {
        if (_isWindows)
        {
            return new WindowsDisplayListService(_provider.GetRequiredService<ILogger<WindowsDisplayListService>>());
        }
        throw new PlatformNotSupportedException();
    }
}