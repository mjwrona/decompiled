// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.Tracer
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public sealed class Tracer : IDisposable
  {
    private readonly ITraceRequest trace;
    private readonly string area;
    private readonly string layer;
    private readonly string methodName;
    private readonly int tracepoint;

    private Tracer(
      ITraceRequest trace,
      string area,
      string layer,
      int tracepoint,
      string methodName)
    {
      this.trace = trace;
      this.area = area;
      this.layer = layer;
      this.methodName = methodName;
      this.tracepoint = tracepoint;
    }

    public static Tracer Enter(
      IVssRequestContext requestContext,
      TraceData data,
      int tracepoint,
      [CallerMemberName] string methodName = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<TraceData>(data, nameof (data));
      ArgumentUtility.CheckGreaterThanZero((float) tracepoint, nameof (tracepoint));
      return Tracer.Enter(requestContext.RequestTracer, data, tracepoint, methodName);
    }

    private static Tracer Enter(
      ITraceRequest trace,
      TraceData data,
      int tracepoint,
      [CallerMemberName] string methodName = null)
    {
      Tracer tracer = new Tracer(trace, data.Area, data.Layer, tracepoint, methodName);
      trace.TraceEnter(tracepoint + Offset.Enter, data.Area, data.Layer, methodName);
      return tracer;
    }

    public static Tracer Enter(
      IVssRequestContext requestContext,
      SingleLocationTracePoint tracePoint,
      [CallerMemberName] string methodName = null)
    {
      ITraceRequest requestTracer = requestContext.RequestTracer;
      TraceData data = new TraceData();
      data.Area = tracePoint.Area;
      data.Layer = tracePoint.Layer;
      int tracePoint1 = tracePoint.TracePoint;
      string methodName1 = methodName;
      return Tracer.Enter(requestTracer, data, tracePoint1, methodName1);
    }

    public bool TraceExceptionFilter(Exception exception)
    {
      this.TraceException(exception);
      return false;
    }

    public void Dispose() => this.TraceLeave();

    public void TraceAlways(string format, params object[] args) => this.trace.TraceAlways(this.tracepoint, TraceLevel.Info, this.area, this.layer, format, args);

    public void TraceInfo(string message, int offset = 0) => this.trace.Trace(this.tracepoint + offset, TraceLevel.Info, this.area, this.layer, message);

    public void TraceError(string message, int offset = 0) => this.trace.Trace(this.tracepoint + offset, TraceLevel.Error, this.area, this.layer, message);

    public void TraceWarning(string message, int offset = 0) => this.trace.Trace(this.tracepoint + offset, TraceLevel.Warning, this.area, this.layer, message);

    public void TraceException(Exception exception)
    {
      if (exception == null)
        throw new ArgumentNullException(nameof (exception));
      if (!(exception is AggregateException aggregateException1))
      {
        this.trace.TraceException(this.tracepoint + Offset.Exception, TraceLevel.Error, this.area, this.layer, exception);
      }
      else
      {
        AggregateException aggregateException = aggregateException1.Flatten();
        if (aggregateException.InnerExceptions.Count > 1)
          this.trace.Trace(this.tracepoint + Offset.Exception, TraceLevel.Info, this.area, this.layer, "Tracing {0} exceptions from AggregateException.", (object) aggregateException.InnerExceptions.Count);
        foreach (Exception innerException in aggregateException.InnerExceptions)
          this.trace.TraceException(this.tracepoint + Offset.Exception, TraceLevel.Error, this.area, this.layer, innerException);
      }
    }

    private void TraceLeave() => this.trace.TraceLeave(this.tracepoint + Offset.Leave, this.area, this.layer, this.methodName);
  }
}
