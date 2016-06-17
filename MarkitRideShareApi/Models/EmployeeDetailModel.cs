using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace MarkitRideShareApi.Models
{
	public class EmployeeDetailModel
	{
		private IMongoDatabase DB;

		public EmployeeDetailModel()
		{
			//MongoConfig config = new MongoConfig();
			//DB = config.MongoDatabase;
		}

		public Coordinates GetEmployeeCoord(string email)
		{
			//var collection = DB.GetCollection<BsonDocument>("EmployeeDetails");
			//var filter = Builders<BsonDocument>.Filter.Eq("Email", email);
			//var projection = Builders<BsonDocument>.Projection.Exclude("_id");
			return new Coordinates();
		}
	}

	public class EmployeeData
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Contact { get; set; }
		public string Address { get; set; }
		public string Latitude { get; set; }
		public string Longitude { get; set; }
		public string VehicleNo { get; set; }
		public string ShiftStartTime { get; set; }
		public string ShiftEndTime { get; set; }
		public string UserType { get; set; } // Pooler = 1 & Seeker = 2
		public string TravelFreq { get; set; } // Regular = 1 & Today = 2
		public string Active { get; set; }
		public int Radius { get; set; }
		public string Gender { get; set; }
	}

	public enum UserType
	{
		Pooler = 1,
		Seeker = 2
	}

	public enum TravelFrequency
	{
		Regular = 1,
		Today = 2
	}

	public class Coordinates
	{
		public double X { get; set; }
		public double Y { get; set; }
	}
}