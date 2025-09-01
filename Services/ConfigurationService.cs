using Microsoft.Extensions.Configuration;

namespace EmuDiskExplorer.Services;

public class ConfigurationService(IConfiguration configuration, IFileBrowserService fileBrowserService) : IConfigurationService
{
    private readonly string _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
    private readonly IConfiguration _configuration = configuration;
    private readonly IFileBrowserService _fileBrowserService = fileBrowserService;

    public void SaveLastFolder()
    {
        if (_fileBrowserService.CurrentParent == null)
        {
            return;
        }

        string[] lastFolderConfigParts = ConfigKeys.AppLastFolderPath.Split(':');
        if (lastFolderConfigParts.Length != 2)
        {
            throw new InvalidOperationException();
        }

        UpdateIniFile(lastFolderConfigParts[0], lastFolderConfigParts[1], _fileBrowserService.CurrentParent.FullName);
    }

    private void UpdateIniFile(string section, string key, string value)
    {
        var lines = new List<string>();
        bool sectionFound = false;
        bool keyUpdated = false;
        string currentSection = "";

        // Read existing file if it exists
        if (File.Exists(_configPath))
        {
            lines.AddRange(File.ReadAllLines(_configPath));
        }

        // Process existing lines
        for (int i = 0; i < lines.Count; i++)
        {
            string line = lines[i].Trim();
            
            // Check if this is a section header
            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                currentSection = line.Substring(1, line.Length - 2);
                sectionFound = currentSection.Equals(section, StringComparison.OrdinalIgnoreCase);
            }
            // Check if this is the key we want to update in the correct section
            else if (sectionFound && line.StartsWith($"{key}=", StringComparison.OrdinalIgnoreCase))
            {
                lines[i] = $"{key}={value}";
                keyUpdated = true;
                sectionFound = false; // Reset to avoid updating the same key in other sections
            }
        }

        // If section doesn't exist, add it
        if (!lines.Any(l => l.Trim().Equals($"[{section}]", StringComparison.OrdinalIgnoreCase)))
        {
            if (lines.Count > 0 && !string.IsNullOrWhiteSpace(lines.Last()))
            {
                lines.Add(""); // Add empty line before new section if file doesn't end with empty line
            }
            lines.Add($"[{section}]");
            lines.Add($"{key}={value}");
            keyUpdated = true;
        }
        // If section exists but key doesn't, add the key to the section
        else if (!keyUpdated)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i].Trim();
                if (line.Equals($"[{section}]", StringComparison.OrdinalIgnoreCase))
                {
                    // Find the end of this section (next section or end of file)
                    int insertIndex = i + 1;
                    for (int j = i + 1; j < lines.Count; j++)
                    {
                        if (lines[j].Trim().StartsWith('[') && lines[j].Trim().EndsWith(']'))
                        {
                            insertIndex = j;
                            break;
                        }
                        insertIndex = j + 1;
                    }
                    lines.Insert(insertIndex, $"{key}={value}");
                    break;
                }
            }
        }

        // Write back to file
        File.WriteAllLines(_configPath, lines);
    }
}
