﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NBitcoin.DataEncoders;

namespace NBitcoin
{
	public class base_uint256
	{
		protected const int WIDTH = 256 / 32;
		protected const int WIDTH_BYTE = 256 / 8;
		protected internal UInt32[] pn = new UInt32[WIDTH];

		public void SetHex(string str)
		{
			Array.Clear(pn, 0, pn.Length);
			str = str.TrimStart();

			int i = 0;
			if(str.Length >= 2)
				if(str[0] == '0' && char.ToLower(str[1]) == 'x')
					i += 2;

			int pBegin = i;
			while(i < str.Length && HexEncoder.IsDigit(str[i]) != -1)
				i++;

			i--;

			int p1 = 0;
			int pend = p1 + WIDTH * 4;
			while(i >= pBegin && p1 < pend)
			{
				SetByte(p1, (byte)HexEncoder.IsDigit(str[i]));
				i--;
				if(i >= pBegin)
				{
					byte n = (byte)HexEncoder.IsDigit(str[i]);
					n = (byte)(n << 4);
					SetByte(p1, (byte)(GetByte(p1) | n));
					i--;
					p1++;
				}
			}
		}
		protected void SetByte(int index, byte value)
		{
			var uintIndex = index / sizeof(uint);
			var byteIndex = index % sizeof(uint);

			var currentValue = pn[uintIndex];
			var mask = ((uint)0xFF << (byteIndex * 8));
			currentValue = currentValue & ~mask;
			var shiftedValue = (uint)value << (byteIndex * 8);
			currentValue |= shiftedValue;
			pn[uintIndex] = currentValue;
		}

		public string GetHex()
		{
			StringBuilder builder = new StringBuilder();
			for(int i = 0 ; i < WIDTH_BYTE; i++)
			{
				var b = GetByte(WIDTH_BYTE - i - 1);
				builder.Append(b.ToString("x2"));
			}
			return builder.ToString();
		}
		
		public byte GetByte(int index)
		{
			var uintIndex = index / sizeof(uint);
			var byteIndex = index % sizeof(uint);
			var value = pn[uintIndex];
			return (byte)(value >> (byteIndex * 8));
		}

		public override string ToString()
		{
			return GetHex();
		}

	}
	public class uint256 : base_uint256,  IBitcoinSerializable
	{

		public uint256()
		{
			for(int i = 0 ; i < WIDTH ; i++)
				pn[i] = 0;
		}

		public uint256(base_uint256 b)
		{
			for(int i = 0 ; i < WIDTH ; i++)
				pn[i] = b.pn[i];
		}

		//uint256 operator=(base_uint256& b)
		//{
		//	for (int i = 0; i < WIDTH; i++)
		//		pn[i] = b.pn[i];
		//	return  *this;
		//}


		public uint256(ulong b)
		{
			pn[0] = (uint)b;
			pn[1] = (uint)(b >> 32);
			for (int i = 2; i < WIDTH; i++)
				pn[i] = 0;
		}
		public uint256(byte[] vch, bool lendian = true)
		{
			if(!lendian)
				vch = vch.Reverse().ToArray();
			if(vch.Length == pn.Length * 4)
			{
				for(int i = 0, y = 0 ; i < pn.Length && y < vch.Length ; i++, y += 4)
				{
					pn[i] = BitConverter.ToUInt32(vch, y);
				}
			}
			else
				throw new FormatException("the byte array should be 256 byte long");
		}

		public uint256(string str)
		{
			SetHex(str);
		}

		public uint256(byte[] vch)
		{
			if(vch.Length == pn.Length * 4)
			{
				for(int i = 0, y = 0 ; i < pn.Length && y < vch.Length ; i++, y += 4)
				{
					pn[i] = BitConverter.ToUInt32(vch, y);
				}
			}
			else
				throw new FormatException("the byte array should be 256 byte long");
		}

		public override bool Equals(object obj)
		{
			var item = obj as uint256;
			if(item == null)
				return false;
			return AreEquals(pn, item.pn);
		}
		public static bool operator ==(uint256 a, uint256 b)
		{
			if(System.Object.ReferenceEquals(a, b))
				return true;
			if(((object)a == null) || ((object)b == null))
				return false;
			return AreEquals(a.pn, b.pn);
		}

		private static bool AreEquals(uint[] ar1, uint[] ar2)
		{
			if(ar1.Length != ar2.Length)
				return false;
			for(int i = 0 ; i < ar1.Length ; i++)
			{
				if(ar1[i] != ar2[i])
					return false;
			}
			return true;
		}

