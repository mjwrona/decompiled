// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.TaskAwaiterHelper
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR
{
  internal static class TaskAwaiterHelper
  {
    internal static TaskAwaiterHelper.PreserveCultureAwaiter PreserveCulture(this Task task) => new TaskAwaiterHelper.PreserveCultureAwaiter(task, true);

    internal static TaskAwaiterHelper.PreserveCultureAwaiter PreserveCultureNotContext(
      this Task task)
    {
      return new TaskAwaiterHelper.PreserveCultureAwaiter(task, false);
    }

    internal static TaskAwaiterHelper.PreserveCultureAwaiter<T> PreserveCulture<T>(this Task<T> task) => new TaskAwaiterHelper.PreserveCultureAwaiter<T>(task, true);

    internal static TaskAwaiterHelper.PreserveCultureAwaiter<T> PreserveCultureNotContext<T>(
      this Task<T> task)
    {
      return new TaskAwaiterHelper.PreserveCultureAwaiter<T>(task, false);
    }

    private static void PreserveCultureUnsafeOnCompleted(
      ICriticalNotifyCompletion notifier,
      Action continuation,
      bool useSyncContext)
    {
      if (useSyncContext && SynchronizationContext.Current != null)
      {
        notifier.UnsafeOnCompleted(continuation);
      }
      else
      {
        TaskAsyncHelper.CulturePair preservedCulture = TaskAsyncHelper.SaveCulture();
        notifier.UnsafeOnCompleted((Action) (() => TaskAsyncHelper.RunWithPreservedCulture(preservedCulture, continuation)));
      }
    }

    internal struct PreserveCultureAwaiter : ICriticalNotifyCompletion, INotifyCompletion
    {
      private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _awaiter;
      private readonly bool _useSyncContext;

      public PreserveCultureAwaiter(Task task, bool useSyncContext)
      {
        this._awaiter = task.ConfigureAwait(useSyncContext).GetAwaiter();
        this._useSyncContext = useSyncContext;
      }

      public bool IsCompleted => this._awaiter.IsCompleted;

      public void OnCompleted(Action continuation) => throw new NotImplementedException();

      public void UnsafeOnCompleted(Action continuation) => TaskAwaiterHelper.PreserveCultureUnsafeOnCompleted((ICriticalNotifyCompletion) this._awaiter, continuation, this._useSyncContext);

      public void GetResult() => this._awaiter.GetResult();

      public TaskAwaiterHelper.PreserveCultureAwaiter GetAwaiter() => this;
    }

    internal struct PreserveCultureAwaiter<T> : ICriticalNotifyCompletion, INotifyCompletion
    {
      private readonly ConfiguredTaskAwaitable<T>.ConfiguredTaskAwaiter _awaiter;
      private readonly bool _useSyncContext;

      public PreserveCultureAwaiter(Task<T> task, bool useSyncContext)
      {
        this._awaiter = task.ConfigureAwait(useSyncContext).GetAwaiter();
        this._useSyncContext = useSyncContext;
      }

      public bool IsCompleted => this._awaiter.IsCompleted;

      public void OnCompleted(Action continuation) => throw new NotImplementedException();

      public void UnsafeOnCompleted(Action continuation) => TaskAwaiterHelper.PreserveCultureUnsafeOnCompleted((ICriticalNotifyCompletion) this._awaiter, continuation, this._useSyncContext);

      public T GetResult() => this._awaiter.GetResult();

      public TaskAwaiterHelper.PreserveCultureAwaiter<T> GetAwaiter() => this;
    }
  }
}
