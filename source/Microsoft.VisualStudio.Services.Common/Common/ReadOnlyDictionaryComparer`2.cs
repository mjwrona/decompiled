// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.ReadOnlyDictionaryComparer`2
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Common
{
  public class ReadOnlyDictionaryComparer<K, V> : IEqualityComparer<IReadOnlyDictionary<K, V>>
  {
    public static bool Equals(
      IReadOnlyDictionary<K, V> thisDictionary,
      IReadOnlyDictionary<K, V> thatDictionary,
      IEqualityComparer<V> valueComparer = null,
      Action<int, int> whenCountsNotEqual = null,
      Action<K, V> whenCorrespondingValueNotFound = null,
      Action<K, V, V> whenCorrespondingValueNotEqual = null)
    {
      if (thisDictionary == thatDictionary)
        return true;
      if (thisDictionary == null)
        return thatDictionary == null;
      if (thatDictionary == null)
        return false;
      if (thisDictionary.Count != thatDictionary.Count)
      {
        if (whenCountsNotEqual != null)
          whenCountsNotEqual(thisDictionary.Count, thatDictionary.Count);
        return false;
      }
      valueComparer = valueComparer ?? (IEqualityComparer<V>) EqualityComparer<V>.Default;
      return ReadOnlyDictionaryComparer<K, V>.IsSubset(thisDictionary, thatDictionary, valueComparer, whenCorrespondingValueNotFound, whenCorrespondingValueNotEqual) && ReadOnlyDictionaryComparer<K, V>.IsSubset(thatDictionary, thisDictionary, valueComparer, whenCorrespondingValueNotFound, whenCorrespondingValueNotEqual);
    }

    public static bool IsSubset(
      IReadOnlyDictionary<K, V> candidateSubsetDictionary,
      IReadOnlyDictionary<K, V> candidateSupersetDictionary,
      IEqualityComparer<V> valueComparer = null,
      Action<K, V> whenCorrespondingValueNotFound = null,
      Action<K, V, V> whenCorrespondingValueNotEqual = null)
    {
      foreach (KeyValuePair<K, V> candidateSubset in (IEnumerable<KeyValuePair<K, V>>) candidateSubsetDictionary)
      {
        V y = default (V);
        if (!candidateSupersetDictionary.TryGetValue(candidateSubset.Key, out y))
        {
          if (whenCorrespondingValueNotFound != null)
            whenCorrespondingValueNotFound(candidateSubset.Key, candidateSubset.Value);
          return false;
        }
        if (!valueComparer.Equals(candidateSubset.Value, y))
        {
          if (whenCorrespondingValueNotEqual != null)
            whenCorrespondingValueNotEqual(candidateSubset.Key, candidateSubset.Value, y);
          return false;
        }
      }
      return true;
    }

    bool IEqualityComparer<IReadOnlyDictionary<K, V>>.Equals(
      IReadOnlyDictionary<K, V> thisDictionary,
      IReadOnlyDictionary<K, V> thatDictionary)
    {
      return ReadOnlyDictionaryComparer<K, V>.Equals(thisDictionary, thatDictionary, (IEqualityComparer<V>) null, (Action<int, int>) null, (Action<K, V>) null, (Action<K, V, V>) null);
    }

    public int GetHashCode(IReadOnlyDictionary<K, V> dictionary)
    {
      int hashCode = 7243;
      foreach (KeyValuePair<K, V> keyValuePair in (IEnumerable<KeyValuePair<K, V>>) dictionary)
      {
        hashCode = 524287 * hashCode + keyValuePair.Key.GetHashCode();
        int num = 524287 * hashCode;
        V v = keyValuePair.Value;
        ref V local = ref v;
        int? nullable1;
        int? nullable2;
        if ((object) local == null)
        {
          nullable1 = new int?();
          nullable2 = nullable1;
        }
        else
          nullable2 = new int?(local.GetHashCode());
        int? nullable3 = nullable2;
        int? nullable4;
        if (!nullable3.HasValue)
        {
          nullable1 = new int?();
          nullable4 = nullable1;
        }
        else
          nullable4 = new int?(num + nullable3.GetValueOrDefault());
        nullable1 = nullable4;
        hashCode = nullable1.GetValueOrDefault();
      }
      return hashCode;
    }
  }
}
