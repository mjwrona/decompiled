// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.AckHandler
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  public class AckHandler : IAckHandler, IDisposable
  {
    private readonly ConcurrentDictionary<string, AckHandler.AckInfo> _acks = new ConcurrentDictionary<string, AckHandler.AckInfo>();
    private readonly TimeSpan _ackThreshold;
    private Timer _timer;

    public AckHandler()
      : this(true, TimeSpan.FromSeconds(30.0), TimeSpan.FromSeconds(5.0))
    {
    }

    public AckHandler(bool completeAcksOnTimeout, TimeSpan ackThreshold, TimeSpan ackInterval)
    {
      if (completeAcksOnTimeout)
        this._timer = new Timer((TimerCallback) (_ => this.CheckAcks()), (object) null, ackInterval, ackInterval);
      this._ackThreshold = ackThreshold;
    }

    public Task CreateAck(string id) => (Task) this._acks.GetOrAdd(id, (Func<string, AckHandler.AckInfo>) (_ => new AckHandler.AckInfo())).Tcs.Task;

    public bool TriggerAck(string id)
    {
      AckHandler.AckInfo ackInfo;
      if (!this._acks.TryRemove(id, out ackInfo))
        return false;
      ackInfo.Tcs.TrySetResult((object) null);
      return true;
    }

    private void CheckAcks()
    {
      foreach (KeyValuePair<string, AckHandler.AckInfo> ack in this._acks)
      {
        AckHandler.AckInfo ackInfo;
        if (DateTime.UtcNow - ack.Value.Created > this._ackThreshold && this._acks.TryRemove(ack.Key, out ackInfo))
          ackInfo.Tcs.TrySetCanceled();
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this._timer != null)
        this._timer.Dispose();
      foreach (KeyValuePair<string, AckHandler.AckInfo> ack in this._acks)
      {
        AckHandler.AckInfo ackInfo;
        if (this._acks.TryRemove(ack.Key, out ackInfo))
          ackInfo.Tcs.TrySetCanceled();
      }
    }

    public void Dispose() => this.Dispose(true);

    private class AckInfo
    {
      public DispatchingTaskCompletionSource<object> Tcs { get; private set; }

      public DateTime Created { get; private set; }

      public AckInfo()
      {
        this.Tcs = new DispatchingTaskCompletionSource<object>();
        this.Created = DateTime.UtcNow;
      }
    }
  }
}
