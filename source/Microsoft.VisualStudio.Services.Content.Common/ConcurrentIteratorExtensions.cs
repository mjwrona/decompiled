// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ConcurrentIteratorExtensions
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class ConcurrentIteratorExtensions
  {
    public static ForkedConcurrentIterator<T> Fork<T>(
      this IConcurrentIterator<T> enumerator,
      int forkCount,
      int boundedCapacity,
      CancellationToken cancellationToken)
    {
      return new ForkedConcurrentIterator<T>(enumerator, forkCount, boundedCapacity, cancellationToken);
    }

    public static IConcurrentIterator<IReadOnlyCollection<T>> GetPages<T>(
      this IConcurrentIterator<T> enumerator,
      int pageSize,
      CancellationToken cancellationToken)
    {
      return (IConcurrentIterator<IReadOnlyCollection<T>>) new ConcurrentIterator<IReadOnlyCollection<T>>(new int?(2), cancellationToken, (Func<TryAddValueAsyncFunc<IReadOnlyCollection<T>>, CancellationToken, Task>) (async (valueAdderAsync, cancelToken) =>
      {
        using (enumerator)
        {
          bool flag1 = true;
          while (true)
          {
            bool flag2 = flag1;
            ConfiguredTaskAwaitable<bool> configuredTaskAwaitable;
            if (flag2)
            {
              configuredTaskAwaitable = enumerator.MoveNextAsync(cancelToken).ConfigureAwait(false);
              flag2 = await configuredTaskAwaitable;
            }
            if (flag2)
            {
              List<T> currentPage = new List<T>(pageSize)
              {
                enumerator.Current
              };
              while (true)
              {
                bool flag3 = currentPage.Count < pageSize;
                if (flag3)
                {
                  configuredTaskAwaitable = enumerator.MoveNextAsync(cancelToken).ConfigureAwait(false);
                  flag3 = await configuredTaskAwaitable;
                }
                if (flag3)
                  currentPage.Add(enumerator.Current);
                else
                  break;
              }
              configuredTaskAwaitable = valueAdderAsync((IReadOnlyCollection<T>) currentPage).ConfigureAwait(false);
              flag1 = await configuredTaskAwaitable;
              currentPage = (List<T>) null;
            }
            else
              break;
          }
        }
      }));
    }

    public static IConcurrentIterator<T2> Select<T1, T2>(
      this IConcurrentIterator<T1> enumerator,
      Func<T1, T2> selector)
    {
      return (IConcurrentIterator<T2>) new ConcurrentIteratorSelect<T1, T2>(enumerator, selector);
    }

    public static IConcurrentIterator<T2> SelectAsync<T1, T2>(
      this IConcurrentIterator<T1> enumerator,
      Func<T1, Task<T2>> selector)
    {
      return (IConcurrentIterator<T2>) new ConcurrentIteratorSelectAsync<T1, T2>(enumerator, selector);
    }

    public static IConcurrentIterator<T2> SelectMany<T1, T2>(
      this IConcurrentIterator<T1> enumerator,
      Func<T1, IConcurrentIterator<T2>> selector)
    {
      return (IConcurrentIterator<T2>) new ConcurrentIteratorSelectMany<T1, T2>(enumerator, selector);
    }

    public static IConcurrentIterator<T> SelectMany<T>(
      this IConcurrentIterator<IEnumerable<T>> enumerator)
    {
      return enumerator.SelectMany<IEnumerable<T>, T>((Func<IEnumerable<T>, IEnumerable<T>>) (x => x));
    }

    public static IConcurrentIterator<T> SelectMany<T>(
      this IConcurrentIterator<IConcurrentIterator<T>> enumerator)
    {
      return enumerator.SelectMany<IConcurrentIterator<T>, T>((Func<IConcurrentIterator<T>, IConcurrentIterator<T>>) (x => x));
    }

    public static IConcurrentIterator<T2> SelectManyAsync<T1, T2>(
      this IConcurrentIterator<T1> enumerator,
      Func<T1, CancellationToken, Task<IConcurrentIterator<T2>>> selector)
    {
      return (IConcurrentIterator<T2>) new ConcurrentIteratorSelectManyAsync<T1, T2>(enumerator, selector);
    }

    public static IConcurrentIterator<T2> SelectMany<T1, T2>(
      this IConcurrentIterator<T1> enumerator,
      Func<T1, IEnumerable<T2>> selector)
    {
      return (IConcurrentIterator<T2>) new ConcurrentIteratorSelectMany<T1, T2>(enumerator, (Func<T1, IConcurrentIterator<T2>>) (x => (IConcurrentIterator<T2>) new ConcurrentIteratorExtensions.UnbufferedConcurrentIterator<T2>(selector(x))));
    }

    public static IConcurrentIterator<TOut> SelectWithState<TState, TIn, TOut>(
      this IConcurrentIterator<TIn> enumerator,
      Func<TState, TIn, Tuple<TState, TOut>> update,
      TState initialState)
    {
      return enumerator.Select<TIn, TOut>((Func<TIn, TOut>) (x =>
      {
        Tuple<TState, TOut> tuple = update(initialState, x);
        initialState = tuple.Item1;
        return tuple.Item2;
      }));
    }

    public static IConcurrentIterator<T> Where<T>(
      this IConcurrentIterator<T> enumerator,
      Func<T, bool> selector)
    {
      return (IConcurrentIterator<T>) new ConcurrentIteratorWhere<T>(enumerator, selector);
    }

    public static IConcurrentIterator<T> Skip<T>(this IConcurrentIterator<T> enumerator, long n)
    {
      int i = 0;
      return enumerator.Where<T>((Func<T, bool>) (x => (long) ++i > n));
    }

    public static IConcurrentIterator<T> Take<T>(this IConcurrentIterator<T> enumerator, long n) => (IConcurrentIterator<T>) new ConcurrentIteratorTake<T>(enumerator, n);

    public static async Task<T> SingleOrDefaultAsync<T>(
      this IConcurrentIterator<T> enumerator,
      CancellationToken token)
    {
      using (enumerator)
      {
        if (!await enumerator.MoveNextAsync(token).ConfigureAwait(false))
          return default (T);
        T singleItem = enumerator.Current;
        if (await enumerator.MoveNextAsync(token).ConfigureAwait(false))
          throw new ArgumentException("Enumerator had more than one item.");
        return singleItem;
      }
    }

    public static IConcurrentIterator<T> Concat<T>(
      this IConcurrentIterator<T> firstConcurrentIterator,
      IConcurrentIterator<T> secondConcurrentIterator,
      CancellationToken cancellationToken)
    {
      return Enumerable.Repeat<IConcurrentIterator<T>>(firstConcurrentIterator, 1).Concat<IConcurrentIterator<T>>(Enumerable.Repeat<IConcurrentIterator<T>>(secondConcurrentIterator, 1)).CollectOrdered<T>(cancellationToken);
    }

    public static IConcurrentIterator<T> Concat<T>(
      this IConcurrentIterator<T> firstConcurrentIterator,
      IEnumerable<IConcurrentIterator<T>> otherEnumerators,
      CancellationToken cancellationToken)
    {
      return Enumerable.Repeat<IConcurrentIterator<T>>(firstConcurrentIterator, 1).Concat<IConcurrentIterator<T>>(otherEnumerators).CollectOrdered<T>(cancellationToken);
    }

    public static IConcurrentIterator<T> CollectOrdered<T>(
      this IEnumerable<IConcurrentIterator<T>> enumerators,
      CancellationToken cancellationToken)
    {
      return (IConcurrentIterator<T>) new ConcurrentIterator<T>(new int?(2), cancellationToken, (Func<TryAddValueAsyncFunc<T>, CancellationToken, Task>) (async (valueAdderAsync, cancelToken) =>
      {
        bool keepGoing = true;
        foreach (IConcurrentIterator<T> enumerator in enumerators)
        {
          await enumerator.DoWhileAsyncNoContext<T>(cancelToken, (Func<T, Task<bool>>) (async v =>
          {
            keepGoing = await valueAdderAsync(v).ConfigureAwait(false);
            return keepGoing;
          }));
          if (!keepGoing)
            break;
        }
      }));
    }

    public static IConcurrentIterator<T> CollectSortOrdered<T>(
      this IEnumerable<IConcurrentIterator<T>> sourceEnumerators,
      int? boundedCapacity,
      IComparer<T> itemComparer,
      CancellationToken cancellationToken)
    {
      return sourceEnumerators.Count<IConcurrentIterator<T>>() == 1 ? sourceEnumerators.First<IConcurrentIterator<T>>() : (IConcurrentIterator<T>) new ConcurrentIterator<T>(boundedCapacity, cancellationToken, (Func<TryAddValueAsyncFunc<T>, CancellationToken, Task>) (async (valueAdderAsync, cancelToken) =>
      {
        T previousValue = default (T);
        bool isFirstItem = true;
        try
        {
          SortedDictionary<T, Stack<IConcurrentIterator<T>>> sortedEnumerators = new SortedDictionary<T, Stack<IConcurrentIterator<T>>>(itemComparer);
          IConcurrentIterator<T> enumerator;
          foreach (IConcurrentIterator<T> sourceEnumerator in sourceEnumerators)
          {
            enumerator = sourceEnumerator;
            if (await enumerator.MoveNextAsync(cancelToken))
            {
              Stack<IConcurrentIterator<T>> concurrentIteratorStack;
              if (!sortedEnumerators.TryGetValue(enumerator.Current, out concurrentIteratorStack))
              {
                concurrentIteratorStack = new Stack<IConcurrentIterator<T>>(1);
                sortedEnumerators.Add(enumerator.Current, concurrentIteratorStack);
              }
              concurrentIteratorStack.Push(enumerator);
            }
            enumerator = (IConcurrentIterator<T>) null;
          }
          while (sortedEnumerators.Any<KeyValuePair<T, Stack<IConcurrentIterator<T>>>>())
          {
            KeyValuePair<T, Stack<IConcurrentIterator<T>>> keyValuePair = sortedEnumerators.First<KeyValuePair<T, Stack<IConcurrentIterator<T>>>>();
            Stack<IConcurrentIterator<T>> minEnumerators = keyValuePair.Value;
            enumerator = minEnumerators.Pop();
            if (!minEnumerators.Any<IConcurrentIterator<T>>())
              sortedEnumerators.Remove(keyValuePair.Key);
            if (!isFirstItem && itemComparer.Compare(previousValue, enumerator.Current) > 0)
              throw new InvalidOperationException(string.Format("Enumeration is not in sorted order, prev: {0}, curr: {1}", (object) previousValue, (object) enumerator.Current));
            isFirstItem = false;
            previousValue = enumerator.Current;
            if (!await valueAdderAsync(enumerator.Current).ConfigureAwait(false))
            {
              previousValue = default (T);
              return;
            }
            if (await enumerator.MoveNextAsync(cancelToken))
            {
              Stack<IConcurrentIterator<T>> concurrentIteratorStack;
              if (!sortedEnumerators.TryGetValue(enumerator.Current, out concurrentIteratorStack))
              {
                concurrentIteratorStack = !minEnumerators.Any<IConcurrentIterator<T>>() ? minEnumerators : new Stack<IConcurrentIterator<T>>(1);
                sortedEnumerators.Add(enumerator.Current, concurrentIteratorStack);
              }
              concurrentIteratorStack.Push(enumerator);
            }
            minEnumerators = (Stack<IConcurrentIterator<T>>) null;
            enumerator = (IConcurrentIterator<T>) null;
          }
          sortedEnumerators = (SortedDictionary<T, Stack<IConcurrentIterator<T>>>) null;
          previousValue = default (T);
        }
        finally
        {
          List<Exception> innerExceptions = new List<Exception>();
          foreach (IConcurrentIterator<T> sourceEnumerator in sourceEnumerators)
          {
            try
            {
              sourceEnumerator.Dispose();
            }
            catch (Exception ex)
            {
              innerExceptions.Add(ex);
            }
          }
          if (innerExceptions.Count == 1)
            throw innerExceptions[0];
          if (innerExceptions.Count > 1)
            throw new AggregateException("Multiple exceptions while disposing enumerators", (IEnumerable<Exception>) innerExceptions);
        }
      }));
    }

    public static IConcurrentIterator<TValue> CollectUnordered<TValue>(
      this IEnumerable<IConcurrentIterator<TValue>> enumerators,
      int? boundedCapacity,
      CancellationToken cancellationToken)
    {
      return (IConcurrentIterator<TValue>) new ConcurrentIterator<IConcurrentIterator<TValue>, TValue>(enumerators, boundedCapacity, cancellationToken, (Func<IConcurrentIterator<TValue>, TryAddValueAsyncFunc<TValue>, CancellationToken, Task>) ((enumerator, valueAdderAsync, cancelToken) => enumerator.DoWhileAsyncNoContext<TValue>(cancelToken, (Func<TValue, Task<bool>>) (t => valueAdderAsync(t)))));
    }

    public static IConcurrentIterator<TValue> CollectUnordered<TValue>(
      int? boundedCapacity,
      CancellationToken cancellationToken,
      params IConcurrentIterator<TValue>[] enumerators)
    {
      return ((IEnumerable<IConcurrentIterator<TValue>>) enumerators).CollectUnordered<TValue>(boundedCapacity, cancellationToken);
    }

    public static async Task<T> SingleAsync<T>(
      this IConcurrentIterator<T> enumerator,
      CancellationToken cancellationToken)
    {
      T obj;
      using (enumerator)
      {
        T singleItem = await enumerator.MoveNextAsync(cancellationToken).ConfigureAwait(false) ? enumerator.Current : throw new ArgumentException("Enumerator had no items.");
        if (await enumerator.MoveNextAsync(cancellationToken).ConfigureAwait(false))
          throw new ArgumentException("Enumerator had more than one item.");
        obj = singleItem;
      }
      return obj;
    }

    public static Task ForEachAsyncNoContext<T>(
      this IConcurrentIterator<T> enumerator,
      CancellationToken cancellationToken,
      Action<T> loopBody)
    {
      return enumerator.ForEachAsync<T>(false, cancellationToken, loopBody);
    }

    public static Task ForEachAsyncNoContext<T>(
      this IConcurrentIterator<T> enumerator,
      CancellationToken cancellationToken,
      Func<T, Task> loopBodyAsync)
    {
      return enumerator.ForEachAsync<T>(false, cancellationToken, loopBodyAsync);
    }

    public static Task ForEachAsyncCaptureContext<T>(
      this IConcurrentIterator<T> enumerator,
      CancellationToken cancellationToken,
      Action<T> loopBody)
    {
      return enumerator.ForEachAsync<T>(true, cancellationToken, loopBody);
    }

    public static Task ForEachAsyncCaptureContext<T>(
      this IConcurrentIterator<T> enumerator,
      CancellationToken cancellationToken,
      Func<T, Task> loopBodyAsync)
    {
      return enumerator.ForEachAsync<T>(true, cancellationToken, loopBodyAsync);
    }

    public static async Task ForEachAsync<T>(
      this IConcurrentIterator<T> enumerator,
      bool continueOnCapturedContext,
      CancellationToken cancellationToken,
      Action<T> loopBody)
    {
      enumerator.AssertNotEnumerated<T>();
      using (enumerator)
      {
        while (true)
        {
          if (await enumerator.MoveNextAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext))
            loopBody(enumerator.Current);
          else
            break;
        }
      }
    }

    public static async Task ForEachAsync<T>(
      this IConcurrentIterator<T> enumerator,
      bool continueOnCapturedContext,
      CancellationToken cancellationToken,
      Func<T, Task> loopBodyAsync)
    {
      enumerator.AssertNotEnumerated<T>();
      using (enumerator)
      {
        while (true)
        {
          if (await enumerator.MoveNextAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext))
            await loopBodyAsync(enumerator.Current).ConfigureAwait(continueOnCapturedContext);
          else
            break;
        }
      }
    }

    public static async Task<List<T>> TakeAsList<T>(
      this IConcurrentIterator<T> enumerator,
      long n,
      CancellationToken cancellationToken)
    {
      List<T> list = new List<T>();
      if (n > 0L)
      {
        long count = n;
        await enumerator.DoWhileAsyncNoContext<T>(cancellationToken, (Func<T, bool>) (item =>
        {
          list.Add(item);
          return --count > 0L;
        })).ConfigureAwait(false);
      }
      return list;
    }

    public static Task DoWhileAsyncNoContext<T>(
      this IConcurrentIterator<T> enumerator,
      CancellationToken cancellationToken,
      Func<T, bool> loopBody)
    {
      return enumerator.DoWhileAsync<T>(false, cancellationToken, loopBody);
    }

    public static Task DoWhileAsyncCaptureContext<T>(
      this IConcurrentIterator<T> enumerator,
      CancellationToken cancellationToken,
      Func<T, bool> loopBody)
    {
      return enumerator.DoWhileAsync<T>(true, cancellationToken, loopBody);
    }

    public static Task DoWhileAsyncNoContext<T>(
      this IConcurrentIterator<T> enumerator,
      CancellationToken cancellationToken,
      Func<T, Task<bool>> loopBodyAsync)
    {
      return enumerator.DoWhileAsync<T>(false, cancellationToken, loopBodyAsync);
    }

    public static Task DoWhileAsyncCaptureContext<T>(
      this IConcurrentIterator<T> enumerator,
      CancellationToken cancellationToken,
      Func<T, Task<bool>> loopBodyAsync)
    {
      return enumerator.DoWhileAsync<T>(true, cancellationToken, loopBodyAsync);
    }

    public static async Task DoWhileAsync<T>(
      this IConcurrentIterator<T> enumerator,
      bool continueOnCapturedContext,
      CancellationToken cancellationToken,
      Func<T, bool> loopBody)
    {
      enumerator.AssertNotEnumerated<T>();
      using (enumerator)
      {
        do
        {
          if (!await enumerator.MoveNextAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext))
            break;
        }
        while (loopBody(enumerator.Current));
      }
    }

    public static async Task DoWhileAsync<T>(
      this IConcurrentIterator<T> enumerator,
      bool continueOnCapturedContext,
      CancellationToken cancellationToken,
      Func<T, Task<bool>> loopBodyAsync)
    {
      enumerator.AssertNotEnumerated<T>();
      using (enumerator)
      {
        do
        {
          if (!await enumerator.MoveNextAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext))
            break;
        }
        while (await loopBodyAsync(enumerator.Current).ConfigureAwait(continueOnCapturedContext));
      }
    }

    public static Task<bool> AllAsyncCaptureContext<T>(
      this IConcurrentIterator<T> enumerator,
      CancellationToken cancellationToken,
      Func<T, bool> predicateFunc)
    {
      return enumerator.AllAsync<T>(true, cancellationToken, predicateFunc);
    }

    public static Task<bool> AllAsyncNoContext<T>(
      this IConcurrentIterator<T> enumerator,
      CancellationToken cancellationToken,
      Func<T, bool> predicateFunc)
    {
      return enumerator.AllAsync<T>(false, cancellationToken, predicateFunc);
    }

    public static async Task<bool> AllAsync<T>(
      this IConcurrentIterator<T> enumerator,
      bool continueOnCapturedContext,
      CancellationToken cancellationToken,
      Func<T, bool> predicateFunc)
    {
      enumerator.AssertNotEnumerated<T>();
      IConcurrentIterator<T> concurrentIterator = enumerator;
      try
      {
        do
        {
          if (!await enumerator.MoveNextAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext))
            goto label_9;
        }
        while (predicateFunc(enumerator.Current));
        return false;
      }
      finally
      {
        concurrentIterator?.Dispose();
      }
