using System.Threading.Tasks;

namespace MessageSender
{
    interface IMessageSender
    {
        Task SendMessagesAsync(string messageBody);
    }
}
