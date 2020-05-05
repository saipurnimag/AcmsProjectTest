using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Confluent.Kafka;

namespace EventsConsumers
{
    public class Program
    {
        
        public static ConcurrentQueue<Confluent.Kafka.ConsumeResult<Confluent.Kafka.Ignore, string>> queue;
        public const int MaxQueueSize = 400;
        public static void Main(string[] args)
        {
            queue = new ConcurrentQueue<Confluent.Kafka.ConsumeResult<Confluent.Kafka.Ignore,string>>();
            CancellationTokenSource cancellationTokenSource  = new CancellationTokenSource();
            
                
            Console.CancelKeyPress += (_, eventArgs) =>
            {
                eventArgs.Cancel = true; // prevent the process from terminating.
                cancellationTokenSource.Cancel();
            };
            var consumer = new EventConsumer(queue, MaxQueueSize);
            var dispatcher = new EventDispatcher(queue);
            consumer.Start(cancellationTokenSource.Token);
            dispatcher.Start(cancellationTokenSource.Token);
        }
    }
}