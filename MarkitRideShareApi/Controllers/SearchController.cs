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
	//[EnableCors(origins: "*", headers: "*", methods: "*")]
	public class SearchController : BaseApiController
    {
		private EmployeeDetailModel EmployeeDetailModel;
		private IMongoCollection<BsonDocument> EmpDetailsCollection = MongoConfig.MongoDatabase.GetCollection<BsonDocument>("EmployeeDetails");

		public SearchController()
		{
			EmployeeDetailModel = new EmployeeDetailModel();
		}

		//// GET api/search
		//public IEnumerable<string> Get()
		//{
		//	return new string[] { "value1", "value2" };
		//}

		//TODO: remove looged in user from the returned result.

		[Route("api/search/{empData}")]
		[HttpGet]
		public async Task<HttpResponseMessage> Get(EmployeeData empData)
		{
			//if (!string.IsNullOrEmpty(empData))
			//{
			//	var JsonDe = JsonConvert.DeserializeObject<EmployeeData>(empData);
			//}
			//var allEmpDatadocs = await EmpDetailsCollection.Find(_ => true).ToListAsync();
			//List<EmployeeData> allEmpDataList = BsonSerializer.Deserialize<List<EmployeeData>>(allEmpDatadocs.ToJson());

			//EmployeeData loggedInEmpData = allEmpDataList.Where(e => e.Email == "gurpreet.kaur1@markit.com").FirstOrDefault();

			//var filteredData = allEmpDataList.Where(e => e.TravelFreq == (string.IsNullOrEmpty(empData.TravelFreq) ? e.TravelFreq : empData.TravelFreq)
			//	&& e.UserType == (string.IsNullOrEmpty(empData.UserType) ? e.UserType : empData.UserType)
			//	&& e.VehicleNo == (string.IsNullOrEmpty(empData.VehicleNo) ? e.VehicleNo : empData.VehicleNo)
			//	&& MapUtility.distance(Convert.ToDouble(e.Latitude), Convert.ToDouble(e.Longitude), Convert.ToDouble(loggedInEmpData.Latitude), Convert.ToDouble(loggedInEmpData.Longitude), "K") < empData.Radius).ToList();

			var jsonData = new
			{
				Result = empData.ToJson()
			};

			var response = Request.CreateResponse(HttpStatusCode.OK, jsonData, GenerateJsonFormatting());
			response.Content.Headers.ContentType = GenerateMediaType();
			return response;
		}

        // POST api/search
        public void Post([FromBody]string value)
        {
        }

        // PUT api/search/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/search/5
        public void Delete(int id)
        {
        }
    }
}
