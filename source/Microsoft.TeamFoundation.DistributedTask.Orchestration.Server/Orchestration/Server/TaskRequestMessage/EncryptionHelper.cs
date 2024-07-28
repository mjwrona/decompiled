// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskRequestMessage.EncryptionHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskRequestMessage
{
  internal class EncryptionHelper
  {
    public static string Encrypt(
      IVssRequestContext requestContext,
      string message,
      IAesEncryptionKeyProvider keyProvider)
    {
      byte[] key = keyProvider.GetKey(requestContext);
      EncryptionHelper.AesEncryptedMessage encryptedMessage = new EncryptionHelper.AesEncryptedMessage();
      using (AesCryptoServiceProvider cryptoServiceProvider = new AesCryptoServiceProvider())
      {
        cryptoServiceProvider.Key = key;
        cryptoServiceProvider.Mode = CipherMode.CBC;
        cryptoServiceProvider.Padding = PaddingMode.PKCS7;
        using (ICryptoTransform encryptor = cryptoServiceProvider.CreateEncryptor())
        {
          using (MemoryStream memoryStream = new MemoryStream())
          {
            using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, encryptor, CryptoStreamMode.Write))
            {
              using (StreamWriter streamWriter = new StreamWriter((Stream) cryptoStream, Encoding.UTF8, 1024, true))
                streamWriter.Write(message);
              cryptoStream.Flush();
              if (!cryptoStream.HasFlushedFinalBlock)
                cryptoStream.FlushFinalBlock();
              encryptedMessage.IV = Convert.ToBase64String(cryptoServiceProvider.IV);
              encryptedMessage.Message = Convert.ToBase64String(memoryStream.ToArray());
            }
          }
        }
      }
      return encryptedMessage.Serialize<EncryptionHelper.AesEncryptedMessage>();
    }

    public static string Decrypt(
      IVssRequestContext requestContext,
      string message,
      IAesEncryptionKeyProvider keyProvider)
    {
      EncryptionHelper.AesEncryptedMessage encryptedMessage = JsonUtilities.Deserialize<EncryptionHelper.AesEncryptedMessage>(message);
      byte[] key = keyProvider.GetKey(requestContext);
      using (AesCryptoServiceProvider cryptoServiceProvider = new AesCryptoServiceProvider())
      {
        cryptoServiceProvider.Key = key;
        cryptoServiceProvider.IV = Convert.FromBase64String(encryptedMessage.IV);
        cryptoServiceProvider.Mode = CipherMode.CBC;
        cryptoServiceProvider.Padding = PaddingMode.PKCS7;
        using (ICryptoTransform decryptor = cryptoServiceProvider.CreateDecryptor())
        {
          using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(encryptedMessage.Message)))
          {
            using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, decryptor, CryptoStreamMode.Read))
            {
              using (StreamReader streamReader = new StreamReader((Stream) cryptoStream, Encoding.UTF8))
                message = streamReader.ReadToEnd();
            }
          }
        }
      }
      return message;
    }

    private class AesEncryptedMessage
    {
      public string Message { get; set; }

      public string IV { get; set; }
    }
  }
}
