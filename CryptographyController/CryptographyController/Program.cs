using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace CryptographController
{
	#region Data Types
	public class Key
	{
		public string KEY;
		public string IV;			// Initialization Vector
	}

	public class Ciphertext
	{
		public string CIPHERTEXTSTRING;
	}
	#endregion

	class MainProgram
	{
		#region Application Info
		public static string applicationName = " Cryptography Controller     ";
		public static string applicationVersion = "2.1";
		#endregion

		#region Encryption & Decryption
		static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
		{
			// Check arguments. 
			if (cipherText == null || cipherText.Length <= 0)
				throw new ArgumentNullException("cipherText");
			if (Key == null || Key.Length <= 0)
				throw new ArgumentNullException("Key");
			if (IV == null || IV.Length <= 0)
				throw new ArgumentNullException("Key");

			// Declare the string used to hold 
			// the decrypted text. 
			string plaintext = null;

			// Create an AesManaged object 
			// with the specified key and IV. 
			using (AesManaged aesAlg = new AesManaged())
			{
				aesAlg.Key = Key;
				aesAlg.IV = IV;

				// Create a decrytor to perform the stream transform.
				ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

				// Create the streams used for decryption. 
				using (MemoryStream msDecrypt = new MemoryStream(cipherText))
				{
					using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
					{
						using (StreamReader srDecrypt = new StreamReader(csDecrypt))
						{
							// Read the decrypted bytes from the decrypting stream 
							// and place them in a string.
							plaintext = srDecrypt.ReadToEnd();
						}
					}
				}

			}

			return plaintext;
		}
		static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
		{
			// Check arguments. 
			if (plainText == null || plainText.Length <= 0)
				throw new ArgumentNullException("plainText");
			if (Key == null || Key.Length <= 0)
				throw new ArgumentNullException("Key");
			if (IV == null || IV.Length <= 0)
				throw new ArgumentNullException("Key");
			byte[] encrypted;
			// Create an AesManaged object 
			// with the specified key and IV. 
			using (AesManaged aesAlg = new AesManaged())
			{
				aesAlg.Key = Key;
				aesAlg.IV = IV;

				// Create a decrytor to perform the stream transform.
				ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

				// Create the streams used for encryption. 
				using (MemoryStream msEncrypt = new MemoryStream())
				{
					using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
						{

							//Write all data to the stream.
							swEncrypt.Write(plainText);
						}
						encrypted = msEncrypt.ToArray();
					}
				}
			}


			// Return the encrypted bytes from the memory stream. 
			return encrypted;

		}
		#endregion

		#region Initialization
		static void Main(string[] args)
		{
			Console.Title = applicationName;
			OperationMode();
		}
		#endregion

		#region Main Functions
		// A function for generating the "Header" of the console. (Title, Version, Decorations)
		public static void DisplayHeader(string _error = null, bool _admin = false)
		{
			Console.Clear();			// Clear the console
			string header = "";			// Define an empty string
			// Get the length of the header text and add a "-" for every charachter (Decoration)
			for (int i = 0; i <= (applicationName + "   " + applicationVersion).Length; i++)
			{
				header += "-";
			}
			Console.WriteLine(header);			// Writes the decoration to the console
			Console.ForegroundColor = ConsoleColor.Red;			// Changet the writing color to "Red"
			Console.Write(applicationName);			// Write the application Name
			Console.ForegroundColor = ConsoleColor.Cyan;			// Change the color to "Cyan"
			Console.WriteLine(" " + applicationVersion);			// Write the application version
			Console.ResetColor();			// Reset the color to default
			Console.WriteLine(header + "\n");			// Write the decoration and add an additional new line
			if (_error != null)			// If we got an error write it to the console
			{
				Console.WriteLine(_error);
			}
			// If the user activates Admin Mode display this
			if (_admin)
			{
				Console.WriteLine("Welcome Adminstrator!\n");
			}
		}

		//Asks the use what mode they want
		public static void OperationMode(bool retry = false)
		{
			if (retry)
			{
				DisplayHeader("Invalid Operation!");
			}
			else
			{
				DisplayHeader();
			}
			Console.WriteLine("Use (Exit) to exit.");
			Console.Write("Operation Mode (ENCRYPT / DECRYPT / KEYGEN): ");
			string mode = Console.ReadLine().ToUpper();
			if (mode == "EXIT")
			{
				System.Environment.Exit(0);
			}
			if (mode != "ENCRYPT" && mode != "DECRYPT" && mode != "@" && mode != "KEYGEN")
			{
				OperationMode(true);
			}
			if (mode == "ENCRYPT")
			{
				EncryptMode();
			}
			else if (mode == "DECRYPT")
			{
				DecryptMode();
			}
			else if (mode == "@")
			{
				DisplayHeader(_admin: true);
				Console.Write("Select an Admin Action! (KEYGEN): ");
				if (Console.ReadLine().ToUpper() == "KEYGEN")
				{
					KeyGenMode();
				}
				else
				{
					OperationMode(true);
				}
			}
			else if (mode == "KEYGEN")
			{
				KeyGenMode();
			}
		}
		#endregion

		#region Operational Modes
		private static void KeyGenMode(bool _retry = false)
		{
			if (_retry)
			{
				DisplayHeader("Invalid Input!");
			}
			else
			{
				DisplayHeader();
			}
			try
			{
				using (AesManaged myAes = new AesManaged())
				{
					Console.Write("Where would you like to output to? (FILE / CONSOLE): ");
					string input = Console.ReadLine().ToUpper();

					Key cipher = new Key();
					cipher.KEY = Convert.ToBase64String(myAes.Key);
					cipher.IV = Convert.ToBase64String(myAes.IV);
					if (input == "FILE")
					{
						Console.Write(@"Please enter a path (Ex. C:\MainKey.AES): ");
						string path = Console.ReadLine();
						if (!path.Contains(@":\"))
						{
							KeyGenMode(true);
						}
						System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(Key));
						System.IO.StreamWriter file = new System.IO.StreamWriter(path);
						writer.Serialize(file, cipher);
						file.Close();
						Console.WriteLine("Key successfully written to file at (" + path + ")");
					}
					else if (input == "CONSOLE")
					{
						Console.WriteLine("Key:       {0}", cipher.KEY);
						Console.WriteLine("IV:        {0}", cipher.IV);
					}
					else
					{
						KeyGenMode(true);
					}
					// myAes.Key, myAes.IV
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Error: {0}", e.Message);
			}
			Console.Write("Press any key to continue...");
			Console.ReadKey();
			OperationMode();
			
		}
		
		public static void EncryptMode(bool _retry = false)
		{
			if (_retry)
			{
				DisplayHeader("Invalid Input!");
			}
			else
			{
				DisplayHeader();
			}

			try
			{
				string input;
				string original = null;
				// TODO: Allow text to be inputed from a file
				Key cipher = new Key();
				Console.Write("How would you like to input the text to Encrypt? (FILE / CONSOLE): ");
				input = Console.ReadLine().ToUpper();
				if (input == "FILE")
				{
					Console.Write(@"Please enter the path to your text. (Ex. C:\TextDocument.txt): ");
					original = System.IO.File.ReadAllText(Console.ReadLine());
				}
				else if (input == "CONSOLE")
				{
					Console.Write("Text to Encrypt: ");
					original = Console.ReadLine();
				}
				else
				{
					EncryptMode(true);
				}
				
				byte[] key = null;
				byte[] iv = null;

				Console.Write("How would you like to input a key? (FILE / TEXT): ");
				input = Console.ReadLine().ToUpper();

				if (input == "TEXT")
				{
					Console.Write("Key: ");
					key = Convert.FromBase64String(Console.ReadLine());
					Console.Write("IV: ");
					iv = Convert.FromBase64String(Console.ReadLine());
				}
				else if (input == "FILE")
				{
					Console.Write(@"Please enter the path to your key. (Ex. C:\MainKey.AES): ");
					string path = Console.ReadLine();
					System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Key));
					System.IO.StreamReader file = new System.IO.StreamReader(path);
					cipher = (Key)reader.Deserialize(file);
					key = Convert.FromBase64String(cipher.KEY);
					iv = Convert.FromBase64String(cipher.IV);
				}
				else
				{
					EncryptMode(true);
				}

				// Create a new instance of the AesManaged 
				// class.  This can generate a new key and  
				// initialization vector (IV).
				using (AesManaged myAes = new AesManaged())
				{
					byte[] encrypted;
					string example;
					
					// Encrypt the string to an array of bytes. 
					encrypted = EncryptStringToBytes_Aes(original, key, iv);
					example = Convert.ToBase64String(encrypted);

					//Display the original data and the decrypted data.
					DisplayHeader();
					Console.Write("Where would you like to output to? (CONSOLE/FILE): ");
					string line = Console.ReadLine().ToUpper();
					if (line == "CONSOLE")
					{
						DisplayHeader();
						Console.WriteLine("Cyphertext: {0}", example);
					}
					else if (line == "FILE")
					{
						DisplayHeader();
						Ciphertext cyphertext = new Ciphertext();
						cyphertext.CIPHERTEXTSTRING = example;
						Console.Write(@"Please enter a path (Ex. C:\EncrpytedText.AESTXT): ");
						string path = Console.ReadLine();
						System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(Ciphertext));
						System.IO.StreamWriter file = new System.IO.StreamWriter(path);
						writer.Serialize(file, cyphertext);
						file.Close();
						Console.WriteLine("File successfully written to file at (" + path + ")");
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Error: {0}", e.Message);
			}
			Console.Write("Press any key to continue...");
			Console.ReadKey();
			OperationMode();
		}

		public static void DecryptMode(bool _retry = false)
		{
			if (_retry)
			{
				DisplayHeader("Invalid Input! Try again!");
			}
			else { 
				DisplayHeader();
			}
			try
			{
				Key cipher = new Key();
				Ciphertext ctext = new Ciphertext();

				byte[] ciphertext = null;
				byte[] key = null;
				byte[] iv = null;

				string input;
				Console.Write("How would you like to input ciphertext? (FILE / TEXT): ");
				input = Console.ReadLine().ToUpper();

				if (input == "TEXT")
				{
					Console.Write("Ciphertext: ");
					ciphertext = Convert.FromBase64String(Console.ReadLine());
				}
				else if (input == "FILE")
				{
					Console.WriteLine(@"Please enter the path to your ciphertext. (Ex. C:\EncrpytedText.AESTXT):");
					string path = Console.ReadLine();
					System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Ciphertext));
					System.IO.StreamReader file = new System.IO.StreamReader(path);
					ctext = (Ciphertext)reader.Deserialize(file);
					ciphertext = Convert.FromBase64String(ctext.CIPHERTEXTSTRING);
				}
				else
				{
					DecryptMode(true);
				}
				

				Console.Write("How would you like to input a key? (FILE / TEXT): ");
				input = Console.ReadLine().ToUpper();

				if (input == "TEXT")
				{
					Console.Write("Key: ");
					key = Convert.FromBase64String(Console.ReadLine());
					Console.Write("IV: ");
					iv = Convert.FromBase64String(Console.ReadLine());
				}
				else if (input == "FILE")
				{
					Console.Write(@"Please enter the path to your key. (Ex. C:\MainKey.AES): ");
					string path = Console.ReadLine();
					System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Key));
					System.IO.StreamReader file = new System.IO.StreamReader(path);
					cipher = (Key)reader.Deserialize(file);
					key = Convert.FromBase64String(cipher.KEY);
					iv = Convert.FromBase64String(cipher.IV);
				}
				else
				{
					DecryptMode(true);
				}

				using (AesManaged myAes = new AesManaged())
				{
					// TODO: Allow the text to be exported to a file.
					// Decrypt the bytes to a string. 
					string roundtrip = DecryptStringFromBytes_Aes(ciphertext, key, iv);

					DisplayHeader();
					Console.Write("How would you like to output your text? (FILE / TEXT): ");
					input = Console.ReadLine().ToUpper();

					if (input == "TEXT")
					{
						Console.WriteLine("{0}", roundtrip);
					}
					else if (input == "FILE")
					{
						Console.WriteLine(@"Please enter the path to save your text. (Ex. C:\Document.txt):");
						string path = Console.ReadLine();
						System.IO.File.WriteAllText(path, roundtrip);
					}
					else
					{
						DecryptMode(true);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Error: {0}", e.Message);
			}
			Console.Write("Press any key to continue...");
			Console.ReadKey();
			OperationMode();
		}
		#endregion
	}
}