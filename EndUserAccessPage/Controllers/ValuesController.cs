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
       
        // GET api/values 
        // to return seller health status
        [HttpGet("{orderDate}")]
        public ActionResult<IEnumerable<String>> Get(DateTime orderDate)
        {
            String res = fetchFromDB1(orderDate);
            return new String[] { res };
        }
          
        // GET api/values/5
        // to return seller metrics for an order
        [HttpGet("{id}")] // id is orderID
        public ActionResult<IEnumerable<String>> Get(String id)
        {
            String healthstatus = fetchFromDB2(id);
            return new String[] {DeliveryEstimateMetric , shippingEstimateMetric  , cancellationMetric , returnMetric };
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
        public  DateTime orderDate;
        public  String DeliveryEstimateMetric = "false";
        public  String shippingEstimateMetric = "false";
        public  String cancellationMetric = "false";
        public  String returnMetric = "false";
        public  String sellerHealthStatus = "NOT_HEALTHY";
        public String DeliveryEstimateMetricFrOrder = "false";
        public String shippingEstimateMetricFrOrder = "false";
        public String cancellationMetricFrOrder = "false";
        public String returnMetricFrOrder = "false";
        public String sellerHealthStatusFrOrder = "NOT_HEALTHY";
        //fetch from db
        private  String fetchFromDB1(DateTime orderDate)
        {
            //to calculate seller health status for given order date
            var dbClient = new MongoClient("mongodb://127.0.0.1:27017");
            IMongoDatabase db = dbClient.GetDatabase("newdemodb");
            var coll = db.GetCollection<BsonDocument>("demodbcollection");
            var documents = coll.Find(new BsonDocument()).ToList();
            var totalDocuments = coll.Find(new BsonDocument()).CountDocuments();
            var filter1 = Builders<BsonDocument>.Filter.Gte("PromisedShipDate", "ActualShipDate");
            var filter2 = Builders<BsonDocument>.Filter.Gte("PromisedDeliveryDate", "ActualDeliveryDate");
            var filter3 = Builders<BsonDocument>.Filter.Eq("isReturned", true);
            var filter4 = Builders<BsonDocument>.Filter.Eq("OrderDate", orderDate);
            var documentsWithFilter1 = coll.Find(filter1).ToList();
            var documentsWithFilter2 = coll.Find(filter2).ToList();
            var documentsWithFilter3 = coll.Find(filter3).ToList();
            var documentsWithFilter4 = coll.Find(filter4).ToList();
            long totalDocumentsWithFilter4 = coll.Find(filter4).CountDocuments();
            long DrEstimateMetric = coll.Find(filter4 & filter2).CountDocuments(); //promised delivery estimate metric
            long shpEstimateMetric = coll.Find(filter4 & filter1).CountDocuments(); //promised shipment estimate metric
            long retEstimateMetric = coll.Find(filter4 & filter3).CountDocuments(); // return meric
            long drPercentage = DrEstimateMetric / totalDocumentsWithFilter4;
            if(drPercentage>=98)
            {
                DeliveryEstimateMetric = "true";
            }
            else
            {
                DeliveryEstimateMetric = "false";
            }
            long shpPercentage = shpEstimateMetric / totalDocumentsWithFilter4;
            if(shpPercentage>=97)
            {
                shippingEstimateMetric = "true";
            }
            else
            {
                shippingEstimateMetric = "false";
            }
            long retPercentage = retEstimateMetric / totalDocumentsWithFilter4;
            if(retPercentage<=1)
            {
                returnMetric = "false";
            }
            if (shippingEstimateMetric.Equals("true") && DeliveryEstimateMetric.Equals("true") && returnMetric.Equals("false"))
            {
                sellerHealthStatus = "HEALTHY";
            }
            else
            {
                sellerHealthStatus = "NOT_HEALTHY";
            }
            return sellerHealthStatus;
        }
        private  String fetchFromDB2(String id)
        {
            //seller performance metrics for a given orderid
            var dbClient = new MongoClient("mongodb://127.0.0.1:27017");
            IMongoDatabase db = dbClient.GetDatabase("newdemodb");
            var coll = db.GetCollection<BsonDocument>("demodbcollection");     
            var filter1 = Builders<BsonDocument>.Filter.Gte("PromisedShipDate", "ActualShipDate");
            var filter2 = Builders<BsonDocument>.Filter.Gte("PromisedDeliveryDate", "ActualDeliveryDate");
            var filter3 = Builders<BsonDocument>.Filter.Eq("isReturned", false);
            //every document has unique order id
            var filter4 = Builders<BsonDocument>.Filter.Eq("OrderId", id);
            var document = coll.Find(filter4);
            var documentsWithFilter1 = coll.Find(filter1).ToList();
            var documentsWithFilter2 = coll.Find(filter2).ToList();
            var documentsWithFilter3 = coll.Find(filter3).ToList();
            foreach(BsonDocument doc in documentsWithFilter1)
            {
                if(document.Equals(doc))
                {
                    shippingEstimateMetricFrOrder = "true";
                }
            }
            foreach (BsonDocument doc in documentsWithFilter2)
            {
                if (document.Equals(doc))
                {
                    DeliveryEstimateMetricFrOrder = "true";
                }
            }
            foreach (BsonDocument doc in documentsWithFilter3)
            {
                if (document.Equals(doc))
                {
                    returnMetricFrOrder = "false";
                }
            }
            if(shippingEstimateMetricFrOrder.Equals("true")&&DeliveryEstimateMetricFrOrder.Equals("true")&&returnMetricFrOrder.Equals("false"))
            {
                sellerHealthStatusFrOrder = "HEALTHY";
            }
            return sellerHealthStatusFrOrder;
        }
}
}
