// Decompiled with JetBrains decompiler
// Type: Nest.GetEnumerator`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Nest
{
  internal class GetEnumerator<TSource> : 
    IEnumerator<TSource>,
    IDisposable,
    IEnumerator,
    IObserver<TSource>
  {
    private readonly SemaphoreSlim _gate;
    private readonly ConcurrentQueue<TSource> _queue;
    private TSource _current;
    private bool _disposed;
    private Exception _error;
    private IDisposable _subscription;

    public GetEnumerator()
    {
      this._queue = new ConcurrentQueue<TSource>();
      this._gate = new SemaphoreSlim(0);
    }

    public TSource Current => this._current;

    object IEnumerator.Current => (object) this._current;

    public void Dispose()
    {
      this._subscription.Dispose();
      this._disposed = true;
      this._gate.Release();
    }

    public bool MoveNext()
    {
      this._gate.Wait();
      if (this._disposed)
        throw new ObjectDisposedException("");
      if (this._queue.TryDequeue(out this._current))
        return true;
      if (this._error != null)
        throw this._error;
      this._gate.Release();
      return false;
    }

    public void Reset() => throw new NotSupportedException();

    public void OnCompleted()
    {
      this._subscription.Dispose();
      this._gate.Release();
    }

    public void OnError(Exception error)
    {
      this._error = error;
      this._subscription.Dispose();
      this._gate.Release();
    }

    public virtual void OnNext(TSource value)
    {
      this._queue.Enqueue(value);
      this._gate.Release();
    }

    private IEnumerator<TSource> Run(IObservable<TSource> source)
    {
      this._subscription = source.Subscribe((IObserver<TSource>) this);
      return (IEnumerator<TSource>) this;
    }

    public IEnumerable<TSource> ToEnumerable(IObservable<TSource> source) => (IEnumerable<TSource>) new GetEnumerator<TSource>.AnonymousEnumerable<TSource>((Func<IEnumerator<TSource>>) (() => this.Run(source)));

    internal sealed class AnonymousEnumerable<T> : IEnumerable<T>, IEnumerable
    {
      private readonly Func<IEnumerator<T>> _getEnumerator;

      public AnonymousEnumerable(Func<IEnumerator<T>> getEnumerator) => this._getEnumerator = getEnumerator;

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

      public IEnumerator<T> GetEnumerator() => this._getEnumerator();
    }
  }
}
