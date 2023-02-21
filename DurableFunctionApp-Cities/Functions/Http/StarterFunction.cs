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
            [DurableClient(TaskHub = "%DurableTaskHubName%")] DurableTaskClient client,
            FunctionContext executionContext)
        {
            var taskHubName = Environment.GetEnvironmentVariable("DurableTaskHubName", EnvironmentVariableTarget.Process);
            _logger.LogInformation("StarterFunction triggered on DurableTaskHub: {taskHubName}", taskHubName);

            ILogger logger = executionContext.GetLogger(nameof(StarterFunction));

            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(OrchestratorFunction));
            logger.LogInformation("Created new orchestration with instance ID = {instanceId}", instanceId);

            return client.CreateCheckStatusResponse(req, instanceId);
        }
    }   
}
