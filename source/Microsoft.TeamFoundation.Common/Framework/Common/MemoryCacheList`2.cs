// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.MemoryCacheList`2
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class MemoryCacheList<TKey, TValue> : IMemoryCacheList<TKey, TValue>
  {
    private readonly IMemoryCacheList<TKey, TValue> m_cache;

    public MemoryCacheList(
      IMemoryCacheCapacityPolicy<TKey, TValue> capacityPolicy,
      IMemoryCacheEvictionPolicy<TKey, TValue> evictionPolicy,
      IMemoryCacheSubscriber<TKey, TValue> subscriber,
      bool synchronized = true)
      : this((IEqualityComparer<TKey>) null, capacityPolicy, evictionPolicy, subscriber, synchronized)
    {
    }

    public MemoryCacheList(
      IEqualityComparer<TKey> comparer,
      IMemoryCacheCapacityPolicy<TKey, TValue> capacityPolicy,
      IMemoryCacheEvictionPolicy<TKey, TValue> evictionPolicy,
      IMemoryCacheSubscriber<TKey, TValue> subscriber,
      bool synchronized = true)
    {
      IMemoryCacheSubscriber<TKey, TValue> subscriber1 = MemoryCacheMultiSubscriber.Create<TKey, TValue>(subscriber, (IMemoryCacheSubscriber<TKey, TValue>) new MemoryCacheList<TKey, TValue>.CacheSubscriber(this));
      if (synchronized)
        this.m_cache = (IMemoryCacheList<TKey, TValue>) new MemoryCacheListSynchronized<TKey, TValue>(comparer, capacityPolicy, evictionPolicy, subscriber1);
      else
        this.m_cache = (IMemoryCacheList<TKey, TValue>) new MemoryCacheListUnsafe<TKey, TValue>(comparer, capacityPolicy, evictionPolicy, subscriber1);
    }

    public event EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheReplacementEventArgs> EntryReplaced;

    public event EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs> EntryEvicted;

    public event EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs> EntryInvalidated;

    public event EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs> EntryRemoved;

    public event EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs> EntryAdded;

    public event EventHandler Cleared;

    public int Count => this.m_cache.Count;

    public bool Add(
      TKey key,
      TValue value,
      bool overwrite,
      IMemoryCacheValidityPolicy<TKey, TValue> validityPolicy)
    {
      return this.m_cache.Add(key, value, overwrite, validityPolicy);
    }

    public bool TryGetValue(TKey key, out TValue value) => this.m_cache.TryGetValue(key, out value);

    public bool Remove(TKey key) => this.m_cache.Remove(key);

    public void Clear() => this.m_cache.Clear();

    public int Sweep() => this.m_cache.Sweep();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class MemoryCacheEventArgs : EventArgs
    {
      public MemoryCacheEventArgs(TKey key, TValue value)
      {
        this.Key = key;
        this.Value = value;
      }

      public TKey Key { get; private set; }

      public TValue Value { get; private set; }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public class MemoryCacheReplacementEventArgs : EventArgs
    {
      public MemoryCacheReplacementEventArgs(TKey key, TValue oldValue, TValue newValue)
      {
        this.Key = key;
        this.OldValue = oldValue;
        this.NewValue = newValue;
      }

      public TKey Key { get; private set; }

      public TValue OldValue { get; private set; }

      public TValue NewValue { get; private set; }
    }

    private class CacheSubscriber : IMemoryCacheSubscriber<TKey, TValue>
    {
      private readonly MemoryCacheList<TKey, TValue> m_owner;

      public CacheSubscriber(MemoryCacheList<TKey, TValue> owner) => this.m_owner = owner;

      public void OnEntryLookupSucceeded(TKey key, TValue value)
      {
      }

      public void OnEntryLookupFailed(TKey key)
      {
      }

      public void OnEntryReplaced(
        TKey key,
        TValue previousValue,
        TValue newValue,
        MemoryCacheOperationStatistics stats)
      {
        EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheReplacementEventArgs> entryReplaced = this.m_owner.EntryReplaced;
        if (entryReplaced == null)
          return;
        entryReplaced((object) this.m_owner, new MemoryCacheList<TKey, TValue>.MemoryCacheReplacementEventArgs(key, previousValue, newValue));
      }

      public void OnEntryEvicted(TKey key, TValue value, MemoryCacheOperationStatistics stats)
      {
        EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs> entryEvicted = this.m_owner.EntryEvicted;
        if (entryEvicted == null)
          return;
        entryEvicted((object) this.m_owner, new MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs(key, value));
      }

      public void OnEntryInvalidated(TKey key, TValue value, MemoryCacheOperationStatistics stats)
      {
        EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs> entryInvalidated = this.m_owner.EntryInvalidated;
        if (entryInvalidated == null)
          return;
        entryInvalidated((object) this.m_owner, new MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs(key, value));
      }

      public void OnEntryRemoved(TKey key, TValue value, MemoryCacheOperationStatistics stats)
      {
        EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs> entryRemoved = this.m_owner.EntryRemoved;
        if (entryRemoved == null)
          return;
        entryRemoved((object) this.m_owner, new MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs(key, value));
      }

      public void OnEntryAdded(TKey key, TValue value, MemoryCacheOperationStatistics stats)
      {
        EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs> entryAdded = this.m_owner.EntryAdded;
        if (entryAdded == null)
          return;
        entryAdded((object) this.m_owner, new MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs(key, value));
      }

      public void OnCleared(MemoryCacheOperationStatistics stats)
      {
        EventHandler cleared = this.m_owner.Cleared;
        if (cleared == null)
          return;
        cleared((object) this.m_owner, (EventArgs) null);
      }
    }
  }
}
