// Decompiled with JetBrains decompiler
// Type: Nest.IsADictionaryBase`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Nest
{
  public abstract class IsADictionaryBase<TKey, TValue> : 
    IIsADictionary<TKey, TValue>,
    IDictionary<TKey, TValue>,
    ICollection<KeyValuePair<TKey, TValue>>,
    IEnumerable<KeyValuePair<TKey, TValue>>,
    IEnumerable,
    IIsADictionary
  {
    protected IsADictionaryBase() => this.BackingDictionary = new Dictionary<TKey, TValue>();

    protected IsADictionaryBase(IDictionary<TKey, TValue> backingDictionary)
    {
      if (backingDictionary != null)
      {
        foreach (TKey key in (IEnumerable<TKey>) backingDictionary.Keys)
          this.ValidateKey(this.Sanitize(key));
      }
      this.BackingDictionary = backingDictionary != null ? new Dictionary<TKey, TValue>(backingDictionary) : new Dictionary<TKey, TValue>();
    }

    public TValue this[TKey key]
    {
      get => this.BackingDictionary[this.Sanitize(key)];
      set => this.BackingDictionary[this.ValidateKey(this.Sanitize(key))] = value;
    }

    protected Dictionary<TKey, TValue> BackingDictionary { get; }

    int ICollection<KeyValuePair<TKey, TValue>>.Count => this.BackingDictionary.Count;

    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => this.Self.IsReadOnly;

    TValue IDictionary<TKey, TValue>.this[TKey key]
    {
      get => this.BackingDictionary[this.Sanitize(key)];
      set => this.BackingDictionary[this.ValidateKey(this.Sanitize(key))] = value;
    }

    ICollection<TKey> IDictionary<TKey, TValue>.Keys => (ICollection<TKey>) this.BackingDictionary.Keys;

    private ICollection<KeyValuePair<TKey, TValue>> Self => (ICollection<KeyValuePair<TKey, TValue>>) this.BackingDictionary;

    ICollection<TValue> IDictionary<TKey, TValue>.Values => (ICollection<TValue>) this.BackingDictionary.Values;

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
      this.ValidateKey(this.Sanitize(item.Key));
      this.Self.Add(item);
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Clear() => this.BackingDictionary.Clear();

    [EditorBrowsable(EditorBrowsableState.Never)]
    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => this.Self.Contains(item);

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(
      KeyValuePair<TKey, TValue>[] array,
      int arrayIndex)
    {
      this.Self.CopyTo(array, arrayIndex);
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => this.Self.Remove(item);

    void IDictionary<TKey, TValue>.Add(TKey key, TValue value) => this.BackingDictionary.Add(this.ValidateKey(this.Sanitize(key)), value);

    [EditorBrowsable(EditorBrowsableState.Never)]
    bool IDictionary<TKey, TValue>.ContainsKey(TKey key) => this.BackingDictionary.ContainsKey(this.Sanitize(key));

    bool IDictionary<TKey, TValue>.Remove(TKey key) => this.BackingDictionary.Remove(this.Sanitize(key));

    bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value) => this.BackingDictionary.TryGetValue(this.Sanitize(key), out value);

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.BackingDictionary.GetEnumerator();

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => (IEnumerator<KeyValuePair<TKey, TValue>>) this.BackingDictionary.GetEnumerator();

    protected virtual TKey ValidateKey(TKey key) => key;

    protected virtual TKey Sanitize(TKey key) => key;
  }
}
