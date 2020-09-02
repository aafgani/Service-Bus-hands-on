using Microsoft.ServiceBus.Messaging;
using System.IO;

namespace MessageReceiver.Helper
{
    public class ChunkedMessageBuilder
    {
        public static Stream ReconstructMessageBody(BrokeredMessage[] messages)
        {
            var messageBodyStream = new MemoryStream();
            var streamWriter = new StreamWriter(messageBodyStream);
            StreamReader streamReader = null;

            foreach (var message in messages)
            {
                streamReader = new StreamReader(message.GetBody<Stream>());
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
