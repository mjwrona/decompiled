// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Tracing.OrchestrationTraceListener
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Orchestration.Tracing
{
  internal class OrchestrationTraceListener : TraceListener
  {
    private readonly IVssRequestContext m_requestContext;

    public OrchestrationTraceListener(IVssRequestContext requestContext) => this.m_requestContext = requestContext;

    public override bool IsThreadSafe => true;

    public Action<TraceEventType, string> GeneralEvent { get; set; }

    public Action<TraceEventType, string, string> SessionEvent { get; set; }

    public Action<TraceEventType, string, string, string> InstanceEvent { get; set; }

    public override void TraceData(
      TraceEventCache eventCache,
      string source,
      TraceEventType eventType,
      int id,
      object data)
    {
      if (this.Filter != null && !this.Filter.ShouldTrace(eventCache, source, eventType, id, (string) null, (object[]) null, data, (object[]) null) || id < 1 || id > 5)
        return;
      Action<TraceEventType, string> generalEvent = this.GeneralEvent;
      if (generalEvent == null)
        return;
      generalEvent(eventType, (string) data);
    }

    public override void TraceData(
      TraceEventCache eventCache,
      string source,
      TraceEventType eventType,
      int id,
      params object[] data)
    {
      if (this.Filter != null && !this.Filter.ShouldTrace(eventCache, source, eventType, id, (string) null, (object[]) null, (object) null, data))
        return;
      if (id >= 1 && id <= 5)
      {
        Action<TraceEventType, string> generalEvent = this.GeneralEvent;
        if (generalEvent != null)
          generalEvent(eventType, (string) data[0]);
      }
      if (id >= 6 && id <= 10)
      {
        Action<TraceEventType, string, string> sessionEvent = this.SessionEvent;
        if (sessionEvent != null)
          sessionEvent(eventType, (string) data[0], (string) data[1]);
      }
      if (id < 11 || id > 15)
        return;
      Action<TraceEventType, string, string, string> instanceEvent = this.InstanceEvent;
      if (instanceEvent == null)
        return;
      instanceEvent(eventType, (string) data[0], (string) data[1], (string) data[2]);
    }

    public override void Write(string message)
    {
    }

    public override void WriteLine(string message)
    {
    }
  }
}
