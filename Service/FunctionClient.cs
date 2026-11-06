using AgriStream_ST10157545_Practium.Models;

namespace AgriStream_ST10157545_Practium.Service
{
    public interface IFunctionsClient
    {
        Task<IReadOnlyList<TelemetryReading>> GetTelemetryAsync(string fieldId, CancellationToken ct);


        Task<bool> TriggerIrrigationAsync(IrrigationCommand command, CancellationToken ct);
    }


    public class FunctionsClient : IFunctionsClient
    {
        private const string KeyHeader = "x-functions-key";
        private readonly HttpClient _http;


        public FunctionsClient(HttpClient http, IConfiguration configuration)
        {
            _http = http;
            var baseUrl = configuration["Functions:BaseUrl"]
                ?? throw new InvalidOperationException("Functions:BaseUrl is not configured.");
            _http.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");


            var key = configuration["Functions:Key"];
            if (!string.IsNullOrWhiteSpace(key))
            {
                _http.DefaultRequestHeaders.Add(KeyHeader, key);
            }
        }


        public async Task<IReadOnlyList<TelemetryReading>> GetTelemetryAsync(string fieldId, CancellationToken ct)
        {
            var url = $"api/telemetry/{Uri.EscapeDataString(fieldId)}";
            var result = await _http.GetFromJsonAsync<List<TelemetryReading>>(url, ct);
            return result ?? new List<TelemetryReading>();
        }


        public async Task<bool> TriggerIrrigationAsync(IrrigationCommand command, CancellationToken ct)
        {
            var response = await _http.PostAsJsonAsync("api/irrigation", command, ct);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> TriggerIrrigationAsync(string deviceId, string fieldId, string action, CancellationToken ct)
        {
            var command = new IrrigationCommand(deviceId, fieldId, action);
            return await TriggerIrrigationAsync(command, ct);
        }

        public async Task<bool> BlobServiceClient(string containerName, string blobName, Stream content, CancellationToken ct)
        {
            var url = $"api/blobs/{Uri.EscapeDataString(containerName)}/{Uri.EscapeDataString(blobName)}";
            var response = await _http.PutAsync(url, new StreamContent(content), ct);
            return response.IsSuccessStatusCode;
        }

    }


}
