using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventsConsumers
{
    class Order
    {

        [BsonId]
        public ObjectId Id { get; set; }


        public string OrderId { get; set; }
        public string SellerId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime PromisedShipDate { get; set; }
        public DateTime PromisedDeliveryDate { get; set; }
        public DateTime ActualShipDate { get; set; }
        public DateTime ActualDeliveryDate { get; set; }
        public string CancellationOrigin { get; set; }
        public string CancellationReason { get; set; }
        public bool isReturned { get; set; }

        public Order()
        {
            Id = ObjectId.GenerateNewId();
        }

    }
    class Seller
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string SellerId { get; set; }

        public Seller()
        {
            Id = ObjectId.GenerateNewId();
        }
    }
}
