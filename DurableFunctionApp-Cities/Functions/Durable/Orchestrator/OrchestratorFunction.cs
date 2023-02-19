using System.Net;
using DurableFunctionApp1.Functions.Durable.Activity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace DurableFunctionApp1.Functions.Durable.Orchestrator
{
    public class OrchestratorFunction
    {
        private readonly ILogger _logger;

        public OrchestratorFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<OrchestratorFunction>();
        }
        
        [Function(nameof(OrchestratorFunction))]
        public async Task<string> HelloCitiesAsync([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            string result = "";
            result += await context.CallActivityAsync<string>(nameof(ActivityFunction), "Toronto") + " ";
            result += await context.CallActivityAsync<string>(nameof(ActivityFunction), "Montreal") + " ";
            result += await context.CallActivityAsync<string>(nameof(ActivityFunction), "Vancouver");
            return result;
        }
    }
}
