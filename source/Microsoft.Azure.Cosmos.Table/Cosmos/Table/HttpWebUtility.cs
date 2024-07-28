// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.HttpWebUtility
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Cosmos.Table
{
  internal static class HttpWebUtility
  {
    public static IDictionary<string, string> ParseQueryString(string query)
    {
      Dictionary<string, string> queryString = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (string.IsNullOrEmpty(query))
        return (IDictionary<string, string>) queryString;
      if (query.StartsWith("?", StringComparison.Ordinal))
      {
        if (query.Length == 1)
          return (IDictionary<string, string>) queryString;
        query = query.Substring(1);
      }
      string str1 = query;
      char[] chArray = new char[1]{ '&' };
      foreach (string stringToUnescape in str1.Split(chArray))
      {
        int length = stringToUnescape.IndexOf("=", StringComparison.Ordinal);
        string key;
        string str2;
        if (length < 0)
        {
          key = string.Empty;
          str2 = Uri.UnescapeDataString(stringToUnescape);
        }
        else
        {
          key = Uri.UnescapeDataString(stringToUnescape.Substring(0, length));
          str2 = Uri.UnescapeDataString(stringToUnescape.Substring(length + 1));
        }
        string str3;
        queryString[key] = !queryString.TryGetValue(key, out str3) ? str2 : str3 + "," + str2;
      }
      return (IDictionary<string, string>) queryString;
    }

    public static string ConvertDateTimeToHttpString(DateTimeOffset dateTime) => dateTime.UtcDateTime.ToString("R", (IFormatProvider) CultureInfo.InvariantCulture);

    public static string CombineHttpHeaderValues(IEnumerable<string> headerValues)
    {
      if (headerValues == null)
        return (string) null;
      return headerValues.Count<string>() != 0 ? string.Join(",", headerValues) : (string) null;
    }
  }
}
