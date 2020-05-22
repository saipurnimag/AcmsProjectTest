using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace EndUserAccessPage.Main

{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : Program
    {
        String res = fetchFromDB();
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<String>> Get()
        {
            
            return new String[] { res };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        //fetch from db
        private static String fetchFromDB()
        {
            bool DeliveryEstimateMetric = false;
            bool shippingEstimateMetric = false;
            bool cancellationMetric = false;
            bool returnMetric = false;
            var orderID = "";
            orderID = Console.ReadLine();
            String sellerHealthStatus = "NOT_HEALTHY";
            var dbClient = new MongoClient("mongodb://127.0.0.1:27017");
            IMongoDatabase db = dbClient.GetDatabase("newdemodb");
            var coll = db.GetCollection<BsonDocument>("demodbcollection");
            var documents = coll.Find(new BsonDocument()).ToList();
            var totalDocuments = coll.Find(new BsonDocument()).CountDocuments();
            //seller performance metrics per order
            var filter1 = Builders<BsonDocument>.Filter.Gte("PromisedShipDate","ActualShipDate");
            var filter2 = Builders<BsonDocument>.Filter.Gte("PromisedDeliveryDate", "ActualDeliveryDate");
            var filter3 = Builders<BsonDocument>.Filter.Eq("isReturned", false);
            var filter4 = Builders<BsonDocument>.Filter.Eq("OrderId", orderID)& filter1 & filter2 & filter3;
            //assuming every document has unique order id
            var doc = coll.Find(filter4).CountDocuments();
            if(doc==1)
            {
                Console.WriteLine("Seller Performance For given OrderID is Healthy");
            }
            else
            {
                Console.WriteLine("Seller Performance For given OrderID is Not Healthy");
            }
            var documents1 = coll.Find(filter1).CountDocuments();
            var documents2 = coll.Find(filter2).CountDocuments();
            var documents3 = coll.Find(filter3).CountDocuments();
            foreach (BsonDocument docs in documents)
            {
                
            }
            return sellerHealthStatus;
        }
}
}
