using System.Collections;
using EmuDiskExplorer.Services;
using Terminal.Gui;

namespace EmuDiskExplorer.UI;

public class ListDataSource(ICollection<FileSystemInfo> items) : IListDataSource
{
    private readonly ICollection<FileSystemInfo> _items = items;
    private int? _marked = null;

    public int Count => _items.Count;

    public int Length => _items.Max(item => item.Name.Length);

    public bool IsMarked(int item) => _marked == item;
    public void Render(ListView container, ConsoleDriver driver, bool selected, int item, int col, int line, int width, int start = 0)
    {
        FileSystemInfo entry = _items.ElementAt(item);
        string displayName = entry is DirectoryInfo || entry is DirectoryNavigateUpInfo ? $"/{entry.Name}" : $"{entry.Name}";
        if (selected)
        {
            driver.AddStr(displayName.PadRight(width));
            driver.SetAttribute(container.GetFocusColor());
        }
        else
        {
            driver.AddStr(displayName.PadRight(width));
        }
    }
    public void SetMark(int item, bool value) { if (value) _marked = item; else _marked = null; }
    public IList ToList() => _items.ToList();
}
