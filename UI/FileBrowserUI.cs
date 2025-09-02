using Terminal.Gui;
using EmuDiskExplorer.Services;

namespace EmuDiskExplorer.UI;

public class FileBrowserUI
{
    private readonly IFileBrowserService _fileBrowserService;
    private readonly IEmulatorApiService _emulatorApi;
    private ListView? _listView;
    private Label? _pathLabel;

    public FileBrowserUI(IFileBrowserService fileBrowserService, IEmulatorApiService emulatorApi)
    {
        _fileBrowserService = fileBrowserService;
        _emulatorApi = emulatorApi;
        _fileBrowserService.EntriesChanged += RefreshFileList;
        _fileBrowserService.FileExecuted += FileExecuted;
    }

    public void Run()
    {
        Application.Init();
        Toplevel top = Application.Top;

        CreateMenuBar(top);
        CreateMainWindow(top);
        CreateStatusBar(top);

        SetupEventHandlers();

        Application.Run();
        Application.Shutdown();
    }

    private static void CreateMenuBar(Toplevel top)
    {
        var menu = new MenuBar([
            new MenuBarItem("_File", [
                new MenuItem("_Quit", "", () => Application.RequestStop())
            ]),
            new MenuBarItem("_Help", [
                new MenuItem("_About", "", () => MessageBox.Query("About", "EmuDiskExplorer v1.0\nFile browser using Terminal.Gui", "Ok"))
            ])
        ]);
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

        _pathLabel = new Label(_fileBrowserService.CurrentParent.FullName)
        {
            X = 1,
            Y = 0,
            Width = Dim.Fill() - 2
        };
        win.Add(_pathLabel);

        ICollection<FileSystemInfo> entries = _fileBrowserService.GetEntries();
        ListDataSource dataSource = new(entries);
        _listView = new ListView(dataSource)
        {
            X = 1,
            Y = 2,
            Width = Dim.Fill() - 2,
            Height = Dim.Fill() - 4
        };
        win.Add(_listView);
    }

    private static void CreateStatusBar(Toplevel top)
    {
        var statusBar = new StatusBar([
            new(Key.Enter, "~ENTER~ Open/Navigate", null),
            new(Key.Backspace, "~BACKSPACE~ Go Up", null),
            new(Key.F10, "~F10~ Quit", () => Application.RequestStop())
        ]);
        top.Add(statusBar);
    }

    private void SetupEventHandlers()
    {
        if (_listView == null) return;

        _listView.KeyPress += (args) =>
        {
            if (args.KeyEvent.Key == Key.Backspace)
            {
                _fileBrowserService.NavigateUp();
                args.Handled = true;
            }
        };

        _listView.OpenSelectedItem += (args) =>
        {
            if (args.Value is FileSystemInfo selectedEntry)
            {
                _fileBrowserService.ExecuteEntry(selectedEntry);
            }
        };
    }

    private void RefreshFileList(object? sender, FileBrowserEntriesChangedEventArgs e)
    {
        if (_listView == null || _pathLabel == null) return;

        _listView.Source = new ListDataSource(e.Entries);
        _listView.SetNeedsDisplay();

        _pathLabel.Text = e.CurrentParent.FullName;
        _pathLabel.SetNeedsDisplay();
    }

    public async void FileExecuted(object? sender, FileBrowserFileExecutedEventArgs e)
    {
        await _emulatorApi.LoadFloppyDrive(e.ExecutedFile.FullName);
        Application.RequestStop();
    }
}
