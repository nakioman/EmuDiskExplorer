namespace EmuDiskExplorer.Services
{
    public interface IEmulatorApiService
    {
        Task LoadFloppyDrive(string filePath, CancellationToken cancellationToken = default);
        Task EjectFloppyDrive(CancellationToken cancellationToken = default);
    }

}
