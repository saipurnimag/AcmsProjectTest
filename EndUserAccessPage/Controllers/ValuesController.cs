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
        public string Get(DateTime orderDate, string id)
        {
            string res = fetchFromDB1(orderDate, id);
            return res;
        }

        // requirement 1 in seller performance management system
        // to return seller metrics per order
        [HttpGet] // id is orderID
        public IEnumerable<String> Get(String id)
        {
            if(id==null)
            {
                return new String[] { "REQUIREMENTS", "api/values?id=(OrderId)", "api/values/(OrderDate)/(SellerId)", "api/values/get1/(SellerId)" };
            }
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
        private  string fetchFromDB1(DateTime orderDate,string id)
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
            //to handle null case
            if(totalDocuments==0)
            {
                sellerHealthStatus = "One or both the params are incorrect";
                return sellerHealthStatus;
            }
            long drDocsCount=0;
            long shpDocsCount=0;
            long retDocsCount=0;
            long cancDocsCount=0;
            string DeliveryEstimateMetric = "";
            string shippingEstimateMetric = "";
            string cancellationMetric = "";
            string returnMetric = "";
            long c1 = 0;
            long c2 = 0;
            foreach(BsonDocument doc in documents)
            {
                bool ActualShipDate = doc.Contains("ActualShipDate");
                bool ActualDeliveryDate = doc.Contains("ActualDeliveryDate");
                DateTime actualShipDate;
                DateTime actualDeliveryDate;
                if (ActualShipDate)
                {
                    actualShipDate = (DateTime)doc.GetValue("ActualShipDate");
                    var filter3 = Builders<BsonDocument>.Filter.Gte("PromisedShipDate", actualShipDate);
                    var doc1 = coll.Find(filter1 & filter2 & filter3).FirstOrDefault();
                    if (doc1 != null)
                    {
                        shpDocsCount++;
                    }
                }
                else
                {
                    c1++;
                }
                if (ActualDeliveryDate)
                {
                    actualDeliveryDate = (DateTime)doc.GetValue("ActualDeliveryDate");
                    var filter4 = Builders<BsonDocument>.Filter.Gte("PromisedDeliveryDate", actualDeliveryDate);
                    var doc2 = coll.Find(filter1 & filter2 & filter4).FirstOrDefault();
                    if (doc2 != null)
                    {
                        drDocsCount++;
                    }
                }
                else
                {
                    c2++;
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
            drEstimatePercentage = (drDocsCount / (totalDocuments-c2))*100;
            shpEstimatePercentage = (shpDocsCount / (totalDocuments-c1))*100;
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
            String cancellationMetricFrOrder = "False";
            String returnMetricFrOrder = "";
            String[] res = new String[4];
            var dbClient = new MongoClient("mongodb://127.0.0.1:27017");
            IMongoDatabase db = dbClient.GetDatabase("newdemodb");
            var coll = db.GetCollection<BsonDocument>("demodbcollection");
            var alldocuments = coll.Find(new BsonDocument()).ToList();
            //every document has unique order id
            var filter = Builders<BsonDocument>.Filter.Eq("OrderId", id);
            var document = coll.Find(filter).FirstOrDefault();
            //if there's no doc with given orderid
            if(document==null)
            {
                return new String[] {"No","Document","Exists","with given orderid" };
            }
            else if (document != null)
            {
              
               
                var filter3 = Builders<BsonDocument>.Filter.Eq("isReturned", false);
                
                if (!document.Contains("ActualShipDate"))
                {
                    shippingEstimateMetricFrOrder = "Actual Shipment Date is null";
                }
                else
                {
                    BsonDateTime actualShipDate = (BsonDateTime)document.GetValue("ActualShipDate");
                    var filter1 = Builders<BsonDocument>.Filter.Gte("PromisedShipDate", actualShipDate);
                    var doc1 = coll.Find(filter & filter1).FirstOrDefault();
                    if (doc1 != null)
                    {
                        shippingEstimateMetricFrOrder = "True";
                    }
                    else
                    {
                        shippingEstimateMetricFrOrder = "False";
                    }
                }
                
                if (!document.Contains("ActualDeliveryDate"))
                {
                    DeliveryEstimateMetricFrOrder = "Actual Delivery Date is null";
                }
                else
                {
                    BsonDateTime actualDeliveryDate = (BsonDateTime)document.GetValue("ActualDeliveryDate");
                    var filter2 = Builders<BsonDocument>.Filter.Gte("PromisedDeliveryDate", actualDeliveryDate);
                    var doc2 = coll.Find(filter & filter2).FirstOrDefault();
                    if (doc2 != null)
                    {
                        DeliveryEstimateMetricFrOrder = "True";
                    }
                    else
                    {
                        DeliveryEstimateMetricFrOrder = "False";
                    }
                }
                var doc3 = coll.Find(filter & filter3).FirstOrDefault();
                if(doc3!=null)
                {
                    returnMetricFrOrder = "False";
                }
                else
                {
                    returnMetricFrOrder = "True";
                }
            }
            res[0] = "Shipping Estimate Metric  :   "+shippingEstimateMetricFrOrder;
            res[1] = "Delivery Estimate Metric  :   "+DeliveryEstimateMetricFrOrder;
            res[2] = "Cancellation Estimate Metric :   "+cancellationMetricFrOrder;
            res[3] = "Return Estimate Metric   :   "+returnMetricFrOrder;
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
            if(len==0)
            {
                return "NO SELLER WITH GIVEN SELLER ID";
            }
            long shpMetric_Count = 0;
            long drMetric_Count = 0;
            long retMetric_Count = 0;
            long cancMetric_Count = 0;
            long sellerWithNoOrder_Count = 0;
            long c1 = 0;
            long c2 = 0;
            foreach(BsonDocument doc in documents)
            {
                if (!doc.Contains("OrderId"))
                {
                    sellerWithNoOrder_Count++;
                }
                else
                {
                    String orderId = (String)doc.GetValue("OrderId");
                    String[] res = new String[4];
                    res = fetchFromDB2(orderId);
                    if (res[0].Equals("True") && !(res[0].Equals("False")))
                    {
                        shpMetric_Count++;
                    }
                    else if(res[0].Equals("Actual Shipment Date is null"))
                    {
                        c1++; //case where actual shipment date hasn't been there in db yet
                    }
                    if (res[1].Equals("True") && !(res[1].Equals("False")))
                    {
                        drMetric_Count++;
                    }
                    else if(res[1].Equals("Actual Delivery Date is null"))
                    {
                        c2++; //case where actual delivery date hasn't been in db yet
                    }
                    if (!res[2].Equals("False"))
                    {
                        cancMetric_Count++;
                    }
                    if (!res[3].Equals("False"))
                    {
                        retMetric_Count++;
                    }
                }
            }
            long shippingEstimate = (shpMetric_Count /(len-c1-sellerWithNoOrder_Count)) * 100;
            long drEstimate = (drMetric_Count / (len - c2-sellerWithNoOrder_Count)) * 100;
            long cancEstimate = (cancMetric_Count / (len - sellerWithNoOrder_Count)) * 100;
            long retEstimate = (retMetric_Count / (len - sellerWithNoOrder_Count)) * 100;
           
            String shpMetric;
            String drMetric;
            String cancMetric;
            String retMetric;
            if(shippingEstimate>=97)
            {
                shpMetric = "true";
            }
            else
            {
                shpMetric = "false";
            }
            if(drEstimate>=98)
            {
                drMetric = "true";
            }
            else
            {
                drMetric = "false";
            }
            if(cancEstimate<=2)
            {
                cancMetric = "false";
            }
            else
            {
                cancMetric = "true";
            }
            if(retEstimate<=1)
            {
                retMetric = "false";
            }
            else
            {
                retMetric = "true";
            }
            if(shpMetric.Equals("true")&&drMetric.Equals("true")&&cancMetric.Equals("false")&&retMetric.Equals("false"))
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
