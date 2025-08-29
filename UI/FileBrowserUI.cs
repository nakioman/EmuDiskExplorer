using Terminal.Gui;
using EmuDiskExplorer.Services;

namespace EmuDiskExplorer.UI;

public class FileBrowserUI
{
    private readonly IFileBrowserService _fileBrowserService;
    private ListView? _listView;
    private Label? _pathLabel;

    public FileBrowserUI(IFileBrowserService fileBrowserService)
    {
        _fileBrowserService = fileBrowserService;
    }

    public void Run()
    {
        Application.Init();
        var top = Application.Top;

        CreateMenuBar(top);
        CreateMainWindow(top);
        CreateStatusBar(top);

        RefreshFileList();
        SetupEventHandlers();

        Application.Run();
        Application.Shutdown();
    }

    private void CreateMenuBar(Toplevel top)
    {
        var menu = new MenuBar(new MenuBarItem[] {
            new MenuBarItem("_File", new MenuItem[] {
                new MenuItem("_Quit", "", () => Application.RequestStop())
            }),
            new MenuBarItem("_Help", new MenuItem[] {
                new MenuItem("_About", "", () => MessageBox.Query("About", "EmuDiskExplorer v1.0\nFile browser using Terminal.Gui", "Ok"))
            })
        });
        top.Add(menu);
    }

    private void CreateMainWindow(Toplevel top)
    {
        var win = new Window("File Browser")
        {
            X = 0,
            Y = 1,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        top.Add(win);

        _pathLabel = new Label(_fileBrowserService.CurrentPath)
        {
            X = 1,
            Y = 0,
            Width = Dim.Fill() - 2
        };
        win.Add(_pathLabel);

        _listView = new ListView()
        {
            X = 1,
            Y = 2,
            Width = Dim.Fill() - 2,
            Height = Dim.Fill() - 4
        };
        win.Add(_listView);
    }

    private void CreateStatusBar(Toplevel top)
    {
        var statusBar = new StatusBar(new StatusItem[] {
            new StatusItem(Key.Enter, "~ENTER~ Open/Navigate", null),
            new StatusItem(Key.Backspace, "~BACKSPACE~ Go Up", null),
            new StatusItem(Key.F10, "~F10~ Quit", () => Application.RequestStop())
        });
        top.Add(statusBar);
    }

    private void SetupEventHandlers()
    {
        if (_listView == null) return;

        _listView.KeyPress += (args) => {
            if (args.KeyEvent.Key == Key.Enter)
            {
                HandleEnterKey();
                args.Handled = true;
            }
            else if (args.KeyEvent.Key == Key.Backspace)
            {
                NavigateUp();
                args.Handled = true;
            }
        };

        _listView.OpenSelectedItem += (args) => {
            HandleEnterKey();
        };
    }

    private void RefreshFileList()
    {
        try
        {
            var entries = _fileBrowserService.GetDirectoryEntries();
            _listView?.SetSource(entries.ToArray());
            if (_pathLabel != null)
            {
                _pathLabel.Text = _fileBrowserService.CurrentPath;
            }
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Error", $"Cannot access directory: {ex.Message}", "Ok");
        }
    }

    private void HandleEnterKey()
    {
        if (_listView?.Source?.Count == 0) return;
        
        var selectedIndex = _listView?.SelectedItem ?? 0;
        var source = _listView?.Source?.ToList();
        if (source == null || selectedIndex >= source.Count) return;
        
        var selectedItem = source[selectedIndex]?.ToString();
        if (string.IsNullOrEmpty(selectedItem)) return;

        if (selectedItem == "..")
        {
            NavigateUp();
            return;
        }

        var itemName = selectedItem.Substring(2).Trim();
        var fullPath = Path.Combine(_fileBrowserService.CurrentPath, itemName);

        if (Directory.Exists(fullPath))
        {
            if (_fileBrowserService.NavigateToPath(fullPath))
            {
                RefreshFileList();
            }
        }
        else if (File.Exists(fullPath))
        {
            _fileBrowserService.ExecuteFile(fullPath);
        }
    }

    private void NavigateUp()
    {
        if (_fileBrowserService.NavigateUp())
        {
            RefreshFileList();
        }
    }
}