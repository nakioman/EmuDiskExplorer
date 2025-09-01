using Microsoft.Extensions.Hosting;

namespace EmuDiskExplorer.Services;

public class ShutdownService(IHostApplicationLifetime lifetime) : IHostedService
{
    private readonly IHostApplicationLifetime _lifetime = lifetime;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _lifetime.ApplicationStopping.Register(OnStopping);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private void OnStopping()
    {
        // Save configuration when application is stopping
        Console.WriteLine("Saving configuration before shutdown...");
        // Add any additional configuration saving logic here
    }
}
