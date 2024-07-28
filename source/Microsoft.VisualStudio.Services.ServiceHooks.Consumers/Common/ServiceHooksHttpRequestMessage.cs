// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.ServiceHooksHttpRequestMessage
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common
{
  public class ServiceHooksHttpRequestMessage : HttpRequestMessage
  {
    private const string c_basicAuthScheme = "Basic";
    private const string c_basicAuthTokenFormat = "{0}:{1}";
    private const string c_hooksActivityIdHeaderName = "X-VSS-ActivityId";
    private const string c_hooksSubscriptionIdHeaderName = "X-VSS-SubscriptionId";

    public ServiceHooksHttpRequestMessage(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification,
      HttpMethod method,
      string url)
      : base(method, url)
    {
      this.Headers.Add("X-VSS-ActivityId", requestContext.ActivityId.ToString());
      this.Headers.Add("X-VSS-SubscriptionId", notification.SubscriptionId.ToString());
      ServiceHooksHttpRequestMessage.AddUserAgentHeaders(this.Headers);
    }

    public ServiceHooksHttpRequestMessage(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification,
      HttpMethod method,
      string url,
      string username,
      string password)
      : this(requestContext, notification, method, url)
    {
      this.Headers.Authorization = ServiceHooksHttpRequestMessage.CreateBasicAuthenticationHeader(username, password);
    }

    public static void AddUserAgentHeaders(HttpRequestHeaders headers)
    {
      if (headers == null)
        return;
      List<ProductInfoHeaderValue> defaultRestUserAgent = UserAgentUtility.GetDefaultRestUserAgent();
      if (!defaultRestUserAgent.Any<ProductInfoHeaderValue>())
        return;
      foreach (ProductInfoHeaderValue productInfoHeaderValue in defaultRestUserAgent)
      {
        if (!headers.UserAgent.Contains(productInfoHeaderValue))
          headers.UserAgent.Add(productInfoHeaderValue);
      }
    }

    public static AuthenticationHeaderValue CreateBasicAuthenticationHeader(
      string username,
      string password)
    {
      return new AuthenticationHeaderValue("Basic", ServiceHooksHttpRequestMessage.BuildBasicAuthenticationHeaderTokenValue(username, password));
    }

    public static string BuildBasicAuthenticationHeaderTokenValue(string username, string password) => Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", (object) (username ?? ""), (object) (password ?? ""))));
  }
}
