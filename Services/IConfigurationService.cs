using EmuDiskExplorer.Models;

namespace EmuDiskExplorer.Services;

public interface IConfigurationService
{
    string GetLastFolder();
    void SaveLastFolder(string folderPath);
}