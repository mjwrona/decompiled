// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ClientRateLimiterHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ClientRateLimiterHandler : DelegatingHandler
  {
    private volatile bool isDisposed;
    private readonly string clientName;
    private const string CachedRateLimitType = "RateLimitType";
    private const int MinConcurrentSleepSeconds = 2;
    private const int MaxConcurrentSleepSeconds = 10;

    public ClientRateLimiterHandler(Type requestedType) => this.clientName = requestedType.Name;

    public ClientRateLimiterHandler(Type requestedType, HttpMessageHandler innerHandler)
      : this(requestedType)
    {
      this.InnerHandler = innerHandler;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && !this.isDisposed)
        this.isDisposed = true;
      base.Dispose(disposing);
    }

    private void CheckForDisposed()
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(this.GetType().Name);
    }

    protected override async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      this.CheckForDisposed();
      bool continueOnCapturedContext;
      request.Properties.TryGetValue<bool>(FrameworkServerConstants.HandlerConfigureAwaitFeatureFlag, out continueOnCapturedContext);
      bool flag;
      request.Properties.TryGetValue<bool>(FrameworkServerConstants.HandlerHttpClientRateLimiterFeatureFlag, out flag);
      IVssRequestContext deploymentContext;
      if (!flag || !this.TryCreateDeploymentContext(request, out deploymentContext))
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(continueOnCapturedContext);
      deploymentContext.TraceEnter(34003800, "DelegatingHandler", nameof (ClientRateLimiterHandler), nameof (SendAsync));
      IRateLimiterCacheService rateLimiterService = deploymentContext.GetService<IRateLimiterCacheService>();
      this.EnsureHttpClientIsNotThrottled(deploymentContext, rateLimiterService, cancellationToken);
      HttpResponseMessage response = await base.SendAsync(request, cancellationToken).ConfigureAwait(continueOnCapturedContext);
      DateTime? retryAfterDate;
      Dictionary<string, string> headersFromResponse = this.GetRateLimitHeadersFromResponse(deploymentContext, response, out retryAfterDate);
      if (headersFromResponse.Any<KeyValuePair<string, string>>())
        deploymentContext.TraceAlways(34003803, TraceLevel.Info, "DelegatingHandler", nameof (ClientRateLimiterHandler), "New throttling headers were parsed from response. Client=" + this.clientName + ", ThrottlingHeaders=" + headersFromResponse.Serialize<Dictionary<string, string>>());
      rateLimiterService.UpdateRateLimiterHeaders(deploymentContext, deploymentContext.GetUserId(), this.clientName, (IDictionary<string, string>) headersFromResponse);
      if (retryAfterDate.HasValue)
      {
        deploymentContext.TraceAlways(34003802, TraceLevel.Info, "DelegatingHandler", nameof (ClientRateLimiterHandler), "Leaving ClientRateLimiterHandler. Client=" + this.clientName + ", RetryAfter=" + retryAfterDate.Value.ToString("o"));
        throw new ClientRequestThrottledException(FrameworkResources.ClientRequestThrottledRateLimitReachedException(), retryAfterDate.Value);
      }
      deploymentContext.TraceLeave(34003801, "DelegatingHandler", nameof (ClientRateLimiterHandler), nameof (SendAsync));
      return response;
    }

    public virtual bool TryCreateDeploymentContext(
      HttpRequestMessage request,
      out IVssRequestContext deploymentContext)
    {
      if (TeamFoundationApplicationCore.DeploymentInitialized)
      {
        deploymentContext = TeamFoundationApplicationCore.DeploymentServiceHost.CreateSystemContext(false);
        return true;
      }
      object obj;
      if (request.Properties.TryGetValue(TfsApiPropertyKeys.TfsRequestContextClient, out obj) && obj is IVssRequestContext vssRequestContext)
      {
        deploymentContext = vssRequestContext.ServiceHost.DeploymentServiceHost.CreateSystemContext(false);
        return true;
      }
      deploymentContext = (IVssRequestContext) null;
      return false;
    }

    protected string GetHeaderValue(HttpResponseMessage response, string headerName)
    {
      if (response == null)
        return string.Empty;
      IEnumerable<string> values;
      if (!response.Headers.TryGetValues(headerName, out values) && response.Content != null)
        response.Content.Headers.TryGetValues(headerName, out values);
      return values == null ? (string) null : values.FirstOrDefault<string>();
    }

    private bool TryGetValue(Dictionary<string, string> dictionary, string key, out DateTime value)
    {
      value = DateTime.MinValue;
      string s;
      if (!dictionary.TryGetValue(key, out s) || !DateTime.TryParse(s, out value))
        return false;
      value = value.ToUniversalTime();
      return true;
    }

    private void EnsureHttpClientIsNotThrottled(
      IVssRequestContext deploymentContext,
      IRateLimiterCacheService service,
      CancellationToken cancellationToken)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      IDictionary<string, string> rateLimiterHeaders;
      if (service.TryGetRateLimiterHeaders(deploymentContext, deploymentContext.GetUserId(), this.clientName, out rateLimiterHeaders))
        dictionary = new Dictionary<string, string>(rateLimiterHeaders);
      string str;
      if (!dictionary.TryGetValue("X-RateLimit-Resource", out str))
        return;
      string s;
      dictionary.TryGetValue("RateLimitType", out s);
      RateLimitType result;
      if (!EnumUtility.TryParse<RateLimitType>(s, true, out result))
        return;
      if (result == RateLimitType.Concurrent)
      {
        int num = new Random(Environment.CurrentManagedThreadId).Next(2, 10);
        DateTime retryAfterDateTime = DateTime.UtcNow.AddSeconds((double) num);
        deploymentContext.TraceAlways(34003804, TraceLevel.Info, "DelegatingHandler", nameof (ClientRateLimiterHandler), string.Format("Sleeping {0} seconds to avoid concurrent throttling {1}. Resource={2}, Client={3}, RetryAfter={4}", (object) num, (object) nameof (ClientRateLimiterHandler), (object) str, (object) this.clientName, (object) retryAfterDateTime.ToString("o")));
        throw new ClientRequestThrottledException(FrameworkResources.ClientRequestThrottledRateLimitReachedException(), retryAfterDateTime);
      }
      DateTime retryAfterDateTime1;
      if (this.TryGetValue(dictionary, "Retry-After", out retryAfterDateTime1))
      {
        if (DateTime.UtcNow < retryAfterDateTime1)
        {
          deploymentContext.TraceAlways(34003806, TraceLevel.Info, "DelegatingHandler", nameof (ClientRateLimiterHandler), "ClientRateLimiterHandler throttling due to " + str + " header. Client=" + this.clientName);
          throw new ClientRequestThrottledException(FrameworkResources.ClientRequestThrottledRateLimitReachedException(), retryAfterDateTime1);
        }
      }
      else
      {
        int num = result.ThrottlingWindowInSeconds();
        if (result == RateLimitType.Long)
          num /= 2;
        DateTime retryAfterDateTime2 = DateTime.UtcNow.AddSeconds((double) num);
        DateTime dateTime;
        if (this.TryGetValue(dictionary, "X-RateLimit-Reset", out dateTime) && dateTime < retryAfterDateTime2)
          retryAfterDateTime2 = dateTime;
        if (DateTime.UtcNow < retryAfterDateTime2)
        {
          deploymentContext.TraceAlways(34003805, TraceLevel.Info, "DelegatingHandler", nameof (ClientRateLimiterHandler), "RetryAfter could not be retrieved from cache. Resource=" + str + ", Client=" + this.clientName + ", Reset Rate Limit Date=" + retryAfterDateTime2.ToString("o"));
          throw new ClientRequestThrottledException(FrameworkResources.ClientRequestThrottledRateLimitReachedException(), retryAfterDateTime2);
        }
      }
    }

    private Dictionary<string, string> GetRateLimitHeadersFromResponse(
      IVssRequestContext deploymentContext,
      HttpResponseMessage response,
      out DateTime? retryAfterDate)
    {
      retryAfterDate = new DateTime?();
      Dictionary<string, string> headersFromResponse = new Dictionary<string, string>();
      string headerValue1 = this.GetHeaderValue(response, "X-RateLimit-Resource");
      if (!string.IsNullOrWhiteSpace(headerValue1))
      {
        string[] strArray = headerValue1.Split('/');
        if (strArray.Length >= 2)
        {
          RateLimitType result1;
          EnumUtility.TryParse<RateLimitType>(strArray[1], true, out result1);
          string headerValue2 = this.GetHeaderValue(response, "X-RateLimit-Remaining");
          long result2;
          DateTime dateTime;
          if (long.TryParse(this.GetHeaderValue(response, "Retry-After"), out result2))
          {
            ref DateTime? local = ref retryAfterDate;
            dateTime = DateTime.UtcNow;
            DateTime? nullable = new DateTime?(dateTime.AddSeconds((double) result2));
            local = nullable;
            Dictionary<string, string> dictionary = headersFromResponse;
            dateTime = retryAfterDate.Value;
            string str = dateTime.ToString("o");
            dictionary["Retry-After"] = str;
          }
          long result3;
          if (long.TryParse(this.GetHeaderValue(response, "X-RateLimit-Reset"), out result3))
          {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(result3);
            Dictionary<string, string> dictionary = headersFromResponse;
            dateTime = dateTimeOffset.UtcDateTime;
            string str = dateTime.ToString("o");
            dictionary["X-RateLimit-Reset"] = str;
          }
          headersFromResponse["X-RateLimit-Resource"] = headerValue1;
          headersFromResponse["X-RateLimit-Remaining"] = headerValue2;
          headersFromResponse["RateLimitType"] = result1.ToString("D");
        }
      }
      return headersFromResponse;
    }
  }
}
