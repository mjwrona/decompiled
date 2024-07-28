// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureServiceEndpointExtensions
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public static class AzureServiceEndpointExtensions
  {
    public static string GetEndpointUrl(this ServiceEndpoint serviceEndpoint) => serviceEndpoint.Url.AbsoluteUri;

    public static string GetResourceUrl(
      this ServiceEndpoint serviceEndpoint,
      IVssRequestContext requestContext)
    {
      if (!"AzureStack".Equals(serviceEndpoint.GetAzureEnvironmentName(), StringComparison.OrdinalIgnoreCase))
        return serviceEndpoint.Url.OriginalString;
      string environmentDataUrl = serviceEndpoint.GetEnvironmentDataUrl(requestContext, "ActiveDirectoryServiceEndpointResourceId");
      return !string.IsNullOrEmpty(environmentDataUrl) ? environmentDataUrl.TrimEnd('/') : throw new InvalidAuthorizationDetailsException(ServiceEndpointSdkResources.FailedToObtainAzureStackActiveDirectoryResourceId());
    }

    public static string GetAzureEnvironmentName(this ServiceEndpoint serviceEndpoint)
    {
      string str;
      return !serviceEndpoint.Data.TryGetValue("environment", out str) ? "AzureCloud" : str;
    }

    public static string GetAuthorityUrl(
      this ServiceEndpoint serviceEndpoint,
      IVssRequestContext requestContext)
    {
      string parameter = serviceEndpoint.Authorization.Parameters["TenantId"];
      string environmentDataUrl = serviceEndpoint.GetEnvironmentDataUrl(requestContext, "environmentAuthorityUrl");
      string str1 = string.IsNullOrEmpty(environmentDataUrl) ? "https://login.windows.net/" : environmentDataUrl;
      string str2 = str1.EndsWith("/") ? str1 : str1 + "/";
      string str3;
      serviceEndpoint.Authorization.Parameters.TryGetValue("EnableAdfsAuthentication", out str3);
      return (str3 == null ? (str2.EndsWith("/adfs/") ? 1 : 0) : (Convert.ToBoolean(str3) ? 1 : 0)) == 0 ? str2 + parameter : str2;
    }

    public static bool IsAdfsAuthenticationEnabled(this ServiceEndpoint serviceEndpoint)
    {
      string str;
      return serviceEndpoint.Data.TryGetValue("EnableAdfsAuthentication", out str) && bool.TrueString.Equals(str, StringComparison.OrdinalIgnoreCase);
    }

    public static string GetEnvironmentDataUrl(
      this ServiceEndpoint serviceEndpoint,
      IVssRequestContext requestContext,
      string environmentDataUrlKey)
    {
      if (serviceEndpoint.Data == null || string.IsNullOrEmpty(environmentDataUrlKey))
        return (string) null;
      string selectedEnvironment = serviceEndpoint.GetAzureEnvironmentName();
      if ("AzureStack".Equals(selectedEnvironment, StringComparison.OrdinalIgnoreCase))
      {
        string environmentDataUrl;
        if (!serviceEndpoint.Data.TryGetValue(environmentDataUrlKey, out environmentDataUrl))
          serviceEndpoint.GetAzureStackDependencyData().TryGetValue(environmentDataUrlKey, out environmentDataUrl);
        return environmentDataUrl;
      }
      ServiceEndpointType serviceEndpointType = requestContext.GetService<IServiceEndpointTypesService2>().GetServiceEndpointTypes(requestContext, serviceEndpoint.Type, serviceEndpoint.Authorization.Scheme).FirstOrDefault<ServiceEndpointType>();
      if (serviceEndpointType?.DependencyData != null)
        return serviceEndpointType.DependencyData.Where<DependencyData>((Func<DependencyData, bool>) (d => "environment".Equals(d.Input, StringComparison.OrdinalIgnoreCase))).SelectMany<DependencyData, KeyValuePair<string, List<KeyValuePair<string, string>>>>((Func<DependencyData, IEnumerable<KeyValuePair<string, List<KeyValuePair<string, string>>>>>) (d => (IEnumerable<KeyValuePair<string, List<KeyValuePair<string, string>>>>) d.Map)).Where<KeyValuePair<string, List<KeyValuePair<string, string>>>>((Func<KeyValuePair<string, List<KeyValuePair<string, string>>>, bool>) (m => !string.IsNullOrEmpty(m.Key) && m.Key.Equals(selectedEnvironment, StringComparison.OrdinalIgnoreCase))).SelectMany<KeyValuePair<string, List<KeyValuePair<string, string>>>, KeyValuePair<string, string>>((Func<KeyValuePair<string, List<KeyValuePair<string, string>>>, IEnumerable<KeyValuePair<string, string>>>) (m => (IEnumerable<KeyValuePair<string, string>>) m.Value)).Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (d => environmentDataUrlKey.Equals(d.Key, StringComparison.OrdinalIgnoreCase))).Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (d => d.Value)).FirstOrDefault<string>();
      requestContext.TraceWarning("WebApiProxy", "Unable to fetch dependency data for endpoint type : {0}", (object) serviceEndpointType);
      return (string) null;
    }
  }
}
