// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Timers.TimerWheelTimerCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.Azure.Cosmos.Timers
{
  internal sealed class TimerWheelTimerCore : TimerWheelTimer
  {
    private static readonly object completedObject = new object();
    private readonly TaskCompletionSource<object> taskCompletionSource;
    private readonly object memberLock;
    private readonly TimerWheel timerWheel;
    private bool timerStarted;

    internal TimerWheelTimerCore(TimeSpan timeoutPeriod, TimerWheel timerWheel)
    {
      if (timeoutPeriod.Ticks == 0L)
        throw new ArgumentOutOfRangeException(nameof (timeoutPeriod));
      this.timerWheel = timerWheel ?? throw new ArgumentNullException(nameof (timerWheel));
      this.Timeout = timeoutPeriod;
      this.taskCompletionSource = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
      this.memberLock = new object();
    }

    public override TimeSpan Timeout { get; }

    public override Task StartTimerAsync()
    {
      lock (this.memberLock)
      {
        if (this.timerStarted)
          throw new InvalidOperationException("Timer Already Started");
        this.timerWheel.SubscribeForTimeouts((TimerWheelTimer) this);
        this.timerStarted = true;
        return (Task) this.taskCompletionSource.Task;
      }
    }

    public override bool CancelTimer() => this.taskCompletionSource.TrySetCanceled();

    public override bool FireTimeout() => this.taskCompletionSource.TrySetResult(TimerWheelTimerCore.completedObject);
  }
}
