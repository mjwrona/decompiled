// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.TaskUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal static class TaskUtils
  {
    private static Task completedTask;

    internal static Task CompletedTask
    {
      get
      {
        if (TaskUtils.completedTask == null)
        {
          TaskCompletionSource<object> completionSource = new TaskCompletionSource<object>();
          completionSource.SetResult((object) null);
          TaskUtils.completedTask = (Task) completionSource.Task;
        }
        return TaskUtils.completedTask;
      }
    }

    internal static Task<T> GetCompletedTask<T>(T value)
    {
      TaskCompletionSource<T> completionSource = new TaskCompletionSource<T>();
      completionSource.SetResult(value);
      return completionSource.Task;
    }

    internal static Task GetFaultedTask(Exception exception) => (Task) TaskUtils.GetFaultedTask<object>(exception);

    internal static Task<T> GetFaultedTask<T>(Exception exception)
    {
      TaskCompletionSource<T> completionSource = new TaskCompletionSource<T>();
      completionSource.SetException(exception);
      return completionSource.Task;
    }

    internal static Task GetTaskForSynchronousOperation(Action synchronousOperation)
    {
      try
      {
        synchronousOperation();
        return TaskUtils.CompletedTask;
      }
      catch (Exception ex)
      {
        if (ExceptionUtils.IsCatchableExceptionType(ex))
          return TaskUtils.GetFaultedTask(ex);
        throw;
      }
    }

    internal static Task<T> GetTaskForSynchronousOperation<T>(Func<T> synchronousOperation)
    {
      try
      {
        return TaskUtils.GetCompletedTask<T>(synchronousOperation());
      }
      catch (Exception ex)
      {
        if (ExceptionUtils.IsCatchableExceptionType(ex))
          return TaskUtils.GetFaultedTask<T>(ex);
        throw;
      }
    }

    internal static Task GetTaskForSynchronousOperationReturningTask(Func<Task> synchronousOperation)
    {
      try
      {
        return synchronousOperation();
      }
      catch (Exception ex)
      {
        if (ExceptionUtils.IsCatchableExceptionType(ex))
          return TaskUtils.GetFaultedTask(ex);
        throw;
      }
    }

    internal static Task<TResult> GetTaskForSynchronousOperationReturningTask<TResult>(
      Func<Task<TResult>> synchronousOperation)
    {
      try
      {
        return synchronousOperation();
      }
      catch (Exception ex)
      {
        if (ExceptionUtils.IsCatchableExceptionType(ex))
          return TaskUtils.GetFaultedTask<TResult>(ex);
        throw;
      }
    }

    internal static Task FollowOnSuccessWith(this Task antecedentTask, Action<Task> operation) => (Task) TaskUtils.FollowOnSuccessWithImplementation<object>(antecedentTask, (Func<Task, object>) (t =>
    {
      operation(t);
      return (object) null;
    }));

    internal static Task<TFollowupTaskResult> FollowOnSuccessWith<TFollowupTaskResult>(
      this Task antecedentTask,
      Func<Task, TFollowupTaskResult> operation)
    {
      return TaskUtils.FollowOnSuccessWithImplementation<TFollowupTaskResult>(antecedentTask, operation);
    }

    internal static Task FollowOnSuccessWith<TAntecedentTaskResult>(
      this Task<TAntecedentTaskResult> antecedentTask,
      Action<Task<TAntecedentTaskResult>> operation)
    {
      return (Task) TaskUtils.FollowOnSuccessWithImplementation<object>((Task) antecedentTask, (Func<Task, object>) (t =>
      {
        operation((Task<TAntecedentTaskResult>) t);
        return (object) null;
      }));
    }

    internal static Task<TFollowupTaskResult> FollowOnSuccessWith<TAntecedentTaskResult, TFollowupTaskResult>(
      this Task<TAntecedentTaskResult> antecedentTask,
      Func<Task<TAntecedentTaskResult>, TFollowupTaskResult> operation)
    {
      return TaskUtils.FollowOnSuccessWithImplementation<TFollowupTaskResult>((Task) antecedentTask, (Func<Task, TFollowupTaskResult>) (t => operation((Task<TAntecedentTaskResult>) t)));
    }

    internal static Task FollowOnSuccessWithTask(
      this Task antecedentTask,
      Func<Task, Task> operation)
    {
      TaskCompletionSource<Task> taskCompletionSource = new TaskCompletionSource<Task>();
      antecedentTask.ContinueWith((Action<Task>) (taskToContinueOn => TaskUtils.FollowOnSuccessWithContinuation<Task>(taskToContinueOn, taskCompletionSource, operation)), TaskContinuationOptions.ExecuteSynchronously);
      return taskCompletionSource.Task.Unwrap();
    }

    internal static Task<TFollowupTaskResult> FollowOnSuccessWithTask<TFollowupTaskResult>(
      this Task antecedentTask,
      Func<Task, Task<TFollowupTaskResult>> operation)
    {
      TaskCompletionSource<Task<TFollowupTaskResult>> taskCompletionSource = new TaskCompletionSource<Task<TFollowupTaskResult>>();
      antecedentTask.ContinueWith((Action<Task>) (taskToContinueOn => TaskUtils.FollowOnSuccessWithContinuation<Task<TFollowupTaskResult>>(taskToContinueOn, taskCompletionSource, operation)), TaskContinuationOptions.ExecuteSynchronously);
      return taskCompletionSource.Task.Unwrap<TFollowupTaskResult>();
    }

    internal static Task FollowOnSuccessWithTask<TAntecedentTaskResult>(
      this Task<TAntecedentTaskResult> antecedentTask,
      Func<Task<TAntecedentTaskResult>, Task> operation)
    {
      TaskCompletionSource<Task> taskCompletionSource = new TaskCompletionSource<Task>();
      antecedentTask.ContinueWith((Action<Task<TAntecedentTaskResult>>) (taskToContinueOn => TaskUtils.FollowOnSuccessWithContinuation<Task>((Task) taskToContinueOn, taskCompletionSource, (Func<Task, Task>) (taskForOperation => operation((Task<TAntecedentTaskResult>) taskForOperation)))), TaskContinuationOptions.ExecuteSynchronously);
      return taskCompletionSource.Task.Unwrap();
    }

    internal static Task<TFollowupTaskResult> FollowOnSuccessWithTask<TAntecedentTaskResult, TFollowupTaskResult>(
      this Task<TAntecedentTaskResult> antecedentTask,
      Func<Task<TAntecedentTaskResult>, Task<TFollowupTaskResult>> operation)
    {
      TaskCompletionSource<Task<TFollowupTaskResult>> taskCompletionSource = new TaskCompletionSource<Task<TFollowupTaskResult>>();
      antecedentTask.ContinueWith((Action<Task<TAntecedentTaskResult>>) (taskToContinueOn => TaskUtils.FollowOnSuccessWithContinuation<Task<TFollowupTaskResult>>((Task) taskToContinueOn, taskCompletionSource, (Func<Task, Task<TFollowupTaskResult>>) (taskForOperation => operation((Task<TAntecedentTaskResult>) taskForOperation)))), TaskContinuationOptions.ExecuteSynchronously);
      return taskCompletionSource.Task.Unwrap<TFollowupTaskResult>();
    }

    internal static Task FollowOnFaultWith(this Task antecedentTask, Action<Task> operation) => (Task) TaskUtils.FollowOnFaultWithImplementation<object>(antecedentTask, (Func<Task, object>) (t => (object) null), operation);

    internal static Task<TResult> FollowOnFaultWith<TResult>(
      this Task<TResult> antecedentTask,
      Action<Task<TResult>> operation)
    {
      return TaskUtils.FollowOnFaultWithImplementation<TResult>((Task) antecedentTask, (Func<Task, TResult>) (t => ((Task<TResult>) t).Result), (Action<Task>) (t => operation((Task<TResult>) t)));
    }

    internal static Task<TResult> FollowOnFaultAndCatchExceptionWith<TResult, TExceptionType>(
      this Task<TResult> antecedentTask,
      Func<TExceptionType, TResult> catchBlock)
      where TExceptionType : Exception
    {
      return TaskUtils.FollowOnFaultAndCatchExceptionWithImplementation<TResult, TExceptionType>((Task) antecedentTask, (Func<Task, TResult>) (t => ((Task<TResult>) t).Result), catchBlock);
    }

    internal static Task FollowAlwaysWith(this Task antecedentTask, Action<Task> operation) => (Task) antecedentTask.FollowAlwaysWithImplementation<object>((Func<Task, object>) (t => (object) null), operation);

    internal static Task<TResult> FollowAlwaysWith<TResult>(
      this Task<TResult> antecedentTask,
      Action<Task<TResult>> operation)
    {
      return antecedentTask.FollowAlwaysWithImplementation<TResult>((Func<Task, TResult>) (t => ((Task<TResult>) t).Result), (Action<Task>) (t => operation((Task<TResult>) t)));
    }

    [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", Justification = "Need to access t.Exception to invoke the getter which will mark the Task to not throw the exception.")]
    internal static Task IgnoreExceptions(this Task task)
    {
      AggregateException exception;
      task.ContinueWith((Action<Task>) (t => exception = t.Exception), CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
      return task;
    }

    internal static TaskScheduler GetTargetScheduler(this TaskFactory factory) => factory.Scheduler ?? TaskScheduler.Current;

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Stores the exception so that it doesn't bring down the process but isntead rethrows on the task calling thread.")]
    internal static Task Iterate(this TaskFactory factory, IEnumerable<Task> source)
    {
      IEnumerator<Task> enumerator = source.GetEnumerator();
      TaskCompletionSource<object> trc = new TaskCompletionSource<object>((object) null, factory.CreationOptions);
      trc.Task.ContinueWith((Action<Task<object>>) (_ => enumerator.Dispose()), CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
      Action<Task> recursiveBody = (Action<Task>) null;
      recursiveBody = (Action<Task>) (antecedent =>
      {
        try
        {
          if (antecedent != null && antecedent.IsFaulted)
            trc.TrySetException((Exception) antecedent.Exception);
          else if (enumerator.MoveNext())
            enumerator.Current.ContinueWith(recursiveBody).IgnoreExceptions();
          else
            trc.TrySetResult((object) null);
        }
        catch (Exception ex)
        {
          if (!ExceptionUtils.IsCatchableExceptionType(ex))
            throw;
          else if (ex is OperationCanceledException canceledException2 && canceledException2.CancellationToken == factory.CancellationToken)
            trc.TrySetCanceled();
          else
            trc.TrySetException(ex);
        }
      });
      factory.StartNew((Action) (() => recursiveBody((Task) null)), CancellationToken.None, TaskCreationOptions.None, factory.GetTargetScheduler()).IgnoreExceptions();
      return (Task) trc.Task;
    }

    private static void FollowOnSuccessWithContinuation<TResult>(
      Task antecedentTask,
      TaskCompletionSource<TResult> taskCompletionSource,
      Func<Task, TResult> operation)
    {
      switch (antecedentTask.Status)
      {
        case TaskStatus.RanToCompletion:
          try
          {
            taskCompletionSource.TrySetResult(operation(antecedentTask));
            break;
          }
          catch (Exception ex)
          {
            if (!ExceptionUtils.IsCatchableExceptionType(ex))
            {
              throw;
            }
            else
            {
              taskCompletionSource.TrySetException(ex);
              break;
            }
          }
        case TaskStatus.Canceled:
          taskCompletionSource.TrySetCanceled();
          break;
        case TaskStatus.Faulted:
          taskCompletionSource.TrySetException((Exception) antecedentTask.Exception);
          break;
      }
    }

    private static Task<TResult> FollowOnSuccessWithImplementation<TResult>(
      Task antecedentTask,
      Func<Task, TResult> operation)
    {
      TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
      antecedentTask.ContinueWith((Action<Task>) (taskToContinueOn => TaskUtils.FollowOnSuccessWithContinuation<TResult>(taskToContinueOn, taskCompletionSource, operation)), TaskContinuationOptions.ExecuteSynchronously).IgnoreExceptions();
      return taskCompletionSource.Task;
    }

    private static Task<TResult> FollowOnFaultWithImplementation<TResult>(
      Task antecedentTask,
      Func<Task, TResult> getTaskResult,
      Action<Task> operation)
    {
      TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
      antecedentTask.ContinueWith((Action<Task>) (t =>
      {
        switch (t.Status)
        {
          case TaskStatus.RanToCompletion:
            taskCompletionSource.TrySetResult(getTaskResult(t));
            break;
          case TaskStatus.Canceled:
            taskCompletionSource.TrySetCanceled();
            break;
          case TaskStatus.Faulted:
            try
            {
              operation(t);
              taskCompletionSource.TrySetException((Exception) t.Exception);
              break;
            }
            catch (Exception ex)
            {
              if (!ExceptionUtils.IsCatchableExceptionType(ex))
              {
                throw;
              }
              else
              {
                Exception[] exceptionArray = new Exception[2]
                {
                  (Exception) t.Exception,
                  ex
                };
                taskCompletionSource.TrySetException((Exception) new AggregateException(exceptionArray));
                break;
              }
            }
        }
      }), TaskContinuationOptions.ExecuteSynchronously).IgnoreExceptions();
      return taskCompletionSource.Task;
    }

    private static Task<TResult> FollowOnFaultAndCatchExceptionWithImplementation<TResult, TExceptionType>(
      Task antecedentTask,
      Func<Task, TResult> getTaskResult,
      Func<TExceptionType, TResult> catchBlock)
      where TExceptionType : Exception
    {
      TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
      antecedentTask.ContinueWith((Action<Task>) (t =>
      {
        switch (t.Status)
        {
          case TaskStatus.RanToCompletion:
            taskCompletionSource.TrySetResult(getTaskResult(t));
            break;
          case TaskStatus.Canceled:
            taskCompletionSource.TrySetCanceled();
            break;
          case TaskStatus.Faulted:
            Exception exception = (Exception) t.Exception;
            if (exception is AggregateException aggregateException3)
            {
              AggregateException aggregateException2 = aggregateException3.Flatten();
              if (aggregateException2.InnerExceptions.Count == 1)
                exception = aggregateException2.InnerExceptions[0];
            }
            if (exception is TExceptionType exceptionType2)
            {
              try
              {
                taskCompletionSource.TrySetResult(catchBlock(exceptionType2));
                break;
              }
              catch (Exception ex)
              {
                if (!ExceptionUtils.IsCatchableExceptionType(ex))
                {
                  throw;
                }
                else
                {
                  Exception[] exceptionArray = new Exception[2]
                  {
                    exception,
                    ex
                  };
                  taskCompletionSource.TrySetException((Exception) new AggregateException(exceptionArray));
                  break;
                }
              }
            }
            else
            {
              taskCompletionSource.TrySetException(exception);
              break;
            }
        }
      }), TaskContinuationOptions.ExecuteSynchronously).IgnoreExceptions();
      return taskCompletionSource.Task;
    }

    private static Task<TResult> FollowAlwaysWithImplementation<TResult>(
      this Task antecedentTask,
      Func<Task, TResult> getTaskResult,
      Action<Task> operation)
    {
      TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
      antecedentTask.ContinueWith((Action<Task>) (t =>
      {
        Exception exception1 = (Exception) null;
        try
        {
          operation(t);
        }
        catch (Exception ex)
        {
          if (!ExceptionUtils.IsCatchableExceptionType(ex))
            throw;
          else
            exception1 = ex;
        }
        switch (t.Status)
        {
          case TaskStatus.RanToCompletion:
            if (exception1 != null)
            {
              taskCompletionSource.TrySetException(exception1);
              break;
            }
            taskCompletionSource.TrySetResult(getTaskResult(t));
            break;
          case TaskStatus.Canceled:
            if (exception1 != null)
            {
              taskCompletionSource.TrySetException(exception1);
              break;
            }
            taskCompletionSource.TrySetCanceled();
            break;
          case TaskStatus.Faulted:
            Exception exception2 = (Exception) t.Exception;
            if (exception1 != null)
              exception2 = (Exception) new AggregateException(new Exception[2]
              {
                exception2,
                exception1
              });
            taskCompletionSource.TrySetException(exception2);
            break;
        }
      }), TaskContinuationOptions.ExecuteSynchronously).IgnoreExceptions();
      return taskCompletionSource.Task;
    }
  }
}
