// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.ConcurrencyConsolidator`2
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Common
{
  public class ConcurrencyConsolidator<TKey, TResult> : IConcurrencyConsolidator<TKey, TResult>
  {
    private readonly ConcurrentDictionary<TKey, Task<TResult>>[] taskDictionaries;
    private readonly bool consolidateExceptions;

    public ConcurrencyConsolidator(bool consolidateExceptions, int boundedConcurrency)
      : this(consolidateExceptions, boundedConcurrency, (IEqualityComparer<TKey>) EqualityComparer<TKey>.Default)
    {
    }

    public ConcurrencyConsolidator(
      bool consolidateExceptions,
      int boundedConcurrency,
      IEqualityComparer<TKey> comparer)
    {
      this.consolidateExceptions = consolidateExceptions;
      this.taskDictionaries = Enumerable.Range(0, boundedConcurrency).Select<int, ConcurrentDictionary<TKey, Task<TResult>>>((Func<int, ConcurrentDictionary<TKey, Task<TResult>>>) (_ => new ConcurrentDictionary<TKey, Task<TResult>>(comparer))).ToArray<ConcurrentDictionary<TKey, Task<TResult>>>();
    }

    public async Task<TResult> RunOnceAsync(TKey key, Func<Task<TResult>> taskFunc)
    {
      SynchronizationContext expectedContext = SynchronizationContext.Current;
      bool flag;
      TResult result1;
      do
      {
        (flag, result1) = await this.TryRunInternalAsync(expectedContext, key, taskFunc).ConfigureAwait(true);
      }
      while (!flag);
      TResult result2 = result1;
      expectedContext = (SynchronizationContext) null;
      return result2;
    }

    private async Task<(bool success, TResult value)> TryRunInternalAsync(
      SynchronizationContext expectedContext,
      TKey key,
      Func<Task<TResult>> taskFunc)
    {
      SafeTaskCompletionSource<TResult> tcs = new SafeTaskCompletionSource<TResult>();
      List<Task<TResult>> taskList = new List<Task<TResult>>();
      foreach (ConcurrentDictionary<TKey, Task<TResult>> taskDictionary in this.taskDictionaries)
      {
        Task<TResult> finalValue;
        if (taskDictionary.GetOrAdd<TKey, Task<TResult>>(key, tcs.Task, out finalValue))
          return (true, await ConcurrencyConsolidator<TKey, TResult>.ExecuteAsync(expectedContext, taskDictionary, key, taskFunc, tcs).ConfigureAwait(false));
        taskList.Add(finalValue);
      }
      tcs.MarkTaskAsUnused();
      Task<TResult> task = await Task.WhenAny<TResult>((IEnumerable<Task<TResult>>) taskList).ConfigureAwait(false);
      try
      {
        return (true, await task.ConfigureAwait(false));
      }
      catch (Exception ex) when (!this.consolidateExceptions)
      {
        return (false, default (TResult));
      }
    }

    private static async Task<TResult> ExecuteAsync(
      SynchronizationContext expectedContext,
      ConcurrentDictionary<TKey, Task<TResult>> taskDictionary,
      TKey key,
      Func<Task<TResult>> taskFunc,
      SafeTaskCompletionSource<TResult> tcs)
    {
      bool removed = false;
      TResult result;
      try
      {
        if (expectedContext != SynchronizationContext.Current)
          throw new InvalidOperationException("Not in the correct synchronization context.");
        TResult value = await taskFunc().ConfigureAwait(false);
        Task.Run((Action) (() => tcs.SetResult(value)));
        ConcurrencyConsolidator<TKey, TResult>.Remove(taskDictionary, key, tcs.Task);
        removed = true;
        result = value;
      }
      catch (Exception ex)
      {
        ConcurrencyConsolidator<TKey, TResult>.Remove(taskDictionary, key, tcs.Task);
        removed = true;
        tcs.SetException(ex);
        throw;
      }
      finally
      {
        if (!removed)
          ConcurrencyConsolidator<TKey, TResult>.Remove(taskDictionary, key, tcs.Task);
      }
      return result;
    }

    private static void Remove(
      ConcurrentDictionary<TKey, Task<TResult>> taskDictionary,
      TKey key,
      Task<TResult> task)
    {
      ((ICollection<KeyValuePair<TKey, Task<TResult>>>) taskDictionary).Remove(new KeyValuePair<TKey, Task<TResult>>(key, task));
    }
  }
}
