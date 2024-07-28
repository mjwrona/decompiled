// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Async.TaskSchedulerExtensions
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration.Async
{
  public static class TaskSchedulerExtensions
  {
    public static SynchronizationContext ToSynchronizationContext(this TaskScheduler scheduler) => (SynchronizationContext) new TaskSchedulerExtensions.TaskSchedulerSynchronizationContext(scheduler);

    private sealed class TaskSchedulerSynchronizationContext : SynchronizationContext
    {
      private TaskScheduler _scheduler;

      internal TaskSchedulerSynchronizationContext(TaskScheduler scheduler) => this._scheduler = scheduler != null ? scheduler : throw new ArgumentNullException(nameof (scheduler));

      public override void Post(SendOrPostCallback d, object state) => Task.Factory.StartNew((Action) (() => d(state)), CancellationToken.None, TaskCreationOptions.None, this._scheduler);

      public override void Send(SendOrPostCallback d, object state)
      {
        Task task = new Task((Action) (() => d(state)));
        task.RunSynchronously(this._scheduler);
        task.Wait();
      }
    }
  }
}
