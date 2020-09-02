using System;
using System.IO;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ServiceBusTriggerFunction
{
    public static class CocaColaQueueWithCheckTrigger
    {
        [FunctionName("CocaColaQueueWithCheckTrigger")]
        public static void Run([ServiceBusTrigger("testwithcheck2", Connection = "MyServiceBusConnectionKey", IsSessionsEnabled = true
            )] Message message,
            ILogger log)
        {
            //log.LogInformation($"CorrelationId: {CorrelationId}");
            //log.LogInformation($"MessageId: {MessageId}");

            var msg = Helper.ReadStreamData(message.Body);
            log.LogInformation($"C# ServiceBus queue trigger function processed message (Queue with check) : {message.SessionId} - {message.Body.Length}");
            log.LogInformation($"Message received No. {message.UserProperties["MessageNo"].ToString()}");
        }
    }
}
