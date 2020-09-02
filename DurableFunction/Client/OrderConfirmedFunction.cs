using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using DurableFunction.Model;

namespace DurableFunction.Client
{
    public static class OrderConfirmedFunction
    {
        [FunctionName("OrderConfirmedFunction")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put")] HttpRequestMessage req,
            [OrchestrationClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            return await SendEventToOrderAsync(req, starter, Events.OrderPaid, log);

        }

        private static async Task<HttpResponseMessage> SendEventToOrderAsync(HttpRequestMessage req, 
            IDurableOrchestrationClient starter, 
            string orderEvent, 
            ILogger log)
        {
            var jsonContent = await req.Content.ReadAsStringAsync();
            OrderEventDto orderEventDto = JsonConvert.DeserializeObject<OrderEventDto>(jsonContent);
            if (orderEventDto != null && !string.IsNullOrWhiteSpace(orderEventDto.OrderId))
            {
                await starter.RaiseEventAsync(orderEventDto.OrderId, orderEvent, null);
                return starter.CreateCheckStatusResponse(req, orderEventDto.OrderId);
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest) { ReasonPhrase = "Order not valid" };
        }
    }
}
