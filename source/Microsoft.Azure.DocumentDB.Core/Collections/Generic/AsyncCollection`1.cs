// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Collections.Generic.AsyncCollection`1
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Collections.Generic
{
  internal sealed class AsyncCollection<T>
  {
    private readonly IProducerConsumerCollection<T> collection;
    private readonly int boundingCapacity;
    private readonly SemaphoreSlim notFull;
    private readonly SemaphoreSlim notEmpty;
    private readonly AsyncCollection<T>.TryPeekDelegate tryPeekDelegate;

    public AsyncCollection()
      : this((IProducerConsumerCollection<T>) new ConcurrentQueue<T>(), int.MaxValue)
    {
    }

    public AsyncCollection(int boundingCapacity)
      : this((IProducerConsumerCollection<T>) new ConcurrentQueue<T>(), boundingCapacity)
    {
    }

    public AsyncCollection(IProducerConsumerCollection<T> collection)
      : this(collection, int.MaxValue)
    {
    }

    public AsyncCollection(IProducerConsumerCollection<T> collection, int boundingCapacity)
    {
      if (collection == null)
        throw new ArgumentNullException(nameof (collection));
      if (boundingCapacity < 1)
        throw new ArgumentOutOfRangeException("boundedCapacity is not a positive value.");
      int count = collection.Count;
      if (boundingCapacity < count)
        throw new ArgumentOutOfRangeException("boundedCapacity is less than the size of collection.");
      this.collection = collection;
      this.boundingCapacity = boundingCapacity;
      this.notFull = this.IsUnbounded ? (SemaphoreSlim) null : new SemaphoreSlim(boundingCapacity - count, boundingCapacity);
      this.notEmpty = new SemaphoreSlim(count);
      MethodInfo method = CustomTypeExtensions.GetMethod(this.collection.GetType(), "TryPeek", BindingFlags.Instance | BindingFlags.Public);
      this.tryPeekDelegate = (object) method == null ? (AsyncCollection<T>.TryPeekDelegate) null : (AsyncCollection<T>.TryPeekDelegate) CustomTypeExtensions.CreateDelegate(typeof (AsyncCollection<T>.TryPeekDelegate), (object) this.collection, method);
    }

    public int Count => this.collection.Count;

    public bool IsUnbounded => this.boundingCapacity >= int.MaxValue;

    public async Task AddAsync(T item, CancellationToken token = default (CancellationToken))
    {
      if (!this.IsUnbounded)
        await this.notFull.WaitAsync(token);
      if (!this.collection.TryAdd(item))
        return;
      this.notEmpty.Release();
    }

    public async Task AddRangeAsync(IEnumerable<T> items, CancellationToken token = default (CancellationToken))
    {
      if (!this.IsUnbounded)
      {
        foreach (T obj in items)
          await this.AddAsync(obj);
      }
      else
      {
        int releaseCount = 0;
        foreach (T obj in items)
        {
          if (this.collection.TryAdd(obj))
            ++releaseCount;
        }
        if (releaseCount <= 0)
          return;
        this.notEmpty.Release(releaseCount);
      }
    }

    public async Task<T> TakeAsync(CancellationToken token = default (CancellationToken))
    {
      await this.notEmpty.WaitAsync(token);
      T async;
      if (this.collection.TryTake(out async) && !this.IsUnbounded)
        this.notFull.Release();
      return async;
    }

    public async Task<T> PeekAsync(CancellationToken token = default (CancellationToken))
    {
      if (this.tryPeekDelegate == null)
        throw new NotImplementedException();
      await this.notEmpty.WaitAsync(token);
      T obj;
      int num = this.tryPeekDelegate(out obj) ? 1 : 0;
      this.notEmpty.Release();
      return obj;
    }

    public bool TryPeek(out T item)
    {
      if (this.tryPeekDelegate == null)
        throw new NotImplementedException();
      return this.tryPeekDelegate(out item);
    }

    public async Task<IReadOnlyList<T>> DrainAsync(
      int maxElements = 2147483647,
      TimeSpan timeout = default (TimeSpan),
      Func<T, bool> callback = null,
      CancellationToken token = default (CancellationToken))
    {
      if (maxElements < 1)
        throw new ArgumentOutOfRangeException("maxElements is not a positive value.");
      List<T> elements = new List<T>();
      Stopwatch stopWatch = Stopwatch.StartNew();
      while (true)
      {
        bool flag = elements.Count < maxElements;
        if (flag)
          flag = await this.notEmpty.WaitAsync(timeout, token);
        T obj;
        if (flag && this.collection.TryTake(out obj) && (callback == null || callback(obj)))
        {
          elements.Add(obj);
          timeout.Subtract(TimeSpan.FromTicks(Math.Min(stopWatch.ElapsedTicks, timeout.Ticks)));
          stopWatch.Restart();
        }
        else
          break;
      }
      if (!this.IsUnbounded && elements.Count > 0)
        this.notFull.Release(elements.Count);
      return (IReadOnlyList<T>) elements;
    }

    private delegate bool TryPeekDelegate(out T item);
  }
}
