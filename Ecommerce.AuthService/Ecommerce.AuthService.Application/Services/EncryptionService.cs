using System;
using System.Security.Cryptography;
using System.Text;
using Ecommerce.AuthService.Application.Interfaces.IService;
using Microsoft.Extensions.Configuration;

namespace Ecommerce.AuthService.Application.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _encryptionKey;
        private readonly byte[] _encryptionIv;

        public EncryptionService(IConfiguration configuration)
        {
            var encryptionSettings = configuration.GetSection("EncryptionSettings");

            var keyString = encryptionSettings["Key"];
            var ivString = encryptionSettings["IV"];

            if (string.IsNullOrEmpty(keyString))
                throw new ArgumentException("Encryption key is required in configuration.", nameof(keyString));

            if (string.IsNullOrEmpty(ivString))
                throw new ArgumentException("Encryption IV is required in configuration.", nameof(ivString));

            // Convert strings to bytes and validate length
            _encryptionKey = ConvertKeyToBytes(keyString, 32); // 32 bytes for AES-256
            _encryptionIv = ConvertKeyToBytes(ivString, 16);   // 16 bytes for AES IV
        }

        private static byte[] ConvertKeyToBytes(string key, int requiredLength)
        {
            // If key is hex string (64 chars for 32 bytes, 32 chars for 16 bytes)
            if (key.Length == requiredLength * 2)
            {
                try
                {
                    return Convert.FromHexString(key);
                }
                catch
                {
                    // Fall through to UTF-8 conversion
                }
            }

            // Convert UTF-8 string to bytes and pad/truncate if necessary
            var bytes = Encoding.UTF8.GetBytes(key);
            
            if (bytes.Length == requiredLength)
                return bytes;

            if (bytes.Length < requiredLength)
            {
                // Pad with zeros if too short
                var paddedBytes = new byte[requiredLength];
                Buffer.BlockCopy(bytes, 0, paddedBytes, 0, bytes.Length);
                return paddedBytes;
            }
            else
            {
                // Truncate if too long (not ideal, but better than exception)
                var truncatedBytes = new byte[requiredLength];
                Buffer.BlockCopy(bytes, 0, truncatedBytes, 0, requiredLength);
                return truncatedBytes;
            }
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));

            using (var aes = Aes.Create())
            {
                aes.Key = _encryptionKey;
                aes.IV = _encryptionIv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var ms = new System.IO.MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (var sw = new System.IO.StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException(nameof(cipherText));

            try
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = _encryptionKey;
                    aes.IV = _encryptionIv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (var ms = new System.IO.MemoryStream(Convert.FromBase64String(cipherText)))
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (var sr = new System.IO.StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Decryption failed. The cipher text may be corrupted or the key/IV is incorrect.", ex);
            }
        }
    }
}
