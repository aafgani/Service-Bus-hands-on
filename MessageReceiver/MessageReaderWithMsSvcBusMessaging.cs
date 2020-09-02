using MessageReceiver.Helper;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MessageReceiver
{
    public class MessageReaderWithMsSvcBusMessaging : IMessageReader
    {
        public void ReadMessage()
        {
            MessagingFactory _messagingFactory;
            const string RequestQueue = "testwithcheck";

            //Console.ForegroundColor = ConsoleColor.Green;
            var connectionString = ConfigurationManager.ConnectionStrings["ServiceBusConnectionString"];
            _messagingFactory = MessagingFactory.CreateFromConnectionString(connectionString.ConnectionString);
            

            var receiver = _messagingFactory.CreateQueueClient(RequestQueue, ReceiveMode.PeekLock);

            #region variables
            var messageBodyStream = new MemoryStream();
            MessageSession receiverSession = null;
            var keepPolling = true;
            var isFirstMessage = true;
            var expectedNoMessages = 0;
            BrokeredMessage[] messages = null;
            messageBodyStream = new MemoryStream();
            var messagesReceived = 0;
            var responseQueue = string.Empty;
            var sessionId = string.Empty;
            var correlationId = string.Empty;
            Stream fullMessageBodyStream = null;
            #endregion

            while (true)
            {
                Console.WriteLine("Waiting for new message");
                using (var scope = new TransactionScope())
                {
                    receiverSession = receiver.AcceptMessageSession();
                    #region variables
                    messageBodyStream = new MemoryStream();
                    keepPolling = true;
                    isFirstMessage = true;
                    expectedNoMessages = 0;
                    messages = null;
                    messagesReceived = 0;
                    responseQueue = string.Empty;
                    sessionId = string.Empty;
                    correlationId = string.Empty;
                    #endregion

                    while (keepPolling)
                    {
                        var message = receiverSession.Receive(TimeSpan.FromSeconds(10));
                        if (message == null)
                            continue;

                        if (isFirstMessage)
                        {
                            Console.WriteLine("Receiving first message");
                            expectedNoMessages = (int)message.Properties["TotalMessages"];
                            messages = new BrokeredMessage[expectedNoMessages];
                            isFirstMessage = false;
                            responseQueue = message.ReplyTo;
                            sessionId = message.SessionId;
                            correlationId = message.CorrelationId;
                        }

                        var messageNo = (int)message.Properties["MessageNo"];
                        var messageIndex = messageNo - 1;
                        Console.WriteLine(string.Format("Receiving message {0}", messageNo));
                        messages[messageIndex] = message;
                        messagesReceived++;

                        if (messagesReceived == expectedNoMessages)
                            keepPolling = false;
                    }

                    //Rebuild Object
                    fullMessageBodyStream = ChunkedMessageBuilder.ReconstructMessageBody(messages);

                    var completeTasks = new List<Task>();
                    foreach (var message in messages)
                        completeTasks.Add(message.CompleteAsync());

                    Task.WaitAll(completeTasks.ToArray());
                }
                var obj = SerializationHelper.Deserialize<string>(fullMessageBodyStream);

                Console.WriteLine(string.Format("Request Message is = {0}", obj));
                Console.WriteLine("");

                //var stringBuilder = new StringBuilder();
                //stringBuilder.Append('I', Convert.ToInt32(fullMessageBodyStream.Length));

                //var responseMessageBody = new Messages.VeryBigDataType();
                //responseMessageBody.BigValue = stringBuilder.ToString();

                //SendMessage(responseMessageBody, responseQueue, sessionId, correlationId);

            }

            Console.ReadLine();
        }
    }
}
