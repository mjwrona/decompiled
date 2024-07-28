// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.CurrentThreadTaskScheduler
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation
{
  internal sealed class CurrentThreadTaskScheduler : TaskScheduler
  {
    public static readonly TaskScheduler Instance = (TaskScheduler) new CurrentThreadTaskScheduler();

    public override int MaximumConcurrencyLevel => 1;

    protected override void QueueTask(Task task) => this.TryExecuteTask(task);

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => this.TryExecuteTask(task);

    protected override IEnumerable<Task> GetScheduledTasks() => Enumerable.Empty<Task>();
  }
}
