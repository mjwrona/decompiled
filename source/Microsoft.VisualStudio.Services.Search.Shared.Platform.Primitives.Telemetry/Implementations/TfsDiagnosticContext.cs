// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations.TfsDiagnosticContext
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4AA9C920-1627-4C01-9F3D-849A7BC9C916
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Search.Platform.Common;
using Microsoft.VisualStudio.Services.WebPlatform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations
{
  [Serializable]
  public class TfsDiagnosticContext : IDiagnosticContext
  {
    private readonly IVssRequestContext m_tfsRequestContext;
    private readonly ISearchKpiLoggerService m_kpiLoggerService;
    private readonly ISearchCiLoggerService m_ciLoggerService;
    private readonly ISearchClientTraceLoggerService m_clientTraceLoggerService;
    private const int MaxTraceMessageChunkSize = 8000;

    public TfsDiagnosticContext(IVssRequestContext tfsRequestContext)
    {
      this.m_tfsRequestContext = tfsRequestContext;
      this.m_kpiLoggerService = SearchPlatformHelper.GetSearchKpiLoggerService(tfsRequestContext);
      this.m_ciLoggerService = SearchPlatformHelper.GetSearchCiLoggerService(tfsRequestContext);
      this.m_clientTraceLoggerService = SearchPlatformHelper.GetSearchClientTraceLoggerService(tfsRequestContext);
    }

    public Guid ActivityId => this.m_tfsRequestContext.ActivityId;

    public Dictionary<string, string> OnPremiseTelemetryProperties { get; } = new Dictionary<string, string>();

    public bool IsOnPremisesEnvironment => this.m_tfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment;

    public void TraceEnter(int tracepoint, string area, string layer, string method) => this.m_tfsRequestContext.TraceEnter(tracepoint, area, layer, method);

    public void TraceLeave(int tracepoint, string area, string layer, string method) => this.m_tfsRequestContext.TraceLeave(tracepoint, area, layer, method);

    public void TraceException(int tracepoint, string area, string layer, Exception e) => this.m_tfsRequestContext.TraceException(tracepoint, TraceLevel.Error, area, layer, e);

    public void Trace(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string message,
      string[] tags)
    {
      if (string.IsNullOrEmpty(message) || message.Length <= 8000)
      {
        this.TraceInternal(tracepoint, level, area, layer, message, tags);
      }
      else
      {
        foreach (string messageChunk in TfsDiagnosticContext.GetMessageChunks(message, 8000))
          this.TraceInternal(tracepoint, level, area, layer, messageChunk, tags);
      }
    }

    public void TraceConditionally(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Func<string> message)
    {
      if (message == null || level != TraceLevel.Error && !this.IsTracing(tracepoint, level, area, layer))
        return;
      string message1 = message();
      this.Trace(tracepoint, level, area, layer, message1, (string[]) null);
    }

    public void TraceAlways(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string message)
    {
      if (string.IsNullOrEmpty(message))
        return;
      if (message.Length <= 8000)
      {
        this.m_tfsRequestContext.TraceAlways(tracepoint, level, area, layer, message);
      }
      else
      {
        foreach (string messageChunk in TfsDiagnosticContext.GetMessageChunks(message, 8000))
          this.m_tfsRequestContext.TraceAlways(tracepoint, level, area, layer, messageChunk);
      }
    }

    public void EnterMethod(MethodInformation methodInformation) => this.m_tfsRequestContext.EnterMethod(methodInformation);

    public void LeaveMethod() => this.m_tfsRequestContext.LeaveMethod();

    public void LogKpi(string kpiArea, string scope, List<KpiMetric> kpiMetrics) => this.m_kpiLoggerService.Publish(this.m_tfsRequestContext, kpiArea, scope, kpiMetrics);

    public void LogKpi(string kpiArea, string scope, string kpiName, double value)
    {
      List<KpiMetric> kpiMetric = new List<KpiMetric>()
      {
        new KpiMetric()
        {
          Name = kpiName,
          Value = value,
          TimeStamp = DateTime.UtcNow
        }
      };
      this.m_kpiLoggerService.Publish(this.m_tfsRequestContext, kpiArea, scope, kpiMetric);
    }

    public void LogCiData(string area, string feature, CustomerIntelligenceData properties) => this.m_ciLoggerService.Publish(this.m_tfsRequestContext, area, feature, properties);

    public void LogCiData(string area, string feature, string propertyInfo, bool value) => this.m_ciLoggerService.Publish(this.m_tfsRequestContext, area, feature, propertyInfo, value);

    public void LogCiData(string area, string feature, string propertyInfo, double value) => this.m_ciLoggerService.Publish(this.m_tfsRequestContext, area, feature, propertyInfo, value);

    public void LogCiData(string area, string feature, string propertyInfo, string value) => this.m_ciLoggerService.Publish(this.m_tfsRequestContext, area, feature, propertyInfo, value);

    public void LogClientTraceData(string area, string feature, ClientTraceData properties) => this.m_clientTraceLoggerService.Publish(this.m_tfsRequestContext, area, feature, properties);

    public void LogClientTraceData(string area, string feature, Level level, string message) => this.m_clientTraceLoggerService.Publish(this.m_tfsRequestContext, area, feature, level, message);

    public void LogClientTraceData(string area, string feature, Exception exception) => this.m_clientTraceLoggerService.Publish(this.m_tfsRequestContext, area, feature, exception);

    public void LogIndicator(string eventName)
    {
      this.m_ciLoggerService.PublishOnPremisesEvent(this.m_tfsRequestContext, eventName, this.OnPremiseTelemetryProperties);
      this.OnPremiseTelemetryProperties.Clear();
    }

    private void TraceInternal(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string message,
      string[] tags)
    {
      if (tags != null)
        this.m_tfsRequestContext.Trace(tracepoint, level, area, layer, tags, message);
      else
        this.m_tfsRequestContext.Trace(tracepoint, level, area, layer, message);
    }

    private bool IsTracing(int tracepoint, TraceLevel level, string area, string layer) => this.m_tfsRequestContext.IsTracing(tracepoint, level, area, layer);

    private static IEnumerable<string> GetMessageChunks(string message, int chunkSize)
    {
      int chunkCount = 0;
      if (chunkSize > 0)
      {
        for (int i = 0; i < message.Length; i += chunkSize)
          yield return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}] {1}", (object) chunkCount++, (object) message.Substring(i, Math.Min(chunkSize, message.Length - i)));
      }
      else
        yield return message;
    }
  }
}
