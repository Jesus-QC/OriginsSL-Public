using System.Security.Cryptography;
using System.Text;

namespace OriginsBot.Security;

public static class AesEncryptor
{
    private const string DefaultPassword = "E4edBxg3nLI78RRA";
    
    public static string Encrypt(string content, string password = DefaultPassword)
    {
        byte[] encryptedBytes;
        byte[] iv;
        
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(password);
            iv = aesAlg.IV;
            
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            
            using (MemoryStream msEncrypt = new())
            {
                using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new(csEncrypt))
                    {
                        swEncrypt.Write(content);
                    }

                    encryptedBytes = msEncrypt.ToArray();
                }
            }
        }
        
        byte[] combinedBytes = new byte[iv.Length + encryptedBytes.Length];
        Array.Copy(iv, 0, combinedBytes, 0, iv.Length);
        Array.Copy(encryptedBytes, 0, combinedBytes, iv.Length, encryptedBytes.Length);

        return Convert.ToBase64String(combinedBytes);
    }
    
    public static string Decrypt(string content, string password = DefaultPassword)
    {
        // Convert the input content from base64 string to byte array.
        byte[] combinedBytes = Convert.FromBase64String(content);

        // Extract the IV (Initialization Vector) from the combined byte array.
        byte[] iv = new byte[16]; // AES block size is 128 bits (16 bytes)
        Array.Copy(combinedBytes, iv, iv.Length);

        // Extract the encrypted data from the combined byte array.
        byte[] encryptedBytes = new byte[combinedBytes.Length - iv.Length];
        Array.Copy(combinedBytes, iv.Length, encryptedBytes, 0, encryptedBytes.Length);

        string decryptedContent;
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(password);
            aesAlg.IV = iv;

            // Create a decryptor to perform the stream transform.
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new(encryptedBytes))
            {
                using (CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new(csDecrypt))
                    {
                        // Read the decrypted bytes from the decrypting stream and convert them to a string.
                        decryptedContent = srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        return decryptedContent;
    }
}