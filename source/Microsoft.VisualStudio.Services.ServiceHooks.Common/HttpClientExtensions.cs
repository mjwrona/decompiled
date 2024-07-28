// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.HttpClientExtensions
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public static class HttpClientExtensions
  {
    public static HttpResponseMessage PauseTimerAndGetResult(
      this HttpClient client,
      IVssRequestContext requestContext,
      string requestUri,
      bool validateIpAddress = true)
    {
      HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
      return HttpClientExtensions.PauseTimerAndGetHttpResponse(requestContext, requestMessage, (Func<HttpRequestMessage, HttpResponseMessage>) (req => client.SendAsync(req).Result), validateIpAddress);
    }

    public static HttpResponseMessage PauseTimerAndSendResult(
      this HttpClient client,
      IVssRequestContext requestContext,
      HttpRequestMessage request,
      bool validateIpAddress = true)
    {
      return HttpClientExtensions.PauseTimerAndGetHttpResponse(requestContext, request, (Func<HttpRequestMessage, HttpResponseMessage>) (req => client.SendAsync(req).Result), validateIpAddress);
    }

    public static HttpResponseMessage PauseTimerAndSendResult(
      this HttpClient client,
      IVssRequestContext requestContext,
      HttpRequestMessage request,
      HttpCompletionOption completionOption,
      bool validateIpAddress = true)
    {
      return HttpClientExtensions.PauseTimerAndGetHttpResponse(requestContext, request, (Func<HttpRequestMessage, HttpResponseMessage>) (req => client.SendAsync(req, completionOption).Result), validateIpAddress);
    }

    private static HttpResponseMessage PauseTimerAndGetHttpResponse(
      IVssRequestContext requestContext,
      HttpRequestMessage requestMessage,
      Func<HttpRequestMessage, HttpResponseMessage> clientAction,
      bool validateIpAddress)
    {
      if (validateIpAddress)
        requestContext.GetService<IUrlAddressIpValidatorService>().ApplyIPAddressAllowedRangeOnHttpRequest(requestContext, requestMessage);
      try
      {
        requestContext.RequestTimer.PauseTimeToFirstPageTimer();
        return clientAction(requestMessage);
      }
      finally
      {
        requestContext.RequestTimer.ResumeTimeToFirstPageTimer();
      }
    }
  }
}
