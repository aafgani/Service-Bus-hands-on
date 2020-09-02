using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ServiceBusTriggerFunction
{
    public static class Helper
    {
        public const string ServiceBusConstring = "Endpoint=sb://ccaidauesdevir2sb001.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=OLQnsp0G3L2TtD9WfNUTWZ0TdJ9lnpuFggJV2R6Vc/A=";
        public const string queueTest = "test";
        public static string ReadStreamData(byte[] body)
        {
            var stream = new MemoryStream(body);
            StreamReader reader = new StreamReader(stream);
            string text = reader.ReadToEnd();

            return text;
        }
    }
}
