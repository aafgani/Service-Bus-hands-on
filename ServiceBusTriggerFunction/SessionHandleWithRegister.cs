using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.ServiceBus;
using System;
using Newtonsoft.Json;
using System.Text;
using System.Threading;
using System.IO;

namespace ServiceBusTriggerFunction
{
    public static class SessionHandleWithRegister
    {
        private static Message[] messages;
        private static bool isFirstMessage = true;
        private static string sessionId ;
        private static string correlationId;
        private static int messagesReceived=0, expectedNoMessages=0;

        [FunctionName("SessionHandleWithRegister")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var connectionString = "Endpoint=sb://ccaidauesdevir2sb001.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=OLQnsp0G3L2TtD9WfNUTWZ0TdJ9lnpuFggJV2R6Vc/A=";
            string queueName = req.Query["queueName"];
            var client = new QueueClient(connectionString, queueName, ReceiveMode.PeekLock);
            client.RegisterSessionHandler(
               Aselole,
               new SessionHandlerOptions(e => LogMessageHandlerException(e, log))
                {
                    MessageWaitTimeout = TimeSpan.FromSeconds(5),
                    MaxConcurrentSessions = 1,
                    AutoComplete = false
                });

            //if (messagesReceived == expectedNoMessages)
            //{
            //    Stream fullMessageBodyStream = ReconstructMessageBody(messages);
            //    var result = ReadStreamData(fullMessageBodyStream);

            //    log.LogInformation($"SessionId = {sessionId}.");
            //    log.LogInformation($"result = {result}.");

            //    return new OkObjectResult("OK");
            //}

            return new BadRequestObjectResult("Please pass a name on the query string or in the request body");

        }

        private static string ReadStreamData(Stream fullMessageBodyStream)
        {
            StreamReader reader = new StreamReader(fullMessageBodyStream);
            return reader.ReadToEnd();
        }

        private static Stream ReconstructMessageBody(Message[] messages)
        {
            var messageBodyStream = new MemoryStream();
            var streamWriter = new StreamWriter(messageBodyStream);
            StreamReader streamReader = null;
            foreach (var message in messages)
            {
                streamReader = new StreamReader(new MemoryStream(message.Body));
                var text = streamReader.ReadToEnd();
                streamWriter.Write(text);
                streamWriter.Flush();
                messageBodyStream.Flush();
            }

            messageBodyStream.Seek(0, SeekOrigin.Begin);
            return messageBodyStream;
        }

        private static async Task Aselole(IMessageSession session, Message message, CancellationToken arg3)
        {
            if (message.Label != null &&
                  message.ContentType != null &&
                  //message.Label.Equals("RecipeStep", StringComparison.InvariantCultureIgnoreCase) &&
                  message.ContentType.Equals("application/json", StringComparison.InvariantCultureIgnoreCase))
            {

                if (message.SessionId == null)
                {
                    return;
                }

                if (isFirstMessage)
                {
                    expectedNoMessages = (int)message.UserProperties["TotalMessages"];
                    messages = new Message[expectedNoMessages];
                    isFirstMessage = false;
                    sessionId = message.SessionId;
                    correlationId = message.CorrelationId;
                    isFirstMessage = false;
                }

                var messageNo = (int)message.UserProperties["MessageNo"];
                var messageIndex = messageNo - 1;
                Console.WriteLine(string.Format("Receiving message {0}", messageNo));
                messages[messageIndex] = message;
                messagesReceived++;

                var body = message.Body;

                //string recipeStep = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(body));
                lock (Console.Out)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(
                        "\t\t\t\tMessage received:  \n\t\t\t\t\t\tSessionId = {0}, \n\t\t\t\t\t\tMessageId = {1}, \n\t\t\t\t\t\tSequenceNumber = {2}," +
                        "\n\t\t\t\t\t\tContent: [ step = {3}, title = {4} ]",
                        message.SessionId,
                        message.MessageId,
                        message.SystemProperties.SequenceNumber,
                        "",
                        "");
                    Console.ResetColor();
                }
                await session.CompleteAsync(message.SystemProperties.LockToken);

                if (message.UserProperties["EOF"] != null)
                {
                    await session.CloseAsync();
                }
            }
            else
            {
                await session.DeadLetterAsync(message.SystemProperties.LockToken);//, "BadMessage", "Unexpected message");
            }
        }

        private static Task LogMessageHandlerException(ExceptionReceivedEventArgs e, ILogger log)
        {
            log.LogInformation("Exception: \"{0}\" {1}", e.Exception.Message, e.ExceptionReceivedContext.EntityPath);
            return Task.CompletedTask;
        }
    }
}
