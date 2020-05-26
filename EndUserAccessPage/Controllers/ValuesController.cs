using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace core.api.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        //requirement 2 in seller performance management system
        [HttpGet("{orderDate}/{id}")]
        public string Get(BsonDateTime orderDate, string id)
        {
            string res = fetchFromDB1(orderDate, id);
            return res;
        }

        // requirement 1 in seller performance management system
        // to return seller metrics per order
        [HttpGet] // id is orderID
        public IEnumerable<String> Get(String id)
        {
            String[] healthstatus = fetchFromDB2(id);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            return healthstatus;
        }

        //requirement 3 in seller performance management system
        //to calculate seller health status for all orders
         [HttpGet]
         [Route("get1/{id}")]
         public string GetAll(string id)
         {
            //id is seller id
            string sellerStatus = fetchFromDB3(id);
            return sellerStatus;
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
        private  string fetchFromDB1(BsonDateTime orderDate,string id)
        {
            //to calculate seller performance metrics per day across all orders
            //id is seller id
            string sellerHealthStatus = "";
            var dbClient = new MongoClient("mongodb://127.0.0.1:27017");
            IMongoDatabase db = dbClient.GetDatabase("newdemodb");
            var coll = db.GetCollection<BsonDocument>("demodbcollection");
            var filter1 = Builders<BsonDocument>.Filter.Eq("OrderDate", orderDate);
            var filter2 = Builders<BsonDocument>.Filter.Eq("SellerId", id);
            var documents = coll.Find(filter1 & filter2).ToList();
            long drEstimatePercentage;
            long shpEstimatePercentage;
            long retEstimatePercentage;
            long cancEstimatePercentage;
            long totalDocuments = documents.Count();
            long drDocsCount=0;
            long shpDocsCount=0;
            long retDocsCount=0;
            long cancDocsCount=0;
            string DeliveryEstimateMetric = "";
            string shippingEstimateMetric = "";
            string cancellationMetric = "";
            string returnMetric = "";
            foreach(BsonDocument doc in documents)
            {
                BsonDateTime actualShipDate = (BsonDateTime)doc.GetValue("ActualShipDate");
                BsonDateTime actualDeliveryDate = (BsonDateTime)doc.GetValue("ActualDeliveryDate");
                var filter3 = Builders<BsonDocument>.Filter.Gte("PromisedShipDate", actualShipDate);
                var filter4 = Builders<BsonDocument>.Filter.Gte("PromisedDeliveryDate", actualDeliveryDate);
                var doc1 = coll.Find(filter1 & filter2 & filter3).FirstOrDefault();
                if(doc1!=null)
                {
                    shpDocsCount++;
                }
                var doc2 = coll.Find(filter1 & filter2 & filter4).FirstOrDefault();
                if(doc2!=null)
                {
                    drDocsCount++;
                }
                var filter5 = Builders<BsonDocument>.Filter.Eq("isReturned", true);
                var doc3 = coll.Find(filter1 & filter2 & filter5).FirstOrDefault();
                if(doc3!=null)
                {
                    retDocsCount++;
                }
                var filter6 = Builders<BsonDocument>.Filter.Eq("CancellationReason", "seller");
                var doc4 = coll.Find(filter1 & filter2 & filter6).FirstOrDefault();
                if(doc4!=null)
                {
                    cancDocsCount++;
                }
                
            }
            drEstimatePercentage = (drDocsCount / totalDocuments)*100;
            shpEstimatePercentage = (shpDocsCount / totalDocuments)*100;
            retEstimatePercentage = (retDocsCount / totalDocuments)*100;
            cancEstimatePercentage = (cancDocsCount / totalDocuments)*100;
            if(drEstimatePercentage>=98)
            {
                DeliveryEstimateMetric = "True";
            }
            else
            {
                DeliveryEstimateMetric = "False";
            }
            if(shpEstimatePercentage>=97)
            {
                shippingEstimateMetric = "True";
            }
            else
            {
                shippingEstimateMetric = "False";
            }
            if(retEstimatePercentage<=1)
            {
                returnMetric = "False";
            }
            else
            {
                returnMetric = "True";
            }
            if(cancEstimatePercentage<=2)
            {
                cancellationMetric = "False";
            }
            else
            {
                cancellationMetric = "True";
            }
            if(DeliveryEstimateMetric.Equals("True")&&shippingEstimateMetric.Equals("True")&&returnMetric.Equals("False")&&cancellationMetric.Equals("False"))
            {
                sellerHealthStatus = "HEALTHY";
            }
            else
            {
                sellerHealthStatus = "NOT_HEALTHY";
            }
            return sellerHealthStatus;
        }



        public String[] fetchFromDB2(String id)
        {
            //to calculate seller performance metrics per order
            String DeliveryEstimateMetricFrOrder = "";
            String shippingEstimateMetricFrOrder = "";
            String cancellationMetricFrOrder = "false";
            String returnMetricFrOrder = "";
            String[] res = new String[4];
            var dbClient = new MongoClient("mongodb://127.0.0.1:27017");
            IMongoDatabase db = dbClient.GetDatabase("newdemodb");
            var coll = db.GetCollection<BsonDocument>("demodbcollection");
            var alldocuments = coll.Find(new BsonDocument()).ToList();
            //every document has unique order id
            var filter = Builders<BsonDocument>.Filter.Eq("OrderId", id);
            var document = coll.Find(filter).FirstOrDefault();
            if (document != null)
            {
                BsonDateTime actualShipDate = (BsonDateTime)document.GetValue("ActualShipDate");
                BsonDateTime actualDeliveryDate = (BsonDateTime)document.GetValue("ActualDeliveryDate");
                var filter1 = Builders<BsonDocument>.Filter.Gte("PromisedShipDate", actualShipDate);
                var filter2 = Builders<BsonDocument>.Filter.Gte("PromisedDeliveryDate", actualDeliveryDate);
                var filter3 = Builders<BsonDocument>.Filter.Eq("isReturned", false);
                var doc1 = coll.Find(filter&filter1).FirstOrDefault();
                if(doc1!=null)
                {
                    shippingEstimateMetricFrOrder = "True";  
                }
                else
                {
                    shippingEstimateMetricFrOrder = "False";
                }
                var doc2 = coll.Find(filter&filter2).FirstOrDefault();
                if(doc2!=null)
                {
                    DeliveryEstimateMetricFrOrder = "True";
                }
                else
                {
                    DeliveryEstimateMetricFrOrder = "False";
                }
                var doc3 = coll.Find(filter & filter3).FirstOrDefault();
                if(doc3!=null)
                {
                    returnMetricFrOrder = "false";
                }
                else
                {
                    returnMetricFrOrder = "true";
                }
            }
            res[0] = shippingEstimateMetricFrOrder;
            res[1] = DeliveryEstimateMetricFrOrder;
            res[2] = cancellationMetricFrOrder;
            res[3] = returnMetricFrOrder;
            return res;
        }


        //to calculate seller health status
        string fetchFromDB3(string id)
        {
            string sellerStatus = "";
            var dbClient = new MongoClient("mongodb://127.0.0.1:27017");
            IMongoDatabase db = dbClient.GetDatabase("newdemodb");
            var coll = db.GetCollection<BsonDocument>("demodbcollection");
            var filter1 = Builders<BsonDocument>.Filter.Eq("SellerId", id);
            var documents = coll.Find(filter1).ToList();
            long len = documents.Count();
            long not_healthyCount = 0;
            long healthy_Count = 0;
            foreach(BsonDocument doc in documents)
            {
                BsonDateTime orderDate = (BsonDateTime)doc.GetValue("OrderDate");
                string status = fetchFromDB1(orderDate, id);
                if(status.Equals("NOT_HEALTHY"))
                {
                    not_healthyCount++;
                }
                else
                {
                    healthy_Count++;
                }
                    
            }
            if(healthy_Count>=not_healthyCount)
            {
                sellerStatus = "HEALTHY";
            }
            else
            {
                sellerStatus = "NOT_HEALTHY";
            }
            return sellerStatus;
        }


}
}
