using System.Diagnostics;

namespace EmuDiskExplorer.Services;

public interface IFileBrowserService
{
    string CurrentPath { get; }
    List<string> GetDirectoryEntries();
    bool NavigateToPath(string path);
    bool NavigateUp();
    void ExecuteFile(string filePath);
}