using System;
using MongoDB.Bson.Serialization;

namespace Data.Mongo
{
	internal class CustomSerializer : IBsonSerializationProvider
	{
		public IBsonSerializer GetSerializer(Type type)
		{
			if (type == typeof(ObjectId))
			{
				return new TopssObjectIdSer();
			}

			return null;
		}


	}
}