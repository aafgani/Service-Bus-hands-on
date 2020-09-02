using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using DurableFunction.Entities;
using Microsoft.WindowsAzure.Storage.Table;
using DurableFunction.Extensions;

namespace DurableFunction.Activities
{
    public static class AddOrder
    {
        [FunctionName(FunctionNames.AddOrderFunction)]
        public static async Task<bool> Run([ActivityTrigger] Order order,
            [Table(SourceNames.OrdersTable, Connection = "StorageAccount")] CloudTable orderTable,
            ILogger log)
        {
            log.LogInformation($"[START ACTIVITY] --> {FunctionNames.AddOrderFunction} for {order}");
            bool retVal = false;
            try
            {
                retVal = await orderTable.InsertAsync(order);
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Error during adding order  {order}");
                retVal = false;
            }
            return retVal;
        }
    }
}
