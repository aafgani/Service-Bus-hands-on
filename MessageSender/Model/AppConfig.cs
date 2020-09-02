using System;
using System.Collections.Generic;
using System.Text;

namespace MessageSender.Model
{
    public class AppConfig
    {
        public string ServiceBusConnectionString { get; set; }
        public string QueueName { get; set; }
        public string HttpTriggeredFunctionBaseUrl { get; set; }
    }
}
