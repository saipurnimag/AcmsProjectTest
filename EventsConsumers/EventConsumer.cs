using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Confluent.Kafka;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;


namespace EventsConsumers
{
    public class EventConsumer
    {
        private ConcurrentQueue<Confluent.Kafka.ConsumeResult<Confluent.Kafka.Ignore, string>> q;
        private int maxQueueSize;
        public EventConsumer(ConcurrentQueue<Confluent.Kafka.ConsumeResult<Confluent.Kafka.Ignore, string>> q,
            int maxQueueSize)
        {
            this.q = q;
            this.maxQueueSize = maxQueueSize;
        }

        public void Start(CancellationToken ct)
        {
            Thread t1 = new Thread(()=>this.ConsumeEventsFromKafka(ct));
            t1.Start(ct);
        }
        public void ConsumeEventsFromKafka(CancellationToken ct)
        {
            ConcurrentQueue<Confluent.Kafka.ConsumeResult<Confluent.Kafka.Ignore, string>> queue = q;
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
                    while (true)
                    {
                        // queue full
                        if (queue.Count < maxQueueSize)
                        {
                            try
                            {
                                ConsumeResult<Ignore, string> cr = consumer.Consume(ct);
                                Console.WriteLine(
                                    $"Enqueuing this '{cr.Message.Value}' at: '{cr.TopicPartitionOffset}'.");
                                queue.Enqueue(cr);
                            }
                            catch (ConsumeException e)
                            {
                                Console.WriteLine($"Error occurred :{e.Error.Reason}");
                            }
                        }
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