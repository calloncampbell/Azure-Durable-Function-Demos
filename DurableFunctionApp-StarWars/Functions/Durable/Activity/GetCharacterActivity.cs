using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using DurableFunctionApp_StarWars.Abstrations.Models;
using Newtonsoft.Json;

namespace DurableFunctionApp_StarWars.Functions.Durable.Activity
{
    public class GetCharacterActivity
    {
        private readonly ILogger _logger;
        private HttpClient _httpClient;

        public GetCharacterActivity(
            ILoggerFactory loggerFactory, 
            HttpClient httpClient)
        {
            _logger = loggerFactory.CreateLogger<GetCharacterActivity>();
            _httpClient = httpClient;
        }

        [FunctionName(nameof(GetCharacterActivity))]
        public async Task<Person> RunAsync([ActivityTrigger] string characterUri)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, characterUri);
            var result = await _httpClient.SendAsync(requestMessage);

            if (!result.IsSuccessStatusCode)
            {
                return null;
            }

            var characterContent = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Person>(characterContent);
        }
    }
}
