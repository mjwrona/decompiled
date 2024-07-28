// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.ApiResourceVersionExtensions
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public static class ApiResourceVersionExtensions
  {
    public const string c_apiVersionHeaderKey = "api-version";
    internal const string c_legacyResourceVersionHeaderKey = "res-version";

    public static void AddApiResourceVersionValues(
      this ICollection<NameValueHeaderValue> headerValues,
      ApiResourceVersion version)
    {
      headerValues.AddApiResourceVersionValues(version, true, false);
    }

    public static void AddApiResourceVersionValues(
      this ICollection<NameValueHeaderValue> headerValues,
      ApiResourceVersion version,
      bool replaceExisting)
    {
      headerValues.AddApiResourceVersionValues(version, replaceExisting, false);
    }

    internal static void AddApiResourceVersionValues(
      this ICollection<NameValueHeaderValue> headerValues,
      ApiResourceVersion version,
      bool replaceExisting,
      bool useLegacyFormat)
    {
      string str = (string) null;
      string apiVersionString;
      if (useLegacyFormat)
      {
        apiVersionString = version.ApiVersionString;
        if (version.ResourceVersion > 0)
          str = version.ResourceVersion.ToString();
      }
      else
        apiVersionString = version.ToString();
      NameValueHeaderValue valueHeaderValue1 = headerValues.FirstOrDefault<NameValueHeaderValue>((Func<NameValueHeaderValue, bool>) (h => string.Equals("api-version", h.Name)));
      if (valueHeaderValue1 != null)
      {
        if (!replaceExisting)
          return;
        valueHeaderValue1.Value = apiVersionString;
        if (string.IsNullOrEmpty(str))
          return;
        NameValueHeaderValue valueHeaderValue2 = headerValues.FirstOrDefault<NameValueHeaderValue>((Func<NameValueHeaderValue, bool>) (h => string.Equals("res-version", h.Name)));
        if (valueHeaderValue2 != null)
          valueHeaderValue2.Value = str;
        else
          headerValues.Add(new NameValueHeaderValue("res-version", str));
      }
      else
      {
        headerValues.Add(new NameValueHeaderValue("api-version", apiVersionString));
        if (string.IsNullOrEmpty(str))
          return;
        headerValues.Add(new NameValueHeaderValue("res-version", str));
      }
    }
  }
}
