// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TimedActionTracer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class TimedActionTracer : ActionTracer
  {
    private readonly string m_rootActionName;
    private const string c_separator = "-";

    protected TimedActionTracer(string area, string layer)
      : base(area, layer)
    {
      this.m_rootActionName = area + "-" + layer;
    }

    private void BeginAction(IVssRequestContext requestContext, string fullActionName) => this.OnBeginAction(fullActionName);

    private void EndAction(
      IVssRequestContext requestContext,
      string fullActionName,
      int slowTracePoint,
      int slowThresholdMilliSeconds,
      long timeTakenMicroSeconds)
    {
      long timeTakenMilliSeconds = timeTakenMicroSeconds / 1000L;
      requestContext.Trace(slowTracePoint, TraceLevel.Verbose, this.Area, this.Layer, "Action {0} took {1} micro seconds", (object) fullActionName, (object) timeTakenMicroSeconds);
      this.OnEndAction(fullActionName, timeTakenMilliSeconds);
      if (timeTakenMilliSeconds <= (long) slowThresholdMilliSeconds)
        return;
      this.OnSlowAction(fullActionName);
      this.Trace(requestContext, slowTracePoint, TraceLevel.Warning, (Func<string>) (() => string.Format("Action {0} was expected to take < {1} ms but it took: {2} ms", (object) fullActionName, (object) slowThresholdMilliSeconds, (object) timeTakenMilliSeconds)), nameof (EndAction));
    }

    public TimedActionTracer.TimedAction TraceTimedAction(
      IVssRequestContext requestContext,
      int slowTracePoint,
      int slowThresholdMilliSeconds = 1000,
      [CallerMemberName] string actionName = null)
    {
      return new TimedActionTracer.TimedAction(requestContext, this, actionName, slowTracePoint, slowThresholdMilliSeconds);
    }

    public T TraceTimedAction<T>(
      IVssRequestContext requestContext,
      TimedActionTracePoints tracePoints,
      Func<T> action,
      int slowThresholdMilliSeconds = 1000,
      [CallerMemberName] string actionName = null)
    {
      using (this.TraceTimedAction(requestContext, tracePoints.Slow, slowThresholdMilliSeconds, actionName))
        return this.TraceAction<T>(requestContext, (ActionTracePoints) tracePoints, action, actionName);
    }

    public void TraceTimedAction(
      IVssRequestContext requestContext,
      TimedActionTracePoints tracePoints,
      Action action,
      int slowThresholdMilliSeconds = 1000,
      [CallerMemberName] string actionName = null)
    {
      using (this.TraceTimedAction(requestContext, tracePoints.Slow, slowThresholdMilliSeconds, actionName))
        this.TraceAction(requestContext, (ActionTracePoints) tracePoints, action, actionName);
    }

    public void TraceCacheHit(
      IVssRequestContext requestContext,
      int tracePoint,
      string cacheKey,
      [CallerMemberName] string actionName = null)
    {
      this.TraceCacheHit(requestContext, tracePoint, (object) cacheKey, actionName);
    }

    public void TraceCacheHit(
      IVssRequestContext requestContext,
      int tracePoint,
      object cacheKey,
      [CallerMemberName] string actionName = null)
    {
      string fullActionName = this.GenerateFullActionName(actionName);
      requestContext.Trace(tracePoint, TraceLevel.Verbose, this.Area, this.Layer, "Cache hit: action={0}, key={1}", (object) fullActionName, cacheKey);
      this.OnCacheHit(fullActionName);
    }

    public void TraceCacheMiss(
      IVssRequestContext requestContext,
      int tracePoint,
      string cacheKey,
      [CallerMemberName] string actionName = null)
    {
      this.TraceCacheMiss(requestContext, tracePoint, (object) cacheKey, actionName);
    }

    public void TraceCacheMiss(
      IVssRequestContext requestContext,
      int tracePoint,
      object cacheKey,
      [CallerMemberName] string actionName = null)
    {
      string fullActionName = this.GenerateFullActionName(actionName);
      requestContext.Trace(tracePoint, TraceLevel.Verbose, this.Area, this.Layer, "Cache miss: action={0}, key={1}", (object) fullActionName, cacheKey);
      this.OnCacheMiss(fullActionName);
    }

    public void TraceCacheInvalidation(
      IVssRequestContext requestContext,
      int tracePoint,
      string cacheKey,
      [CallerMemberName] string actionName = null)
    {
      this.TraceCacheInvalidation(requestContext, tracePoint, (Func<string>) (() => cacheKey), actionName);
    }

    public void TraceCacheInvalidation(
      IVssRequestContext requestContext,
      int tracePoint,
      Func<string> cacheKey,
      [CallerMemberName] string actionName = null)
    {
      string fullActionName = this.GenerateFullActionName(actionName);
      this.Trace(requestContext, tracePoint, TraceLevel.Verbose, (Func<string>) (() => "Cache invalidation: action=" + fullActionName + ", key=" + cacheKey()), nameof (TraceCacheInvalidation));
      this.OnCacheInvalidation(fullActionName);
    }

    protected override void OnTraceException(
      IVssRequestContext requestContext,
      int tracePoint,
      Exception ex,
      string actionName)
    {
      string fullActionName = this.GenerateFullActionName(actionName);
      base.OnTraceException(requestContext, tracePoint, ex, fullActionName);
      this.OnException(fullActionName);
    }

    protected override void OnTraceError(
      IVssRequestContext requestContext,
      int tracePoint,
      Func<string> message,
      string actionName)
    {
      string fullActionName = this.GenerateFullActionName(actionName);
      base.OnTraceError(requestContext, tracePoint, message, fullActionName);
      this.OnError(fullActionName);
    }

    protected abstract void OnCacheHit(string actionName);

    protected abstract void OnCacheMiss(string actionName);

    protected abstract void OnCacheInvalidation(string actionName);

    protected abstract void OnBeginAction(string actionName);

    protected abstract void OnSlowAction(string actionName);

    protected abstract void OnEndAction(string acionName, long timeTakenMilliSeconds);

    protected abstract void OnException(string actionName);

    protected abstract void OnError(string actionName);

    private string GenerateFullActionName(string actionName) => !string.IsNullOrEmpty(actionName) ? this.m_rootActionName + "-" + actionName : this.m_rootActionName;

    public sealed class TimedAction : IDisposable
    {
      private readonly long m_startTimeMicroSeconds;
      private readonly int m_slowThresholdMilliSeconds;
      private readonly int m_slowTracePoint;
      private readonly string m_fullActionName;
      private readonly TimedActionTracer m_tracer;
      private readonly IVssRequestContext m_requestContext;

      public TimedAction(
        IVssRequestContext requestContext,
        TimedActionTracer tracer,
        string actionName,
        int slowTracePoint,
        int slowThresholdMilliSeconds = 1000)
      {
        this.m_requestContext = requestContext;
        this.m_tracer = tracer;
        this.m_fullActionName = this.m_tracer.GenerateFullActionName(actionName);
        this.m_slowTracePoint = slowTracePoint;
        this.m_slowThresholdMilliSeconds = slowThresholdMilliSeconds;
        this.m_tracer.BeginAction(this.m_requestContext, this.m_fullActionName);
        this.m_startTimeMicroSeconds = requestContext.ExecutionTime();
      }

      void IDisposable.Dispose() => this.m_tracer.EndAction(this.m_requestContext, this.m_fullActionName, this.m_slowTracePoint, this.m_slowThresholdMilliSeconds, this.m_requestContext.ExecutionTime() - this.m_startTimeMicroSeconds);
    }
  }
}
