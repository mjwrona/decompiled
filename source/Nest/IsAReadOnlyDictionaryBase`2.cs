// Decompiled with JetBrains decompiler
// Type: Nest.IsAReadOnlyDictionaryBase`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  public abstract class IsAReadOnlyDictionaryBase<TKey, TValue> : 
    IIsAReadOnlyDictionary<TKey, TValue>,
    IReadOnlyDictionary<TKey, TValue>,
    IReadOnlyCollection<KeyValuePair<TKey, TValue>>,
    IEnumerable<KeyValuePair<TKey, TValue>>,
    IEnumerable,
    IIsAReadOnlyDictionary
  {
    protected IsAReadOnlyDictionaryBase(
      IReadOnlyDictionary<TKey, TValue> backingDictionary)
    {
      if (backingDictionary == null)
        return;
      Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>(backingDictionary.Count);
      foreach (TKey key in backingDictionary.Keys)
        dictionary[this.Sanitize(key)] = backingDictionary[key];
      this.BackingDictionary = (IReadOnlyDictionary<TKey, TValue>) dictionary;
    }

    public int Count => this.BackingDictionary.Count;

    public TValue this[TKey key] => this.BackingDictionary[key];

    public IEnumerable<TKey> Keys => this.BackingDictionary.Keys;

    public IEnumerable<TValue> Values => this.BackingDictionary.Values;

    protected internal IReadOnlyDictionary<TKey, TValue> BackingDictionary { get; } = EmptyReadOnly<TKey, TValue>.Dictionary;

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.BackingDictionary.GetEnumerator();

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => this.BackingDictionary.GetEnumerator();

    public bool ContainsKey(TKey key) => this.BackingDictionary.ContainsKey(key);

    public bool TryGetValue(TKey key, out TValue value) => this.BackingDictionary.TryGetValue(key, out value);

    protected virtual TKey Sanitize(TKey key) => key;
  }
}
