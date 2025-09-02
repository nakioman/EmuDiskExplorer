using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace EmuDiskExplorer.Services
{
    public class EmulatorApiService(HttpClient httpClient, IConfiguration configuration) : IEmulatorApiService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly int _floppyDrive = int.Parse(configuration[ConfigKeys.EmulatorFloppyDrive] ?? "0");

        public async Task LoadFloppyDrive(string filePath, CancellationToken cancellationToken = default)
        {
            LoadFloppyRequest request = new()
            {
                FLoppyDrive = _floppyDrive,
                FilePath = filePath,
            };

            await _httpClient.PostAsJsonAsync("api/load_floppy", request, LoadFloppyRequestSerializerContext.Default.LoadFloppyRequest, cancellationToken);
        }

        public async Task EjectFloppyDrive(CancellationToken cancellationToken = default)
        {
            LoadFloppyRequest request = new()
            {
                FLoppyDrive = _floppyDrive,
            };

            await _httpClient.PostAsJsonAsync("api/load_floppy", request, LoadFloppyRequestSerializerContext.Default.LoadFloppyRequest, cancellationToken);
        }
    }

    public class LoadFloppyRequest
    {
        [JsonPropertyName("drive")]
        public int FLoppyDrive { get; set; }

        [JsonPropertyName("file_path")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? FilePath { get; set; }
    }

    // 2. Create a JsonSerializerContext for source generation
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(LoadFloppyRequest))]
    public partial class LoadFloppyRequestSerializerContext : JsonSerializerContext
    {

    }

}