		public static bool operator <(uint256 a, uint256 b)
		{

			return Comparison(a, b) < 0;

		}

		public static bool operator >(uint256 a, uint256 b)
		{

			return Comparison(a, b) > 0;

		}

		public static bool operator <=(uint256 a, uint256 b)
		{

			return Comparison(a, b) <= 0;

		}

		public static bool operator >=(uint256 a, uint256 b)
		{

			return Comparison(a, b) >= 0;

		}

		private static int Comparison(uint256 a, uint256 b)
		{
			 for (int i = WIDTH-1; i >= 0; i--)
			{
				if (a.pn[i] < b.pn[i])
					return -1;
				else if (a.pn[i] > b.pn[i])
					return 1;
			}
			return 0;
		}

		public static bool operator !=(uint256 a, uint256 b)
		{
			return !(a == b);
		}

		public static bool operator ==(uint256 a, ulong b)
		{
			return (a == new uint256(b));
		}
		public static bool operator !=(uint256 a, ulong b)
		{
			return !(a == new uint256(b));
		}
		public static uint256 operator ^(uint256 a, uint256 b)
		{
			var c = new uint256();
			c.pn = new uint[a.pn.Length];
			for(int i = 0 ; i < c.pn.Length ; i++)
			{
				c.pn[i] = a.pn[i] ^ b.pn[i];
			}
			return c;
		}

		public static bool operator!(uint256 a)
	    {
	     for (int i = 0; i < WIDTH; i++)
	         if (a.pn[i] != 0)
	             return false;
	     return true;
	   }

	    public static uint256 operator-(uint256 a, uint256 b)
    {
		return a + (-b);
    }

	   public static uint256 operator+(uint256 a, uint256 b)
    {
		var result = new uint256();
        ulong carry = 0;
        for (int i = 0; i < WIDTH; i++)
        {
            ulong n = carry + a.pn[i] + b.pn[i];
            result.pn[i] = (uint)(n & 0xffffffff);
            carry = n >> 32;
        }
        return result;
    }

	public static uint256 operator+(uint256 a, ulong b)
    {
		return a + new uint256(b);
    }

	

	public static implicit operator uint256(ulong value)
	{
		return new uint256(value);
	}

		public static uint256 operator &(uint256 a, uint256 b)
		{
			var n = new uint256(a);
			for(int i = 0 ; i < WIDTH ; i++)
				n.pn[i] &= b.pn[i];
			return n;
		}
		public static uint256 operator |(uint256 a, uint256 b)
		{
			var n = new uint256(a);
			for(int i = 0 ; i < WIDTH ; i++)
				n.pn[i] |= b.pn[i];
			return n;
		}
		public static uint256 operator <<(uint256 a, int shift)
		{
			var result = new uint256();
			int k = shift / 32;
			shift = shift % 32;
			for(int i = 0 ; i < WIDTH ; i++)
			{
				if(i + k + 1 < WIDTH && shift != 0)
					result.pn[i + k + 1] |= (a.pn[i] >> (32 - shift));
				if(i + k < WIDTH)
					result.pn[i + k] |= (a.pn[i] << shift);
			}
			return result;
		}

		public static uint256 operator >>(uint256 a, int shift)
		{
			var result = new uint256();
			int k = shift / 32;
			shift = shift % 32;
			for(int i = 0 ; i < WIDTH ; i++)
			{
				if(i - k - 1 >= 0 && shift != 0)
					result.pn[i - k - 1] |= (a.pn[i] << (32 - shift));
				if(i - k >= 0)
					result.pn[i - k] |= (a.pn[i] >> shift);
			}
			return result;
		}

		
		public static uint256 operator ~(uint256 a)
		{
			var b = new uint256();
			for(int i = 0 ; i < b.pn.Length ; i++)
			{
				b.pn[i] = ~a.pn[i];
			}
			return b;
		}
		public static uint256 operator -(uint256 a)
		{
			var b = new uint256();
			for(int i = 0 ; i < b.pn.Length ; i++)
			{
				b.pn[i] = ~a.pn[i];
			}
			b++;
			return b;
		}

		 public static uint256 operator ++(uint256 a)
		{
			var ret = new uint256(a);
			return a + new uint256(1);
		}
		public static uint256 operator --(uint256 a)
		{
			return a - 1;
		}
		
