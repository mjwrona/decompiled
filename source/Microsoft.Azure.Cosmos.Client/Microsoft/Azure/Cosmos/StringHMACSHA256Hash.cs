// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.StringHMACSHA256Hash
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Concurrent;
using System.Security;
using System.Security.Cryptography;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class StringHMACSHA256Hash : IComputeHash, IDisposable
  {
    private readonly string base64EncodedKey;
    private readonly byte[] keyBytes;
    private SecureString secureString;
    private ConcurrentQueue<HMACSHA256> hmacPool;

    public StringHMACSHA256Hash(string base64EncodedKey)
    {
      this.base64EncodedKey = base64EncodedKey;
      this.keyBytes = Convert.FromBase64String(base64EncodedKey);
      this.hmacPool = new ConcurrentQueue<HMACSHA256>();
    }

    public byte[] ComputeHash(ArraySegment<byte> bytesToHash)
    {
      HMACSHA256 result;
      if (this.hmacPool.TryDequeue(out result))
        result.Initialize();
      else
        result = new HMACSHA256(this.keyBytes);
      try
      {
        return result.ComputeHash(bytesToHash.Array, 0, bytesToHash.Count);
      }
      finally
      {
        this.hmacPool.Enqueue(result);
      }
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
      HMACSHA256 result;
      while (this.hmacPool.TryDequeue(out result))
        result.Dispose();
      if (this.secureString == null)
        return;
      this.secureString.Dispose();
      this.secureString = (SecureString) null;
    }
  }
}
