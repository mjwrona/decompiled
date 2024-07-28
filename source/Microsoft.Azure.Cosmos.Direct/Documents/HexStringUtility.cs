// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.HexStringUtility
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;

namespace Microsoft.Azure.Documents
{
  internal static class HexStringUtility
  {
    internal static byte[] HexStringToBytes(string hexString)
    {
      if (hexString == null)
        return (byte[]) null;
      byte[] bytes = new byte[hexString.Length / 2];
      for (int startIndex = 0; startIndex < hexString.Length; startIndex += 2)
        bytes[startIndex / 2] = Convert.ToByte(hexString.Substring(startIndex, 2), 16);
      return bytes;
    }
  }
}
