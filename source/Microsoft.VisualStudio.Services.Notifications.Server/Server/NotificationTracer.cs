// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationTracer
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class NotificationTracer : INotificationTrace
  {
    private IVssRequestContext m_requestContext;
    private string m_area;
    private string m_layer;
    private Dictionary<int, bool> m_traceEnabledCache;
    private Dictionary<string, bool> m_traceTagsEnabledCache;

    public NotificationTracer(IVssRequestContext requestContext, string area, string layer)
    {
      this.m_requestContext = requestContext;
      this.m_area = area;
      this.m_layer = layer;
      this.m_traceEnabledCache = new Dictionary<int, bool>();
      this.m_traceTagsEnabledCache = new Dictionary<string, bool>();
    }

    public bool IsTracing(int tracepoint, TraceLevel level, string[] tags = null) => this.m_requestContext.IsTracing(tracepoint, level, this.m_area, this.m_layer, tags);

    public void Trace(int tracepoint, TraceLevel level, string message) => this.m_requestContext.Trace(tracepoint, level, this.m_area, this.m_layer, message);

    public void Trace(int tracepoint, TraceLevel level, string format, params object[] args) => VssRequestContextExtensions.Trace(this.m_requestContext, tracepoint, level, this.m_area, this.m_layer, format, args);

    public void Trace(
      int tracepoint,
      TraceLevel level,
      string[] tags,
      string format,
      params object[] args)
    {
      this.m_requestContext.Trace(tracepoint, level, this.m_area, this.m_layer, tags, format, args);
    }

    public void TraceEnter(int tracepoint, [CallerMemberName] string methodName = null) => this.m_requestContext.TraceEnter(tracepoint, this.m_area, this.m_layer, methodName);

    public void TraceException(int tracepoint, Exception exception) => this.m_requestContext.TraceException(tracepoint, this.m_area, this.m_layer, exception);

    public void TraceLeave(int tracepoint, [CallerMemberName] string methodName = null) => this.m_requestContext.TraceLeave(tracepoint, this.m_area, this.m_layer, methodName);

    public void TraceMessage(int tracepoint, string message, params object[] args) => VssRequestContextExtensions.Trace(this.m_requestContext, tracepoint, TraceLevel.Info, this.m_area, this.m_layer, message, args);

    public void TraceAlways(int tracepoint, string message) => this.m_requestContext.TraceAlways(tracepoint, TraceLevel.Info, this.m_area, this.m_layer, message);

    public void LogMessage(string message)
    {
    }

    public void LogMessage(string message, params object[] args)
    {
    }

    public void TraceWarning(int tracepoint, string message) => this.m_requestContext.Trace(tracepoint, TraceLevel.Warning, this.m_area, this.m_layer, message);

    public void TraceError(int tracepoint, string message) => this.m_requestContext.Trace(tracepoint, TraceLevel.Error, this.m_area, this.m_layer, message);

    public void TraceException(int tracepoint, Exception exception, string message) => this.m_requestContext.TraceExceptionMsg(tracepoint, this.m_area, this.m_layer, exception, message);

    public void LogWarning(string message)
    {
    }

    public void LogError(string message)
    {
    }
  }
}
