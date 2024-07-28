// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Common.AsyncCache`2
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Common
{
  internal sealed class AsyncCache<TKey, TValue>
  {
    private readonly IEqualityComparer<TValue> valueEqualityComparer;
    private readonly IEqualityComparer<TKey> keyEqualityComparer;
    private ConcurrentDictionary<TKey, AsyncLazy<TValue>> values;

    public AsyncCache(
      IEqualityComparer<TValue> valueEqualityComparer,
      IEqualityComparer<TKey> keyEqualityComparer = null)
    {
      this.keyEqualityComparer = keyEqualityComparer ?? (IEqualityComparer<TKey>) EqualityComparer<TKey>.Default;
      this.values = new ConcurrentDictionary<TKey, AsyncLazy<TValue>>(this.keyEqualityComparer);
      this.valueEqualityComparer = valueEqualityComparer;
    }

    public AsyncCache()
      : this((IEqualityComparer<TValue>) EqualityComparer<TValue>.Default)
    {
    }

    public ICollection<TKey> Keys => this.values.Keys;

    public void Set(TKey key, TValue value)
    {
      AsyncLazy<TValue> lazyValue = new AsyncLazy<TValue>(value);
      TValue result = lazyValue.Value.Result;
      this.values.AddOrUpdate(key, lazyValue, (Func<TKey, AsyncLazy<TValue>, AsyncLazy<TValue>>) ((k, existingValue) =>
      {
        if (existingValue.IsValueCreated)
          existingValue.Value.ContinueWith<AggregateException>((Func<Task<TValue>, AggregateException>) (c => c.Exception), TaskContinuationOptions.OnlyOnFaulted);
        return lazyValue;
      }));
    }

    public async Task<TValue> GetAsync(
      TKey key,
      TValue obsoleteValue,
      Func<Task<TValue>> singleValueInitFunc,
      CancellationToken cancellationToken,
      bool forceRefresh = false)
    {
      cancellationToken.ThrowIfCancellationRequested();
      AsyncLazy<TValue> initialLazyValue;
      if (this.values.TryGetValue(key, out initialLazyValue))
      {
        if (!initialLazyValue.IsValueCreated || !initialLazyValue.Value.IsCompleted)
        {
          try
          {
            return await initialLazyValue.Value;
          }
          catch
          {
          }
        }
        else if (initialLazyValue.Value.Exception == null && !initialLazyValue.Value.IsCanceled)
        {
          TValue x = await initialLazyValue.Value;
          if (!forceRefresh && !this.valueEqualityComparer.Equals(x, obsoleteValue))
            return x;
        }
      }
      AsyncLazy<TValue> newLazyValue = new AsyncLazy<TValue>(singleValueInitFunc, cancellationToken);
      AsyncLazy<TValue> actualValue = this.values.AddOrUpdate(key, newLazyValue, (Func<TKey, AsyncLazy<TValue>, AsyncLazy<TValue>>) ((existingKey, existingValue) => existingValue != initialLazyValue ? existingValue : newLazyValue));
      Task<TValue> task = actualValue.Value;
      task.ContinueWith<AggregateException>((Func<Task<TValue>, AggregateException>) (c => c.Exception), TaskContinuationOptions.OnlyOnFaulted);
      try
      {
        return await task;
      }
      catch (Exception ex) when (actualValue == newLazyValue)
      {
        this.TryRemoveValue(key, actualValue);
        throw;
      }
    }

    public void Remove(TKey key)
    {
      AsyncLazy<TValue> asyncLazy;
      if (!this.values.TryRemove(key, out asyncLazy) || !asyncLazy.IsValueCreated)
        return;
      asyncLazy.Value.ContinueWith<AggregateException>((Func<Task<TValue>, AggregateException>) (c => c.Exception), TaskContinuationOptions.OnlyOnFaulted);
    }

    public bool TryRemoveIfCompleted(TKey key)
    {
      AsyncLazy<TValue> initialLazyValue;
      if (!this.values.TryGetValue(key, out initialLazyValue) || !initialLazyValue.IsValueCreated || !initialLazyValue.Value.IsCompleted)
        return false;
      AggregateException exception = initialLazyValue.Value.Exception;
      return this.TryRemoveValue(key, initialLazyValue);
    }

    private bool TryRemoveValue(TKey key, AsyncLazy<TValue> initialLazyValue)
    {
      ConcurrentDictionary<TKey, AsyncLazy<TValue>> values = this.values;
      return values != null && ((ICollection<KeyValuePair<TKey, AsyncLazy<TValue>>>) values).Remove(new KeyValuePair<TKey, AsyncLazy<TValue>>(key, initialLazyValue));
    }

    public async Task<TValue> RemoveAsync(TKey key)
    {
      int num;
      AsyncLazy<TValue> asyncLazy;
      if (num == 0 || this.values.TryRemove(key, out asyncLazy))
      {
        try
        {
          return await asyncLazy.Value;
        }
        catch
        {
        }
      }
      return default (TValue);
    }

    public void Clear()
    {
      ConcurrentDictionary<TKey, AsyncLazy<TValue>> concurrentDictionary = Interlocked.Exchange<ConcurrentDictionary<TKey, AsyncLazy<TValue>>>(ref this.values, new ConcurrentDictionary<TKey, AsyncLazy<TValue>>(this.keyEqualityComparer));
      foreach (AsyncLazy<TValue> asyncLazy in (IEnumerable<AsyncLazy<TValue>>) concurrentDictionary.Values)
      {
        if (asyncLazy.IsValueCreated)
          asyncLazy.Value.ContinueWith<AggregateException>((Func<Task<TValue>, AggregateException>) (c => c.Exception), TaskContinuationOptions.OnlyOnFaulted);
      }
      concurrentDictionary.Clear();
    }

    public void BackgroundRefreshNonBlocking(TKey key, Func<Task<TValue>> singleValueInitFunc) => Task.Factory.StartNewOnCurrentTaskSchedulerAsync<Task>((Func<Task>) (async () =>
    {
      try
      {
        AsyncLazy<TValue> asyncLazy;
        if (this.values.TryGetValue(key, out asyncLazy) && (!asyncLazy.IsValueCreated || !asyncLazy.Value.IsCompleted))
          return;
        TValue async = await this.GetAsync(key, default (TValue), singleValueInitFunc, CancellationToken.None, true);
      }
      catch
      {
      }
    })).Unwrap();
  }
}
