using System;
using System.Collections.Generic;
using System.Text;

namespace AgriStream_ST10157545_Practium.API
{
    public record IrrigationCommand(string DeviceId, string FieldId, string Action);


    public record TelemetryDto(string? DeviceId, double SoilMoisture, double Temperature, DateTimeOffset? Timestamp);
}