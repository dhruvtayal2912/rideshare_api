using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarkitRideShareApi.Models
{
	public class TokenModel
	{
		public string UserName { get; set; }
		public string AuthToken { get; set; }
		public System.DateTime IssuedOn { get; set; }
		public System.DateTime ExpiresOn { get; set; }
	}
}