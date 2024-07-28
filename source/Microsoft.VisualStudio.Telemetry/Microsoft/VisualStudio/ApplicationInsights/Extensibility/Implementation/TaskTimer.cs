// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.TaskTimer
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation
{
  internal class TaskTimer : IDisposable
  {
    public static readonly TimeSpan InfiniteTimeSpan = new TimeSpan(0, 0, 0, 0, -1);
    private TimeSpan delay = TimeSpan.FromMinutes(1.0);
    private CancellationTokenSource tokenSource;

    public TimeSpan Delay
    {
      get => this.delay;
      set
      {
        if ((value <= TimeSpan.Zero || value.TotalMilliseconds > (double) int.MaxValue) && value != TaskTimer.InfiniteTimeSpan)
          throw new ArgumentOutOfRangeException(nameof (value));
        this.delay = value;
      }
    }

    public bool IsStarted => this.tokenSource != null;

    public void Start(Func<Task> elapsed)
    {
      CancellationTokenSource newTokenSource = new CancellationTokenSource();
      Task.Delay(this.Delay, newTokenSource.Token).ContinueWith<Task>((Func<Task, Task>) (async previousTask =>
      {
        TaskTimer.CancelAndDispose(Interlocked.CompareExchange<CancellationTokenSource>(ref this.tokenSource, (CancellationTokenSource) null, newTokenSource));
        try
        {
          await elapsed();
        }
        catch (Exception ex)
        {
          if (ex is AggregateException)
          {
            foreach (Exception innerException in ((AggregateException) ex).InnerExceptions)
              CoreEventSource.Log.LogError(innerException.ToString());
          }
          CoreEventSource.Log.LogError(ex.ToString());
        }
      }), CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
      TaskTimer.CancelAndDispose(Interlocked.Exchange<CancellationTokenSource>(ref this.tokenSource, newTokenSource));
    }

    public void Cancel() => TaskTimer.CancelAndDispose(Interlocked.Exchange<CancellationTokenSource>(ref this.tokenSource, (CancellationTokenSource) null));

    public void Dispose() => this.Cancel();

    private static void CancelAndDispose(CancellationTokenSource tokenSource)
    {
      if (tokenSource == null)
        return;
      tokenSource.Cancel();
      tokenSource.Dispose();
    }
  }
}
