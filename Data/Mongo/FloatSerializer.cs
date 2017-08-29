using System;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Data.Mongo
{
	internal class FloatSerializer : SerializerBase<float>
	{

		public override float Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			return float.Parse(context.Reader.ReadDouble().ToString());

		}

		public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, float value)
		{
			context.Writer.WriteDouble(double.Parse(value.ToString()));
		}
	}

	public class SecondFloatSerializer : IBsonSerializer
	{
		public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
		{
			return (float)context.Reader.ReadDouble();
		}

		public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
		{
			context.Writer.WriteDouble((float)value);
		}

		public Type ValueType { get { return typeof(float); } }
	}
}