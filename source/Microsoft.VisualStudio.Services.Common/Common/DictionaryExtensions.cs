// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.DictionaryExtensions
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class DictionaryExtensions
  {
    public static V AddOrUpdate<K, V>(
      this IDictionary<K, V> dictionary,
      K key,
      V addValue,
      Func<V, V, V> updateValueFactory)
    {
      if (dictionary is ConcurrentDictionary<K, V> concurrentDictionary)
        return concurrentDictionary.AddOrUpdate(key, addValue, (Func<K, V, V>) ((k, existing) => updateValueFactory(existing, addValue)));
      V v;
      if (dictionary.TryGetValue(key, out v))
        addValue = updateValueFactory(v, addValue);
      dictionary[key] = addValue;
      return addValue;
    }

    public static V AddOrUpdate<K, V>(
      this IDictionary<K, V> dictionary,
      K key,
      V addValue,
      Func<K, V, V> updateValueFactory)
    {
      if (dictionary is ConcurrentDictionary<K, V> concurrentDictionary)
        return concurrentDictionary.AddOrUpdate(key, addValue, updateValueFactory);
      V v;
      if (dictionary.TryGetValue(key, out v))
        addValue = updateValueFactory(key, v);
      dictionary[key] = addValue;
      return addValue;
    }

    public static V GetValueOrDefault<K, V>(this IDictionary<K, V> dictionary, K key, V @default = null)
    {
      V v;
      return !dictionary.TryGetValue(key, out v) ? @default : v;
    }

    public static V GetValueOrDefault<K, V>(
      this IReadOnlyDictionary<K, V> dictionary,
      K key,
      V @default = null)
    {
      V v;
      return !dictionary.TryGetValue(key, out v) ? @default : v;
    }

    public static V GetValueOrDefault<K, V>(this Dictionary<K, V> dictionary, K key, V @default = null)
    {
      V v;
      return !dictionary.TryGetValue(key, out v) ? @default : v;
    }

    public static V? GetNullableValueOrDefault<K, V>(
      this IDictionary<K, V> dictionary,
      K key,
      V? @default = null)
      where V : struct
    {
      V v;
      return !dictionary.TryGetValue(key, out v) ? @default : new V?(v);
    }

    public static V? GetNullableValueOrDefault<K, V>(
      this IReadOnlyDictionary<K, V> dictionary,
      K key,
      V? @default = null)
      where V : struct
    {
      V v;
      return !dictionary.TryGetValue(key, out v) ? @default : new V?(v);
    }

    public static V? GetNullableValueOrDefault<K, V>(
      this Dictionary<K, V> dictionary,
      K key,
      V? @default = null)
      where V : struct
    {
      V v;
      return !dictionary.TryGetValue(key, out v) ? @default : new V?(v);
    }

    public static V GetCastedValueOrDefault<K, V>(
      this IReadOnlyDictionary<K, object> dictionary,
      K key,
      V @default = null)
    {
      object obj;
      return !dictionary.TryGetValue(key, out obj) || !(obj is V v) ? @default : v;
    }

    public static V GetCastedValueOrDefault<K, V>(
      this IDictionary<K, object> dictionary,
      K key,
      V @default = null)
    {
      object obj;
      return !dictionary.TryGetValue(key, out obj) || !(obj is V v) ? @default : v;
    }

    public static V GetCastedValueOrDefault<K, V>(
      this Dictionary<K, object> dictionary,
      K key,
      V @default = null)
    {
      return DictionaryExtensions.GetCastedValueOrDefault<K, V>(dictionary, key, @default);
    }

    public static V GetOrAddValue<K, V>(this IDictionary<K, V> dictionary, K key) where V : new()
    {
      V orAddValue = default (V);
      if (!dictionary.TryGetValue(key, out orAddValue))
      {
        orAddValue = new V();
        dictionary.Add(key, orAddValue);
      }
      return orAddValue;
    }

    public static V GetOrAddValue<K, V>(
      this IDictionary<K, V> dictionary,
      K key,
      Func<V> createValueToAdd)
    {
      V orAddValue = default (V);
      if (!dictionary.TryGetValue(key, out orAddValue))
      {
        orAddValue = createValueToAdd();
        dictionary.Add(key, orAddValue);
      }
      return orAddValue;
    }

    public static TDictionary SetRange<K, V, TDictionary>(
      this TDictionary dictionary,
      IEnumerable<KeyValuePair<K, V>> keyValuePairs)
      where TDictionary : IDictionary<K, V>
    {
      foreach (KeyValuePair<K, V> keyValuePair in keyValuePairs)
      {
        ref TDictionary local = ref dictionary;
        TDictionary dictionary1 = default (TDictionary);
        if ((object) dictionary1 == null)
        {
          dictionary1 = local;
          local = ref dictionary1;
        }
        K key = keyValuePair.Key;
        V v = keyValuePair.Value;
        local[key] = v;
      }
      return dictionary;
    }

    public static TDictionary SetRangeIfRangeNotNull<K, V, TDictionary>(
      this TDictionary dictionary,
      IEnumerable<KeyValuePair<K, V>> keyValuePairs)
      where TDictionary : IDictionary<K, V>
    {
      if (keyValuePairs != null)
        dictionary.SetRange<K, V, TDictionary>(keyValuePairs);
      return dictionary;
    }

    public static Lazy<TDictionary> SetRangeIfRangeNotNullOrEmpty<K, V, TDictionary>(
      this Lazy<TDictionary> lazyDictionary,
      IEnumerable<KeyValuePair<K, V>> keyValuePairs)
      where TDictionary : IDictionary<K, V>
    {
      if (keyValuePairs != null && keyValuePairs.Any<KeyValuePair<K, V>>())
        lazyDictionary.Value.SetRange<K, V, TDictionary>(keyValuePairs);
      return lazyDictionary;
    }

    public static bool TryAdd<TKey, TValue>(
      this IDictionary<TKey, TValue> dictionary,
      TKey key,
      TValue value)
    {
      if (dictionary.ContainsKey(key))
        return false;
      dictionary.Add(key, value);
      return true;
    }

    public static bool TryAddRange<TKey, TValue, TDictionary>(
      this TDictionary dictionary,
      IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
      where TDictionary : IDictionary<TKey, TValue>
    {
      bool flag = true;
      foreach (KeyValuePair<TKey, TValue> keyValuePair in keyValuePairs)
        flag &= dictionary.TryAdd<TKey, TValue>(keyValuePair.Key, keyValuePair.Value);
      return flag;
    }

    public static bool TryGetValue<T>(
      this IDictionary<string, object> dictionary,
      string key,
      out T value)
    {
      object obj1;
      if (dictionary.TryGetValue(key, out obj1))
      {
        Guid guid;
        if (typeof (T) == typeof (Guid) && dictionary.TryGetGuid(key, out guid))
        {
          value = (T) (ValueType) guid;
          return true;
        }
        if (typeof (T).GetTypeInfo().IsEnum && dictionary.TryGetEnum<T>(key, out value))
          return true;
        if (obj1 is T obj2)
        {
          value = obj2;
          return true;
        }
      }
      value = default (T);
      return false;
    }

    public static bool TryGetValidatedValue<T>(
      this IDictionary<string, object> dictionary,
      string key,
      out T value,
      bool allowNull = true)
    {
      value = default (T);
      if (!PropertyValidation.IsValidConvertibleType(typeof (T)))
        return false;
      if (typeof (T) == typeof (Guid))
      {
        Guid guid;
        if (dictionary.TryGetGuid(key, out guid))
        {
          value = (T) (ValueType) guid;
          return true;
        }
      }
      else
      {
        object obj = (object) null;
        if (dictionary.TryGetValue(key, out obj))
        {
          if (obj == null)
            return allowNull;
          if (typeof (T).GetTypeInfo().IsAssignableFrom(obj.GetType().GetTypeInfo()))
          {
            value = (T) obj;
            return true;
          }
          if (typeof (T).GetTypeInfo().IsEnum && dictionary.TryGetEnum<T>(key, out value))
            return true;
          if (obj is string)
          {
            TypeCode typeCode = Type.GetTypeCode(typeof (T));
            try
            {
              value = (T) Convert.ChangeType(obj, typeCode, (IFormatProvider) CultureInfo.CurrentCulture);
              return true;
            }
            catch (Exception ex)
            {
              return false;
            }
          }
        }
      }
      return false;
    }

    public static bool TryGetEnum<T>(
      this IDictionary<string, object> dictionary,
      string key,
      out T value)
    {
      value = default (T);
      object obj = (object) null;
      if (dictionary.TryGetValue(key, out obj))
      {
        if (obj is string)
        {
          try
          {
            value = (T) Enum.Parse(typeof (T), (string) obj, true);
            return true;
          }
          catch (ArgumentException ex)
          {
          }
        }
        else
        {
          try
          {
            value = (T) obj;
            return true;
          }
          catch (InvalidCastException ex)
          {
          }
        }
      }
      return false;
    }

    public static bool TryGetGuid(
      this IDictionary<string, object> dictionary,
      string key,
      out Guid value)
    {
      value = Guid.Empty;
      object input = (object) null;
      if (dictionary.TryGetValue(key, out input))
      {
        switch (input)
        {
          case Guid guid:
            value = guid;
            return true;
          case string _:
            return Guid.TryParse((string) input, out value);
        }
      }
      return false;
    }

    public static IDictionary<TKey, TValue> Copy<TKey, TValue>(
      this IDictionary<TKey, TValue> source,
      IDictionary<TKey, TValue> dest,
      Predicate<TKey> filter)
    {
      if (dest == null)
        return dest;
      foreach (TKey key in (IEnumerable<TKey>) source.Keys)
      {
        if (filter == null || filter(key))
          dest[key] = source[key];
      }
      return dest;
    }

    public static IDictionary<TKey, TValue> Copy<TKey, TValue>(
      this IDictionary<TKey, TValue> source,
      IDictionary<TKey, TValue> dest)
    {
      return source.Copy<TKey, TValue>(dest, (Predicate<TKey>) null);
    }

    public static IDictionary<TKey, TValue> SetIfNotNull<TKey, TValue>(
      this IDictionary<TKey, TValue> dictionary,
      TKey key,
      TValue value)
      where TValue : class
    {
      if ((object) value != null)
        dictionary[key] = value;
      return dictionary;
    }

    public static Lazy<IDictionary<TKey, TValue>> SetIfNotNull<TKey, TValue>(
      this Lazy<IDictionary<TKey, TValue>> dictionary,
      TKey key,
      TValue value)
      where TValue : class
    {
      if ((object) value != null)
        dictionary.Value[key] = value;
      return dictionary;
    }

    public static IDictionary<TKey, TValue> SetIfNotNullAndNotConflicting<TKey, TValue>(
      this IDictionary<TKey, TValue> dictionary,
      TKey key,
      TValue value,
      string valuePropertyName = "value",
      string dictionaryName = "dictionary")
      where TValue : class
    {
      if ((object) value == null)
        return dictionary;
      dictionary.CheckForConflict<TKey, TValue>(key, value, valuePropertyName, dictionaryName);
      dictionary[key] = value;
      return dictionary;
    }

    public static IDictionary<TKey, TValue> SetIfNotConflicting<TKey, TValue>(
      this IDictionary<TKey, TValue> dictionary,
      TKey key,
      TValue value,
      string valuePropertyName = "value",
      string dictionaryName = "dictionary")
    {
      dictionary.CheckForConflict<TKey, TValue>(key, value, valuePropertyName, dictionaryName, false);
      dictionary[key] = value;
      return dictionary;
    }

    public static void CheckForConflict<TKey, TValue>(
      this IDictionary<TKey, TValue> dictionary,
      TKey key,
      TValue value,
      string valuePropertyName = "value",
      string dictionaryName = "dictionary",
      bool ignoreDefaultValue = true)
    {
      if (object.Equals((object) value, (object) default (TValue)) & ignoreDefaultValue)
        return;
      TValue obj = default (TValue);
      if (dictionary.TryGetValue(key, out obj) && !(object.Equals((object) obj, (object) default (TValue)) & ignoreDefaultValue) && !object.Equals((object) value, (object) obj))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Parameter {0} = '{1}' inconsistent with {2}['{3}'] => '{4}'", (object) valuePropertyName, (object) value, (object) dictionaryName, (object) key, (object) obj));
    }

    public static void CheckForConflict<TKey, TValue>(
      this IReadOnlyDictionary<TKey, TValue> dictionary,
      TKey key,
      TValue value,
      string valuePropertyName = "value",
      string dictionaryName = "dictionary",
      bool ignoreDefaultValue = true)
    {
      if (object.Equals((object) value, (object) default (TValue)) & ignoreDefaultValue)
        return;
      TValue obj = default (TValue);
      if (dictionary.TryGetValue(key, out obj) && !(object.Equals((object) obj, (object) default (TValue)) & ignoreDefaultValue) && !object.Equals((object) value, (object) obj))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Parameter {0} = \"{1}\" is inconsistent with {2}[\"{3}\"] => \"{4}\"", (object) valuePropertyName, (object) value, (object) dictionaryName, (object) key, (object) obj));
    }
  }
}
