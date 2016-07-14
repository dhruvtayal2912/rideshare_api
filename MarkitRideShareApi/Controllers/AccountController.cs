using MarkitRideShareApi.Models;
using System;
using System.Configuration;
using System.DirectoryServices;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MarkitRideShareApi.Controllers
{
	[EnableCors(origins: "*", headers: "*", methods: "*")]
	public class AccountController : BaseApiController
	{
		private readonly TokenService _tokenService;

		public AccountController()
		{
			_tokenService = new TokenService();
		}

		[Route("api/account/login")]
		[HttpGet]
		public HttpResponseMessage Login()
		{
			string username = "dhruv.tayal";
			string password = "June$321";
			String domainAndUsername = "markit.com" + @"\" + username;
			DirectoryEntry entry = new DirectoryEntry("LDAP://markit", domainAndUsername, password);

			try
			{	//Bind to the native AdsObject to force authentication.			
				Object obj = entry.NativeObject;

				DirectorySearcher search = new DirectorySearcher(entry);

				search.Filter = "(SAMAccountName=" + username + ")";
				search.PropertiesToLoad.Add("cn");
				SearchResult result = search.FindOne();

				if (null != result)
				{
					return GetAuthToken(username);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error authenticating user. " + ex.Message);
			}

			var response = Request.CreateResponse(HttpStatusCode.Unauthorized);
			return response;
		}

		/// <summary>
		/// Returns auth token for the validated user.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		private HttpResponseMessage GetAuthToken(string username)
		{
			TokenModel token = _tokenService.GenerateToken(username);
			var jsonData = new
			{
				Result = token
			};

			var response = Request.CreateResponse(HttpStatusCode.OK, jsonData, GenerateJsonFormatting());
			response.Content.Headers.ContentType = GenerateMediaType();
			return response;
		}
	}
}
