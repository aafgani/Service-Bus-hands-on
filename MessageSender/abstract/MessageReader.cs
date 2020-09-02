using System.Threading.Tasks;

namespace MessageSender
{
    public abstract class MessageReader : IMessageReader
    {
        protected string ServiceBusConnectionString = "Endpoint=sb://ccaidauesdevir2sb001.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=OLQnsp0G3L2TtD9WfNUTWZ0TdJ9lnpuFggJV2R6Vc/A=";

        public virtual void ReadMessagesAsync()
        {
            throw new System.NotImplementedException();
        }

        public virtual Task<int> ReportMessageCount()
        {
            throw new System.NotImplementedException();
        }
    }
}
