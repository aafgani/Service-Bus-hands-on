using DurableFunction.Entities;
using DurableFunction.Model;
using System;

namespace DurableFunction.Extensions
{
    public static class OrderDtoExtensions
    {
        public static Order ToOrder(this OrderDto dto)
        {
            if (dto == null)
                throw new NullReferenceException(nameof(dto));

            var orderId = Guid.NewGuid().ToString();
            return new Order()
            {
                Id = orderId,
                RowKey = orderId,
                PartitionKey = dto.Customer,
                Amount = dto.Amount,
                Customer = dto.Customer,
                CreationTimestamp = dto.CreationTimestamp,
                Timestamp = dto.CreationTimestamp,
                State = OrderStatus.Created,
                CustomerMail = dto.CustomerMail
            };
        }
    }
}
