using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.CosmosDB;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureFunctionsWebApplication();

// If using Application Insights
// builder.Services.AddApplicationInsightsTelemetryWorkerService();

builder.Build().Run();