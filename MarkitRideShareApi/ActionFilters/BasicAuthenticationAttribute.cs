using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.Filters;

namespace MarkitRideShareApi.ActionFilters
{
	public class BasicAuthenticationAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
		{
			try
			{
				if (actionContext.Request.Headers.Authorization == null)
				{
					actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
				}
				else
				{
					Dictionary<string, string> credentials = ParseRequestHeaders(actionContext);

					if (IsUserValid(credentials))
					{
						actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK);
					}
					else
					{
						actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
					}
				}
			}
			catch
			{
				actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
			}
		}

		private Boolean IsUserValid(Dictionary<string, string> credentials)
		{
			if (credentials["UserName"].Equals("joydip") && credentials["Password"].Equals("joydip123"))
				return true;

			return false;
		}

		private Dictionary<string, string> ParseRequestHeaders(System.Web.Http.Controllers.HttpActionContext actionContext)
		{
			Dictionary<string, string> credentials = new Dictionary<string, string>();

			var httpRequestHeader = actionContext.Request.Headers.GetValues("Authorization").FirstOrDefault();
			httpRequestHeader = httpRequestHeader.Substring("Authorization".Length);
			string[] httpRequestHeaderValues = httpRequestHeader.Split(':');

			string username = Encoding.UTF8.GetString(Convert.FromBase64String(httpRequestHeaderValues[0]));
			string password = Encoding.UTF8.GetString(Convert.FromBase64String(httpRequestHeaderValues[1]));

			credentials.Add("UserName", username);
			credentials.Add("Password", password);
			return credentials;

		}
	}
}