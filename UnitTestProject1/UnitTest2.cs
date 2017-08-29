using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTestProject2
{
	[TestClass]
	public class UnitTest2
	{

		[TestMethod]
		public void ObjectId_Tests()
		{
			var id = ObjectId.GenerateNewId();
			MongoDB.Bson.ObjectId mobjid;
			Assert.IsTrue(
				MongoDB.Bson.ObjectId.TryParse(id.ToString(), out mobjid),
				"failed to convert custom object id string to mongo object id");
			Assert.AreEqual(id.ToString(), mobjid.ToString());

			var id2 = ObjectId.GenerateNewId();
			Assert.AreNotEqual(id.GetHashCode(), id2.GetHashCode(), ".GetHashCode() problems");
			Assert.AreNotEqual(id.ToString(), id2.ToString(), ".ToString() problems");
			var eq = id == id2;
			Assert.IsFalse(eq, "equality operators have problems");
			eq = id != id2;
			Assert.IsTrue(eq, "equality operators have problems");
			id = new ObjectId(id2);
			Assert.AreEqual(id.GetHashCode(), id2.GetHashCode(), "GetHashCode() problems");
			Assert.AreEqual(id.ToString(), id2.ToString(), "ToString() problems");
			eq = id == id2;
			Assert.IsTrue(eq, "equality operators have problems");
			eq = id != id2;
			Assert.IsFalse(eq, "equality operators have problems");

			// diff instances of object id empty equate
			var fin = ObjectId.Empty;
			var fon = ObjectId.Empty;
			Assert.AreEqual(fin, fon, "Empty instance does not equal separate Empty instance");


			var a = (ObjectId)"1234";
			Assert.IsTrue(a.ToString() == "1234", "string failed to correctly convert to objectid.");
			var b = (string)a;
			Assert.IsTrue(b.Equals("1234"), "objectid failed to correctly convert to string");
			var toi = ObjectId.GenerateNewId();
			var e = (string)toi;
			Assert.IsTrue(e.Equals(toi.ToString()), "equality operators have problems");
		}
		[TestMethod]
		public void ObjectId_Gen_Unique_Test()
		{
			var dict = new List<string>();
			var a = new Action(() =>
			{
				for (int i = 0; i < 25000; i++)
				{

					var id = ObjectId.GenerateNewId().ToString();
					Assert.IsNotNull(id);					
					Assert.IsFalse(dict.Contains(id));
					dict.Add(id);

				}
			});
			var tasks = new List<Task>();
			for (int i = 0; i < 1; i++)
			{
				var t = new Task(a);
				tasks.Add(t);
				t.Start();
			}
			Task.WaitAll(tasks.ToArray());
			Assert.IsFalse(dict.Contains(null));
			//var complete = false;
			//while (!complete)
			//{
			//	complete = tasks[0].IsCompleted && tasks[1].IsCompleted && tasks[2].IsCompleted && tasks[3].IsCompleted;
			//	System.Threading.Thread.Sleep(250);
			//}
		}
	}

}
