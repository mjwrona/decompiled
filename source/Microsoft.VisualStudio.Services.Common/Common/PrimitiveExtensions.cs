// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.PrimitiveExtensions
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections;
using System.Text;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class PrimitiveExtensions
  {
    public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    private static readonly long maxSecondsSinceUnixEpoch = (long) DateTime.MaxValue.Subtract(PrimitiveExtensions.UnixEpoch).TotalSeconds;

    public static long ToUnixEpochTime(this DateTime dateTime) => Convert.ToInt64((dateTime.ToUniversalTime() - PrimitiveExtensions.UnixEpoch).TotalSeconds);

    public static DateTime FromUnixEpochTime(this long unixTime) => unixTime >= PrimitiveExtensions.maxSecondsSinceUnixEpoch ? DateTime.MaxValue : PrimitiveExtensions.UnixEpoch + TimeSpan.FromSeconds((double) unixTime);

    public static string ToBase64StringNoPaddingFromString(string utf8String) => Encoding.UTF8.GetBytes(utf8String).ToBase64StringNoPadding();

    public static string FromBase64StringNoPaddingToString(string base64String)
    {
      byte[] bytes = base64String.FromBase64StringNoPadding();
      return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
    }

    public static string ToBase64StringNoPadding(this byte[] bytes)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) bytes, nameof (bytes));
      return Convert.ToBase64String(bytes).Split('=')[0].Replace('+', '-').Replace('/', '_');
    }

    public static byte[] FromBase64StringNoPadding(this string base64String)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(base64String, nameof (base64String));
      string s = base64String.Replace('-', '+').Replace('_', '/');
      switch (s.Length % 4)
      {
        case 0:
          return Convert.FromBase64String(s);
        case 2:
          s += "==";
          goto case 0;
        case 3:
          s += "=";
          goto case 0;
        default:
          throw new ArgumentException(CommonResources.IllegalBase64String(), nameof (base64String));
      }
    }

    public static string ConvertToHex(string base64String) => HexConverter.ToString(base64String.FromBase64StringNoPadding());
  }
}
