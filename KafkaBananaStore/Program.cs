using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;

namespace KafkaBananaStore
{
    class Program  
    {
        static void Main(string[] args)
        {
            var config = new ProducerConfig {BootstrapServers = "localhost:9092"};
            Action <DeliveryReport<Null,string>> handler = r => 
                Console.WriteLine(!r.Error.IsError 
                ? $"Delivered Message to {r.TopicPartitionOffset}"
                : $"Delivery Error: {r.Error.Reason}");

            using ( var producer = new ProducerBuilder<Null, string>(config).Build()){
                for (int i=0; i<100; ++i)
                {
                    producer.Produce("my-topic", new Message<Null, string> { Value = i.ToString() }, handler);
                }

                // wait for up to 10 seconds for any inflight messages to be delivered.
                producer.Flush(TimeSpan.FromSeconds(10));
            }
            
        }
    }
}
