using System;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ServiceBusTriggerFunction
{
    public static class DeadLetterTriggered
    {
        [FunctionName("DeadLetterTriggered")]
        public static void Run([ServiceBusTrigger("test//$deadletterqueue", Connection = "MyServiceBusConnectionKey")]Message message, ILogger log)
        {
            var msg = Helper.ReadStreamData(message.Body);
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {msg}");
        }
    }
}
