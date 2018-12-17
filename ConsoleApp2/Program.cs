using System;
using System.Reflection;
using System.Threading.Tasks;
using NServiceBus;

namespace ConsoleApp2
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var endpointConfiguration = new EndpointConfiguration("Samples.Endpoint.ILMerge");

            // workaround to avoid 
            //  Can't find any behaviors/connectors for the root context (NServiceBus.Pipeline.ITransportReceiveContext)
            var types = Assembly.GetExecutingAssembly().GetTypes();
            var methodInfo = endpointConfiguration.GetType()
                .GetMethod("TypesToScanInternal", BindingFlags.Instance | BindingFlags.NonPublic);
            methodInfo.Invoke(endpointConfiguration, new object[] { types });

            endpointConfiguration.UseTransport<LearningTransport>();
            endpointConfiguration.UsePersistence<InMemoryPersistence>();

            try
            {
                await Endpoint.Start(endpointConfiguration);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.ReadLine();
        }
    }

    public class MyEvent : IEvent { }

    public class Handler : IHandleMessages<MyEvent>
    {
        public Task Handle(MyEvent message, IMessageHandlerContext context)
        {
            return Task.CompletedTask;
        }
    }
}
