using GoogleMaps.LocationServices;
using MarkitRideShareApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Excel = Microsoft.Office.Interop.Excel;

namespace MarkitRideShareApi
{
	public class Util
	{
		public static string UpdateDBFromExcel()
		{
			string statusMsg = "";
			const string conStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Projects\MarkitRideShareApi\MarkitRideShareApi\App_Data\EmployeeDetails.xlsx;Extended Properties=Excel 12.0 Xml";

			try
			{
				using (OleDbConnection connection = new OleDbConnection(conStr))
				{
					connection.Open();
					OleDbCommand command = new OleDbCommand("select * from [Sheet1$]", connection);
					OleDbDataAdapter adap = new OleDbDataAdapter(command);
					DataTable dt = new DataTable();
					adap.Fill(dt);

					if (dt.Rows.Count > 1)
					{
						var locationService = new GoogleLocationService();
						List<EmployeeData> empData = (from row in dt.AsEnumerable()

													  
									   select new EmployeeData
									   {
										   //Id = Convert.ToString(row.Field<double>("Id")),
										   Name = row.Field<string>("Name"),
										   Email = row.Field<string>("Email"),
										   Contact = Convert.ToString(row.Field<double>("Contact")),
										   Address = row.Field<string>("Address"),
										   Latitude = Convert.ToString(locationService.GetLatLongFromAddress(row.Field<string>("Address")).Latitude),
										   Longitude = Convert.ToString(locationService.GetLatLongFromAddress(row.Field<string>("Address")).Longitude),
										   VehicleNo = row.Field<string>("VehicleNo"),
										   ShiftStartTime = Convert.ToString(row.Field<double>("ShiftStartTime")),
										   ShiftEndTime = Convert.ToString(row.Field<double>("ShiftEndTime")),
										   UserType = Convert.ToString(row.Field<double>("UserType")),
										   TravelFreq = Convert.ToString(row.Field<double>("TravelFrequency")),
										   Active = Convert.ToString(row.Field<double>("Active")),
									   }).ToList();

						var collection = MongoConfig.MongoDatabase.GetCollection<BsonDocument>("EmployeeDetails");
						var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
						foreach (var item in empData)
						{
							BsonDocument doc = BsonSerializer.Deserialize<BsonDocument>(item.ToJson(jsonWriterSettings));
							collection.InsertOneAsync(doc);
						}
					}
				}

				statusMsg = "Records updated successfully.";
			}
			catch (Exception ex)
			{
				statusMsg = "There is some problem in updating -- " + ex.Message;
			}

			return statusMsg;

		//	Excel.Application xlApp = new Excel.Application();
		//	Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(@"C:\Users\dhruv.tayal\Desktop\EmployeeDetails.xlsx");
		//	Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
		//	Excel.Range xlRange = xlWorksheet.UsedRange;
			
		//	int rowCount = xlRange.Rows.Count;
		//	int colCount = xlRange.Columns.Count;

		//	var collection = MongoConfig.MongoDatabase.GetCollection<BsonDocument>("EmployeeDetails");
		//	var locationService = new GoogleLocationService();
		//	string statusMsg = "";

		//	for (int i = 2; i <= rowCount; i++) //leaving behind the header row
		//	{
		//		string address = xlWorksheet.Cells[i, 5].Text;

		//		var point = locationService.GetLatLongFromAddress(address);
		//		var latitude = point.Latitude;
		//		var longitude = point.Longitude;

		//		try
		//		{
		//			var document = new BsonDocument
		//				{
		//					{"_id", xlWorksheet.Cells[i, 1].Text},
		//					{"Name", xlWorksheet.Cells[i, 2].Text},
		//					{"Email", xlWorksheet.Cells[i, 3].Text},
		//					{"Contact", xlWorksheet.Cells[i, 4].Text},
		//					{"Address", xlWorksheet.Cells[i, 5].Text},
		//					{"VehicleNo", xlWorksheet.Cells[i, 6].Text},
		//					{"ShiftStartTime", xlWorksheet.Cells[i, 7].Text},
		//					{"ShiftEndTime", xlWorksheet.Cells[i, 8].Text},
		//					{"UserType", xlWorksheet.Cells[i, 9].Text},
		//					{"TravelFreq", xlWorksheet.Cells[i, 10].Text},
		//					{"Active", xlWorksheet.Cells[i, 11].Text},
		//					{"Lat", latitude},
		//					{"Long", longitude},
		//					{"CreatedOn", DateTime.Now.ToShortDateString()}
		//				};

		//			collection.InsertOneAsync(document);
		//		}
		//		catch (System.Exception ex)
		//		{
		//			return statusMsg = "Error occured: " + ex.Message;
		//		}
		//	}
			
		}

		public static string GetEvenOdd(string vehicleNo)
		{
			return (Convert.ToInt32(vehicleNo.Substring(vehicleNo.Length - 1)) % 2).ToString();
		}
	}
}