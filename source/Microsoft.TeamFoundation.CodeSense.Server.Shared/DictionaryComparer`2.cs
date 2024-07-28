// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.DictionaryComparer`2
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  internal class DictionaryComparer<TKey, TValue> : IEqualityComparer<Dictionary<TKey, TValue>>
  {
    private static readonly DictionaryComparer<TKey, TValue> DefaultComparer = new DictionaryComparer<TKey, TValue>((IEqualityComparer<TValue>) EqualityComparer<TValue>.Default);
    private readonly IEqualityComparer<TValue> valueComparer;

    public DictionaryComparer(IEqualityComparer<TValue> valueComparer) => this.valueComparer = valueComparer;

    public static DictionaryComparer<TKey, TValue> Default => DictionaryComparer<TKey, TValue>.DefaultComparer;

    public bool Equals(Dictionary<TKey, TValue> x, Dictionary<TKey, TValue> y)
    {
      if (x == null || y == null)
        return x == y;
      TValue value;
      return x.Count == y.Count && x.All<KeyValuePair<TKey, TValue>>((Func<KeyValuePair<TKey, TValue>, bool>) (keyValue => y.TryGetValue(keyValue.Key, out value) && this.valueComparer.Equals(value, keyValue.Value)));
    }

    public int GetHashCode(Dictionary<TKey, TValue> obj) => obj.Aggregate<KeyValuePair<TKey, TValue>, int>(0, (Func<int, KeyValuePair<TKey, TValue>, int>) ((hash, keyValue) => hash ^ obj.Comparer.GetHashCode(keyValue.Key) ^ this.valueComparer.GetHashCode(keyValue.Value)));
  }
}
