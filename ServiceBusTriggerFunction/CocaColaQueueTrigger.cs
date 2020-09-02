using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ServiceBusTriggerFunction
{
    public static class CocaColaQueueTrigger
    {
        [FunctionName("CocaColaQueueTrigger")]
        public static async Task RunAsync([ServiceBusTrigger("test", Connection = "MyServiceBusConnectionKey")] Message message,
            Int32 deliveryCount,
            DateTime enqueuedTimeUtc,
            string messageId, 
            ILogger log)
        {
            //var client = new QueueClient(Helper.ServiceBusConstring, Helper.queueTest, ReceiveMode.PeekLock);
            var msg = Helper.ReadStreamData(message.Body);
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {msg}");
            log.LogInformation($"Delivery count : {deliveryCount}");
            log.LogInformation($"Time : {DateTime.Now}");

            //await client.CompleteAsync(message.SystemProperties.LockToken);
        }
    }
}
