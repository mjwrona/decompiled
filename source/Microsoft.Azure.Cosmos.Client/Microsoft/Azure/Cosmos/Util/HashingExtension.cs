// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Util.HashingExtension
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.Azure.Cosmos.Util
{
  internal class HashingExtension
  {
    internal static string ComputeHash(string rawData)
    {
      if (string.IsNullOrEmpty(rawData))
        throw new ArgumentNullException(nameof (rawData));
      using (SHA256 shA256 = SHA256.Create())
      {
        byte[] hash = shA256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        Array.Resize<byte>(ref hash, 16);
        return new Guid(hash).ToString();
      }
    }
  }
}
