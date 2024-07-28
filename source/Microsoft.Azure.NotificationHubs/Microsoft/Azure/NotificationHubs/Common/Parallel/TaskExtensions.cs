// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.Parallel.TaskExtensions
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.NotificationHubs.Common.Parallel
{
  internal static class TaskExtensions
  {
    public static Task Then(this Task task, Func<Task> next)
    {
      if (task == null)
        throw new ArgumentNullException(nameof (task));
      if (next == null)
        throw new ArgumentNullException(nameof (next));
      TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
      task.ContinueWith((Action<Task>) delegate
      {
        if (task.IsFaulted)
          tcs.TrySetException((IEnumerable<Exception>) task.Exception.InnerExceptions);
        else if (task.IsCanceled)
        {
          tcs.TrySetCanceled();
        }
        else
        {
          try
          {
            next().ContinueWith<bool>((Func<Task, bool>) (t => tcs.TrySetFromTask<object>(t)), TaskScheduler.Default);
          }
          catch (Exception ex)
          {
            tcs.TrySetException(ex);
          }
        }
      }, TaskScheduler.Default);
      return (Task) tcs.Task;
    }

    public static Task Then<T>(this Task<T> task, Func<T, Task> next)
    {
      if (task == null)
        throw new ArgumentNullException(nameof (task));
      if (next == null)
        throw new ArgumentNullException(nameof (next));
      TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
      task.ContinueWith((Action<Task<T>>) delegate
      {
        if (task.IsFaulted)
          tcs.TrySetException((IEnumerable<Exception>) task.Exception.InnerExceptions);
        else if (task.IsCanceled)
        {
          tcs.TrySetCanceled();
        }
        else
        {
          try
          {
            next(task.Result).ContinueWith<bool>((Func<Task, bool>) (t => tcs.TrySetFromTask<object>(t)), TaskScheduler.Default);
          }
          catch (Exception ex)
          {
            tcs.TrySetException(ex);
          }
        }
      }, TaskScheduler.Default);
      return (Task) tcs.Task;
    }

    public static Task<TResult> Then<TResult>(this Task task, Func<Task<TResult>> next)
    {
      if (task == null)
        throw new ArgumentNullException(nameof (task));
      if (next == null)
        throw new ArgumentNullException(nameof (next));
      TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
      task.ContinueWith((Action<Task>) delegate
      {
        if (task.IsFaulted)
          tcs.TrySetException((IEnumerable<Exception>) task.Exception.InnerExceptions);
        else if (task.IsCanceled)
        {
          tcs.TrySetCanceled();
        }
        else
        {
          try
          {
            next().ContinueWith<bool>((Func<Task<TResult>, bool>) (t => tcs.TrySetFromTask<TResult>((Task) t)), TaskScheduler.Default);
          }
          catch (Exception ex)
          {
            tcs.TrySetException(ex);
          }
        }
      }, TaskScheduler.Default);
      return tcs.Task;
    }

    public static Task<TNewResult> Then<TResult, TNewResult>(
      this Task<TResult> task,
      Func<TResult, Task<TNewResult>> next)
    {
      if (task == null)
        throw new ArgumentNullException(nameof (task));
      if (next == null)
        throw new ArgumentNullException(nameof (next));
      TaskCompletionSource<TNewResult> tcs = new TaskCompletionSource<TNewResult>();
      task.ContinueWith((Action<Task<TResult>>) delegate
      {
        if (task.IsFaulted)
          tcs.TrySetException((IEnumerable<Exception>) task.Exception.InnerExceptions);
        else if (task.IsCanceled)
        {
          tcs.TrySetCanceled();
        }
        else
        {
          try
          {
            next(task.Result).ContinueWith<bool>((Func<Task<TNewResult>, bool>) (t => tcs.TrySetFromTask<TNewResult>((Task) t)), TaskScheduler.Default);
          }
          catch (Exception ex)
          {
            tcs.TrySetException(ex);
          }
        }
      }, TaskScheduler.Default);
      return tcs.Task;
    }
  }
}
