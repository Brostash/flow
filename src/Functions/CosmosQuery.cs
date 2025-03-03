using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Flow.Models.Entities;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace Flow.Functions;

public static class CosmosQuery
{

    [Function("ActionQuery")]

    public static async Task<Action> RunActionQuery(
     [ActivityTrigger] QueryById queryInput,
      FunctionContext executionContext,
      [CosmosDBInput("flow-db", "Actions",
            Connection = "CosmosDbConnectionString",
            Id = "{id}",
            PartitionKey = "{partitionKey}")] Action action
      )
    {
        var logger = executionContext.GetLogger("ActionQuery");
        logger.LogInformation($"QUERYING ACTION: {action.id}, {action.actionType}");

        return action;
    }
}

