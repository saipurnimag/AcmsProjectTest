using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;

namespace KafkaBananaStore
{
    class Program
    {
        public static void Main(string[] args)
        {
            var conf = new ProducerConfig { BootstrapServers = "localhost:9092" };

            Action<DeliveryReport<Null, string>> handler = r =>
                Console.WriteLine(!r.Error.IsError
                    ? $"Delivered message to {r.TopicPartitionOffset}"
                    : $"Delivery Error: {r.Error.Reason}");

            using (var p = new ProducerBuilder<Null, string>(conf).Build())
            {
                int i = 1;
                while(true)
                {
                    P obj = new P();
                    p.Produce("order-confirmed", new Message<Null, string>{Value = obj.createJSON(i)}, handler);
                    System.Threading.Thread.Sleep((1000));
                }

                // wait for up to 10 seconds for any inflight messages to be delivered.
                p.Flush(TimeSpan.FromSeconds(10));
            }
        }
    }
}
