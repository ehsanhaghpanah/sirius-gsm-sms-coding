/**
 * Copyright (C) Ehsan Haghpanah, 2010.
 * All rights reserved.
 * Ehsan Haghpanah, (github.com/ehsanhaghpanah)
 */

using System;

namespace sirius.GSM.Coding
{
	public sealed class BinaryOp
	{
		private static readonly string[] _BinaryString = { 
               "0000", "0001", "0010", "0011", 
               "0100", "0101", "0110", "0111", 
               "1000", "1001", "1010", "1011",
               "1100", "1101", "1110", "1111"
          };

		/// <summary>
		/// 
		/// </summary>
		public static string ToHexString(byte[] data)
		{
			string r = string.Empty;

			for (int i = 0; i < data.Length; i++)
				r += data[i].ToString("x2");

			return (r);
		}

		/// <summary>
		/// 
		/// </summary>
		public static string ToBinaryString(byte data)
		{
			int h = int.Parse(data.ToString("x2").Substring(0, 1), System.Globalization.NumberStyles.HexNumber);
			int l = int.Parse(data.ToString("x2").Substring(1, 1), System.Globalization.NumberStyles.HexNumber);

			return (_BinaryString[h] + _BinaryString[l]);
		}

		/// <summary>
		/// 
		/// </summary>
		public static byte BinaryToByte(string data)
		{
			if (data.Length != 8)
				throw new ArgumentException();

			int a = Array.BinarySearch(_BinaryString, data.Substring(0, 4));
			int b = Array.BinarySearch(_BinaryString, data.Substring(4, 4));

			if ((!((0 <= a) && (a <= 15))) && (!((0 <= b) && (b <= 15))))
				throw new ArgumentException();

			return ((byte)(a * 16 + b));
		}

		/// <summary>
		/// 
		/// </summary>
		public static byte[] Convert(byte[] data, DataCoding sourceCoding, DataCoding targetCoding)
		{
			if ((sourceCoding == DataCoding.Unicode) || (targetCoding == DataCoding.Unicode))
				throw new ArgumentException();

			string t = string.Empty;

			for (int i = 0; i < data.Length; i++)
				t = ToBinaryString(data[i]).Substring((int)sourceCoding) + t;

			int q = 8 - (int)targetCoding;

			if ((t.Length % q) != 0)
				t = t.PadLeft(t.Length + (q - (t.Length % q)), '0');

			byte[] r = new byte[(t.Length / q)];
			for (int i = 0; i < r.Length; i++)
				r[i] = BinaryToByte(t.Substring(t.Length - q - (q * i), q).PadLeft(8, '0'));

			return (r);
		}

		/// <summary>
		/// 
		/// </summary>
		public static byte[] Encode(byte[] data, DataCoding coding)
		{
			if (coding == DataCoding.Unicode)
				throw new ArgumentException();

			string t = string.Empty;

			for (int i = 0; i < data.Length; i++)
				t = ToBinaryString(data[i]).Substring((int)coding) + t;

			if ((t.Length % 8) != 0)
				t = t.PadLeft(t.Length + (8 - (t.Length % 8)), '0');

			byte[] r = new byte[(t.Length / 8)];
			for (int i = 0; i < r.Length; i++)
				r[i] = BinaryToByte(t.Substring(t.Length - 8 - (8 * i), 8));

			return (r);
		}

		/// <summary>
		/// 
		/// </summary>
		public static byte[] Decode(byte[] data, DataCoding coding)
		{
			if (coding == DataCoding.Unicode)
				throw new ArgumentException();

			string t = string.Empty;

			for (int i = 0; i < data.Length; i++)
				t = ToBinaryString(data[i]) + t;

			int m = 8 - (int)coding;
			byte[] r = new byte[(t.Length - (t.Length % m)) / m];
			for (int i = 0; i < r.Length; i++)
				r[i] = BinaryToByte(t.Substring(t.Length - m - (m * i), m).PadLeft(8, '0'));

			if (r[r.Length - 1] == 0)
			{
				byte[] s = new byte[r.Length - 1];
				for (int i = 0; i < s.Length; i++)
					s[i] = r[i];

				return (s);
			}

			return (r);
		}
	}
}