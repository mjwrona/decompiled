// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics.ReleaseManagementTimer
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics
{
  public class ReleaseManagementTimer : IDisposable
  {
    public const string MessageFormat = "ReleaseManagementTimer {0} {1} {2}ms";
    public const string EndpointName = "End";
    private const int DefaultSamplingRate = 1;
    private readonly string actionName;
    private readonly string layer;
    private readonly IVssRequestContext context;
    private readonly IStopwatch stopwatch;
    private readonly Func<IVssRequestContext, string, string, int, bool, PerformanceDescriptor> measurePerformanceTelemetry;
    private readonly int defaultTracePoint;
    private readonly PerformanceDescriptor performanceDescriptor;
    private long previousElapsedMilliseconds;
    private string currentLapLayer;

    public static ReleaseManagementTimer Create(
      IVssRequestContext context,
      string layer,
      string actionName,
      int defaultTracePoint)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return new ReleaseManagementTimer(context, layer, actionName, defaultTracePoint, 1, false, (IStopwatch) new PlatformStopwatch(), ReleaseManagementTimer.\u003C\u003EO.\u003C0\u003E__Measure ?? (ReleaseManagementTimer.\u003C\u003EO.\u003C0\u003E__Measure = new Func<IVssRequestContext, string, string, int, bool, PerformanceDescriptor>(PerformanceTelemetryService.Measure)));
    }

    public static ReleaseManagementTimer Create(
      IVssRequestContext context,
      string layer,
      string actionName,
      int defaultTracePoint,
      int samplingRate,
      bool isTopLevelScenario)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return new ReleaseManagementTimer(context, layer, actionName, defaultTracePoint, samplingRate, isTopLevelScenario, (IStopwatch) new PlatformStopwatch(), ReleaseManagementTimer.\u003C\u003EO.\u003C0\u003E__Measure ?? (ReleaseManagementTimer.\u003C\u003EO.\u003C0\u003E__Measure = new Func<IVssRequestContext, string, string, int, bool, PerformanceDescriptor>(PerformanceTelemetryService.Measure)));
    }

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "7", Justification = "This is being passed in by the c'tor")]
    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "6", Justification = "This is being passed in by the c'tor")]
    protected ReleaseManagementTimer(
      IVssRequestContext context,
      string layer,
      string actionName,
      int defaultTracePoint,
      int samplingRate,
      bool isTopLevelScenario,
      IStopwatch stopwatch,
      Func<IVssRequestContext, string, string, int, bool, PerformanceDescriptor> measurePerformanceTelemetry)
    {
      this.context = context;
      this.layer = layer;
      this.actionName = actionName;
      this.defaultTracePoint = defaultTracePoint;
      this.stopwatch = stopwatch;
      this.currentLapLayer = layer;
      this.measurePerformanceTelemetry = measurePerformanceTelemetry;
      this.performanceDescriptor = measurePerformanceTelemetry(context, layer, actionName, samplingRate, isTopLevelScenario);
      stopwatch.Start();
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    public void RecordLap(string lapLayer, string lapName, int tracePoint)
    {
      long elapsedTime = this.stopwatch.ElapsedMilliseconds - this.previousElapsedMilliseconds;
      this.context.Trace(tracePoint, TraceLevel.Verbose, "ReleaseManagementService", this.currentLapLayer, "ReleaseManagementTimer {0} {1} {2}ms", (object) this.actionName, (object) lapName, (object) elapsedTime);
      this.PublishLapData(lapLayer, lapName, elapsedTime);
      this.previousElapsedMilliseconds = this.stopwatch.ElapsedMilliseconds;
      this.currentLapLayer = lapLayer;
    }

    public void SetAdditionalData<TValue>(string key, TValue value) where TValue : IConvertible => this.performanceDescriptor.SetAdditionalData<TValue>(key, value);

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.stopwatch.Stop();
      long elapsedMilliseconds = this.stopwatch.ElapsedMilliseconds;
      this.performanceDescriptor.SetElapsedTime(elapsedMilliseconds);
      this.performanceDescriptor.Dispose();
      this.context.Trace(this.defaultTracePoint, TraceLevel.Verbose, "ReleaseManagementService", this.layer, "ReleaseManagementTimer {0} {1} {2}ms", (object) this.actionName, (object) "End", (object) elapsedMilliseconds);
    }

    private void PublishLapData(string lapLayer, string lapName, long elapsedTime)
    {
      using (PerformanceDescriptor performanceDescriptor = this.measurePerformanceTelemetry(this.context, lapLayer, lapName, 1, false))
        performanceDescriptor.SetElapsedTime(elapsedTime);
    }
  }
}
