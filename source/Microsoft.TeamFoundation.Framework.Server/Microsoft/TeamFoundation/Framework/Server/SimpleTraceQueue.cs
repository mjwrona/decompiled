// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SimpleTraceQueue
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SimpleTraceQueue
  {
    private object m_lock = new object();
    private Queue<SimpleTraceQueueMessage> m_msgs = new Queue<SimpleTraceQueueMessage>();

    public void Trace(int tracepoint, TraceLevel level, string format, params object[] args) => this.Enqueue(new SimpleTraceQueueMessage()
    {
      Tracepoint = tracepoint,
      Level = level,
      Message = string.Format(format, args)
    });

    public void TracePendingMessages(IVssRequestContext requestContext, string area, string layer)
    {
      Queue<SimpleTraceQueueMessage> msgs;
      lock (this.m_lock)
      {
        msgs = this.m_msgs;
        this.m_msgs = new Queue<SimpleTraceQueueMessage>();
      }
      foreach (SimpleTraceQueueMessage msg in msgs)
        this.TraceMessage(requestContext, area, layer, msg);
    }

    protected virtual void TraceMessage(
      IVssRequestContext requestContext,
      string area,
      string layer,
      SimpleTraceQueueMessage msg)
    {
      requestContext.Trace(msg.Tracepoint, msg.Level, area, layer, msg.Message);
    }

    private void Enqueue(SimpleTraceQueueMessage msg)
    {
      lock (this.m_lock)
        this.m_msgs.Enqueue(msg);
    }

    internal void TracePendingMessages(
      IVssRequestContext requestContext,
      string area,
      object layer)
    {
      throw new NotImplementedException();
    }
  }
}
