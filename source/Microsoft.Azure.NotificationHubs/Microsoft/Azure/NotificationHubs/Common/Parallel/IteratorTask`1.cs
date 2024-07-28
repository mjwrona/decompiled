// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.Parallel.IteratorTask`1
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.Azure.NotificationHubs.Common.Parallel
{
  internal abstract class IteratorTask<TResult> : TaskCompletionSource<TResult>
  {
    private IEnumerator<IteratorTask<TResult>.TaskStep> steps;
    private IteratorTask<TResult>.TaskStep currentStep;
    protected Action<IteratorTask<TResult>, Exception> OnSetException;

    public System.Threading.Tasks.Task<TResult> Start()
    {
      try
      {
        this.steps = this.GetTasks();
        this.EnumerateSteps();
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
          throw;
        else
          this.DoSetException(ex);
      }
      return this.Task;
    }

    protected System.Threading.Tasks.Task LastTask => this.currentStep.Task;

    protected TTaskResult LastTaskResult<TTaskResult>() => ((System.Threading.Tasks.Task<TTaskResult>) this.LastTask).Result;

    protected abstract IEnumerator<IteratorTask<TResult>.TaskStep> GetTasks();

    protected IteratorTask<TResult>.TaskStep CallTask(
      System.Threading.Tasks.Task task,
      IteratorTask<TResult>.ExceptionPolicy policy = IteratorTask<TResult>.ExceptionPolicy.Transfer)
    {
      return new IteratorTask<TResult>.TaskStep(task, policy);
    }

    protected IteratorTask<TResult>.TaskStep CallTask<TState>(
      Func<TState, System.Threading.Tasks.Task> taskFunc,
      TState state,
      IteratorTask<TResult>.ExceptionPolicy policy = IteratorTask<TResult>.ExceptionPolicy.Transfer)
    {
      System.Threading.Tasks.Task task;
      if (policy == IteratorTask<TResult>.ExceptionPolicy.Continue)
      {
        try
        {
          task = taskFunc(state);
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
            task = (System.Threading.Tasks.Task) completionSource.Task;
          }
        }
      }
      else
        task = taskFunc(state);
      return new IteratorTask<TResult>.TaskStep(task, policy);
    }

    private void EnumerateSteps()
    {
      if (this.Task.IsCompleted)
        return;
      if (!this.steps.MoveNext())
      {
        this.StepsComplete();
      }
      else
      {
        this.currentStep = this.steps.Current;
        this.currentStep.Task.ContinueWith((Action<System.Threading.Tasks.Task>) (t =>
        {
          if (t.IsCanceled)
          {
            if (this.currentStep.Policy != IteratorTask<TResult>.ExceptionPolicy.Transfer)
              return;
            this.TrySetCanceled();
            this.StepsComplete();
          }
          else if (t.IsFaulted && this.currentStep.Policy == IteratorTask<TResult>.ExceptionPolicy.Transfer)
          {
            this.DoSetException(t.Exception.GetBaseException());
            this.StepsComplete();
          }
          else
          {
            try
            {
              this.EnumerateSteps();
            }
            catch (Exception ex)
            {
              if (Fx.IsFatal(ex))
                throw;
              else
                this.DoSetException(ex);
            }
          }
        }), TaskContinuationOptions.ExecuteSynchronously);
      }
    }

    private void DoSetException(Exception exception)
    {
      if (this.OnSetException != null)
      {
        try
        {
          this.OnSetException(this, exception);
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
            throw;
          else
            exception = ex;
        }
      }
      this.TrySetException(exception);
    }

    private void StepsComplete()
    {
      this.steps.Dispose();
      if (this.Task.IsCompleted)
        return;
      this.TrySetResult(default (TResult));
    }

    [DebuggerStepThrough]
    protected struct TaskStep
    {
      private readonly IteratorTask<TResult>.ExceptionPolicy policy;
      private readonly System.Threading.Tasks.Task task;

      public TaskStep(System.Threading.Tasks.Task task, IteratorTask<TResult>.ExceptionPolicy policy)
      {
        this.task = task;
        this.policy = policy;
      }

      public System.Threading.Tasks.Task Task => this.task;

      public IteratorTask<TResult>.ExceptionPolicy Policy => this.policy;
    }

    protected enum ExceptionPolicy
    {
      Transfer,
      Continue,
    }
  }
}
