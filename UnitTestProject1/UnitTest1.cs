using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
//using MongoDB.Bson;
using Data;
using System.Collections.Generic;

namespace UnitTestProject2
{
	[TestClass]
	public class UnitTest1
	{

		private static string _connectString = "mongodb://localhost";
		private WidgetRepo repo;

		[TestInitialize]
		public void Init()
		{
			repo = new WidgetRepo(_connectString, "testing");
		}
		[TestMethod]
		public void Reset_Coll_And_Create_Fakes_Test()
		{
			repo.ResetCollection();
			repo.PopulateFakes(2500);
		}
		[TestMethod]
		public void Get_All_Edit_First_Save_And_Verify_Changed()
		{
			var all = repo.GetAll();
			var first = all.First();
			first.ExtraElements.Add("abc", 123);
			var rand = new Random();
			first.Name = rand.Next(1000, 10000).ToString();
			repo.Upsert(all.First());
			var verify = repo.GetOne(first.Id);
			Assert.IsTrue(verify.Id.Equals(first.Id) && verify.Name.Equals(first.Name));
			var verifyAgai = repo.GetByName(first.Name).First();
			Assert.IsTrue(verifyAgai.Id.Equals(first.Id) && verifyAgai.Name.Equals(first.Name));

		}

		[TestMethod]
		public void Create_New_Widget()
		{
			var w = new Widget() { Name = "alonzo" };
			repo.Upsert(w);
			var q = repo.GetByName("alonzo").Last();
			Assert.IsTrue(q.Id.Equals(w.Id));
			w.Name = "changin'";
			repo.Upsert(w);
			q = repo.GetByName("changin'").Last();
			Assert.IsTrue(q.Id.Equals(w.Id) && q.Name.Equals(w.Name));
			q = repo.GetOne(w.Id);
			Assert.IsTrue(w.Id.Equals(q.Id) && w.Name.Equals(q.Name));
		}

		[TestMethod]
		public void Delete_By_Found_Ids()
		{
			var all = repo.GetAll();
			if (!all.Any()) Assert.Inconclusive("no recs to test against");
			var ids = all.Select(o => o.Id).ToList();
			repo.Delete(ids);
			all = repo.GetAll();
			Assert.IsFalse(all.Any());
		}

		[TestMethod]
		public void Float_vs_Double_Test()
		{
			// in place of atrtibs, have to use a custom float serializer
			var r = new Random();
			var time = new TimeSpan(0, 0, 30);
			var dt = DateTime.Now;
			Int64 iters = 0;
			while (DateTime.Now.Subtract(dt) < time)
			{
				var all = repo.GetAll();
				if (!all.Any()) Assert.Inconclusive("no records against which to test");
				foreach (var item in all)
				{
					iters++;
					double d = item.Floater;
					Assert.IsTrue(d.Equals(item.Floater));
					float f = float.Parse(r.Next(0, 10000).ToString() + "." + r.Next(0, 10000000).ToString());
					item.Floater = f;

					repo.Upsert(item);
					var checkin = repo.GetOne(item.Id);
					Assert.IsTrue(Math.Abs(checkin.Floater - f) < .001); // very stupid test. needs fixed
				}
			}
			Console.WriteLine("iterations: " + iters);
		}
		[TestMethod]
		public void GetMultiple()
		{
			var all = repo.GetAll().ToList();
			if (!all.Any()) Assert.Inconclusive("no recs to test against.");
			var some = new[] { all[1].Id, all[3].Id, all[5].Id };
			var someWidgets = repo.Get(some);
			var someWidgetIds = new List<ObjectId>();
			foreach (var w in someWidgets)
			{
				someWidgetIds.Add(w.Id);
			}
			foreach (var s in some)
			{
				Assert.IsTrue(someWidgetIds.Contains(s));
			}
		}

	}
}

