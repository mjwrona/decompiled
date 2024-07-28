// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Channels.TaskAsyncHelper
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Client.Channels
{
  internal static class TaskAsyncHelper
  {
    private static readonly Task s_completedTask = (Task) Task.FromResult<object>((object) null);

    internal static IAsyncResult BeginTask<TResult>(
      Func<Task<TResult>> taskFunc,
      AsyncCallback callback,
      object state)
    {
      Task<TResult> task = taskFunc();
      if (task == null)
        return (IAsyncResult) null;
      TaskWrapperAsyncResult<TResult> resultToReturn = new TaskWrapperAsyncResult<TResult>(task, state);
      bool isCompleted = task.IsCompleted;
      if (isCompleted)
        resultToReturn.CompletedSynchronously = true;
      if (callback != null)
      {
        if (isCompleted)
          callback((IAsyncResult) resultToReturn);
        else
          task.ContinueWith((Action<Task<TResult>>) (_ => callback((IAsyncResult) resultToReturn)));
      }
      return (IAsyncResult) resultToReturn;
    }

    internal static T EndTask<T>(IAsyncResult ar)
    {
      if (ar == null)
        throw new ArgumentNullException(nameof (ar));
      return ar is TaskWrapperAsyncResult<T> wrapperAsyncResult ? wrapperAsyncResult.Task.GetAwaiter().GetResult() : throw new ArgumentException(TFCommonResources.InvalidAsynchronousOperationParameter((object) nameof (ar)), nameof (ar));
    }

    internal static Task CompletedTask => TaskAsyncHelper.s_completedTask;
  }
}
