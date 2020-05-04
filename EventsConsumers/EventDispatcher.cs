using System;
using System.Collections.Concurrent;
using System.Threading;

namespace EventsConsumers
{
    public class EventDispatcher
    {
        private ConcurrentQueue<Confluent.Kafka.ConsumeResult<Confluent.Kafka.Ignore, string>> q;
        public EventDispatcher(ConcurrentQueue<Confluent.Kafka.ConsumeResult<Confluent.Kafka.Ignore, string>> q)
        {
            this.q = q;
        }

        public void Start()
        {
            Thread thread = new Thread(() => this.ConsumeEventsFromQueue(q));
        }
        public void ConsumeEventsFromQueue(
            ConcurrentQueue<Confluent.Kafka.ConsumeResult<Confluent.Kafka.Ignore, string>> queue)
        {
            while (true)
            {
                if (queue.TryDequeue(out Confluent.Kafka.ConsumeResult<Confluent.Kafka.Ignore, string> e))
                {
                    Console.WriteLine($"I dequed this : {e.Message.Value}");
                }
                else
                {
                    continue;
                }
            }
        }
    }
}