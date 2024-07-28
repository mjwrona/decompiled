// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.Utils.TaskExtensions
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.Utils
{
  internal static class TaskExtensions
  {
    internal static async Task<T> WithCancellation<T>(
      this Task<T> task,
      CancellationToken cancellationToken)
    {
      TaskCompletionSource<bool> state = new TaskCompletionSource<bool>();
      CancellationTokenRegistration tokenRegistration = cancellationToken.Register((Action<object>) (taskCompletionSource => ((TaskCompletionSource<bool>) taskCompletionSource).TrySetResult(true)), (object) state);
      try
      {
        object obj = (object) task;
        Task[] taskArray = new Task[2]
        {
          (Task) task,
          (Task) state.Task
        };
        if (obj != await Task.WhenAny(taskArray).ConfigureAwait(false))
        {
          obj = (object) null;
          throw new OperationCanceledException(cancellationToken);
        }
      }
      finally
      {
        tokenRegistration.Dispose();
      }
      tokenRegistration = new CancellationTokenRegistration();
      return await task.ConfigureAwait(false);
    }

    internal static async Task WithCancellation(this Task task, CancellationToken cancellationToken)
    {
      TaskCompletionSource<bool> state = new TaskCompletionSource<bool>();
      CancellationTokenRegistration tokenRegistration = cancellationToken.Register((Action<object>) (taskCompletionSource => ((TaskCompletionSource<bool>) taskCompletionSource).TrySetResult(true)), (object) state);
      try
      {
        object obj = (object) task;
        Task[] taskArray = new Task[2]
        {
          task,
          (Task) state.Task
        };
        if (obj != await Task.WhenAny(taskArray).ConfigureAwait(false))
        {
          obj = (object) null;
          throw new OperationCanceledException(cancellationToken);
        }
      }
      finally
      {
        tokenRegistration.Dispose();
      }
      tokenRegistration = new CancellationTokenRegistration();
      await task.ConfigureAwait(false);
    }
  }
}
