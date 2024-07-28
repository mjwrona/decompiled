// Decompiled with JetBrains decompiler
// Type: WebGrease.Extensions.EnumerableExtensions
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using WebGrease.Configuration;
using WebGrease.Css.Extensions;

namespace WebGrease.Extensions
{
  internal static class EnumerableExtensions
  {
    internal static bool HasAtLeast<T>(this IEnumerable<T> source, int atLeast)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      int num = 0;
      using (IEnumerator<T> enumerator = source.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          if (++num == atLeast)
            return true;
        }
      }
      return false;
    }

    internal static IEnumerable<TSource> DistinctBy<TSource, TKey>(
      this IEnumerable<TSource> source,
      Func<TSource, TKey> keySelector)
    {
      HashSet<TKey> hash = new HashSet<TKey>();
      return source.Where<TSource>((Func<TSource, bool>) (p => hash.Add(keySelector(p))));
    }

    internal static void AddRange<TKey, TValue>(
      this IDictionary<TKey, TValue> dictionary,
      IEnumerable<KeyValuePair<TKey, TValue>> range)
    {
      range.ForEach<KeyValuePair<TKey, TValue>>(new Action<KeyValuePair<TKey, TValue>>(((ICollection<KeyValuePair<TKey, TValue>>) dictionary).Add));
    }

    internal static void AddRange<TValue>(
      this BlockingCollection<TValue> collection,
      IEnumerable<TValue> range)
    {
      range.ForEach<TValue>(new Action<TValue>(collection.Add));
    }

    internal static void Add<TKey>(
      this IDictionary<TKey, double> dictionary1,
      IEnumerable<KeyValuePair<TKey, double>> dictionary2)
    {
      foreach (KeyValuePair<TKey, double> keyValuePair in dictionary2)
      {
        TKey key1 = keyValuePair.Key;
        if (!dictionary1.ContainsKey(key1))
          dictionary1[key1] = 0.0;
        IDictionary<TKey, double> dictionary;
        TKey key2;
        (dictionary = dictionary1)[key2 = key1] = dictionary[key2] + keyValuePair.Value;
      }
    }

    internal static TValue TryGetValue<TKey, TValue>(
      this IDictionary<TKey, TValue> dictionary,
      TKey key)
    {
      if (dictionary == null)
        throw new ArgumentNullException(nameof (dictionary));
      TValue obj;
      return !dictionary.TryGetValue(key, out obj) ? default (TValue) : obj;
    }

    internal static void Add<TKey>(
      this IDictionary<TKey, int> dictionary1,
      IEnumerable<KeyValuePair<TKey, int>> dictionary2)
    {
      foreach (KeyValuePair<TKey, int> keyValuePair in dictionary2)
      {
        TKey key1 = keyValuePair.Key;
        if (!dictionary1.ContainsKey(key1))
          dictionary1[key1] = 0;
        IDictionary<TKey, int> dictionary;
        TKey key2;
        (dictionary = dictionary1)[key2 = key1] = dictionary[key2] + keyValuePair.Value;
      }
    }

    internal static void AddNamedConfig<TConfig>(
      this IDictionary<string, TConfig> configs,
      TConfig config)
      where TConfig : INamedConfig, new()
    {
      configs[config.Name ?? string.Empty] = config;
    }

    internal static T GetNamedConfig<T>(
      this IDictionary<string, T> configDictionary,
      string configName = null)
      where T : class, INamedConfig, new()
    {
      if (configDictionary == null || !configDictionary.Any<KeyValuePair<string, T>>())
        return new T();
      configName = configName.AsNullIfWhiteSpace() ?? string.Empty;
      return configDictionary.TryGetValue<string, T>(configName) ?? configDictionary.TryGetValue<string, T>(string.Empty) ?? (configName.IsNullOrWhitespace() ? configDictionary.FirstOrDefault<KeyValuePair<string, T>>().Value : default (T)) ?? new T();
    }

    internal static TResult NullSafeAction<TObject, TResult>(
      this TObject obj,
      Func<TObject, TResult> action)
    {
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      return (object) obj != null ? action(obj) : default (TResult);
    }
  }
}
