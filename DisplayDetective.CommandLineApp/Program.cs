using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;

using DisplayDetective.Library.Common;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var listCmd = new Command("list");
listCmd.SetHandler(static (context) =>
{
    var services = context.GetHost().Services;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var service = services.GetRequiredService<IDisplayListService>();
    logger.LogInformation("🔍 Scanning displays");
    var displays = service.GetDisplays();
    logger.LogInformation("✨ Found {count} display(s)", displays.Count);
    for (int i = 0; i < displays.Count; i++)
    {
        var display = displays[i];
        logger.LogInformation("\n📺 Display {i}\n{display}", i + 1, display.ToMultilineString());
    }
});

var monitorCmd = new Command("monitor");
monitorCmd.SetHandler(async context =>
{
    var token = context.GetCancellationToken();
    var services = context.GetHost().Services;
    var service = services.GetRequiredService<IDisplayDetectiveService>();
    await service.RunAsync(token);
});

var rootCmd = new RootCommand();
rootCmd.AddCommand(listCmd);
rootCmd.AddCommand(monitorCmd);
rootCmd.Handler = listCmd.Handler;

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
        builder.ConfigureServices(services =>
        {
            services.AddTransient((_) => DisplayListServiceFactory.Create());
            services.AddTransient((_) => DisplayMonitorServiceFactory.Create());
            services.AddTransient<ICommandRunnerService, CommandRunnerService>();
            services.AddTransient<IDisplayDetectiveService, DisplayDetectiveService>();
        });
    })
    .Build();

await parser.InvokeAsync(args);