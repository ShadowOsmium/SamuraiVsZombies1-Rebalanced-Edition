using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Debug = UnityEngine.Debug;

public class FileEncryptAndDecrypt
{
	public class CryptoHelpException : ApplicationException
	{
		public CryptoHelpException(string msg)
			: base(msg)
		{
		}
	}

	private static string password = "_Glu#Games$2012_";

	private static RandomNumberGenerator rand = new RNGCryptoServiceProvider();

	private static byte[] GenerateRandomBytes(int count)
	{
		byte[] array = new byte[count];
		rand.GetBytes(array);
		return array;
	}

	private static SymmetricAlgorithm CreateRijndael(string password, byte[] salt)
	{
		PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(password, salt, "SHA256", 1000);
		SymmetricAlgorithm symmetricAlgorithm = Rijndael.Create();
		symmetricAlgorithm.KeySize = 256;
		symmetricAlgorithm.Key = passwordDeriveBytes.GetBytes(32);
		symmetricAlgorithm.Padding = PaddingMode.PKCS7;
		return symmetricAlgorithm;
	}

	private static bool CheckByteArrays(byte[] b1, byte[] b2)
	{
		if (b1.Length == b2.Length)
		{
			for (int i = 0; i < b1.Length; i++)
			{
				if (b1[i] != b2[i])
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	public static void EncryptFile(string _inFile)
	{
		string text = _inFile + ".bak";
		using (FileStream fileStream = File.OpenRead(_inFile))
		{
			using (FileStream fileStream2 = File.OpenWrite(text))
			{
				long length = fileStream.Length;
				byte[] array = new byte[length];
				int num = -1;
				int num2 = 0;
				byte[] array2 = GenerateRandomBytes(16);
				byte[] array3 = GenerateRandomBytes(16);
				SymmetricAlgorithm symmetricAlgorithm = CreateRijndael(password, array3);
				symmetricAlgorithm.IV = array2;
				fileStream2.Write(array2, 0, array2.Length);
				fileStream2.Write(array3, 0, array3.Length);
				HashAlgorithm hashAlgorithm = SHA256.Create();
				using (CryptoStream cryptoStream = new CryptoStream(fileStream2, symmetricAlgorithm.CreateEncryptor(), CryptoStreamMode.Write))
				{
					using (CryptoStream cryptoStream2 = new CryptoStream(Stream.Null, hashAlgorithm, CryptoStreamMode.Write))
					{
						BinaryWriter binaryWriter = new BinaryWriter(cryptoStream);
						binaryWriter.Write(length);
						while ((num = fileStream.Read(array, 0, array.Length)) != 0)
						{
							cryptoStream.Write(array, 0, num);
							cryptoStream2.Write(array, 0, num);
							num2 += num;
						}
						cryptoStream2.Flush();
						cryptoStream2.Close();
						byte[] hash = hashAlgorithm.Hash;
						cryptoStream.Write(hash, 0, hash.Length);
						cryptoStream.Flush();
						cryptoStream.Close();
					}
				}
				fileStream.Close();
				fileStream2.Close();
			}
		}
		File.Copy(text, _inFile, true);
		if (!File.Exists(Profile.prevSaveFilePath))
		{
			File.Copy(text, Profile.prevSaveFilePath, true);
		}
		File.Delete(text);
	}

	public static string DecryptFile(string inFile, bool prevCorrupted)
	{
		using (FileStream fileStream = File.OpenRead(inFile))
		{
			StringBuilder stringBuilder = new StringBuilder();
			Encoding aSCII = Encoding.ASCII;
			int num = -1;
			int num2 = 0;
			int num3 = 0;
			byte[] array = new byte[16];
			fileStream.Read(array, 0, 16);
			byte[] array2 = new byte[16];
			fileStream.Read(array2, 0, 16);
			SymmetricAlgorithm symmetricAlgorithm = CreateRijndael(password, array2);
			symmetricAlgorithm.IV = array;
			num2 = 32;
			long num4 = -1L;
			HashAlgorithm hashAlgorithm = SHA256.Create();
			using (CryptoStream cryptoStream = new CryptoStream(fileStream, symmetricAlgorithm.CreateDecryptor(), CryptoStreamMode.Read))
			{
				using (CryptoStream cryptoStream2 = new CryptoStream(Stream.Null, hashAlgorithm, CryptoStreamMode.Write))
				{
					BinaryReader binaryReader = new BinaryReader(cryptoStream);
					num4 = binaryReader.ReadInt64();
					if (Debug.isDebugBuild)
					{
						Debug.Log("***lSize = " + num4);
					}
					if (num4 > 0)
					{
						byte[] array3 = new byte[num4];
						num = cryptoStream.Read(array3, 0, array3.Length);
						stringBuilder.Append(aSCII.GetString(array3));
						cryptoStream2.Write(array3, 0, num);
						num2 += num;
						num3 += num;
					}
					cryptoStream2.Flush();
					cryptoStream2.Close();
					byte[] hash = hashAlgorithm.Hash;
					byte[] array4 = new byte[hashAlgorithm.HashSize / 8];
					num = cryptoStream.Read(array4, 0, array4.Length);
					if (array4.Length != num || !CheckByteArrays(array4, hash))
					{
						if (prevCorrupted || !File.Exists(Profile.prevSaveFilePath))
						{
							Singleton<Analytics>.instance.LogEvent("SAVE_DATA_CORRUPTION", "<Resetting save data>");
							fileStream.Close();
							Singleton<Profile>.instance.ResetData();
							stringBuilder = new StringBuilder(DecryptFile(inFile, false));
							return stringBuilder.ToString();
						}
						Singleton<Analytics>.instance.LogEvent("SAVE_DATA_CORRUPTION", "Restore save data <START>");
						if (Debug.isDebugBuild)
						{
							Debug.Log("***  Corrupted Data ***" + stringBuilder.ToString());
						}
						fileStream.Close();
						File.Copy(Profile.prevSaveFilePath, inFile, true);
						Singleton<Analytics>.instance.LogEvent("SAVE_DATA_CORRUPTION", "Restore sava data <SUCCESS>");
						stringBuilder = new StringBuilder(DecryptFile(inFile, true));
						return stringBuilder.ToString();
					}
				}
			}
			if (num3 != num4)
			{
				Singleton<Analytics>.instance.LogEvent("SAVE_DATA_CORRUPTION", "Input File size mis-match!");
				throw new CryptoHelpException("File Sizes don't match!");
			}
			return stringBuilder.ToString();
		}
	}
}
