namespace EmuDiskExplorer.Services;

public class FileBrowserFileExecutedEventArgs(FileInfo executedFile, bool executed) : EventArgs
{
    public FileInfo ExecutedFile { get; } = executedFile;
    public bool Executed { get; } = executed;
}
