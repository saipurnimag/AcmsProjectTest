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
        public EventConsumer(ConcurrentQueue<Confluent.Kafka.ConsumeResult<Confluent.Kafka.Ignore, string>> q)
        {
            this.q = q;
        }

        public void Start()
        {
            Thread t1 = new Thread(()=>this.ConsumeEventsFromKafka());
            t1.Start();
        }
        public void ConsumeEventsFromKafka()
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
                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true; // prevent the process from terminating.
                    cts.Cancel();
                };
                try
                {
                    while (true)
                    {
                        if (ThreadPool.ThreadCount != 0)
                        {
                            //queue full
                            try
                            {
                                var cr = consumer.Consume(cts.Token);
                                Console.WriteLine(
                                    $"Enqueuing this '{cr.Message.Value}' at: '{cr.TopicPartitionOffset}'.");
                                queue.Enqueue(cr);
                                continue;
                            }
                            catch (ConsumeException e)
                            {
                                Console.WriteLine($"Error occurred :{e.Error.Reason}");
                            }
                        }
                        else
                        {
                            continue;
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