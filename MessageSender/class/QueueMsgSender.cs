using Microsoft.Azure.ServiceBus;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MessageSender
{
    public class QueueMsgSender : MessageSender
    {
        const string QueueName = "test";

        public override async Task SendMessagesAsync(string messageBody)
        {
            QueueClient queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

            var message = new Message(Encoding.UTF8.GetBytes(messageBody));
            message.SessionId = Guid.NewGuid().ToString();

            // Write the body of the message to the console
            Console.WriteLine($"Sending message: {messageBody}");

            // Send the message to the queue
            await queueClient.SendAsync(message);

            Console.WriteLine($"Message sent successfully !");

            await queueClient.CloseAsync();

        }
    }
}
