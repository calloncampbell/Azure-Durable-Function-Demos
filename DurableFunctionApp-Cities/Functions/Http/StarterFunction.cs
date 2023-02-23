using System.Net;
using DurableFunctionApp1.Functions.Durable.Orchestrator;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DurableFunctionApp1.Functions.Http
{
    public class StarterFunction
    {
        private readonly ILogger _logger;
        
        public StarterFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<StarterFunction>();
        }

        [Function(nameof(StarterFunction))]
        public async Task<HttpResponseData> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            [DurableClient(TaskHub = "%DurableTaskHubName%")] DurableTaskClient client)
        {
            var taskHubName = Environment.GetEnvironmentVariable("DurableTaskHubName", EnvironmentVariableTarget.Process);

            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(OrchestratorFunction));
            _logger.LogInformation("Created new orchestration with instance ID = {instanceId} on task hub {taskHubName}.", instanceId, taskHubName);

            return client.CreateCheckStatusResponse(req, instanceId);
        }
    }   
}
