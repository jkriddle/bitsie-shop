﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBitcoin.DataEncoders
{
	public class DataEncoder
	{
		public static readonly char[] SpaceCharacters = new[] { ' ', '\t', '\n', '\v', '\f', '\r' };
		public static bool IsSpace(char c)
		{
			return SpaceCharacters.Contains(c);
		}

		

		public string EncodeData(byte[] data)
		{
			return EncodeData(data, data.Length);
		}
		public virtual string EncodeData(byte[] data, int length)
		{
			throw new NotSupportedException();
		}

		public virtual byte[] DecodeData(string encoded)
		{
			throw new NotSupportedException();
		}
	}
	public class Encoders
	{
		public static ASCIIEncoder ASCII
		{
			get
			{
				return new ASCIIEncoder();
			}
		}
		public static DataEncoder Hex
		{
			get
			{
				return new HexEncoder();
			}
		}

		public static DataEncoder Base58
		{
			get
			{
				return new Base58Encoder();
			}
		}
		public static DataEncoder Base58Check
		{
			get
			{
				return new Base58Encoder()
				{
					Check = true
				};
			}
		}
		public static DataEncoder Base64
		{
			get
			{
				return new Base64Encoder();
			}
		}

		//public static DataEncoder Bin
		//{
		//	get
		//	{
		//		return null;
		//	}
		//}
		//public static DataEncoder Dec
		//{
		//	get
		//	{
		//		return null;
		//	}
		//}
		//public static DataEncoder RFC1751
		//{
		//	get
		//	{
		//		return null;
		//	}
		//}
		//public static DataEncoder Poetry
		//{
		//	get
		//	{
		//		return null;
		//	}
		//}
		//public static DataEncoder Rot13
		//{
		//	get
		//	{
		//		return null;
		//	}
		//}
		//public static DataEncoder Easy16
		//{
		//	get
		//	{
		//		return null;
		//	}
		//}
	}
}
