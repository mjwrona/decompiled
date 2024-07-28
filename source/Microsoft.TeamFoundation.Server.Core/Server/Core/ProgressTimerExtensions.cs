// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProgressTimerExtensions
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Tracing;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.TeamFoundation.Server.Core
{
  public static class ProgressTimerExtensions
  {
    private const string TraceArea = "ProgressTimer";

    public static IDisposable TimeRegion(
      this IVssRequestContext requestContext,
      string area,
      string layer,
      long executionTimeThreshold = -1,
      [CallerMemberName] string regionName = null,
      int tracepoint = 0)
    {
      return (IDisposable) requestContext.TimeOrchestration(area, layer, new Guid(), executionTimeThreshold, (string) null, regionName: regionName, tracepoint: tracepoint);
    }

    public static ITimedOrchestrationRegion TimeOrchestration(
      this IVssRequestContext requestContext,
      string area,
      string layer,
      Guid orchestrationId,
      long executionTimeThreshold,
      string orchestrationFeature,
      bool orchestrationAlreadyInProgress = false,
      [CallerMemberName] string regionName = null,
      int tracepoint = 0)
    {
      try
      {
        return (ITimedOrchestrationRegion) ProgressTimerExtensions.RegionTimer.Enter(requestContext, area, layer, orchestrationId, executionTimeThreshold, orchestrationFeature, orchestrationAlreadyInProgress, regionName, tracepoint);
      }
      catch (Exception ex)
      {
        if (requestContext != null && !(ex is RequestCanceledException))
          requestContext.TraceException(1013788, "ProgressTimer", "RegionTimer", ex);
        return (ITimedOrchestrationRegion) null;
      }
    }

    public static void ProgressTimerJoin(
      this IVssRequestContext requestContext,
      IVssRequestContext previousRequestContext)
    {
      try
      {
        ProgressTimerExtensions.RegionTimer.ProgressTimerJoin(requestContext, previousRequestContext);
      }
      catch (Exception ex)
      {
        if (requestContext == null || ex is RequestCanceledException)
          return;
        requestContext.TraceException(1013788, "ProgressTimer", "RegionTimer", ex);
      }
    }

    private class RegionTimer : 
      List<ProgressTimerExtensions.RegionTimer>,
      IDisposable,
      ITimedOrchestrationRegion
    {
      private const string AmbientRegionKey = "RegionTimer.Ambient";
      private ProgressTimerExtensions.RegionTimer parentRegion;
      private string area;
      private string layer;
      private string regionName;
      private Guid orchestrationId;
      private string orchestrationFeature;
      private bool orchestrationAlreadyInProgress;
      private long executionTimeThreshold;
      private bool orchestrationDidNotComplete;
      private Exception exception;
      private bool isExceptionExpected;
      private int tracepoint;
      private DateTime traceStartTime;
      private Stopwatch timer = new Stopwatch();
      private int callCount;
      private DateTime firstStartTime;
      private IVssRequestContext requestContext;

      public static ProgressTimerExtensions.RegionTimer Enter(
        IVssRequestContext requestContext,
        string area,
        string layer,
        Guid orchestrationId,
        long executionTimeThreshold,
        string orchestrationFeature,
        bool orchestrationAlreadyInProgress,
        string regionName,
        int tracepoint)
      {
        ProgressTimerExtensions.RegionTimer regionTimer = (ProgressTimerExtensions.RegionTimer) null;
        ProgressTimerExtensions.RegionTimer ambientRegion = ProgressTimerExtensions.RegionTimer.GetAmbientRegion(requestContext);
        if (ambientRegion != null)
          regionTimer = ambientRegion.FirstOrDefault<ProgressTimerExtensions.RegionTimer>((Func<ProgressTimerExtensions.RegionTimer, bool>) (r => r.regionName == regionName));
        if (regionTimer != null)
        {
          regionTimer.requestContext = requestContext;
          regionTimer.area = area;
          regionTimer.layer = layer;
          regionTimer.orchestrationId = orchestrationId;
          regionTimer.executionTimeThreshold = executionTimeThreshold;
          regionTimer.orchestrationFeature = orchestrationFeature;
          regionTimer.orchestrationAlreadyInProgress = orchestrationAlreadyInProgress;
          regionTimer.tracepoint = tracepoint;
        }
        else
        {
          regionTimer = new ProgressTimerExtensions.RegionTimer(requestContext, area, layer, orchestrationId, executionTimeThreshold, orchestrationFeature, orchestrationAlreadyInProgress, regionName, tracepoint, ambientRegion);
          ambientRegion?.Add(regionTimer);
        }
        regionTimer.Enter();
        return regionTimer;
      }

      public static void ProgressTimerJoin(
        IVssRequestContext requestContext,
        IVssRequestContext previousRequestContext)
      {
        ProgressTimerExtensions.RegionTimer.SetAmbientRegion(requestContext, ProgressTimerExtensions.RegionTimer.GetAmbientRegion(previousRequestContext));
      }

      private static ProgressTimerExtensions.RegionTimer GetAmbientRegion(
        IVssRequestContext requestContext)
      {
        object ambientRegion;
        requestContext.Items.TryGetValue("RegionTimer.Ambient", out ambientRegion);
        return ambientRegion as ProgressTimerExtensions.RegionTimer;
      }

      private static void SetAmbientRegion(
        IVssRequestContext requestContext,
        ProgressTimerExtensions.RegionTimer region)
      {
        requestContext.Items["RegionTimer.Ambient"] = (object) region;
      }

      private RegionTimer(
        IVssRequestContext requestContext,
        string area,
        string layer,
        Guid orchestrationId,
        long executionTimeThreshold,
        string orchestrationFeature,
        bool orchestrationAlreadyInProgress,
        string regionName,
        int tracepoint,
        ProgressTimerExtensions.RegionTimer parentRegion)
      {
        this.requestContext = requestContext;
        this.area = area;
        this.layer = layer;
        this.orchestrationId = orchestrationId;
        this.executionTimeThreshold = executionTimeThreshold;
        this.orchestrationFeature = orchestrationFeature;
        this.orchestrationAlreadyInProgress = orchestrationAlreadyInProgress;
        this.regionName = regionName;
        this.tracepoint = tracepoint;
        this.parentRegion = parentRegion;
      }

      private void Enter()
      {
        DateTime resolutionUtcNow = DateTimeUtility.GetHighResolutionUtcNow();
        if (this.firstStartTime == new DateTime())
          this.firstStartTime = resolutionUtcNow;
        if (this.tracepoint != 0)
          this.traceStartTime = resolutionUtcNow;
        ++this.callCount;
        this.timer.Start();
        this.requestContext.TraceEnter(0, this.area, this.layer, this.regionName);
        ProgressTimerExtensions.RegionTimer.SetAmbientRegion(this.requestContext, this);
        this.WriteOrchestrationStartEntries();
      }

      void IDisposable.Dispose()
      {
        try
        {
          this.timer.Stop();
          if (this.requestContext == null)
            return;
          this.requestContext.TraceLeave(0, this.area, this.layer, this.regionName);
          ProgressTimerExtensions.RegionTimer.SetAmbientRegion(this.requestContext, this.parentRegion);
          if (this.tracepoint != 0)
          {
            this.WriteTracepoint();
            this.tracepoint = 0;
          }
          if (this.orchestrationId != new Guid())
          {
            this.WriteOrchestrationCompletionEvent();
            this.orchestrationId = new Guid();
          }
          this.requestContext = (IVssRequestContext) null;
        }
        catch (Exception ex)
        {
          if (this.requestContext == null || ex is RequestCanceledException)
            return;
          this.requestContext.TraceException(1013788, "ProgressTimer", nameof (RegionTimer), ex);
        }
      }

      void ITimedOrchestrationRegion.OrchestrationDidNotComplete() => this.orchestrationDidNotComplete = true;

      bool ITimedOrchestrationRegion.SetOrchestrationException(
        Exception exception,
        bool isExceptionExpected)
      {
        this.exception = exception;
        this.isExceptionExpected = isExceptionExpected;
        return false;
      }

      private void WriteOrchestrationCompletionEvent()
      {
        IOrchestrationLogTracingService service = this.requestContext.GetService<IOrchestrationLogTracingService>();
        if (this.orchestrationDidNotComplete)
          return;
        if (this.exception != null)
          service.TraceOrchestrationLogCompletionWithError(this.requestContext, this.orchestrationId, this.area, this.orchestrationFeature, this.regionName, this.exception.GetType().FullName, this.exception.Message, this.isExceptionExpected);
        else
          service.TraceOrchestrationLogCompletion(this.requestContext, this.orchestrationId, this.area, this.orchestrationFeature, this.regionName);
      }

      private void WriteOrchestrationStartEntries()
      {
        IOrchestrationLogTracingService service = (IOrchestrationLogTracingService) null;
        for (ProgressTimerExtensions.RegionTimer regionTimer = this; regionTimer != null; regionTimer = regionTimer.parentRegion)
        {
          if (regionTimer.orchestrationId != new Guid())
          {
            long num = this.executionTimeThreshold <= 0L ? -1L : this.executionTimeThreshold;
            if (service == null)
              service = this.requestContext.GetService<IOrchestrationLogTracingService>();
            string area = regionTimer.area;
            string orchestrationFeature = regionTimer.orchestrationFeature;
            if ((regionTimer != this ? 0 : (!this.orchestrationAlreadyInProgress ? 1 : 0)) != 0)
              service.TraceOrchestrationLogNewOrchestration(this.requestContext, this.orchestrationId, num, area, orchestrationFeature);
            else
              service.TraceOrchestrationLogPhaseStarted(this.requestContext, regionTimer.orchestrationId, num, area, orchestrationFeature, this.regionName);
          }
        }
      }

      private void WriteTracepoint() => this.requestContext.TraceAlways(this.tracepoint, TraceLevel.Info, this.area, this.layer, this.SerializeTracepointToJson(), (object[]) null);

      private string SerializeTracepointToJson()
      {
        long num = (DateTimeUtility.GetHighResolutionUtcNow().Ticks - this.traceStartTime.Ticks) / 10000L;
        StringBuilder stringBuilder = new StringBuilder(1100);
        stringBuilder.AppendFormat("{{\"$elapsedMs\":{0},\"$region\":\"{1}\"", (object) num, (object) this.regionName);
        foreach (ProgressTimerExtensions.RegionTimer regionTimer in (List<ProgressTimerExtensions.RegionTimer>) this)
        {
          stringBuilder.Append(",");
          StringBuilder buffer = stringBuilder;
          DateTime traceStartTime = this.traceStartTime;
          regionTimer.WriteJsonToBuffer(buffer, traceStartTime);
        }
        stringBuilder.Append('}');
        return stringBuilder.ToString();
      }

      private void WriteJsonToBuffer(StringBuilder buffer, DateTime parentStartTime)
      {
        long num = (this.firstStartTime.Ticks - parentStartTime.Ticks) / 10000L;
        buffer.AppendFormat("\"{0}\":{{\"$elapsedMs\":{1},\"$startMs\":{2}", (object) this.regionName, (object) this.timer.ElapsedMilliseconds, (object) num);
        if (this.callCount > 1)
          buffer.AppendFormat(",\"$callCount\":{0}", (object) this.callCount);
        foreach (ProgressTimerExtensions.RegionTimer regionTimer in (List<ProgressTimerExtensions.RegionTimer>) this)
        {
          buffer.Append(",");
          StringBuilder buffer1 = buffer;
          DateTime firstStartTime = this.firstStartTime;
          regionTimer.WriteJsonToBuffer(buffer1, firstStartTime);
        }
        buffer.Append('}');
      }
    }
  }
}
