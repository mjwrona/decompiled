// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.PooledTimer
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class PooledTimer
  {
    private long beginTicks;
    private TimeSpan timeoutPeriod;
    private TimerPool timerPool;
    private readonly TaskCompletionSource<object> tcs;
    private readonly object memberLock;
    private bool timerStarted;

    public PooledTimer(int timeout, TimerPool timerPool)
    {
      this.timeoutPeriod = TimeSpan.FromSeconds((double) timeout);
      this.tcs = new TaskCompletionSource<object>();
      this.timerPool = timerPool;
      this.memberLock = new object();
    }

    public long TimeoutTicks => this.beginTicks + this.Timeout.Ticks;

    public TimeSpan Timeout
    {
      get => this.timeoutPeriod;
      set => this.timeoutPeriod = value;
    }

    public Task StartTimerAsync()
    {
      lock (this.memberLock)
      {
        if (this.timerStarted)
          throw new InvalidOperationException("Timer Already Started");
        this.beginTicks = this.timerPool.SubscribeForTimeouts(this);
        this.timerStarted = true;
        return (Task) this.tcs.Task;
      }
    }

    public bool CancelTimer() => this.tcs.TrySetCanceled();

    internal bool FireTimeout() => this.tcs.TrySetResult((object) null);
  }
}
