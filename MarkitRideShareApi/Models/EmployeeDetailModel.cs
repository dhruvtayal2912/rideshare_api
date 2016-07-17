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
		//public string Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Contact { get; set; }
		public string Address { get; set; }
		public string Latitude { get; set; }
		public string Longitude { get; set; }
		public string VehicleNo { get; set; }
		public string ShiftStartTime { get; set; }
		public string ShiftEndTime { get; set; }
		public string UserType { get; set; } // Pooler = 0 & Seeker = 1
		public string TravelFreq { get; set; } // Regular = 0 & Today = 1
		public string Active { get; set; } //Yes = 1 & No = 0
		public int Radius { get; set; }
		public string Gender { get; set; } // Male = 0 & Female = 1
	}

	public enum UserType
	{
		Pooler = 0,
		Seeker = 1
	}

	public enum TravelFrequency
	{
		Regular = 0,
		Today = 1
	}

	public class Coordinates
	{
		public double X { get; set; }
		public double Y { get; set; }
	}
}