using System;
using System.Collections.Generic;
using Confluent.Kafka;
using System.Linq;
namespace KafkaBananaStore
{
    public class EventProducer
    {
        public void ProduceEvents(int start, int stop)
        {
            List<Int32> eventorder;
            eventorder = new List<int>();
            eventorder.Add(1); //Order Created
            eventorder.Add(2); //Order Shipped
            eventorder.Add(3); //Order Delivered
            eventorder.Add(4); //Order Cancelled
            eventorder.Add(5); //Order Returned
            var conf = new ProducerConfig {BootstrapServers = "localhost:9092"};

            Action<DeliveryReport<Null, string>> handler = r =>
                Console.WriteLine(!r.Error.IsError
                    ? $"Delivered message to {r.TopicPartitionOffset}"
                    : $"Delivery Error: {r.Error.Reason}");

            using (var p = new ProducerBuilder<Null, string>(conf).Build())
            {
                Random random = new Random();

                for (int i = start; i <= stop; ++i)
                {
                    int sellerId = random.Next(start, stop+1);
                    //userId is randomly generated
                    //The events should also be generated in random order
                    //shuffle the list
                    eventorder = eventorder.OrderBy(x => Guid.NewGuid()).ToList();
                    Console.WriteLine(eventorder);
                    foreach (int num in eventorder)
                    {
                        string message = "";
                        switch (num)
                        {
                            case 1:
                                //Order Created
                                OrderCreated orderCreated = new OrderCreated();
                                message = orderCreated.CreateJson(i, sellerId);
                                //Console.WriteLine("Order Created Orderid: "+i+" SellerId: "+sellerId);
                                p.Produce("order-created", new Message<Null, string> {Value = message}, handler);
                                break;
                            case 2:
                                //Order Shipped
                                OrderShipped orderShipped = new OrderShipped();
                                message = orderShipped.createJson(i, sellerId);
                                //Console.WriteLine("Order Shipped Orderid: "+i+" SellerId: "+sellerId);
                                p.Produce("order-shipped", new Message<Null, string> {Value = message}, handler);
                                break;
                            case 3:
                                //Order Delivered
                                OrderDelivered orderDelivered = new OrderDelivered();
                                message = orderDelivered.CreateJson(i, sellerId);
                                //Console.WriteLine("Order Delivered Orderid: "+i+" SellerId: "+sellerId);
                                p.Produce("order-delivered", new Message<Null, string> {Value = message}, handler);
                                break;
                            case 4:
                                //Order Cancelled
                                OrderCancelled orderCancelled = new OrderCancelled();
                                message = orderCancelled.createJson(i, sellerId, "customer",
                                    "The reason for cancellation");
                                //Console.WriteLine("Order Cancelled Orderid: "+i+" SellerId: "+sellerId);
                                p.Produce("order-cancelled", new Message<Null, string> {Value = message}, handler);
                                break;
                            case 5:
                                //Order Returned
                                OrderReturned orderReturned = new OrderReturned();
                                message = orderReturned.createJson(i, sellerId);
                                //Console.WriteLine("Order Returned Orderid: "+i+" SellerId: "+sellerId);
                                p.Produce("order-returned", new Message<Null, string> {Value = message}, handler);
                                break;
                            default:
                                break;
                        }

                        
                    }
                }

                // wait for up to 10 seconds for any inflight messages to be delivered.
                p.Flush(TimeSpan.FromSeconds(10));
            }
        }
    }
}