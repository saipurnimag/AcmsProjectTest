using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EventsConsumers.Models
{
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
