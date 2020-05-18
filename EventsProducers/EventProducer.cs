using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Confluent.Kafka;

namespace EventsProducers
{
     public class EventProducer
    {
	    public  const int v = 5;
        public static int i;

        public static DateTime CreateRandomDate(Random gen, int range)
        {
            DateTime randomDate = DateTime.Today.AddDays(-gen.Next(range));
            //string date = DateTime.Now.ToString("dd/MM/yyyy");
            // var x = DateTime.Now.ToShortDateString();
            //string dt = String.Format("{0:dd/MM/yyyy}", randomDate);
            return randomDate;
        }
        public static void date()
        {
            DateTime[] dateTime = new DateTime[v];

            DateTime[] dates = dateTime;

            try
            {
                Random gen = new Random();
                int range = 50 * 365;
                for (i = 0; i < 5; i++)
                {

                    Console.WriteLine(CreateRandomDate(gen, range));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public static void ProduceEvents()
        {
            List<int> eventorder;
            eventorder = new List<int>();
            eventorder.Add(1); //Order Created
            eventorder.Add(2); //Order Shipped
            eventorder.Add(3); //Order Delivered
            eventorder.Add(4); //Order Cancelled
            eventorder.Add(5); //Order Returned
            var conf = new ProducerConfig {BootstrapServers = "localhost:9092"};


            Action<DeliveryReport<Null, string>> handler = r =>
                Console.WriteLine(r.Error.IsError
                    ? $"Delivery Error: {r.Error.Reason}"
                    : $"Delivered message to {r.TopicPartitionOffset}");

            using (var p = new ProducerBuilder<Null, string>(conf).Build())
            {
                var random = new Random();
                while(true)
                {
                    string sellerId = Guid.NewGuid().ToString();
                    //seller can repeat

                    //userId is randomly generated
                    //The events should also be generated in random order
                    //shuffle the list
                    //7 orders per seller 
                    for (int i = 0; i < 7; ++i)
                    {
                        eventorder = eventorder.OrderBy(x => Guid.NewGuid()).ToList();
                        var orderId = Guid.NewGuid().ToString();
                        foreach (var num in eventorder)
                        {

                            var message = "";
                            switch (num)
                            {
                                case 1:
                                    //Order Created
                                    var orderCreated = new OrderCreated();
                                    message = orderCreated.CreateJson(orderId, sellerId);
                                    Console.WriteLine("order-created " + message);
                                    p.Produce("order-created", new Message<Null, string> { Value = message }, handler);
                                    break;
                                case 2:
                                    //Order Shipped

                                    var orderShipped = new OrderShipped();
                                    message = orderShipped.createJson(orderId, sellerId);
                                    Console.WriteLine("order-shipped " + message);
                                    p.Produce("order-shipped", new Message<Null, string> { Value = message }, handler);
                                    break;
                                case 3:
                                    //Order Delivered
                                    var orderDelivered = new OrderDelivered();
                                    message = orderDelivered.CreateJson(orderId, sellerId);
                                    Console.WriteLine("order-delivered " + message);
                                    p.Produce("order-delivered", new Message<Null, string> { Value = message }, handler);
                                    break;
                                case 4:
                                    //Order Cancelled
                                    var orderCancelled = new OrderCancelled();
                                    message = orderCancelled.createJson(orderId, sellerId, "customer",
                                        "The reason for cancellation");
                                    Console.WriteLine("order-cancelled " + message);
                                    p.Produce("order-cancelled", new Message<Null, string> { Value = message }, handler);
                                    break;
                                case 5:
                                    //Order Returned
                                    var orderReturned = new OrderReturned();
                                    message = orderReturned.createJson(orderId, sellerId);
                                    Console.WriteLine("order-returned " + message);
                                    p.Produce("order-returned", new Message<Null, string> { Value = message }, handler);
                                    break;
                            }

                            Thread.Sleep(1000);
                        }
                        Thread.Sleep(1000);
                    }
                }
            }
        }
    }
}