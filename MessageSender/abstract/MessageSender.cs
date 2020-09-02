using System.Threading.Tasks;

namespace MessageSender
{
    public abstract class MessageSender : IMessageSender
    {
        protected string ServiceBusConnectionString= "Endpoint=sb://ccaidauesdevir2sb001.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=OLQnsp0G3L2TtD9WfNUTWZ0TdJ9lnpuFggJV2R6Vc/A=";

        public abstract Task SendMessagesAsync(string messageBody);
    }
}
