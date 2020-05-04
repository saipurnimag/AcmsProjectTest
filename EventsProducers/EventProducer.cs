using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Confluent.Kafka;

namespace EventsProducers
{
     #region Models

    public abstract class Event
    {
        public String OrderId { get; set; }
        public int SellerId { get; set; }
    }

    public class OrderCreated : Event
    {
        public DateTime OrderDate { get; set; }
        public DateTime PromisedShipDate { get; set; }
        public DateTime PromisedDeliveryDate { get; set; }
        public string CreateJson(string orderId,int sellerId)
        {
            DateTime date1 = new DateTime();
            OrderCreated obj = new OrderCreated();
            obj.OrderId = orderId;
            obj.OrderDate = date1;
            obj.PromisedShipDate = date1;
            obj.PromisedDeliveryDate = date1;
            obj.SellerId = sellerId;
            return JsonSerializer.Serialize<OrderCreated>(obj);
        }
    }
    
   public class OrderShipped : Event
    {
        public DateTime ActualShipDate { get; set; }

        public string createJson(string orderID, int sellerID)
        {
            DateTime date1 = new DateTime();
            OrderShipped obj = new OrderShipped();
            obj.OrderId = orderID;
            obj.ActualShipDate = date1;
            obj.SellerId = sellerID;
            return JsonSerializer.Serialize<OrderShipped>(obj);
        }
    }

   
    public class OrderDelivered : Event
    {
        public DateTime ActualDeliveryDate { get; set; }
        public string CreateJson(string orderID, int sellerId)
        {
            DateTime date1 = new DateTime();
            OrderDelivered obj = new OrderDelivered();
            obj.OrderId = orderID;
            obj.SellerId = sellerId;
            obj.ActualDeliveryDate = date1;
            return JsonSerializer.Serialize<OrderDelivered>(obj);
        }
    }

    public class OrderCancelled : Event
    {
        public string CancellationOrigin,CancellationReason;
        
        public string createJson(string orderId, int sellerId, String cancellationOrigin, String cancellationReason)
        {
            DateTime date1 = new DateTime();
            OrderCancelled obj = new OrderCancelled();
            obj.OrderId = orderId;
            obj.SellerId = sellerId;
            obj.CancellationOrigin = cancellationOrigin;
            obj.CancellationReason = cancellationReason;
            return JsonSerializer.Serialize<OrderCancelled>(obj);
        }
    }

    public class OrderReturned : Event
    {
        public string createJson(string orderId, int sellerId)
        {
            OrderReturned obj = new OrderReturned();
            obj.OrderId = orderId;
            obj.SellerId = sellerId;
            return JsonSerializer.Serialize<OrderReturned>(obj);
        }
    }
    #endregion

    public class EventProducer
    {
        public static void ProduceEvents(int start, int stop)
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
                Console.WriteLine(r.Error.IsError
                    ? $"Delivery Error: {r.Error.Reason}"
                    : $"Delivered message to {r.TopicPartitionOffset}");
            
            using (var p = new ProducerBuilder<Null, string>(conf).Build())
            {
                Random random = new Random();
                for (int i = start; i <= stop; ++i)
                {
                    int sellerId = random.Next(start, stop+1);
                    //seller can repeat
                    
                    //userId is randomly generated
                    //The events should also be generated in random order
                    //shuffle the list
                    eventorder = eventorder.OrderBy(x => Guid.NewGuid()).ToList();
                    
                    foreach (int num in eventorder)
                    {
                        string orderId = Guid.NewGuid().ToString();
                        string message = "";
                        switch (num)
                        {
                            case 1:
                                //Order Created
                                OrderCreated orderCreated = new OrderCreated();
                                message = orderCreated.CreateJson(orderId, sellerId);
                                p.Produce("order-created", new Message<Null, string> {Value = message}, handler);
                                break;
                            case 2:
                                //Order Shipped
                                OrderShipped orderShipped = new OrderShipped();
                                message = orderShipped.createJson(orderId, sellerId);
                                //Console.WriteLine("Order Shipped Orderid: "+i+" SellerId: "+sellerId);
                                p.Produce("order-shipped", new Message<Null, string> {Value = message}, handler);
                                break;
                            case 3:
                                //Order Delivered
                                OrderDelivered orderDelivered = new OrderDelivered();
                                message = orderDelivered.CreateJson(orderId, sellerId);
                                //Console.WriteLine("Order Delivered Orderid: "+i+" SellerId: "+sellerId);
                                p.Produce("order-delivered", new Message<Null, string> {Value = message}, handler);
                                break;
                            case 4:
                                //Order Cancelled
                                OrderCancelled orderCancelled = new OrderCancelled();
                                message = orderCancelled.createJson(orderId, sellerId, "customer",
                                    "The reason for cancellation");
                                //Console.WriteLine("Order Cancelled Orderid: "+i+" SellerId: "+sellerId);
                                p.Produce("order-cancelled", new Message<Null, string> {Value = message}, handler);
                                break;
                            case 5:
                                //Order Returned
                                OrderReturned orderReturned = new OrderReturned();
                                message = orderReturned.createJson(orderId, sellerId);
                                //Console.WriteLine("Order Returned Orderid: "+i+" SellerId: "+sellerId);
                                p.Produce("order-returned", new Message<Null, string> {Value = message}, handler);
                                break;
                            default:
                                break;
                        }
                        System.Threading.Thread.Sleep(250);
                    }
                }
                // wait for up to 10 seconds for any inflight messages to be delivered.
                p.Flush(TimeSpan.FromSeconds(10));
            }
        }
    }
}