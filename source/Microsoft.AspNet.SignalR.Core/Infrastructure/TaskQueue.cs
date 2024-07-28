// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.TaskQueue
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  internal sealed class TaskQueue
  {
    private readonly object _lockObj = new object();
    private Task _lastQueuedTask;
    private volatile bool _drained;
    private readonly int? _maxSize;
    private long _size;

    public TaskQueue()
      : this(TaskAsyncHelper.Empty)
    {
    }

    public TaskQueue(Task initialTask) => this._lastQueuedTask = initialTask;

    public TaskQueue(Task initialTask, int maxSize)
    {
      this._lastQueuedTask = initialTask;
      this._maxSize = new int?(maxSize);
    }

    public IPerformanceCounter QueueSizeCounter { get; set; }

    public bool IsDrained => this._drained;

    public Task Enqueue(Func<object, Task> taskFunc, object state)
    {
      lock (this._lockObj)
      {
        if (this._drained)
          return this._lastQueuedTask;
        if (this._maxSize.HasValue)
        {
          long num = Interlocked.Increment(ref this._size);
          int? maxSize = this._maxSize;
          long? nullable = maxSize.HasValue ? new long?((long) maxSize.GetValueOrDefault()) : new long?();
          long valueOrDefault = nullable.GetValueOrDefault();
          if (num > valueOrDefault & nullable.HasValue)
          {
            Interlocked.Decrement(ref this._size);
            return (Task) null;
          }
          this.QueueSizeCounter?.Increment();
        }
        Task task = this._lastQueuedTask.Then<Func<object, Task>, object, TaskQueue>((Func<Func<object, Task>, object, TaskQueue, Task>) ((n, ns, q) => q.InvokeNext(n, ns)), taskFunc, state, this);
        this._lastQueuedTask = task;
        return task;
      }
    }

    private Task InvokeNext(Func<object, Task> next, object nextState) => next(nextState).Finally((Action<object>) (s => ((TaskQueue) s).Dequeue()), (object) this);

    private void Dequeue()
    {
      if (!this._maxSize.HasValue)
        return;
      Interlocked.Decrement(ref this._size);
      this.QueueSizeCounter?.Decrement();
    }

    public Task Enqueue(Func<Task> taskFunc) => this.Enqueue((Func<object, Task>) (state => ((Func<Task>) state)()), (object) taskFunc);

    public Task Drain()
    {
      lock (this._lockObj)
      {
        this._drained = true;
        return this._lastQueuedTask;
      }
    }
  }
}
