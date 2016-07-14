using MarkitRideShareApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MarkitRideShareApi
{
	public class TokenService
	{
		private IMongoCollection<BsonDocument> tokenCollection = MongoConfig.MongoDatabase.GetCollection<BsonDocument>("Tokens");

		public TokenModel GenerateToken(string username)
		{
			string token = Guid.NewGuid().ToString();
			DateTime issuedOn = DateTime.Now;
			DateTime expiredOn = DateTime.Now.AddSeconds(Convert.ToDouble(ConfigurationManager.AppSettings["AuthTokenExpiry"]));
			var tokenModel = new TokenModel()
			{
				UserName = username,
				IssuedOn = issuedOn,
				ExpiresOn = expiredOn,
				AuthToken = token
			};

			try
			{
				var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
				BsonDocument doc = BsonSerializer.Deserialize<BsonDocument>(tokenModel.ToJson());

				tokenCollection.InsertOneAsync(doc);			
			}
			catch(System.Exception ex)
			{
				throw new Exception("Error occured while inserting token" + ex.Message);
			}

			return tokenModel;
		}

		//public bool ValidateToken(string tokenId)
		//{
		//	//var token = _unitOfWork.TokenRepository.Get(t => t.AuthToken == tokenId && t.ExpiresOn > DateTime.Now); // fetch token from db
		//	if (token != null && !(DateTime.Now > token.ExpiresOn))
		//	{
		//		token.ExpiresOn = token.ExpiresOn.AddSeconds(Convert.ToDouble(ConfigurationManager.AppSettings["AuthTokenExpiry"]));
		//		_unitOfWork.TokenRepository.Update(token);
		//		_unitOfWork.Save();
		//		return true;
		//	}
		//	return false;
		//}

		//public bool Kill(string tokenId)
		//{
		//	_unitOfWork.TokenRepository.Delete(x => x.AuthToken == tokenId);
		//	_unitOfWork.Save();
		//	var isNotDeleted = _unitOfWork.TokenRepository.GetMany(x => x.AuthToken == tokenId).Any();
		//	if (isNotDeleted) { return false; }
		//	return true;
		//}

		public bool DeleteByUserName(int username)
		{
			//delete token in mongodb
			//_unitOfWork.TokenRepository.Delete(x => x.UserId == userId);
			//_unitOfWork.Save();

			//var isNotDeleted = _unitOfWork.TokenRepository.GetMany(x => x.UserId == userId).Any();
			//return !isNotDeleted;

			return false;
		}
	}
}