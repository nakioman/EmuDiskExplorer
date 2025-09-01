namespace EmuDiskExplorer.Services;

public class DirectoryNavigateUpInfo(string name) : FileSystemInfo
{
    public override bool Exists => false;

    public override string Name => name;

    public override void Delete() { }
}
