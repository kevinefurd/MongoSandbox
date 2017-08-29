using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Data.Mongo
{
	public class MongoRepo<T> where T : MongoDoc
	{
		protected MongoClient mClient;
		protected IMongoDatabase mDb;
		protected IMongoCollection<T> Coll { get { _coll = _coll ?? mDb.GetCollection<T>(_collectionName); return _coll; } }

		protected string _collectionName;
		protected string _dbName;
		private IMongoCollection<T> _coll;
		private static bool _inited;
		public MongoRepo(string connectString, string dbName, string collectionName)
		{
			_dbName = dbName;
			_collectionName = collectionName;

			mClient = new MongoClient(connectString);
			mDb = mClient.GetDatabase(dbName);

			if (_inited) return;
			// INIT ONCE

			// REG SERIALIZERS BEFORE CLASS MAPS -- mongo will make a ser for your weird objects if you don't and hten it beefs.

			// this is the sauce that gives us custom ser/deser for our object. even works against an id field!
			BsonSerializer.RegisterSerializer(typeof(ObjectId), new TopssObjectIdSer());

			BsonSerializer.RegisterSerializer(typeof(float), new FloatSerializer());

			BsonClassMap.RegisterClassMap<MongoDoc>(cm =>
			{
				cm.AutoMap();
				cm.MapMember(c => c.MySillyEnum).SetDefaultValue(SillyEnum.golf | SillyEnum.carts | SillyEnum.are | SillyEnum.baller);
				cm.MapMember(c => c.MySimpleDefault).SetDefaultValue("beerplease");
				//var o = new RepresentationConverter(false, true);	
				//cm.MapMember(c => c.Wingy).SetSerializer(
				//	new DictionaryInterfaceImplementerSerializer<Dictionary<string, object>>(DictionaryRepresentation.ArrayOfDocuments));
			});
			// prefer this over mongo attribs so we can keep mongo deps out of our classes
			// if you stick with names mongo knows like Id and ExtraElements, this stuff isn't necessary 99% of the time.
			//if (!BsonClassMap.IsClassMapRegistered(typeof(MongoDoc)))
			//{
			//	BsonClassMap.RegisterClassMap<MongoDoc>(cm =>
			//	{
			//		cm.AutoMap();
			//		cm.SetIdMember(cm.GetMemberMap(c => c.Id));
			//		// below saves upon initial saving of things such that you don't have to do this again unless you recreate the repo maybe. i'm learning lots.
			//		cm.SetExtraElementsMember(cm.GetMemberMap(c => c.Catch));
			//	});
			//}
			_inited = true;

		}
		public void ResetCollection()
		{
			mDb.DropCollection(_collectionName);
			mDb.CreateCollection(_collectionName);
		}

		private void Prep(T t)
		{
			t.LastSaved = DateTime.Now;
			t.Id = t.Id.Equals(ObjectId.Empty) ? ObjectId.GenerateNewId() : t.Id;
		}
		public T Insert(T t)
		{
			Prep(t);
			Coll.InsertOne(t);
			return t;
		}
		// TODO  make ref param
		public T Upsert(T t)
		{
			Prep(t);

			var result = Coll.ReplaceOne(
				o => o.Id.Equals(t.Id),
				options: new UpdateOptions { IsUpsert = true },
				replacement: t);
			return t;
		}

		public IEnumerable<T> GetAll()
		{
			return Coll.Find(Builders<T>.Filter.Empty).ToList();
		}
		public T GetOne(string id)
		{
#if USE_OBJECT_ID
			//return Coll.Find(Builders<T>.Filter.Eq(o => o.Id, new ObjectId(id))).FirstOrDefault();
			return Coll.Find(Builders<T>.Filter.Eq(o => o.Id, new ObjectId(id))).FirstOrDefault();
#else
			return Coll.Find(Builders<T>.Filter.Eq(o => o.Id, id)).FirstOrDefault();
#endif
		}
		public T GetOne(ObjectId id)
		{
			return GetOne(id.ToString());
		}
		public void Delete(IEnumerable<ObjectId> ids)
		{
			var filter = Builders<T>.Filter.In(o => o.Id, ids);
			_coll.DeleteMany(filter);

		}
		public IEnumerable<T> Get(ObjectId[] ids)
		{
			return Coll.Find(Builders<T>.Filter.In(o => o.Id, ids)).ToList();
		}
	}
}
