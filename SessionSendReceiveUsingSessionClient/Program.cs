using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SessionSendReceiveUsingSessionClient
{
    class Program
    {
        // Connection String for the namespace can be obtained from the Azure portal under the 
        // 'Shared Access policies' section.
        const string ServiceBusConnectionString = "Endpoint=sb://ccaidauesdevir2sb001.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=OLQnsp0G3L2TtD9WfNUTWZ0TdJ9lnpuFggJV2R6Vc/A=";
        const string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=ccaidauesdevir2stg001;AccountKey=fEuum13uELMlpp2D7ONXTQW6ZPZoCYA7s6FSdVgKkP7sjHRhewbRR9X+aFklTYthk4+JTDaDmXebvgTtErrDJg==;EndpointSuffix=core.windows.net";
        const string QueueName = "ovp-session-body-queue";
        static IMessageSender messageSender;
        static IMessageReceiver messageReceiver;
        static ISessionClient sessionClient;
        const string SessionPrefix = "session-prefix";
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }
        static async Task MainAsync()
        {
            const int numberOfSessions = 1;
            const int numberOfMessagesPerSession = 7;

            messageSender = new MessageSender(ServiceBusConnectionString, QueueName);
            messageReceiver = new MessageReceiver(ServiceBusConnectionString, QueueName);
            sessionClient = new SessionClient(ServiceBusConnectionString, QueueName);

            // Send messages with sessionId set
            //await SendSessionMessagesAsync(numberOfSessions, numberOfMessagesPerSession);

            // Receive all Session based messages using SessionClient
            //await ReceiveSessionMessagesAsync(numberOfSessions, numberOfMessagesPerSession);
            //await ReceiveMessagesAsync();
            await ReceiveSessionMessagesAsync();

            Console.WriteLine("=========================================================");
            Console.WriteLine("Completed Receiving all messages... Press any key to exit");
            Console.WriteLine("=========================================================");

            Console.ReadKey();

            await messageSender.CloseAsync();
            await sessionClient.CloseAsync();
        }

        private static async Task ReceiveMessagesAsync()
        {
            Console.WriteLine("===================================================================");
            Console.WriteLine("Accepting message in the reverse order of sends for demo purposes");
            Console.WriteLine("===================================================================");

            var configuration = new AzureStorageAttachmentConfiguration(StorageConnectionString);
            messageReceiver.RegisterAzureStorageAttachmentPlugin(configuration);
            
            var message = await messageReceiver.ReceiveAsync().ConfigureAwait(false);

            var stream = new MemoryStream(message.Body);
            StreamReader reader = new StreamReader(stream);
            string text = reader.ReadToEnd();

            
        }

        static async Task ReceiveSessionMessagesAsync()
        {
            Console.WriteLine("===================================================================");
            Console.WriteLine("Accepting sessions in the reverse order of sends for demo purposes");
            Console.WriteLine("===================================================================");

            var configuration = new AzureStorageAttachmentConfiguration(StorageConnectionString, containerName: "ovp");
            sessionClient.RegisterAzureStorageAttachmentPlugin(configuration);

            // AcceptMessageSessionAsync(i.ToString()) as below with session id as parameter will try to get a session with that sessionId.
            // AcceptMessageSessionAsync() without any messages will try to get any available session with messages associated with that session.
            IMessageSession session = await sessionClient.AcceptMessageSessionAsync();

            if (session != null)
            {
                Message message = await session.ReceiveAsync();

                var stream = new MemoryStream(message.Body);
                StreamReader reader = new StreamReader(stream);
                string text = reader.ReadToEnd();

                Console.WriteLine($"message body : {text}");

                await session.CompleteAsync(message.SystemProperties.LockToken);
            }
        }

        static async Task ReceiveSessionMessagesAsync(int numberOfSessions, int messagesPerSession)
        {
            Console.WriteLine("===================================================================");
            Console.WriteLine("Accepting sessions in the reverse order of sends for demo purposes");
            Console.WriteLine("===================================================================");

            for (int i = 0; i < numberOfSessions; i++)
            {
                int messagesReceivedPerSession = 0;

                // AcceptMessageSessionAsync(i.ToString()) as below with session id as parameter will try to get a session with that sessionId.
                // AcceptMessageSessionAsync() without any messages will try to get any available session with messages associated with that session.
                IMessageSession session = await sessionClient.AcceptMessageSessionAsync("2c812750-f89e-4e1e-b8bc-a104bd8dde16");

                if (session != null)
                {
                    // Messages within a session will always arrive in order.
                    Console.WriteLine("=====================================");
                    Console.WriteLine($"Received Session: {session.SessionId}");

                    while (messagesReceivedPerSession++ < messagesPerSession)
                    {
                        Message message = await session.ReceiveAsync();

                        //Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");
                        Console.WriteLine($"Message {message.UserProperties["MessageNo"].ToString()} of {message.UserProperties["TotalMessages"].ToString()} received");

                        // Complete the message so that it is not received again.
                        // This can be done only if the queueClient is created in ReceiveMode.PeekLock mode (which is default).
                        await session.CompleteAsync(message.SystemProperties.LockToken);
                    }

                    Console.WriteLine($"Received all messages for Session: {session.SessionId}");
                    Console.WriteLine("=====================================");

                    // Close the Session after receiving all messages from the session
                    await session.CloseAsync();
                }
            }
        }

        private static Task SendSessionMessagesAsync(int numberOfSessions, int numberOfMessagesPerSession)
        {
            throw new NotImplementedException();
        }
    }
}
