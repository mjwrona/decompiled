// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Messaging.ScaleoutStream
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Messaging
{
  internal class ScaleoutStream
  {
    private TaskCompletionSource<object> _taskCompletionSource;
    private static Task _initializeDrainTask;
    private TaskQueue _queue;
    private ScaleoutStream.StreamState _state;
    private ExceptionDispatchInfo _error;
    private readonly int _size;
    private readonly QueuingBehavior _queueBehavior;
    private readonly TraceSource _trace;
    private readonly string _tracePrefix;
    private readonly IPerformanceCounterManager _perfCounters;
    private readonly object _lockObj = new object();

    public ScaleoutStream(
      TraceSource trace,
      string tracePrefix,
      QueuingBehavior queueBehavior,
      int size,
      IPerformanceCounterManager performanceCounters)
    {
      this._trace = trace != null ? trace : throw new ArgumentNullException(nameof (trace));
      this._tracePrefix = tracePrefix;
      this._size = size;
      this._perfCounters = performanceCounters;
      this._queueBehavior = queueBehavior;
      this.InitializeCore();
    }

    private bool UsingTaskQueue
    {
      get
      {
        if (this._queueBehavior == QueuingBehavior.Always)
          return true;
        return this._queueBehavior == QueuingBehavior.InitialOnly && this._state == ScaleoutStream.StreamState.Initial;
      }
    }

    public void Open()
    {
      lock (this._lockObj)
      {
        bool usingTaskQueue = this.UsingTaskQueue;
        ScaleoutStream.StreamState previousState;
        if (!this.ChangeState(ScaleoutStream.StreamState.Open, out previousState))
          return;
        this._perfCounters.ScaleoutStreamCountOpen.Increment();
        this._perfCounters.ScaleoutStreamCountBuffering.Decrement();
        this._error = (ExceptionDispatchInfo) null;
        if (!usingTaskQueue)
          return;
        this.EnsureQueueStarted();
        if (previousState != ScaleoutStream.StreamState.Initial || this._queueBehavior != QueuingBehavior.InitialOnly)
          return;
        ScaleoutStream._initializeDrainTask = ScaleoutStream.Drain(this._queue, this._trace);
      }
    }

    public Task Send(Func<object, Task> send, object state)
    {
      lock (this._lockObj)
      {
        if (this._error != null)
          this._error.Throw();
        if (this._state == ScaleoutStream.StreamState.Closed)
          throw new InvalidOperationException(Resources.Error_StreamClosed);
        ScaleoutStream.SendContext state1 = new ScaleoutStream.SendContext(this, send, state);
        if (ScaleoutStream._initializeDrainTask != null && !ScaleoutStream._initializeDrainTask.IsCompleted)
          ScaleoutStream._initializeDrainTask.Wait();
        if (!this.UsingTaskQueue)
          return ScaleoutStream.Send((object) state1);
        return (this._queue.Enqueue(new Func<object, Task>(ScaleoutStream.Send), (object) state1) ?? throw new InvalidOperationException(Resources.Error_TaskQueueFull)).Catch<Task>(this._trace);
      }
    }

    public void SetError(Exception error)
    {
      this.Trace(TraceEventType.Error, "Error has happened with the following exception: {0}.", (object) error);
      lock (this._lockObj)
      {
        this._perfCounters.ScaleoutErrorsTotal.Increment();
        this._perfCounters.ScaleoutErrorsPerSec.Increment();
        this.Buffer();
        this._error = ExceptionDispatchInfo.Capture(error);
      }
    }

    public void Close()
    {
      Task task = TaskAsyncHelper.Empty;
      lock (this._lockObj)
      {
        if (this.ChangeState(ScaleoutStream.StreamState.Closed))
        {
          this._perfCounters.ScaleoutStreamCountOpen.RawValue = 0L;
          this._perfCounters.ScaleoutStreamCountBuffering.RawValue = 0L;
          if (this.UsingTaskQueue)
          {
            this.EnsureQueueStarted();
            task = ScaleoutStream.Drain(this._queue, this._trace);
          }
        }
      }
      if (!this.UsingTaskQueue)
        return;
      task.Wait();
    }

    private static Task Send(object state)
    {
      ScaleoutStream.SendContext state1 = (ScaleoutStream.SendContext) state;
      state1.InvokeSend().Then<DispatchingTaskCompletionSource<object>>((Action<DispatchingTaskCompletionSource<object>>) (tcs => tcs.TrySetResult((object) null)), state1.TaskCompletionSource).Catch<Task>((Action<AggregateException, object>) ((ex, obj) =>
      {
        ScaleoutStream.SendContext sendContext = (ScaleoutStream.SendContext) obj;
        sendContext.Stream.Trace(TraceEventType.Error, "Send failed: {0}", (object) ex);
        lock (sendContext.Stream._lockObj)
        {
          sendContext.Stream.SetError(ex.InnerException);
          sendContext.TaskCompletionSource.TrySetUnwrappedException((Exception) ex);
        }
      }), (object) state1, state1.Stream._trace);
      return (Task) state1.TaskCompletionSource.Task;
    }

    private void Buffer()
    {
      lock (this._lockObj)
      {
        if (!this.ChangeState(ScaleoutStream.StreamState.Buffering))
          return;
        this._perfCounters.ScaleoutStreamCountOpen.Decrement();
        this._perfCounters.ScaleoutStreamCountBuffering.Increment();
        this.InitializeCore();
      }
    }

    private void InitializeCore()
    {
      if (!this.UsingTaskQueue)
        return;
      this._queue = new TaskQueue(this.DrainPreviousQueue(), this._size);
      this._queue.QueueSizeCounter = this._perfCounters.ScaleoutSendQueueLength;
    }

    private Task DrainPreviousQueue()
    {
      if (this._taskCompletionSource == null || this._taskCompletionSource.Task.IsCompleted)
        this._taskCompletionSource = new TaskCompletionSource<object>();
      return this._queue != null ? this._taskCompletionSource.Task.Then<TaskQueue, TraceSource>((Func<TaskQueue, TraceSource, Task>) ((q, t) => ScaleoutStream.Drain(q, t)), this._queue, this._trace) : (Task) this._taskCompletionSource.Task;
    }

    private void EnsureQueueStarted()
    {
      if (this._taskCompletionSource == null)
        return;
      this._taskCompletionSource.TrySetResult((object) null);
    }

    private bool ChangeState(ScaleoutStream.StreamState newState) => this.ChangeState(newState, out ScaleoutStream.StreamState _);

    private bool ChangeState(
      ScaleoutStream.StreamState newState,
      out ScaleoutStream.StreamState previousState)
    {
      previousState = this._state;
      if (this._state == ScaleoutStream.StreamState.Closed || this._state == newState)
        return false;
      this.Trace(TraceEventType.Information, "Changed state from {0} to {1}", (object) this._state, (object) newState);
      this._state = newState;
      return true;
    }

    private static Task Drain(TaskQueue queue, TraceSource traceSource)
    {
      if (queue == null)
        return TaskAsyncHelper.Empty;
      TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
      queue.Drain().Catch<Task>(traceSource).ContinueWith((Action<Task>) (task => tcs.TrySetResult((object) null)));
      return (Task) tcs.Task;
    }

    private void Trace(TraceEventType traceEventType, string value, params object[] args) => this._trace.TraceEvent(traceEventType, 0, this._tracePrefix + " - " + value, args);

    private class SendContext
    {
      private readonly Func<object, Task> _send;
      private readonly object _state;
      public readonly ScaleoutStream Stream;
      public readonly DispatchingTaskCompletionSource<object> TaskCompletionSource;

      public SendContext(ScaleoutStream stream, Func<object, Task> send, object state)
      {
        this.Stream = stream;
        this.TaskCompletionSource = new DispatchingTaskCompletionSource<object>();
        this._send = send;
        this._state = state;
      }

      public Task InvokeSend()
      {
        try
        {
          return this._send(this._state);
        }
        catch (Exception ex)
        {
          return TaskAsyncHelper.FromError(ex);
        }
      }
    }

    private enum StreamState
    {
      Initial,
      Open,
      Buffering,
      Closed,
    }
  }
}
