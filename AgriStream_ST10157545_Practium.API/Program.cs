using Azure.Data.Tables;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Functions.Worker.ApplicationInsights;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var storage = builder.Configuration["StorageConnection"]
    ?? throw new InvalidOperationException("StorageConnection is not configured.");

builder.Services.AddSingleton(new TableServiceClient(storage));
builder.Services.AddSingleton(new QueueServiceClient(
    storage,
    new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 }));

/*builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();*/

builder.Build().Run();
