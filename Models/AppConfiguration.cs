namespace EmuDiskExplorer.Models;

public class AppConfiguration
{
    public AppSection App { get; set; } = new();
}

public class AppSection
{
    public string LastFolder { get; set; } = string.Empty;
}