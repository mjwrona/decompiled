// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Common.TaskHelpers
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Microsoft.AspNet.OData.Common
{
  internal static class TaskHelpers
  {
    private static readonly Task _defaultCompleted = (Task) Task.FromResult<TaskHelpers.AsyncVoid>(new TaskHelpers.AsyncVoid());

    internal static Task Canceled() => (Task) TaskHelpers.CancelCache<TaskHelpers.AsyncVoid>.Canceled;

    internal static Task Completed() => TaskHelpers._defaultCompleted;

    internal static Task FromError(Exception exception) => (Task) TaskHelpers.FromError<TaskHelpers.AsyncVoid>(exception);

    internal static Task<TResult> FromError<TResult>(Exception exception)
    {
      TaskCompletionSource<TResult> completionSource = new TaskCompletionSource<TResult>();
      completionSource.SetException(exception);
      return completionSource.Task;
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    private struct AsyncVoid
    {
    }

    private static class CancelCache<TResult>
    {
      public static readonly Task<TResult> Canceled = TaskHelpers.CancelCache<TResult>.GetCancelledTask();

      private static Task<TResult> GetCancelledTask()
      {
        TaskCompletionSource<TResult> completionSource = new TaskCompletionSource<TResult>();
        completionSource.SetCanceled();
        return completionSource.Task;
      }
    }
  }
}
