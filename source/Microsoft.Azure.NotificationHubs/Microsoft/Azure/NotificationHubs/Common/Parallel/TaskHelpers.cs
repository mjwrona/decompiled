// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.Parallel.TaskHelpers
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.NotificationHubs.Common.Parallel
{
  internal static class TaskHelpers
  {
    private const string TimeoutExceptionExtensionData = "TaskExtension.WithTimeout";

    public static Task CreateTask(
      Func<AsyncCallback, object, IAsyncResult> begin,
      Action<IAsyncResult> end)
    {
      try
      {
        return Task.Factory.FromAsync(begin, end, (object) null);
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
        {
          throw;
        }
        else
        {
          TaskCompletionSource<object> completionSource = new TaskCompletionSource<object>();
          completionSource.SetException(ex);
          return (Task) completionSource.Task;
        }
      }
    }

    public static Task<T> CreateTask<T>(
      Func<AsyncCallback, object, IAsyncResult> begin,
      Func<IAsyncResult, T> end)
    {
      try
      {
        return Task<T>.Factory.FromAsync(begin, end, (object) null);
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
        {
          throw;
        }
        else
        {
          TaskCompletionSource<T> completionSource = new TaskCompletionSource<T>();
          completionSource.SetException(ex);
          return completionSource.Task;
        }
      }
    }

    public static Task<TResult> CreateTask<TState, TResult>(
      Func<TState, AsyncCallback, object, IAsyncResult> begin,
      Func<TState, IAsyncResult, TResult> end,
      TState state)
    {
      try
      {
        return Task<TResult>.Factory.FromAsync((Func<AsyncCallback, object, IAsyncResult>) ((c, s) => begin((TState) s, c, s)), (Func<IAsyncResult, TResult>) (a => end((TState) a.AsyncState, a)), (object) state);
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
        {
          throw;
        }
        else
        {
          TaskCompletionSource<TResult> completionSource = new TaskCompletionSource<TResult>((object) state);
          completionSource.SetException(ex);
          return completionSource.Task;
        }
      }
    }

    public static void Fork(this Task thisTask) => thisTask.ContinueWith((Action<Task>) (t => Fx.Exception.TraceHandled((Exception) t.Exception, "TaskHelpers.Fork")), TaskContinuationOptions.OnlyOnFaulted);

    public static Task<T> GetCompletedTask<T>(T val = null) where T : class
    {
      if ((object) val == null)
        return CompletedTask<T>.Default;
      TaskCompletionSource<T> completionSource = new TaskCompletionSource<T>();
      completionSource.SetResult(val);
      return completionSource.Task;
    }

    public static Task ExecuteAndGetCompletedTask(Action action)
    {
      TaskCompletionSource<object> completionSource = new TaskCompletionSource<object>();
      try
      {
        action();
        completionSource.SetResult((object) null);
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
          throw;
        else
          completionSource.SetException(ex);
      }
      return (Task) completionSource.Task;
    }

    public static Task<TResult> ExecuteAndGetCompletedTask<TResult>(Func<TResult> function)
    {
      TaskCompletionSource<TResult> completionSource = new TaskCompletionSource<TResult>();
      try
      {
        completionSource.SetResult(function());
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
          throw;
        else
          completionSource.SetException(ex);
      }
      return completionSource.Task;
    }

    public static IAsyncResult ToAsyncResult(this Task task, AsyncCallback callback, object state)
    {
      if (task.AsyncState == state)
      {
        if (callback != null)
          task.ContinueWith((Action<Task>) (t => callback((IAsyncResult) task)), TaskContinuationOptions.ExecuteSynchronously);
        return (IAsyncResult) task;
      }
      TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(state);
      task.ContinueWith((Action<Task>) (_ =>
      {
        if (task.IsFaulted)
          tcs.TrySetException((IEnumerable<Exception>) task.Exception.InnerExceptions);
        else if (task.IsCanceled)
          tcs.TrySetCanceled();
        else
          tcs.TrySetResult((object) null);
        if (callback == null)
          return;
        callback((IAsyncResult) tcs.Task);
      }), TaskContinuationOptions.ExecuteSynchronously);
      return (IAsyncResult) tcs.Task;
    }

    public static IAsyncResult ToAsyncResult<TResult>(
      this Task<TResult> task,
      AsyncCallback callback,
      object state)
    {
      if (task.AsyncState == state)
      {
        if (callback != null)
          task.ContinueWith((Action<Task<TResult>>) (t => callback((IAsyncResult) task)), TaskContinuationOptions.ExecuteSynchronously);
        return (IAsyncResult) task;
      }
      TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>(state);
      task.ContinueWith((Action<Task<TResult>>) (_ =>
      {
        if (task.IsFaulted)
          tcs.TrySetException((IEnumerable<Exception>) task.Exception.InnerExceptions);
        else if (task.IsCanceled)
          tcs.TrySetCanceled();
        else
          tcs.TrySetResult(task.Result);
        if (callback == null)
          return;
        callback((IAsyncResult) tcs.Task);
      }), TaskContinuationOptions.ExecuteSynchronously);
      return (IAsyncResult) tcs.Task;
    }

    public static void EndAsyncResult(IAsyncResult asyncResult)
    {
      if (!(asyncResult is Task task))
        throw Fx.Exception.AsError((Exception) new ArgumentException(Resources.InvalidAsyncResult));
      try
      {
        task.Wait();
      }
      catch (AggregateException ex)
      {
        ExceptionDispatcher.Throw(ex.GetBaseException());
      }
    }

    public static TResult EndAsyncResult<TResult>(IAsyncResult asyncResult)
    {
      if (!(asyncResult is Task<TResult> task))
        throw Fx.Exception.AsError((Exception) new ArgumentException(Resources.InvalidAsyncResult));
      try
      {
        return task.Result;
      }
      catch (AggregateException ex)
      {
        ExceptionDispatcher.Throw(ex.GetBaseException());
        throw ex.GetBaseException();
      }
    }

    public static Task<TResult> WithTimeout<TResult>(
      this Task<TResult> actualTask,
      TimeSpan timeout,
      string taskFriendlyName = "Unnamed",
      Action<TResult, Exception> onCompletionAfterTimeout = null)
    {
      TaskCompletionSource<TResult> returnTcs = new TaskCompletionSource<TResult>(actualTask.AsyncState);
      Timer timer = new Timer((TimerCallback) (state =>
      {
        ((TaskCompletionSource<TResult>) state).TrySetException((Exception) new TimeoutException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The {0} task timed out", new object[1]
        {
          (object) taskFriendlyName
        }))
        {
          Data = {
            {
              (object) "TaskExtension.WithTimeout",
              (object) null
            }
          }
        });
      }), (object) returnTcs, timeout, TimeSpan.FromMilliseconds(-1.0));
      actualTask.ContinueWith((Action<Task<TResult>>) (t =>
      {
        timer.Dispose();
        if (!(!t.IsFaulted ? (!t.IsCanceled ? !returnTcs.TrySetResult(t.Result) : !returnTcs.TrySetCanceled()) : !returnTcs.TrySetException(t.Exception.InnerException)) || onCompletionAfterTimeout == null)
          return;
        if (t.IsFaulted)
          onCompletionAfterTimeout(default (TResult), t.Exception.InnerException);
        else
          onCompletionAfterTimeout(t.Result, (Exception) null);
      }), TaskContinuationOptions.ExecuteSynchronously);
      return returnTcs.Task;
    }
  }
}
