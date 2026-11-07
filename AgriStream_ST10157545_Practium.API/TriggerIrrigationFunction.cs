using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using System;
using System.Collections.Generic;
using System.Text;
using Azure.Storage.Queues;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace AgriStream_ST10157545_Practium.API
{
    public class TriggerIrrigationFunction
    {
        public const string QueueName = "irrigation-commands";


        private readonly QueueServiceClient _queues;

        private readonly IConfiguration _config; // Configuration is injected into the constructor and stored in a private field.
        public TriggerIrrigationFunction(QueueServiceClient queues, IConfiguration config)
        {
            _queues = queues;
            _config = config;
        }


        [Function(nameof(TriggerIrrigationFunction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "irrigation")] HttpRequest req)
        {
            if (!ApiKeyGuard.IsValid(req, _config["ApiSecret"]))
                return new UnauthorizedResult();

            IrrigationCommand? command;
            try
            {
                command = await req.ReadFromJsonAsync<IrrigationCommand>(req.HttpContext.RequestAborted);
            }
            catch (JsonException)
            {
                return new BadRequestObjectResult("Body must be valid JSON.");
            }


            if (command is null ||
                string.IsNullOrWhiteSpace(command.DeviceId) ||
                string.IsNullOrWhiteSpace(command.Action))
            {
                return new BadRequestObjectResult("DeviceId and Action are required.");
            }


            var queue = _queues.GetQueueClient(QueueName);
            await queue.CreateIfNotExistsAsync();
            await queue.SendMessageAsync(JsonSerializer.Serialize(command));


            return new AcceptedResult();
        }
    }


}
