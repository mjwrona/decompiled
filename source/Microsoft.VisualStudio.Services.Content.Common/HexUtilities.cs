// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.HexUtilities
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class HexUtilities
  {
    public static bool IsHexString(this string data)
    {
      if (data == null)
        throw new ArgumentNullException(nameof (data));
      if (data.Length % 2 != 0)
        return false;
      for (int index = 0; index < data.Length; ++index)
      {
        char ch = data[index];
        if ((ch < '0' || ch > '9') && (ch < 'a' || ch > 'f') && (ch < 'A' || ch > 'F'))
          return false;
      }
      return true;
    }

    public static string ToHexString(this byte[] data) => data != null ? HexConverter.ToString(data) : throw new ArgumentNullException(nameof (data));

    public static byte[] ToByteArray(this string hexString)
    {
      if (hexString == null)
        throw new ArgumentNullException(nameof (hexString));
      byte[] bytes;
      if (!HexConverter.TryToByteArray(hexString, out bytes))
        throw new ArgumentException(nameof (hexString), Resources.InvalidHexString((object) hexString));
      return bytes;
    }
  }
}
