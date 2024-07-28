// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.UrlHelper
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public class UrlHelper
  {
    public static IPAddress[] VerifyUrlIsAllowedIPAddress(string url, bool throwIfInvalidHost = false)
    {
      IPAddress[] hostAddresses = IPAddressRange.GetHostAddresses(url, throwIfInvalidHost);
      if (IPAddressRange.IsLoopbackIPAddress(hostAddresses) || IPAddressRange.IsLocalhostIPAddress(hostAddresses, out string _))
        throw new ServiceEndpointException(ServiceEndpointSdkResources.InvalidUrlLoopback());
      string matchedHostIPValue;
      return !IPAddressRange.IsSpecialPurposeIPAddress(hostAddresses, out matchedHostIPValue, out string _) ? hostAddresses : throw new ServiceEndpointException(string.Format(ServiceEndpointSdkResources.InvalidUrlSpecialPurposeFormat((object) url), (object) matchedHostIPValue));
    }

    public static bool IsSubDomain(string subDomain, string domain)
    {
      subDomain = subDomain.TrimEnd('/');
      if (string.IsNullOrEmpty(subDomain) || string.IsNullOrEmpty(domain))
        return false;
      return subDomain.EndsWith("." + domain, StringComparison.OrdinalIgnoreCase) || subDomain.Equals(domain, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsAbsolute(string url) => url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

    public static Uri ValidateAndNormalize(
      IVssRequestContext requestContext,
      string workUrl,
      string endpointUrl,
      IList<TaskSourceDefinition> sourceDefinitions,
      bool supportsAbsoluteEndpoint)
    {
      int num = UrlHelper.IsAbsolute(workUrl) ? 1 : 0;
      if (num != 0)
      {
        if (!supportsAbsoluteEndpoint)
          throw new InvalidOperationException(ServiceEndpointSdkResources.AbsoluteUriNotAllowed());
      }
      else if (!string.IsNullOrWhiteSpace(endpointUrl))
      {
        if (!endpointUrl.EndsWith("/"))
          endpointUrl += "/";
        if (workUrl.StartsWith("/"))
          workUrl = workUrl.Substring(1);
        workUrl = endpointUrl + workUrl;
      }
      if (num != 0)
      {
        UrlHelper.ValidateEndpoint(sourceDefinitions, workUrl);
      }
      else
      {
        try
        {
          UrlHelper.ValidateEndpoint(sourceDefinitions, workUrl);
        }
        catch (Exception ex)
        {
          if (!requestContext.IsFeatureEnabled("ServiceEndpoints.EndpointProxyValidateRelativeUrlsErrorLogging.Disabled"))
            requestContext.TraceWarning("WebApiProxy", ex.Message);
        }
      }
      return new Uri(workUrl);
    }

    public static void ValidateEndpoint(
      IList<TaskSourceDefinition> sourceDefinitions,
      string workUrl)
    {
      Uri uri = new Uri(workUrl);
      foreach (TaskSourceDefinition sourceDefinition in (IEnumerable<TaskSourceDefinition>) sourceDefinitions)
      {
        if (!string.IsNullOrWhiteSpace(sourceDefinition.Endpoint) && UrlHelper.IsAbsolute(sourceDefinition.Endpoint) && new Uri(sourceDefinition.Endpoint).Host.Equals(uri.Host, StringComparison.OrdinalIgnoreCase))
          return;
      }
      throw new ServiceEndpointUntrustedHostException(ServiceEndpointSdkResources.UntrustedHost((object) uri.Host));
    }
  }
}
