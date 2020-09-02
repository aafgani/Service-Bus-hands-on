namespace MessageReceiver
{
    class Program
    {
        static void Main(string[] args)
        {
            IMessageReader program = new MessageReaderWithMsSvcBusMessaging();
            program.ReadMessage();
        }
    }
}
