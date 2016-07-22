using Microsoft.Exchange.WebServices.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;

namespace MarkitRideShareApi.Controllers
{
    public class BaseApiController : ApiController
    {
		//private ExchangeService ExchangeService;
		protected IMongoCollection<BsonDocument> EmpDetailsCollection = MongoConfig.MongoDatabase.GetCollection<BsonDocument>("EmployeeDetails");

		public BaseApiController()
		{
			//ExchangeService = new ExchangeService(ExchangeVersion.Exchange2013);
			//ExchangeService.Credentials = new WebCredentials("dhruv.tayal@markit.com", "June$321");
			//ExchangeService.AutodiscoverUrl("dhruv.tayal@markit.com", Util.RedirectionUrlValidationCallback);
		}
		
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
			var docs = EmpDetailsCollection.Find(filter).ToList();

			return (docs != null && docs.Count > 0) ? true : false;
		}

		//protected string GetUserPhoto(string email)
		//{

		//	System.Net.Http.HttpClient httpclient = new System.Net.Http.HttpClient();

		//	httpclient.BaseAddress = new System.Uri(ExchangeService.Url + "/s/GetUserPhoto?email=" + email + "&size=HR48x48");

		//	var credentials = Encoding.ASCII.GetBytes("dhruv.tayal:June$321");
		//	httpclient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));
		//	httpclient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("image/jpg"));

		//	HttpResponseMessage messge = httpclient.GetAsync(ExchangeService.Url + "/s/GetUserPhoto?email=" + email + "&size=HR48x48").Result;
		//	string result = messge.Content.ReadAsStringAsync().Result;


		//	//HttpWebRequest request = WebRequest.Create(ExchangeService.Url + "/s/GetUserPhoto?email=" + email + "&size=HR48x48") as HttpWebRequest;
		//	//// Submit the request.
		//	//using (HttpWebResponse resp = request.GetResponse() as HttpWebResponse)
		//	//{
		//	//	// Take the response and save it as an image.
		//	//	Bitmap image = new Bitmap(resp.GetResponseStream());
		//	//	//image.Save("Sadie.jpg");
		//	//}
		//	return "";
		//}
    }
}
