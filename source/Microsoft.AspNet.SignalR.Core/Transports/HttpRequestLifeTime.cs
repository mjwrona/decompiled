// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Transports.HttpRequestLifeTime
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Transports
{
  internal class HttpRequestLifeTime
  {
    private readonly DispatchingTaskCompletionSource<object> _lifetimeTcs;
    private readonly TransportDisconnectBase _transport;
    private readonly TaskQueue _writeQueue;
    private readonly TraceSource _trace;
    private readonly string _connectionId;

    public HttpRequestLifeTime(
      TransportDisconnectBase transport,
      TaskQueue writeQueue,
      TraceSource trace,
      string connectionId)
    {
      this._lifetimeTcs = new DispatchingTaskCompletionSource<object>();
      this._transport = transport;
      this._trace = trace;
      this._connectionId = connectionId;
      this._writeQueue = writeQueue;
    }

    public Task Task => (Task) this._lifetimeTcs.Task;

    public void Complete() => this.Complete((Exception) null);

    public void Complete(Exception error)
    {
      this._trace.TraceEvent(TraceEventType.Verbose, 0, "DrainWrites(" + this._connectionId + ")");
      HttpRequestLifeTime.LifetimeContext state1 = new HttpRequestLifeTime.LifetimeContext(this._transport, this._lifetimeTcs, error);
      this._transport.ApplyState(TransportConnectionStates.QueueDrained);
      this._writeQueue.Drain().Catch<Task>(this._trace).Finally((Action<object>) (state => ((HttpRequestLifeTime.LifetimeContext) state).Complete()), (object) state1);
      if (error != null)
        this._trace.TraceEvent(TraceEventType.Error, 0, "CompleteRequest (" + this._connectionId + ") failed: " + (object) error.GetBaseException());
      else
        this._trace.TraceInformation("CompleteRequest (" + this._connectionId + ")");
    }

    private class LifetimeContext
    {
      private readonly DispatchingTaskCompletionSource<object> _lifetimeTcs;
      private readonly Exception _error;
      private readonly TransportDisconnectBase _transport;

      public LifetimeContext(
        TransportDisconnectBase transport,
        DispatchingTaskCompletionSource<object> lifeTimetcs,
        Exception error)
      {
        this._transport = transport;
        this._lifetimeTcs = lifeTimetcs;
        this._error = error;
      }

      public void Complete()
      {
        this._transport.ApplyState(TransportConnectionStates.HttpRequestEnded);
        if (this._error != null)
          this._lifetimeTcs.TrySetUnwrappedException(this._error);
        else
          this._lifetimeTcs.TrySetResult((object) null);
      }
    }
  }
}
