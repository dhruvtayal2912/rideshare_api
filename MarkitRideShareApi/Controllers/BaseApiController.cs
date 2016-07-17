using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;

namespace MarkitRideShareApi.Controllers
{
    public class BaseApiController : ApiController
    {
		protected IMongoCollection<BsonDocument> EmpDetailsCollection = MongoConfig.MongoDatabase.GetCollection<BsonDocument>("EmployeeDetails");
		
		protected MediaTypeHeaderValue GenerateMediaType()
		{
			return new MediaTypeHeaderValue("application/json");
		}

		protected JsonMediaTypeFormatter GenerateJsonFormatting()
		{
			var json = Configuration.Formatters.JsonFormatter;
			json.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
			return json;
		}

		protected bool ExistingUser(string userName)
		{
			var filter = Builders<BsonDocument>.Filter.Eq("Email", userName + "@markit.com");
			var docs = EmpDetailsCollection.Find(filter).ToListAsync();

			return (docs != null) ? true : false;
		}
    }
}
