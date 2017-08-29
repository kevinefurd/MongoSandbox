using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Mongo
{
	/// <summary>
	/// migrates objectids to strings for all documents of all collections
	/// </summary>
	public class RepoMigrator
	{
		MongoClient mClient;
		IMongoDatabase mDb;
		private List<Tuple<string, string>> failIds = new List<Tuple<string, string>>();
		private List<string> successIds = new List<string>();
		public List<string> successColls = new List<string>();
		public List<string> failColls = new List<string>();
		public RepoMigrator(string connectString, string dbName)
		{
			var a = new ObjectId();
			var b = ObjectId.Empty;
			mClient = new MongoClient(connectString);
			mDb = mClient.GetDatabase(dbName);
			//widgetColl = mDb.GetCollection<Widget>("widgets");
		}


		public void Migration(bool addUpdated = false, bool deleteSuccessfuls = false)
		{

			failColls.Clear();
			var colls = mDb.ListCollections();
			var resume = true;
			// SKIP system.js nam and system.profile nam and pages nam 
			var collsToAvoid = new List<string>() { "system.js", "system.profile", "pages" };
			foreach (var item in colls.ToList())
			{
				successIds.Clear();
				failIds.Clear();
				var typ = item.GetValue("type").ToString();
				if (!typ.Equals("collection")) continue;

				var nam = item.GetValue("name").ToString();
				if (collsToAvoid.Contains(nam) || !resume)
				{
					Console.WriteLine("skipping " + nam);
					continue;
				}

				Console.WriteLine(nam);

				// need to test SavedUnits202 and Units and FanCurves?  -- note from 2017.08.04 evening vs dev mongo db remote work
				MigrateColl(nam, addUpdated, deleteSuccessfuls);
				successColls.Add(nam.ToString());

			}


		}

		private void MigrateColl(string collName, bool addUpdated, bool deleteSuccessfuls)
		{
			var coll = mDb.GetCollection<BsonDocument>(collName);
			var hereWeGo = coll.AsQueryable().Where(o => true);
			//var hereWeGo = coll.Find(_ => true). .ToList();
			var count = 0;
			var dt = DateTime.Now;
			foreach (var item in hereWeGo)
			{
				count++;
				if (DateTime.Now.Subtract(dt) > new TimeSpan(0, 0, 30))
				{
					dt = DateTime.Now;
					Console.Write(" " + count);
				}
				var id = item.GetValue("_id").ToString();
				// {{ "_id" : ObjectId("59831ad8858dcf1f802d9a6d"), "Things" : [{ "_id" : ObjectId("59831ad8858dcf1f802d9a6e"), "Location" : "head" }, { "_id" : ObjectId("59831ad8858dcf1f802d9a6f"), "Location" : "16" }, { "_id" : ObjectId("59831ad8858dcf1f802d9a70"), "Location" : "0" }, { "_id" : ObjectId("59831ad8858dcf1f802d9a71"), "Location" : "26" }, { "_id" : ObjectId("59831ad8858dcf1f802d9a72"), "Location" : "10" }, { "_id" : ObjectId("59831ad8858dcf1f802d9a73"), "Location" : "39" }], "Name" : "abe", "SomeValue" : 0, "LastSave" : ISODate("2017-08-03T12:45:12.677Z") }}
				//var strang = item.ToString();
				var sb = new StringBuilder(item.ToString());

				var objIdIdx = sb.ToString().IndexOf("ObjectId(");

				var hitCount = 0;
				while (objIdIdx > -1)
				{
					hitCount++;
					var closingParen = sb.ToString().IndexOf(")", objIdIdx);
					sb.Remove(closingParen, 1);
					sb.Remove(objIdIdx, 9); // work backwards yo		
					objIdIdx = sb.ToString().IndexOf("ObjectId(");
				}
				//Console.Write(GetNumbah(hitCount));
				BsonDocument result;
				var effort = BsonDocument.TryParse(sb.ToString(), out result);
				if (effort)
				{
					successIds.Add(id);
					if (addUpdated) coll.InsertOne(result);
				}
				else
				{
					failIds.Add(new Tuple<string, string>(id, "failed to parse bsondocument"));
					Console.WriteLine(failIds.Last().Item1 + "  " + failIds.Last().Item2);
				}
				if (deleteSuccessfuls)
				{
					var f = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(id.ToString()));
					//var fubu = coll.Find(f).ToList();
					var delResult = coll.DeleteOne(f);
					if (delResult.DeletedCount != 1 | !delResult.IsAcknowledged)
					{
						failIds.Add(new Tuple<string, string>(id, "database failed to delete/acknowledge deletion"));
					}
				}
			}
			Console.WriteLine(count + " docs");
			Console.WriteLine(failIds.Count + " failed doc translations");
		}

		private string GetNumbah(int hitCount)
		{
			return (hitCount / 10).ToString();			
		}
	}
}