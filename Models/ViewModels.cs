using System.ComponentModel.DataAnnotations;

namespace AgriStream_ST10157545_Practium.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterViewModel
    {
        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = AppRoles.Farmer;
    }

    public record TelemetryReading(string DeviceId, double SoilMoisture, double Temperature, DateTimeOffset? Timestamp);

    public record IrrigationCommand(string DeviceId, string FieldId, string Action);

    public class DashboardViewModel
    {
        public string FieldId { get; set; } = string.Empty;
        public IReadOnlyList<TelemetryReading> Readings { get; set; } = Array.Empty<TelemetryReading>();
        public IReadOnlyList<Device> Devices { get; set; } = Array.Empty<Device>();
    }

    public class ImagesViewModel
    {
        public IReadOnlyList<string> BlobNames { get; set; } = Array.Empty<string>();
    }

}
