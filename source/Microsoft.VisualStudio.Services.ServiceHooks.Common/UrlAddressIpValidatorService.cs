// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.UrlAddressIpValidatorService
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public class UrlAddressIpValidatorService : IUrlAddressIpValidatorService, IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void VerifyUrlIsAllowedIPAddress(
      IVssRequestContext requestContext,
      string url,
      bool throwIfInvalidHost = true)
    {
      this.VerifyUrlIsAllowedIPAddressInternal(requestContext, url, throwIfInvalidHost);
    }

    protected virtual IPAddress[] VerifyUrlIsAllowedIPAddressInternal(
      IVssRequestContext requestContext,
      string url,
      bool throwIfInvalidHost)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment || requestContext.ExecutionEnvironment.IsDevFabricDeployment)
        return Array.Empty<IPAddress>();
      IPAddress[] hostAddresses = IPAddressRange.GetHostAddresses(url, throwIfInvalidHost);
      if (IPAddressRange.IsLoopbackIPAddress(hostAddresses) || IPAddressRange.IsLocalhostIPAddress(hostAddresses, out string _))
        throw new SubscriptionInputException(CommonResources.InvalidUrlLoopback);
      string matchedHostIPValue;
      return !IPAddressRange.IsSpecialPurposeIPAddress(hostAddresses, out matchedHostIPValue, out string _) ? hostAddresses : throw new SubscriptionInputException(string.Format(CommonResources.InvalidUrlSpecialPurposeFormat, (object) matchedHostIPValue));
    }

    public void ApplyIPAddressAllowedRangeOnHttpRequest(
      IVssRequestContext requestContext,
      HttpRequestMessage message)
    {
      Uri requestUri = message.RequestUri;
      IPAddress[] source = this.VerifyUrlIsAllowedIPAddressInternal(requestContext, requestUri.AbsoluteUri, true);
      if (!requestContext.IsFeatureEnabled("ServiceHooks.Http.UseApplyIPAddressAllowedRangeOnHttpRequest") || source == null || source.Length == 0)
        return;
      IPAddress ipAddress = (IPAddress) null;
      if (requestContext.IsFeatureEnabled("ServiceHooks.Http.IPAddressOnHttpRequestPreferIPv4"))
        ipAddress = ((IEnumerable<IPAddress>) source).FirstOrDefault<IPAddress>((Func<IPAddress, bool>) (ip => ip.AddressFamily == AddressFamily.InterNetwork));
      if (ipAddress == null)
        ipAddress = source[0];
      message.Headers.Host = requestUri.Host;
      Uri uri = new UriBuilder(requestUri)
      {
        Host = ipAddress.ToString()
      }.Uri;
      message.RequestUri = uri;
    }
  }
}
