// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.IteratorAsyncResult`1
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using Microsoft.Azure.NotificationHubs.Common.Parallel;
using Microsoft.Azure.NotificationHubs.Tracing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  [DebuggerStepThrough]
  internal abstract class IteratorAsyncResult<TIteratorAsyncResult> : 
    AsyncResult<TIteratorAsyncResult>
    where TIteratorAsyncResult : IteratorAsyncResult<TIteratorAsyncResult>
  {
    private static readonly Action<AsyncResult, Exception> onFinally = new Action<AsyncResult, Exception>(IteratorAsyncResult<TIteratorAsyncResult>.Finally);
    private static AsyncResult.AsyncCompletion stepCallbackDelegate;
    private Microsoft.Azure.NotificationHubs.Common.TimeoutHelper timeoutHelper;
    private volatile bool everCompletedAsynchronously;
    private IEnumerator<IteratorAsyncResult<TIteratorAsyncResult>.AsyncStep> steps;
    private Exception lastAsyncStepException;

    protected IteratorAsyncResult(TimeSpan timeout, AsyncCallback callback, object state)
      : base(callback, state)
    {
      this.timeoutHelper = new Microsoft.Azure.NotificationHubs.Common.TimeoutHelper(timeout, true);
      this.OnCompleting = this.OnCompleting + IteratorAsyncResult<TIteratorAsyncResult>.onFinally;
    }

    protected Exception LastAsyncStepException
    {
      get => this.lastAsyncStepException;
      set => this.lastAsyncStepException = value;
    }

    public TimeSpan OriginalTimeout => this.timeoutHelper.OriginalTimeout;

    private static AsyncResult.AsyncCompletion StepCallbackDelegate
    {
      get
      {
        if (IteratorAsyncResult<TIteratorAsyncResult>.stepCallbackDelegate == null)
          IteratorAsyncResult<TIteratorAsyncResult>.stepCallbackDelegate = new AsyncResult.AsyncCompletion(IteratorAsyncResult<TIteratorAsyncResult>.StepCallback);
        return IteratorAsyncResult<TIteratorAsyncResult>.stepCallbackDelegate;
      }
    }

    public IAsyncResult Start()
    {
      try
      {
        this.steps = this.GetAsyncSteps();
        this.EnumerateSteps(IteratorAsyncResult<TIteratorAsyncResult>.CurrentThreadType.StartingThread);
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
          throw;
        else
          this.Complete(ex);
      }
      return (IAsyncResult) this;
    }

    public TIteratorAsyncResult RunSynchronously()
    {
      try
      {
        this.steps = this.GetAsyncSteps();
        this.EnumerateSteps(IteratorAsyncResult<TIteratorAsyncResult>.CurrentThreadType.Synchronous);
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
          throw;
        else
          this.Complete(ex);
      }
      return AsyncResult.End<TIteratorAsyncResult>((IAsyncResult) this);
    }

    protected IteratorAsyncResult<TIteratorAsyncResult>.AsyncStep CallAsync(
      IteratorAsyncResult<TIteratorAsyncResult>.BeginCall beginCall,
      IteratorAsyncResult<TIteratorAsyncResult>.EndCall endCall,
      IteratorAsyncResult<TIteratorAsyncResult>.Call call,
      IteratorAsyncResult<TIteratorAsyncResult>.ExceptionPolicy policy)
    {
      return new IteratorAsyncResult<TIteratorAsyncResult>.AsyncStep((Transaction) null, beginCall, endCall, call, policy);
    }

    protected IteratorAsyncResult<TIteratorAsyncResult>.AsyncStep CallAsync(
      IteratorAsyncResult<TIteratorAsyncResult>.BeginCall beginCall,
      IteratorAsyncResult<TIteratorAsyncResult>.EndCall endCall,
      IteratorAsyncResult<TIteratorAsyncResult>.ExceptionPolicy policy)
    {
      return new IteratorAsyncResult<TIteratorAsyncResult>.AsyncStep((Transaction) null, beginCall, endCall, (IteratorAsyncResult<TIteratorAsyncResult>.Call) null, policy);
    }

    protected IteratorAsyncResult<TIteratorAsyncResult>.AsyncStep CallParallelAsync<TWorkItem>(
      ICollection<TWorkItem> workItems,
      IteratorAsyncResult<TIteratorAsyncResult>.BeginCall<TWorkItem> beginCall,
      IteratorAsyncResult<TIteratorAsyncResult>.EndCall<TWorkItem> endCall,
      IteratorAsyncResult<TIteratorAsyncResult>.ExceptionPolicy policy)
    {
      return this.CallAsync((IteratorAsyncResult<TIteratorAsyncResult>.BeginCall) ((thisPtr, t, c, s) => (IAsyncResult) new IteratorAsyncResult<TIteratorAsyncResult>.ParallelAsyncResult<TWorkItem>(thisPtr, workItems, beginCall, endCall, t, c, s)), (IteratorAsyncResult<TIteratorAsyncResult>.EndCall) ((thisPtr, r) => AsyncResult<IteratorAsyncResult<TIteratorAsyncResult>.ParallelAsyncResult<TWorkItem>>.End(r)), policy);
    }

    protected IteratorAsyncResult<TIteratorAsyncResult>.AsyncStep CallParallelAsync<TWorkItem>(
      ICollection<TWorkItem> workItems,
      IteratorAsyncResult<TIteratorAsyncResult>.BeginCall<TWorkItem> beginCall,
      IteratorAsyncResult<TIteratorAsyncResult>.EndCall<TWorkItem> endCall,
      TimeSpan timeout,
      IteratorAsyncResult<TIteratorAsyncResult>.ExceptionPolicy policy)
    {
      return this.CallAsync((IteratorAsyncResult<TIteratorAsyncResult>.BeginCall) ((thisPtr, t, c, s) => (IAsyncResult) new IteratorAsyncResult<TIteratorAsyncResult>.ParallelAsyncResult<TWorkItem>(thisPtr, workItems, beginCall, endCall, timeout, c, s)), (IteratorAsyncResult<TIteratorAsyncResult>.EndCall) ((thisPtr, r) => AsyncResult<IteratorAsyncResult<TIteratorAsyncResult>.ParallelAsyncResult<TWorkItem>>.End(r)), policy);
    }

    protected IteratorAsyncResult<TIteratorAsyncResult>.AsyncStep CallTask(
      Func<TIteratorAsyncResult, TimeSpan, Task> taskFunc,
      IteratorAsyncResult<TIteratorAsyncResult>.ExceptionPolicy policy)
    {
      return this.CallAsync((IteratorAsyncResult<TIteratorAsyncResult>.BeginCall) ((thisPtr, t, c, s) =>
      {
        Task task = taskFunc(thisPtr, t);
        if (task.Status == TaskStatus.Created)
          task.Start();
        return task.ToAsyncResult(c, s);
      }), (IteratorAsyncResult<TIteratorAsyncResult>.EndCall) ((thisPtr, r) => TaskHelpers.EndAsyncResult(r)), policy);
    }

    protected IteratorAsyncResult<TIteratorAsyncResult>.AsyncStep CallTransactionalAsync(
      Transaction transaction,
      IteratorAsyncResult<TIteratorAsyncResult>.BeginCall beginCall,
      IteratorAsyncResult<TIteratorAsyncResult>.EndCall endCall,
      IteratorAsyncResult<TIteratorAsyncResult>.Call call,
      IteratorAsyncResult<TIteratorAsyncResult>.ExceptionPolicy policy)
    {
      return new IteratorAsyncResult<TIteratorAsyncResult>.AsyncStep(transaction, beginCall, endCall, call, policy);
    }

    protected IteratorAsyncResult<TIteratorAsyncResult>.AsyncStep CallTransactionalAsync(
      Transaction transaction,
      IteratorAsyncResult<TIteratorAsyncResult>.BeginCall beginCall,
      IteratorAsyncResult<TIteratorAsyncResult>.EndCall endCall,
      IteratorAsyncResult<TIteratorAsyncResult>.ExceptionPolicy policy)
    {
      return new IteratorAsyncResult<TIteratorAsyncResult>.AsyncStep(transaction, beginCall, endCall, (IteratorAsyncResult<TIteratorAsyncResult>.Call) null, policy);
    }

    protected IteratorAsyncResult<TIteratorAsyncResult>.AsyncStep CallAsyncSleep(
      TimeSpan amountToSleep)
    {
      return this.CallAsyncSleep(amountToSleep, CancellationToken.None);
    }

    protected IteratorAsyncResult<TIteratorAsyncResult>.AsyncStep CallAsyncSleep(
      TimeSpan amountToSleep,
      CancellationToken cancellationToken)
    {
      return this.CallAsync((IteratorAsyncResult<TIteratorAsyncResult>.BeginCall) ((thisPtr, t, c, s) => (IAsyncResult) new IteratorAsyncResult<TIteratorAsyncResult>.SleepAsyncResult(amountToSleep, cancellationToken, c, s)), (IteratorAsyncResult<TIteratorAsyncResult>.EndCall) ((thisPtr, r) => IteratorAsyncResult<TIteratorAsyncResult>.SleepAsyncResult.End(r)), (IteratorAsyncResult<TIteratorAsyncResult>.Call) ((thisPtr, t) => Thread.Sleep(amountToSleep)), IteratorAsyncResult<TIteratorAsyncResult>.ExceptionPolicy.Transfer);
    }

    protected TimeSpan RemainingTime() => this.timeoutHelper.RemainingTime();

    protected abstract IEnumerator<IteratorAsyncResult<TIteratorAsyncResult>.AsyncStep> GetAsyncSteps();

    protected void Complete(Exception operationException) => this.Complete(!this.everCompletedAsynchronously, operationException);

    private static bool StepCallback(IAsyncResult result)
    {
      IteratorAsyncResult<TIteratorAsyncResult> asyncState = (IteratorAsyncResult<TIteratorAsyncResult>) result.AsyncState;
      bool flag = asyncState.CheckSyncContinue(result);
      if (!flag)
      {
        asyncState.everCompletedAsynchronously = true;
        try
        {
          asyncState.steps.Current.EndCall((TIteratorAsyncResult) asyncState, result);
        }
        catch (Exception ex)
        {
          if (!Fx.IsFatal(ex))
          {
            if (asyncState.HandleException(ex))
              goto label_6;
          }
          throw;
        }
label_6:
        asyncState.EnumerateSteps(IteratorAsyncResult<TIteratorAsyncResult>.CurrentThreadType.Callback);
      }
      return flag;
    }

    private static void Finally(AsyncResult result, Exception exception)
    {
      IteratorAsyncResult<TIteratorAsyncResult> iteratorAsyncResult = (IteratorAsyncResult<TIteratorAsyncResult>) result;
      try
      {
        iteratorAsyncResult.steps?.Dispose();
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
        {
          throw;
        }
        else
        {
          MessagingClientEtwProvider.Provider.EventWriteExceptionAsWarning(ex.ToStringSlim());
          if (exception != null)
            return;
          throw;
        }
      }
    }

    private bool MoveNextStep() => this.steps.MoveNext();

    private void EnumerateSteps(
      IteratorAsyncResult<TIteratorAsyncResult>.CurrentThreadType state)
    {
      while (!this.IsCompleted && this.MoveNextStep())
      {
        this.LastAsyncStepException = (Exception) null;
        IteratorAsyncResult<TIteratorAsyncResult>.AsyncStep current = this.steps.Current;
        if (current.BeginCall != null)
        {
          IAsyncResult asyncResult = (IAsyncResult) null;
          if (state == IteratorAsyncResult<TIteratorAsyncResult>.CurrentThreadType.Synchronous && current.HasSynchronous)
          {
            if (current.Policy == IteratorAsyncResult<TIteratorAsyncResult>.ExceptionPolicy.Transfer)
            {
              using (this.PrepareTransactionalCall(current.Transaction))
                current.Call((TIteratorAsyncResult) this, this.timeoutHelper.RemainingTime());
            }
            else
            {
              try
              {
                using (this.PrepareTransactionalCall(current.Transaction))
                  current.Call((TIteratorAsyncResult) this, this.timeoutHelper.RemainingTime());
              }
              catch (Exception ex)
              {
                if (!Fx.IsFatal(ex))
                {
                  if (this.HandleException(ex))
                    goto label_31;
                }
                throw;
              }
            }
          }
          else if (current.Policy == IteratorAsyncResult<TIteratorAsyncResult>.ExceptionPolicy.Transfer)
          {
            using (this.PrepareTransactionalCall(current.Transaction))
              asyncResult = current.BeginCall((TIteratorAsyncResult) this, this.timeoutHelper.RemainingTime(), this.PrepareAsyncCompletion(IteratorAsyncResult<TIteratorAsyncResult>.StepCallbackDelegate), (object) this);
          }
          else
          {
            try
            {
              using (this.PrepareTransactionalCall(current.Transaction))
                asyncResult = current.BeginCall((TIteratorAsyncResult) this, this.timeoutHelper.RemainingTime(), this.PrepareAsyncCompletion(IteratorAsyncResult<TIteratorAsyncResult>.StepCallbackDelegate), (object) this);
            }
            catch (Exception ex)
            {
              if (!Fx.IsFatal(ex))
              {
                if (this.HandleException(ex))
                  goto label_31;
              }
              throw;
            }
          }
label_31:
          if (asyncResult != null)
          {
            if (!this.CheckSyncContinue(asyncResult))
              return;
            try
            {
              this.steps.Current.EndCall((TIteratorAsyncResult) this, asyncResult);
            }
            catch (Exception ex)
            {
              if (!Fx.IsFatal(ex))
              {
                if (this.HandleException(ex))
                  continue;
              }
              throw;
            }
          }
        }
      }
      if (this.IsCompleted)
        return;
      this.Complete(!this.everCompletedAsynchronously);
    }

    private bool HandleException(Exception e)
    {
      this.LastAsyncStepException = e;
      bool flag;
      switch (this.steps.Current.Policy)
      {
        case IteratorAsyncResult<TIteratorAsyncResult>.ExceptionPolicy.Transfer:
          flag = false;
          if (!this.IsCompleted)
          {
            this.Complete(e);
            flag = true;
            break;
          }
          break;
        case IteratorAsyncResult<TIteratorAsyncResult>.ExceptionPolicy.Continue:
          flag = true;
          break;
        default:
          flag = false;
          break;
      }
      return flag;
    }

    protected delegate IAsyncResult BeginCall(
      TIteratorAsyncResult thisPtr,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
      where TIteratorAsyncResult : IteratorAsyncResult<TIteratorAsyncResult>;

    protected delegate void EndCall(TIteratorAsyncResult thisPtr, IAsyncResult ar) where TIteratorAsyncResult : IteratorAsyncResult<TIteratorAsyncResult>;

    protected delegate void Call(TIteratorAsyncResult thisPtr, TimeSpan timeout) where TIteratorAsyncResult : IteratorAsyncResult<TIteratorAsyncResult>;

    private enum CurrentThreadType
    {
      Synchronous,
      StartingThread,
      Callback,
    }

    [DebuggerStepThrough]
    protected struct AsyncStep
    {
      private readonly IteratorAsyncResult<TIteratorAsyncResult>.ExceptionPolicy policy;
      private readonly IteratorAsyncResult<TIteratorAsyncResult>.BeginCall beginCall;
      private readonly IteratorAsyncResult<TIteratorAsyncResult>.EndCall endCall;
      private readonly IteratorAsyncResult<TIteratorAsyncResult>.Call call;
      private readonly Transaction transaction;
      public static readonly IteratorAsyncResult<TIteratorAsyncResult>.AsyncStep Empty;

      public AsyncStep(
        Transaction transaction,
        IteratorAsyncResult<TIteratorAsyncResult>.BeginCall beginCall,
        IteratorAsyncResult<TIteratorAsyncResult>.EndCall endCall,
        IteratorAsyncResult<TIteratorAsyncResult>.Call call,
        IteratorAsyncResult<TIteratorAsyncResult>.ExceptionPolicy policy)
      {
        this.transaction = transaction;
        this.policy = policy;
        this.beginCall = beginCall;
        this.endCall = endCall;
        this.call = call;
      }

      public IteratorAsyncResult<TIteratorAsyncResult>.BeginCall BeginCall => this.beginCall;

      public IteratorAsyncResult<TIteratorAsyncResult>.EndCall EndCall => this.endCall;

      public IteratorAsyncResult<TIteratorAsyncResult>.Call Call => this.call;

      public Transaction Transaction => this.transaction;

      public bool HasSynchronous => this.call != null;

      public IteratorAsyncResult<TIteratorAsyncResult>.ExceptionPolicy Policy => this.policy;
    }

    protected enum ExceptionPolicy
    {
      Transfer,
      Continue,
    }

    private sealed class SleepAsyncResult : 
      AsyncResult<IteratorAsyncResult<TIteratorAsyncResult>.SleepAsyncResult>
    {
      private static readonly Action<object> onTimer = new Action<object>(IteratorAsyncResult<TIteratorAsyncResult>.SleepAsyncResult.OnTimer);
      private static readonly Action<object> StaticOnCancellation = new Action<object>(IteratorAsyncResult<TIteratorAsyncResult>.SleepAsyncResult.OnCancellation);
      private readonly IOThreadTimer timer;
      private CancellationTokenRegistration cancellationTokenRegistration;

      public SleepAsyncResult(
        TimeSpan amount,
        CancellationToken cancellationToken,
        AsyncCallback callback,
        object state)
        : base(callback, state)
      {
        this.timer = new IOThreadTimer(IteratorAsyncResult<TIteratorAsyncResult>.SleepAsyncResult.onTimer, (object) this, false);
        this.timer.Set(amount);
        try
        {
          this.cancellationTokenRegistration = cancellationToken.Register(IteratorAsyncResult<TIteratorAsyncResult>.SleepAsyncResult.StaticOnCancellation, (object) this);
        }
        catch (ObjectDisposedException ex)
        {
          this.HandleCancellation(false);
        }
      }

      public static void End(IAsyncResult result)
      {
        IteratorAsyncResult<TIteratorAsyncResult>.SleepAsyncResult sleepAsyncResult = AsyncResult<IteratorAsyncResult<TIteratorAsyncResult>.SleepAsyncResult>.End(result);
        try
        {
          sleepAsyncResult.cancellationTokenRegistration.Dispose();
        }
        catch (ObjectDisposedException ex)
        {
        }
      }

      private static void OnTimer(object state) => ((AsyncResult) state).Complete(false);

      private static void OnCancellation(object state) => ((IteratorAsyncResult<TIteratorAsyncResult>.SleepAsyncResult) state).HandleCancellation(true);

      private void HandleCancellation(bool scheduleComplete)
      {
        if (!this.timer.Cancel())
          return;
        if (scheduleComplete)
          IOThreadScheduler.ScheduleCallbackNoFlow((Action<object>) (s => ((AsyncResult) s).Complete(false)), (object) this);
        else
          this.Complete(true);
      }
    }

    protected delegate IAsyncResult BeginCall<TWorkItem>(
      TIteratorAsyncResult thisPtr,
      TWorkItem workItem,
      TimeSpan timeout,
      AsyncCallback callback,
      object state)
      where TIteratorAsyncResult : IteratorAsyncResult<TIteratorAsyncResult>;

    protected delegate void EndCall<TWorkItem>(
      TIteratorAsyncResult thisPtr,
      TWorkItem workItem,
      IAsyncResult ar)
      where TIteratorAsyncResult : IteratorAsyncResult<TIteratorAsyncResult>;

    private sealed class ParallelAsyncResult<TWorkItem> : 
      AsyncResult<IteratorAsyncResult<TIteratorAsyncResult>.ParallelAsyncResult<TWorkItem>>
    {
      private static AsyncCallback completed = new AsyncCallback(IteratorAsyncResult<TIteratorAsyncResult>.ParallelAsyncResult<TWorkItem>.OnCompleted);
      private readonly TIteratorAsyncResult iteratorAsyncResult;
      private readonly ICollection<TWorkItem> workItems;
      private readonly IteratorAsyncResult<TIteratorAsyncResult>.EndCall<TWorkItem> endCall;
      private long actions;
      private Exception firstException;

      public ParallelAsyncResult(
        TIteratorAsyncResult iteratorAsyncResult,
        ICollection<TWorkItem> workItems,
        IteratorAsyncResult<TIteratorAsyncResult>.BeginCall<TWorkItem> beginCall,
        IteratorAsyncResult<TIteratorAsyncResult>.EndCall<TWorkItem> endCall,
        TimeSpan timeout,
        AsyncCallback callback,
        object state)
        : base(callback, state)
      {
        this.iteratorAsyncResult = iteratorAsyncResult;
        this.workItems = workItems;
        this.endCall = endCall;
        this.actions = (long) (this.workItems.Count + 1);
        foreach (TWorkItem workItem in (IEnumerable<TWorkItem>) workItems)
        {
          try
          {
            IAsyncResult asyncResult = beginCall(iteratorAsyncResult, workItem, timeout, IteratorAsyncResult<TIteratorAsyncResult>.ParallelAsyncResult<TWorkItem>.completed, (object) new IteratorAsyncResult<TIteratorAsyncResult>.ParallelAsyncResult<TWorkItem>.CallbackState(this, workItem));
          }
          catch (Exception ex)
          {
            if (Fx.IsFatal(ex))
              throw;
            else
              this.TryComplete(ex, true);
          }
        }
        this.TryComplete((Exception) null, true);
      }

      private void TryComplete(Exception exception, bool completedSynchronously)
      {
        if (this.firstException == null)
          this.firstException = exception;
        if (Interlocked.Decrement(ref this.actions) != 0L)
          return;
        this.Complete(completedSynchronously, this.firstException);
      }

      private static void OnCompleted(IAsyncResult ar)
      {
        IteratorAsyncResult<TIteratorAsyncResult>.ParallelAsyncResult<TWorkItem>.CallbackState asyncState = (IteratorAsyncResult<TIteratorAsyncResult>.ParallelAsyncResult<TWorkItem>.CallbackState) ar.AsyncState;
        IteratorAsyncResult<TIteratorAsyncResult>.ParallelAsyncResult<TWorkItem> asyncResult = asyncState.AsyncResult;
        try
        {
          asyncResult.endCall(asyncResult.iteratorAsyncResult, asyncState.AsyncData, ar);
          asyncResult.TryComplete((Exception) null, ar.CompletedSynchronously);
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
            throw;
          else
            asyncResult.TryComplete(ex, ar.CompletedSynchronously);
        }
      }

      private sealed class CallbackState
      {
        public CallbackState(
          IteratorAsyncResult<TIteratorAsyncResult>.ParallelAsyncResult<TWorkItem> asyncResult,
          TWorkItem data)
        {
          this.AsyncResult = asyncResult;
          this.AsyncData = data;
        }

        public IteratorAsyncResult<TIteratorAsyncResult>.ParallelAsyncResult<TWorkItem> AsyncResult { get; private set; }

        public TWorkItem AsyncData { get; private set; }
      }
    }
  }
}
