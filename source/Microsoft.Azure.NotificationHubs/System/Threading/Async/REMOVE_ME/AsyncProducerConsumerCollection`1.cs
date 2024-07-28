// Decompiled with JetBrains decompiler
// Type: System.Threading.Async.REMOVE_ME.AsyncProducerConsumerCollection`1
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace System.Threading.Async.REMOVE_ME
{
  [DebuggerDisplay("Count={CurrentCount}")]
  internal sealed class AsyncProducerConsumerCollection<T> : IDisposable
  {
    private AsyncSemaphore _semaphore = new AsyncSemaphore();
    private IProducerConsumerCollection<T> _collection;

    public AsyncProducerConsumerCollection()
      : this((IProducerConsumerCollection<T>) new ConcurrentQueue<T>())
    {
    }

    public AsyncProducerConsumerCollection(IProducerConsumerCollection<T> collection) => this._collection = collection != null ? collection : throw new ArgumentNullException(nameof (collection));

    public void Add(T item)
    {
      if (!this._collection.TryAdd(item))
        throw new InvalidOperationException("Invalid collection");
      this._semaphore.Release();
    }

    public Task<T> Take() => this._semaphore.WaitAsync().ContinueWith<T>((Func<Task, T>) (_ =>
    {
      T obj;
      if (!this._collection.TryTake(out obj))
        throw new InvalidOperationException("Invalid collection");
      return obj;
    }), TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);

    public int Count => this._collection.Count;

    public void CancelAllExisting() => this._semaphore.CancelAllExisting();

    public void Dispose()
    {
      if (this._semaphore == null)
        return;
      this._semaphore.Dispose();
      this._semaphore = (AsyncSemaphore) null;
    }
  }
}
