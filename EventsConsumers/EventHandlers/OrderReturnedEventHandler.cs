using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace EventsConsumers
{
    public class OrderReturnedEventHandler
    {
        private readonly string JsonString;
        public MongoClient mongo;
        public OrderReturnedEventHandler(MongoClient mongo,string message)
        {
            JsonString = message;
            this.mongo = mongo;
        }

        public async void ProcessEvent()
        {
            OrderReturned orderReturned  = JsonSerializer.Deserialize<OrderReturned>(JsonString);
            var db = mongo.GetDatabase("SellersDatabase");

            var collection = db.GetCollection<Object>("SellersOrders");

            var findSellerFilter = Builders<Object>.Filter.Eq("SellerId", orderReturned.SellerId);

            var sellerDocument = (Seller)collection.Find<Object>(findSellerFilter).FirstOrDefault();

            if (sellerDocument == null)
            {
                Seller seller = new Seller();
                seller.SellerId = orderReturned.SellerId;
                collection.InsertOne(seller);
                sellerDocument = (Seller)collection.Find<Object>(findSellerFilter).FirstOrDefault();
            }
            try
            {
                var findOrderFilter = Builders<Object>.Filter.Eq("OrderId", orderReturned.OrderId);
                var orderDocument = (Order)collection.Find<Object>(findOrderFilter).FirstOrDefault();
                if (orderDocument == null)
                {
                    try
                    {
                        // adding a new order
                        Order order = new Order();
                        order.OrderId = orderReturned.OrderId;
                        order.isReturned = true;
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
                                    Builders<Object>.Filter.Eq("OrderId", orderReturned.OrderId),
                                    Builders<Object>.Update
                                    .Set("isReturned", true));
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
