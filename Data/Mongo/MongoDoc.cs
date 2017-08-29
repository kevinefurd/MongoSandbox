using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Data.Mongo
{
	public class MongoDoc
	{
		private static Random _random = new Random();

		protected bool SomeBewl { get; set; } // won't save to mongo; it's not public
		public float Floater { get; set; }
		public IDictionary<string, object> ExtraElements { get; set; }
		public ObjectId Id { get; set; }
		//public Dictionary<string, object> ExtraElements { get; set; }
		public DateTime Created { get; protected set; }
		public MongoDoc()
		{
			Id = ObjectId.GenerateNewId();
			Created = DateTime.Now;
			ExtraElements = new Dictionary<string, object>();
			Wingy = new ThingyWingy();
			Wingy.Add("abc", "def");
			Wingy.Add("123", "xxz");
			Floater = float.Parse(_random.Next(0, 10000).ToString() + "." + _random.Next(0, 10000000).ToString());
			//if (r.Next(2) == 1) QuestionFloater = null;
		}
		public MongoDoc(string id) : this()
		{
			Id = new ObjectId(id);
		}
		public DateTime LastSaved { get; set; }
		public SillyEnum MySillyEnum { get; set; }
		public string MySimpleDefault { get; set; }
		public ThingyWingy Wingy { get; set; }
	}
	public class ThingyWingy : Dictionary<string, object>
	{

	}
	public enum SillyEnum
	{
		golf = 1,
		carts = 2,
		are = 4,
		baller = 8
	}
}
