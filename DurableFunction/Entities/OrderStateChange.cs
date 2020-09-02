namespace DurableFunction.Entities
{
    public class OrderStateChange
    {
        public string OrderId { get; set; }
        public OrderStatus NewOrderState { get; set; }
    }
}
