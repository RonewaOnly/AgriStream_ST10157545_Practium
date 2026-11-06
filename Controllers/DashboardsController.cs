using AgriStream_ST10157545_Practium.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriStream_ST10157545_Practium.Controllers
{
    [Authorize(Roles = AppRoles.AllRoles)]

    public class DashboardController : Controller
    {
        private const string DefaultField = "field-A";
        private const string TelemetryTable = "Telemetry";
        private const string StatusKey = "StatusMessage";
        private readonly IFunctionsClient _functions;
        private readonly AppDbContext _db;
        private readonly TableServiceClient _tables;
        private readonly ILogger<DashboardController> _logger;
        public DashboardController(
            IFunctionsClient functions,
            AppDbContext db,
            TableServiceClient tables,
            ILogger<DashboardController> logger)
        {
            _functions = functions;
            _db = db;
            _tables = tables;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string? fieldId, CancellationToken ct)
        {
            var field = string.IsNullOrWhiteSpace(fieldId) ? DefaultField : fieldId;
            IReadOnlyList<TelemetryReading> readings;
            try
            {
                readings = await _functions.GetTelemetryAsync(field, ct);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "Telemetry function call failed for {Field}", field);
                TempData[StatusKey] = "Telemetry is temporarily unavailable.";
                readings = Array.Empty<TelemetryReading>();
            }
            var devices = await _db.Devices
                .Where(d => d.FieldId == field)
                .OrderBy(d => d.DeviceId)
                .ToListAsync(ct);
            return View(new DashboardViewModel { FieldId = field, Readings = readings, Devices = devices });
        }
        [HttpPost]
        public async Task<IActionResult> Irrigate(string deviceId, string fieldId, string command, CancellationToken ct)
        {
            // The form field is named "command" because "action" would collide with the MVC route value.
            if (command != "Start" && command != "Stop")
            {
                return BadRequest("Unknown command.");
            }
            var ok = await _functions.TriggerIrrigationAsync(new IrrigationCommand(deviceId, fieldId, command), ct);
            TempData[StatusKey] = ok
                ? $"Irrigation command '{command}' queued for {deviceId}."
                : "Could not queue the irrigation command.";
            return RedirectToAction(nameof(Index), new { fieldId });
        }
        /// <summary>Inserts demo sensor readings so the dashboard has data to show.</summary>
        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> SeedTelemetry(string fieldId, CancellationToken ct)
        {
            var table = _tables.GetTableClient(TelemetryTable);
            await table.CreateIfNotExistsAsync(ct);
            var devices = await _db.Devices.Where(d => d.FieldId == fieldId).ToListAsync(ct);
            var random = new Random();
            foreach (var device in devices)
            {
                for (var i = 0; i < 10; i++)
                {
                    var moment = DateTime.UtcNow.AddMinutes(-i * 5);
                    var rowKey = (DateTime.MaxValue.Ticks - moment.Ticks).ToString("D19");
                    var entity = new TableEntity(fieldId, $"{rowKey}-{device.DeviceId}")
            {
                { "DeviceId", device.DeviceId },
                { "SoilMoisture", Math.Round(20 + random.NextDouble() * 40, 1) },
                { "Temperature", Math.Round(15 + random.NextDouble() * 20, 1) }
            };
                    await table.AddEntityAsync(entity, ct);
                }
            }
            TempData[StatusKey] = "Demo telemetry added.";
            return RedirectToAction(nameof(Index), new { fieldId });
        }
    }
}
