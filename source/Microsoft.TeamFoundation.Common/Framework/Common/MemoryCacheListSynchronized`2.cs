// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.MemoryCacheListSynchronized`2
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class MemoryCacheListSynchronized<TKey, TValue> : 
    MemoryCacheListUnsafe<TKey, TValue>,
    IMemoryCacheList<TKey, TValue>
  {
    private readonly ReaderWriterLockSlim m_readerWriterLock = new ReaderWriterLockSlim();

    public MemoryCacheListSynchronized(
      IMemoryCacheCapacityPolicy<TKey, TValue> capacityPolicy,
      IMemoryCacheEvictionPolicy<TKey, TValue> evictionPolicy,
      IMemoryCacheSubscriber<TKey, TValue> subscriber)
      : base((IEqualityComparer<TKey>) null, capacityPolicy, evictionPolicy, subscriber)
    {
    }

    public MemoryCacheListSynchronized(
      IEqualityComparer<TKey> comparer,
      IMemoryCacheCapacityPolicy<TKey, TValue> capacityPolicy,
      IMemoryCacheEvictionPolicy<TKey, TValue> evictionPolicy,
      IMemoryCacheSubscriber<TKey, TValue> subscriber)
      : base(comparer, capacityPolicy, evictionPolicy, subscriber)
    {
    }

    public new void Clear()
    {
      this.m_readerWriterLock.EnterWriteLock();
      try
      {
        base.Clear();
      }
      finally
      {
        this.m_readerWriterLock.ExitWriteLock();
      }
    }

    public new bool Add(
      TKey key,
      TValue value,
      bool overwrite,
      IMemoryCacheValidityPolicy<TKey, TValue> validityPolicy)
    {
      this.m_readerWriterLock.EnterWriteLock();
      try
      {
        return base.Add(key, value, overwrite, validityPolicy);
      }
      finally
      {
        this.m_readerWriterLock.ExitWriteLock();
      }
    }

    public new bool Remove(TKey key)
    {
      this.m_readerWriterLock.EnterWriteLock();
      try
      {
        return base.Remove(key);
      }
      finally
      {
        this.m_readerWriterLock.ExitWriteLock();
      }
    }

    public new bool TryGetValue(TKey key, out TValue value)
    {
      this.m_readerWriterLock.EnterReadLock();
      try
      {
        return base.TryGetValue(key, out value);
      }
      finally
      {
        this.m_readerWriterLock.ExitReadLock();
      }
    }

    public new int Count
    {
      get
      {
        this.m_readerWriterLock.EnterReadLock();
        try
        {
          return base.Count;
        }
        finally
        {
          this.m_readerWriterLock.ExitReadLock();
        }
      }
    }
  }
}
