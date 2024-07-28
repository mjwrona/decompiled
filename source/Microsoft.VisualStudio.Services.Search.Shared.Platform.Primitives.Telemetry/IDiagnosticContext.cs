// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.IDiagnosticContext
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4AA9C920-1627-4C01-9F3D-849A7BC9C916
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.WebPlatform;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry
{
  public interface IDiagnosticContext
  {
    Guid ActivityId { get; }

    Dictionary<string, string> OnPremiseTelemetryProperties { get; }

    bool IsOnPremisesEnvironment { get; }

    void TraceEnter(int tracepoint, string area, string layer, string method);

    void TraceLeave(int tracepoint, string area, string layer, string method);

    void TraceException(int tracepoint, string area, string layer, Exception e);

    void Trace(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string message,
      string[] tags);

    void TraceConditionally(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Func<string> message);

    void TraceAlways(int tracepoint, TraceLevel level, string area, string layer, string message);

    void EnterMethod(MethodInformation methodInformation);

    void LeaveMethod();

    void LogKpi(string kpiArea, string scope, string kpiName, double value);

    void LogKpi(string kpiArea, string scope, List<KpiMetric> kpiMetrics);

    void LogIndicator(string eventName);

    void LogCiData(string area, string feature, CustomerIntelligenceData properties);

    void LogCiData(string area, string feature, string propertyInfo, bool value);

    void LogCiData(string area, string feature, string propertyInfo, double value);

    void LogCiData(string area, string feature, string propertyInfo, string value);

    void LogClientTraceData(string area, string feature, ClientTraceData properties);

    void LogClientTraceData(string area, string feature, Level level, string message);

    void LogClientTraceData(string area, string feature, Exception exception);
  }
}
