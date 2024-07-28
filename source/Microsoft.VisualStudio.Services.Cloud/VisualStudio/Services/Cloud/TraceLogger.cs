// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.TraceLogger
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class TraceLogger : ITFLogger
  {
    private const int c_tracepoint = 15080300;
    private readonly string m_area;
    private readonly string m_layer;
    private readonly ITraceRequest m_tracer;

    public TraceLogger(IVssRequestContext requestContext, string area, string layer)
      : this(requestContext.RequestTracer, area, layer)
    {
    }

    public TraceLogger(ITraceRequest tracer, string area, string layer)
    {
      this.m_tracer = tracer;
      this.m_area = area;
      this.m_layer = layer;
    }

    public virtual void Info(string message) => this.m_tracer.TraceAlways(15080300, TraceLevel.Info, this.m_area, this.m_layer, message);

    public virtual void Info(string message, params object[] args) => this.m_tracer.TraceAlways(15080300, TraceLevel.Info, this.m_area, this.m_layer, message, args);

    public virtual void Warning(string message) => this.m_tracer.TraceAlways(15080300, TraceLevel.Warning, this.m_area, this.m_layer, message);

    public virtual void Warning(string message, params object[] args) => this.m_tracer.TraceAlways(15080300, TraceLevel.Warning, this.m_area, this.m_layer, message, args);

    public virtual void Warning(Exception exception) => this.m_tracer.TraceException(15080300, TraceLevel.Warning, this.m_area, this.m_layer, exception);

    public virtual void Error(string message) => this.m_tracer.TraceAlways(15080300, TraceLevel.Error, this.m_area, this.m_layer, message);

    public virtual void Error(string message, params object[] args) => this.m_tracer.TraceAlways(15080300, TraceLevel.Error, this.m_area, this.m_layer, message, args);

    public virtual void Error(Exception exception) => this.m_tracer.TraceException(15080300, TraceLevel.Error, this.m_area, this.m_layer, exception);
  }
}
