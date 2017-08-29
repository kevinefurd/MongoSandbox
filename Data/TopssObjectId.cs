using System;
using System.Linq;
using System.Threading;
using MongoId = MongoDB.Bson.ObjectId;
namespace Data
{
	//[Serializable]
	public struct ObjectId : IComparable<ObjectId>, IEquatable<ObjectId>, IConvertible
	{
		private static ObjectId _empty = new ObjectId("000000000000000000000000"); // getting away from mongo usage
		public static ObjectId Empty { get { return _empty; } }
		private string _value;

		/// <summary>
		/// keeping this private for now since this isn't in keeping with Mongo ObjectId. just use .ToString().
		/// </summary>
		private string Value
		{
			get { _value = _value ?? Empty.ToString(); return _value; }
			set { _value = value; }
		}
		public ObjectId(string _id)
		{
			_value = _id;
		}
		public ObjectId(ObjectId _id)
		{
			_value = _id.ToString();
		}

		// can't have these two (even pviate!) without having to ref mongo in calling projects
		//public static explicit operator Mid(TopssObjectId id)
		//{
		//	return new Mid(id.ToString());
		//}
		//public static explicit operator TopssObjectId(Mid id)
		//{
		//	return new TopssObjectId(id);
		//}

		public static explicit operator string(ObjectId _id)
		{
			return _id.ToString();
		}

		public override string ToString()
		{
			return Value;
		}
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null)) return false;

			string s = obj as string;
			if (s != null) return this.ToString().Equals(s);
			try
			{
				ObjectId t = (ObjectId)obj;
				return ToString().Equals(t.ToString());
			}
			catch
			{
				return false;
			}
		}
		const string chars = "abcdef0123456789";
		static Semaphore semaphore = new Semaphore(5, 5);
		static Random random = new Random();
		public static ObjectId GenerateNewId()
		{
			semaphore.WaitOne();
			//return new ObjectId(Mid.GenerateNewId().ToString());
			//return new ObjectId("0");
			try
			{
				return new ObjectId(
					new string(Enumerable.Repeat(chars, 24)
					.Select(s => s[random.Next(s.Length)]).ToArray()));

			}
			finally
			{
				semaphore.Release(1);
			}
		}

		public static explicit operator ObjectId(string v)
		{
			return new ObjectId(v);
		}

		public static bool operator ==(ObjectId lhs, ObjectId rhs)
		{
			if (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null)) return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null)) return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(ObjectId lhs, ObjectId rhs)
		{
			return !Equals(lhs, rhs);
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		// must parse into mongo object ids in order to be considered a sucessful parse.

		public static bool TryParse(string name, out ObjectId userId)
		{
			userId = Empty;
			try
			{
				userId = new ObjectId(new MongoId(name).ToString());
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static ObjectId Parse(string id)
		{
			return new ObjectId(new MongoId(id).ToString());
		}

		/// <summary>
		/// impl of IEquatable
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(ObjectId other)
		{
			return ToString().CompareTo(other.ToString());
		}

		/// <summary>
		/// impl of IComparable
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(ObjectId other)
		{
			return ToString().Equals(other.ToString());
		}

		#region [impl of IConvertable]
		public TypeCode GetTypeCode()
		{
			return TypeCode.String;
		}

		public bool ToBoolean(IFormatProvider provider)
		{
			throw new NotImplementedException();
		}

		public char ToChar(IFormatProvider provider)
		{
			throw new NotImplementedException();
		}

		public sbyte ToSByte(IFormatProvider provider)
		{
			throw new NotImplementedException();
		}

		public byte ToByte(IFormatProvider provider)
		{
			throw new NotImplementedException();
		}

		public short ToInt16(IFormatProvider provider)
		{
			throw new NotImplementedException();
		}

		public ushort ToUInt16(IFormatProvider provider)
		{
			throw new NotImplementedException();
		}

		public int ToInt32(IFormatProvider provider)
		{
			throw new NotImplementedException();
		}

		public uint ToUInt32(IFormatProvider provider)
		{
			throw new NotImplementedException();
		}

		public long ToInt64(IFormatProvider provider)
		{
			throw new NotImplementedException();
		}

		public ulong ToUInt64(IFormatProvider provider)
		{
			throw new NotImplementedException();
		}

		public float ToSingle(IFormatProvider provider)
		{
			throw new NotImplementedException();
		}

		public double ToDouble(IFormatProvider provider)
		{
			throw new NotImplementedException();
		}

		public decimal ToDecimal(IFormatProvider provider)
		{
			throw new NotImplementedException();
		}

		public DateTime ToDateTime(IFormatProvider provider)
		{
			throw new NotImplementedException();
		}

		public string ToString(IFormatProvider provider)
		{
			return Value;
		}

		public object ToType(Type conversionType, IFormatProvider provider)
		{
			switch (conversionType.Name)
			{
				case "String":
					return Value;
				default:
					throw new NotImplementedException();
			}
		}

		#endregion
	}

}
