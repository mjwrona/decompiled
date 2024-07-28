// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.Parallel.TaskCompletionSourceExtensions
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.NotificationHubs.Common.Parallel
{
  internal static class TaskCompletionSourceExtensions
  {
    public static bool TrySetFromTask<TResult>(
      this TaskCompletionSource<TResult> resultSetter,
      Task task)
    {
      switch (task.Status)
      {
        case TaskStatus.RanToCompletion:
          return resultSetter.TrySetResult(task is Task<TResult> ? ((Task<TResult>) task).Result : default (TResult));
        case TaskStatus.Canceled:
          return resultSetter.TrySetCanceled();
        case TaskStatus.Faulted:
          return resultSetter.TrySetException((IEnumerable<Exception>) task.Exception.InnerExceptions);
        default:
          throw new InvalidOperationException("The task was not completed.");
      }
    }
  }
}
