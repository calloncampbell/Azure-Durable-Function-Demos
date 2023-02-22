using Castle.Core.Configuration;
using DurableFunctionApp_StarWars.Abstrations.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DurableFunctionApp_StarWars.Functions.Durable.Activity
{
    public class SearchPlanetActivity
    {
        private readonly ILogger _logger;
        private readonly IConfigurationRoot _configuration;
        private HttpClient _httpClient;

        public SearchPlanetActivity(
            ILoggerFactory loggerFactory,
            IConfigurationRoot configuration,
            HttpClient httpClient)
        {
            _logger = loggerFactory.CreateLogger<SearchPlanetActivity>();
            _configuration = configuration;
            _httpClient = httpClient;            
        }

        [FunctionName(nameof(SearchPlanetActivity))]
        public async Task<Planet> RunAsync([ActivityTrigger] string name)
        {
            var uri = $"{_configuration["SwapiBaseUrl"]}planets?search={name}";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            var result = await _httpClient.SendAsync(requestMessage);
            if (!result.IsSuccessStatusCode)
            {
                return null;
            }

            var planetContent = await result.Content.ReadAsStringAsync();
            var planets = JToken.Parse(planetContent).SelectToken("results").ToObject<Planet[]>();

            return planets.FirstOrDefault();

        }
    }
}
