// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Common.CollectionExtensions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.AspNet.OData.Common
{
  internal static class CollectionExtensions
  {
    public static T[] AppendAndReallocate<T>(this T[] array, T value)
    {
      int length = array.Length;
      T[] objArray = new T[length + 1];
      array.CopyTo((Array) objArray, 0);
      objArray[length] = value;
      return objArray;
    }

    public static T[] AsArray<T>(this IEnumerable<T> values)
    {
      if (!(values is T[] objArray))
        objArray = values.ToArray<T>();
      return objArray;
    }

    public static Collection<T> AsCollection<T>(this IEnumerable<T> enumerable)
    {
      switch (enumerable)
      {
        case Collection<T> collection:
          return collection;
        case IList<T> list:
label_3:
          return new Collection<T>(list);
        default:
          list = (IList<T>) new List<T>(enumerable);
          goto label_3;
      }
    }

    public static IList<T> AsIList<T>(this IEnumerable<T> enumerable) => enumerable is IList<T> objList ? objList : (IList<T>) new List<T>(enumerable);

    public static List<T> AsList<T>(this IEnumerable<T> enumerable)
    {
      switch (enumerable)
      {
        case List<T> objList:
          return objList;
        case ListWrapperCollection<T> wrapperCollection:
          return wrapperCollection.ItemsList;
        default:
          return new List<T>(enumerable);
      }
    }

    public static void RemoveFrom<T>(this List<T> list, int start) => list.RemoveRange(start, list.Count - start);

    public static T SingleDefaultOrError<T, TArg1>(
      this IList<T> list,
      Action<TArg1> errorAction,
      TArg1 errorArg1)
    {
      switch (list.Count)
      {
        case 0:
          return default (T);
        case 1:
          return list[0];
        default:
          errorAction(errorArg1);
          return default (T);
      }
    }

    public static TMatch SingleOfTypeDefaultOrError<TInput, TMatch, TArg1>(
      this IList<TInput> list,
      Action<TArg1> errorAction,
      TArg1 errorArg1)
      where TMatch : class
    {
      TMatch match1 = default (TMatch);
      for (int index = 0; index < list.Count; ++index)
      {
        if (list[index] is TMatch match2)
        {
          if ((object) match1 == null)
          {
            match1 = match2;
          }
          else
          {
            errorAction(errorArg1);
            return default (TMatch);
          }
        }
      }
      return match1;
    }

    public static T[] ToArrayWithoutNulls<T>(this ICollection<T> collection) where T : class
    {
      T[] sourceArray = new T[collection.Count];
      int length = 0;
      foreach (T obj in (IEnumerable<T>) collection)
      {
        if ((object) obj != null)
        {
          sourceArray[length] = obj;
          ++length;
        }
      }
      if (length == collection.Count)
        return sourceArray;
      T[] destinationArray = new T[length];
      Array.Copy((Array) sourceArray, (Array) destinationArray, length);
      return destinationArray;
    }

    public static Dictionary<TKey, TValue> ToDictionaryFast<TKey, TValue>(
      this TValue[] array,
      Func<TValue, TKey> keySelector,
      IEqualityComparer<TKey> comparer)
    {
      Dictionary<TKey, TValue> dictionaryFast = new Dictionary<TKey, TValue>(array.Length, comparer);
      for (int index = 0; index < array.Length; ++index)
      {
        TValue obj = array[index];
        dictionaryFast.Add(keySelector(obj), obj);
      }
      return dictionaryFast;
    }

    public static Dictionary<TKey, TValue> ToDictionaryFast<TKey, TValue>(
      this IList<TValue> list,
      Func<TValue, TKey> keySelector,
      IEqualityComparer<TKey> comparer)
    {
      return list is TValue[] array ? array.ToDictionaryFast<TKey, TValue>(keySelector, comparer) : CollectionExtensions.ToDictionaryFastNoCheck<TKey, TValue>(list, keySelector, comparer);
    }

    public static Dictionary<TKey, TValue> ToDictionaryFast<TKey, TValue>(
      this IEnumerable<TValue> enumerable,
      Func<TValue, TKey> keySelector,
      IEqualityComparer<TKey> comparer)
    {
      switch (enumerable)
      {
        case TValue[] array:
          return array.ToDictionaryFast<TKey, TValue>(keySelector, comparer);
        case IList<TValue> list:
          return CollectionExtensions.ToDictionaryFastNoCheck<TKey, TValue>(list, keySelector, comparer);
        default:
          Dictionary<TKey, TValue> dictionaryFast = new Dictionary<TKey, TValue>(comparer);
          foreach (TValue obj in enumerable)
            dictionaryFast.Add(keySelector(obj), obj);
          return dictionaryFast;
      }
    }

    private static Dictionary<TKey, TValue> ToDictionaryFastNoCheck<TKey, TValue>(
      IList<TValue> list,
      Func<TValue, TKey> keySelector,
      IEqualityComparer<TKey> comparer)
    {
      int count = list.Count;
      Dictionary<TKey, TValue> dictionaryFastNoCheck = new Dictionary<TKey, TValue>(count, comparer);
      for (int index = 0; index < count; ++index)
      {
        TValue obj = list[index];
        dictionaryFastNoCheck.Add(keySelector(obj), obj);
      }
      return dictionaryFastNoCheck;
    }

    public static void MergeWithReplace<TKey, TValue>(
      this Dictionary<TKey, TValue> target,
      Dictionary<TKey, TValue> source)
    {
      foreach (KeyValuePair<TKey, TValue> keyValuePair in source)
        target[keyValuePair.Key] = keyValuePair.Value;
    }
  }
}
