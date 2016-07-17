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
	[EnableCors(origins: "*", headers: "*", methods: "*")]
	[RoutePrefix("api/employeedetail")]
	[BasicAuthentication]
	public class EmployeeDetailController : BaseApiController
    {
		private EmployeeDetailModel EmployeeDetailModel;
		//private IMongoCollection<BsonDocument> EmpDetailsCollection = MongoConfig.MongoDatabase.GetCollection<BsonDocument>("EmployeeDetails");

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
		[Route("firsttimeempdata/{email}")]
		[HttpGet]
		public async Task<HttpResponseMessage> FirstTimeEmpData([FromUri]string email)
        {
			var projection = Builders<BsonDocument>.Projection.Exclude("_id");

			var allEmpDatadocs = await EmpDetailsCollection.Find(_ => true).Project(projection).ToListAsync();
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
				var projection = Builders<BsonDocument>.Projection.Exclude("_id");
				var allEmpDatadocs = await EmpDetailsCollection.Find(_ => true).Project(projection).ToListAsync();

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

		[Route("createprofile")]
		[HttpPost]
		public async Task<HttpResponseMessage> CreateProfile([FromBody]EmployeeData profileData)
        {
			try
			{
				var locationService = new GoogleLocationService();
				profileData.Latitude = Convert.ToString(locationService.GetLatLongFromAddress(profileData.Address).Latitude);
				profileData.Longitude = Convert.ToString(locationService.GetLatLongFromAddress(profileData.Address).Longitude);

				profileData.UserType = profileData.VehicleNo != null ?  "0" : "1";

				BsonDocument doc = BsonSerializer.Deserialize<BsonDocument>(profileData.ToJson());
				await EmpDetailsCollection.InsertOneAsync(doc);

				var response = Request.CreateResponse(HttpStatusCode.OK, "Profile has been created successfully.", GenerateJsonFormatting());
				response.Content.Headers.ContentType = GenerateMediaType();
				return response;
			}
			catch
			{
				return Request.CreateErrorResponse(HttpStatusCode.NotModified, new HttpError("There is some problem at the moment. Please try again."));
			}
        }

		[Route("hasprofile")]
		[HttpGet]
		public HttpResponseMessage HasProfile(string username)
		{
			bool hasProfile = ExistingUser(username);

			var jsonData = new
			{
				Result = hasProfile
			};

			var response = Request.CreateResponse(HttpStatusCode.OK, jsonData, GenerateJsonFormatting());
			response.Content.Headers.ContentType = GenerateMediaType();
			return response;
		}

        // DELETE api/employeedetail/5
        public void Delete(int id)
        {
		}
	}
}
