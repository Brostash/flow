using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading;

namespace Flow.Function
{
    // Define the entity interface
    public interface IStateEntity
    {
        void SetState(Dictionary<string, object> state);
        Dictionary<string, object> GetState();
        void AddOrUpdateValue(KeyValuePair<string, object> keyValue);
    }

    // Define the entity class
    [JsonObject(MemberSerialization.OptIn)]
    public class StateEntity : IStateEntity
    {
        [JsonProperty("state")]
        public Dictionary<string, object> State { get; set; } = new Dictionary<string, object>();

        public void SetState(Dictionary<string, object> state)
        {
            State = state;
        }

        public Dictionary<string, object> GetState()
        {
            return State;
        }

        public void AddOrUpdateValue(KeyValuePair<string, object> keyValue)
        {
            State[keyValue.Key] = keyValue.Value;
        }

        [FunctionName(nameof(StateEntity))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
        {
            return ctx.DispatchAsync<StateEntity>();
        }
    }

    public static class DurableFunction
    {
        [FunctionName("StateManagementOrchestrator")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();
            
            // Create a unique entity ID for this orchestration
            var entityId = new EntityId(nameof(StateEntity), context.InstanceId);
            
            // Initialize state
            var initialState = new Dictionary<string, object>
            {
                { "InitialKey", "InitialValue" }
            };
            
            // Set the initial state in the entity
            await context.CallEntityAsync(entityId, "SetState", initialState);
            
            // Update specific values in the entity
            await context.CallEntityAsync(entityId, "AddOrUpdateValue", new KeyValuePair<string, object>("UpdatedKey", "UpdatedValue"));
            await context.CallEntityAsync(entityId, "AddOrUpdateValue", 
                new KeyValuePair<string, object>("Timestamp", context.CurrentUtcDateTime.ToString()));
            
            // Read the state from the entity
            var savedState = await context.CallEntityAsync<Dictionary<string, object>>(
                entityId, "GetState");
            
            // Add state to outputs
            outputs.Add(JsonConvert.SerializeObject(savedState));
            
            return outputs;
        }

        [FunctionName("HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            string instanceId = await starter.StartNewAsync("StateManagementOrchestrator", null);
            log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);
            return starter.CreateCheckStatusResponse(req, instanceId);
        }
        
        // Example of how to query entity state directly via HTTP
        [FunctionName("GetEntityState")]
        public static async Task<HttpResponseMessage> GetEntityState(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestMessage req,
            [DurableClient] IDurableEntityClient entityClient,
            ILogger log)
        {
            // Get instance ID from query string
            var queryParams = req.RequestUri.ParseQueryString();
            string instanceId = queryParams["instanceId"];
            
            if (string.IsNullOrEmpty(instanceId))
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Please provide an instanceId query parameter")
                };
            }
            
            // Create entity ID and read state
            var entityId = new EntityId(nameof(StateEntity), instanceId);
            var entityState = await entityClient.ReadEntityStateAsync<StateEntity>(entityId);
            
            if (!entityState.EntityExists)
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
                {
                    Content = new StringContent($"No entity found for instance ID {instanceId}")
                };
            }
            
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(entityState.EntityState.State),
                    System.Text.Encoding.UTF8,
                    "application/json")
            };
        }
    }
}