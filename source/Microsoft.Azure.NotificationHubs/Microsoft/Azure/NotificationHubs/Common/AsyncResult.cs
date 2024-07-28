// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.AsyncResult
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Tracing;
using System;
using System.Diagnostics;
using System.Threading;
using System.Transactions;

namespace Microsoft.Azure.NotificationHubs.Common
{
  [DebuggerStepThrough]
  internal abstract class AsyncResult : IAsyncResult
  {
    public const string DisablePrepareForRethrow = "DisablePrepareForRethrow";
    private static AsyncCallback asyncCompletionWrapperCallback;
    private AsyncCallback callback;
    private bool completedSynchronously;
    private bool endCalled;
    private Exception exception;
    private bool isCompleted;
    private AsyncResult.AsyncCompletion nextAsyncCompletion;
    private IAsyncResult deferredTransactionalResult;
    private AsyncResult.TransactionSignalScope transactionContext;
    private object state;
    private ManualResetEvent manualResetEvent;
    private object thisLock;

    protected AsyncResult(AsyncCallback callback, object state)
    {
      this.callback = callback;
      this.state = state;
      this.thisLock = new object();
    }

    public object AsyncState => this.state;

    public WaitHandle AsyncWaitHandle
    {
      get
      {
        if (this.manualResetEvent != null)
          return (WaitHandle) this.manualResetEvent;
        lock (this.ThisLock)
        {
          if (this.manualResetEvent == null)
            this.manualResetEvent = new ManualResetEvent(this.isCompleted);
        }
        return (WaitHandle) this.manualResetEvent;
      }
    }

    public bool CompletedSynchronously => this.completedSynchronously;

    public bool HasCallback => this.callback != null;

    public bool IsCompleted => this.isCompleted;

    protected Action<AsyncResult, Exception> OnCompleting { get; set; }

    protected internal virtual EventTraceActivity Activity => (EventTraceActivity) null;

    protected virtual TraceEventType TraceEventType => TraceEventType.Verbose;

    protected object ThisLock => this.thisLock;

    protected Action<AsyncCallback, IAsyncResult> VirtualCallback { get; set; }

    protected bool TryComplete(bool didCompleteSynchronously, Exception exception)
    {
      lock (this.ThisLock)
      {
        if (this.isCompleted)
          return false;
        this.exception = exception;
        this.isCompleted = true;
      }
      this.completedSynchronously = didCompleteSynchronously;
      if (this.OnCompleting != null)
      {
        try
        {
          this.OnCompleting(this, this.exception);
        }
        catch (Exception ex)
        {
          if (Fx.IsFatal(ex))
            throw;
          else
            this.exception = ex;
        }
      }
      if (!didCompleteSynchronously)
      {
        lock (this.ThisLock)
        {
          if (this.manualResetEvent != null)
            this.manualResetEvent.Set();
        }
      }
      if (this.callback != null)
      {
        try
        {
          if (this.VirtualCallback != null)
            this.VirtualCallback(this.callback, (IAsyncResult) this);
          else
            this.callback((IAsyncResult) this);
        }
        catch (Exception ex)
        {
          if (!Fx.IsFatal(ex))
            throw Fx.Exception.AsError((Exception) new CallbackException(SRCore.AsyncCallbackThrewException, ex));
          throw;
        }
      }
      return true;
    }

    protected bool TryComplete(bool didcompleteSynchronously) => this.TryComplete(didcompleteSynchronously, (Exception) null);

    protected void Complete(bool didCompleteSynchronously) => this.Complete(didCompleteSynchronously, (Exception) null);

    protected void Complete(bool didCompleteSynchronously, Exception e)
    {
      if (!this.TryComplete(didCompleteSynchronously, e))
        throw Fx.Exception.AsError((Exception) new InvalidOperationException(SRCore.AsyncResultCompletedTwice((object) this.GetType())));
    }

