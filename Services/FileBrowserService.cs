namespace EmuDiskExplorer.Services;

public class FileBrowserService : IFileBrowserService
{
    private readonly IConfigurationService _configurationService;
    private string _currentPath;

    public FileBrowserService(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
        _currentPath = _configurationService.GetLastFolder();
    }

    public string CurrentPath => _currentPath;

    public List<string> GetDirectoryEntries()
    {
        try
        {
            var entries = new List<string>();
            
            // Add parent directory entry if not at root
            if (Directory.GetParent(_currentPath) != null)
            {
                entries.Add("..");
            }

            // Add directories
            var directories = Directory.GetDirectories(_currentPath)
                .Where(d => (new DirectoryInfo(d).Attributes & FileAttributes.Hidden) == 0)
                .Select(d => $"📁 {Path.GetFileName(d)}")
                .OrderBy(d => d);
            entries.AddRange(directories);

            // Add files
            var files = Directory.GetFiles(_currentPath)
                .Where(f => (new FileInfo(f).Attributes & FileAttributes.Hidden) == 0)
                .Select(f => $"📄 {Path.GetFileName(f)}")
                .OrderBy(f => f);
            entries.AddRange(files);

            return entries;
        }
        catch
        {
            return new List<string>();
        }
    }

    public bool NavigateToPath(string path)
    {
        if (Directory.Exists(path))
        {
            _currentPath = path;
            _configurationService.SaveLastFolder(_currentPath);
            return true;
        }
        return false;
    }

    public bool NavigateUp()
    {
        var parent = Directory.GetParent(_currentPath);
        if (parent != null)
        {
            _currentPath = parent.FullName;
            return true;
        }
        return false;
    }

    public void ExecuteFile(string filePath)
    {
        // Currently disabled - just save the current path
        _configurationService.SaveLastFolder(_currentPath);
    }
}
