using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace EmuDiskExplorer.Services;

public class FileBrowserService : IFileBrowserService
{
    private readonly Regex _supportedImageTypesRegex;

    public DirectoryInfo CurrentParent { get; private set; }
    public event EventHandler<FileBrowserEntriesChangedEventArgs>? EntriesChanged;
    public event EventHandler<FileBrowserFileExecutedEventArgs>? FileExecuted;

    public FileBrowserService(IConfiguration configuration)
    {
        string lastFolderPath = configuration[ConfigKeys.AppLastFolderPath] ?? AppContext.BaseDirectory;
        string supportedImageTypes = configuration[ConfigKeys.AppDiskImageTypes] ?? "*.*";
        _supportedImageTypesRegex = CreateRegexFromExtensions(supportedImageTypes);
        CurrentParent = new DirectoryInfo(lastFolderPath);
    }

    private static Regex CreateRegexFromExtensions(string extensionsConfig)
    {
        // Split extensions by comma and remove duplicates
        string[] extensions = [.. extensionsConfig.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(ext => ext.Trim())
            .Distinct()];

        // Convert wildcard patterns to regex patterns
        IEnumerable<string> regexPatterns = extensions.Select(ConvertWildcardToRegex);
        
        // Combine all patterns with OR (|)
        string combinedPattern = $@"\.(?:{string.Join("|", regexPatterns)})$";
        
        return new Regex(combinedPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    private static string ConvertWildcardToRegex(string wildcardPattern)
    {
        // Escape special regex characters except ? and *
        string escaped = Regex.Escape(wildcardPattern);
        
        // Convert wildcards: ? becomes \w, * becomes \w*
        escaped = escaped.Replace(@"\?", @"\w")
                        .Replace(@"\*", @"\w*");
        
        return escaped;
    }

    public ICollection<FileSystemInfo> GetEntries()
    {
        var entries = new List<FileSystemInfo>();

        // Add parent directory entry if not at root
        if (Directory.GetParent(CurrentParent.FullName) != null)
        {
            entries.Add(new DirectoryNavigateUpInfo(".."));
        }

        // Add directories
        IOrderedEnumerable<DirectoryInfo> directories = Directory.GetDirectories(CurrentParent.FullName)
            .Where(d => (new DirectoryInfo(d).Attributes & FileAttributes.Hidden) == 0)
            .Select(d => new DirectoryInfo(d))
            .OrderBy(d => d.Name);
        entries.AddRange(directories);

        // Add files using regex filtering
        IOrderedEnumerable<FileInfo> files = Directory.GetFiles(CurrentParent.FullName)
            .Where(f => (new FileInfo(f).Attributes & FileAttributes.Hidden) == 0)
            .Select(f => new FileInfo(f))
            .Where(f => _supportedImageTypesRegex.IsMatch(f.Name))
            .OrderBy(f => f.Name);
        entries.AddRange(files);

        EntriesChanged?.Invoke(this, new FileBrowserEntriesChangedEventArgs(entries, CurrentParent));
        return entries;
    }

    public bool ExecuteEntry(FileSystemInfo entry) => entry switch
    {
        DirectoryNavigateUpInfo => NavigateUp(),
        DirectoryInfo => NavigateToPath((DirectoryInfo)entry),
        FileInfo => ExecuteFile((FileInfo)entry),
        _ => false,
    };

    public bool NavigateToPath(DirectoryInfo entry)
    {
        if (Directory.Exists(entry.FullName))
        {
            CurrentParent = entry;
            GetEntries();
            return true;
        }
        return false;
    }

    public bool NavigateUp()
    {
        DirectoryInfo? parent = Directory.GetParent(CurrentParent.FullName);
        if (parent != null)
        {
            CurrentParent = parent;
            GetEntries();
            return true;
        }
        return false;
    }

    public bool ExecuteFile(FileInfo entry)
    {
        FileExecuted?.Invoke(this, new FileBrowserFileExecutedEventArgs(entry, entry.Exists));
        return true;
    }
}
