// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.Tracing.VssSignalRTraceListener
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Microsoft.VisualStudio.Services.SignalR.Tracing
{
  public class VssSignalRTraceListener : TraceListener
  {
    private const string c_area = "SignalR";
    private readonly IVssSignalRTraceForwarder m_forwarder;
    private readonly ThreadLocal<StringBuilder> _message = new ThreadLocal<StringBuilder>((Func<StringBuilder>) (() => new StringBuilder()));

    public VssSignalRTraceListener()
      : this((IVssSignalRTraceForwarder) new DefaultTraceForwarder())
    {
    }

    public VssSignalRTraceListener(IVssSignalRTraceForwarder forwarder)
    {
      ArgumentUtility.CheckForNull<IVssSignalRTraceForwarder>(forwarder, nameof (forwarder));
      this.m_forwarder = forwarder;
    }

    public override void TraceData(
      TraceEventCache eventCache,
      string source,
      TraceEventType eventType,
      int id,
      object data)
    {
      base.TraceData(eventCache, source, eventType, id, data);
      this.TraceMessage(source, eventType);
    }

    public override void TraceData(
      TraceEventCache eventCache,
      string source,
      TraceEventType eventType,
      int id,
      params object[] data)
    {
      base.TraceData(eventCache, source, eventType, id, data);
      this.TraceMessage(source, eventType);
    }

    public override void TraceEvent(
      TraceEventCache eventCache,
      string source,
      TraceEventType eventType,
      int id)
    {
      base.TraceEvent(eventCache, source, eventType, id);
      this.TraceMessage(source, eventType);
    }

    public override void TraceEvent(
      TraceEventCache eventCache,
      string source,
      TraceEventType eventType,
      int id,
      string message)
    {
      base.TraceEvent(eventCache, source, eventType, id, message);
      this.TraceMessage(source, eventType);
    }

    public override void TraceEvent(
      TraceEventCache eventCache,
      string source,
      TraceEventType eventType,
      int id,
      string format,
      params object[] args)
    {
      base.TraceEvent(eventCache, source, eventType, id, format, args);
      this.TraceMessage(source, eventType);
    }

    public override void Write(string message) => this._message.Value.Append(message);

    public override void WriteLine(string message) => this._message.Value.AppendLine(message);

    private void TraceMessage(string layer, TraceEventType eventType)
    {
      try
      {
        TraceLevel traceLevel = VssSignalRTraceListener.GetTraceLevel(eventType);
        if (this._message.Value.Length <= 0 || traceLevel == TraceLevel.Off)
          return;
        this.TraceMessageInternal(traceLevel, layer, this._message.ToString());
      }
      finally
      {
        this._message.Value.Clear();
      }
    }

    private void TraceMessageInternal(TraceLevel level, string layer, string message) => this.m_forwarder.Trace(10017100, level, "SignalR", layer, message);

    private static TraceLevel GetTraceLevel(TraceEventType eventType)
    {
      switch (eventType)
      {
        case TraceEventType.Critical:
        case TraceEventType.Error:
          return TraceLevel.Error;
        case TraceEventType.Warning:
          return TraceLevel.Warning;
        case TraceEventType.Information:
          return TraceLevel.Info;
        case TraceEventType.Verbose:
          return TraceLevel.Verbose;
        default:
          return TraceLevel.Off;
      }
    }
  }
}
