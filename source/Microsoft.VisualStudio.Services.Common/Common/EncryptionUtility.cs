// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.EncryptionUtility
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class EncryptionUtility
  {
    public static readonly AlgorithmIdentifier Aes256Algorithm = new AlgorithmIdentifier(new Oid("2.16.840.1.101.3.4.1.42", "aes"));
    public static readonly AlgorithmIdentifier TripleDesAlgorithm = new AlgorithmIdentifier(new Oid("1.2.840.113549.3.7", "3des"));

    public static string EncryptSecret(string secret, X509Certificate2 cert) => EncryptionUtility.EncryptSecret(secret, cert, EncryptionUtility.Aes256Algorithm);

    public static string EncryptSecret(
      string secret,
      X509Certificate2 cert,
      AlgorithmIdentifier algorithm)
    {
      EnvelopedCms envelopedCms = new EnvelopedCms(new ContentInfo(Encoding.UTF8.GetBytes(secret)), algorithm);
      envelopedCms.Encrypt(new CmsRecipient(cert));
      return Convert.ToBase64String(envelopedCms.Encode());
    }

    public static SecureString DecryptSecret(string encryptedSecret) => EncryptionUtility.DecryptSecretHelper(encryptedSecret).ToSecureString();

    public static bool TryDecryptSecret(string encryptedSecret, out SecureString secureSecret)
    {
      secureSecret = (SecureString) null;
      try
      {
        secureSecret = EncryptionUtility.DecryptSecret(encryptedSecret);
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    [Obsolete("Use DecryptSecret which returns a SecureString instead.", false)]
    public static string DecryptSecretInsecure(string encryptedSecret) => Encoding.UTF8.GetString(EncryptionUtility.DecryptSecretHelper(encryptedSecret));

    [Obsolete("Use TryDecryptSecret which returns a SecureString instead.", false)]
    public static string TryDecryptSecretInsecure(string encryptedSecret)
    {
      try
      {
        return EncryptionUtility.DecryptSecretInsecure(encryptedSecret);
      }
      catch (Exception ex)
      {
        return encryptedSecret;
      }
    }

    private static byte[] DecryptSecretHelper(string encryptedSecret)
    {
      byte[] encodedMessage = Convert.FromBase64String(encryptedSecret);
      EnvelopedCms envelopedCms = new EnvelopedCms();
      X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
      x509Store.Open(OpenFlags.ReadOnly);
      try
      {
        envelopedCms.Decode(encodedMessage);
        envelopedCms.Decrypt(x509Store.Certificates);
        return envelopedCms.ContentInfo.Content;
      }
      finally
      {
        x509Store.Close();
      }
    }

    public static SecureString ToSecureString(this byte[] bytes)
    {
      if (bytes == null || bytes.Length == 0)
        return (SecureString) null;
      char[] chars = (char[]) null;
      try
      {
        chars = Encoding.UTF8.GetChars(bytes);
        return chars.ToSecureString();
      }
      finally
      {
        if (chars != null)
          Array.Clear((Array) chars, 0, chars.Length);
      }
    }

    public static SecureString ToSecureString(this string s) => s.ToCharArray().ToSecureString();

    public static SecureString ToSecureString(this char[] chars)
    {
      SecureString secureString = new SecureString();
      foreach (char c in chars)
        secureString.AppendChar(c);
      secureString.MakeReadOnly();
      return secureString;
    }

    public static unsafe byte[] ToByteArray(this SecureString secureString)
    {
      if (secureString == null)
        return Array.Empty<byte>();
      char[] chArray = new char[secureString.Length];
      IntPtr globalAllocUnicode = Marshal.SecureStringToGlobalAllocUnicode(secureString);
      try
      {
        fixed (char* chPtr1 = chArray)
        {
          char* chPtr2 = chPtr1;
          try
          {
            Marshal.Copy(globalAllocUnicode, chArray, 0, chArray.Length);
            return Encoding.UTF8.GetBytes(chArray);
          }
          finally
          {
            Array.Clear((Array) chArray, 0, chArray.Length);
          }
        }
      }
      finally
      {
        Marshal.ZeroFreeGlobalAllocUnicode(globalAllocUnicode);
      }
    }
  }
}
