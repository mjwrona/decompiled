// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Utilities.Internal.DictionaryExtensions
// Assembly: Microsoft.VisualStudio.Utilities.Internal, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E02F1555-E063-4795-BC05-853CA7424F51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Utilities.Internal.dll

using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Utilities.Internal
{
  public static class DictionaryExtensions
  {
    public static TV GetOrDefault<TK, TV>(this IDictionary<TK, TV> dictionary, TK key)
    {
      TV orDefault;
      dictionary.TryGetValue(key, out orDefault);
      return orDefault;
    }

    public static void AddRange<TKey, TValue>(
      this IDictionary<TKey, TValue> target,
      IDictionary<TKey, TValue> source,
      bool forceUpdate = true)
    {
      source.RequiresArgumentNotNull<IDictionary<TKey, TValue>>(nameof (source));
      foreach (KeyValuePair<TKey, TValue> keyValuePair in (IEnumerable<KeyValuePair<TKey, TValue>>) source)
      {
        if (forceUpdate || !target.ContainsKey(keyValuePair.Key))
          target[keyValuePair.Key] = keyValuePair.Value;
      }
    }

    public static void Remove<TK, TV>(this ConcurrentDictionary<TK, TV> dictionary, TK key) => ((IDictionary<TK, TV>) dictionary).Remove(key);
  }
}
