using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KafkaBananaStore
{
    #region Models

    public abstract class Event
    {
        public Int32 OrderId { get; set; }
        public int SellerId { get; set; }
    }

    public class OrderCreated : Event
    {
        public DateTime OrderDate { get; set; }
        public DateTime PromisedShipDate { get; set; }
        public DateTime PromisedDeliveryDate { get; set; }
        public string CreateJson(int orderId,int sellerId)
        {
            DateTime date1 = new DateTime();
            OrderCreated obj = new OrderCreated();
            obj.OrderId = orderId;
            obj.OrderDate = date1;
            obj.PromisedShipDate = date1;
            obj.PromisedDeliveryDate = date1;
            obj.SellerId = sellerId;
            Console.WriteLine(JsonSerializer.Serialize<OrderCreated>(obj));
            return JsonSerializer.Serialize<OrderCreated>(obj);
        }
    }
    
   public class OrderShipped : Event
    {
        public DateTime ActualShipDate { get; set; }

        public string createJson(int orderID, int sellerID)
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
        public string CreateJson(int orderID, int sellerId)
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
        
        public string createJson(int orderId, int sellerId, String cancellationOrigin, String cancellationReason)
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
        public string createJson(int orderId, int sellerId)
        {
            OrderReturned obj = new OrderReturned();
            obj.OrderId = orderId;
            obj.SellerId = sellerId;
            return JsonSerializer.Serialize<OrderReturned>(obj);
        }
    }
    #endregion
   

    class P
    {
        public string createJSON( int sellerID, int orderID)
        {
            return "";
        }
    }
}