using MarkitRideShareApi.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using MarkitRideShareApi.ActionFilters;
using GoogleMaps.LocationServices;

namespace MarkitRideShareApi.Controllers
{
	//[EnableCors(origins: "*", headers: "*", methods: "*")]
	[RoutePrefix("api/employeedetail")]
	public class EmployeeDetailController : BaseApiController
    {
		private EmployeeDetailModel EmployeeDetailModel;
		private IMongoCollection<BsonDocument> EmpDetailsCollection = MongoConfig.MongoDatabase.GetCollection<BsonDocument>("EmployeeDetails");

		public EmployeeDetailController()
		{
			EmployeeDetailModel = new EmployeeDetailModel();
		}

        // GET api/employeedetail
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

		//TODO: remove looged in user from the returned result.
		//[BasicAuthentication]
		[Route("firsttimeempdata/{email}")]
		[HttpGet]
		public async Task<HttpResponseMessage> FirstTimeEmpData([FromUri]string email)
        {
			var allEmpDatadocs = await EmpDetailsCollection.Find(_ => true).ToListAsync();
			List<EmployeeData> allEmpDataList = BsonSerializer.Deserialize<List<EmployeeData>>(allEmpDatadocs.ToJson());

			EmployeeData loggedInEmpData = allEmpDataList.Where(e => e.Email == email).FirstOrDefault();
			var result = allEmpDataList.Where(t => MapUtility.distance(Convert.ToDouble(t.Latitude), Convert.ToDouble(t.Longitude), Convert.ToDouble(loggedInEmpData.Latitude), Convert.ToDouble(loggedInEmpData.Longitude), "K") < 5).ToList();

			var jsonData = new
			{
				Result = result
			};

			var response = Request.CreateResponse(HttpStatusCode.OK, jsonData, GenerateJsonFormatting());
			response.Content.Headers.ContentType = GenerateMediaType();
			return response;
        }

		[Route("getfiltereddata")]
		[HttpPost]
		public async Task<HttpResponseMessage> GetFilteredData([FromBody]EmployeeData empData)
        {
			try
			{
				var allEmpDatadocs = await EmpDetailsCollection.Find(_ => true).ToListAsync();
				List<EmployeeData> allEmpDataList = BsonSerializer.Deserialize<List<EmployeeData>>(allEmpDatadocs.ToJson());

				EmployeeData loggedInEmpData = allEmpDataList.Where(e => e.Email == "gurpreet.kaur1@markit.com").FirstOrDefault();

				var filteredData = allEmpDataList.Where(e => e.TravelFreq == (string.IsNullOrEmpty(empData.TravelFreq) ? e.TravelFreq : empData.TravelFreq)
					&& e.UserType == (string.IsNullOrEmpty(empData.UserType) ? e.UserType : empData.UserType)
					&& Util.GetEvenOdd(e.VehicleNo) == (string.IsNullOrEmpty(empData.VehicleNo) ? Util.GetEvenOdd(e.VehicleNo) : empData.VehicleNo)

					&& MapUtility.distance(Convert.ToDouble(e.Latitude), Convert.ToDouble(e.Longitude), Convert.ToDouble(loggedInEmpData.Latitude), Convert.ToDouble(loggedInEmpData.Longitude), "K") < empData.Radius).ToList();

				var jsonData = new
				{
					Result = filteredData
				};

				var response = Request.CreateResponse(HttpStatusCode.OK, jsonData, GenerateJsonFormatting());
				response.Content.Headers.ContentType = GenerateMediaType();
				return response;
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.NotFound, ex.Message, GenerateJsonFormatting());
			}
        }

		[BasicAuthentication]
		[Route("getbyaddress/{address}")]
		[HttpGet]
		public async Task<HttpResponseMessage> GetByAddress([FromUri]string address)
		{
			try
			{
				var locationService = new GoogleLocationService();
				var data = locationService.GetLatLongFromAddress(address);

				var allEmpDatadocs = await EmpDetailsCollection.Find(_ => true).ToListAsync();
				List<EmployeeData> allEmpDataList = BsonSerializer.Deserialize<List<EmployeeData>>(allEmpDatadocs.ToJson());

				var filteredData = allEmpDataList.Where(e => MapUtility.distance(Convert.ToDouble(e.Latitude), Convert.ToDouble(e.Longitude), Convert.ToDouble(data.Latitude), Convert.ToDouble(data.Longitude), "K") < 3).ToList();

				var jsonData = new
				{
					Result = filteredData
				};

				var response = Request.CreateResponse(HttpStatusCode.OK, jsonData, GenerateJsonFormatting());
				response.Content.Headers.ContentType = GenerateMediaType();
				return response;
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.NotFound);
			}
		}

        // PUT api/employeedetail/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/employeedetail/5
        public void Delete(int id)
        {
		}
	}
}
