// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ReadOnlyDictionary`2
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Client
{
  public class ReadOnlyDictionary<TKey, TValue> : 
    IDictionary<TKey, TValue>,
    ICollection<KeyValuePair<TKey, TValue>>,
    IEnumerable<KeyValuePair<TKey, TValue>>,
    IEnumerable,
    IDictionary,
    ICollection
  {
    private IDictionary<TKey, TValue> m_dictionary;

    public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary) => this.m_dictionary = dictionary;

    public bool ContainsKey(TKey key) => this.m_dictionary.ContainsKey(key);

    public ICollection<TKey> Keys => this.m_dictionary.Keys;

    public bool TryGetValue(TKey key, out TValue value) => this.m_dictionary.TryGetValue(key, out value);

    public ICollection<TValue> Values => this.m_dictionary.Values;

    public TValue this[TKey key] => this.m_dictionary[key];

    public int Count => this.m_dictionary.Count;

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => this.m_dictionary.GetEnumerator();

    void IDictionary<TKey, TValue>.Add(TKey key, TValue value) => throw new NotImplementedException();

    bool IDictionary<TKey, TValue>.Remove(TKey key) => throw new NotImplementedException();

    TValue IDictionary<TKey, TValue>.this[TKey key]
    {
      get => this.m_dictionary[key];
      set => throw new NotImplementedException();
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => throw new NotImplementedException();

    void ICollection<KeyValuePair<TKey, TValue>>.Clear() => throw new NotImplementedException();

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => this.m_dictionary.Contains(item);

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(
      KeyValuePair<TKey, TValue>[] array,
      int arrayIndex)
    {
      this.m_dictionary.CopyTo(array, arrayIndex);
    }

    int ICollection<KeyValuePair<TKey, TValue>>.Count => this.m_dictionary.Count;

    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => true;

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => throw new NotImplementedException();

    IEnumerator IEnumerable.GetEnumerator() => this.m_dictionary.GetEnumerator();

    void IDictionary.Add(object key, object value) => throw new NotImplementedException();

    void IDictionary.Clear() => throw new NotImplementedException();

    bool IDictionary.Contains(object key) => ((IDictionary) this.m_dictionary).Contains(key);

    IDictionaryEnumerator IDictionary.GetEnumerator() => ((IDictionary) this.m_dictionary).GetEnumerator();

    bool IDictionary.IsFixedSize => true;

    bool IDictionary.IsReadOnly => true;

    ICollection IDictionary.Keys => ((IDictionary) this.m_dictionary).Keys;

    void IDictionary.Remove(object key) => throw new NotImplementedException();

    ICollection IDictionary.Values => ((IDictionary) this.m_dictionary).Values;

    object IDictionary.this[object key]
    {
      get => ((IDictionary) this.m_dictionary)[key];
      set => throw new NotImplementedException();
    }

    void ICollection.CopyTo(Array array, int index) => ((ICollection) this.m_dictionary).CopyTo(array, index);

    int ICollection.Count => this.m_dictionary.Count;

    bool ICollection.IsSynchronized => ((ICollection) this.m_dictionary).IsSynchronized;

    object ICollection.SyncRoot => ((ICollection) this.m_dictionary).SyncRoot;
  }
}
