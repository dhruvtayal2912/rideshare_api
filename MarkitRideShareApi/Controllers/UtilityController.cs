using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MarkitRideShareApi.Controllers
{
	public class UtilityController : BaseApiController
    {
		[Route("api/utility/updatedb")]
		[HttpGet]
		public HttpResponseMessage UpdateDB()
		{
			string statusMsg = Util.UpdateDBFromExcel();

			var jsonData = new
			{
				Result = statusMsg
			};

			var response = Request.CreateResponse(HttpStatusCode.OK, jsonData, GenerateJsonFormatting());
			response.Content.Headers.ContentType = GenerateMediaType();
			return response;
		}
    }
}
