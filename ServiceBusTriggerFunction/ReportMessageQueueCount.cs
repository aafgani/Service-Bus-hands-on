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
    public static class ReportMessageQueueCount
    {
        [FunctionName("ReportMessageQueueCount")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post",Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            var managementClient = new ManagementClient("Endpoint=sb://ccaidauesdevir2sb001.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=OLQnsp0G3L2TtD9WfNUTWZ0TdJ9lnpuFggJV2R6Vc/A=");
            var queue = await managementClient.GetQueueRuntimeInfoAsync(name);
            var messageCount = queue.MessageCountDetails;

            return name != null
                ? (ActionResult) new OkObjectResult(messageCount)
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
