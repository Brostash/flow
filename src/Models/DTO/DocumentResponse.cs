using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Flow.Models.Entities;

public class DocumentResponse
{
    [CosmosDBOutput("my-database", "my-container",
        Connection = "CosmosDbConnectionString", CreateIfNotExists = true)]
    public MyDocument Document { get; set; }
}