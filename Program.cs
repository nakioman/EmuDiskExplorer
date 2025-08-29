using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using EmuDiskExplorer.Services;
using EmuDiskExplorer.UI;
using EmuDiskExplorer.Models;

namespace EmuDiskExplorer;

class Program
{
    static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        
        var fileBrowserUI = host.Services.GetRequiredService<FileBrowserUI>();
        fileBrowserUI.Run();
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
                // Configure strongly typed configuration
                services.Configure<AppConfiguration>(context.Configuration);
                
                // Register services
                services.AddSingleton<IConfigurationService, ConfigurationService>();
                services.AddSingleton<IFileBrowserService, FileBrowserService>();
                
                // Register UI
                services.AddTransient<FileBrowserUI>();
            });
}
