using DisplayDetective.Library.Common;
using DisplayDetective.WindowsServiceApp;

using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "DisplayDetective.WindowsServiceApp";
});

LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(builder.Services);

builder.Services.AddTransient((_) =>
{
    var factory = new DisplayMonitorServiceFactory(OperatingSystem.IsWindows());
    return factory.Create();
});
builder.Services.AddTransient<ICommandRunnerService, CommandRunnerService>();
builder.Services.AddTransient<IDisplayDetectiveService, DisplayDetectiveService>();
builder.Services.AddHostedService<DisplayDetectiveWorker>();

var host = builder.Build();
host.Run();