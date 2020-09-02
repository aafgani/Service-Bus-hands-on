using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using DurableFunction.Entities;
using System.Threading;

namespace DurableFunction.Orchestrator
{
    public static class OrderWorkflow
    {
        [FunctionName(FunctionNames.OrderWorkflowFunction)]
        public static async Task Run(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            log.LogInformation($"[START ORCHESTRATOR] --> {FunctionNames.OrderWorkflowFunction}");
            var order = context.GetInput<Order>();

            log.LogTrace($"Adding Order {order}");
            var addResult = await context.CallActivityWithRetryAsync<bool>(FunctionNames.AddOrderFunction,
                new RetryOptions(TimeSpan.FromSeconds(1), 10), order);

            if (addResult)
            {
                DateTime orderDeadline = GetOrderDeadLine(context);

                var orderPaidEvent = context.WaitForExternalEvent(Events.OrderPaid);
                var orderCancelledEvent = context.WaitForExternalEvent(Events.OrderCancelled);
                var cancelTimer = context.CreateTimer(orderDeadline, CancellationToken.None);

                var taskCompleted = await Task.WhenAny(orderPaidEvent, orderCancelledEvent, cancelTimer);
                if (taskCompleted == orderCancelledEvent || taskCompleted == cancelTimer)
                {
                    log.LogWarning($"Order Cancelled : {order}");
                    order = await context.CallActivityAsync<Order>(FunctionNames.FinalizeOrderFunction,
                        new OrderStateChange()
                        {
                            NewOrderState = OrderStatus.Cancelled,
                            OrderId = order.Id
                        });
                }
                else if (taskCompleted == orderPaidEvent)
                {
                    log.LogTrace($"Order Paid : {order}");
                    order = await context.CallActivityAsync<Order>(FunctionNames.FinalizeOrderFunction,
                        new OrderStateChange()
                        {
                            NewOrderState = OrderStatus.Paid,
                            OrderId = order.Id
                        });
                    await context.CallActivityAsync<string>(FunctionNames.GenerateInvoiceFunction, order);
                }

                if (order != null)
                {
                    var sendMailResult = await context.CallActivityAsync<bool>(FunctionNames.SendMailFunction, order);
                    log.LogTrace($"Sendmail result : {sendMailResult}");
                }
            }
        }

        private static DateTime GetOrderDeadLine(IDurableOrchestrationContext context)
        {
            var orderExpireTimeoutConfig = Environment.GetEnvironmentVariable("OrderExpireTimeout");
            int orderExpireTimeout;
            if (!int.TryParse(orderExpireTimeoutConfig, out orderExpireTimeout))
                orderExpireTimeout = 1;

            var orderDeadline = context.CurrentUtcDateTime.ToLocalTime().AddMinutes(orderExpireTimeout);

            return orderDeadline;
        }
    }
}
