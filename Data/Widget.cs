using Data.Mongo;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using System.Collections.Generic;

namespace Data
{
	public class Widget : MongoDoc
	{
		public Widget()
		{
			Things = new List<Thing>();

		}
		public string Name { get; set; }
		public int SomeValue { get; set; }
		public List<Thing> Things { get; set; }
	}
	public class Thing
	{
		public string Location { get; set; }
	}

}
