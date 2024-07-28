// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations.DebugDiagnosticContext
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4AA9C920-1627-4C01-9F3D-849A7BC9C916
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.WebPlatform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations
{
  [Serializable]
  public class DebugDiagnosticContext : IDiagnosticContext
  {
    public Guid ActivityId { get; } = Guid.NewGuid();

    public Dictionary<string, string> OnPremiseTelemetryProperties { get; } = new Dictionary<string, string>();

    public bool IsOnPremisesEnvironment => false;

    public void EnterMethod(MethodInformation methodInformation) => DebugDiagnosticContext.Log((string) null, (string) null, "Entering method: {0}", (object) methodInformation.Name);

    public void LeaveMethod() => DebugDiagnosticContext.Log((string) null, (string) null, "Leaving method");

    public void LogCiData(string area, string feature, CustomerIntelligenceData properties) => DebugDiagnosticContext.Log(area, feature, "Logging CI properties");

    public void LogCiData(string area, string feature, string propertyInfo, double value) => DebugDiagnosticContext.Log(area, feature, "Logging CI property- {0} : {1}", (object) propertyInfo, (object) value);

    public void LogCiData(string area, string feature, string propertyInfo, string value) => DebugDiagnosticContext.Log(area, feature, "Logging CI property- {0} : {1}", (object) propertyInfo, (object) value);

    public void LogCiData(string area, string feature, string propertyInfo, bool value) => DebugDiagnosticContext.Log(area, feature, "Logging CI property- {0} : {1}", (object) propertyInfo, (object) value);

    public void LogClientTraceData(string area, string feature, ClientTraceData properties) => DebugDiagnosticContext.Log(area, feature, "Logging ClientTrace properties");

    public void LogClientTraceData(string area, string feature, Level level, string message) => DebugDiagnosticContext.Log(area, feature, FormattableString.Invariant(FormattableStringFactory.Create("Logging ClientTrace message: [Area = {0}, Feature = {1}, Level = {2}, Message = {3}]", (object) area, (object) feature, (object) level, (object) message)));

    public void LogClientTraceData(string area, string feature, Exception exception) => DebugDiagnosticContext.Log(area, feature, FormattableString.Invariant(FormattableStringFactory.Create("Logging ClientTrace exception: [Area = {0}, Feature = {1}, Exception = {2}]", (object) area, (object) feature, (object) exception)));

    public void LogKpi(string kpiArea, string scope, string kpiName, double value) => DebugDiagnosticContext.Log(kpiArea, scope, "Logging KPI property- {0} : {1}", (object) kpiName, (object) value);

    public void LogKpi(string kpiArea, string scope, List<KpiMetric> kpiMetrics)
    {
      if (kpiMetrics == null || kpiMetrics.Count <= 0)
        return;
      foreach (KpiMetric kpiMetric in kpiMetrics)
        DebugDiagnosticContext.Log(kpiArea, scope, "Logging KPI property- {0} : {1}", (object) kpiMetric.Name, (object) kpiMetric.Value);
    }

    public void LogIndicator(string eventName)
    {
    }

    public void Trace(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string message,
      string[] tags)
    {
      DebugDiagnosticContext.Log(area, layer, "[{0} {1}]: {2}", (object) tracepoint, (object) level, (object) message);
    }

    public void TraceConditionally(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Func<string> message)
    {
      this.Trace(tracepoint, level, area, layer, message(), (string[]) null);
    }

    public void TraceAlways(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      string message)
    {
      this.Trace(tracepoint, level, area, layer, message, (string[]) null);
    }

    public void TraceEnter(int tracepoint, string area, string layer, string method) => DebugDiagnosticContext.Log(area, layer, "[{0}] Entering method: {1}", (object) tracepoint, (object) method);

    public void TraceException(int tracepoint, string area, string layer, Exception e) => DebugDiagnosticContext.Log(area, layer, "[{0}]: Exception- {1} {2}\n{3}", (object) tracepoint, (object) e.HResult, (object) e.Message, (object) e.StackTrace);

    public void TraceLeave(int tracepoint, string area, string layer, string method) => DebugDiagnosticContext.Log(area, layer, "[{0}] Leaving method: {1}", (object) tracepoint, (object) method);

    private static void Log(string area, string layer, string format, params object[] args)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}] [{1}.{2}] ", (object) DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss.fff", (IFormatProvider) DateTimeFormatInfo.InvariantInfo), (object) (area ?? string.Empty), (object) (layer ?? string.Empty)));
      stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args));
    }
  }
}
