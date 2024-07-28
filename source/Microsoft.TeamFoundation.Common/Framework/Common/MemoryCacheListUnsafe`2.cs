// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.MemoryCacheListUnsafe`2
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class MemoryCacheListUnsafe<TKey, TValue> : IMemoryCacheList<TKey, TValue>
  {
    private readonly Dictionary<TKey, IMemoryCacheEntry<TKey, TValue>> m_dictionary;
    private readonly IMemoryCacheCapacityPolicy<TKey, TValue> m_capacityPolicy;
    private readonly IMemoryCacheSubscriber<TKey, TValue> m_subscriber;
    private readonly IMemoryCacheEvictionPolicy<TKey, TValue> m_evictionPolicy;

    public MemoryCacheListUnsafe(
      IMemoryCacheCapacityPolicy<TKey, TValue> capacityPolicy,
      IMemoryCacheEvictionPolicy<TKey, TValue> evictionPolicy,
      IMemoryCacheSubscriber<TKey, TValue> subscriber)
      : this((IEqualityComparer<TKey>) null, capacityPolicy, evictionPolicy, subscriber)
    {
    }

    public MemoryCacheListUnsafe(
      IEqualityComparer<TKey> comparer,
      IMemoryCacheCapacityPolicy<TKey, TValue> capacityPolicy,
      IMemoryCacheEvictionPolicy<TKey, TValue> evictionPolicy,
      IMemoryCacheSubscriber<TKey, TValue> subscriber)
    {
      ArgumentUtility.CheckForNull<IMemoryCacheCapacityPolicy<TKey, TValue>>(capacityPolicy, nameof (capacityPolicy));
      ArgumentUtility.CheckForNull<IMemoryCacheEvictionPolicy<TKey, TValue>>(evictionPolicy, nameof (evictionPolicy));
      ArgumentUtility.CheckForNull<IMemoryCacheSubscriber<TKey, TValue>>(subscriber, nameof (subscriber));
      this.m_capacityPolicy = capacityPolicy;
      this.m_evictionPolicy = evictionPolicy;
      this.m_subscriber = subscriber;
      this.m_dictionary = new Dictionary<TKey, IMemoryCacheEntry<TKey, TValue>>(comparer);
    }

    private void NotifyEntryLookupSucceeded(IMemoryCacheEntry<TKey, TValue> entry) => this.m_subscriber.OnEntryLookupSucceeded(entry.Key, entry.Value);

    private void NotifyEntryLookupFailed(TKey key) => this.m_subscriber.OnEntryLookupFailed(key);

    private void NotifyEntryAdded(IMemoryCacheEntry<TKey, TValue> entry)
    {
      long sizeDiff = this.m_capacityPolicy.OnEntryAdded(entry.Key, entry.Value);
      this.m_subscriber.OnEntryAdded(entry.Key, entry.Value, new MemoryCacheOperationStatistics(1, sizeDiff));
    }

    private void NotifyEntryReplaced(
      IMemoryCacheEntry<TKey, TValue> entry,
      TValue previousValue,
      bool valueValid)
    {
      SizePair sizePair = this.m_capacityPolicy.OnEntryReplaced(entry.Key, previousValue, entry.Value);
      if (valueValid)
      {
        this.m_subscriber.OnEntryReplaced(entry.Key, previousValue, entry.Value, new MemoryCacheOperationStatistics(0, sizePair.NewItemSize - sizePair.PreviousItemSize));
      }
      else
      {
        this.m_subscriber.OnEntryInvalidated(entry.Key, previousValue, new MemoryCacheOperationStatistics(-1, -sizePair.PreviousItemSize));
        this.m_subscriber.OnEntryAdded(entry.Key, entry.Value, new MemoryCacheOperationStatistics(1, sizePair.NewItemSize));
      }
    }

    private void NotifyEntryEvicted(IMemoryCacheEntry<TKey, TValue> entry)
    {
      long num = this.m_capacityPolicy.OnEntryRemoved(entry.Key, entry.Value);
      this.m_subscriber.OnEntryEvicted(entry.Key, entry.Value, new MemoryCacheOperationStatistics(-1, -num));
    }

    private void NotifyEntryInvalidated(IMemoryCacheEntry<TKey, TValue> entry)
    {
      long num = this.m_capacityPolicy.OnEntryRemoved(entry.Key, entry.Value);
      this.m_subscriber.OnEntryInvalidated(entry.Key, entry.Value, new MemoryCacheOperationStatistics(-1, -num));
    }

    private void NotifyEntryRemoved(IMemoryCacheEntry<TKey, TValue> entry)
    {
      long num = this.m_capacityPolicy.OnEntryRemoved(entry.Key, entry.Value);
      this.m_subscriber.OnEntryRemoved(entry.Key, entry.Value, new MemoryCacheOperationStatistics(-1, -num));
    }

    private void NotifyCleared()
    {
      int length = this.m_capacityPolicy.Length;
      long size = this.m_capacityPolicy.Size;
      this.m_capacityPolicy.OnCleared();
      this.m_subscriber.OnCleared(new MemoryCacheOperationStatistics(-1 * length, -1L * size));
    }

    public void Clear()
    {
      this.m_dictionary.Clear();
      this.m_evictionPolicy.UnlinkAll();
      this.NotifyCleared();
    }

    public int Sweep()
    {
      int num = 0;
      foreach (IMemoryCacheEntry<TKey, TValue> invalidationCandidate in this.m_evictionPolicy.GetInvalidationCandidates())
      {
        this.m_dictionary.Remove(invalidationCandidate.Key);
        this.m_evictionPolicy.Unlink(invalidationCandidate);
        this.NotifyEntryInvalidated(invalidationCandidate);
        ++num;
      }
      return num;
    }

    public bool Add(
      TKey key,
      TValue value,
      bool overwrite,
      IMemoryCacheValidityPolicy<TKey, TValue> validityPolicy)
    {
      IMemoryCacheEntry<TKey, TValue> extantEntry = (IMemoryCacheEntry<TKey, TValue>) null;
      TValue previousValue = default (TValue);
      bool flag = false;
      bool valueValid = true;
      if (this.m_dictionary.TryGetValue(key, out extantEntry))
      {
        if (!overwrite && (valueValid = this.IsValid(extantEntry)))
          return false;
        flag = true;
        previousValue = extantEntry.Value;
      }
      if (!this.EnsureSpace(key, value, ref extantEntry))
      {
        this.m_subscriber.OnEntryEvicted(key, value, new MemoryCacheOperationStatistics(0, 0L));
        return true;
      }
      IMemoryCacheEntry<TKey, TValue> entry;
      if (extantEntry != null)
      {
        extantEntry.Value = value;
        extantEntry.ValidityPolicy = validityPolicy;
        entry = extantEntry;
      }
      else
      {
        entry = this.m_evictionPolicy.Link(key, value, validityPolicy);
        this.m_dictionary.Add(key, entry);
      }
      if (flag)
        this.NotifyEntryReplaced(entry, previousValue, valueValid);
      else
        this.NotifyEntryAdded(entry);
      return true;
    }

    public bool Remove(TKey key)
    {
      IMemoryCacheEntry<TKey, TValue> entry;
      if (!this.m_dictionary.TryGetValue(key, out entry))
        return false;
      this.m_dictionary.Remove(entry.Key);
      this.m_evictionPolicy.Unlink(entry);
      this.NotifyEntryRemoved(entry);
      return true;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      IMemoryCacheEntry<TKey, TValue> entry;
      if (this.m_dictionary.TryGetValue(key, out entry) && this.IsValid(entry))
      {
        entry.OnCacheHit();
        value = entry.Value;
        this.NotifyEntryLookupSucceeded(entry);
        return true;
      }
      value = default (TValue);
      this.NotifyEntryLookupFailed(key);
      return false;
    }

    public int Count => this.m_dictionary.Count;

    private bool EnsureSpace(
      TKey key,
      TValue value,
      ref IMemoryCacheEntry<TKey, TValue> extantEntry)
    {
      bool flag = true;
      if (extantEntry != null)
      {
        while ((flag = this.m_capacityPolicy.NeedRoom(key, extantEntry.Value, value)) && this.Count > 0)
        {
          if (this.Evict() == extantEntry)
          {
            extantEntry = (IMemoryCacheEntry<TKey, TValue>) null;
            break;
          }
        }
      }
      if (flag)
      {
        while ((flag = this.m_capacityPolicy.NeedRoom(key, value)) && this.Count > 0)
          this.Evict();
      }
      return !flag;
    }

    private IMemoryCacheEntry<TKey, TValue> Evict()
    {
      IMemoryCacheEntry<TKey, TValue> evictionCandidate = this.m_evictionPolicy.GetEvictionCandidate();
      this.m_dictionary.Remove(evictionCandidate.Key);
      this.m_evictionPolicy.Unlink(evictionCandidate);
      this.NotifyEntryEvicted(evictionCandidate);
      return evictionCandidate;
    }

    private bool IsValid(IMemoryCacheEntry<TKey, TValue> entry) => entry.ValidityPolicy == null || entry.ValidityPolicy.IsValid(entry.Key, entry.Value);
  }
}
