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
        public static void Main(string[] args)
        {
            queue = new ConcurrentQueue<Confluent.Kafka.ConsumeResult<Confluent.Kafka.Ignore,string>>();
            var consumer = new EventConsumer(queue);
            var dispatcher = new EventDispatcher(queue);
           
            consumer.Start();
            dispatcher.Start();
        }
    }
}