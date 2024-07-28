// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ShardedDictionary`2
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class ShardedDictionary<TKey, TValue> : 
    IDictionary<TKey, TValue>,
    ICollection<KeyValuePair<TKey, TValue>>,
    IEnumerable<KeyValuePair<TKey, TValue>>,
    IEnumerable
  {
    private readonly Dictionary<TKey, TValue>[] m_shards;
    private ShardedDictionary<TKey, TValue>.KeyCollection m_keyCollection;
    private ShardedDictionary<TKey, TValue>.ValueCollection m_valueCollection;
    private const int c_defaultShards = 4;

    public TValue this[TKey key]
    {
      get => this.GetBucket(key)[key];
      set => this.GetBucket(key)[key] = value;
    }

    public ShardedDictionary<TKey, TValue>.KeyCollection Keys
    {
      get
      {
        if (this.m_keyCollection == null)
          this.m_keyCollection = new ShardedDictionary<TKey, TValue>.KeyCollection(this);
        return this.m_keyCollection;
      }
    }

    ICollection<TKey> IDictionary<TKey, TValue>.Keys => (ICollection<TKey>) this.Keys;

    public ShardedDictionary<TKey, TValue>.ValueCollection Values
    {
      get
      {
        if (this.m_valueCollection == null)
          this.m_valueCollection = new ShardedDictionary<TKey, TValue>.ValueCollection(this);
        return this.m_valueCollection;
      }
    }

    ICollection<TValue> IDictionary<TKey, TValue>.Values => (ICollection<TValue>) this.Values;

    public int Count
    {
      get
      {
        int count = 0;
        foreach (Dictionary<TKey, TValue> shard in this.m_shards)
          count += shard.Count;
        return count;
      }
    }

    public bool IsReadOnly => false;

    public int ShardCount => this.m_shards.Length;

    public ShardedDictionary()
      : this(4)
    {
    }

    public ShardedDictionary(int shardCount)
    {
      ArgumentUtility.CheckForOutOfRange(shardCount, nameof (shardCount), 2, 20);
      if (shardCount % 2 == 1)
        ++shardCount;
      this.m_shards = new Dictionary<TKey, TValue>[shardCount];
      for (int index = 0; index < shardCount; ++index)
        this.m_shards[index] = new Dictionary<TKey, TValue>();
    }

    public void Add(TKey key, TValue value) => this.GetBucket(key).Add(key, value);

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>) this.GetBucket(item.Key)).Add(item);

    public void Clear()
    {
      foreach (Dictionary<TKey, TValue> shard in this.m_shards)
        shard.Clear();
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>) this.GetBucket(item.Key)).Contains(item);

    public bool ContainsKey(TKey key) => this.GetBucket(key).ContainsKey(key);

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(
      KeyValuePair<TKey, TValue>[] array,
      int arrayIndex)
    {
      for (int index = 0; index < this.m_shards.Length; ++index)
      {
        int count = this.m_shards[index].Count;
        if (count > 0)
        {
          ((ICollection<KeyValuePair<TKey, TValue>>) this.m_shards[index]).CopyTo(array, arrayIndex);
          arrayIndex += count;
        }
      }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => this.BuildEnumerable().GetEnumerator();

    public bool Remove(TKey key) => this.GetBucket(key).Remove(key);

    public bool Remove(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>) this.GetBucket(item.Key)).Remove(item);

    public bool TryGetValue(TKey key, out TValue value) => this.GetBucket(key).TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.BuildEnumerable().GetEnumerator();

    private Dictionary<TKey, TValue> GetBucket(TKey key) => this.m_shards[(int) ((long) (uint) key.GetHashCode() % (long) this.m_shards.Length)];

    private IEnumerable<KeyValuePair<TKey, TValue>> BuildEnumerable()
    {
      for (int i = 0; i < this.m_shards.Length; ++i)
      {
        foreach (KeyValuePair<TKey, TValue> keyValuePair in this.m_shards[i])
          yield return keyValuePair;
      }
    }

    public class KeyCollection : ICollection<TKey>, IEnumerable<TKey>, IEnumerable
    {
      private const string c_errorMessage = "Cannot modify a key collection";
      private readonly ShardedDictionary<TKey, TValue> m_dictionary;

      public KeyCollection(ShardedDictionary<TKey, TValue> dictionary) => this.m_dictionary = dictionary;

      public int Count => this.m_dictionary.Count;

      bool ICollection<TKey>.IsReadOnly => true;

      void ICollection<TKey>.Add(TKey key) => throw new NotSupportedException("Cannot modify a key collection");

      void ICollection<TKey>.Clear() => throw new NotSupportedException("Cannot modify a key collection");

      bool ICollection<TKey>.Contains(TKey key)
      {
        foreach (Dictionary<TKey, TValue> shard in this.m_dictionary.m_shards)
        {
          if (shard.ContainsKey(key))
            return true;
        }
        return false;
      }

      public void CopyTo(TKey[] array, int arrayIndex)
      {
        for (int index = 0; index < this.m_dictionary.m_shards.Length; ++index)
        {
          int count = this.m_dictionary.m_shards[index].Count;
          if (count > 0)
          {
            this.m_dictionary.m_shards[index].Keys.CopyTo(array, arrayIndex);
            arrayIndex += count;
          }
        }
      }

      public IEnumerator<TKey> GetEnumerator() => this.BuildEnumerable().GetEnumerator();

      bool ICollection<TKey>.Remove(TKey key) => throw new NotSupportedException("Cannot modify a key collection");

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.BuildEnumerable().GetEnumerator();

      private IEnumerable<TKey> BuildEnumerable()
      {
        foreach (KeyValuePair<TKey, TValue> keyValuePair in this.m_dictionary)
          yield return keyValuePair.Key;
      }
    }

    public class ValueCollection : ICollection<TValue>, IEnumerable<TValue>, IEnumerable
    {
      private const string c_errorMessage = "Cannot modify a value collection";
      private readonly ShardedDictionary<TKey, TValue> m_dictionary;

      public ValueCollection(ShardedDictionary<TKey, TValue> dictionary) => this.m_dictionary = dictionary;

      public int Count => this.m_dictionary.Count;

      bool ICollection<TValue>.IsReadOnly => true;

      void ICollection<TValue>.Add(TValue value) => throw new NotSupportedException("Cannot modify a value collection");

      void ICollection<TValue>.Clear() => throw new NotSupportedException("Cannot modify a value collection");

      bool ICollection<TValue>.Contains(TValue value)
      {
        foreach (Dictionary<TKey, TValue> shard in this.m_dictionary.m_shards)
        {
          if (shard.ContainsValue(value))
            return true;
        }
        return false;
      }

      public void CopyTo(TValue[] array, int arrayIndex)
      {
        for (int index = 0; index < this.m_dictionary.m_shards.Length; ++index)
        {
          int count = this.m_dictionary.m_shards[index].Count;
          if (count > 0)
          {
            this.m_dictionary.m_shards[index].Values.CopyTo(array, arrayIndex);
            arrayIndex += count;
          }
        }
      }

      public IEnumerator<TValue> GetEnumerator() => this.BuildEnumerable().GetEnumerator();

      bool ICollection<TValue>.Remove(TValue value) => throw new NotSupportedException("Cannot modify a value collection");

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.BuildEnumerable().GetEnumerator();

      private IEnumerable<TValue> BuildEnumerable()
      {
        foreach (KeyValuePair<TKey, TValue> keyValuePair in this.m_dictionary)
          yield return keyValuePair.Value;
      }
    }
  }
}
