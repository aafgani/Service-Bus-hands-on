using MessageSender.Helper;
using MessageSender.Model;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MessageSender
{
    public class QueueLargeMessageSender : MessageSender
    {
        const string QueueName = "testwithcheck2";
        const string ResponseQueue = "response";

        public override async Task SendMessagesAsync(string messageBody)
        {
            var sb = new StringBuilder();
            sb.Append('A', Convert.ToInt32(messageBody));
            var sequenceNo = 0;

            var size = System.Text.ASCIIEncoding.ASCII.GetByteCount(sb.ToString());

            Console.WriteLine($"Mesage size : {size} bytes");
            if (size<256000)
            {
                Console.Write($"Are you sure to continue? [Y/N] ");
                var val = Console.ReadLine();

                if (val.Equals("N"))
                {
                    return;
                }
            }


            var bigMessage = new VeryBigDataType();
            bigMessage.BigValue = sb.ToString();

            var sessionId = Guid.NewGuid().ToString();
            var correlationId = Guid.NewGuid().ToString();

            await SendMessageAsync(bigMessage, sessionId, correlationId);
        }

        private async Task SendMessageAsync(VeryBigDataType o, string sessionId, string correlationId)
        {
            QueueClient queueClient = new QueueClient(ServiceBusConnectionString, QueueName, ReceiveMode.PeekLock);

            var sendTasks = new List<Task>();
            var requestData = SerializationHelper.Serialize(o.BigValue);
            var msgSize = System.Text.ASCIIEncoding.ASCII.GetByteCount(o.BigValue);
            var sequenceNo = 0;
            int chunkSize = getChunkSize((double) requestData.Length);

            using (var scope = new TransactionScope())
            {
                //var chunkSize = 4096;
                var expectedNo = (double)requestData.Length / chunkSize;
                var expectedNoMessages = (int)Math.Ceiling(expectedNo);

                var bytesToRead = requestData.Length;
                var nextRead = chunkSize;
                var lastChunk = false;
                byte[] buffer = new byte[chunkSize];
                while (bytesToRead > 0)
                {
                    sequenceNo++;

                    if (bytesToRead < chunkSize)
                        lastChunk = true;

                    if (lastChunk)
                    {
                        buffer = new byte[bytesToRead];
                        nextRead = (int)bytesToRead;
                    }
                    requestData.Read(buffer, 0, nextRead);

                    //var serviceBusMessageBody = new MemoryStream();
                    //serviceBusMessageBody.Write(buffer, 0, buffer.Length);
                    //serviceBusMessageBody.Flush();
                    //serviceBusMessageBody.Seek(0, SeekOrigin.Begin);
                    //var serviceBusMessage = new BrokeredMessage(serviceBusMessageBody, false);
                    //serviceBusMessage.SessionId = sessionId;
                    //serviceBusMessage.CorrelationId = correlationId;
                    //serviceBusMessage.ContentType = "application/json";
                    //serviceBusMessage.ReplyToSessionId = sessionId;
                    //serviceBusMessage.ReplyTo = ResponseQueue;
                    //serviceBusMessage.Properties.Add("MessageNo", sequenceNo);
                    //serviceBusMessage.Properties.Add("TotalMessages", expectedNoMessages);                    

                    var serviceBusMessage = new Message(buffer);
                    serviceBusMessage.SessionId = sessionId;
                    //serviceBusMessage.CorrelationId = correlationId;
                    serviceBusMessage.ContentType = "application/json";
                    //serviceBusMessage.ReplyToSessionId = sessionId;
                    //serviceBusMessage.ReplyTo = ResponseQueue;
                    serviceBusMessage.UserProperties.Add("MessageNo", sequenceNo);
                    serviceBusMessage.UserProperties.Add("TotalMessages", expectedNoMessages);
                    serviceBusMessage.Label = "test sequence message";

                    if (lastChunk)
                        serviceBusMessage.UserProperties.Add(new KeyValuePair<string, object>("EOF", true));

                    Console.WriteLine(string.Format("Sending message {0}", sequenceNo));
                    sendTasks.Add(queueClient.SendAsync(serviceBusMessage));
                    //await queueClient.SendAsync(serviceBusMessage);

                    bytesToRead = bytesToRead - nextRead;
                }
                Task.WaitAll(sendTasks.ToArray());
                scope.Complete();
            }

            Console.WriteLine($"Message with sessionId {sessionId} has sent successfully to Queue : {QueueName}!");
            await queueClient.CloseAsync();

            Console.Write(string.Format("{0} messages sent", sequenceNo));
            Console.WriteLine();
        }

        private int getChunkSize(double length)
        {
            var maxChunkNumber = 25;
            var chunkSize = 4096;
            var chunkNumber = length / chunkSize;

            while (chunkNumber > maxChunkNumber)
            {
                chunkSize = chunkSize * 2;
                chunkNumber = length / chunkSize;
            }

            return chunkSize;
        }
    }
}
