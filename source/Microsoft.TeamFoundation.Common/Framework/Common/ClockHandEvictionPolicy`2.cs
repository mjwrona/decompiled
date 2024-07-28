// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.ClockHandEvictionPolicy`2
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ClockHandEvictionPolicy<TKey, TValue> : IMemoryCacheEvictionPolicy<TKey, TValue>
  {
    private ClockHandEvictionPolicy<TKey, TValue>.MemoryCacheEntry m_beforeClockHand;

    public IMemoryCacheEntry<TKey, TValue> Link(
      TKey key,
      TValue value,
      IMemoryCacheValidityPolicy<TKey, TValue> validityPolicy)
    {
      ClockHandEvictionPolicy<TKey, TValue>.MemoryCacheEntry memoryCacheEntry = new ClockHandEvictionPolicy<TKey, TValue>.MemoryCacheEntry(key, value, validityPolicy);
      if (this.m_beforeClockHand == null)
      {
        memoryCacheEntry.Next = memoryCacheEntry;
        memoryCacheEntry.Prev = memoryCacheEntry;
      }
      else
      {
        memoryCacheEntry.Next = this.m_beforeClockHand.Next;
        memoryCacheEntry.Prev = this.m_beforeClockHand;
        this.m_beforeClockHand.Next.Prev = memoryCacheEntry;
        this.m_beforeClockHand.Next = memoryCacheEntry;
      }
      this.m_beforeClockHand = memoryCacheEntry;
      return (IMemoryCacheEntry<TKey, TValue>) memoryCacheEntry;
    }

    public void Unlink(IMemoryCacheEntry<TKey, TValue> entry)
    {
      ClockHandEvictionPolicy<TKey, TValue>.MemoryCacheEntry memoryCacheEntry = (ClockHandEvictionPolicy<TKey, TValue>.MemoryCacheEntry) entry;
      memoryCacheEntry.Prev.Next = memoryCacheEntry.Next;
      memoryCacheEntry.Next.Prev = memoryCacheEntry.Prev;
      if (entry != this.m_beforeClockHand)
        return;
      if (this.m_beforeClockHand.Next == this.m_beforeClockHand)
        this.m_beforeClockHand = (ClockHandEvictionPolicy<TKey, TValue>.MemoryCacheEntry) null;
      else
        this.m_beforeClockHand = this.m_beforeClockHand.Next;
    }

    public void UnlinkAll() => this.m_beforeClockHand = (ClockHandEvictionPolicy<TKey, TValue>.MemoryCacheEntry) null;

    public IMemoryCacheEntry<TKey, TValue> GetEvictionCandidate()
    {
      ClockHandEvictionPolicy<TKey, TValue>.MemoryCacheEntry next;
      for (next = this.m_beforeClockHand.Next; next.Referenced && next.IsValid(); next = next.Next)
        next.Referenced = false;
      return (IMemoryCacheEntry<TKey, TValue>) next;
    }

    public IEnumerable<IMemoryCacheEntry<TKey, TValue>> GetInvalidationCandidates()
    {
      if (this.m_beforeClockHand != null)
      {
        ClockHandEvictionPolicy<TKey, TValue>.MemoryCacheEntry invalidationCandidate = this.m_beforeClockHand.Next;
        while (invalidationCandidate != this.m_beforeClockHand)
        {
          if (!invalidationCandidate.IsValid())
          {
            ClockHandEvictionPolicy<TKey, TValue>.MemoryCacheEntry tmp = invalidationCandidate.Next;
            yield return (IMemoryCacheEntry<TKey, TValue>) invalidationCandidate;
            invalidationCandidate = tmp;
            tmp = (ClockHandEvictionPolicy<TKey, TValue>.MemoryCacheEntry) null;
          }
          else
            invalidationCandidate = invalidationCandidate.Next;
        }
        if (!this.m_beforeClockHand.IsValid())
          yield return (IMemoryCacheEntry<TKey, TValue>) this.m_beforeClockHand;
      }
    }

    private class MemoryCacheEntry : IMemoryCacheEntry<TKey, TValue>
    {
      public bool Referenced;
      public ClockHandEvictionPolicy<TKey, TValue>.MemoryCacheEntry Prev;
      public ClockHandEvictionPolicy<TKey, TValue>.MemoryCacheEntry Next;

      public MemoryCacheEntry(
        TKey key,
        TValue value,
        IMemoryCacheValidityPolicy<TKey, TValue> validityPolicy)
      {
        this.Key = key;
        this.Value = value;
        this.Prev = (ClockHandEvictionPolicy<TKey, TValue>.MemoryCacheEntry) null;
        this.Next = (ClockHandEvictionPolicy<TKey, TValue>.MemoryCacheEntry) null;
        this.ValidityPolicy = validityPolicy;
        this.Referenced = false;
      }

      void IMemoryCacheEntry<TKey, TValue>.OnCacheHit() => this.Referenced = true;

      public bool IsValid() => this.ValidityPolicy == null || this.ValidityPolicy.IsValid(this.Key, this.Value);

      public TKey Key { get; private set; }

      public TValue Value { get; set; }

      public IMemoryCacheValidityPolicy<TKey, TValue> ValidityPolicy { get; set; }
    }
  }
}
