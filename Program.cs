using EmuDiskExplorer.Services;
using EmuDiskExplorer.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EmuDiskExplorer;

class Program
{
    static void Main(string[] args)
    {
        IHost host = CreateHostBuilder(args).Build();

        // Register shutdown handler
        IHostApplicationLifetime lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

        lifetime.ApplicationStopping.Register(() =>
        {
            IConfigurationService configService = host.Services.GetRequiredService<IConfigurationService>();
            configService.SaveLastFolder();
        });

        FileBrowserUI fileBrowserUI = host.Services.GetRequiredService<FileBrowserUI>();
        fileBrowserUI.Run();

        host.StopAsync().Wait();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                // Clear default configuration sources if needed
                config.Sources.Clear();

                // Add our INI configuration
                config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                      .AddIniFile("config.ini", optional: true, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                // Register services
                services.AddSingleton<IConfigurationService, ConfigurationService>();
                services.AddSingleton<IFileBrowserService, FileBrowserService>();

                services.AddLogging(builder =>
                {
                    builder.AddFilter("System.Net.Http.HttpClient", LogLevel.None);
                });

                services.AddHttpClient<IEmulatorApiService, EmulatorApiService>((serviceProvider, client) =>
                {
                    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
                    string baseAddress = configuration[ConfigKeys.EmulatorBaseUrl] ?? "http://localhost:8090/";

                    client.BaseAddress = new Uri(baseAddress);
                });

                // Register UI
                services.AddTransient<FileBrowserUI>();
            });
}
