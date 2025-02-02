using System;
using System.Threading.Tasks;
using System.Xml.Xsl;
using Azure.Messaging.ServiceBus;

namespace MessageReader
{

    class Program
    {

        static string ServiceBusConnectionString = "";

        static string QueueName = "messagequeue";

        static ServiceBusClient client = default;

        static ServiceBusProcessor processor = default;

        static async Task MessageHandler(ProcessMessageEventArgs args)
        {

            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body}");
            await args.CompleteMessageAsync(args.Message);
        }

        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        static async Task Main(string[] args)
        {
            client = new ServiceBusClient(ServiceBusConnectionString);
            processor = client.CreateProcessor(QueueName, new ServiceBusProcessorOptions());

            try
            {
                processor.ProcessMessageAsync += MessageHandler;
                processor.ProcessErrorAsync += ErrorHandler;
                await processor.StartProcessingAsync();
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                await processor.StopProcessingAsync();
            }
            finally
            {
                await processor.DisposeAsync();
                await client.DisposeAsync();
            }
        }
    }
}
