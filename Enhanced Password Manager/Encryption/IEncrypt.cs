namespace Enhanced_Password_Manager.Encryption
{
	/// <summary>
	/// Interface for a type of encryption algorithm.
	/// </summary>
	public interface IEncrypt
	{
		/// <summary>
		/// Encrypts the given data using the given seed.
		/// </summary>
		/// <param name="data">Data to operate on</param>
		/// <param name="seed">Used in encryption algorithm</param>
		/// <returns>Modified data</returns>
		byte[] Encrypt(byte[] data, string seed);

		/// <summary>
		/// Decrypts the given data using the given seed.
		/// </summary>
		/// <param name="data">Data to operate on</param>
		/// <param name="seed">Used in encryption algorithm</param>
		/// <returns>Modified data</returns>
		byte[] Decrypt(byte[] data, string seed);
	}
}
