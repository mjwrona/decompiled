// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ConcurrentIteratorWithCursorExtensions
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class ConcurrentIteratorWithCursorExtensions
  {
    public static IConcurrentIteratorWithCursor<TValue, TCursor> WithCursor<TValue, TCursor>(
      this IConcurrentIterator<TValue> enumerator,
      Func<TValue, TCursor> mkCursor,
      TCursor initial = null)
    {
      return (IConcurrentIteratorWithCursor<TValue, TCursor>) new ConcurrentIteratorWithCursor<TValue, TValue, TCursor>(enumerator, mkCursor, (Func<TValue, TValue>) (x => x), initial);
    }

    public static IConcurrentIteratorWithCursor<TValue, TValue> WithCursor<TValue>(
      this IConcurrentIterator<TValue> enumerator,
      TValue initial = null)
    {
      return enumerator.WithCursor<TValue, TValue>((Func<TValue, TValue>) (x => x), initial);
    }

    public static IConcurrentIteratorWithCursor<T2, TCursor> WithCursor<T1, T2, TCursor>(
      this IConcurrentIterator<T1> enumerator,
      Func<T1, TCursor> mkCursor,
      Func<T1, T2> select,
      TCursor initial = null)
    {
      return (IConcurrentIteratorWithCursor<T2, TCursor>) new ConcurrentIteratorWithCursor<T1, T2, TCursor>(enumerator, mkCursor, select, initial);
    }

    public static IConcurrentIteratorWithCursor<TValue, CompositeCursor<TId, TCursor>> CollectUnordered<TValue, TCursor, TId>(
      this IEnumerable<KeyValuePair<TId, IConcurrentIterator<TValue>>> enumerators,
      int? boundedCapacity,
      CancellationToken cancellationToken,
      Func<TValue, TCursor> mkCursor)
    {
      return enumerators.Select<KeyValuePair<TId, IConcurrentIterator<TValue>>, IConcurrentIterator<KeyValuePair<TId, TValue>>>((Func<KeyValuePair<TId, IConcurrentIterator<TValue>>, IConcurrentIterator<KeyValuePair<TId, TValue>>>) (s => s.Value.Select<TValue, KeyValuePair<TId, TValue>>((Func<TValue, KeyValuePair<TId, TValue>>) (x => Kvp.Create<TId, TValue>(s.Key, x))))).CollectUnordered<KeyValuePair<TId, TValue>>(boundedCapacity, cancellationToken).WithCompositeCursor<TValue, TId, TCursor>(mkCursor);
    }

    public static IConcurrentIteratorWithCursor<TValue, CompositeCursor<TId, TCursor>> CollectUnordered<TValue, TCursor, TId>(
      this IEnumerable<KeyValuePair<TId, IConcurrentIteratorWithCursor<TValue, TCursor>>> enumerators,
      int? boundedCapacity,
      CancellationToken cancellationToken)
    {
      CompositeCursor<TId, TCursor> initial = new CompositeCursor<TId, TCursor>(enumerators.Select<KeyValuePair<TId, IConcurrentIteratorWithCursor<TValue, TCursor>>, KeyValuePair<TId, TCursor>>((Func<KeyValuePair<TId, IConcurrentIteratorWithCursor<TValue, TCursor>>, KeyValuePair<TId, TCursor>>) (x => Kvp.Create<TId, TCursor>(x.Key, x.Value.Cursor))));
      return ((IEnumerable<IConcurrentIterator<KeyValuePair<TId, Tuple<TValue, TCursor>>>>) enumerators.Select<KeyValuePair<TId, IConcurrentIteratorWithCursor<TValue, TCursor>>, IConcurrentIteratorWithCursor<KeyValuePair<TId, Tuple<TValue, TCursor>>, TCursor>>((Func<KeyValuePair<TId, IConcurrentIteratorWithCursor<TValue, TCursor>>, IConcurrentIteratorWithCursor<KeyValuePair<TId, Tuple<TValue, TCursor>>, TCursor>>) (s => s.Value.Select<TValue, KeyValuePair<TId, Tuple<TValue, TCursor>>, TCursor>((Func<TValue, KeyValuePair<TId, Tuple<TValue, TCursor>>>) (x => Kvp.Create<TId, Tuple<TValue, TCursor>>(s.Key, Tuple.Create<TValue, TCursor>(x, s.Value.Cursor))))))).CollectUnordered<KeyValuePair<TId, Tuple<TValue, TCursor>>>(boundedCapacity, cancellationToken).WithCompositeCursor<Tuple<TValue, TCursor>, TId, TCursor>((Func<Tuple<TValue, TCursor>, TCursor>) (x => x.Item2), initial).Select<Tuple<TValue, TCursor>, TValue, CompositeCursor<TId, TCursor>>((Func<Tuple<TValue, TCursor>, TValue>) (x => x.Item1));
    }

    public static IConcurrentIteratorWithCursor<TValue, CompositeCursor<int, TCursor>> CollectUnordered<TValue, TCursor>(
      this IEnumerable<IConcurrentIterator<TValue>> enumerators,
      int? boundedCapacity,
      CancellationToken cancellationToken,
      Func<TValue, TCursor> mkCursor)
    {
      return enumerators.Select<IConcurrentIterator<TValue>, KeyValuePair<int, IConcurrentIterator<TValue>>>((Func<IConcurrentIterator<TValue>, int, KeyValuePair<int, IConcurrentIterator<TValue>>>) ((x, i) => Kvp.Create<int, IConcurrentIterator<TValue>>(i, x))).CollectUnordered<TValue, TCursor, int>(boundedCapacity, cancellationToken, mkCursor);
    }

    public static IConcurrentIteratorWithCursor<TValue, CompositeCursor<int, TCursor>> CollectUnordered<TValue, TCursor>(
      this IEnumerable<IConcurrentIteratorWithCursor<TValue, TCursor>> enumerators,
      int? boundedCapacity,
      CancellationToken cancellationToken)
    {
      return enumerators.Select<IConcurrentIteratorWithCursor<TValue, TCursor>, KeyValuePair<int, IConcurrentIteratorWithCursor<TValue, TCursor>>>((Func<IConcurrentIteratorWithCursor<TValue, TCursor>, int, KeyValuePair<int, IConcurrentIteratorWithCursor<TValue, TCursor>>>) ((x, i) => Kvp.Create<int, IConcurrentIteratorWithCursor<TValue, TCursor>>(i, x))).CollectUnordered<TValue, TCursor, int>(boundedCapacity, cancellationToken);
    }

    private static IConcurrentIteratorWithCursor<TValue, CompositeCursor<TId, TCursor>> WithCompositeCursor<TValue, TId, TCursor>(
      this IConcurrentIterator<KeyValuePair<TId, TValue>> enumerator,
      Func<TValue, TCursor> mkCursor,
      CompositeCursor<TId, TCursor> initial)
    {
      return enumerator.SelectWithState<CompositeCursor<TId, TCursor>, KeyValuePair<TId, TValue>, Tuple<CompositeCursor<TId, TCursor>, TValue>>((Func<CompositeCursor<TId, TCursor>, KeyValuePair<TId, TValue>, Tuple<CompositeCursor<TId, TCursor>, Tuple<CompositeCursor<TId, TCursor>, TValue>>>) ((cursor, current) =>
      {
        cursor[current.Key] = mkCursor(current.Value);
        return Tuple.Create<CompositeCursor<TId, TCursor>, Tuple<CompositeCursor<TId, TCursor>, TValue>>(cursor, Tuple.Create<CompositeCursor<TId, TCursor>, TValue>(cursor, current.Value));
      }), initial).WithCursor<Tuple<CompositeCursor<TId, TCursor>, TValue>, TValue, CompositeCursor<TId, TCursor>>((Func<Tuple<CompositeCursor<TId, TCursor>, TValue>, CompositeCursor<TId, TCursor>>) (x => x.Item1), (Func<Tuple<CompositeCursor<TId, TCursor>, TValue>, TValue>) (x => x.Item2), initial);
    }

    private static IConcurrentIteratorWithCursor<TValue, CompositeCursor<TId, TCursor>> WithCompositeCursor<TValue, TId, TCursor>(
      this IConcurrentIterator<KeyValuePair<TId, TValue>> enumerator,
      Func<TValue, TCursor> mkCursor)
    {
      return enumerator.WithCompositeCursor<TValue, TId, TCursor>(mkCursor, new CompositeCursor<TId, TCursor>());
    }

    public static IConcurrentIteratorWithCursor<TValue, CompositeCursor<TId, TCursor>> CollectUnordered<TValue, TCursor, TId>(
      this CompositeCursor<TId, TCursor> cursor,
      Func<TId, TCursor, IConcurrentIteratorWithCursor<TValue, TCursor>> factory,
      int? boundedCapacity,
      CancellationToken cancellationToken)
    {
      return cursor.Select<KeyValuePair<TId, TCursor>, KeyValuePair<TId, IConcurrentIteratorWithCursor<TValue, TCursor>>>((Func<KeyValuePair<TId, TCursor>, KeyValuePair<TId, IConcurrentIteratorWithCursor<TValue, TCursor>>>) (x => Kvp.Create<TId, IConcurrentIteratorWithCursor<TValue, TCursor>>(x.Key, factory(x.Key, x.Value)))).CollectUnordered<TValue, TCursor, TId>(boundedCapacity, cancellationToken);
    }

    public static IConcurrentIteratorWithCursor<TValue, TCursor> ToConcurrentIterator<TValue, TCursor>(
      this TCursor cursor,
      Func<TCursor, IConcurrentIteratorWithCursor<TValue, TCursor>> factory)
    {
      return factory(cursor);
    }

    public static IConcurrentIteratorWithCursor<T2, TCursor> Select<T1, T2, TCursor>(
      this IConcurrentIteratorWithCursor<T1, TCursor> enumerator,
      Func<T1, T2> select)
    {
      TCursor cursor = enumerator.Cursor;
      return enumerator.WithCursor<T1, T2, TCursor>((Func<T1, TCursor>) (_ => enumerator.Cursor), select, cursor);
    }
  }
}
