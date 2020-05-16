using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Confluent.Kafka;

namespace EventsProducers
{
    #region Models

    public abstract class Event
    {
        public string OrderId { get; set; }
        public string SellerId { get; set; }
    }

    public class OrderCreated : Event
    {
        public DateTime OrderDate { get; set; }
        public DateTime PromisedShipDate { get; set; }
        public DateTime PromisedDeliveryDate { get; set; }

        public string CreateJson(string orderId, string sellerId)
        {
            var date1 = new DateTime();
            var obj = new OrderCreated();
            obj.OrderId = orderId;
            obj.OrderDate = date1;
            obj.PromisedShipDate = date1;
            obj.PromisedDeliveryDate = date1;
            obj.SellerId = sellerId;
            return JsonSerializer.Serialize(obj);
        }
    }

    public class OrderShipped : Event
    {
        public DateTime ActualShipDate { get; set; }

        public string createJson(string orderID, string sellerID)
        {
            var date1 = new DateTime();
            var obj = new OrderShipped();
            obj.OrderId = orderID;
            obj.ActualShipDate = date1;
            obj.SellerId = sellerID;
            return JsonSerializer.Serialize(obj);
        }
    }


    public class OrderDelivered : Event
    {
        public DateTime ActualDeliveryDate { get; set; }

        public string CreateJson(string orderID, string sellerId)
        {
            var date1 = new DateTime();
            var obj = new OrderDelivered();
            obj.OrderId = orderID;
            obj.SellerId = sellerId;
            obj.ActualDeliveryDate = date1;
            return JsonSerializer.Serialize(obj);
        }
    }

    public class OrderCancelled : Event
    {
        public string CancellationOrigin, CancellationReason;

        public string createJson(string orderId, string sellerId, string cancellationOrigin, string cancellationReason)
        {
            var date1 = new DateTime();
            var obj = new OrderCancelled();
            obj.OrderId = orderId;
            obj.SellerId = sellerId;
            obj.CancellationOrigin = cancellationOrigin;
            obj.CancellationReason = cancellationReason;
            return JsonSerializer.Serialize(obj);
        }
    }

    public class OrderReturned : Event
    {
        public string createJson(string orderId, string sellerId)
        {
            var obj = new OrderReturned();
            obj.OrderId = orderId;
            obj.SellerId = sellerId;
            return JsonSerializer.Serialize(obj);
        }
    }

    #endregion

    public class EventProducer
    {
        public static void ProduceEvents(int start, int stop)
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
                
                // wait for up to 10 seconds for any inflight messages to be delivered.
                p.Flush(TimeSpan.FromSeconds(10));
            }
        }
    }
}