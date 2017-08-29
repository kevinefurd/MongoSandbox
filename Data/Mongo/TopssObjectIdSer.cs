using System;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Data.Mongo
{
	internal class TopssObjectIdSer : SerializerBase<ObjectId>
	{
		public override ObjectId Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			return new ObjectId(context.Reader.ReadObjectId().ToString());
		}

		public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, ObjectId value)
		{
			context.Writer.WriteObjectId(new MongoDB.Bson.ObjectId(value.ToString()));
		}
	}
}