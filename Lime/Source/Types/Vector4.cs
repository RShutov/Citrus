﻿using System;
using ProtoBuf;

namespace Lime
{
	[ProtoContract]
	public struct Vector4 : IEquatable<Vector4>
	{
		[ProtoMember(1)]
		public float X;

		[ProtoMember(2)]
		public float Y;

		[ProtoMember(3)]
		public float Z;

		[ProtoMember(4)]
		public float W;

		public Vector3 XYZ { get { return new Vector3(X, Y, Z); } }

		public float Length
		{
			get { return Mathf.Sqrt(SqrLength); }
		}

		public float SqrLength
		{
			get { return X * X + Y * Y + Z * Z + W * W; }
		}

		public Vector4 Normalized
		{
			get
			{
				var v = new Vector4(X, Y, Z, W);
				var length = Length;
				if (length > 0) {
					v /= length;
				}
				return v;
			}
		}

		public Vector4(float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public Vector4(Vector3 value, float w)
			: this(value.X, value.Y, value.Z, w)
		{
		}

		public override bool Equals(object obj)
		{
			return obj is Vector4 && Equals((Vector4)obj);
		}

		public bool Equals(Vector4 value)
		{
			return this == value;
		}

		public static float DotProduct(Vector4 value1, Vector4 value2)
		{
			return
				value1.X * value2.X +
				value1.Y * value2.Y +
				value1.Z * value2.Z +
				value1.W * value2.W;
		}

		public static Vector4 operator -(Vector4 value)
		{
			return new Vector4(-value.X, -value.Y, -value.Z, -value.W);
		}

		public static bool operator ==(Vector4 value1, Vector4 value2)
		{
			return value1.W == value2.W
				&& value1.X == value2.X
				&& value1.Y == value2.Y
				&& value1.Z == value2.Z;
		}

		public static bool operator !=(Vector4 value1, Vector4 value2)
		{
			return !(value1 == value2);
		}

		public static Vector4 operator +(Vector4 value1, Vector4 value2)
		{
			value1.W += value2.W;
			value1.X += value2.X;
			value1.Y += value2.Y;
			value1.Z += value2.Z;
			return value1;
		}

		public static Vector4 operator -(Vector4 value1, Vector4 value2)
		{
			value1.W -= value2.W;
			value1.X -= value2.X;
			value1.Y -= value2.Y;
			value1.Z -= value2.Z;
			return value1;
		}

		public static Vector4 operator *(Vector4 value1, Vector4 value2)
		{
			value1.W *= value2.W;
			value1.X *= value2.X;
			value1.Y *= value2.Y;
			value1.Z *= value2.Z;
			return value1;
		}

		public static Vector4 operator *(Vector4 value, float scaleFactor)
		{
			value.W *= scaleFactor;
			value.X *= scaleFactor;
			value.Y *= scaleFactor;
			value.Z *= scaleFactor;
			return value;
		}

		public static Vector4 operator *(float scaleFactor, Vector4 value)
		{
			value.W *= scaleFactor;
			value.X *= scaleFactor;
			value.Y *= scaleFactor;
			value.Z *= scaleFactor;
			return value;
		}

		public static Vector4 operator /(Vector4 value1, Vector4 value2)
		{
			value1.W /= value2.W;
			value1.X /= value2.X;
			value1.Y /= value2.Y;
			value1.Z /= value2.Z;
			return value1;
		}

		public static Vector4 operator /(Vector4 value1, float divider)
		{
			float factor = 1f / divider;
			value1.W *= factor;
			value1.X *= factor;
			value1.Y *= factor;
			value1.Z *= factor;
			return value1;
		}
	}
}