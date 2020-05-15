using MongoDB.Driver;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace EventsConsumers
{
    #region Models

    public abstract class Event
    {
        public string OrderId { get; set; }
        public string SellerId { get; set; }
    }

    public class OrderCreated : Event
    {
        public DateTime OrderDate { get; set; }
        public DateTime PromisedShipDate { get; set; }
        public DateTime PromisedDeliveryDate { get; set; }

        public string CreateJson(string orderId, string sellerId)
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

        public string createJson(string orderID, string sellerID)
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

        public string CreateJson(string orderID, string sellerId)
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

        public string createJson(string orderId, string sellerId, string cancellationOrigin, string cancellationReason)
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
        public string createJson(string orderId, string sellerId)
        {
            var obj = new OrderReturned();
            obj.OrderId = orderId;
            obj.SellerId = sellerId;
            return JsonSerializer.Serialize(obj);
        }
    }

    #endregion
    public class OrderCreatedEventHandler
    {
        private readonly string JsonString;

        public static MongoClient mongo = new MongoClient();
        public OrderCreatedEventHandler(string message)
        {
            JsonString = message;
            
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