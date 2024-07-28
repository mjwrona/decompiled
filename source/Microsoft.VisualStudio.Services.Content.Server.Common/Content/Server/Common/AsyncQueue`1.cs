// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.AsyncQueue`1
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public sealed class AsyncQueue<T> : IDisposable
  {
    private readonly ConcurrentQueue<T> queue = new ConcurrentQueue<T>();
    private readonly SemaphoreSlim enqueuedSignal = new SemaphoreSlim(0, int.MaxValue);
    private readonly SafeTaskCompletionSource<int> completedAddingTcs = new SafeTaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);

    public bool IsCompleted => this.completedAddingTcs.Task.IsCompleted;

    public bool IsEmpty => this.queue.IsEmpty;

    public void Enqueue(T item)
    {
      this.queue.Enqueue(item);
      this.enqueuedSignal.Release();
    }

    public void CompleteAdding() => this.completedAddingTcs.SetResult(0);

    public void Dispose()
    {
      this.completedAddingTcs.TrySetException((Exception) new ObjectDisposedException("AsyncQueue is disposing"));
      this.enqueuedSignal.Dispose();
    }

    public IEnumerable<T> GetReader()
    {
      bool addingComplete;
      do
      {
        addingComplete = Task.WaitAny((Task) this.completedAddingTcs.Task, this.enqueuedSignal.WaitAsync()) == 0;
        T result;
        while (this.queue.TryDequeue(out result))
          yield return result;
      }
      while (!addingComplete);
    }

    public IConcurrentIterator<T> GetAsyncReader() => (IConcurrentIterator<T>) new AsyncQueue<T>.Enumerator(this);

    private sealed class Enumerator : IConcurrentIterator<T>, IDisposable
    {
      private readonly AsyncQueue<T> queue;

      public Enumerator(AsyncQueue<T> queue) => this.queue = queue;

      public T Current { get; private set; }

      public bool EnumerationStarted { get; private set; }

      public void Dispose()
      {
      }

      public async Task<bool> MoveNextAsync(CancellationToken token)
      {
        this.EnumerationStarted = true;
        Task task;
        do
        {
          task = await Task.WhenAny((Task) this.queue.completedAddingTcs.Task, this.queue.enqueuedSignal.WaitAsync(token)).ConfigureAwait(false);
          bool flag = task == this.queue.completedAddingTcs.Task;
          T result;
          if (this.queue.queue.TryDequeue(out result))
          {
            this.Current = result;
            return true;
          }
          if (!flag)
          {
            if (task.IsCanceled)
              throw new OperationCanceledException();
          }
          else
            goto label_9;
        }
        while (!task.IsFaulted);
        throw task.Exception;
label_9:
        return false;
      }
    }
  }
}
