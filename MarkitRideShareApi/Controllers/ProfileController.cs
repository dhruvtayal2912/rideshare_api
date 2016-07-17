using MarkitRideShareApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MarkitRideShareApi.Controllers
{
	public class ProfileController : BaseApiController
    {
		private EmployeeDetailModel EmployeeDetailModel;
		private IMongoCollection<BsonDocument> EmpDetailsCollection = MongoConfig.MongoDatabase.GetCollection<BsonDocument>("EmployeeDetails");

		public ProfileController()
		{
			EmployeeDetailModel = new EmployeeDetailModel();
		}
    }
}
