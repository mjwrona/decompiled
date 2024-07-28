// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CurrentTaskDetails`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class CurrentTaskDetails<T>
  {
    public CurrentTaskDetails(long taskId, DateTime queueTime, TaskQueueEntry<T> task)
    {
      this.QueueTime = queueTime;
      this.TaskId = taskId;
      this.Task = task;
      this.StartTime = DateTime.UtcNow;
    }

    internal DateTime StartTime { get; }

    internal DateTime QueueTime { get; }

    internal long TaskId { get; }

    internal TaskQueueEntry<T> Task { get; }

    public override string ToString() => string.Format("The taskID:{0} with taskDetails:{1} had StartTime:{2} & QueueTime:{3}", (object) this.TaskId, (object) this.Task, (object) this.StartTime, (object) this.QueueTime);
  }
}
