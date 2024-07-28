// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.CryptoUtility
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.Azure.Cosmos.Table
{
  internal static class CryptoUtility
  {
    internal static string ComputeHmac256(string keyValue, string message)
    {
      using (HashAlgorithm hashAlgorithm = (HashAlgorithm) new HMACSHA256(Convert.FromBase64String(keyValue)))
      {
        byte[] bytes = Encoding.UTF8.GetBytes(message);
        return Convert.ToBase64String(hashAlgorithm.ComputeHash(bytes));
      }
    }
  }
}
