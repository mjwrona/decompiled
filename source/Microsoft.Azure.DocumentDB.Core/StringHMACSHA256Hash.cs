// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.StringHMACSHA256Hash
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Security;
using System.Security.Cryptography;

namespace Microsoft.Azure.Documents
{
  internal sealed class StringHMACSHA256Hash : IComputeHash, IDisposable
  {
    private readonly string base64EncodedKey;
    private readonly byte[] keyBytes;
    private SecureString secureString;

    public StringHMACSHA256Hash(string base64EncodedKey)
    {
      this.base64EncodedKey = base64EncodedKey;
      this.keyBytes = Convert.FromBase64String(base64EncodedKey);
    }

    public byte[] ComputeHash(byte[] bytesToHash)
    {
      using (HMACSHA256 hmacshA256 = new HMACSHA256(this.keyBytes))
        return hmacshA256.ComputeHash(bytesToHash);
    }

    public SecureString Key
    {
      get
      {
        if (this.secureString != null)
          return this.secureString;
        this.secureString = SecureStringUtility.ConvertToSecureString(this.base64EncodedKey);
        return this.secureString;
      }
    }

    public void Dispose()
    {
      if (this.secureString == null)
        return;
      this.secureString.Dispose();
      this.secureString = (SecureString) null;
    }
  }
}
