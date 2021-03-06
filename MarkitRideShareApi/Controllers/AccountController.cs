﻿using MarkitRideShareApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
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
	[RoutePrefix("api/account")]
	public class AccountController : BaseApiController
	{
		private readonly TokenService _tokenService;

		public AccountController()
		{
			_tokenService = new TokenService();
		}

		[Route("login")]
		[HttpGet]
		public HttpResponseMessage Login(string username, string password)
		{
			//username = "dhruv.tayal";
			//password = "June$321";
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
					TokenModel token = _tokenService.GenerateToken(username); //create token
					bool hasProfile = ExistingUser(username);

					if (token != null)
					{
						var jsonData = new
						{
							Token = token,
							HasProfile = hasProfile, // TODO: hasProfile
							ExistingProfileData = new { Name = result.Properties["cn"][0], Email = username + "@markit.com" } // TODO: !hasProfile
						};

						var response = Request.CreateResponse(HttpStatusCode.OK, jsonData, GenerateJsonFormatting());
						response.Content.Headers.ContentType = GenerateMediaType();
						return response;
					}
					else
					{
						return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "There is some problem at the moment. Please try again.");
					}
				}
				else
				{
					var response = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "The username or password is invalid.");
					return response;
				}
			}
			catch
			{
				return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "There is some problem at the moment. Please try again.");
			}
		}

		[Route("logout")]
		[HttpPost]
		public HttpResponseMessage Logout([FromBody]TokenModel token)
		{
			bool deleted = _tokenService.DeleteToken(token);
			var jsonData = new
			{
				IsDeleted = deleted ? true : false				
			};

			if(deleted)
			{
				var response = Request.CreateResponse(HttpStatusCode.OK, jsonData, GenerateJsonFormatting());
				response.Content.Headers.ContentType = GenerateMediaType();
				return response;
			}
			return Request.CreateErrorResponse(HttpStatusCode.NotModified, "There is some problem at the moment. Please try again.");
		}
	}
}
