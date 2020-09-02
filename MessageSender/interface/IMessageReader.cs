using System.Threading.Tasks;

namespace MessageSender
{
    interface IMessageReader
    {
        void ReadMessagesAsync();
        Task<int> ReportMessageCount();
    }
}
