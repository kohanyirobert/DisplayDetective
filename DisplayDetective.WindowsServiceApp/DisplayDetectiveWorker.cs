namespace DisplayDetective.WindowsServiceApp;

public sealed class DisplayDetectiveWorker(ILogger<DisplayDetectiveWorker> logger, IDisplayDetectiveService service) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        try
        {
            logger.LogInformation("🚀 Starting Windows service");
            await service.RunAsync(token);
            logger.LogInformation("🏁 Stopping Windows service");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "⁉️ Stopping Windows service due to failure");
            Environment.Exit(1);
        }
    }
}