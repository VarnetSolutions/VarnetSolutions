using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace VarnetFileService.BO
{
    public class FileEncryption
    {
        private static readonly byte[] Key = HexStringToByteArray("df813cb58707e91a930903874f5e5e6f94c6c02ef4c824ce00d244c58deb638b");
        private static readonly byte[] IV = HexStringToByteArray("f8eef6eb0016b58b241d4272599aba88");

        public static string EncryptFile(string inputFile, string outputFile)
        {
            outputFile = outputFile.Replace(Path.GetExtension(outputFile),$"encr_{Path.GetExtension(outputFile)}" );

            using (FileStream fsInput = new FileStream(inputFile, FileMode.Open))
            {
                using (FileStream fsEncrypted = new FileStream(outputFile, FileMode.Create))
                {
                    using (Aes aesAlg = Aes.Create())
                    {
                        aesAlg.Key = Key;
                        aesAlg.IV = IV;

                        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                        using (CryptoStream cryptoStream = new CryptoStream(fsEncrypted, encryptor, CryptoStreamMode.Write))
                        {
                            fsInput.CopyTo(cryptoStream);
                        }
                    }
                }
            }


            File.Delete(inputFile);

            return outputFile;
        }

        public static void DecryptFile(string inputFile, string outputFile)
        {
            using (FileStream fsEncrypted = new FileStream(inputFile, FileMode.Open))
            {
                using (FileStream fsDecrypted = new FileStream(outputFile, FileMode.Create))
                {
                    using (Aes aesAlg = Aes.Create())
                    {
                        aesAlg.Key = Key;
                        aesAlg.IV = IV;

                        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                        using (CryptoStream cryptoStream = new CryptoStream(fsEncrypted, decryptor, CryptoStreamMode.Read))
                        {
                            cryptoStream.CopyTo(fsDecrypted);
                        }
                    }
                }
            }
        }

        public static byte[] DecryptFileAndGetBytes(string inputFile)            
        {
            using (FileStream fsEncrypted = new FileStream(inputFile, FileMode.Open))
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Key;
                    aesAlg.IV = IV;

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msDecrypted = new MemoryStream())
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(fsEncrypted, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            csDecrypt.CopyTo(msDecrypted);
                        }
                        return msDecrypted.ToArray();
                    }
                }
                
            }           
        }

        private static byte[] HexStringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }
    }
}