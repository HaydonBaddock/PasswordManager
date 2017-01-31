using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Enhanced_Password_Manager.Encryption;
using Enhanced_Password_Manager.Model;

namespace Cracking
{
	class Program
	{
		private static string _characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 _";

		static void Main(string[] args)
		{
			IEncrypt encrypter = new OneTimePad();
			var encryptedBytes = File.ReadAllBytes("Passwords.cpt");
			var password = new IncrementalString(_characters.ToCharArray(), 8);
			var binaryFormatter = new BinaryFormatter();
			var memoryStream = new MemoryStream();
			while (true)
			{
				var decryptedBytes = encrypter.Decrypt(encryptedBytes, password.Next());
				memoryStream.Write(decryptedBytes, 0, decryptedBytes.Length);
				memoryStream.Position = 0;
				try
				{
					var deserialised = (SortedObservableCollection<Entry>) binaryFormatter.Deserialize(memoryStream);
					Console.WriteLine(@"Right: " + password);
				}
				catch (Exception ex) when (ex is DecoderFallbackException || ex is SerializationException || ex is NullReferenceException)
				{
					Console.WriteLine(@"Wrong: " + password);
				}
			}
		}
	}
}
