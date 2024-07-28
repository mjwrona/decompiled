// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ClientTraceHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ClientTraceHandler : DelegatingHandler
  {
    private Type RequestedType;
    public readonly string TraceArea;
    public readonly string TraceLayer;
    public readonly uint SlowRequestThreshold;
    public readonly byte TracePercentage;
    private readonly HashSet<string> m_sensitiveHeaders;
    private static int s_requestCount;
    private static readonly HashSet<string> s_noSensitiveHeaders = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    internal ClientTraceHandler(
      Type requestedType,
      string traceArea,
      string traceLayer,
      TimeSpan slowRequestThreshold,
      byte tracePercentage,
      HashSet<string> sensitiveHeaders)
    {
      this.RequestedType = requestedType;
      this.TraceArea = traceArea;
      this.TraceLayer = traceLayer;
      this.SlowRequestThreshold = (uint) slowRequestThreshold.TotalMilliseconds;
      this.TracePercentage = tracePercentage;
      this.m_sensitiveHeaders = sensitiveHeaders ?? ClientTraceHandler.s_noSensitiveHeaders;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage message,
      CancellationToken token)
    {
      requestContext = (IVssRequestContext) null;
      VssHttpMessageHandlerTraceInfo.SetTraceInfo(message, new VssHttpMessageHandlerTraceInfo());
      object obj = (object) null;
      if (message.Properties.TryGetValue(TfsApiPropertyKeys.TfsRequestContextClient, out obj) && obj is IVssRequestContext requestContext)
      {
        HttpRequestHeaders headers1 = message.Headers;
        Guid guid1 = requestContext.ActivityId;
        string str1 = guid1.ToString("D");
        headers1.Add("X-TFS-Session", str1);
        message.Headers.Remove("X-VSS-E2EID");
        HttpRequestHeaders headers2 = message.Headers;
        guid1 = requestContext.E2EId;
        string str2 = guid1.ToString("D");
        headers2.Add("X-VSS-E2EID", str2);
        if (!string.IsNullOrEmpty(requestContext.OrchestrationId))
        {
          message.Headers.Remove("X-VSS-OrchestrationId");
          message.Headers.Add("X-VSS-OrchestrationId", requestContext.OrchestrationId);
        }
        Guid guid2;
        if (requestContext.RootContext.Items.TryGetGuid(RequestContextItemsKeys.AuditLogCorrelationId, out guid2))
        {
          message.Headers.Remove("X-VSS-Audit-CorrelationId");
          message.Headers.Add("X-VSS-Audit-CorrelationId", guid2.ToString("D"));
        }
      }
      DateTime startTime = DateTime.UtcNow;
      bool traceRequest = (uint) Interlocked.Increment(ref ClientTraceHandler.s_requestCount) % 100U < (uint) this.TracePercentage;
      bool continueOnCapturedContext = false;
      string requestPhase = this.GetCurrentRequestPhase(requestContext);
      message.Properties.TryGetValue<bool>(FrameworkServerConstants.HandlerConfigureAwaitFeatureFlag, out continueOnCapturedContext);
      HttpResponseMessage httpResponseMessage1 = (HttpResponseMessage) null;
      try
      {
        if (requestContext != null)
        {
          this.PerfCounterBeginRequest();
          requestContext.RequestTimer.RequestTimerInternal().RecordTraceEnterTiming(this.TraceArea, this.TraceLayer, string.Format("RestCallStart {0} {1}", (object) message.Method.Method, (object) message.RequestUri.AbsoluteUri));
          PerformanceTimer perfTimer = PerformanceTimer.StartMeasure(requestContext, "VssClient");
          try
          {
            perfTimer.AddProperty("Layer", (object) this.TraceLayer);
            perfTimer.AddProperty("Method", (object) message.Method.Method);
            perfTimer.AddProperty("Uri", (object) message.RequestUri.AbsoluteUri);
            httpResponseMessage1 = await base.SendAsync(message, token).ConfigureAwait(continueOnCapturedContext);
            perfTimer.AddProperty("Status", (object) (int) httpResponseMessage1.StatusCode);
          }
          finally
          {
            perfTimer.Dispose();
          }
          perfTimer = new PerformanceTimer();
          requestContext.RequestTimer.RequestTimerInternal().RecordTraceLeaveTiming(this.TraceArea, this.TraceLayer, "RestCallEnd");
        }
        else
          httpResponseMessage1 = await base.SendAsync(message, token).ConfigureAwait(continueOnCapturedContext);
      }
      catch (Exception ex)
      {
        int totalMilliseconds1 = (int) (DateTime.UtcNow - startTime).TotalMilliseconds;
        VssHttpMessageHandlerTraceInfo handlerTraceInfo = VssHttpMessageHandlerTraceInfo.GetTraceInfo(message) ?? new VssHttpMessageHandlerTraceInfo();
        DateTime startTime1 = startTime;
        int timeTaken = totalMilliseconds1;
        string name = this.RequestedType.Name;
        string method = message.Method.Method;
        string host = message.RequestUri.Host;
        string absolutePath = message.RequestUri.AbsolutePath;
        string readableStackTrace = ex.ToReadableStackTrace();
        Guid e2Eid = requestContext.E2EId;
        string requestPriority = requestContext.RequestPriority.GetString();
        Guid empty = Guid.Empty;
        string requestPhase1 = requestPhase;
        string orchestrationId = requestContext.OrchestrationId;
        int tokenRetries = handlerTraceInfo.TokenRetries;
        int totalMilliseconds2 = (int) handlerTraceInfo.HandlerStartTime.TotalMilliseconds;
        int totalMilliseconds3 = (int) handlerTraceInfo.BufferedRequestTime.TotalMilliseconds;
        TimeSpan timeSpan = handlerTraceInfo.RequestSendTime;
        int totalMilliseconds4 = (int) timeSpan.TotalMilliseconds;
        timeSpan = handlerTraceInfo.ResponseContentTime;
        int totalMilliseconds5 = (int) timeSpan.TotalMilliseconds;
        int totalMilliseconds6 = (int) handlerTraceInfo.GetTokenTime.TotalMilliseconds;
        int totalMilliseconds7 = (int) handlerTraceInfo.TrailingTime.TotalMilliseconds;
        TeamFoundationTracingService.TraceHttpOutgoingRequest(startTime1, timeTaken, name, method, host, absolutePath, -1, readableStackTrace, e2Eid, "", requestPriority, empty, requestPhase1, orchestrationId, tokenRetries, totalMilliseconds2, totalMilliseconds3, totalMilliseconds4, totalMilliseconds5, totalMilliseconds6, totalMilliseconds7);
        throw;
      }
      TimeSpan timeSpan1 = DateTime.UtcNow - startTime;
      uint totalMilliseconds8 = (uint) timeSpan1.TotalMilliseconds;
      this.RecordTiming(totalMilliseconds8, message);
      this.PerfCounterEndRequest(totalMilliseconds8);
      string[] tags = new string[1]
      {
        ((int) httpResponseMessage1.StatusCode).ToString()
      };
      if (httpResponseMessage1.StatusCode == HttpStatusCode.Found || TeamFoundationTracingService.IsRawTracingEnabled(1001001, TraceLevel.Info, this.TraceArea, this.TraceLayer, tags))
      {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Request:");
        sb.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) message.Method.Method, (object) message.RequestUri.AbsoluteUri));
        this.TraceHeaders(sb, (HttpHeaders) message.Headers);
        if (message.Content != null)
          this.TraceHeaders(sb, (HttpHeaders) message.Content.Headers);
        sb.AppendLine("Response:");
        sb.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "HTTP/{0} {1} {2}", (object) httpResponseMessage1.Version, (object) (int) httpResponseMessage1.StatusCode, (object) httpResponseMessage1.ReasonPhrase));
        this.TraceHeaders(sb, (HttpHeaders) httpResponseMessage1.Headers);
        if (httpResponseMessage1.Content != null)
          this.TraceHeaders(sb, (HttpHeaders) httpResponseMessage1.Content.Headers);
        string message1 = sb.ToString();
        if (httpResponseMessage1.StatusCode == HttpStatusCode.Found)
          TeamFoundationTracingService.TraceRaw(1001001, TraceLevel.Error, this.TraceArea, this.TraceLayer, (string[]) null, message1);
        else
          TeamFoundationTracingService.TraceRaw(1001001, TraceLevel.Info, this.TraceArea, this.TraceLayer, tags, message1);
      }
      if (traceRequest || totalMilliseconds8 > this.SlowRequestThreshold)
      {
        IEnumerable<string> values1;
        httpResponseMessage1.Headers.TryGetValues("X-MSEdge-Ref", out values1);
        string str = (values1 != null ? values1.FirstOrDefault<string>() : (string) null) ?? string.Empty;
        IEnumerable<string> values2;
        httpResponseMessage1.Headers.TryGetValues("ActivityId", out values2);
        Guid result;
        if (!Guid.TryParse((values2 != null ? values2.FirstOrDefault<string>() : (string) null) ?? string.Empty, out result))
          result = Guid.Empty;
        VssHttpMessageHandlerTraceInfo handlerTraceInfo = VssHttpMessageHandlerTraceInfo.GetTraceInfo(message) ?? new VssHttpMessageHandlerTraceInfo();
        DateTime startTime2 = startTime;
        int timeTaken = (int) totalMilliseconds8;
        string name = this.RequestedType.Name;
        string method = message.Method.Method;
        string host = message.RequestUri.Host;
        string absolutePath = message.RequestUri.AbsolutePath;
        int statusCode = (int) httpResponseMessage1.StatusCode;
        Guid e2Eid = requestContext.E2EId;
        string afdRefInfo = str;
        string requestPriority = requestContext.RequestPriority.GetString();
        Guid calledActivityId = result;
        string requestPhase2 = requestPhase;
        string orchestrationId = requestContext.OrchestrationId;
        int tokenRetries = handlerTraceInfo.TokenRetries;
        timeSpan1 = handlerTraceInfo.HandlerStartTime;
        int totalMilliseconds9 = (int) timeSpan1.TotalMilliseconds;
        timeSpan1 = handlerTraceInfo.BufferedRequestTime;
        int totalMilliseconds10 = (int) timeSpan1.TotalMilliseconds;
        timeSpan1 = handlerTraceInfo.RequestSendTime;
        int totalMilliseconds11 = (int) timeSpan1.TotalMilliseconds;
        timeSpan1 = handlerTraceInfo.ResponseContentTime;
        int totalMilliseconds12 = (int) timeSpan1.TotalMilliseconds;
        timeSpan1 = handlerTraceInfo.GetTokenTime;
        int totalMilliseconds13 = (int) timeSpan1.TotalMilliseconds;
        timeSpan1 = handlerTraceInfo.TrailingTime;
        int totalMilliseconds14 = (int) timeSpan1.TotalMilliseconds;
        TeamFoundationTracingService.TraceHttpOutgoingRequest(startTime2, timeTaken, name, method, host, absolutePath, statusCode, "", e2Eid, afdRefInfo, requestPriority, calledActivityId, requestPhase2, orchestrationId, tokenRetries, totalMilliseconds9, totalMilliseconds10, totalMilliseconds11, totalMilliseconds12, totalMilliseconds13, totalMilliseconds14);
      }
      HttpResponseMessage httpResponseMessage2 = httpResponseMessage1;
      requestContext = (IVssRequestContext) null;
      requestPhase = (string) null;
      return httpResponseMessage2;
    }

    private string GetCurrentRequestPhase(IVssRequestContext requestContext)
    {
      if (!requestContext.IsHostProcessType(HostProcessType.ApplicationTier))
        return "NonAT";
      if (requestContext.RequestTimer.PreControllerTime == 0L)
        return "PreController";
      return requestContext.RequestTimer.ControllerTime == 0L ? "Controller" : "PostController";
    }

    internal void TraceHeaders(StringBuilder sb, HttpHeaders headers)
    {
      if (headers == null)
        return;
      HttpRequestHeaders httpRequestHeaders = headers as HttpRequestHeaders;
      foreach (KeyValuePair<string, IEnumerable<string>> header in headers)
      {
        if (httpRequestHeaders != null && header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(httpRequestHeaders.Authorization.Parameter))
          sb.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}: {1} <secret removed>", (object) header.Key, (object) httpRequestHeaders.Authorization.Scheme));
        else if (this.m_sensitiveHeaders.Contains(header.Key))
        {
          sb.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}: <sensitive header removed>", (object) header.Key));
        }
        else
        {
          string str = string.Join(",", header.Value);
          sb.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}: {1}", (object) header.Key, (object) str));
        }
      }
    }

    private void RecordTiming(uint elapsedTimeInMilliseconds, HttpRequestMessage request)
    {
      if (elapsedTimeInMilliseconds <= this.SlowRequestThreshold)
        return;
      TeamFoundationTracingService.TraceRaw(100005, TraceLevel.Error, this.TraceArea, this.TraceLayer, "SlowRequest: Url: {0} ElapsedTime: {1}ms", (object) request.RequestUri.OriginalString, (object) elapsedTimeInMilliseconds);
    }

    private void PerfCounterBeginRequest()
    {
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CallsExecuting", this.RequestedType.Name).Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CallsPerSec", this.RequestedType.Name).Increment();
    }

    private void PerfCounterEndRequest(uint elapsedTimeInMilliseconds)
    {
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_CallsExecuting", this.RequestedType.Name).Decrement();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ResponseTime", this.RequestedType.Name).IncrementMilliseconds((long) elapsedTimeInMilliseconds);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ResponseTimeBase", this.RequestedType.Name).Increment();
    }
  }
}
