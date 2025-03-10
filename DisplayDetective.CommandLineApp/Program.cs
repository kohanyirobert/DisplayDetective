using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;

using DisplayDetective.Library.Common;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;

var listCmd = new Command("list");
listCmd.SetHandler(static (context) =>
{
    var services = context.GetHost().Services;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var service = services.GetRequiredService<IDisplayListService>();
    service.ListDisplays();
});

var monitorCmd = new Command("monitor");
monitorCmd.SetHandler(static async context =>
{
    var token = context.GetCancellationToken();
    var services = context.GetHost().Services;
    var service = services.GetRequiredService<IDisplayDetectiveService>();
    await service.RunAsync(token);
});

var rootCmd = new RootCommand();
rootCmd.AddCommand(listCmd);
rootCmd.AddCommand(monitorCmd);
rootCmd.Handler = monitorCmd.Handler;

var parser = new CommandLineBuilder(rootCmd)
    .UseDefaults()
    .UseHost(Host.CreateDefaultBuilder, builder =>
    {
        builder.ConfigureAppConfiguration((host, config) =>
        {
            var env = host.HostingEnvironment;
            if (env.IsDevelopment())
            {
                config.AddUserSecrets<Program>();
            }
        });
        builder.ConfigureServices((host, services) =>
        {
            if (OperatingSystem.IsWindows())
            {
                services.Configure<EventLogSettings>(host.Configuration.GetSection("Logging:EventLog"));
            }
            services.AddTransient((provider) =>
            {
                var factory = new DisplayListServiceFactory(provider, OperatingSystem.IsWindows());
                return factory.Create();
            });
            services.AddTransient((_) =>
            {
                var factory = new DisplayMonitorServiceFactory(OperatingSystem.IsWindows());
                return factory.Create();
            });
            services.AddTransient<ICommandRunnerService, CommandRunnerService>();
            services.AddTransient<IDisplayDetectiveService, DisplayDetectiveService>();
        });
    })
    .Build();

await parser.InvokeAsync(args);