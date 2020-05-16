using MongoDB.Driver;
using System;
using System.Text.Json;

namespace EventsConsumers
{
   
    public class OrderCreatedEventHandler
    {
        public MongoClient mongo;
        private readonly string JsonString;
        public OrderCreatedEventHandler(MongoClient mongo,string message)
        {
            JsonString = message;
            this.mongo = mongo;
        }

        public void ProcessEvent()
        {
            OrderCreated orderCreated = JsonSerializer.Deserialize<OrderCreated>(JsonString);
            //step 1 : check if the seller's document is there in the 

            var db = mongo.GetDatabase("SellersDatabase");

            var collection = db.GetCollection<Object>("SellersOrders");

            var findSellerFilter = Builders<Object>.Filter.Eq("SellerId", orderCreated.SellerId);

            var sellerDocument = (Seller)collection.Find<Object>(findSellerFilter).FirstOrDefault();

            if (sellerDocument == null)
            {
                Seller seller = new Seller();
                seller.SellerId = orderCreated.SellerId;
                collection.InsertOne(seller);
                sellerDocument = (Seller)collection.Find<Object>(findSellerFilter).FirstOrDefault();
            }
            
            var findOrderFilter = Builders<Object>.Filter.Eq("OrderId", orderCreated.OrderId);
            var orderDocument = (Order)collection.Find<Object>(findOrderFilter).FirstOrDefault();

            if (orderDocument == null)
            {
                try
                {
                    // adding a new order
                    Order order = new Order();
                    order.OrderId = orderCreated.OrderId;
                    order.OrderDate = orderCreated.OrderDate;
                    order.PromisedShipDate = orderCreated.PromisedShipDate;
                    order.PromisedDeliveryDate = orderCreated.PromisedDeliveryDate;
                    order.SellerId = sellerDocument.Id.ToString();
                    // insert
                    collection.InsertOne(order);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                // update information in the order
                var updmanyresult = collection.UpdateMany(
                                Builders<Object>.Filter.Eq("OrderId", orderCreated.OrderId),
                                Builders<Object>.Update
                                .Set("OrderDate", orderCreated.OrderDate)
                                .Set("PromisedShipDate", orderCreated.PromisedDeliveryDate)
                                .Set("PromisedDeliveryDate", orderCreated.PromisedDeliveryDate));           
            }
            
        }
    }

    

   

   

    
}