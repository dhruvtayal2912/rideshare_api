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
using System.Web.Script.Serialization;

namespace MarkitRideShareApi
{
	public class TokenService
	{
		private IMongoCollection<BsonDocument> tokenCollection = MongoConfig.MongoDatabase.GetCollection<BsonDocument>("Tokens");

		public TokenModel GenerateToken(string username)
		{
			string token = Guid.NewGuid().ToString();
			string issuedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			string expiredOn = DateTime.Now.AddSeconds(Convert.ToDouble(ConfigurationManager.AppSettings["AuthTokenExpiry"])).ToString("yyyy-MM-dd HH:mm:ss");
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
				BsonDocument doc = BsonSerializer.Deserialize<BsonDocument>(tokenModel.ToJson(jsonWriterSettings));

				tokenCollection.InsertOneAsync(doc);			
			}
			catch(System.Exception ex)
			{
				//throw new Exception("Error occured while inserting token" + ex.Message);
				return null;
			}

			return tokenModel;
		}

		public bool ValidateToken(TokenModel token)
		{
			var filter = Builders<BsonDocument>.Filter.Eq("AuthToken", token.AuthToken);
			var projection = Builders<BsonDocument>.Projection.Exclude("_id");

			var docs = tokenCollection.Find(filter).Project(projection).ToList();
			TokenModel existingToken = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(docs[0].ToJson());

			DateTime existingTokenExpiresOn = Convert.ToDateTime(existingToken.ExpiresOn);

			if (existingToken != null && !(DateTime.Now > existingTokenExpiresOn))
			{
				existingToken.ExpiresOn = existingTokenExpiresOn.AddSeconds(
					Convert.ToDouble(ConfigurationManager.AppSettings["AuthTokenExpiry"])).ToString("yyyy-MM-dd HH:mm:ss");
				try
				{
					BsonDocument doc = BsonSerializer.Deserialize<BsonDocument>(existingToken.ToJson());
					tokenCollection.ReplaceOneAsync(filter, doc);
				}
				catch (System.Exception ex)
				{
					throw new Exception("Error occured while inserting token" + ex.Message);
				}
				return true;
			}
			return false;
		}

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