		public byte[] ToBytes(bool lendian = true)
{
	var copy = new byte[WIDTH_BYTE];
	for(int i = 0 ; i < WIDTH_BYTE ; i++)
	{
		copy[i] = GetByte(i);
	}
	if(!lendian)
		Array.Reverse(copy);
	return copy;
}

		public void ReadWrite(BitcoinStream stream)
		{
			if(stream.Serializing)
			{
				var b = ToBytes();
				stream.ReadWrite(ref b);
			}
			else
			{
				byte[] b = new byte[WIDTH_BYTE];
				stream.ReadWrite(ref b);
				this.pn = new uint256(b).pn;
			}
		}

		public void Serialize(Stream stream, int nType = 0, ProtocolVersion protocolVersion = ProtocolVersion.PROTOCOL_VERSION)
		{
			for(int i = 0 ; i < WIDTH_BYTE ; i++)
			{
				stream.WriteByte(GetByte(i));
			}
		}

		public void Unserialize(Stream stream, int nType = 0, ProtocolVersion protocolVersion = ProtocolVersion.PROTOCOL_VERSION)
		{
			for(int i = 0 ; i < WIDTH_BYTE ; i++)
			{
				var b = stream.ReadByte();
				if(b != -1)
				{
					SetByte(i,(byte)b);
				}
			}
		}

		public int GetSerializeSize(int nType=0, ProtocolVersion protocolVersion = ProtocolVersion.PROTOCOL_VERSION)
		{
			return WIDTH_BYTE;
		}
		public int Size
		{
			get
			{
				return WIDTH_BYTE;
			}
		}

		public ulong GetLow64()
		{
			return pn[0] | (ulong)pn[1] << 32;
		}
		public uint GetLow32()
		{
			return pn[0];
		}
		//public double GetDouble()
		//{
		//	double ret = 0.0;
		//	double fact = 1.0;
		//	for (int i = 0; i < WIDTH; i++) {
		//		ret += fact * pn[i];
		//		fact *= 4294967296.0;
		//	}
		//	return ret;
		//}
		public override int GetHashCode()
		{
			unchecked
			{
				if(pn == null)
				{
					return 0;
				}
				int hash = 17;
				foreach(var element in pn)
				{
					hash = hash * 31 + element.GetHashCode();
				}
				return hash;
			}
		}
	}
	public class base_uint160
	{
		protected const int WIDTH = 160 / 32;
		protected const int WIDTH_BYTE = 160 / 8;
		protected internal UInt32[] pn = new UInt32[WIDTH];

		public void SetHex(string str)
		{
			Array.Clear(pn, 0, pn.Length);
			str = str.TrimStart();

			int i = 0;
			if(str.Length >= 2)
				if(str[0] == '0' && char.ToLower(str[1]) == 'x')
					i += 2;

			int pBegin = i;
			while(i < str.Length && HexEncoder.IsDigit(str[i]) != -1)
				i++;

			i--;

			int p1 = 0;
			int pend = p1 + WIDTH * 4;
			while(i >= pBegin && p1 < pend)
			{
				SetByte(p1, (byte)HexEncoder.IsDigit(str[i]));
				i--;
				if(i >= pBegin)
				{
					byte n = (byte)HexEncoder.IsDigit(str[i]);
					n = (byte)(n << 4);
					SetByte(p1, (byte)(GetByte(p1) | n));
					i--;
					p1++;
				}
			}
		}
		protected void SetByte(int index, byte value)
		{
			var uintIndex = index / sizeof(uint);
			var byteIndex = index % sizeof(uint);

			var currentValue = pn[uintIndex];
			var mask = ((uint)0xFF << (byteIndex * 8));
			currentValue = currentValue & ~mask;
			var shiftedValue = (uint)value << (byteIndex * 8);
			currentValue |= shiftedValue;
			pn[uintIndex] = currentValue;
		}

		public string GetHex()
		{
			StringBuilder builder = new StringBuilder();
			for(int i = 0 ; i < WIDTH_BYTE; i++)
			{
				var b = GetByte(WIDTH_BYTE - i - 1);
				builder.Append(b.ToString("x2"));
			}
			return builder.ToString();
		}
		
		public byte GetByte(int index)
		{
			var uintIndex = index / sizeof(uint);
			var byteIndex = index % sizeof(uint);
			var value = pn[uintIndex];
			return (byte)(value >> (byteIndex * 8));
		}

		public override string ToString()
		{
			return GetHex();
		}

	}
	public class uint160 : base_uint160,  IBitcoinSerializable
	{

