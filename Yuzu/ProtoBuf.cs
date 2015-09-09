﻿using System;
using System.Text;

namespace Yuzu
{
	internal enum WireType {
		Varint = 0,
		Double = 1,
		LengthDelimited = 2,
		StartGroup = 3,
		EndGroup = 4,
		Single = 5,
	}

	public class ProtobufSerializeOptions
	{
	}

	public class ProtobufSerializer : AbstractWriterSerializer
	{
		public ProtobufSerializeOptions ProtobufOptions = new ProtobufSerializeOptions();

		private void WriteVarint(long value)
		{
			do {
				byte b = (byte)(value & 0x7f);
				value >>= 7;
				if (value != 0) {
					b |= 0x80;
				}
				writer.Write(b);
			} while (value != 0);
		}

		protected override void ToWriter(object obj)
		{
			if (Options.ClassNames)
				throw new NotImplementedException();

			int count = 1;
			foreach (var yi in Utils.GetYuzuItems(obj.GetType(), Options)) {
				if (yi.Type == typeof(int) || yi.Type == typeof(uint)) {
					WriteVarint((count << 3) + (int)WireType.Varint);
					WriteVarint((int)yi.GetValue(obj));
				}
				else if (yi.Type == typeof(string)) {
					var s = yi.GetValue(obj).ToString();
					WriteVarint((count << 3) + (int)WireType.LengthDelimited);
					WriteVarint(Encoding.UTF8.GetByteCount(s));
					writer.Write(Encoding.UTF8.GetBytes(s));
				}
				else {
					throw new NotImplementedException(yi.Type.Name);
				}
				++count;
			}
		}
	}

	public class ProtobufDeserializeOptions
	{
	}

	public class ProtobufDeserializer : AbstractReaderDeserializer
	{
		public ProtobufDeserializeOptions ProtobufOptions = new ProtobufDeserializeOptions();

		private long ReadVarint()
		{
			long result = 0;
			int shift = 0;
			byte b;
			do {
				b = Reader.ReadByte();
				result |= (long)(b & 0x7f) << shift;
				shift += 7;
			} while ((b & 0x80) != 0);
			return result;
		}

		public override object FromReaderInt()
		{
			throw new NotImplementedException();
		}

		public override object FromReaderInt(object obj)
		{
			if (Options.ClassNames)
				throw new NotImplementedException();

			int count = 1;
			foreach (var yi in Utils.GetYuzuItems(obj.GetType(), Options)) {
				if (yi.Type == typeof(int) || yi.Type == typeof(uint)) {
					if (ReadVarint() != (count << 3) + (int)WireType.Varint)
						throw new YuzuException();
					yi.SetValue(obj, (int)ReadVarint());
				}
				else if (yi.Type == typeof(string)) {
					if (ReadVarint() != (count << 3) + (int)WireType.LengthDelimited)
						throw new YuzuException();
					yi.SetValue(obj, Encoding.UTF8.GetString(Reader.ReadBytes((int)ReadVarint())));
				}
				else {
					throw new NotImplementedException(yi.Type.Name);
				}
				++count;
			}
			return obj;
		}
	}
}
