// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssApiResourceVersionExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class VssApiResourceVersionExtensions
  {
    private const int c_backCompatBefore = 3;
    private const string c_apiVersionHeaderKey = "api-version";
    private const string c_resourceVersionHeaderKey = "res-version";

    public static ApiResourceVersion GetApiResourceVersion(this HttpRequestMessage request) => request.GetApiResourceVersion((Version) null);

    public static ApiResourceVersion GetApiResourceVersion(this HttpRequestBase request) => request.GetApiResourceVersion((Version) null);

    public static ApiResourceVersion GetApiResourceVersion(
      this HttpRequestMessage request,
      Version defaultVersion)
    {
      string apiVersion;
      string resourceVersion;
      request.ParseApiResourceVersionAndResourceVersion(out apiVersion, out resourceVersion);
      return VssApiResourceVersionExtensions.GetApiResourceVersion(apiVersion, resourceVersion, defaultVersion);
    }

    public static ApiResourceVersion GetApiResourceVersion(
      this HttpRequestBase request,
      Version defaultVersion)
    {
      string apiVersion;
      string resourceVersion;
      request.ParseApiResourceVersionAndResourceVersion(out apiVersion, out resourceVersion);
      return VssApiResourceVersionExtensions.GetApiResourceVersion(apiVersion, resourceVersion, defaultVersion);
    }

    public static Version GetApiVersionSafe(this HttpRequestMessage request)
    {
      try
      {
        return request.GetApiVersion();
      }
      catch (VssServiceException ex)
      {
        return (Version) null;
      }
    }

    public static Version GetApiVersion(this HttpRequestMessage request) => request.GetApiResourceVersion()?.ApiVersion;

    public static Version GetApiVersion(this HttpRequestBase request) => request.GetApiResourceVersion()?.ApiVersion;

    internal static void ParseApiResourceVersionAndResourceVersion(
      this HttpRequestMessage request,
      out string apiVersion,
      out string resourceVersion)
    {
      apiVersion = request.GetQueryStringValue("api-version");
      resourceVersion = string.IsNullOrEmpty(apiVersion) ? (string) null : request.GetQueryStringValue("res-version");
      IEnumerable<string> values;
      if (!string.IsNullOrEmpty(apiVersion) && !string.IsNullOrEmpty(resourceVersion) || !request.Headers.TryGetValues("accept", out values))
        return;
      string apiVersionString;
      string resourceVersionString;
      VssApiResourceVersionExtensions.ParseAcceptHeaderForApiVersion(values, out apiVersionString, out resourceVersionString);
      if (string.IsNullOrEmpty(apiVersion))
        apiVersion = apiVersionString;
      if (!string.IsNullOrEmpty(resourceVersion))
        return;
      resourceVersion = resourceVersionString;
    }

    private static ApiResourceVersion GetApiResourceVersion(
      string apiVersionString,
      string resourceVersionString,
      Version defaultVersion)
    {
      if (string.IsNullOrEmpty(apiVersionString))
        return defaultVersion == (Version) null ? (ApiResourceVersion) null : new ApiResourceVersion(defaultVersion);
      ApiResourceVersion apiResourceVersion = new ApiResourceVersion(apiVersionString);
      int result = 0;
      if (!string.IsNullOrEmpty(resourceVersionString) && int.TryParse(resourceVersionString, out result))
      {
        apiResourceVersion.ResourceVersion = result;
        apiResourceVersion.IsPreview = true;
      }
      return apiResourceVersion;
    }

    private static void ParseApiResourceVersionAndResourceVersion(
      this HttpRequestBase request,
      out string apiVersion,
      out string resourceVersion)
    {
      apiVersion = (string) null;
      resourceVersion = (string) null;
      foreach (string allKey in request.QueryString.AllKeys)
      {
        if (string.Equals(allKey, "api-version", StringComparison.OrdinalIgnoreCase))
          apiVersion = request.QueryString[allKey];
        else if (string.Equals(allKey, "res-version", StringComparison.OrdinalIgnoreCase))
          resourceVersion = request.QueryString[allKey];
      }
      if (!string.IsNullOrEmpty(apiVersion) && !string.IsNullOrEmpty(resourceVersion))
        return;
      string name = ((IEnumerable<string>) request.Headers.AllKeys).FirstOrDefault<string>((Func<string, bool>) (k => k.Equals("accept", StringComparison.OrdinalIgnoreCase)));
      if (name == null)
        return;
      string apiVersionString;
      string resourceVersionString;
      VssApiResourceVersionExtensions.ParseAcceptHeaderForApiVersion((IEnumerable<string>) new string[1]
      {
        request.Headers[name]
      }, out apiVersionString, out resourceVersionString);
      if (string.IsNullOrEmpty(apiVersion))
        apiVersion = apiVersionString;
      if (!string.IsNullOrEmpty(resourceVersion))
        return;
      resourceVersion = resourceVersionString;
    }

    private static void ParseAcceptHeaderForApiVersion(
      IEnumerable<string> acceptHeaderValues,
      out string apiVersionString,
      out string resourceVersionString)
    {
      apiVersionString = (string) null;
      resourceVersionString = (string) null;
      foreach (string acceptHeaderValue in acceptHeaderValues)
      {
        char[] chArray1 = new char[1]{ ';' };
        foreach (string str in acceptHeaderValue.Split(chArray1))
        {
          char[] chArray2 = new char[1]{ '=' };
          string[] strArray = str.Split(chArray2);
          if (strArray.Length == 2)
          {
            if (string.Equals(strArray[0].Trim(), "api-version", StringComparison.OrdinalIgnoreCase))
              apiVersionString = strArray[1].Trim();
            else if (string.Equals(strArray[0].Trim(), "res-version", StringComparison.OrdinalIgnoreCase))
              resourceVersionString = strArray[1].Trim();
          }
        }
      }
    }

    public static bool IsBackCompatApiResourceVersion(this HttpRequestMessage request)
    {
      ApiResourceVersion apiResourceVersion = request.GetApiResourceVersion();
      return apiResourceVersion != null && apiResourceVersion.ApiVersion.Major < 3;
    }
  }
}