		public uint160()
		{
			for(int i = 0 ; i < WIDTH ; i++)
				pn[i] = 0;
		}

		public uint160(base_uint160 b)
		{
			for(int i = 0 ; i < WIDTH ; i++)
				pn[i] = b.pn[i];
		}

		//uint256 operator=(base_uint160& b)
		//{
		//	for (int i = 0; i < WIDTH; i++)
		//		pn[i] = b.pn[i];
		//	return  *this;
		//}


		public uint160(ulong b)
		{
			pn[0] = (uint)b;
			pn[1] = (uint)(b >> 32);
			for (int i = 2; i < WIDTH; i++)
				pn[i] = 0;
		}
		public uint160(byte[] vch, bool lendian = true)
		{
			if(!lendian)
				vch = vch.Reverse().ToArray();
			if(vch.Length == pn.Length * 4)
			{
				for(int i = 0, y = 0 ; i < pn.Length && y < vch.Length ; i++, y += 4)
				{
					pn[i] = BitConverter.ToUInt32(vch, y);
				}
			}
			else
				throw new FormatException("the byte array should be 160 byte long");
		}

		public uint160(string str)
		{
			SetHex(str);
		}

		public uint160(byte[] vch)
		{
			if(vch.Length == pn.Length * 4)
			{
				for(int i = 0, y = 0 ; i < pn.Length && y < vch.Length ; i++, y += 4)
				{
					pn[i] = BitConverter.ToUInt32(vch, y);
				}
			}
			else
				throw new FormatException("the byte array should be 160 byte long");
		}

		public override bool Equals(object obj)
		{
			var item = obj as uint160;
			if(item == null)
				return false;
			return AreEquals(pn, item.pn);
		}
		public static bool operator ==(uint160 a, uint160 b)
		{
			if(System.Object.ReferenceEquals(a, b))
				return true;
			if(((object)a == null) || ((object)b == null))
				return false;
			return AreEquals(a.pn, b.pn);
		}

		private static bool AreEquals(uint[] ar1, uint[] ar2)
		{
			if(ar1.Length != ar2.Length)
				return false;
			for(int i = 0 ; i < ar1.Length ; i++)
			{
				if(ar1[i] != ar2[i])
					return false;
			}
			return true;
		}

		public static bool operator <(uint160 a, uint160 b)
		{

			return Comparison(a, b) < 0;

		}

		public static bool operator >(uint160 a, uint160 b)
		{

			return Comparison(a, b) > 0;

		}

		public static bool operator <=(uint160 a, uint160 b)
		{

			return Comparison(a, b) <= 0;

		}

		public static bool operator >=(uint160 a, uint160 b)
		{

			return Comparison(a, b) >= 0;

		}

		private static int Comparison(uint160 a, uint160 b)
		{
			 for (int i = WIDTH-1; i >= 0; i--)
			{
				if (a.pn[i] < b.pn[i])
					return -1;
				else if (a.pn[i] > b.pn[i])
					return 1;
			}
			return 0;
		}

		public static bool operator !=(uint160 a, uint160 b)
		{
			return !(a == b);
		}

		public static bool operator ==(uint160 a, ulong b)
		{
			return (a == new uint160(b));
		}
		public static bool operator !=(uint160 a, ulong b)
		{
			return !(a == new uint160(b));
		}
		public static uint160 operator ^(uint160 a, uint160 b)
		{
			var c = new uint160();
			c.pn = new uint[a.pn.Length];
			for(int i = 0 ; i < c.pn.Length ; i++)
			{
				c.pn[i] = a.pn[i] ^ b.pn[i];
			}
			return c;
		}

		public static bool operator!(uint160 a)
	    {
	     for (int i = 0; i < WIDTH; i++)
	         if (a.pn[i] != 0)
	             return false;
	     return true;
	   }

	    public static uint160 operator-(uint160 a, uint160 b)
    {
		return a + (-b);
    }

	   public static uint160 operator+(uint160 a, uint160 b)
    {
		var result = new uint160();
        ulong carry = 0;
        for (int i = 0; i < WIDTH; i++)
        {
            ulong n = carry + a.pn[i] + b.pn[i];
            result.pn[i] = (uint)(n & 0xffffffff);
            carry = n >> 32;
        }
        return result;
    }

	public static uint160 operator+(uint160 a, ulong b)
    {
		return a + new uint160(b);
    }

	

	public static implicit operator uint160(ulong value)
	{
		return new uint160(value);
	}

