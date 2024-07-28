// Decompiled with JetBrains decompiler
// Type: Nest.ResolvableDictionaryProxy`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections;
using System.Collections.Generic;

namespace Nest
{
  public class ResolvableDictionaryProxy<TKey, TValue> : 
    IIsAReadOnlyDictionary<TKey, TValue>,
    IReadOnlyDictionary<TKey, TValue>,
    IReadOnlyCollection<KeyValuePair<TKey, TValue>>,
    IEnumerable<KeyValuePair<TKey, TValue>>,
    IEnumerable,
    IIsAReadOnlyDictionary
    where TKey : IUrlParameter
  {
    private readonly IConnectionConfigurationValues _connectionSettings;

    internal ResolvableDictionaryProxy(
      IConnectionConfigurationValues connectionSettings,
      IReadOnlyDictionary<TKey, TValue> backingDictionary)
    {
      this._connectionSettings = connectionSettings;
      if (backingDictionary == null)
        return;
      this.Original = backingDictionary;
      Dictionary<string, TValue> dictionary = new Dictionary<string, TValue>(backingDictionary.Count);
      foreach (TKey key in backingDictionary.Keys)
        dictionary[this.Sanitize(key)] = backingDictionary[key];
      this.BackingDictionary = (IReadOnlyDictionary<string, TValue>) dictionary;
    }

    public int Count => this.BackingDictionary.Count;

    public TValue this[TKey key]
    {
      get
      {
        TValue obj;
        return !this.BackingDictionary.TryGetValue(this.Sanitize(key), out obj) ? default (TValue) : obj;
      }
    }

    public TValue this[string key]
    {
      get
      {
        TValue obj;
        return !this.BackingDictionary.TryGetValue(key, out obj) ? default (TValue) : obj;
      }
    }

    public IEnumerable<TKey> Keys => this.Original.Keys;

    public IEnumerable<string> ResolvedKeys => this.BackingDictionary.Keys;

    public IEnumerable<TValue> Values => this.BackingDictionary.Values;

    protected internal IReadOnlyDictionary<string, TValue> BackingDictionary { get; } = EmptyReadOnly<string, TValue>.Dictionary;

    private IReadOnlyDictionary<TKey, TValue> Original { get; } = EmptyReadOnly<TKey, TValue>.Dictionary;

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.Original.GetEnumerator();

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => this.Original.GetEnumerator();

    public bool ContainsKey(TKey key) => this.BackingDictionary.ContainsKey(this.Sanitize(key));

    public bool TryGetValue(TKey key, out TValue value) => this.BackingDictionary.TryGetValue(this.Sanitize(key), out value);

    private string Sanitize(TKey key) => key?.GetString(this._connectionSettings);
  }
}
