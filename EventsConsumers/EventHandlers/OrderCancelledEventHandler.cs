using MongoDB.Driver;
using System;
using System.Text.Json;

namespace EventsConsumers
{
    
    public class OrderCancelledEventHandler
    {
        private readonly string JsonString;
        public MongoClient mongo;
        public OrderCancelledEventHandler(MongoClient mongo,string message)
        {
            JsonString = message;
            this.mongo = mongo;
        }

        public void ProcessEvent()
        {
            OrderCancelled orderCancelled = JsonSerializer.Deserialize<OrderCancelled>(JsonString);
            var db = mongo.GetDatabase("SellersDatabase");

            var collection = db.GetCollection<Object>("SellersOrders");

            var findSellerFilter = Builders<Object>.Filter.Eq("SellerId", orderCancelled.SellerId);

            var sellerDocument = (Seller)collection.Find<Object>(findSellerFilter).FirstOrDefault();

            if (sellerDocument == null)
            {
                Seller seller = new Seller();
                seller.SellerId = orderCancelled.SellerId;
                collection.InsertOne(seller);
                sellerDocument = (Seller)collection.Find<Object>(findSellerFilter).FirstOrDefault();
            }
            var findOrderFilter = Builders<Object>.Filter.Eq("OrderId", orderCancelled.OrderId);
            var orderDocument = (Order)collection.Find<Object>(findOrderFilter).FirstOrDefault();

            if (orderDocument == null)
            {
                try
                {
                    // adding a new order
                    Order order = new Order();
                    order.OrderId = orderCancelled.OrderId;
                    order.CancellationOrigin = orderCancelled.CancellationOrigin;
                    order.CancellationReason = orderCancelled.CancellationReason;
                    order.SellerId = sellerDocument.Id.ToString();
                    // insert
                    collection.InsertOne(order);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                // update information in the order
                var updmanyresult = collection.UpdateMany(
                                Builders<Object>.Filter.Eq("OrderId", orderCancelled.OrderId),
                                Builders<Object>.Update
                                .Set("CancellationOrigin", orderCancelled.CancellationOrigin)
                                .Set("CancellationReason",orderCancelled.CancellationReason));
            }
        }
    }
}
