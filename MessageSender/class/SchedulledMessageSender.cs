using Microsoft.Azure.ServiceBus;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MessageSender
{
    public class SchedulledMessageSender : MessageSender
    {
        const string QueueName = "test";
        public override async Task SendMessagesAsync(string messageBody)
        {
            QueueClient queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

            var schedule = DateTime.UtcNow.AddSeconds(60);

            var message = new Message(Encoding.UTF8.GetBytes(messageBody));
            message.SessionId = Guid.NewGuid().ToString();

            // Write the body of the message to the console
            Console.WriteLine($"Sending scheduled message: {messageBody}");

            // Send the message to the queue
           var result = await queueClient.ScheduleMessageAsync(message, schedule);

            Console.WriteLine($"Message sent successfully at {DateTime.Now}!");

            await queueClient.CloseAsync();
        }
    }
}