    private static void AsyncCompletionWrapperCallback(IAsyncResult result)
    {
      if (result == null)
        throw Fx.Exception.AsError((Exception) new InvalidOperationException(SRCore.InvalidNullAsyncResult));
      if (result.CompletedSynchronously)
        return;
      AsyncResult asyncState = (AsyncResult) result.AsyncState;
      if (asyncState.transactionContext != null && !asyncState.transactionContext.Signal(result))
        return;
      AsyncResult.AsyncCompletion nextCompletion = asyncState.GetNextCompletion();
      if (nextCompletion == null)
        AsyncResult.ThrowInvalidAsyncResult(result);
      Exception e = (Exception) null;
      bool flag;
      try
      {
        flag = nextCompletion(result);
      }
      catch (Exception ex)
      {
        flag = true;
        e = ex;
      }
      if (!flag)
        return;
      asyncState.Complete(false, e);
    }

    protected AsyncCallback PrepareAsyncCompletion(AsyncResult.AsyncCompletion callback)
    {
      if (this.transactionContext != null)
      {
        if (this.transactionContext.IsPotentiallyAbandoned)
          this.transactionContext = (AsyncResult.TransactionSignalScope) null;
        else
          this.transactionContext.Prepared();
      }
      this.nextAsyncCompletion = callback;
      if (AsyncResult.asyncCompletionWrapperCallback == null)
        AsyncResult.asyncCompletionWrapperCallback = new AsyncCallback(AsyncResult.AsyncCompletionWrapperCallback);
      return AsyncResult.asyncCompletionWrapperCallback;
    }

    protected IDisposable PrepareTransactionalCall(Transaction transaction)
    {
      if (this.transactionContext != null && !this.transactionContext.IsPotentiallyAbandoned)
        AsyncResult.ThrowInvalidAsyncResult("PrepareTransactionalCall should only be called as the object of non-nested using statements. If the Begin succeeds, Check/SyncContinue must be called before another PrepareTransactionalCall.");
      return (IDisposable) (this.transactionContext = transaction == (Transaction) null ? (AsyncResult.TransactionSignalScope) null : new AsyncResult.TransactionSignalScope(this, transaction));
    }

    protected bool CheckSyncContinue(IAsyncResult result) => this.TryContinueHelper(result, out AsyncResult.AsyncCompletion _);

    protected bool SyncContinue(IAsyncResult result)
    {
      AsyncResult.AsyncCompletion callback;
      return this.TryContinueHelper(result, out callback) && callback(result);
    }

    private bool TryContinueHelper(IAsyncResult result, out AsyncResult.AsyncCompletion callback)
    {
      if (result == null)
        throw Fx.Exception.AsError((Exception) new InvalidOperationException(SRCore.InvalidNullAsyncResult));
      callback = (AsyncResult.AsyncCompletion) null;
      if (result.CompletedSynchronously)
      {
        if (this.transactionContext != null)
        {
          if (this.transactionContext.State != AsyncResult.TransactionSignalState.Completed)
            AsyncResult.ThrowInvalidAsyncResult("Check/SyncContinue cannot be called from within the PrepareTransactionalCall using block.");
          else if (this.transactionContext.IsSignalled)
            AsyncResult.ThrowInvalidAsyncResult(result);
        }
      }
      else
      {
        if (result != this.deferredTransactionalResult)
          return false;
        if (this.transactionContext == null || !this.transactionContext.IsSignalled)
          AsyncResult.ThrowInvalidAsyncResult(result);
        this.deferredTransactionalResult = (IAsyncResult) null;
      }
      callback = this.GetNextCompletion();
      if (callback == null)
        AsyncResult.ThrowInvalidAsyncResult("Only call Check/SyncContinue once per async operation (once per PrepareAsyncCompletion).");
      return true;
    }

    private AsyncResult.AsyncCompletion GetNextCompletion()
    {
      AsyncResult.AsyncCompletion nextAsyncCompletion = this.nextAsyncCompletion;
      this.transactionContext = (AsyncResult.TransactionSignalScope) null;
      this.nextAsyncCompletion = (AsyncResult.AsyncCompletion) null;
      return nextAsyncCompletion;
    }

    protected static void ThrowInvalidAsyncResult(IAsyncResult result) => throw Fx.Exception.AsError((Exception) new InvalidOperationException(SRCore.InvalidAsyncResultImplementation((object) result.GetType())));

