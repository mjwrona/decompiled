// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryScheduler
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class TelemetryScheduler : ITelemetryScheduler
  {
    internal const int IsInProgressOfProcessingEvent = 1;
    internal const int NotInProgressOfProcessingEvent = 0;
    private int isInProcess;
    private TelemetryTaskTimer taskTimer;

    internal int IsInProgress => this.isInProcess;

    public void Schedule(Func<Task> actionTask, CancellationToken? token = null)
    {
      actionTask.RequiresArgumentNotNull<Func<Task>>(nameof (actionTask));
      if (token.HasValue)
        Task.Run(actionTask, token.Value);
      else
        Task.Run(actionTask);
    }

    public void Schedule(Action action, CancellationToken? token = null)
    {
      action.RequiresArgumentNotNull<Action>(nameof (action));
      if (token.HasValue)
        Task.Run(action, token.Value);
      else
        Task.Run(action);
    }

    public void InitializeTimed(TimeSpan delay) => this.taskTimer = this.taskTimer == null ? new TelemetryTaskTimer(delay) : throw new ArgumentException("cannot initialize twice");

    public void ScheduleTimed(Func<Task> actionTask, bool recurring = false)
    {
      if (this.taskTimer == null)
        throw new InvalidOperationException("Cannot mix usage of scheduler");
      actionTask.RequiresArgumentNotNull<Func<Task>>(nameof (actionTask));
      if (this.taskTimer.IsStarted)
        return;
      this.taskTimer.Start(actionTask, recurring);
    }

    public void ScheduleTimed(Action action, bool recurring = false)
    {
      if (this.taskTimer == null)
        throw new InvalidOperationException("Cannot mix usage of scheduler");
      action.RequiresArgumentNotNull<Action>(nameof (action));
      if (this.taskTimer.IsStarted)
        return;
      this.taskTimer.Start(action, recurring);
    }

    public bool CanEnterTimedDelegate()
    {
      if (this.taskTimer == null)
        throw new InvalidOperationException("Cannot mix usage of scheduler");
      return Interlocked.CompareExchange(ref this.isInProcess, 1, 0) != 1;
    }

    public void ExitTimedDelegate()
    {
      if (this.taskTimer == null)
        throw new InvalidOperationException("Cannot mix usage of scheduler");
      this.isInProcess = this.isInProcess != 0 ? 0 : throw new InvalidOperationException("Cannot exit before enter");
    }

    public void CancelTimed(bool wait = false)
    {
      if (this.taskTimer == null)
        throw new InvalidOperationException("Cannot mix usage of scheduler");
      if (wait)
        this.taskTimer.WaitThenCancel();
      else
        this.taskTimer.Cancel();
    }
  }
}
