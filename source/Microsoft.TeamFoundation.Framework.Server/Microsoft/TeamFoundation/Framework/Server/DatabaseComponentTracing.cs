// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseComponentTracing
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class DatabaseComponentTracing
  {
    private string m_nonVersionedComponentClassName;
    protected readonly string m_traceLayer;
    private string m_dataspaceCategory;
    private string m_className;

    protected DatabaseComponentTracing(string traceLayer) => this.m_traceLayer = traceLayer;

    public string DataspaceCategory
    {
      get => this.m_dataspaceCategory;
      protected set => this.m_dataspaceCategory = value;
    }

    public string NameOfClass
    {
      get => this.m_className;
      protected set => this.m_className = value;
    }

    protected string NonVersionedComponentClassName
    {
      get => this.m_nonVersionedComponentClassName;
      set => this.m_nonVersionedComponentClassName = value;
    }

    protected abstract IVssRequestContext RequestContext { get; }

    protected virtual string TraceArea => this.m_dataspaceCategory ?? this.m_className ?? this.m_nonVersionedComponentClassName;

    protected virtual void Trace(int tracepoint, TraceLevel level, string message) => this.Trace(tracepoint, level, message, (object[]) null);

    protected virtual void Trace(
      int tracepoint,
      TraceLevel level,
      string format,
      params object[] args)
    {
      this.Trace(tracepoint, this.m_traceLayer, level, format, args);
    }

    protected virtual void Trace(
      int tracepoint,
      string layer,
      TraceLevel level,
      string format,
      params object[] args)
    {
      if (this.RequestContext != null)
        VssRequestContextExtensions.Trace(this.RequestContext, tracepoint, level, this.TraceArea, layer, format, args);
      else
        TeamFoundationTracingService.TraceRaw(tracepoint, level, this.TraceArea, this.m_traceLayer, format, args);
    }

    protected virtual void Trace(int tracepoint, TraceLevel level, string[] tags, string message) => this.Trace(tracepoint, level, tags, message, (object[]) null);

    protected virtual void Trace(
      int tracepoint,
      TraceLevel level,
      string[] tags,
      string format,
      params object[] args)
    {
      if (this.RequestContext != null)
        this.RequestContext.Trace(tracepoint, level, this.TraceArea, this.m_traceLayer, tags, format, args);
      else
        TeamFoundationTracingService.TraceRaw(tracepoint, level, this.TraceArea, this.m_traceLayer, tags, format, args);
    }

    protected virtual void TraceConditionally(
      int tracepoint,
      TraceLevel level,
      Func<string> message)
    {
      if (this.RequestContext != null)
        this.RequestContext.TraceConditionally(tracepoint, level, this.TraceArea, this.m_traceLayer, message);
      else
        TeamFoundationTracingService.TraceRawConditionally(tracepoint, level, this.TraceArea, this.m_traceLayer, message);
    }

    protected virtual void TraceException(int tracepoint, Exception exception, TraceLevel level = TraceLevel.Error)
    {
      if (this.RequestContext != null)
        this.RequestContext.TraceException(tracepoint, level, this.TraceArea, this.m_traceLayer, exception);
      else
        TeamFoundationTracingService.TraceExceptionRaw(tracepoint, this.TraceArea, this.m_traceLayer, exception);
    }

    protected virtual void TraceEnter(int tracepoint, [CallerMemberName] string methodName = null)
    {
      if (this.RequestContext != null)
        this.RequestContext.TraceEnter(tracepoint, this.TraceArea, this.m_traceLayer, methodName);
      else
        TeamFoundationTracingService.TraceEnterRaw(tracepoint, this.TraceArea, this.m_traceLayer, methodName, new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
    }

    protected virtual void TraceLeave(int tracepoint, [CallerMemberName] string methodName = null)
    {
      if (this.RequestContext != null)
        this.RequestContext.TraceLeave(tracepoint, this.TraceArea, this.m_traceLayer, methodName);
      else
        TeamFoundationTracingService.TraceLeaveRaw(tracepoint, this.TraceArea, this.m_traceLayer, methodName);
    }

    protected virtual void TraceAlways(int tracepoint, TraceLevel traceLevel, string message) => this.TraceAlways(tracepoint, traceLevel, message, (object[]) null);

    protected virtual void TraceAlways(
      int tracepoint,
      TraceLevel traceLevel,
      string format,
      params object[] args)
    {
      if (this.RequestContext != null)
        this.RequestContext.TraceAlways(tracepoint, traceLevel, this.TraceArea, this.m_traceLayer, format, args);
      else
        TeamFoundationTracingService.TraceRawAlwaysOn(tracepoint, traceLevel, this.TraceArea, this.m_traceLayer, format, args);
    }

    protected virtual bool IsTracing(int tracepoint, TraceLevel traceLevel, string[] tags = null) => this.RequestContext != null ? this.RequestContext.IsTracing(tracepoint, traceLevel, this.TraceArea, this.m_traceLayer, tags) : TeamFoundationTracingService.IsRawTracingEnabled(tracepoint, traceLevel, this.TraceArea, this.m_traceLayer, tags);
  }
}
