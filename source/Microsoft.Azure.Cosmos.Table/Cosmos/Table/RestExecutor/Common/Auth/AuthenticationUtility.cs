// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.Common.Auth.AuthenticationUtility
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.RestExecutor.Utils;
using System;
using System.Net.Http;
using System.Text;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.Common.Auth
{
  internal static class AuthenticationUtility
  {
    private const int ExpectedResourceStringLength = 100;

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

    public static string GetCanonicalizedHeaderValue(DateTimeOffset? value) => value.HasValue ? HttpWebUtility.ConvertDateTimeToHttpString(value.Value) : (string) null;

    public static string GetCanonicalizedResourceString(Uri uri, string accountName)
    {
      StringBuilder stringBuilder = new StringBuilder(100);
      stringBuilder.Append('/');
      stringBuilder.Append(accountName);
      stringBuilder.Append(AuthenticationUtility.GetAbsolutePathWithoutSecondarySuffix(uri, accountName));
      string str;
      if (HttpWebUtility.ParseQueryString(uri.Query).TryGetValue("comp", out str))
      {
        stringBuilder.Append("?comp=");
        stringBuilder.Append(str);
      }
      return stringBuilder.ToString();
    }

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
  }
}