		public static uint160 operator &(uint160 a, uint160 b)
		{
			var n = new uint160(a);
			for(int i = 0 ; i < WIDTH ; i++)
				n.pn[i] &= b.pn[i];
			return n;
		}
		public static uint160 operator |(uint160 a, uint160 b)
		{
			var n = new uint160(a);
			for(int i = 0 ; i < WIDTH ; i++)
				n.pn[i] |= b.pn[i];
			return n;
		}
		public static uint160 operator <<(uint160 a, int shift)
		{
			var result = new uint160();
			int k = shift / 32;
			shift = shift % 32;
			for(int i = 0 ; i < WIDTH ; i++)
			{
				if(i + k + 1 < WIDTH && shift != 0)
					result.pn[i + k + 1] |= (a.pn[i] >> (32 - shift));
				if(i + k < WIDTH)
					result.pn[i + k] |= (a.pn[i] << shift);
			}
			return result;
		}

		public static uint160 operator >>(uint160 a, int shift)
		{
			var result = new uint160();
			int k = shift / 32;
			shift = shift % 32;
			for(int i = 0 ; i < WIDTH ; i++)
			{
				if(i - k - 1 >= 0 && shift != 0)
					result.pn[i - k - 1] |= (a.pn[i] << (32 - shift));
				if(i - k >= 0)
					result.pn[i - k] |= (a.pn[i] >> shift);
			}
			return result;
		}

		
		public static uint160 operator ~(uint160 a)
		{
			var b = new uint160();
			for(int i = 0 ; i < b.pn.Length ; i++)
			{
				b.pn[i] = ~a.pn[i];
			}
			return b;
		}
		public static uint160 operator -(uint160 a)
		{
			var b = new uint160();
			for(int i = 0 ; i < b.pn.Length ; i++)
			{
				b.pn[i] = ~a.pn[i];
			}
			b++;
			return b;
		}

		 public static uint160 operator ++(uint160 a)
		{
			var ret = new uint160(a);
			return a + new uint160(1);
		}
		public static uint160 operator --(uint160 a)
		{
			return a - 1;
		}
		
		public byte[] ToBytes(bool lendian = true)
{
	var copy = new byte[WIDTH_BYTE];
	for(int i = 0 ; i < WIDTH_BYTE ; i++)
	{
		copy[i] = GetByte(i);
	}
	if(!lendian)
		Array.Reverse(copy);
	return copy;
}

		public void ReadWrite(BitcoinStream stream)
		{
			if(stream.Serializing)
			{
				var b = ToBytes();
				stream.ReadWrite(ref b);
			}
			else
			{
				byte[] b = new byte[WIDTH_BYTE];
				stream.ReadWrite(ref b);
				this.pn = new uint160(b).pn;
			}
		}

		public void Serialize(Stream stream, int nType = 0, ProtocolVersion protocolVersion = ProtocolVersion.PROTOCOL_VERSION)
		{
			for(int i = 0 ; i < WIDTH_BYTE ; i++)
			{
				stream.WriteByte(GetByte(i));
			}
		}

		public void Unserialize(Stream stream, int nType = 0, ProtocolVersion protocolVersion = ProtocolVersion.PROTOCOL_VERSION)
		{
			for(int i = 0 ; i < WIDTH_BYTE ; i++)
			{
				var b = stream.ReadByte();
				if(b != -1)
				{
					SetByte(i,(byte)b);
				}
			}
		}

		public int GetSerializeSize(int nType=0, ProtocolVersion protocolVersion = ProtocolVersion.PROTOCOL_VERSION)
		{
			return WIDTH_BYTE;
		}
		public int Size
		{
			get
			{
				return WIDTH_BYTE;
			}
		}

		public ulong GetLow64()
		{
			return pn[0] | (ulong)pn[1] << 32;
		}
		public uint GetLow32()
		{
			return pn[0];
		}
		//public double GetDouble()
		//{
		//	double ret = 0.0;
		//	double fact = 1.0;
		//	for (int i = 0; i < WIDTH; i++) {
		//		ret += fact * pn[i];
		//		fact *= 4294967296.0;
		//	}
		//	return ret;
		//}
		public override int GetHashCode()
		{
			unchecked
			{
				if(pn == null)
				{
					return 0;
				}
				int hash = 17;
				foreach(var element in pn)
				{
					hash = hash * 31 + element.GetHashCode();
				}
				return hash;
			}
		}
	}
}