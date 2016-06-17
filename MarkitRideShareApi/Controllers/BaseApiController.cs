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
    }
}
