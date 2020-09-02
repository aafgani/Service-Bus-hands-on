using MessageSender.Model;
using MessageSender.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MessageSender
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            //IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            //configBuilder = configBuilder.SetBasePath(Directory.GetCurrentDirectory());
            //configBuilder = configBuilder.AddJsonFile("appSetting.json");
            //IConfiguration config = configBuilder.Build();

            //AppConfig appConfig = new AppConfig();
            //config.Bind("AppConfig", appConfig);

            //var serviceCollection = new ServiceCollection()
            //       .Configure<AppConfig>(x => config.GetSection("AppConfig").Bind(x))
            //       .AddSingleton<IMessageSender, QueueMsgSender>()
            //       .AddScoped<IMessageSender, TopicMessageSender>()
            //       .BuildServiceProvider();

            //.AddHttpClient<FunctionClientService>(c =>
            // {
            //     c.BaseAddress = new Uri(appConfig.HttpTriggeredFunctionBaseUrl);
            // });

            List<int> list = new List<int> { 1,2,3,4,5,6,7,8,9,10};
            var a = list.Take(5);
            var b = list.Skip(5);
            var c = list.Skip(5).Take(2);
            var d = list.Skip(7).Take(10);


            bool Ok = true;
            while (Ok)
            {
                Console.WriteLine("======================================================");
                Console.WriteLine("Press ENTER key to exit after sending all the messages.");
                Console.WriteLine("======================================================");
                Console.WriteLine("1. Service Bus Queue");
                Console.WriteLine("2. Service Bus Topic");                

                string val;
                int mode;
                MessageSender messageSender;

                Console.Write("\nChoose Mode [1/2]: ");
                val = Console.ReadLine();
                mode = Convert.ToInt32(val);
                if (mode==1)
                {
                    Console.WriteLine("Mode selected : Service Bus Queue");
                    Console.WriteLine("A. Small Message");
                    Console.WriteLine("B. Large Message");
                    Console.WriteLine("C. Scheduled Message");
                    Console.Write("\nChoose size : ");
                    val = Console.ReadLine();

                    if (val.Equals("A"))
                    {
                        messageSender = new QueueMsgSender();
                    }
                    else if(val.Equals("B"))
                    {
                        messageSender = new QueueLargeMessageSender();
                    }
                    else
                    {
                        messageSender = new SchedulledMessageSender();
                    }

                    Console.Write("\nEnter Message: ");
                }
                else 
                {
                    Console.WriteLine("Mode selected : Service Bus Topic");
                    messageSender = new TopicMessageSender();
                    Console.Write("\nEnter Message [Name,Count]: ");
                }

                val = Console.ReadLine();
                Console.WriteLine("\nYour messgae: {0}", val);
                
                await messageSender.SendMessagesAsync(val);

                Console.Write("\nContinue [Y/N] ? ");
                val = Console.ReadLine();

                if (val.Equals("N"))
                {
                    Ok = false;
                }
                Console.WriteLine();
            }
        }
    }
}
