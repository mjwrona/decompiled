// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.AsyncCacheNonBlocking`2
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class AsyncCacheNonBlocking<TKey, TValue> : IDisposable
  {
    private readonly CancellationTokenSource cancellationTokenSource;
    private readonly ConcurrentDictionary<TKey, AsyncCacheNonBlocking<TKey, TValue>.AsyncLazyWithRefreshTask<TValue>> values;
    private readonly Func<Exception, bool> removeFromCacheOnBackgroundRefreshException;
    private readonly IEqualityComparer<TKey> keyEqualityComparer;
    private bool isDisposed;

    public AsyncCacheNonBlocking(
      Func<Exception, bool> removeFromCacheOnBackgroundRefreshException = null,
      IEqualityComparer<TKey> keyEqualityComparer = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      this.keyEqualityComparer = keyEqualityComparer ?? (IEqualityComparer<TKey>) EqualityComparer<TKey>.Default;
      this.values = new ConcurrentDictionary<TKey, AsyncCacheNonBlocking<TKey, TValue>.AsyncLazyWithRefreshTask<TValue>>(this.keyEqualityComparer);
      this.removeFromCacheOnBackgroundRefreshException = removeFromCacheOnBackgroundRefreshException ?? new Func<Exception, bool>(AsyncCacheNonBlocking<TKey, TValue>.RemoveNotFoundFromCacheOnException);
      CancellationTokenSource cancellationTokenSource;
      if (!(cancellationToken == new CancellationToken()))
        cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
      else
        cancellationTokenSource = new CancellationTokenSource();
      this.cancellationTokenSource = cancellationTokenSource;
    }

    public AsyncCacheNonBlocking()
      : this((Func<Exception, bool>) null, (IEqualityComparer<TKey>) null, new CancellationToken())
    {
    }

    private static bool RemoveNotFoundFromCacheOnException(Exception e)
    {
      if (e is DocumentClientException documentClientException)
      {
        HttpStatusCode? statusCode = documentClientException.StatusCode;
        HttpStatusCode httpStatusCode = HttpStatusCode.NotFound;
        if (statusCode.GetValueOrDefault() == httpStatusCode & statusCode.HasValue)
          return true;
      }
      return e is CosmosException cosmosException && cosmosException.StatusCode == HttpStatusCode.NotFound;
    }

    public async Task<TValue> GetAsync(
      TKey key,
      Func<TValue, Task<TValue>> singleValueInitFunc,
      Func<TValue, bool> forceRefresh)
    {
      AsyncCacheNonBlocking<TKey, TValue>.AsyncLazyWithRefreshTask<TValue> initialLazyValue;
      if (this.values.TryGetValue(key, out initialLazyValue))
      {
        try
        {
          TValue valueAsync = await initialLazyValue.GetValueAsync();
          if (forceRefresh != null)
          {
            if (forceRefresh(valueAsync))
              goto label_8;
          }
          return valueAsync;
        }
        catch (Exception ex)
        {
          if (initialLazyValue.ShouldRemoveFromCacheThreadSafe())
          {
            bool flag = this.TryRemove(key);
            DefaultTrace.TraceError("AsyncCacheNonBlocking Failed GetAsync. key: {0}, tryRemoved: {1}, Exception: {2}", (object) key, (object) flag, (object) ex);
          }
          throw;
        }
label_8:
        try
        {
          return await initialLazyValue.CreateAndWaitForBackgroundRefreshTaskAsync(singleValueInitFunc);
        }
        catch (Exception ex)
        {
          if (initialLazyValue.ShouldRemoveFromCacheThreadSafe())
          {
            DefaultTrace.TraceError("AsyncCacheNonBlocking.GetAsync with ForceRefresh Failed. key: {0}, Exception: {1}", (object) key, (object) ex);
            if (this.removeFromCacheOnBackgroundRefreshException(ex))
              this.TryRemove(key);
          }
          throw;
        }
      }
      else
      {
        AsyncCacheNonBlocking<TKey, TValue>.AsyncLazyWithRefreshTask<TValue> lazyWithRefreshTask = new AsyncCacheNonBlocking<TKey, TValue>.AsyncLazyWithRefreshTask<TValue>(singleValueInitFunc, this.cancellationTokenSource.Token);
        AsyncCacheNonBlocking<TKey, TValue>.AsyncLazyWithRefreshTask<TValue> orAdd = this.values.GetOrAdd(key, lazyWithRefreshTask);
        if (lazyWithRefreshTask != orAdd)
          return await orAdd.GetValueAsync();
        try
        {
          return await orAdd.GetValueAsync();
        }
        catch (Exception ex)
        {
          DefaultTrace.TraceError("AsyncCacheNonBlocking Failed GetAsync with key: {0}, Exception: {1}", (object) key.ToString(), (object) ex.ToString());
          this.values.TryRemove(key, out AsyncCacheNonBlocking<TKey, TValue>.AsyncLazyWithRefreshTask<TValue> _);
          throw;
        }
      }
    }

    public void Set(TKey key1, TValue value)
    {
      AsyncCacheNonBlocking<TKey, TValue>.AsyncLazyWithRefreshTask<TValue> updateValue = new AsyncCacheNonBlocking<TKey, TValue>.AsyncLazyWithRefreshTask<TValue>(value, this.cancellationTokenSource.Token);
      this.values.AddOrUpdate(key1, updateValue, (Func<TKey, AsyncCacheNonBlocking<TKey, TValue>.AsyncLazyWithRefreshTask<TValue>, AsyncCacheNonBlocking<TKey, TValue>.AsyncLazyWithRefreshTask<TValue>>) ((key2, originalValue) => updateValue));
    }

    public bool TryRemove(TKey key) => this.values.TryRemove(key, out AsyncCacheNonBlocking<TKey, TValue>.AsyncLazyWithRefreshTask<TValue> _);

    private void Dispose(bool disposing)
    {
      if (this.isDisposed)
        return;
      if (!disposing)
        return;
      try
      {
        this.cancellationTokenSource.Cancel();
        this.cancellationTokenSource.Dispose();
      }
      catch (ObjectDisposedException ex)
      {
        DefaultTrace.TraceInformation(string.Format("AsyncCacheNonBlocking was already disposed: {0}", (object) 0), (object) ex);
      }
      this.isDisposed = true;
    }

    public void Dispose() => this.Dispose(true);

    private sealed class AsyncLazyWithRefreshTask<T>
    {
      private readonly CancellationToken cancellationToken;
      private readonly Func<T, Task<T>> createValueFunc;
      private readonly object valueLock = new object();
      private readonly object removedFromCacheLock = new object();
      private bool removedFromCache;
      private Task<T> value;
      private Task<T> refreshInProgress;

      public AsyncLazyWithRefreshTask(T value, CancellationToken cancellationToken)
      {
        this.cancellationToken = cancellationToken;
        this.createValueFunc = (Func<T, Task<T>>) null;
        this.value = Task.FromResult<T>(value);
        this.refreshInProgress = (Task<T>) null;
      }

      public AsyncLazyWithRefreshTask(
        Func<T, Task<T>> taskFactory,
        CancellationToken cancellationToken)
      {
        this.cancellationToken = cancellationToken;
        this.createValueFunc = taskFactory;
        this.value = (Task<T>) null;
        this.refreshInProgress = (Task<T>) null;
      }

      public bool IsValueCreated => this.value != null;

      public Task<T> GetValueAsync()
      {
        Task<T> valueAsync = this.value;
        if (valueAsync != null)
          return valueAsync;
        this.cancellationToken.ThrowIfCancellationRequested();
        lock (this.valueLock)
        {
          if (this.value != null)
            return this.value;
          this.cancellationToken.ThrowIfCancellationRequested();
          this.value = this.createValueFunc(default (T));
          return this.value;
        }
      }

      public async Task<T> CreateAndWaitForBackgroundRefreshTaskAsync(
        Func<T, Task<T>> createRefreshTask)
      {
        AsyncCacheNonBlocking<TKey, TValue>.AsyncLazyWithRefreshTask<T> lazyWithRefreshTask = this;
        lazyWithRefreshTask.cancellationToken.ThrowIfCancellationRequested();
        Task<T> t = lazyWithRefreshTask.value;
        if (AsyncCacheNonBlocking<TKey, TValue>.AsyncLazyWithRefreshTask<T>.IsTaskRunning((Task) t))
          return await t;
        T obj = default (T);
        if (t != null)
          obj = await t;
        Task<T> refreshInProgress = lazyWithRefreshTask.refreshInProgress;
        if (AsyncCacheNonBlocking<TKey, TValue>.AsyncLazyWithRefreshTask<T>.IsTaskRunning((Task) refreshInProgress))
          return await refreshInProgress;
        bool flag = false;
        lock (lazyWithRefreshTask.valueLock)
        {
          if (AsyncCacheNonBlocking<TKey, TValue>.AsyncLazyWithRefreshTask<T>.IsTaskRunning((Task) lazyWithRefreshTask.refreshInProgress))
          {
            refreshInProgress = lazyWithRefreshTask.refreshInProgress;
          }
          else
          {
            flag = true;
            lazyWithRefreshTask.refreshInProgress = createRefreshTask(obj);
            refreshInProgress = lazyWithRefreshTask.refreshInProgress;
          }
        }
        if (!flag)
          return await refreshInProgress;
        T result = await refreshInProgress;
        lock (lazyWithRefreshTask)
          lazyWithRefreshTask.value = Task.FromResult<T>(result);
        return result;
      }

      public bool ShouldRemoveFromCacheThreadSafe()
      {
        if (this.removedFromCache)
          return false;
        lock (this.removedFromCacheLock)
        {
          if (this.removedFromCache)
            return false;
          this.removedFromCache = true;
          return true;
        }
      }

      private static bool IsTaskRunning(Task t) => t != null && !t.IsCompleted;
    }
  }
}
