// Decompiled with JetBrains decompiler
// Type: System.Threading.Async.AsyncSemaphore
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace System.Threading.Async
{
  [DebuggerDisplay("CurrentCount={CurrentCount}, MaximumCount={MaximumCount}, WaitingCount={WaitingCount}")]
  internal sealed class AsyncSemaphore : IDisposable
  {
    private int _currentCount;
    private int _maxCount;
    private System.Collections.Generic.Queue<TaskCompletionSource<object>> _waitingTasks;

    public AsyncSemaphore()
      : this(0)
    {
    }

    public AsyncSemaphore(int initialCount)
      : this(initialCount, int.MaxValue)
    {
    }

    public AsyncSemaphore(int initialCount, int maxCount)
    {
      if (maxCount <= 0)
        throw new ArgumentOutOfRangeException(nameof (maxCount));
      this._currentCount = initialCount <= maxCount && initialCount >= 0 ? initialCount : throw new ArgumentOutOfRangeException(nameof (initialCount));
      this._maxCount = maxCount;
      this._waitingTasks = new System.Collections.Generic.Queue<TaskCompletionSource<object>>();
    }

    public int CurrentCount => this._currentCount;

    public int MaximumCount => this._maxCount;

    public int WaitingCount
    {
      get
      {
        lock (this._waitingTasks)
          return this._waitingTasks.Count;
      }
    }

    public Task WaitAsync()
    {
      this.ThrowIfDisposed();
      lock (this._waitingTasks)
      {
        if (this._currentCount > 0)
        {
          --this._currentCount;
          return CompletedTask.Default;
        }
        TaskCompletionSource<object> completionSource = new TaskCompletionSource<object>();
        this._waitingTasks.Enqueue(completionSource);
        return (Task) completionSource.Task;
      }
    }

    public Task Queue(Action action) => this.WaitAsync().ContinueWith((Action<Task>) (_ =>
    {
      try
      {
        action();
      }
      finally
      {
        this.Release();
      }
    }));

    public Task<TResult> Queue<TResult>(Func<TResult> function) => this.WaitAsync().ContinueWith<TResult>((Func<Task, TResult>) (_ =>
    {
      try
      {
        return function();
      }
      finally
      {
        this.Release();
      }
    }));

    public void Release()
    {
      this.ThrowIfDisposed();
      TaskCompletionSource<object> completionSource = (TaskCompletionSource<object>) null;
      lock (this._waitingTasks)
      {
        if (this._currentCount == this._maxCount)
          throw new SemaphoreFullException();
        if (this._waitingTasks.Count > 0)
          completionSource = this._waitingTasks.Dequeue();
        else
          ++this._currentCount;
      }
      completionSource?.SetResult((object) null);
    }

    public void CancelAllExisting()
    {
      List<TaskCompletionSource<object>> completionSourceList = new List<TaskCompletionSource<object>>();
      lock (this._waitingTasks)
      {
        while (this._waitingTasks.Count > 0)
          completionSourceList.Add(this._waitingTasks.Dequeue());
      }
      completionSourceList.ForEach((Action<TaskCompletionSource<object>>) (task => task.TrySetCanceled()));
    }

    private void ThrowIfDisposed()
    {
      if (this._maxCount <= 0)
        throw new ObjectDisposedException(this.GetType().Name);
    }

    public void Dispose()
    {
      if (this._maxCount <= 0)
        return;
      this._maxCount = 0;
      lock (this._waitingTasks)
      {
        while (this._waitingTasks.Count > 0)
          this._waitingTasks.Dequeue().SetCanceled();
      }
    }
  }
}
