using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Azure.Data.Tables;
using Azure.Storage.Blobs;

namespace AgriStream_ST10157545_Practium.API
{
    public class ImageUploadedFunction
    {
        private const string AuditTable = "ImageAudit";
        private const string AuditPartition = "DroneImages";

        private readonly TableServiceClient _tables;
        private readonly ILogger<ImageUploadedFunction> _logger;


        public ImageUploadedFunction(TableServiceClient tables, ILogger<ImageUploadedFunction> logger)
        {
            _tables = tables;
            _logger = logger;
        }

        [Function(nameof(ImageUploadedFunction))]
        public async Task Run(
            [BlobTrigger("drone-images/{name}", Connection = "StorageConnection")] BlobClient blob,
            string name)
        {
            var properties = (await blob.GetPropertiesAsync()).Value;
            _logger.LogInformation("Drone image {Name} uploaded ({Size} bytes)", name, properties.ContentLength);

            var table = _tables.GetTableClient(AuditTable);
            await table.CreateIfNotExistsAsync();

            var entry = new TableEntity(AuditPartition, Guid.NewGuid().ToString())
        {
            { "BlobName", name },
            { "SizeBytes", properties.ContentLength },
            { "ContentType", properties.ContentType },
            { "Status", "Received" },
            { "ProcessedUtc", DateTime.UtcNow }
        };
            await table.AddEntityAsync(entry);
        }
    }
}
