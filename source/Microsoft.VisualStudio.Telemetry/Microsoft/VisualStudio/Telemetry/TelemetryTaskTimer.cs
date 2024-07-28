// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryTaskTimer
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryTaskTimer : TelemetryDisposableObject
  {
    public static readonly TimeSpan InfiniteTimeSpan = new TimeSpan(0, 0, 0, 0, -1);
    private TimeSpan delay;
    private CancellationTokenSource tokenSource;
    private Task delayTask;
    private Task currentTask;

    public TimeSpan Delay
    {
      get => this.delay;
      set
      {
        if ((value <= TimeSpan.Zero || value.TotalMilliseconds > (double) int.MaxValue) && value != TelemetryTaskTimer.InfiniteTimeSpan)
          throw new ArgumentOutOfRangeException(nameof (value));
        this.delay = value;
      }
    }

    public bool IsStarted => this.currentTask != null && !this.currentTask.IsCompleted;

    public TelemetryTaskTimer(TimeSpan taskDelay) => this.Delay = taskDelay;

    public void Start(Action elapsed, bool infinite = false)
    {
      CancellationTokenSource newTokenSource = new CancellationTokenSource();
      this.delayTask = Task.Delay(this.Delay, newTokenSource.Token);
      this.currentTask = this.delayTask.ContinueWith((Action<Task>) (previousTask =>
      {
        TelemetryTaskTimer.CancelAndDispose(Interlocked.CompareExchange<CancellationTokenSource>(ref this.tokenSource, (CancellationTokenSource) null, newTokenSource));
        if (infinite)
          this.Start(elapsed, true);
        elapsed();
      }), CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
      TelemetryTaskTimer.CancelAndDispose(Interlocked.Exchange<CancellationTokenSource>(ref this.tokenSource, newTokenSource));
    }

    public void Start(Func<Task> elapsed, bool infinite = false)
    {
      Task task;
      this.Start((Action) (() => task = elapsed()), infinite);
    }

    public void Cancel() => TelemetryTaskTimer.CancelAndDispose(Interlocked.Exchange<CancellationTokenSource>(ref this.tokenSource, (CancellationTokenSource) null));

    public void WaitThenCancel()
    {
      if (this.delayTask != null && this.currentTask != null && this.delayTask.IsCompleted)
        this.currentTask.Wait();
      this.Cancel();
    }

    protected override void DisposeManagedResources() => this.Cancel();

    private static void CancelAndDispose(CancellationTokenSource tokenSource)
    {
      if (tokenSource == null)
        return;
      tokenSource.Cancel();
      tokenSource.Dispose();
    }
  }
}
