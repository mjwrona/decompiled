// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.ExpirationCache`2
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ExpirationCache<TKey, TValue>
  {
    private int m_lastSweepTickCount;
    private readonly object m_lock = new object();
    private readonly ConcurrentDictionary<TKey, ExpirationCache<TKey, TValue>.CacheEntry> m_cache;
    private readonly Func<TKey, object, TValue> m_valueFactory;
    private readonly uint m_maxAgeInMilliseconds;
    private const uint c_minFalsePositiveDelta = 2147483648;

    public ExpirationCache(
      int expirationTimeInMilliseconds,
      Func<TKey, object, TValue> valueFactory)
      : this(expirationTimeInMilliseconds, valueFactory, (IEqualityComparer<TKey>) null)
    {
    }

    public ExpirationCache(
      int expirationTimeInMilliseconds,
      Func<TKey, object, TValue> valueFactory,
      IEqualityComparer<TKey> comparer)
    {
      ArgumentUtility.CheckForNull<Func<TKey, object, TValue>>(valueFactory, nameof (valueFactory));
      this.m_cache = comparer != null ? new ConcurrentDictionary<TKey, ExpirationCache<TKey, TValue>.CacheEntry>(comparer) : new ConcurrentDictionary<TKey, ExpirationCache<TKey, TValue>.CacheEntry>();
      this.m_valueFactory = valueFactory;
      this.m_maxAgeInMilliseconds = (uint) expirationTimeInMilliseconds;
      this.m_lastSweepTickCount = (int) ExpirationCache<TKey, TValue>.GetTickCount();
    }

    public TValue GetOrAdd(TKey key, object userState)
    {
      uint tickCount = ExpirationCache<TKey, TValue>.GetTickCount();
      ExpirationCache<TKey, TValue>.CacheEntry cacheEntry = this.m_cache.AddOrUpdate(key, (Func<TKey, ExpirationCache<TKey, TValue>.CacheEntry>) (k => new ExpirationCache<TKey, TValue>.CacheEntry(this.m_valueFactory(key, userState), tickCount)), (Func<TKey, ExpirationCache<TKey, TValue>.CacheEntry, ExpirationCache<TKey, TValue>.CacheEntry>) ((k, entryToUpdate) => entryToUpdate.ToTickCount(tickCount)));
      this.MaybeSweep(tickCount);
      return cacheEntry.Item;
    }

    public bool TryRemove(TKey key, out TValue value)
    {
      ExpirationCache<TKey, TValue>.CacheEntry cacheEntry;
      int num = this.m_cache.TryRemove(key, out cacheEntry) ? 1 : 0;
      if (num != 0)
      {
        value = cacheEntry.Item;
        return num != 0;
      }
      value = default (TValue);
      return num != 0;
    }

    public void RefreshExpirationForKey(TKey key)
    {
      ExpirationCache<TKey, TValue>.CacheEntry cacheEntry;
      if (!this.m_cache.TryGetValue(key, out cacheEntry))
        return;
      uint tickCount = ExpirationCache<TKey, TValue>.GetTickCount();
      if ((int) tickCount == (int) cacheEntry.TickCount)
        return;
      this.m_cache[key] = cacheEntry.ToTickCount(tickCount);
    }

    private void MaybeSweep(uint tickCount)
    {
      uint lastSweepTickCount = (uint) this.m_lastSweepTickCount;
      uint num = tickCount - lastSweepTickCount;
      if (num <= this.m_maxAgeInMilliseconds / 2U || num >= 2147483648U || (long) lastSweepTickCount != (long) Interlocked.CompareExchange(ref this.m_lastSweepTickCount, (int) tickCount, (int) lastSweepTickCount))
        return;
      this.Sweep(tickCount);
    }

    private void Sweep(uint tickCount)
    {
      foreach (KeyValuePair<TKey, ExpirationCache<TKey, TValue>.CacheEntry> keyValuePair in this.m_cache)
      {
        uint num = tickCount - keyValuePair.Value.TickCount;
        if (num > this.m_maxAgeInMilliseconds && num < 2147483648U)
          this.m_cache.TryRemove(keyValuePair.Key, out ExpirationCache<TKey, TValue>.CacheEntry _);
      }
    }

    private static uint GetTickCount() => (uint) Environment.TickCount;

    private struct CacheEntry : IEquatable<ExpirationCache<TKey, TValue>.CacheEntry>
    {
      public readonly TValue Item;
      public readonly uint TickCount;
      private static IEqualityComparer<TValue> s_itemComparer = (IEqualityComparer<TValue>) EqualityComparer<TValue>.Default;

      public CacheEntry(TValue item, uint tickCount)
      {
        this.Item = item;
        this.TickCount = tickCount;
      }

      public ExpirationCache<TKey, TValue>.CacheEntry ToTickCount(uint tickCount) => new ExpirationCache<TKey, TValue>.CacheEntry(this.Item, tickCount);

      public bool Equals(ExpirationCache<TKey, TValue>.CacheEntry other) => (int) this.TickCount == (int) other.TickCount && ExpirationCache<TKey, TValue>.CacheEntry.s_itemComparer.Equals(this.Item, other.Item);
    }
  }
}
