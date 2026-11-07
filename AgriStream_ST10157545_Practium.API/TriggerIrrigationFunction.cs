using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using System;
using System.Collections.Generic;
using System.Text;
using Azure.Storage.Queues;
using System.Text.Json;

namespace AgriStream_ST10157545_Practium.API
{
    public class TriggerIrrigationFunction
    {
        public const string QueueName = "irrigation-commands";


        private readonly QueueServiceClient _queues;


        public TriggerIrrigationFunction(QueueServiceClient queues)
        {
            _queues = queues;
        }


        [Function(nameof(TriggerIrrigationFunction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "irrigation")] HttpRequest req)
        {
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
