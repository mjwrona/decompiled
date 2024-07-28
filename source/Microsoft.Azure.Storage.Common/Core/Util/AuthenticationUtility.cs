// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.AuthenticationUtility
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal static class AuthenticationUtility
  {
    private const int ExpectedResourceStringLength = 100;
    private const int ExpectedHeaderNameAndValueLength = 50;
    private const char HeaderNameValueSeparator = ':';
    private const char HeaderValueDelimiter = ',';

    public static string GetPreferredDateHeaderValue(HttpRequestMessage request)
    {
      string singleValueOrDefault = request.Headers.GetHeaderSingleValueOrDefault("x-ms-date");
      return !string.IsNullOrEmpty(singleValueOrDefault) ? singleValueOrDefault : AuthenticationUtility.GetCanonicalizedHeaderValue(request.Headers.Date);
    }

    public static void AppendCanonicalizedContentLengthHeader(
      CanonicalizedString canonicalizedString,
      HttpRequestMessage request)
    {
      long? contentLength = request.Content.Headers.ContentLength;
      if (contentLength.HasValue && contentLength.Value != -1L && contentLength.Value != 0L)
        canonicalizedString.AppendCanonicalizedElement(contentLength.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      else
        canonicalizedString.AppendCanonicalizedElement((string) null);
    }

    public static void AppendCanonicalizedDateHeader(
      CanonicalizedString canonicalizedString,
      HttpRequestMessage request,
      bool allowMicrosoftDateHeader = false)
    {
      string singleValueOrDefault = request.Headers.GetHeaderSingleValueOrDefault("x-ms-date");
      if (string.IsNullOrEmpty(singleValueOrDefault))
        canonicalizedString.AppendCanonicalizedElement(AuthenticationUtility.GetCanonicalizedHeaderValue(request.Headers.Date));
      else if (allowMicrosoftDateHeader)
        canonicalizedString.AppendCanonicalizedElement(singleValueOrDefault);
      else
        canonicalizedString.AppendCanonicalizedElement((string) null);
    }

    public static void AppendCanonicalizedCustomHeaders(
      CanonicalizedString canonicalizedString,
      HttpRequestMessage request)
    {
      SortedDictionary<string, IEnumerable<string>> sortedDictionary = new SortedDictionary<string, IEnumerable<string>>((IComparer<string>) StringComparer.Create(new CultureInfo("en-US"), false));
      foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) request.Headers)
      {
        string key = header.Key;
        if (key.StartsWith("x-ms-", StringComparison.OrdinalIgnoreCase))
          sortedDictionary.Add(key.ToLowerInvariant(), header.Value);
      }
      if (request.Content != null)
      {
        foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) request.Content.Headers)
        {
          string key = header.Key;
          if (key.StartsWith("x-ms-", StringComparison.OrdinalIgnoreCase))
            sortedDictionary.Add(key.ToLowerInvariant(), header.Value);
        }
      }
      StringBuilder stringBuilder = new StringBuilder(50);
      foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair in sortedDictionary)
      {
        stringBuilder.Clear();
        stringBuilder.Append(keyValuePair.Key);
        stringBuilder.Append(':');
        int length = stringBuilder.Length;
        foreach (string str in keyValuePair.Value)
        {
          stringBuilder.Append(str.TrimStart().Replace("\r\n", string.Empty));
          stringBuilder.Append(',');
        }
        canonicalizedString.AppendCanonicalizedElement(stringBuilder.ToString(0, stringBuilder.Length - 1));
      }
    }

    public static string GetCanonicalizedHeaderValue(DateTimeOffset? value) => value.HasValue ? HttpWebUtility.ConvertDateTimeToHttpString(value.Value) : (string) null;

    private static string GetAbsolutePathWithoutSecondarySuffix(Uri uri, string accountName)
    {
      string withoutSecondarySuffix = uri.AbsolutePath;
      string str = accountName + "-secondary";
      int num = withoutSecondarySuffix.IndexOf(str, StringComparison.OrdinalIgnoreCase);
      if (num == 1)
      {
        int startIndex = num + accountName.Length;
        withoutSecondarySuffix = withoutSecondarySuffix.Remove(startIndex, "-secondary".Length);
      }
      return withoutSecondarySuffix;
    }

    public static string GetCanonicalizedResourceString(
      Uri uri,
      string accountName,
      bool isSharedKeyLiteOrTableService = false)
    {
      StringBuilder stringBuilder = new StringBuilder(100);
      stringBuilder.Append('/');
      stringBuilder.Append(accountName);
      stringBuilder.Append(AuthenticationUtility.GetAbsolutePathWithoutSecondarySuffix(uri, accountName));
      IDictionary<string, string> queryString = HttpWebUtility.ParseQueryString(uri.Query);
      if (!isSharedKeyLiteOrTableService)
      {
        List<string> stringList = new List<string>((IEnumerable<string>) queryString.Keys);
        stringList.Sort((IComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (string key in stringList)
        {
          stringBuilder.Append('\n');
          stringBuilder.Append(key.ToLowerInvariant());
          stringBuilder.Append(':');
          stringBuilder.Append(queryString[key]);
        }
      }
      else
      {
        string str;
        if (queryString.TryGetValue("comp", out str))
        {
          stringBuilder.Append("?comp=");
          stringBuilder.Append(str);
        }
      }
      return stringBuilder.ToString();
    }
  }
}
