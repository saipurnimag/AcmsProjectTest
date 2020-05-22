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

namespace EndUserAccessPage
{

    class GetModel
    {
        public ObjectId Id { get; set; }
        public string name { get; set; }
    }

    public class Program 
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
            var connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("newdb"); //newdb is the name of the database
            var collection = database.GetCollection<GetModel>("dbcollection"); //dbcollection is the name of the collection
            var obj = new GetModel { name = "Dhatri" };
            collection.InsertOne(obj); //inserting into db
            var id = obj.Id; //getting reference


        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
