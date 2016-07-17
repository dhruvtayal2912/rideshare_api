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
		public string IssuedOn { get; set; }
		public string ExpiresOn { get; set; }
	}
}