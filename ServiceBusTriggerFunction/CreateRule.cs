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
using Microsoft.Azure.ServiceBus;

namespace ServiceBusTriggerFunction
{
    public static class CreateRule
    {
        [FunctionName("CreateRule")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string topicName = req.Query["topic"];
            string subscriptionName = req.Query["subscription"];
            string ruleName = req.Query["ruleName"];
            string ruleFilter = req.Query["ruleFilter"];
            var ruleDescription = new RuleDescription(ruleName, new SqlFilter(ruleFilter));

            //var managementClient = new ManagementClient("Endpoint=sb://ccaidauesdevir2sb001.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=OLQnsp0G3L2TtD9WfNUTWZ0TdJ9lnpuFggJV2R6Vc/A=");
            //var rule = await managementClient.CreateRuleAsync(topicName, subscriptionName, ruleDescription);

           // var nsManager = NamespaceManager.CreateFromConnectionString("Endpoint=sb://ccaidauesdevir2sb001.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=OLQnsp0G3L2TtD9WfNUTWZ0TdJ9lnpuFggJV2R6Vc/A=");
           //var rule = await nsManager.GetRulesAsync(topicName, subscriptionName);

            return ruleName != null
                ? (ActionResult)new OkObjectResult(ruleName)
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
