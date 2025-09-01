namespace EmuDiskExplorer.Services;

public class FileBrowserEntriesChangedEventArgs(ICollection<FileSystemInfo> entries, DirectoryInfo currentParent) : EventArgs
{
    public DirectoryInfo CurrentParent { get; } = currentParent;
    public ICollection<FileSystemInfo> Entries { get; } = entries;
}