label_9:
      concurrentIterator = (IConcurrentIterator<T>) null;
      return true;
    }

    public static async Task<List<T>> ToListAsync<T>(
      this IConcurrentIterator<T> enumerator,
      CancellationToken cancellationToken)
    {
      List<T> list = new List<T>();
      await enumerator.ForEachAsyncNoContext<T>(cancellationToken, (Action<T>) (item => list.Add(item))).ConfigureAwait(false);
      return list;
    }

    public static async Task<IDictionary<K, V>> ToDictionaryAsync<K, V>(
      this IConcurrentIterator<KeyValuePair<K, V>> enumerator,
      CancellationToken cancellationToken)
    {
      Dictionary<K, V> list = new Dictionary<K, V>();
      await enumerator.ForEachAsyncNoContext<KeyValuePair<K, V>>(cancellationToken, (Action<KeyValuePair<K, V>>) (item => list.Add(item.Key, item.Value))).ConfigureAwait(false);
      return (IDictionary<K, V>) list;
    }

    public static async Task<IConcurrentIterator<T>> WrapAndProbeAsync<T>(
      this IConcurrentIterator<T> enumeratorToProbe,
      CancellationToken cancellationToken)
    {
      if (!await enumeratorToProbe.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        return (IConcurrentIterator<T>) new ConcurrentIterator<T>(Enumerable.Empty<T>());
      return ConcurrentIterator.CollectOrdered<T>(cancellationToken, (IConcurrentIterator<T>) new ConcurrentIterator<T>((IEnumerable<T>) new T[1]
      {
        enumeratorToProbe.Current
      }), (IConcurrentIterator<T>) new ResetStartConcurrentIterator<T>(enumeratorToProbe));
    }

    public static void AssertNotEnumerated<T>(this IConcurrentIterator<T> enumerator)
    {
      if (enumerator.EnumerationStarted)
        throw new ConcurrentIteratorAlreadyStartedException();
    }

    private class UnbufferedConcurrentIterator<T> : IConcurrentIterator<T>, IDisposable
    {
      private readonly IEnumerator<T> baseEnumerator;

      public UnbufferedConcurrentIterator(IEnumerable<T> enumerable) => this.baseEnumerator = enumerable.GetEnumerator();

      public T Current => this.baseEnumerator.Current;

      public bool EnumerationStarted { get; private set; }

      public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
      {
        this.EnumerationStarted = true;
        return Task.FromResult<bool>(this.baseEnumerator.MoveNext());
      }

      public void Dispose(bool disposing)
      {
        if (!disposing)
          return;
        this.baseEnumerator.Dispose();
      }

      public void Dispose() => this.Dispose(true);
    }

    private class ConcurrentIteratorComparer<T> : IComparer<IConcurrentIterator<T>>
    {
      private readonly IComparer<T> itemComparer;

      public ConcurrentIteratorComparer(IComparer<T> itemComparer) => this.itemComparer = itemComparer;

      public int Compare(IConcurrentIterator<T> x, IConcurrentIterator<T> y)
      {
        if (!x.EnumerationStarted || !y.EnumerationStarted)
          throw new ArgumentException("Enumerator has not started enumeration for comparing");
        return this.itemComparer.Compare(x.Current, y.Current);
      }
    }
  }
}
