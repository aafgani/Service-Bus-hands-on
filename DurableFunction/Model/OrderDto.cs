using System;
using System.Collections.Generic;
using System.Text;

namespace DurableFunction.Model
{
   public  class OrderDto
    {
        public string Customer { get; set; }
        public string CustomerMail { get; set; }
        public decimal Amount { get; set; }
        public DateTimeOffset CreationTimestamp { get; set; }
    }
}
