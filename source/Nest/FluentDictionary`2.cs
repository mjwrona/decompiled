// Decompiled with JetBrains decompiler
// Type: Nest.FluentDictionary`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class FluentDictionary<TKey, TValue> : Dictionary<TKey, TValue>
  {
    public FluentDictionary()
    {
    }

    public FluentDictionary(IDictionary<TKey, TValue> copy)
    {
      if (copy == null)
        return;
      foreach (KeyValuePair<TKey, TValue> keyValuePair in (IEnumerable<KeyValuePair<TKey, TValue>>) copy)
        this[keyValuePair.Key] = keyValuePair.Value;
    }

    public FluentDictionary<TKey, TValue> Add(TKey key, TValue value)
    {
      base.Add(key, value);
      return this;
    }

    public FluentDictionary<TKey, TValue> Remove(TKey key)
    {
      base.Remove(key);
      return this;
    }
  }
}
