using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Net.Http;
using DurableFunction.Model;
using DurableFunction.Extensions;

namespace DurableFunction.Client
{
    public static class OrderReceiverFunction
    {
        [FunctionName(FunctionNames.OrderReceiverFunction)]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [OrchestrationClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string instanceId = null;
            var orderDto = await ReadOrderFromRequestAsync(req);
            if (orderDto != null)
            {
                var order = orderDto.ToOrder();
                instanceId = await starter.StartNewAsync(FunctionNames.OrderWorkflowFunction, order.Id, order);
                return starter.CreateCheckStatusResponse(req, instanceId);
            }
            return new BadRequestResult();
        }
        private static async Task<OrderDto> ReadOrderFromRequestAsync(HttpRequest req)
        {
            var jsonContent = await new StreamReader(req.Body).ReadToEndAsync();
            var orderDto = JsonConvert.DeserializeObject<OrderDto>(jsonContent);
            return orderDto;
        }
    }
}
