// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.TaskAsyncResult
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.NotificationHubs.Common
{
  internal sealed class TaskAsyncResult : AsyncResult<TaskAsyncResult>
  {
    private readonly Task task;

    public TaskAsyncResult(Task task, AsyncCallback callback, object state)
      : base(callback, state)
    {
      this.task = task;
      if (this.task.IsCompleted)
        this.CompleteWithTaskResult(true);
      else
        this.task.ContinueWith(new Action<Task>(this.OnTaskContinued));
    }

    private void OnTaskContinued(Task unused) => this.CompleteWithTaskResult(false);

    private void CompleteWithTaskResult(bool completedSynchronously)
    {
      Exception e;
      if (this.task.Exception != null)
      {
        e = (Exception) this.task.Exception;
        if (e is AggregateException aggregateException)
          e = aggregateException.GetBaseException();
      }
      else
        e = (Exception) null;
      this.Complete(completedSynchronously, e);
    }
  }
}
