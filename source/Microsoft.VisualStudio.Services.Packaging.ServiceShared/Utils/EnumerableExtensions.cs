// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.EnumerableExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public static class EnumerableExtensions
  {
    public static List<T> Shuffle<T>(this IEnumerable<T> input, Random? random = null)
    {
      List<T> list = new List<T>(input);
      list.ShuffleInPlace<T>(random);
      return list;
    }

    public static void ShuffleInPlace<T>(this IList<T> list, Random? random = null)
    {
      Random random1 = random ?? new Random();
      for (int index1 = 0; index1 < list.Count; ++index1)
      {
        int index2 = random1.Next(0, list.Count);
        T obj = list[index1];
        list[index1] = list[index2];
        list[index2] = obj;
      }
    }

    public static IEnumerable<List<T>> GetPages<T>(
      this IEnumerable<T> source,
      int pageTargetWeight,
      Func<T, int> elementWeightFunc,
      int maxElementsPerPage = -1)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (elementWeightFunc == null)
        throw new ArgumentNullException(nameof (elementWeightFunc));
      if (pageTargetWeight <= 0)
        throw new ArgumentOutOfRangeException(nameof (pageTargetWeight), "pageTargetWeight must be > 0");
      List<T> source1 = new List<T>();
      int num = 0;
      foreach (T obj in source)
      {
        T element = obj;
        int elementSize = elementWeightFunc(element);
        if (((num + elementSize > pageTargetWeight ? 1 : 0) | (maxElementsPerPage <= 0 ? (false ? 1 : 0) : (source1.Count >= maxElementsPerPage ? 1 : 0))) != 0 && source1.Any<T>())
        {
          yield return source1;
          source1 = new List<T>();
          num = 0;
        }
        source1.Add(element);
        num += elementSize;
        element = default (T);
      }
      if (source1.Any<T>())
        yield return source1;
    }

    public static IEnumerable<(TFirst, TSecond)> Zip<TFirst, TSecond>(
      this IEnumerable<TFirst> first,
      IEnumerable<TSecond> second)
    {
      return first.Zip<TFirst, TSecond, (TFirst, TSecond)>(second, (Func<TFirst, TSecond, (TFirst, TSecond)>) ((a, b) => (a, b)));
    }

    public static IEnumerable<TResult> Zip<TFirst, TSecond, TThird, TResult>(
      this IEnumerable<TFirst> first,
      IEnumerable<TSecond> second,
      IEnumerable<TThird> third,
      Func<TFirst, TSecond, TThird, TResult> func)
    {
      using (IEnumerator<TFirst> e1 = first.GetEnumerator())
      {
        using (IEnumerator<TSecond> e2 = second.GetEnumerator())
        {
          using (IEnumerator<TThird> e3 = third.GetEnumerator())
          {
            while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext())
              yield return func(e1.Current, e2.Current, e3.Current);
          }
        }
      }
    }

    public static IAsyncEnumerable<T> AsAsyncEnumerable<T>(this IEnumerable<T> source) => (IAsyncEnumerable<T>) new EnumerableExtensions.\u003CAsAsyncEnumerable\u003Ed__5<T>(-2)
    {
      \u003C\u003E3__source = source
    };

    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
    {
      List<T> list = new List<T>();
      await foreach (T obj in source)
        list.Add(obj);
      List<T> listAsync = list;
      list = (List<T>) null;
      return listAsync;
    }

    public static TElement MaxBy<TElement, TValue>(
      this IEnumerable<TElement> source,
      Func<TElement, TValue> selector)
    {
      return source.Select<TElement, EnumerableExtensions.MinMaxIntermediate<TElement, TValue>>((Func<TElement, EnumerableExtensions.MinMaxIntermediate<TElement, TValue>>) (x => new EnumerableExtensions.MinMaxIntermediate<TElement, TValue>(x, selector(x)))).Max<EnumerableExtensions.MinMaxIntermediate<TElement, TValue>>().Element;
    }

    public static TElement? MaxByOrDefault<TElement, TValue>(
      this IEnumerable<TElement> source,
      Func<TElement, TValue> selector)
    {
      return source.Select<TElement, EnumerableExtensions.MinMaxIntermediate<TElement, TValue>>((Func<TElement, EnumerableExtensions.MinMaxIntermediate<TElement, TValue>>) (x => new EnumerableExtensions.MinMaxIntermediate<TElement, TValue>(x, selector(x)))).MaxOrDefault<EnumerableExtensions.MinMaxIntermediate<TElement, TValue>>().Element;
    }

    private readonly record struct MinMaxIntermediate<TElement, TValue>(
      TElement Element,
      TValue Value) : IComparable<EnumerableExtensions.MinMaxIntermediate<TElement, TValue>>
    {
      public int CompareTo(
        EnumerableExtensions.MinMaxIntermediate<TElement, TValue> other)
      {
        return Comparer<TValue>.Default.Compare(this.Value, other.Value);
      }
    }
  }
}
