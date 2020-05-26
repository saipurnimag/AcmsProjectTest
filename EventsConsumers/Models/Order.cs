using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EventsConsumers
{
    class Order
    {

        [BsonId]
        public ObjectId Id { get; set; }


        public string OrderId { get; set; }
        public string SellerId { get; set; }
        public string OrderDate { get; set; }
        public string PromisedShipDate { get; set; }
        public string PromisedDeliveryDate { get; set; }
        public string ActualShipDate { get; set; }
        public string ActualDeliveryDate { get; set; }
        public string CancellationOrigin { get; set; }
        public string CancellationReason { get; set; }
        public bool isReturned { get; set; }

        public Order()
        {
            Id = ObjectId.GenerateNewId();
        }

    }
}
