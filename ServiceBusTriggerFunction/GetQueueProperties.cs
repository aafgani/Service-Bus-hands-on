using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.ServiceBus.Management;

namespace ServiceBusTriggerFunction
{
    public static class GetQueueProperties
    {
        [FunctionName("GetQueueProperties")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string queueName = req.Query["queueName"];

            var managementClient = new ManagementClient("Endpoint=sb://ccaidauesdevir2sb001.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=OLQnsp0G3L2TtD9WfNUTWZ0TdJ9lnpuFggJV2R6Vc/A=");
            var queue = await managementClient.GetQueueAsync(queueName);

            return queueName != null
                ? (ActionResult)new OkObjectResult(queue)
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
