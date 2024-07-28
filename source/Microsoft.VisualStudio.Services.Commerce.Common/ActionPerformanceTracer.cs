// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ActionPerformanceTracer
// Assembly: Microsoft.VisualStudio.Services.Commerce.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A442E579-88AD-441C-B92A-FDB0C6C9E30B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class ActionPerformanceTracer
  {
    private readonly string layer;
    private readonly string area;

    public ActionPerformanceTracer(string area, string layer)
    {
      this.area = area;
      this.layer = layer;
    }

    public ActionPerformanceTracer.Timer Trace(
      IVssRequestContext requestContext,
      int tracepoint,
      string actionName = "",
      int slowThresholdMilliseconds = 0)
    {
      return new ActionPerformanceTracer.Timer(requestContext, tracepoint, this.area, this.layer, slowThresholdMilliseconds, actionName);
    }

    public T Trace<T>(
      IVssRequestContext requestContext,
      Func<T> action,
      int tracepoint,
      string actionName = "",
      int slowThresholdMilliseconds = 0)
    {
      using (new ActionPerformanceTracer.Timer(requestContext, tracepoint, this.area, this.layer, slowThresholdMilliseconds, actionName))
        return action();
    }

    public class Timer : IDisposable
    {
      private readonly string actionName;
      private readonly int slowThresholdMilliseconds;
      private readonly IVssRequestContext requestContext;
      private readonly int tracepoint;
      private readonly string layer;
      private readonly string area;
      private long startTime;

      public Timer(
        IVssRequestContext requestContext,
        int tracepoint,
        string area,
        string layer,
        int slowThresholdMilliseconds,
        string actionName)
      {
        this.requestContext = requestContext;
        this.tracepoint = tracepoint;
        this.area = area;
        this.layer = layer;
        this.actionName = actionName;
        this.slowThresholdMilliseconds = slowThresholdMilliseconds;
        this.Start();
      }

      private void Start() => this.startTime = this.requestContext.ExecutionTime();

      private void Stop()
      {
        if (this.slowThresholdMilliseconds > 0)
        {
          if (this.ExecutionTimeInMilliseconds <= (long) this.slowThresholdMilliseconds)
            return;
          this.requestContext.Trace(this.tracepoint, TraceLevel.Warning, this.area, this.layer, string.Format("Action {0} was expected to take < {1} ms but it took: {2} ms", (object) this.actionName, (object) this.slowThresholdMilliseconds, (object) this.ExecutionTimeInMilliseconds));
        }
        else
          this.requestContext.Trace(this.tracepoint, TraceLevel.Verbose, this.area, this.layer, string.Format("Action {0} took {1} milliseconds", (object) this.actionName, (object) this.ExecutionTimeInMilliseconds));
      }

      public void Dispose() => this.Stop();

      public long ExecutionTimeInMicroseconds => this.requestContext.ExecutionTime() - this.startTime;

      public long ExecutionTimeInMilliseconds => this.ExecutionTimeInMicroseconds / 1000L;
    }
  }
}
