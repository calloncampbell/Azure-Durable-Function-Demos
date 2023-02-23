using System.Net;
using DurableFunctionApp_StarWars.Abstrations.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace DurableFunctionApp_StarWars.Functions.Http
{
    public class HttpStart
    {
        private readonly ILogger _logger;

        public HttpStart(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HttpStart>();
        }

        [FunctionName(nameof(HttpStart))]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, Route = "start/{orchestratorName}/")] HttpRequest req,            
            [DurableClient(TaskHub = "%DurableTaskHubName%")] IDurableOrchestrationClient client,            
            string orchestratorName)
        {            
            string orchestratorInput = string.Empty;
            var streamReader = new StreamReader(req.Body);
            orchestratorInput = await streamReader.ReadToEndAsync();           
            
            string instanceId = Guid.NewGuid().ToString();
            await client.StartNewAsync(orchestratorName, instanceId, orchestratorInput);

            var taskHubName = Environment.GetEnvironmentVariable("DurableTaskHubName", EnvironmentVariableTarget.Process);

            _logger.LogInformation("Created new orchestration with instance ID = {instanceId} on task hub {taskHubName}.", instanceId, taskHubName);

            return client.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
