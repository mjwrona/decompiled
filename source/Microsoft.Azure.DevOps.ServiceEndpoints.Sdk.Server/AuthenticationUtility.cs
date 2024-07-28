// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AuthenticationUtility
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  internal static class AuthenticationUtility
  {
    private const int ExpectedResourceStringLength = 100;
    private const int ExpectedHeaderNameAndValueLength = 50;
    private const char HeaderNameValueSeparator = ':';
    private const string c_secondaryLocationAccountSuffix = "-secondary";

    public static string GetPreferredDateHeaderValue(HttpWebRequest request)
    {
      string header = request.Headers["x-ms-date"];
      return !string.IsNullOrEmpty(header) ? header : request.Headers[HttpRequestHeader.Date];
    }

    public static void AppendCanonicalizedContentLengthHeader(
      CanonicalizedString canonicalizedString,
      HttpWebRequest request)
    {
      if (request.ContentLength != -1L && request.ContentLength != 0L)
        canonicalizedString.AppendCanonicalizedElement(request.ContentLength.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      else
        canonicalizedString.AppendCanonicalizedElement((string) null);
    }

    public static void AppendCanonicalizedDateHeader(
      CanonicalizedString canonicalizedString,
      HttpWebRequest request,
      bool allowMicrosoftDateHeader = false)
    {
      string header = request.Headers["x-ms-date"];
      if (string.IsNullOrEmpty(header))
        canonicalizedString.AppendCanonicalizedElement(request.Headers[HttpRequestHeader.Date]);
      else if (allowMicrosoftDateHeader)
        canonicalizedString.AppendCanonicalizedElement(header);
      else
        canonicalizedString.AppendCanonicalizedElement((string) null);
    }

    public static void AppendCanonicalizedCustomHeaders(
      CanonicalizedString canonicalizedString,
      HttpWebRequest request)
    {
      List<string> stringList = new List<string>(request.Headers.AllKeys.Length);
      foreach (string allKey in request.Headers.AllKeys)
      {
        if (allKey.StartsWith("x-ms-", StringComparison.OrdinalIgnoreCase))
          stringList.Add(allKey.ToLowerInvariant());
      }
      StringComparer stringComparer = StringComparer.Create(new CultureInfo("en-US"), false);
      stringList.Sort((IComparer<string>) stringComparer);
      StringBuilder stringBuilder = new StringBuilder(50);
      foreach (string name in stringList)
      {
        string header = request.Headers[name];
        stringBuilder.Length = 0;
        stringBuilder.Append(name);
        stringBuilder.Append(':');
        stringBuilder.Append(header.TrimStart().Replace("\r\n", string.Empty));
        canonicalizedString.AppendCanonicalizedElement(stringBuilder.ToString());
      }
    }

    public static string GetCanonicalizedHeaderValue(DateTimeOffset? value) => value.HasValue ? value.Value.UtcDateTime.ToString("R", (IFormatProvider) CultureInfo.InvariantCulture) : (string) null;

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
      IDictionary<string, string> queryString = AuthenticationUtility.ParseQueryString(uri.Query);
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

    private static class HeaderConstants
    {
      public const string Date = "x-ms-date";
      public const string PrefixForStorageHeader = "x-ms-";
    }
  }
}
