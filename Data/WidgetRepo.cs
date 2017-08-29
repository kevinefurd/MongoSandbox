using Data.Mongo;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Data
{
	public class WidgetRepo : MongoRepo<Widget>
	{
		public WidgetRepo(string connectString, string dbName) : base(connectString, dbName, typeof(Widget).Name) { }
		public IEnumerable<Widget> GetByName(string name)
		{
			var result = Coll.Find(
				filter: o => o.Name.Equals(name));
			return result.ToList();
		}

		public void PopulateFakes(int count)
		{
			var rand = new Random(0);
			for (int i = 0; i < count; i++)
			{
				var w = new Widget();
				w.Name = GetSillyString(rand);
				w.SomeValue = rand.Next(0, 100);
				for (int j = 0; j < 50; j++)
				{
					w.Things.Add(new Thing() { Location = GetSillyString(rand) });
				}
				Upsert(w);
			}
		}
		private string GetSillyString(Random rand)
		{
			return ((char)rand.Next(97, 123)) + ((char)rand.Next(97, 123)) + ((char)rand.Next(97, 123)).ToString();
		}

	}
}
