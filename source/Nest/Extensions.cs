// Decompiled with JetBrains decompiler
// Type: Nest.Extensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nest
{
  internal static class Extensions
  {
    private static readonly ConcurrentDictionary<string, object> EnumCache = new ConcurrentDictionary<string, object>();

    internal static bool NotWritable(this QueryContainer q) => q == null || !q.IsWritable;

    internal static bool NotWritable(this IEnumerable<QueryContainer> qs) => qs == null || qs.All<QueryContainer>((Func<QueryContainer, bool>) (q => q.NotWritable()));

    internal static TReturn InvokeOrDefault<T, TReturn>(this Func<T, TReturn> func, T @default)
      where T : class, TReturn
      where TReturn : class
    {
      return (func != null ? func(@default) : default (TReturn)) ?? (TReturn) @default;
    }

    internal static TReturn InvokeOrDefault<T1, T2, TReturn>(
      this Func<T1, T2, TReturn> func,
      T1 @default,
      T2 param2)
      where T1 : class, TReturn
      where TReturn : class
    {
      return (func != null ? func(@default, param2) : default (TReturn)) ?? (TReturn) @default;
    }

    internal static IEnumerable<T> DistinctBy<T, TKey>(
      this IEnumerable<T> items,
      Func<T, TKey> property)
    {
      return items.GroupBy<T, TKey>(property).Select<IGrouping<TKey, T>, T>((Func<IGrouping<TKey, T>, T>) (x => x.First<T>()));
    }

    internal static string ToEnumValue<T>(this T enumValue) where T : struct
    {
      Type enumType = typeof (T);
      string name = Enum.GetName(enumType, (object) enumValue);
      EnumMemberAttribute customAttribute1 = enumType.GetField(name).GetCustomAttribute<EnumMemberAttribute>();
      if (customAttribute1 != null)
        return customAttribute1.Value;
      AlternativeEnumMemberAttribute customAttribute2 = enumType.GetField(name).GetCustomAttribute<AlternativeEnumMemberAttribute>();
      return customAttribute2 == null ? enumValue.ToString() : customAttribute2.Value;
    }

    internal static T? ToEnum<T>(this string str, StringComparison comparison = StringComparison.OrdinalIgnoreCase) where T : struct
    {
      if (str == null)
        return new T?();
      Type enumType = typeof (T);
      string key = enumType.Name + "." + str;
      object obj1;
      if (Extensions.EnumCache.TryGetValue(key, out obj1))
        return new T?((T) obj1);
      foreach (string name in Enum.GetNames(enumType))
      {
        if (name.Equals(str, comparison))
        {
          T obj2 = (T) Enum.Parse(enumType, name, true);
          Extensions.EnumCache.TryAdd(key, (object) obj2);
          return new T?(obj2);
        }
        FieldInfo field = enumType.GetField(name);
        EnumMemberAttribute customAttribute1 = field.GetCustomAttribute<EnumMemberAttribute>();
        if (customAttribute1 != null && customAttribute1.Value.Equals(str, comparison))
        {
          T obj3 = (T) Enum.Parse(enumType, name);
          Extensions.EnumCache.TryAdd(key, (object) obj3);
          return new T?(obj3);
        }
        AlternativeEnumMemberAttribute customAttribute2 = field.GetCustomAttribute<AlternativeEnumMemberAttribute>();
        if (customAttribute2 != null && customAttribute2.Value.Equals(str, comparison))
        {
          T obj4 = (T) Enum.Parse(enumType, name);
          Extensions.EnumCache.TryAdd(key, (object) obj4);
          return new T?(obj4);
        }
      }
      return new T?();
    }

    internal static string Utf8String(ref this ArraySegment<byte> segment) => StringEncoding.UTF8.GetString(segment.Array, segment.Offset, segment.Count);

    internal static string Utf8String(this byte[] bytes) => bytes != null ? Encoding.UTF8.GetString(bytes, 0, bytes.Length) : (string) null;

    internal static byte[] Utf8Bytes(this string s) => !s.IsNullOrEmpty() ? Encoding.UTF8.GetBytes(s) : (byte[]) null;

    internal static bool IsNullOrEmpty(this IndexName value) => value == (IndexName) null || value.GetHashCode() == 0;

    internal static bool IsNullable(this Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);

    internal static void ThrowIfNullOrEmpty(this string @object, string parameterName, string when = null)
    {
      @object.ThrowIfNull<string>(parameterName, when);
      if (string.IsNullOrWhiteSpace(@object))
        throw new ArgumentException("Argument can't be null or empty" + (when.IsNullOrEmpty() ? "" : " when " + when), parameterName);
    }

    internal static void ThrowIfEmpty<T>(this IEnumerable<T> @object, string parameterName)
    {
      if (@object == null)
        throw new ArgumentNullException(parameterName);
      if (!@object.Any<T>())
        throw new ArgumentException("Argument can not be an empty collection", parameterName);
    }

    internal static List<T> AsInstanceOrToListOrDefault<T>(this IEnumerable<T> list) => list is List<T> objList ? objList : (list != null ? list.ToList<T>() : (List<T>) null) ?? new List<T>();

    internal static List<T> AsInstanceOrToListOrNull<T>(this IEnumerable<T> list)
    {
      if (list is List<T> listOrNull)
        return listOrNull;
      return list == null ? (List<T>) null : list.ToList<T>();
    }

    internal static List<T> EagerConcat<T>(this IEnumerable<T> list, IEnumerable<T> other)
    {
      List<T> listOrDefault1 = list.AsInstanceOrToListOrDefault<T>();
      if (other == null)
        return listOrDefault1;
      List<T> listOrDefault2 = other.AsInstanceOrToListOrDefault<T>();
      List<T> objList = new List<T>(listOrDefault1.Count + listOrDefault2.Count);
      objList.AddRange((IEnumerable<T>) listOrDefault1);
      objList.AddRange((IEnumerable<T>) listOrDefault2);
      return objList;
    }

    internal static IEnumerable<T> AddIfNotNull<T>(this IEnumerable<T> list, T other)
    {
      if ((object) other == null)
        return list;
      List<T> listOrDefault = list.AsInstanceOrToListOrDefault<T>();
      listOrDefault.Add(other);
      return (IEnumerable<T>) listOrDefault;
    }

    internal static bool HasAny<T>(this IEnumerable<T> list, Func<T, bool> predicate) => list != null && list.Any<T>(predicate);

    internal static bool HasAny<T>(this IEnumerable<T> list) => list != null && list.Any<T>();

    internal static bool IsEmpty<T>(this IEnumerable<T> list)
    {
      if (list == null)
        return true;
      if (!(list is T[] objArray))
        objArray = list.ToArray<T>();
      T[] source = objArray;
      return !((IEnumerable<T>) source).Any<T>() || ((IEnumerable<T>) source).All<T>((Func<T, bool>) (t => (object) t == null));
    }

    internal static void ThrowIfNull<T>(this T value, string name, string message = null)
    {
      if ((object) value == null && message.IsNullOrEmpty())
        throw new ArgumentNullException(name);
      if ((object) value == null)
        throw new ArgumentNullException(name, "Argument can not be null when " + message);
    }

    internal static bool IsNullOrEmpty(this string value) => string.IsNullOrWhiteSpace(value);

    internal static bool IsNullOrEmptyCommaSeparatedList(this string value, out string[] split)
    {
      split = (string[]) null;
      if (string.IsNullOrWhiteSpace(value))
        return true;
      split = ((IEnumerable<string>) value.Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries)).Where<string>((Func<string, bool>) (t => !t.IsNullOrEmpty())).Select<string, string>((Func<string, string>) (t => t.Trim())).ToArray<string>();
      return split.Length == 0;
    }

    internal static List<T> ToListOrNullIfEmpty<T>(this IEnumerable<T> enumerable)
    {
      List<T> listOrNull = enumerable.AsInstanceOrToListOrNull<T>();
      return listOrNull == null || listOrNull.Count <= 0 ? (List<T>) null : listOrNull;
    }

    internal static void AddIfNotNull<T>(this IList<T> list, T item) where T : class
    {
      if ((object) item == null)
        return;
      list.Add(item);
    }

    internal static void AddRangeIfNotNull<T>(this List<T> list, IEnumerable<T> item) where T : class
    {
      if (item == null)
        return;
      list.AddRange(item.Where<T>((Func<T, bool>) (x => (object) x != null)));
    }

    internal static Dictionary<TKey, TValue> NullIfNoKeys<TKey, TValue>(
      this Dictionary<TKey, TValue> dictionary)
    {
      return dictionary?.Count.GetValueOrDefault(0) <= 0 ? (Dictionary<TKey, TValue>) null : dictionary;
    }

    internal static async Task ForEachAsync<TSource, TResult>(
      this IEnumerable<TSource> lazyList,
      Func<TSource, long, Task<TResult>> taskSelector,
      Action<TSource, TResult> resultProcessor,
      Action<Exception> done,
      int maxDegreeOfParallelism,
      SemaphoreSlim additionalRateLimiter = null)
    {
      SemaphoreSlim semaphore = new SemaphoreSlim(maxDegreeOfParallelism, maxDegreeOfParallelism);
      long page = 0;
      try
      {
        List<Task> tasks = new List<Task>(maxDegreeOfParallelism);
        foreach (TSource lazy in lazyList)
        {
          tasks.Add(Extensions.ProcessAsync<TSource, TResult>(lazy, taskSelector, resultProcessor, semaphore, additionalRateLimiter, page++));
          if (tasks.Count >= maxDegreeOfParallelism)
          {
            Task task = await Task.WhenAny((IEnumerable<Task>) tasks).ConfigureAwait(false);
            if (task.Exception != null && task.IsFaulted)
            {
              Exception source = task.Exception.Flatten().InnerExceptions.First<Exception>();
              if (source != null)
              {
                ExceptionDispatchInfo.Capture(source).Throw();
                semaphore = (SemaphoreSlim) null;
                return;
              }
            }
            tasks.Remove(task);
          }
        }
        await Task.WhenAll((IEnumerable<Task>) tasks).ConfigureAwait(false);
        done((Exception) null);
        tasks = (List<Task>) null;
        semaphore = (SemaphoreSlim) null;
      }
      catch (Exception ex)
      {
        done(ex);
        throw;
      }
    }

    private static async Task ProcessAsync<TSource, TResult>(
      TSource item,
      Func<TSource, long, Task<TResult>> taskSelector,
      Action<TSource, TResult> resultProcessor,
      SemaphoreSlim localRateLimiter,
      SemaphoreSlim additionalRateLimiter,
      long page)
    {
      if (localRateLimiter != null)
        await localRateLimiter.WaitAsync().ConfigureAwait(false);
      if (additionalRateLimiter != null)
        await additionalRateLimiter.WaitAsync().ConfigureAwait(false);
      try
      {
        resultProcessor(item, await taskSelector(item, page).ConfigureAwait(false));
      }
      finally
      {
        localRateLimiter?.Release();
        additionalRateLimiter?.Release();
      }
    }

    internal static bool NullOrEquals<T>(this T o, T other)
    {
      if ((object) o == null && (object) other == null)
        return true;
      return (object) o != null && (object) other != null && o.Equals((object) other);
    }
  }
}
