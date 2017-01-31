using System;
using System.Numerics;

namespace Enhanced_Password_Manager.Encryption {

	/// <summary>
	/// Encrypts and Decrypts using a one time pad constructed from a given seed.
	/// </summary>
	public class OneTimePad : IEncrypt {

		public byte[] Encrypt(byte[] data, string seed) {
			return Run(data, seed);
		}

		public byte[] Decrypt(byte[] data, string seed) {
			return Run(data, seed);
		}

		/// <summary>
		/// Creates a one-time-pad then combines it with the given data.
		/// </summary>
		/// <param name="data">Data to operate on</param>
		/// <param name="seed">String to create the pad from</param>
		/// <returns>Modified data</returns>
		private byte[] Run(byte[] data, string seed) {
			var key = CreateOneTimePad(data, seed);
			return XorByteArrays(data, key);
		}

		/// <summary>
		/// Creates a byte array at least as long as the given one from a string.
		/// </summary>
		/// <param name="bytes">A byte array for which the one time pad must be at least of equal length</param>
		/// <param name="str">The string from which to generate the byte array</param>
		/// <returns>The generated one time pad</returns>
		private byte[] CreateOneTimePad(byte[] bytes, string str) {

			// Convert str to a byte array
			var strBytes = new byte[str.Length * sizeof(char)];
			Buffer.BlockCopy(str.ToCharArray(), 0, strBytes, 0, strBytes.Length);

			// Extend the size of the password until it is of equal or greater length than the file
			// chur is created so that fileValue does not come out negative
			// Should really be doing this for passwordValue as well...
			var chur = new byte[bytes.Length + 1];
			bytes.CopyTo(chur, 0);
			chur[chur.Length - 1] = Convert.ToByte("0");
			var fileValue = new BigInteger(chur);
			var passwordValue = new BigInteger(strBytes);
			while (passwordValue < fileValue)
				passwordValue *= passwordValue;

			// Return password as a now very large array of bytes
			return passwordValue.ToByteArray();
		}

		/// <summary>
		/// Does a bitwise XOR on two given byte arrays.
		/// </summary>
		/// <param name="filesBytes">Byte array one</param>
		/// <param name="key">Byte array two</param>
		/// <returns>The result of the XOR operation on the byte arrays</returns>
		private byte[] XorByteArrays(byte[] filesBytes, byte[] key) {
			var combinedBytes = new byte[filesBytes.Length];
			for (var i = 0; i < filesBytes.Length; i++)
				combinedBytes[i] = (byte)(filesBytes[i] ^ key[i]);
			return combinedBytes;
		}
	}
}
