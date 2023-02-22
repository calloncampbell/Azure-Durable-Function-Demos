using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DurableFunctionApp_StarWars.Abstrations.Models;
using DurableFunctionApp_StarWars.Functions.Durable.Activity;
using System.Linq;
using Newtonsoft.Json;

namespace DurableFunctionApp_StarWars.Functions.Durable.Orchestrator
{
    public class GetPlanetResidentsOrchestrator
    {
        private readonly ILogger _logger;

        public GetPlanetResidentsOrchestrator(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetPlanetResidentsOrchestrator>();
        }
        
        [FunctionName(nameof(GetPlanetResidentsOrchestrator))]
        public async Task<PlanetResidents> Run(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            [DurableClient] IDurableOrchestrationClient orchestrationClient)
        {
            if (!context.IsReplaying)
            {
                _logger.LogInformation($"{nameof(GetPlanetResidentsOrchestrator)} started for InstanceId: {context.InstanceId}");
            }
            
            var planetName = context.GetInput<string>();

            var result = new PlanetResidents();

            var planetResult = await context.CallActivityAsync<Planet>(
                nameof(SearchPlanetActivity),
                planetName);

            if (planetResult != null)
            {
                result.PlanetName = planetResult.Name;
                
                var tasks = new List<Task<Person>>();
                foreach (var residentUrl in planetResult.ResidentUrls)
                {
                    tasks.Add(context.CallActivityAsync<Person>(nameof(GetCharacterActivity), residentUrl));                    
                }
                
                await Task.WhenAll(tasks);

                result.Residents = tasks.Select(task => task.Result).ToList<Person>();
            }

            return result;
        }
    }
}
