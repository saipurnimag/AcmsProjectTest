using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace EndUserAccessPage.Main
{

   class Order
    {
        public string OrderId { get; set; }
        public String SellerId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime PromisedShipDate { get; set; }
        public DateTime PromisedDeliveryDate { get; set; }
        public DateTime ActualShipDate { get; set; }
        public DateTime ActualDeliveryDate { get; set; }
        public string CancellationOrigin { get; set; }
        public string CancellationReason { get; set; }
        public bool isReturned { get; set; }

    }

    class Seller
    {
      //  [BsonId]
        public ObjectId Id { get; set; }
        public string SellerId { get; set; }

        public Seller()
        {
            Id = ObjectId.GenerateNewId();
        }
    }

    public class Program 
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();

           
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
