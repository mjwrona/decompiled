// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.EnumerableExtensions
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class EnumerableExtensions
  {
    public static IEnumerable<T> AsEmptyIfNull<T>(this IEnumerable<T> source) => source ?? Enumerable.Empty<T>();

    public static TEnumerable AsEmptyIfNull<TEnumerable>(this TEnumerable source) where TEnumerable : class, IEnumerable, new() => source ?? new TEnumerable();

    public static IEnumerable<IList<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
    {
      ArgumentUtility.CheckForNull<IEnumerable<T>>(source, nameof (source));
      ArgumentUtility.CheckBoundsInclusive(batchSize, 1, int.MaxValue, nameof (batchSize));
      List<T> objList = new List<T>(batchSize);
      foreach (T obj in source)
      {
        objList.Add(obj);
        if (objList.Count == batchSize)
        {
          yield return (IList<T>) objList;
          objList = new List<T>(batchSize);
        }
      }
      if (objList.Count > 0)
        yield return (IList<T>) objList;
    }

    public static PartitionResults<T> Partition<T>(
      this IEnumerable<T> source,
      Predicate<T> predicate)
    {
      ArgumentUtility.CheckForNull<IEnumerable<T>>(source, nameof (source));
      ArgumentUtility.CheckForNull<Predicate<T>>(predicate, nameof (predicate));
      PartitionResults<T> partitionResults = new PartitionResults<T>();
      foreach (T obj in source)
      {
        if (predicate(obj))
          partitionResults.MatchingPartition.Add(obj);
        else
          partitionResults.NonMatchingPartition.Add(obj);
      }
      return partitionResults;
    }

    public static MultiPartitionResults<T> Partition<T>(
      this IEnumerable<T> source,
      params Predicate<T>[] predicates)
    {
      ArgumentUtility.CheckForNull<IEnumerable<T>>(source, nameof (source));
      ArgumentUtility.CheckForNull<Predicate<T>[]>(predicates, nameof (predicates));
      List<int> list = Enumerable.Range(0, predicates.Length).ToList<int>();
      MultiPartitionResults<T> partitionResults = new MultiPartitionResults<T>();
      partitionResults.MatchingPartitions.AddRange(list.Select<int, List<T>>((Func<int, List<T>>) (_ => new List<T>())));
      foreach (T obj in source)
      {
        T item = obj;
        bool flag = false;
        using (IEnumerator<int> enumerator = list.Where<int>((Func<int, bool>) (predicateIndex => predicates[predicateIndex](item))).GetEnumerator())
        {
          if (enumerator.MoveNext())
          {
            int current = enumerator.Current;
            partitionResults.MatchingPartitions[current].Add(item);
            flag = true;
          }
        }
        if (!flag)
          partitionResults.NonMatchingPartition.Add(item);
      }
      return partitionResults;
    }

    public static IEnumerable<T> Merge<T>(
      this IEnumerable<T> first,
      IEnumerable<T> second,
      IComparer<T> comparer)
    {
      return first.Merge<T>(second, comparer == null ? (Func<T, T, int>) null : new Func<T, T, int>(comparer.Compare));
    }

    public static IEnumerable<T> Merge<T>(
      this IEnumerable<T> first,
      IEnumerable<T> second,
      Func<T, T, int> comparer)
    {
      ArgumentUtility.CheckForNull<IEnumerable<T>>(first, nameof (first));
      ArgumentUtility.CheckForNull<IEnumerable<T>>(second, nameof (second));
      ArgumentUtility.CheckForNull<Func<T, T, int>>(comparer, nameof (comparer));
      using (IEnumerator<T> e1 = first.GetEnumerator())
      {
        using (IEnumerator<T> e2 = second.GetEnumerator())
        {
          bool e1Valid = e1.MoveNext();
          bool e2Valid = e2.MoveNext();
          while (e1Valid & e2Valid)
          {
            if (comparer(e1.Current, e2.Current) <= 0)
            {
              yield return e1.Current;
              e1Valid = e1.MoveNext();
            }
            else
            {
              yield return e2.Current;
              e2Valid = e2.MoveNext();
            }
          }
          for (; e1Valid; e1Valid = e1.MoveNext())
            yield return e1.Current;
          for (; e2Valid; e2Valid = e2.MoveNext())
            yield return e2.Current;
        }
      }
    }

    public static IEnumerable<T> MergeDistinct<T>(
      this IEnumerable<T> first,
      IEnumerable<T> second,
      IComparer<T> comparer)
    {
      return first.MergeDistinct<T>(second, comparer == null ? (Func<T, T, int>) null : new Func<T, T, int>(comparer.Compare));
    }

    public static IEnumerable<T> MergeDistinct<T>(
      this IEnumerable<T> first,
      IEnumerable<T> second,
      Func<T, T, int> comparer)
    {
      ArgumentUtility.CheckForNull<IEnumerable<T>>(first, nameof (first));
      ArgumentUtility.CheckForNull<IEnumerable<T>>(second, nameof (second));
      ArgumentUtility.CheckForNull<Func<T, T, int>>(comparer, nameof (comparer));
      using (IEnumerator<T> e1 = first.GetEnumerator())
      {
        using (IEnumerator<T> e2 = second.GetEnumerator())
        {
          bool e1Valid = e1.MoveNext();
          bool e2Valid = e2.MoveNext();
          while (e1Valid & e2Valid)
          {
            if (comparer(e1.Current, e2.Current) < 0)
            {
              yield return e1.Current;
              e1Valid = e1.MoveNext();
            }
            else if (comparer(e1.Current, e2.Current) > 0)
            {
              yield return e2.Current;
              e2Valid = e2.MoveNext();
            }
            else
            {
              yield return e1.Current;
              e1Valid = e1.MoveNext();
              e2Valid = e2.MoveNext();
            }
          }
          for (; e1Valid; e1Valid = e1.MoveNext())
            yield return e1.Current;
          for (; e2Valid; e2Valid = e2.MoveNext())
            yield return e2.Current;
        }
      }
    }

    public static HashSet<T> ToHashSet<T>(IEnumerable<T> source) => new HashSet<T>(source);

    public static HashSet<T> ToHashSet<T>(IEnumerable<T> source, IEqualityComparer<T> comparer) => new HashSet<T>(source, comparer);

    public static HashSet<TOut> ToHashSet<TIn, TOut>(
      this IEnumerable<TIn> source,
      Func<TIn, TOut> selector)
    {
      return new HashSet<TOut>(source.Select<TIn, TOut>(selector));
    }

    public static HashSet<TOut> ToHashSet<TIn, TOut>(
      this IEnumerable<TIn> source,
      Func<TIn, TOut> selector,
      IEqualityComparer<TOut> comparer)
    {
      return new HashSet<TOut>(source.Select<TIn, TOut>(selector), comparer);
    }

    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
      ArgumentUtility.CheckForNull<Action<T>>(action, nameof (action));
      ArgumentUtility.CheckForNull<IEnumerable<T>>(collection, nameof (collection));
      foreach (T obj in collection)
        action(obj);
    }

    public static void AddIf<T>(this List<T> list, bool condition, T element)
    {
      if (!condition)
        return;
      list.Add(element);
    }

    public static NameValueCollection ToNameValueCollection(
      this IEnumerable<KeyValuePair<string, string>> pairs)
    {
      NameValueCollection nameValueCollection = new NameValueCollection();
      foreach (KeyValuePair<string, string> pair in pairs)
        nameValueCollection.Add(pair.Key, pair.Value);
      return nameValueCollection;
    }

    public static IList<P> PartitionSolveAndMergeBack<T, P>(
      this IList<T> source,
      Predicate<T> predicate,
      Func<IList<T>, IList<P>> matchingPartitionSolver,
      Func<IList<T>, IList<P>> nonMatchingPartitionSolver)
    {
      ArgumentUtility.CheckForNull<IList<T>>(source, nameof (source));
      ArgumentUtility.CheckForNull<Predicate<T>>(predicate, nameof (predicate));
      ArgumentUtility.CheckForNull<Func<IList<T>, IList<P>>>(matchingPartitionSolver, nameof (matchingPartitionSolver));
      ArgumentUtility.CheckForNull<Func<IList<T>, IList<P>>>(nonMatchingPartitionSolver, nameof (nonMatchingPartitionSolver));
      PartitionResults<Tuple<int, T>> partitionResults = new PartitionResults<Tuple<int, T>>();
      for (int index = 0; index < source.Count; ++index)
      {
        T obj = source[index];
        if (predicate(obj))
          partitionResults.MatchingPartition.Add(new Tuple<int, T>(index, obj));
        else
          partitionResults.NonMatchingPartition.Add(new Tuple<int, T>(index, obj));
      }
      List<P> pList = new List<P>(source.Count);
      if (partitionResults.MatchingPartition.Any<Tuple<int, T>>())
        pList.AddRange((IEnumerable<P>) matchingPartitionSolver((IList<T>) partitionResults.MatchingPartition.Select<Tuple<int, T>, T>((Func<Tuple<int, T>, T>) (x => x.Item2)).ToList<T>()));
      if (partitionResults.NonMatchingPartition.Any<Tuple<int, T>>())
        pList.AddRange((IEnumerable<P>) nonMatchingPartitionSolver((IList<T>) partitionResults.NonMatchingPartition.Select<Tuple<int, T>, T>((Func<Tuple<int, T>, T>) (x => x.Item2)).ToList<T>()));
      List<P> list = Enumerable.Repeat<P>(default (P), source.Count).ToList<P>();
      if (pList.Count != source.Count)
        return (IList<P>) pList;
      for (int index = 0; index < source.Count; ++index)
      {
        if (index < partitionResults.MatchingPartition.Count)
          list[partitionResults.MatchingPartition[index].Item1] = pList[index];
        else
          list[partitionResults.NonMatchingPartition[index - partitionResults.MatchingPartition.Count].Item1] = pList[index];
      }
      return (IList<P>) list;
    }
  }
}
