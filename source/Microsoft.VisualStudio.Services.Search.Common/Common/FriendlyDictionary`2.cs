// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FriendlyDictionary`2
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class FriendlyDictionary<TKey, TValue> : 
    IDictionary<TKey, TValue>,
    ICollection<KeyValuePair<TKey, TValue>>,
    IEnumerable<KeyValuePair<TKey, TValue>>,
    IEnumerable
  {
    private Dictionary<TKey, TValue> m_dict;

    public FriendlyDictionary() => this.m_dict = new Dictionary<TKey, TValue>();

    public FriendlyDictionary(int capacity) => this.m_dict = new Dictionary<TKey, TValue>(capacity);

    public FriendlyDictionary(IEqualityComparer<TKey> comparer) => this.m_dict = new Dictionary<TKey, TValue>(comparer);

    public FriendlyDictionary(IDictionary<TKey, TValue> dictionary) => this.m_dict = new Dictionary<TKey, TValue>(dictionary);

    public FriendlyDictionary(int capacity, IEqualityComparer<TKey> comparer) => this.m_dict = new Dictionary<TKey, TValue>(capacity, comparer);

    public FriendlyDictionary(
      IDictionary<TKey, TValue> dictionary,
      IEqualityComparer<TKey> comparer)
    {
      this.m_dict = new Dictionary<TKey, TValue>(dictionary, comparer);
    }

    public TValue this[TKey key]
    {
      get
      {
        try
        {
          return this.m_dict[key];
        }
        catch (KeyNotFoundException ex)
        {
          throw new KeyNotFoundException(FormattableString.Invariant(FormattableStringFactory.Create("The given key [{0}] was not present in the dictionary.", (object) key)));
        }
      }
      set => this.m_dict[key] = value;
    }

    public int Count => this.m_dict.Count;

    public bool IsReadOnly => false;

    public ICollection<TKey> Keys => (ICollection<TKey>) this.m_dict.Keys;

    public ICollection<TValue> Values => (ICollection<TValue>) this.m_dict.Values;

    public void Add(KeyValuePair<TKey, TValue> item) => this.Add(item.Key, item.Value);

    public void Add(TKey key, TValue value)
    {
      try
      {
        this.m_dict.Add(key, value);
      }
      catch (ArgumentException ex) when (ex.Message == "An item with the same key has already been added.")
      {
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Failed to add item ([{0}] => [{1}]) because an item ([{2}] => [{3}]) with the same key has already been added.", (object) key, (object) value, (object) key, (object) this.m_dict[key])));
      }
    }

    public void Clear() => this.m_dict.Clear();

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
      TValue obj1;
      if (!this.m_dict.TryGetValue(item.Key, out obj1))
        return false;
      ref TValue local1 = ref obj1;
      if ((object) default (TValue) == null)
      {
        TValue obj2 = local1;
        local1 = ref obj2;
      }
      // ISSUE: variable of a boxed type
      __Boxed<TValue> local2 = (object) item.Value;
      return local1.Equals((object) local2);
    }

    public bool ContainsKey(TKey key) => this.m_dict.ContainsKey(key);

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => throw new NotImplementedException();

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => (IEnumerator<KeyValuePair<TKey, TValue>>) this.m_dict.GetEnumerator();

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
      if (!this.m_dict.TryGetValue(item.Key, out TValue _))
        return false;
      this.m_dict.Remove(item.Key);
      return true;
    }

    public bool Remove(TKey key) => this.m_dict.Remove(key);

    public bool TryGetValue(TKey key, out TValue value) => this.m_dict.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.m_dict.GetEnumerator();
  }
}
