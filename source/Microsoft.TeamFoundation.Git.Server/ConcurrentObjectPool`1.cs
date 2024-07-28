// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ConcurrentObjectPool`1
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class ConcurrentObjectPool<T> : IDisposable where T : class
  {
    private readonly Func<T> m_create;
    private int m_minCount;
    private int m_capacity;
    private readonly QueuedActionLimiter m_pruneLimiter;
    private readonly ConcurrentStack<T> m_pool;
    private readonly Predicate<T> m_takeValidator;
    private readonly Predicate<T> m_returnValidator;

    public ConcurrentObjectPool(
      Func<T> create,
      TimeSpan pruneInterval,
      int minCount = 1,
      int capacity = 50,
      Predicate<T> takeValidator = null,
      Predicate<T> returnValidator = null)
    {
      this.m_create = create;
      this.m_minCount = minCount;
      this.m_capacity = capacity;
      this.m_takeValidator = takeValidator;
      this.m_returnValidator = returnValidator;
      this.m_pruneLimiter = new QueuedActionLimiter(new WaitCallback(this.Prune), true, (long) pruneInterval.TotalMilliseconds);
      this.m_pool = new ConcurrentStack<T>();
    }

    public void Dispose()
    {
      this.m_minCount = 0;
      this.m_capacity = 0;
      T result;
      while (this.m_pool.TryPop(out result))
      {
        if (result is IDisposable disposable)
          disposable.Dispose();
      }
    }

    public T Take()
    {
      T result;
      if (this.m_pool.TryPop(out result))
      {
        if (this.m_takeValidator == null || this.m_takeValidator(result))
          return result;
        if (result is IDisposable disposable)
          disposable.Dispose();
      }
      return this.m_create();
    }

    public void Return(T instance)
    {
      if (this.m_pool.Count < this.m_capacity && (this.m_returnValidator == null || this.m_returnValidator(instance)))
        this.m_pool.Push(instance);
      else if (instance is IDisposable disposable)
        disposable.Dispose();
      if (this.m_pool.Count <= this.m_minCount)
        return;
      this.m_pruneLimiter.QueueAction();
    }

    private void Prune(object state)
    {
      int num = Math.Max(this.m_pool.Count / 2, this.m_minCount);
      T result;
      while (this.m_pool.Count > num && this.m_pool.TryPop(out result))
      {
        if (result is IDisposable disposable)
          disposable.Dispose();
      }
      if (this.m_pool.Count <= this.m_minCount)
        return;
      this.m_pruneLimiter.QueueAction();
    }

    internal void TEST_SuppressPrune() => this.m_pruneLimiter.Suppress();

    internal void TEST_UnsuppressPrune() => this.m_pruneLimiter.Unsuppress();

    internal int TEST_Count => this.m_pool.Count;
  }
}
