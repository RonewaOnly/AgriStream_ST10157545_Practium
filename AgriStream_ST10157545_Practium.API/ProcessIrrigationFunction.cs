using Microsoft.Azure.Functions.Worker;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace AgriStream_ST10157545_Practium.API
{
    public class ProcessIrrigationFunction
    {
        private const string UpdateSql = @"
        UPDATE DeviceRegistry
        SET OperationalStatus = @status, LastCommand = @command, LastUpdatedUtc = SYSUTCDATETIME()
        WHERE DeviceId = @deviceId";

        private readonly string _sqlConnection;
        private readonly ILogger<ProcessIrrigationFunction> _logger;

        public ProcessIrrigationFunction(IConfiguration configuration, ILogger<ProcessIrrigationFunction> logger)
        {
            _sqlConnection = configuration["SqlConnection"]
                ?? throw new InvalidOperationException("SqlConnection is not configured.");
            _logger = logger;
        }

        [Function(nameof(ProcessIrrigationFunction))]
        public async Task Run(
            [QueueTrigger(TriggerIrrigationFunction.QueueName, Connection = "StorageConnection")] string message)
        {
            var command = JsonSerializer.Deserialize<IrrigationCommand>(message)
                          ?? throw new InvalidOperationException("Invalid irrigation command message.");

            await using var connection = new SqlConnection(_sqlConnection);
            await connection.OpenAsync();

            await using var sql = new SqlCommand(UpdateSql, connection);
            sql.Parameters.AddWithValue("@status", command.Action == "Start" ? "Irrigating" : "Idle");
            sql.Parameters.AddWithValue("@command", command.Action);
            sql.Parameters.AddWithValue("@deviceId", command.DeviceId);

            var rows = await sql.ExecuteNonQueryAsync();
            _logger.LogInformation("Device {DeviceId} set by {Action} ({Rows} row updated)", command.DeviceId, command.Action, rows);
        }
    }

}
