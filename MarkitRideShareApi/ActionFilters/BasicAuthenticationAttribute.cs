using MarkitRideShareApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.Filters;

namespace MarkitRideShareApi.ActionFilters
{
	public class BasicAuthenticationAttribute : ActionFilterAttribute
	{
		private readonly TokenService _tokenService = new TokenService();

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
					TokenModel token = ParseRequestHeaders(actionContext);

					if (!_tokenService.ValidateToken(token))
					{
						actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
					}
				}
			}
			catch
			{
				actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
			}

			//base.OnActionExecuting(actionContext);
		}

		private TokenModel ParseRequestHeaders(System.Web.Http.Controllers.HttpActionContext actionContext)
		{
			var httpRequestHeader = actionContext.Request.Headers.GetValues("Authorization").FirstOrDefault();
			httpRequestHeader = httpRequestHeader.Substring("Basic ".Length);

			string TokenModel = Encoding.UTF8.GetString(Convert.FromBase64String(httpRequestHeader));
			TokenModel token = JsonConvert.DeserializeObject<TokenModel>(TokenModel);

			return token;
		}
	}
}