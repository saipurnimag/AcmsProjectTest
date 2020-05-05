using System;
using System.Collections.Concurrent;
using System.Threading;
using Confluent.Kafka;

namespace EventsConsumers
{
    public class Program
    {
        public const int MaxQueueSize = 400;

        public static ConcurrentQueue<ConsumeResult<Ignore, string>> queue;

        public static void Main(string[] args)
        {
            queue = new ConcurrentQueue<ConsumeResult<Ignore, string>>();
            
            var cancellationTokenSource = new CancellationTokenSource();


            Console.CancelKeyPress += (_, eventArgs) =>
            {
                eventArgs.Cancel = true; // prevent the process from terminating.
                cancellationTokenSource.Cancel();
            };
            
            var consumer = new EventConsumer(queue, MaxQueueSize);
            var dispatcher = new EventDispatcher(queue);
            
            consumer.Start(cancellationTokenSource.Token);
            dispatcher.Start();
        }
    }
}