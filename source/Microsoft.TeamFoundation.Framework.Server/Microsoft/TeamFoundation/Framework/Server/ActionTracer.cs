// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ActionTracer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ActionTracer
  {
    protected readonly string Area;
    protected readonly string Layer;

    public ActionTracer(string area, string layer)
    {
      ArgumentUtility.CheckForNull<string>(area, nameof (area));
      ArgumentUtility.CheckForNull<string>(layer, nameof (layer));
      this.Area = area;
      this.Layer = layer;
    }

    public T TraceAction<T>(
      IVssRequestContext requestContext,
      ActionTracePoints tracePoints,
      Func<T> action,
      [CallerMemberName] string actionName = null)
    {
      try
      {
        this.TraceEnter(requestContext, tracePoints.Enter, actionName);
        return action();
      }
      catch (Exception ex)
      {
        this.TraceException(requestContext, tracePoints.Exception, ex, actionName);
        throw;
      }
      finally
      {
        this.TraceExit(requestContext, tracePoints.Exit, actionName);
      }
    }

    public void TraceAction(
      IVssRequestContext requestContext,
      ActionTracePoints tracePoints,
      Action action,
      [CallerMemberName] string actionName = null)
    {
      try
      {
        this.TraceEnter(requestContext, tracePoints.Enter, actionName);
        action();
      }
      catch (Exception ex)
      {
        this.TraceException(requestContext, tracePoints.Exception, ex, actionName);
        throw;
      }
      finally
      {
        this.TraceExit(requestContext, tracePoints.Exit, actionName);
      }
    }

    public T TraceAction<T, U>(
      IVssRequestContext requestContext,
      ActionTracePoints tracePoints,
      ActionTracer.FunctionWithOutParam<T, U> action,
      out U output,
      [CallerMemberName] string actionName = null,
      params object[] actionArgs)
    {
      try
      {
        this.TraceEnter(requestContext, tracePoints.Enter, actionName);
        return action(out output, actionArgs);
      }
      catch (Exception ex)
      {
        this.TraceException(requestContext, tracePoints.Exception, ex, actionName);
        throw;
      }
      finally
      {
        this.TraceExit(requestContext, tracePoints.Exit, actionName);
      }
    }

    private void TraceEnter(IVssRequestContext requestContext, int tracePoint, string actionName) => requestContext.TraceEnter(tracePoint, this.Area, this.Layer, actionName);

    private void TraceExit(IVssRequestContext requestContext, int tracePoint, string actionName) => requestContext.TraceLeave(tracePoint, this.Area, this.Layer, actionName);

    public void Trace(
      IVssRequestContext requestContext,
      int tracePoint,
      TraceLevel level,
      Func<string> message,
      [CallerMemberName] string actionName = null)
    {
      if (level == TraceLevel.Error)
        this.TraceError(requestContext, tracePoint, message, actionName);
      else if (level == TraceLevel.Warning)
        requestContext.TraceAlways(tracePoint, level, this.Area, this.Layer, message());
      else
        requestContext.TraceConditionally(tracePoint, level, this.Area, this.Layer, message);
    }

    public void TraceError(
      IVssRequestContext requestContext,
      int tracePoint,
      Func<string> message,
      [CallerMemberName] string actionName = null)
    {
      requestContext.TraceConditionally(tracePoint, TraceLevel.Error, this.Area, this.Layer, message);
      this.OnTraceError(requestContext, tracePoint, message, actionName);
    }

    public void TraceException(
      IVssRequestContext requestContext,
      int tracePoint,
      Exception ex,
      [CallerMemberName] string actionName = null)
    {
      requestContext.TraceException(tracePoint, this.Area, this.Layer, ex);
      this.OnTraceException(requestContext, tracePoint, ex, actionName);
    }

    protected virtual void OnTraceError(
      IVssRequestContext requestContext,
      int tracePoint,
      Func<string> message,
      string actionName)
    {
    }

    protected virtual void OnTraceException(
      IVssRequestContext requestContext,
      int tracePoint,
      Exception ex,
      string actionName)
    {
    }

    public delegate TResult FunctionWithOutParam<out TResult, U>(out U output, params object[] args);
  }
}
