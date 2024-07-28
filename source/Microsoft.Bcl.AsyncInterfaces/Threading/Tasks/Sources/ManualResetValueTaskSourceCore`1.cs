// Decompiled with JetBrains decompiler
// Type: System.Threading.Tasks.Sources.ManualResetValueTaskSourceCore`1
// Assembly: Microsoft.Bcl.AsyncInterfaces, Version=7.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
// MVID: 8B2E828D-BD93-4580-BC63-F76024589A76
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Bcl.AsyncInterfaces.dll

using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;


#nullable enable
namespace System.Threading.Tasks.Sources
{
  [StructLayout(LayoutKind.Auto)]
  public struct ManualResetValueTaskSourceCore<TResult>
  {

    #nullable disable
    private Action<object> _continuation;
    private object _continuationState;
    private ExecutionContext _executionContext;
    private object _capturedContext;
    private bool _completed;
    private TResult _result;
    private ExceptionDispatchInfo _error;
    private short _version;

    public bool RunContinuationsAsynchronously { get; set; }

    public void Reset()
    {
      ++this._version;
      this._completed = false;
      this._result = default (TResult);
      this._error = (ExceptionDispatchInfo) null;
      this._executionContext = (ExecutionContext) null;
      this._capturedContext = (object) null;
      this._continuation = (Action<object>) null;
      this._continuationState = (object) null;
    }


    #nullable enable
    public void SetResult(TResult result)
    {
      this._result = result;
      this.SignalCompletion();
    }

    public void SetException(Exception error)
    {
      this._error = ExceptionDispatchInfo.Capture(error);
      this.SignalCompletion();
    }

    public short Version => this._version;

    public ValueTaskSourceStatus GetStatus(short token)
    {
      this.ValidateToken(token);
      if (this._continuation == null || !this._completed)
        return ValueTaskSourceStatus.Pending;
      if (this._error == null)
        return ValueTaskSourceStatus.Succeeded;
      return !(this._error.SourceException is OperationCanceledException) ? ValueTaskSourceStatus.Faulted : ValueTaskSourceStatus.Canceled;
    }

    public TResult GetResult(short token)
    {
      this.ValidateToken(token);
      if (!this._completed)
        throw new InvalidOperationException();
      this._error?.Throw();
      return this._result;
    }

    public void OnCompleted(
      Action<object?> continuation,
      object? state,
      short token,
      ValueTaskSourceOnCompletedFlags flags)
    {
      if (continuation == null)
        throw new ArgumentNullException(nameof (continuation));
      this.ValidateToken(token);
      if ((flags & ValueTaskSourceOnCompletedFlags.FlowExecutionContext) != ValueTaskSourceOnCompletedFlags.None)
        this._executionContext = ExecutionContext.Capture();
      if ((flags & ValueTaskSourceOnCompletedFlags.UseSchedulingContext) != ValueTaskSourceOnCompletedFlags.None)
      {
        SynchronizationContext current1 = SynchronizationContext.Current;
        if (current1 != null && current1.GetType() != typeof (SynchronizationContext))
        {
          this._capturedContext = (object) current1;
        }
        else
        {
          TaskScheduler current2 = TaskScheduler.Current;
          if (current2 != TaskScheduler.Default)
            this._capturedContext = (object) current2;
        }
      }
      object obj = (object) this._continuation;
      if (obj == null)
      {
        this._continuationState = state;
        obj = (object) Interlocked.CompareExchange<Action<object>>(ref this._continuation, continuation, (Action<object>) null);
      }
      if (obj == null)
        return;
      if (obj != ManualResetValueTaskSourceCoreShared.s_sentinel)
        throw new InvalidOperationException();
      switch (this._capturedContext)
      {
        case null:
          Task.Factory.StartNew(continuation, state, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
          break;
        case SynchronizationContext synchronizationContext:
          Tuple<Action<object>, object> state1 = Tuple.Create<Action<object>, object>(continuation, state);
          synchronizationContext.Post((SendOrPostCallback) (s =>
          {
            Tuple<Action<object>, object> tuple = (Tuple<Action<object>, object>) s;
            tuple.Item1(tuple.Item2);
          }), (object) state1);
          break;
        case TaskScheduler scheduler:
          Task.Factory.StartNew(continuation, state, CancellationToken.None, TaskCreationOptions.DenyChildAttach, scheduler);
          break;
      }
    }

    private void ValidateToken(short token)
    {
      if ((int) token != (int) this._version)
        throw new InvalidOperationException();
    }

    private void SignalCompletion()
    {
      this._completed = !this._completed ? true : throw new InvalidOperationException();
      if (this._continuation == null && Interlocked.CompareExchange<Action<object>>(ref this._continuation, ManualResetValueTaskSourceCoreShared.s_sentinel, (Action<object>) null) == null)
        return;
      if (this._executionContext != null)
        ExecutionContext.Run(this._executionContext, (ContextCallback) (s => ((ManualResetValueTaskSourceCore<TResult>) s).InvokeContinuation()), (object) this);
      else
        this.InvokeContinuation();
    }

    private void InvokeContinuation()
    {
      switch (this._capturedContext)
      {
        case null:
          if (this.RunContinuationsAsynchronously)
          {
            Task.Factory.StartNew(this._continuation, this._continuationState, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
            break;
          }
          this._continuation(this._continuationState);
          break;
        case SynchronizationContext synchronizationContext:
          Tuple<Action<object>, object> state = Tuple.Create<Action<object>, object>(this._continuation, this._continuationState);
          synchronizationContext.Post((SendOrPostCallback) (s =>
          {
            Tuple<Action<object>, object> tuple = (Tuple<Action<object>, object>) s;
            tuple.Item1(tuple.Item2);
          }), (object) state);
          break;
        case TaskScheduler scheduler:
          Task.Factory.StartNew(this._continuation, this._continuationState, CancellationToken.None, TaskCreationOptions.DenyChildAttach, scheduler);
          break;
      }
    }
  }
}
