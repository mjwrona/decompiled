// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.Pool`1
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public class Pool<T> : IDisposable
  {
    private readonly Func<T> factory;
    private readonly Action<T> reset;
    private readonly ConcurrentBag<T> bag = new ConcurrentBag<T>();
    private int maxToKeep;
    private int countOverApproximation;
    private int lentOutBuffers;
    private bool disposedValue;

    internal int CountOverApproximation => this.countOverApproximation;

    internal int CountExactSlow => this.bag.Count;

    internal int LentOutBufferCount => this.lentOutBuffers;

    public Pool(Func<T> factory, Action<T> reset, int maxToKeep)
    {
      this.factory = factory;
      this.reset = reset;
      this.maxToKeep = maxToKeep;
    }

    public void Resize(int maxToKeep) => Interlocked.Exchange(ref this.maxToKeep, maxToKeep);

    public virtual Pool<T>.PoolHandle Get()
    {
      T obj;
      if (!this.TryTakeFromBag(out obj))
        obj = this.factory();
      Interlocked.Increment(ref this.lentOutBuffers);
      return new Pool<T>.PoolHandle(this, obj);
    }

    private void Return(T item)
    {
      Interlocked.Decrement(ref this.lentOutBuffers);
      if (this.countOverApproximation < this.maxToKeep)
      {
        this.reset(item);
        this.AddToBag(item);
      }
      else
      {
        if (!(item is IDisposable disposable))
          return;
        disposable.Dispose();
      }
    }

    private void AddToBag(T item)
    {
      Interlocked.Increment(ref this.countOverApproximation);
      this.bag.Add(item);
    }

    private bool TryTakeFromBag(out T item)
    {
      if (!this.bag.TryTake(out item))
        return false;
      Interlocked.Decrement(ref this.countOverApproximation);
      return true;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      if (disposing)
      {
        T result;
        while (this.bag.TryTake(out result))
        {
          if (result is IDisposable disposable)
            disposable.Dispose();
        }
      }
      this.disposedValue = true;
    }

    public void Dispose() => this.Dispose(true);

    public struct PoolHandle : IPoolHandle<T>, IDisposable
    {
      private readonly Pool<T> pool;
      private readonly T value;
      private bool disposed;

      public PoolHandle(Pool<T> pool, T value)
      {
        this.pool = pool;
        this.value = value;
        this.disposed = false;
      }

      public T Value
      {
        get
        {
          this.AssertValid();
          return this.value;
        }
      }

      public void AssertValid()
      {
        if (this.disposed)
          throw new ObjectDisposedException(this.GetType().FullName);
      }

      public void Dispose()
      {
        if (this.disposed)
          return;
        try
        {
          this.pool.Return(this.value);
        }
        catch (ObjectDisposedException ex)
        {
        }
        this.disposed = true;
      }
    }
  }
}