    protected static void ThrowInvalidAsyncResult(string debugText) => throw Fx.Exception.AsError((Exception) new InvalidOperationException(SRCore.InvalidAsyncResultImplementationGeneric));

    protected static TAsyncResult End<TAsyncResult>(IAsyncResult result) where TAsyncResult : AsyncResult
    {
      if (result == null)
        throw Fx.Exception.ArgumentNull(nameof (result));
      if (!(result is TAsyncResult asyncResult))
        throw Fx.Exception.Argument(nameof (result), SRCore.InvalidAsyncResult);
      asyncResult.endCalled = !asyncResult.endCalled ? true : throw Fx.Exception.AsError((Exception) new InvalidOperationException(SRCore.AsyncResultAlreadyEnded));
      if (!asyncResult.isCompleted)
      {
        lock (asyncResult.ThisLock)
        {
          if (!asyncResult.isCompleted)
          {
            if (asyncResult.manualResetEvent == null)
              asyncResult.manualResetEvent = new ManualResetEvent(asyncResult.isCompleted);
          }
        }
      }
      if (asyncResult.manualResetEvent != null)
      {
        asyncResult.manualResetEvent.WaitOne();
        asyncResult.manualResetEvent.Close();
      }
      if (asyncResult.exception != null)
      {
        Fx.Exception.TraceException<Exception>(asyncResult.exception, asyncResult.TraceEventType, asyncResult.Activity);
        ExceptionDispatcher.Throw(asyncResult.exception);
      }
      return asyncResult;
    }

    private enum TransactionSignalState
    {
      Ready,
      Prepared,
      Completed,
      Abandoned,
    }

    [Serializable]
    private class TransactionSignalScope : SignalGate<IAsyncResult>, IDisposable
    {
      [NonSerialized]
      private TransactionScope transactionScope;
      [NonSerialized]
      private AsyncResult parent;
      private bool disposed;

      public TransactionSignalScope(AsyncResult result, Transaction transaction)
      {
        this.parent = result;
        this.transactionScope = Fx.CreateTransactionScope(transaction);
      }

      public AsyncResult.TransactionSignalState State { get; private set; }

      public bool IsPotentiallyAbandoned
      {
        get
        {
          if (this.State == AsyncResult.TransactionSignalState.Abandoned)
            return true;
          return this.State == AsyncResult.TransactionSignalState.Completed && !this.IsSignalled;
        }
      }

      public void Prepared()
      {
        if (this.State != AsyncResult.TransactionSignalState.Ready)
          AsyncResult.ThrowInvalidAsyncResult("PrepareAsyncCompletion should only be called once per PrepareTransactionalCall.");
        this.State = AsyncResult.TransactionSignalState.Prepared;
      }

      protected virtual void Dispose(bool disposing)
      {
        if (!disposing || this.disposed)
          return;
        this.disposed = true;
        if (this.State == AsyncResult.TransactionSignalState.Ready)
          this.State = AsyncResult.TransactionSignalState.Abandoned;
        else if (this.State == AsyncResult.TransactionSignalState.Prepared)
          this.State = AsyncResult.TransactionSignalState.Completed;
        else
          AsyncResult.ThrowInvalidAsyncResult("PrepareTransactionalCall should only be called in a using. Dispose called multiple times.");
        try
        {
          Fx.CompleteTransactionScope(ref this.transactionScope);
        }
        catch (Exception ex)
        {
          if (!Fx.IsFatal(ex))
            throw Fx.Exception.AsError((Exception) new InvalidOperationException(SRCore.AsyncTransactionException));
          throw;
        }
        IAsyncResult result;
        if (this.State != AsyncResult.TransactionSignalState.Completed || !this.Unlock(out result))
          return;
        if (this.parent.deferredTransactionalResult != null)
          AsyncResult.ThrowInvalidAsyncResult(this.parent.deferredTransactionalResult);
        this.parent.deferredTransactionalResult = result;
      }

      void IDisposable.Dispose()
      {
        this.Dispose(true);
        GC.SuppressFinalize((object) this);
      }
    }

    protected delegate bool AsyncCompletion(IAsyncResult result);
  }
}
