// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.RunOnce`2
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public class RunOnce<TKey, TResult>
  {
    private readonly bool consolidateExceptions;

    protected ConcurrentDictionary<TKey, Task<TResult>> TaskDictionary { get; }

    public IEnumerable<TResult> CompletedValues => (IEnumerable<TResult>) this.TaskDictionary.Values.Where<Task<TResult>>((Func<Task<TResult>, bool>) (v => v.IsCompleted)).Select<Task<TResult>, TResult>((Func<Task<TResult>, TResult>) (v => v.Result)).ToArray<TResult>();

    public RunOnce(bool consolidateExceptions)
      : this(consolidateExceptions, (IEqualityComparer<TKey>) EqualityComparer<TKey>.Default)
    {
    }

    public RunOnce(bool consolidateExceptions, IEqualityComparer<TKey> comparer)
    {
      this.consolidateExceptions = consolidateExceptions;
      this.TaskDictionary = new ConcurrentDictionary<TKey, Task<TResult>>(comparer);
    }

    public bool TryAdd(TKey key, TResult value) => this.TaskDictionary.TryAdd(key, Task.FromResult<TResult>(value));

    public async Task<TResult> RunOnceAsync(TKey key, Func<Task<TResult>> taskFunc)
    {
      bool flag;
      TResult result;
      do
      {
        (flag, result) = await this.TryRunOnceInternalAsync(key, taskFunc).ConfigureAwait(true);
      }
      while (!flag);
      return result;
    }

    public async Task<(bool success, TResult value)> TryRunOnceInternalAsync(
      TKey key,
      Func<Task<TResult>> taskFunc)
    {
      SafeTaskCompletionSource<TResult> tcs = new SafeTaskCompletionSource<TResult>(TaskCreationOptions.RunContinuationsAsynchronously);
      Task<TResult> finalValue;
      if (this.TaskDictionary.GetOrAdd<TKey, Task<TResult>>(key, tcs.Task, out finalValue))
        return (true, await this.ExecuteAsync(key, taskFunc, tcs));
      tcs.MarkTaskAsUnused();
      try
      {
        return (true, await finalValue.ConfigureAwait(false));
      }
      catch (Exception ex) when (!this.consolidateExceptions)
      {
        return (false, default (TResult));
      }
    }

    private async Task<TResult> ExecuteAsync(
      TKey key,
      Func<Task<TResult>> taskFunc,
      SafeTaskCompletionSource<TResult> tcs)
    {
      TResult result1;
      try
      {
        TResult result2 = await taskFunc().ConfigureAwait(false);
        tcs.SetResult(result2);
        result1 = result2;
      }
      catch (Exception ex)
      {
        if (!this.consolidateExceptions)
          RunOnce<TKey, TResult>.Remove(this.TaskDictionary, key, tcs.Task);
        tcs.SetException(ex);
        ex.ReThrow();
        throw new InvalidOperationException("unreachable.");
      }
      return result1;
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
