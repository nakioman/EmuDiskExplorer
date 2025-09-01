using System.Diagnostics;

namespace EmuDiskExplorer.Services;

public interface IFileBrowserService
{
    event EventHandler<FileBrowserEntriesChangedEventArgs>? EntriesChanged;
    event EventHandler<FileBrowserFileExecutedEventArgs>? FileExecuted;

    ICollection<FileSystemInfo> GetEntries();
    bool ExecuteEntry(FileSystemInfo entry);
    bool NavigateUp();
    DirectoryInfo CurrentParent { get; }
}
