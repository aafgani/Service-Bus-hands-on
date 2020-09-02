using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageSender
{
    public class TopicMessageSender : MessageSender
    {
        const string TopicName = "Testtopic1";

        public override async Task SendMessagesAsync(string messageBody)
        {
            TopicClient topicClient = new TopicClient(ServiceBusConnectionString, TopicName);

            // Create a new message to send to the topic
            var data = messageBody.Split(",");
            var session = Guid.NewGuid().ToString();

            var totalAmount = Convert.ToInt32(data[1]) * 100000;
            var message = new Message(Encoding.UTF8.GetBytes(string.Format("{0}  @{1} = {2}", data[0], data[1], totalAmount)));
            var member = totalAmount >= 500000 ? "Gold" : "Silver";
            //message.UserProperties.Add("Member", member);
            message.TimeToLive = TimeSpan.FromMinutes(3);
            message.SessionId = session;

            Console.WriteLine($"Total Amount : {Convert.ToDecimal(totalAmount)}");
            Console.WriteLine($"Member: {member}");
            Console.WriteLine($"Message sent successfully !");

            // Send the message to the topic
            await topicClient.SendAsync(message);

            await topicClient.CloseAsync();
        }
    }
}
