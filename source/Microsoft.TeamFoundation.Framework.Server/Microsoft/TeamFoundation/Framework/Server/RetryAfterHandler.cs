// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RetryAfterHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class RetryAfterHandler : DelegatingHandler
  {
    private const string c_throttleFeatureName = "VisualStudio.FrameworkService.TooManyRequestsHandler.Throttle";
    private const string c_propagateFeatureName = "VisualStudio.FrameworkService.TooManyRequestsHandler.Propagate";
    private const string c_area = "DelegatingHandler";
    private const string c_layer = "RetryAfterHandler";
    private string m_keyRoot;

    public RetryAfterHandler(IVssRequestContext requestContext, string clientName, string hostName) => this.m_keyRoot = clientName + "/" + hostName + "/";

    protected override async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      IVssRequestContext requestContext = (IVssRequestContext) null;
      string key = (string) null;
      ThrottlingCacheService throttlingCacheService = (ThrottlingCacheService) null;
      bool propagateRetryAfter = false;
      bool continueOnCapturedContext = false;
      request.Properties.TryGetValue<bool>(FrameworkServerConstants.HandlerConfigureAwaitFeatureFlag, out continueOnCapturedContext);
      object obj;
      if (request.Properties.TryGetValue(TfsApiPropertyKeys.TfsRequestContextClient, out obj))
      {
        requestContext = obj as IVssRequestContext;
        if (requestContext != null && this.TryCreateCacheKey(requestContext, out key))
        {
          throttlingCacheService = requestContext.GetService<ThrottlingCacheService>();
          propagateRetryAfter = requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.TooManyRequestsHandler.Propagate");
          DateTime dateTime;
          if (throttlingCacheService.TryPeekValue(requestContext, key, out dateTime))
          {
            if (requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.TooManyRequestsHandler.Throttle"))
            {
              if (propagateRetryAfter)
                this.PropagateRetryAfter(requestContext, dateTime);
              requestContext.TraceAlways(639664893, TraceLevel.Verbose, "DelegatingHandler", nameof (RetryAfterHandler), "The Request has been Cancelled and ClientRequestThrottledException was thrown because the Retry-After header was not respected for the key {0} with Retry-After {1}", (object) key, (object) dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
              throw new ClientRequestThrottledException(FrameworkResources.ClientRequestThrottledException(), dateTime);
            }
            requestContext.Trace(639664893, TraceLevel.Verbose, "DelegatingHandler", nameof (RetryAfterHandler), "{0} found in the cache with Retry-After {1}", (object) key, (object) dateTime);
          }
        }
      }
      HttpResponseMessage httpResponseMessage1 = await base.SendAsync(request, cancellationToken).ConfigureAwait(continueOnCapturedContext);
      DateTime retryAfter = DateTime.MinValue;
      if (httpResponseMessage1.Headers?.RetryAfter != null && key != null && this.TryParseRetryAfter(httpResponseMessage1, out retryAfter))
      {
        requestContext.Trace(639664891, TraceLevel.Verbose, "DelegatingHandler", nameof (RetryAfterHandler), "Adding {0} to the cache with retry-value {1} Retry-After-date {2}", (object) key, (object) httpResponseMessage1.Headers.RetryAfter, (object) retryAfter);
        throttlingCacheService.Set((IVssRequestContext) null, key, retryAfter);
        if (propagateRetryAfter)
          this.PropagateRetryAfter(requestContext, retryAfter);
      }
      HttpResponseMessage httpResponseMessage2 = httpResponseMessage1.StatusCode != (HttpStatusCode) 429 ? httpResponseMessage1 : throw new ClientRequestThrottledException(this.GetExceptionMessage(httpResponseMessage1), retryAfter);
      requestContext = (IVssRequestContext) null;
      key = (string) null;
      throttlingCacheService = (ThrottlingCacheService) null;
      return httpResponseMessage2;
    }

    private bool TryCreateCacheKey(IVssRequestContext requestContext, out string key)
    {
      Guid userId = requestContext.GetUserId();
      if (userId == Guid.Empty)
      {
        key = (string) null;
        return false;
      }
      string str = string.Empty;
      if (requestContext.Method != null)
        str = requestContext.Method.Name + "/";
      key = this.m_keyRoot + str + userId.ToString("D");
      return true;
    }

    private void PropagateRetryAfter(IVssRequestContext requestContext, DateTime retryAfter)
    {
      if (!(requestContext is IWebRequestContextInternal requestContextInternal) || requestContextInternal.HttpContext == null || requestContextInternal.HttpContext.Response.HeadersWritten)
        return;
      double num = Math.Ceiling(retryAfter.Subtract(DateTime.UtcNow).TotalSeconds);
      requestContextInternal.HttpContext.Response.AddHeader("Retry-After", num.ToString());
    }

    private bool TryParseRetryAfter(
      HttpResponseMessage httpResponseMessage,
      out DateTime retryAfter)
    {
      retryAfter = DateTime.MinValue;
      IEnumerable<string> values;
      if (!httpResponseMessage.Headers.TryGetValues("Retry-After", out values))
        return false;
      foreach (string s in values)
      {
        DateTime result1;
        int result2;
        if (!DateTime.TryParse(s, out result1) && int.TryParse(s, out result2))
          result1 = DateTime.UtcNow.AddSeconds((double) result2);
        if (result1 > retryAfter)
          retryAfter = result1;
      }
      return retryAfter > DateTime.MinValue;
    }

    private string GetExceptionMessage(HttpResponseMessage httpResponseMessage)
    {
      string exceptionMessage = (string) null;
      IEnumerable<string> values;
      if (httpResponseMessage.Headers.TryGetValues("X-TFS-ServiceError", out values))
        UriUtility.UrlDecode(values.FirstOrDefault<string>());
      else if (string.IsNullOrEmpty(exceptionMessage) && !string.IsNullOrEmpty(httpResponseMessage.ReasonPhrase))
        exceptionMessage = httpResponseMessage.ReasonPhrase;
      return exceptionMessage;
    }
  }
}
