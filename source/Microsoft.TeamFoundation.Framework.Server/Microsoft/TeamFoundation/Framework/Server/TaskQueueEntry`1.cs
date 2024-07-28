// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TaskQueueEntry`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TaskQueueEntry<T> : IComparable<TaskQueueEntry<T>>
  {
    public TaskQueueEntry(TeamFoundationTask<T> task, long taskId)
      : this(task, taskId, new DateTime?())
    {
    }

    internal TaskQueueEntry(TeamFoundationTask<T> task, long taskId, DateTime? queueTime)
    {
      this.Task = task;
      this.QueueTime = queueTime ?? task.GetNextRunTime();
      this.TaskId = taskId;
    }

    public TeamFoundationTask<T> Task { get; set; }

    public DateTime QueueTime { get; set; }

    internal long TaskId { get; private set; }

    public int CompareTo(TaskQueueEntry<T> other)
    {
      if (this.QueueTime == other.QueueTime)
        return this.TaskId.CompareTo(other.TaskId);
      return !(this.QueueTime < other.QueueTime) ? 1 : -1;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\r\n{0}\r\nQueue Time: {1}", (object) this.Task, (object) this.QueueTime);
  }
}
