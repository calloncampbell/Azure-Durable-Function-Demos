using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace DurableFunctionApp1.Functions.Durable.Activity
{
    public class ActivityFunction
    {
        private readonly ILogger _logger;

        public ActivityFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ActivityFunction>();
        }

        [Function(nameof(ActivityFunction))]
        public string Run([ActivityTrigger] string cityName, FunctionContext executionContext)
        {
            _logger.LogInformation("Saying hello to {name}", cityName);
            return $"Hello, {cityName}!";
        }
    }
}
