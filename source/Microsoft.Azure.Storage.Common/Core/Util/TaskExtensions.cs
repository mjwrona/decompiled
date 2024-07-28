// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.TaskExtensions
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Core.Util
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
          task.ContinueWith((Action<Task<T>>) (val => val.Exception.Handle((Func<Exception, bool>) (ex => true))), TaskContinuationOptions.OnlyOnFaulted);
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
          task.ContinueWith((Action<Task>) (val => val.Exception.Handle((Func<Exception, bool>) (ex => true))), TaskContinuationOptions.OnlyOnFaulted);
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
