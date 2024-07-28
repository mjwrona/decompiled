// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.HttpWebUtility
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal static class HttpWebUtility
  {
    public static IDictionary<string, string> ParseQueryString(string query)
    {
      Dictionary<string, ICollection<string>> source = new Dictionary<string, ICollection<string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (string.IsNullOrEmpty(query))
        return (IDictionary<string, string>) source.ToDictionary<KeyValuePair<string, ICollection<string>>, string, string>((Func<KeyValuePair<string, ICollection<string>>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, ICollection<string>>, string>) (kvp => string.Join(",", (IEnumerable<string>) kvp.Value.OrderBy<string, string>((Func<string, string>) (_ => _)))), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (query.StartsWith("?", StringComparison.Ordinal))
      {
        if (query.Length == 1)
          return (IDictionary<string, string>) source.ToDictionary<KeyValuePair<string, ICollection<string>>, string, string>((Func<KeyValuePair<string, ICollection<string>>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, ICollection<string>>, string>) (kvp => string.Join(",", (IEnumerable<string>) kvp.Value.OrderBy<string, string>((Func<string, string>) (_ => _)))), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
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
        ICollection<string> strings;
        if (source.TryGetValue(key, out strings))
          strings.Add(str2);
        else
          source[key] = (ICollection<string>) new List<string>()
          {
            str2
          };
      }
      return (IDictionary<string, string>) source.ToDictionary<KeyValuePair<string, ICollection<string>>, string, string>((Func<KeyValuePair<string, ICollection<string>>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, ICollection<string>>, string>) (kvp => string.Join(",", (IEnumerable<string>) kvp.Value.OrderBy<string, string>((Func<string, string>) (_ => _)))), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public static string ConvertDateTimeToHttpString(DateTimeOffset dateTime) => dateTime.UtcDateTime.ToString("R", (IFormatProvider) CultureInfo.InvariantCulture);

    public static string CombineHttpHeaderValues(IEnumerable<string> headerValues)
    {
      if (headerValues == null)
        return (string) null;
      return headerValues.Count<string>() != 0 ? string.Join(",", headerValues) : (string) null;
    }

    public static string GetHeaderValues(string headerName, HttpHeaders headers)
    {
      IEnumerable<string> values = (IEnumerable<string>) null;
      return headers.TryGetValues(headerName, out values) ? HttpWebUtility.CombineHttpHeaderValues(values) : (string) null;
    }
  }
}
