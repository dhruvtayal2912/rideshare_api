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

		public EmployeeDetailController()
		{
			EmployeeDetailModel = new EmployeeDetailModel();
		}

        // GET api/employeedetail
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

		[Route("firsttimeempdata/{email}")]
		[HttpGet]
		public async Task<HttpResponseMessage> FirstTimeEmpData([FromUri]string email)
        {
			var projection = Builders<BsonDocument>.Projection.Exclude("_id");

			var allEmpDatadocs = await EmpDetailsCollection.Find(_ => true).Project(projection).ToListAsync();
			List<EmployeeData> allEmpDataList = BsonSerializer.Deserialize<List<EmployeeData>>(allEmpDatadocs.ToJson());

			EmployeeData loggedInEmpData = allEmpDataList.Where(e => e.Email == email).FirstOrDefault();
			var result = allEmpDataList.Where(t => t.Active  == "1" && MapUtility.distance(Convert.ToDouble(t.Latitude), Convert.ToDouble(t.Longitude),
				Convert.ToDouble(loggedInEmpData.Latitude), Convert.ToDouble(loggedInEmpData.Longitude), "K") < 5).ToList();

			var jsonData = new
			{
				Result = result,
				CurrentUserData = loggedInEmpData
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

				EmployeeData loggedInEmpData = allEmpDataList.Where(e => e.Email == empData.Email).FirstOrDefault();

				var filteredData = allEmpDataList.Where(e => e.Active  == "1" && e.UserType == (string.IsNullOrEmpty(empData.UserType) ? e.UserType : empData.UserType)
					&& Util.GetEvenOdd(e.VehicleNo) == (string.IsNullOrEmpty(empData.VehicleNo) ? Util.GetEvenOdd(e.VehicleNo) : empData.VehicleNo)
					&& e.Gender == (string.IsNullOrEmpty(empData.Gender) ? e.Gender : empData.Gender)
					&& e.HasParking == (string.IsNullOrEmpty(empData.HasParking) ? e.HasParking : empData.HasParking)
					&& MapUtility.distance(Convert.ToDouble(e.Latitude), Convert.ToDouble(e.Longitude), Convert.ToDouble(loggedInEmpData.Latitude),
					Convert.ToDouble(loggedInEmpData.Longitude), "K") < empData.Radius).ToList();

				var jsonData = new
				{
					Result = filteredData,
					CurrentUserData = loggedInEmpData
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

		[Route("getbyaddress/{address}/{email}")]
		[HttpGet]
		public async Task<HttpResponseMessage> GetByAddress(string address, string email)
		{
			try
			{
				var locationService = new GoogleLocationService();
				var data = locationService.GetLatLongFromAddress(address);

				if (data != null)
				{
					var projection = Builders<BsonDocument>.Projection.Exclude("_id");
					var allEmpDatadocs = await EmpDetailsCollection.Find(_ => true).Project(projection).ToListAsync();
					List<EmployeeData> allEmpDataList = BsonSerializer.Deserialize<List<EmployeeData>>(allEmpDatadocs.ToJson());

					EmployeeData loggedInEmpData = allEmpDataList.Where(e => e.Email == email).FirstOrDefault();

					var filteredData = allEmpDataList.Where(e => e.Active  == "1" &&  MapUtility.distance(Convert.ToDouble(e.Latitude), Convert.ToDouble(e.Longitude), Convert.ToDouble(data.Latitude), Convert.ToDouble(data.Longitude), "K") < 3).ToList();

					//Now coordinates of LoggedIn user will get changed respective to new address
					loggedInEmpData.Latitude = data.Latitude.ToString();
					loggedInEmpData.Longitude = data.Longitude.ToString();

					var jsonData = new
					{
						Result = filteredData,
						CurrentUserData = loggedInEmpData,
					};

					var response = Request.CreateResponse(HttpStatusCode.OK, jsonData, GenerateJsonFormatting());
					response.Content.Headers.ContentType = GenerateMediaType();
					return response;
				}
				return Request.CreateErrorResponse(HttpStatusCode.NotFound, new HttpError("Result not available. Please improve your input address."));
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

		[Route("editprofile")]
		[HttpPost]
		public HttpResponseMessage EditProfile([FromBody]EmployeeData newProfileData)
		{
			try
			{
				var filter = Builders<BsonDocument>.Filter.Eq("Email", newProfileData.Email);
				var projection = Builders<BsonDocument>.Projection.Exclude("_id");

				var docs = EmpDetailsCollection.Find(filter).Project(projection).ToList();
				EmployeeData existingEmpData = Newtonsoft.Json.JsonConvert.DeserializeObject<EmployeeData>(docs[0].ToJson());

				existingEmpData.UserType = newProfileData.VehicleNo != null ? "0" : "1";

				if (!String.IsNullOrEmpty(newProfileData.Contact))
				{
					existingEmpData.Contact = newProfileData.Contact;
				}
				if (!String.IsNullOrEmpty(newProfileData.Address))
				{
					existingEmpData.Address = newProfileData.Address;
				}
				if (!String.IsNullOrEmpty(newProfileData.VehicleNo))
				{
					existingEmpData.VehicleNo = newProfileData.VehicleNo;
				}
				if (!String.IsNullOrEmpty(newProfileData.ShiftStartTime))
				{
					existingEmpData.ShiftStartTime = newProfileData.ShiftStartTime;
				}
				if (!String.IsNullOrEmpty(newProfileData.ShiftStartTime))
				{
					existingEmpData.ShiftEndTime = newProfileData.ShiftEndTime;
				}
				existingEmpData.Active = newProfileData.Active;

				BsonDocument doc = BsonSerializer.Deserialize<BsonDocument>(existingEmpData.ToJson());
				var result = EmpDetailsCollection.ReplaceOneAsync(filter, doc);

				if (result.Result.ModifiedCount > 0)
				{
					var response = Request.CreateResponse(HttpStatusCode.OK, "Profile has been updated successfully.", GenerateJsonFormatting());
					response.Content.Headers.ContentType = GenerateMediaType();
					return response;
				}
				else
				{
					return Request.CreateErrorResponse(HttpStatusCode.NotModified, new HttpError("There is some problem in updating your profile. Please try again."));
				}
				
			}
			catch(Exception ex)
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
