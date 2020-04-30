using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KafkaBananaStore
{
    public class OrderCreated{
        public Int32 OrderId{get; set;}
        public DateTime OrderDate{get; set;}
        public DateTime PromisedShipDate{get;set;}
        public DateTime PromisedDeliveryDate {get;set;}
    }
    class P  
    {
        public  string createJSON(int id)
        {
            DateTime date1 = new DateTime();
            OrderCreated obj = new OrderCreated();
            obj.OrderId = id;
            obj.OrderDate = date1;
            obj.PromisedShipDate = date1;
            obj.PromisedDeliveryDate = date1;
            Console.WriteLine(JsonSerializer.Serialize<OrderCreated>(obj));
            return JsonSerializer.Serialize<OrderCreated>(obj);
        }
    }
}
