using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Confluent.Kafka;

namespace EventsConsumers
{
    public class EventConsumer
    {
        private readonly int maxQueueSize;
        private readonly ConcurrentQueue<ConsumeResult<Ignore, string>> q;

        public EventConsumer(ConcurrentQueue<ConsumeResult<Ignore, string>> q,
            int maxQueueSize)
        {
            this.q = q;
            this.maxQueueSize = maxQueueSize;
        }

        public void Start(CancellationToken ct)
        {
            var t1 = new Thread(() => ConsumeEventsFromKafka(ct));
            t1.Start();
        }

        public void ConsumeEventsFromKafka(CancellationToken ct)
        {
            var queue = q;
            IEnumerable<string> topics = new[]
                {"order-created", "order-shipped", "order-delivered", "order-cancelled", "order-returned"};
            var conf = new ConsumerConfig
            {
                GroupId = "test-consumer-group",
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            using (var consumer = new ConsumerBuilder<Ignore, string>(conf).Build())
            {
                consumer.Subscribe(topics);
                try
                {
                    while (!ct.IsCancellationRequested)
                        // queue full
                        if (queue.Count < maxQueueSize)
                            try
                            {
                                var cr = consumer.Consume(ct);
                                Console.WriteLine(
                                    $"Enqueuing this '{cr.Message.Value}' at: '{cr.TopicPartitionOffset}'.");
                                queue.Enqueue(cr);
                            }
                            catch (ConsumeException e)
                            {
                                Console.WriteLine($"Error occurred :{e.Error.Reason}");
                            }
                }
                catch (OperationCanceledException)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    consumer.Close();
                }
            }
        }
    }
}