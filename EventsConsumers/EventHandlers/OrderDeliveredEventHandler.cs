using MongoDB.Driver;
using System;
using System.Text.Json;

namespace EventsConsumers
{
    public class OrderDeliveredEventHandler
    {
        private readonly string JsonString;
        public MongoClient mongo;
        public OrderDeliveredEventHandler(MongoClient mongo,string message)
        {
            JsonString = message;
            this.mongo = mongo;
        }

        public void ProcessEvent()
        {
            OrderDelivered orderDelivered = JsonSerializer.Deserialize<OrderDelivered>(JsonString);
            var db = mongo.GetDatabase("SellersDatabase");

            var collection = db.GetCollection<Object>("SellersOrders");

            var findSellerFilter = Builders<Object>.Filter.Eq("SellerId", orderDelivered.SellerId);

            var sellerDocument = (Seller)collection.Find<Object>(findSellerFilter).FirstOrDefault();

            if (sellerDocument == null)
            {
                Seller seller = new Seller();
                seller.SellerId = orderDelivered.SellerId;
                collection.InsertOne(seller);
                sellerDocument = (Seller)collection.Find<Object>(findSellerFilter).FirstOrDefault();
            }
            var findOrderFilter = Builders<Object>.Filter.Eq("OrderId", orderDelivered.OrderId);
            var orderDocument = (Order)collection.Find<Object>(findOrderFilter).FirstOrDefault();

            if (orderDocument == null)
            {
                try
                {
                    // adding a new order
                    Order order = new Order();
                    order.OrderId = orderDelivered.OrderId;
                    order.ActualDeliveryDate = orderDelivered.ActualDeliveryDate;
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
                                Builders<Object>.Filter.Eq("OrderId", orderDelivered.OrderId),
                                Builders<Object>.Update
                                .Set("ActualDeliveryDate", orderDelivered.ActualDeliveryDate));
            }
        }
    }
}
