// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Async.CurrentThreadTaskScheduler
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration.Async
{
  public class CurrentThreadTaskScheduler : TaskScheduler
  {
    protected override void QueueTask(Task task) => this.TryExecuteTask(task);

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => this.TryExecuteTask(task);

    protected override IEnumerable<Task> GetScheduledTasks() => Enumerable.Empty<Task>();

    public override int MaximumConcurrencyLevel => 1;
  }
}
