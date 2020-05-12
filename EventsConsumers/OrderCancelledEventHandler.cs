using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace EventsConsumers
{
    #region Models

    public abstract class Event
    {
        public string OrderId { get; set; }
        public int SellerId { get; set; }
    }

    public class OrderCreated : Event
    {
        public DateTime OrderDate { get; set; }
        public DateTime PromisedShipDate { get; set; }
        public DateTime PromisedDeliveryDate { get; set; }

        public string CreateJson(string orderId, int sellerId)
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

        public string createJson(string orderID, int sellerID)
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

        public string CreateJson(string orderID, int sellerId)
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

        public string createJson(string orderId, int sellerId, string cancellationOrigin, string cancellationReason)
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
        public string createJson(string orderId, int sellerId)
        {
            var obj = new OrderReturned();
            obj.OrderId = orderId;
            obj.SellerId = sellerId;
            return JsonSerializer.Serialize(obj);
        }
    }

    #endregion
    public class OrderCancelledEventHandler
    {
        private readonly string JsonString;

        public OrderCancelledEventHandler(string message)
        {
            JsonString = message;
        }

        public void ProcessEvent(object stateInfo)
        {
            Console.WriteLine("Processing OrderCancelledEvent: " + JsonString);
        }
    }
}
