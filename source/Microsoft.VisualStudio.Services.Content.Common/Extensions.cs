// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Extensions
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class Extensions
  {
    private static int encodingBufferSize = 4096;

    public static Stream ToStream(this string input) => input.ToStream(StrictEncodingWithoutBOM.UTF8);

    public static Stream ToStream(this string input, Encoding encoding)
    {
      MemoryStream stream = new MemoryStream();
      using (StreamWriter streamWriter = new StreamWriter((Stream) stream, encoding, Extensions.encodingBufferSize, true))
      {
        streamWriter.Write(input);
        streamWriter.Flush();
        stream.Position = 0L;
      }
      return (Stream) stream;
    }

    public static string GetString(this Stream input)
    {
      using (StreamReader streamReader = new StreamReader(input, StrictEncodingWithBOM.UTF8))
        return streamReader.ReadToEnd();
    }

    public static IEnumerable<KeyValuePair<T3, T4>> KeyValueSelect<T1, T2, T3, T4>(
      this IEnumerable<KeyValuePair<T1, T2>> items,
      Func<KeyValuePair<T1, T2>, T3> keySelect,
      Func<KeyValuePair<T1, T2>, T4> valueSelect)
    {
      return items.Select<KeyValuePair<T1, T2>, KeyValuePair<T3, T4>>((Func<KeyValuePair<T1, T2>, KeyValuePair<T3, T4>>) (kvp => new KeyValuePair<T3, T4>(keySelect(kvp), valueSelect(kvp))));
    }

    public static IEnumerable<KeyValuePair<T3, T4>> BaseCast<T1, T2, T3, T4>(
      this IEnumerable<KeyValuePair<T1, T2>> items)
      where T1 : T3
      where T2 : T4
    {
      return items.KeyValueSelect<T1, T2, T3, T4>((Func<KeyValuePair<T1, T2>, T3>) (kvp => (T3) kvp.Key), (Func<KeyValuePair<T1, T2>, T4>) (kvp => (T4) kvp.Value));
    }

    public static IEnumerable<T2> Values<T1, T2>(this IEnumerable<KeyValuePair<T1, T2>> items) => items.Select<KeyValuePair<T1, T2>, T2>((Func<KeyValuePair<T1, T2>, T2>) (kvp => kvp.Value));

    public static IEnumerable<List<T>> GetPages<T>(this IEnumerable<T> source, int pageSize)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (pageSize <= 0)
        throw new ArgumentOutOfRangeException(nameof (pageSize), "pageSize must be > 0");
      using (IEnumerator<T> enumerator = source.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          List<T> page = new List<T>(pageSize)
          {
            enumerator.Current
          };
          while (page.Count < pageSize && enumerator.MoveNext())
            page.Add(enumerator.Current);
          yield return page;
        }
      }
    }

    [CLSCompliant(false)]
    public static ulong Sum<TSource>(
      this IEnumerable<TSource> source,
      Func<TSource, ulong> selector)
    {
      return source.Aggregate<TSource, ulong>(0UL, (Func<ulong, TSource, ulong>) ((acc, item) => acc + selector(item)));
    }

    public static IEnumerable<IDictionary<TKey, IEnumerable<TValue>>> GetDictionaryPages<TKey, TValue>(
      this IDictionary<TKey, IEnumerable<TValue>> source,
      int pageSize)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (pageSize <= 0)
        throw new ArgumentOutOfRangeException(nameof (pageSize), "pageSize must be > 0");
      int num = 0;
      Dictionary<TKey, IEnumerable<TValue>> dictionaryPage = new Dictionary<TKey, IEnumerable<TValue>>();
      foreach (KeyValuePair<TKey, IEnumerable<TValue>> keyValuePair in (IEnumerable<KeyValuePair<TKey, IEnumerable<TValue>>>) source)
      {
        KeyValuePair<TKey, IEnumerable<TValue>> kvp = keyValuePair;
        IEnumerable<TValue> list = kvp.Value;
        while (list.Any<TValue>())
        {
          int count = pageSize - num;
          IEnumerable<TValue> source1 = list.Take<TValue>(count);
          dictionaryPage[kvp.Key] = source1;
          list = list.Skip<TValue>(count);
          num += source1.Count<TValue>();
          if (num == pageSize)
          {
            yield return (IDictionary<TKey, IEnumerable<TValue>>) dictionaryPage;
            dictionaryPage = new Dictionary<TKey, IEnumerable<TValue>>();
            num = 0;
          }
        }
        list = (IEnumerable<TValue>) null;
        kvp = new KeyValuePair<TKey, IEnumerable<TValue>>();
      }
      if (num != 0)
        yield return (IDictionary<TKey, IEnumerable<TValue>>) dictionaryPage;
    }

    public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(
      this IEnumerable<KeyValuePair<TKey, TValue>> kvps)
    {
      return (IDictionary<TKey, TValue>) kvps.ToDictionary<KeyValuePair<TKey, TValue>, TKey, TValue>((Func<KeyValuePair<TKey, TValue>, TKey>) (kvp => kvp.Key), (Func<KeyValuePair<TKey, TValue>, TValue>) (kvp => kvp.Value));
    }

    public static IDictionary<TKey, IList<TValue>> ToDictionaryOfLists<TKey, TValue>(
      this IEnumerable<IGrouping<TKey, TValue>> groupings,
      int? maxToTakeInEachGroup = null)
    {
      return (IDictionary<TKey, IList<TValue>>) groupings.ToDictionary<IGrouping<TKey, TValue>, TKey, IList<TValue>>((Func<IGrouping<TKey, TValue>, TKey>) (grouping => grouping.Key), (Func<IGrouping<TKey, TValue>, IList<TValue>>) (grouping => (IList<TValue>) (maxToTakeInEachGroup.HasValue ? grouping.Take<TValue>(maxToTakeInEachGroup.Value) : (IEnumerable<TValue>) grouping).ToList<TValue>()));
    }

    public static IDictionary<TKey, IEnumerable<TValue>> ToDictionaryOfEnumerables<TKey, TValue>(
      this IEnumerable<IGrouping<TKey, TValue>> groupings,
      int? maxToTakeInEachGroup = null)
    {
      return (IDictionary<TKey, IEnumerable<TValue>>) groupings.ToDictionary<IGrouping<TKey, TValue>, TKey, IEnumerable<TValue>>((Func<IGrouping<TKey, TValue>, TKey>) (grouping => grouping.Key), (Func<IGrouping<TKey, TValue>, IEnumerable<TValue>>) (grouping => !maxToTakeInEachGroup.HasValue ? (IEnumerable<TValue>) grouping : grouping.Take<TValue>(maxToTakeInEachGroup.Value)));
    }

    public static IDictionary<TKey, IEnumerable<TValue>> ToDictionaryOfEnumerables<TKey, TValue>(
      this IEnumerable<KeyValuePair<TKey, IEnumerable<TValue>>> kvps,
      int? maxToTakeInEachGroup = null)
    {
      return (IDictionary<TKey, IEnumerable<TValue>>) kvps.ToDictionary<KeyValuePair<TKey, IEnumerable<TValue>>, TKey, IEnumerable<TValue>>((Func<KeyValuePair<TKey, IEnumerable<TValue>>, TKey>) (kvp => kvp.Key), (Func<KeyValuePair<TKey, IEnumerable<TValue>>, IEnumerable<TValue>>) (kvp => !maxToTakeInEachGroup.HasValue ? kvp.Value : kvp.Value.Take<TValue>(maxToTakeInEachGroup.Value)));
    }

    public static Dictionary<TKey, TElement> ToDictionaryFirstKeyWins<TSource, TKey, TElement>(
      this IEnumerable<TSource> source,
      Func<TSource, TKey> keySelector,
      Func<TSource, TElement> elementSelector)
    {
      return source.Select<TSource, KeyValuePair<TKey, TElement>>((Func<TSource, KeyValuePair<TKey, TElement>>) (s => new KeyValuePair<TKey, TElement>(keySelector(s), elementSelector(s)))).ToDictionaryFirstKeyWins<TKey, TElement>();
    }

    public static Dictionary<TKey, TValue> ToDictionaryFirstKeyWins<TKey, TValue>(
      this IEnumerable<KeyValuePair<TKey, TValue>> kvps)
    {
      Dictionary<TKey, TValue> dictionaryFirstKeyWins = new Dictionary<TKey, TValue>();
      foreach (KeyValuePair<TKey, TValue> kvp in kvps)
      {
        if (!dictionaryFirstKeyWins.ContainsKey(kvp.Key))
          dictionaryFirstKeyWins.Add(kvp.Key, kvp.Value);
        else if (!dictionaryFirstKeyWins[kvp.Key].Equals((object) kvp.Value))
          throw new ArgumentException("Identical keys have different values.");
      }
      return dictionaryFirstKeyWins;
    }

    public static Dictionary<TKey, TElement> ToDictionaryAnyWins<TSource, TKey, TElement>(
      this IEnumerable<TSource> source,
      Func<TSource, TKey> keySelector,
      Func<TSource, TElement> elementSelector)
    {
      return source.Select<TSource, KeyValuePair<TKey, TElement>>((Func<TSource, KeyValuePair<TKey, TElement>>) (s => new KeyValuePair<TKey, TElement>(keySelector(s), elementSelector(s)))).ToDictionaryAnyWins<TKey, TElement>();
    }

    public static Dictionary<TKey, TValue> ToDictionaryAnyWins<TKey, TValue>(
      this IEnumerable<KeyValuePair<TKey, TValue>> kvps)
    {
      Dictionary<TKey, TValue> dictionaryAnyWins = new Dictionary<TKey, TValue>();
      foreach (KeyValuePair<TKey, TValue> kvp in kvps)
      {
        if (!dictionaryAnyWins.ContainsKey(kvp.Key))
          dictionaryAnyWins.Add(kvp.Key, kvp.Value);
      }
      return dictionaryAnyWins;
    }

    public static bool TryRemoveSpecific<TKey, TValue>(
      this ConcurrentDictionary<TKey, TValue> dictionary,
      TKey key,
      TValue value)
    {
      if (dictionary == null)
        throw new ArgumentNullException(nameof (dictionary));
      return ((ICollection<KeyValuePair<TKey, TValue>>) dictionary).Remove(new KeyValuePair<TKey, TValue>(key, value));
    }

    public static bool GetOrAdd<TKey, TValue>(
      this ConcurrentDictionary<TKey, TValue> dict,
      TKey key,
      TValue value,
      out TValue finalValue)
    {
      while (!dict.TryGetValue(key, out finalValue))
      {
        if (dict.TryAdd(key, value))
        {
          finalValue = value;
          return true;
        }
      }
      return false;
    }

    public static bool GetOrAdd<TKey, TValue>(
      this ConcurrentDictionary<TKey, TValue> dict,
      TKey key,
      Func<TKey, TValue> generator,
      out TValue finalValue)
    {
      while (!dict.TryGetValue(key, out finalValue))
      {
        finalValue = generator(key);
        if (dict.TryAdd(key, finalValue))
          return true;
      }
      return false;
    }

    public static void AddRange<T>(this ConcurrentBag<T> bag, IEnumerable<T> values)
    {
      if (values == null)
        throw new ArgumentNullException("Values cannot be null when adding into conccurent bag.");
      foreach (T obj in values)
        bag.Add(obj);
    }

    public static DateTimeOffset RoundDownUtc(
      this DateTimeOffset dateToRound,
      DateTimeOffset baseDate,
      TimeSpan roundAmount)
    {
      if (dateToRound.Offset != TimeSpan.Zero)
        throw new ArgumentException("dateToRound is not UTC.");
      if (baseDate.Offset != TimeSpan.Zero)
        throw new ArgumentException("baseDate is not UTC.");
      if (dateToRound < baseDate)
        throw new ArgumentException("dateToRound is before baseDate.");
      if (roundAmount < TimeSpan.Zero)
        throw new ArgumentException("roundAmount must be greater than zero.");
      long num = (dateToRound - baseDate).Ticks / roundAmount.Ticks * roundAmount.Ticks;
      DateTimeOffset dateTimeOffset = new DateTimeOffset(baseDate.Ticks + num, TimeSpan.Zero);
      if (dateTimeOffset > dateToRound)
        throw new Exception(string.Format("{0}: {1} {2} > {3} {4}", (object) nameof (RoundDownUtc), (object) "rounded", (object) dateTimeOffset, (object) nameof (dateToRound), (object) dateToRound));
      if (dateTimeOffset <= dateToRound - roundAmount)
        throw new Exception(string.Format("{0}: {1} {2} <= {3} {4} - {5} {6}", (object) nameof (RoundDownUtc), (object) "rounded", (object) dateTimeOffset, (object) nameof (dateToRound), (object) dateToRound, (object) nameof (roundAmount), (object) roundAmount));
      return dateTimeOffset;
    }

    public static DateTimeOffset RoundUpUtc(
      this DateTimeOffset dateToRound,
      DateTimeOffset baseDate,
      TimeSpan roundAmount)
    {
      if (dateToRound.Offset != TimeSpan.Zero)
        throw new ArgumentException("dateToRound is not UTC.");
      if (baseDate.Offset != TimeSpan.Zero)
        throw new ArgumentException("baseDate is not UTC.");
      if (dateToRound < baseDate)
        throw new ArgumentException("dateToRound is before baseDate.");
      if (roundAmount < TimeSpan.Zero)
        throw new ArgumentException("roundAmount must be greater than zero.");
      DateTimeOffset dateTimeOffset = new DateTimeOffset(dateToRound.Ticks + roundAmount.Ticks - 1L, TimeSpan.Zero).RoundDownUtc(baseDate, roundAmount);
      if (dateTimeOffset < dateToRound)
        throw new Exception(string.Format("{0}: {1} {2} < {3} {4}", (object) nameof (RoundUpUtc), (object) "rounded", (object) dateTimeOffset, (object) nameof (dateToRound), (object) dateToRound));
      if (dateTimeOffset >= dateToRound + roundAmount)
        throw new Exception(string.Format("{0}: {1} {2} >= {3} {4} + {5} {6}", (object) nameof (RoundUpUtc), (object) "rounded", (object) dateTimeOffset, (object) nameof (dateToRound), (object) dateToRound, (object) nameof (roundAmount), (object) roundAmount));
      return dateTimeOffset;
    }

    public static DateTime Max(this DateTime date1, DateTime date2) => DateTime.FromFileTimeUtc(Math.Max(date1.ToFileTimeUtc(), date2.ToFileTimeUtc()));

    public static Uri RemoveQueryParameter(this Uri uri, string key)
    {
      NameValueCollection newQueryString = HttpUtility.ParseQueryString(uri.Query);
      newQueryString.Remove(key);
      string leftPart = uri.GetLeftPart(UriPartial.Path);
      string[] array = ((IEnumerable<string>) newQueryString.AllKeys).SelectMany<string, string, string>((Func<string, IEnumerable<string>>) (k => (IEnumerable<string>) newQueryString.GetValues(k)), (Func<string, string, string>) ((k, v) => string.Format("{0}={1}", (object) HttpUtility.UrlPathEncode(k), (object) HttpUtility.UrlPathEncode(v)))).ToArray<string>();
      return new Uri(newQueryString.Count > 0 ? leftPart + "?" + string.Join("&", array) : leftPart);
    }
  }
}
