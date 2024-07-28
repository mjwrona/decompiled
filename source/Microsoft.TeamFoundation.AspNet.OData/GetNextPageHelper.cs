// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.GetNextPageHelper
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Formatting;
using System.Text;

namespace Microsoft.AspNet.OData
{
  internal static class GetNextPageHelper
  {
    internal static Uri GetNextPageLink(
      Uri requestUri,
      int pageSize,
      object instance = null,
      Func<object, string> objectToSkipTokenValue = null)
    {
      return GetNextPageHelper.GetNextPageLink(requestUri, (IEnumerable<KeyValuePair<string, string>>) new FormDataCollection(requestUri), pageSize, instance, objectToSkipTokenValue);
    }

    internal static Uri GetNextPageLink(
      Uri requestUri,
      IEnumerable<KeyValuePair<string, string>> queryParameters,
      int pageSize,
      object instance = null,
      Func<object, string> objectToSkipTokenValue = null,
      CompatibilityOptions options = CompatibilityOptions.None)
    {
      StringBuilder stringBuilder = new StringBuilder();
      int num = pageSize;
      string str1 = objectToSkipTokenValue == null ? (string) null : objectToSkipTokenValue(instance);
      bool flag = string.IsNullOrWhiteSpace(str1);
      foreach (KeyValuePair<string, string> queryParameter in queryParameters)
      {
        string lowerInvariant = queryParameter.Key.ToLowerInvariant();
        string str2 = queryParameter.Value;
        switch (lowerInvariant)
        {
          case "$top":
            int result1;
            if (int.TryParse(str2, out result1))
            {
              if ((options & CompatibilityOptions.AllowNextLinkWithNonPositiveTopValue) == CompatibilityOptions.None && result1 <= pageSize)
                return (Uri) null;
              str2 = (result1 - pageSize).ToString((IFormatProvider) CultureInfo.InvariantCulture);
              break;
            }
            break;
          case "$skip":
            int result2;
            if (flag && int.TryParse(str2, out result2))
            {
              num += result2;
              continue;
            }
            continue;
          case "$skiptoken":
            continue;
        }
        string str3 = lowerInvariant.Length <= 0 || lowerInvariant[0] != '$' ? Uri.EscapeDataString(lowerInvariant) : "$" + Uri.EscapeDataString(lowerInvariant.Substring(1));
        string str4 = Uri.EscapeDataString(str2);
        stringBuilder.Append(str3);
        stringBuilder.Append('=');
        stringBuilder.Append(str4);
        stringBuilder.Append('&');
      }
      if (flag)
        stringBuilder.AppendFormat("$skip={0}", (object) num);
      else
        stringBuilder.AppendFormat("$skiptoken={0}", (object) str1);
      return new UriBuilder(requestUri)
      {
        Query = stringBuilder.ToString()
      }.Uri;
    }
  }
}
