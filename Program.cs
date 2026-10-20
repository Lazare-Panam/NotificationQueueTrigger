using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();
var connectionString = Environment.GetEnvironmentVariable("AcsConnectionString");
ArgumentException.ThrowIfNullOrEmpty(connectionString);
builder.Services.AddAzureClients(clients =>
{
    clients.AddEmailClient(connectionString)
    .ConfigureOptions(options =>
    {
        options.Retry.Mode = Azure.Core.RetryMode.Exponential;
        options.Retry.Delay = TimeSpan.FromSeconds(2);
        options.Retry.MaxDelay = TimeSpan.FromSeconds(16);
        options.Retry.MaxRetries = 3;
    });
});

if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
{
    builder.Services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();
}

builder.Build().Run();
