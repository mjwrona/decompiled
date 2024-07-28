// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.Tracing.ElasticPoolTracingInterceptor
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.TeamFoundation.DistributedTask.AzureSdk.Tracing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic.Tracing
{
  internal class ElasticPoolTracingInterceptor : IAzureClientTracingInterceptor
  {
    private ConcurrentDictionary<string, ElasticPoolTracingInterceptor.RequestTracer> m_requestsInfo = new ConcurrentDictionary<string, ElasticPoolTracingInterceptor.RequestTracer>();
    private const int s_subscriptionReadLimitThreshold = 1200;
    private const int s_subscriptionWriteLimitThreshold = 120;
    private const string s_httpClient = "AzureHttpClientBase";
    private const string s_layer = "ElasticPoolTracingInterceptor";

    public void Configuration(Guid activityId, string source, string name, string value)
    {
      try
      {
        TeamFoundationTracingService.TraceRaw(10015272, TraceLevel.Info, "DistributedTask", nameof (ElasticPoolTracingInterceptor), activityId, Guid.Empty, "Source: " + source + ", Name: " + name + ", Value: " + value);
      }
      catch (Exception ex)
      {
        this.TraceError(activityId, (string) null, ex);
      }
    }

    public void Information(Guid activityId, string invocationId, string message)
    {
      try
      {
        TeamFoundationTracingService.TraceRaw(10015272, TraceLevel.Info, "DistributedTask", nameof (ElasticPoolTracingInterceptor), activityId, Guid.Empty, message);
      }
      catch (Exception ex)
      {
        this.TraceError(activityId, (string) null, ex);
      }
    }

    public void SendRequest(Guid activityId, string invocationId, HttpRequestMessage request)
    {
      try
      {
        if (string.IsNullOrEmpty(invocationId))
          return;
        this.m_requestsInfo.TryAdd(invocationId, new ElasticPoolTracingInterceptor.RequestTracer());
      }
      catch (Exception ex)
      {
        this.TraceError(activityId, (string) null, ex);
      }
    }

    public void ReceiveResponse(Guid activityId, string invocationId, HttpResponseMessage response)
    {
      try
      {
        ElasticPoolTracingInterceptor.RequestTracer requestTracer;
        if (!string.IsNullOrEmpty(invocationId) && this.m_requestsInfo.TryRemove(invocationId, out requestTracer))
        {
          requestTracer.timer.Stop();
          TeamFoundationTracingService.TraceHttpOutgoingRequest(requestTracer.StartTime, (int) requestTracer.timer.ElapsedMilliseconds, "AzureHttpClientBase", response.RequestMessage.Method.Method, response.RequestMessage.RequestUri.Host, response.RequestMessage.RequestUri.AbsolutePath, (int) response.StatusCode, (string) null, activityId, string.Empty, string.Empty, activityId, string.Empty, string.Empty);
        }
        if (response?.Headers == null)
          return;
        IEnumerable<string> values1 = (IEnumerable<string>) null;
        IEnumerable<string> values2 = (IEnumerable<string>) null;
        IEnumerable<string> values3 = (IEnumerable<string>) null;
        IEnumerable<string> values4 = (IEnumerable<string>) null;
        response.Headers.TryGetValues("x-ms-ratelimit-remaining-resource", out values3);
        response.Headers.TryGetValues("x-ms-ratelimit-remaining-subscription-reads", out values1);
        response.Headers.TryGetValues("x-ms-ratelimit-remaining-subscription-writes", out values2);
        response.Headers.TryGetValues("x-ms-request-id", out values4);
        TeamFoundationTracingService.TraceRaw(true, 10015275, TraceLevel.Info, "DistributedTask", nameof (ElasticPoolTracingInterceptor), activityId, Guid.Empty, "Operation Id: " + (values4 != null ? values4.FirstOrDefault<string>() : (string) null) + "\nRemaining resource limit: " + (values3 != null ? values3.FirstOrDefault<string>() : (string) null) + "\nSubscription reads: " + (values1 != null ? values1.FirstOrDefault<string>() : (string) null) + "\nSubscription writes: " + (values2 != null ? values2.FirstOrDefault<string>() : (string) null) + "\nHttpMethod: " + response?.RequestMessage?.Method?.Method + "\nUrlPath: " + response?.RequestMessage?.RequestUri?.AbsolutePath);
      }
      catch (Exception ex)
      {
        this.TraceError(activityId, (string) null, ex);
      }
    }

    public void TraceError(Guid activityId, string invocationId, Exception exception)
    {
      try
      {
        TeamFoundationTracingService.TraceRaw(10015274, TraceLevel.Error, "DistributedTask", nameof (ElasticPoolTracingInterceptor), activityId, (string) null, Guid.Empty, "{0}", (object) new Lazy<string>((Func<string>) (() => exception.ToReadableStackTrace())));
      }
      catch (Exception ex)
      {
      }
    }

    public void EnterMethod(
      Guid activityId,
      string invocationId,
      object instance,
      string method,
      IDictionary<string, object> parameters)
    {
      try
      {
        TeamFoundationTracingService.TraceRaw(10015273, TraceLevel.Verbose, "DistributedTask", nameof (ElasticPoolTracingInterceptor), activityId, (string) null, Guid.Empty, "Entering " + method);
      }
      catch (Exception ex)
      {
        this.TraceError(activityId, (string) null, ex);
      }
    }

    public void ExitMethod(
      Guid activityId,
      string invocationId,
      object instance,
      string method,
      object returnValue)
    {
      try
      {
        TeamFoundationTracingService.TraceRaw(10015273, TraceLevel.Verbose, "DistributedTask", nameof (ElasticPoolTracingInterceptor), activityId, (string) null, Guid.Empty, "Leaving " + method);
      }
      catch (Exception ex)
      {
        this.TraceError(activityId, (string) null, ex);
      }
    }

    private bool ShouldTraceAlways(int statusCode, string readLimit, string writeLimit)
    {
      int result1;
      int result2;
      return statusCode >= 300 || int.TryParse(readLimit, out result1) && result1 <= 1200 || int.TryParse(writeLimit, out result2) && result2 <= 120;
    }

    internal class RequestTracer
    {
      public DateTime StartTime { get; } = DateTime.Now;

      public Stopwatch timer { get; } = Stopwatch.StartNew();
    }
  }
}
