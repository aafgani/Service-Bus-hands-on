using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.ServiceBus;
using System.Transactions;
using System.IO;
using System.Collections.Generic;
using System;

namespace ServiceBusTriggerFunction
{
    public static class SessionHandlerUsingSessionClient
    {
        [FunctionName("SessionHandlerUsingSessionClient")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ExecutionContext context,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var connectionString = "Endpoint=sb://ccaidauesdevir2sb001.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=OLQnsp0G3L2TtD9WfNUTWZ0TdJ9lnpuFggJV2R6Vc/A=";
            string queueName = req.Query["queueName"];
            var policy = new RetryExponential(
                                minimumBackoff: TimeSpan.FromSeconds(10),
                                maximumBackoff: TimeSpan.FromSeconds(30),
                                maximumRetryCount: 3);

            ISessionClient client = new SessionClient(connectionString, queueName, ReceiveMode.PeekLock, policy);

            log.LogInformation("Waiting for new message");
            string sessionIdParam = req.Query["sessionId"];

            #region variables
            IMessageSession messageSession = await client.AcceptMessageSessionAsync(sessionIdParam);

            var keepPolling = true;
            var isFirstMessage = true;
            var expectedNoMessages = 0;
            Message[] messages = null;
            var messagesReceived = 0;
            var sessionId = string.Empty;
            var correlationId = string.Empty;
            Stream fullMessageBodyStream = null;
            #endregion

            while (keepPolling)
            {
                var message = await messageSession.ReceiveAsync();

                if (message == null)
                    continue;

                if (isFirstMessage)
                {
                    log.LogInformation("Receiving first message");
                    expectedNoMessages = (int)message.UserProperties["TotalMessages"];
                    messages = new Message[expectedNoMessages];
                    isFirstMessage = false;
                    sessionId = message.SessionId;
                    correlationId = message.CorrelationId;
                }

                var messageNo = (int)message.UserProperties["MessageNo"];
                var messageIndex = messageNo - 1;
                log.LogInformation(string.Format("Receiving message {0}", messageNo));
                messages[messageIndex] = message;
                messagesReceived++;

                if (messagesReceived == expectedNoMessages)
                    keepPolling = false;
            }

            //Rebuild Object
            fullMessageBodyStream = ReconstructMessageBody(messages);
            var result = ReadStreamData(fullMessageBodyStream);

            log.LogInformation($"SessionId = {sessionId}.");
            log.LogInformation($"result = {result}.");

            var completeTasks = new List<Task>();
            var lockToken = new List<string>();

            foreach (var message in messages)
            {
                lockToken.Add(message.SystemProperties.LockToken);
                //await messageSession.CompleteAsync(message.SystemProperties.LockToken);
            }

            completeTasks.Add(messageSession.CompleteAsync(lockToken));

            Task.WaitAll(completeTasks.ToArray());

            await messageSession.CloseAsync();

            return new OkObjectResult("Ok");
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
    }
}
