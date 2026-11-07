using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client.TelemetryCore.TelemetryClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgriStream_ST10157545_Practium.API
{
    public class GetTelemetryFunction
    {
        private const string TelemetryTable = "Telemetry";
        private const int MaxResults = 20;
        private IConfiguration _config; // Configuration is injected into the constructor and stored in a private field.


        private readonly TableServiceClient _tables;


        public GetTelemetryFunction(TableServiceClient tables, IConfiguration config)
        {
            _tables = tables;
            _config = config;
        }


        [Function(nameof(GetTelemetryFunction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "telemetry/{fieldId}")] HttpRequest req,
            string fieldId)
        {
            if (!ApiKeyGuard.IsValid(req, _config["ApiSecret"]))
                return new UnauthorizedResult();
            var table = _tables.GetTableClient(TelemetryTable);
            await table.CreateIfNotExistsAsync();


            var results = new List<TelemetryDto>();


            // RowKey is an inverted timestamp, so the first rows returned are the newest.
            await foreach (var entity in table.QueryAsync<TableEntity>(
                               e => e.PartitionKey == fieldId,
                               maxPerPage: MaxResults,
                               cancellationToken: req.HttpContext.RequestAborted))
            {
                results.Add(new TelemetryDto(
                    entity.GetString("DeviceId"),
                    entity.GetDouble("SoilMoisture") ?? 0,
                    entity.GetDouble("Temperature") ?? 0,
                    entity.Timestamp));


                if (results.Count >= MaxResults)
                {
                    break;
                }
            }


            return new OkObjectResult(results);
        }
    }
}