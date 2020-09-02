using DurableFunction.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DurableFunction.Extensions
{
    public static class OrderExtensions
    {
        public static string GetInvoicePath(this Order order, string invoiceContainer)
        {
            if (order == null)
                throw new NullReferenceException(nameof(order));


            if (invoiceContainer != null && !invoiceContainer.EndsWith("/"))
            {
                invoiceContainer = invoiceContainer + "/";
            }

            return $"{invoiceContainer}{order.Customer}-{order.Id}.txt";

        }
    }
}
