// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DictionaryMerger
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data
{
  public static class DictionaryMerger
  {
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for complexity of type")]
    public static IDictionary<TKey, TValue> MergeDictionaries<TKey, TValue>(
      IEqualityComparer<TKey> comparer,
      IEnumerable<IDictionary<TKey, TValue>> dictionaries)
    {
      return dictionaries == null || !dictionaries.Any<IDictionary<TKey, TValue>>() ? (IDictionary<TKey, TValue>) null : dictionaries.Aggregate<IDictionary<TKey, TValue>>((Func<IDictionary<TKey, TValue>, IDictionary<TKey, TValue>, IDictionary<TKey, TValue>>) ((dominant, recessive) => DictionaryMerger.Merge<TKey, TValue>(comparer, dominant, recessive)));
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for complexity of type")]
    public static IDictionary<TKey, TValue> MergeDictionaries<TKey, TValue>(
      IEnumerable<IDictionary<TKey, TValue>> dictionaries)
    {
      return dictionaries == null || !dictionaries.Any<IDictionary<TKey, TValue>>() ? (IDictionary<TKey, TValue>) null : dictionaries.Aggregate<IDictionary<TKey, TValue>>((Func<IDictionary<TKey, TValue>, IDictionary<TKey, TValue>, IDictionary<TKey, TValue>>) ((dominant, recessive) => DictionaryMerger.Merge<TKey, TValue>((IEqualityComparer<TKey>) EqualityComparer<TKey>.Default, dominant, recessive)));
    }

    private static IDictionary<TKey, TValue> Merge<TKey, TValue>(
      IEqualityComparer<TKey> comparer,
      IDictionary<TKey, TValue> dominantKvps,
      IDictionary<TKey, TValue> recessiveKvps)
    {
      if (dominantKvps == null)
        throw new ArgumentNullException(nameof (dominantKvps));
      if (recessiveKvps == null)
        throw new ArgumentNullException(nameof (recessiveKvps));
      Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(comparer);
      foreach (KeyValuePair<TKey, TValue> recessiveKvp in (IEnumerable<KeyValuePair<TKey, TValue>>) recessiveKvps)
        dictionary[recessiveKvp.Key] = recessiveKvp.Value;
      foreach (KeyValuePair<TKey, TValue> dominantKvp in (IEnumerable<KeyValuePair<TKey, TValue>>) dominantKvps)
        dictionary[dominantKvp.Key] = dominantKvp.Value;
      return (IDictionary<TKey, TValue>) dictionary;
    }
  }
}
