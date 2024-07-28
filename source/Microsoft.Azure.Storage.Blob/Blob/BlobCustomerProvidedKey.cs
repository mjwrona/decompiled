// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.BlobCustomerProvidedKey
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using System;
using System.Security.Cryptography;

namespace Microsoft.Azure.Storage.Blob
{
  public sealed class BlobCustomerProvidedKey
  {
    public string Key { get; private set; }

    public string KeySHA256 { get; private set; }

    public string EncryptionAlgorithm { get; private set; }

    public BlobCustomerProvidedKey(string key)
    {
      this.Key = key;
      this.EncryptionAlgorithm = "AES256";
      using (SHA256 shA256 = SHA256.Create())
        this.KeySHA256 = Convert.ToBase64String(shA256.ComputeHash(Convert.FromBase64String(key)));
    }

    public BlobCustomerProvidedKey(byte[] key)
    {
      this.Key = Convert.ToBase64String(key);
      this.EncryptionAlgorithm = "AES256";
      using (SHA256 shA256 = SHA256.Create())
        this.KeySHA256 = Convert.ToBase64String(shA256.ComputeHash(key));
    }
  }
}
