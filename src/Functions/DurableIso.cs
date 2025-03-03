using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Flow.Models.Entities;

namespace Flow.Functions;


public static class DurableIso
{
    [Function(nameof(DurableIso))]
    public static async Task<List<string>> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        ILogger logger = context.CreateReplaySafeLogger(nameof(DurableIso));
        logger.LogInformation("Saying hello.");
        var outputs = new List<string>();

        // Replace name and input with values relevant for your Durable Functions Activity
        QueryById input = new QueryById("2", "2");
        outputs.Add((await context.CallActivityAsync<Action>("ActionQuery", input)).id);


        // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
        return outputs;
    }

    [Function(nameof(SayHello))]
    public static string SayHello([ActivityTrigger] string name, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("SayHello");
        logger.LogInformation("Saying hello to {name}.", name);
        return $"Hello {name}!";
    }


    [Function("DocumentResponse")]
    public static DocumentResponse RunDocumentResponse([ActivityTrigger] string name, FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger("DocumentResponse");
        logger.LogInformation("C# HTTP trigger function processed a request.");

        var message = "Welcome to Azure Functions!";


        // Return a response to both HTTP trigger and Azure Cosmos DB output binding.
        return new DocumentResponse()
        {
            Document = new MyDocument
            {
                id = System.Guid.NewGuid().ToString(),
                message = message
            }
        };
    }


    //disable this function 
    [Function("DurableIso_HttpStart")]
    public static async Task<HttpResponseData> HttpStart(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("DurableIso_HttpStart");

        // Function input comes from the request content.
        string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(DurableIso));

        logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

        // Returns an HTTP 202 response with an instance management payload.
        // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
        return await client.CreateCheckStatusResponseAsync(req, instanceId);
    }


}



