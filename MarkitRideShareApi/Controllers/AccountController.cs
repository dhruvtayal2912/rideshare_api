using System;
using System.DirectoryServices;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace MarkitRideShareApi.Controllers
{
	public class AccountController : BaseApiController
	{
		[Route("api/account/login")]
		[HttpPost]
		public HttpResponseMessage Login([FromBody]string email, [FromBody]string password)
		{
			String domainAndUsername = "markit.com" + @"\" + "dhruv.tayal";
			DirectoryEntry entry = new DirectoryEntry("LDAP://markit", domainAndUsername, "March$321");
			bool status = true;
			try
			{	//Bind to the native AdsObject to force authentication.			
				Object obj = entry.NativeObject;

				DirectorySearcher search = new DirectorySearcher(entry);

				search.Filter = "(SAMAccountName=" + "dhruv.tayal" + ")";
				search.PropertiesToLoad.Add("cn");
				SearchResult result = search.FindOne();

				if (null == result)
				{
					status = false;
				}

				//Update the new path to the user in the directory.
				string path = result.Path;
				string filterAttribute = (String)result.Properties["cn"][0];

				DirectorySearcher search1 = new DirectorySearcher(path);
				search.Filter = "(cn=" + filterAttribute + ")";
				search.PropertiesToLoad.Add("memberOf");
				StringBuilder groupNames = new StringBuilder();

				SearchResult result1 = search.FindOne();

				int propertyCount = result1.Properties["memberOf"].Count;

				String dn;
				int equalsIndex, commaIndex;

				for (int propertyCounter = 0; propertyCounter < propertyCount; propertyCounter++)
				{
					dn = (String)result1.Properties["memberOf"][propertyCounter];

					equalsIndex = dn.IndexOf("=", 1);
					commaIndex = dn.IndexOf(",", 1);
					if (-1 == equalsIndex)
					{
						return null;
					}

					groupNames.Append(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));
					groupNames.Append("|");
				}

			}
			catch (Exception ex)
			{
				throw new Exception("Error authenticating user. " + ex.Message);
			}

			//return true;

			var jsonData = new
			{
				Result = status
			};

			var response = Request.CreateResponse(HttpStatusCode.OK, jsonData, GenerateJsonFormatting());
			response.Content.Headers.ContentType = GenerateMediaType();
			return response;
		}
	}
}
