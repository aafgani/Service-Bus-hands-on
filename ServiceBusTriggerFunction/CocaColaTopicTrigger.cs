using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ServiceBusTriggerFunction
{
    public static class CocaColaTopicTrigger
    {
        [FunctionName("CocaColaTopicTrigger")]
        public static void Run([ServiceBusTrigger("testtopic1", "subscription1", Connection = "MyServiceBusConnectionKey")]string mySbMsg, ILogger log)
        {
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
        }
    }
}
