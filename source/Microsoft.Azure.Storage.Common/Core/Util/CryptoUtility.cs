// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.CryptoUtility
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal static class CryptoUtility
  {
    internal static string ComputeHmac256(byte[] key, string message)
    {
      using (HashAlgorithm hashAlgorithm = (HashAlgorithm) new HMACSHA256(key))
      {
        byte[] bytes = Encoding.UTF8.GetBytes(message);
        return Convert.ToBase64String(hashAlgorithm.ComputeHash(bytes));
      }
    }
  }
}
