using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MarkitRideShareApi
{
	public static class MongoConfig
	{
		public static IMongoDatabase MongoDatabase { get; set; }

		static MongoConfig()
		{
			var client = new MongoClient(ConfigurationManager.AppSettings["MongoDBConectionString"]);
			MongoDatabase = client.GetDatabase(ConfigurationManager.AppSettings["MongoDBDatabaseName"]);
		}
	}
}