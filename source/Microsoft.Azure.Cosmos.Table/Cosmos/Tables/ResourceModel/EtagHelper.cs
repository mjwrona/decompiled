// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tables.ResourceModel.EtagHelper
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Tables.SharedFiles;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Cosmos.Tables.ResourceModel
{
  internal static class EtagHelper
  {
    internal const string ETagPrefix = "\"datetime'";
    private static List<int> byteOrder = new List<int>()
    {
      13,
      12,
      11,
      10,
      9,
      8,
      15,
      14,
      1,
      0,
      3,
      2,
      7,
      6,
      5,
      4
    };

    public static DateTimeOffset GetTimeOffSetFromBackEndETagFormat(string etag)
    {
      etag = !string.IsNullOrEmpty(etag) ? etag.Trim('"') : throw new InvalidOperationException();
      byte[] byteArray = new Guid(etag).ToByteArray();
      byte[] numArray = new byte[8];
      for (int index = 0; index < 8; ++index)
        Buffer.SetByte((Array) numArray, index, byteArray[EtagHelper.byteOrder[index]]);
      return DateTimeOffset.FromFileTime(BitConverter.ToInt64(numArray, 0)).ToUniversalTime();
    }

    public static string ConvertFromBackEndETagFormat(string etag) => string.IsNullOrEmpty(etag) ? etag : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "W/\"datetime'{0}'\"", (object) Uri.EscapeDataString(EtagHelper.GetTimeOffSetFromBackEndETagFormat(etag).UtcDateTime.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture)));

    public static string ConvertToBackEndETagFormat(string tableEtag)
    {
      if (string.IsNullOrEmpty(tableEtag) || tableEtag == "*")
        return tableEtag;
      string str1 = tableEtag;
      string str2 = str1.StartsWith("W/", StringComparison.Ordinal) ? str1.Substring(2) : throw new InvalidEtagException("Invalid Etag format.", tableEtag);
      if (str2.Length < "\"datetime'".Length + 2)
        throw new InvalidEtagException("Invalid Etag format.", tableEtag);
      if (!str2.StartsWith("\"datetime'", StringComparison.Ordinal))
        throw new InvalidEtagException("Invalid Etag format.", tableEtag);
      DateTimeOffset result;
      if (!DateTimeOffset.TryParse(Uri.UnescapeDataString(str2.Substring("\"datetime'".Length, str2.Length - 2 - "\"datetime'".Length)), (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
        throw new InvalidEtagException("Invalid Etag format.", tableEtag);
      byte[] bytes1 = BitConverter.GetBytes(result.ToFileTime());
      byte[] b = new byte[16];
      byte[] bytes2 = BitConverter.GetBytes(0L);
      for (int index = 0; index < 8; ++index)
      {
        byte num = index < 8 ? bytes1[index] : bytes2[0];
        Buffer.SetByte((Array) b, EtagHelper.byteOrder[index], num);
      }
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"{0}\"", (object) new Guid(b).ToString());
    }
  }
}
