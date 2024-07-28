// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssMemoryCacheList`2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VssMemoryCacheList<TKey, TValue> : IMemoryCacheList<TKey, TValue>
  {
    private readonly MemoryCacheListUnsafe<TKey, VssMemoryCacheList<TKey, TValue>.ValueWithProperties> m_cache;
    private readonly ISizeProvider<TKey, TValue> m_sizeProvider;
    private readonly ITimeProvider m_timeProvider;
    private readonly IVssCachePerformanceProvider m_performanceProvider;
    private readonly IMemoryCacheValidityPolicy<TKey, VssMemoryCacheList<TKey, TValue>.ValueWithProperties> m_defaultValidityPolicy;
    private readonly ReaderWriterLockSlim m_readerWriterLock = new ReaderWriterLockSlim();

    public VssMemoryCacheList(
      IVssCachePerformanceProvider cacheService,
      VssCacheExpiryProvider<TKey, TValue> expiryProvider = null)
      : this(cacheService, (IEqualityComparer<TKey>) null, expiryProvider)
    {
    }

    public VssMemoryCacheList(
      IVssCachePerformanceProvider cacheService,
      IEqualityComparer<TKey> comparer,
      VssCacheExpiryProvider<TKey, TValue> expiryProvider = null)
      : this(cacheService, comparer, int.MaxValue, expiryProvider)
    {
    }

    public VssMemoryCacheList(
      IVssCachePerformanceProvider cacheService,
      int maxLength,
      VssCacheExpiryProvider<TKey, TValue> expiryProvider = null)
      : this(cacheService, CaptureLength.Create(maxLength), expiryProvider)
    {
    }

    public VssMemoryCacheList(
      IVssCachePerformanceProvider cacheService,
      CaptureLength maxLength,
      VssCacheExpiryProvider<TKey, TValue> expiryProvider = null)
      : this(cacheService, (IEqualityComparer<TKey>) null, maxLength, expiryProvider)
    {
    }

    public VssMemoryCacheList(
      IVssCachePerformanceProvider cacheService,
      IEqualityComparer<TKey> comparer,
      int maxLength,
      VssCacheExpiryProvider<TKey, TValue> expiryProvider = null)
      : this(cacheService, comparer, CaptureLength.Create(maxLength), expiryProvider)
    {
    }

    public VssMemoryCacheList(
      IVssCachePerformanceProvider cacheService,
      IEqualityComparer<TKey> comparer,
      CaptureLength maxLength,
      VssCacheExpiryProvider<TKey, TValue> expiryProvider = null)
      : this(cacheService, comparer, (int) (Capture<int>) maxLength, (long) Capture.Create<long>(long.MaxValue), (ISizeProvider<TKey, TValue>) new NoSizeProvider<TKey, TValue>(), expiryProvider)
    {
    }

    public VssMemoryCacheList(
      IVssCachePerformanceProvider cacheService,
      long maxSize,
      ISizeProvider<TKey, TValue> sizeProvider,
      VssCacheExpiryProvider<TKey, TValue> expiryProvider = null)
      : this(cacheService, CaptureSize.Create(maxSize), sizeProvider, expiryProvider)
    {
    }

    public VssMemoryCacheList(
      IVssCachePerformanceProvider cacheService,
      CaptureSize maxSize,
      ISizeProvider<TKey, TValue> sizeProvider,
      VssCacheExpiryProvider<TKey, TValue> expiryProvider = null)
      : this(cacheService, (IEqualityComparer<TKey>) null, maxSize, sizeProvider, expiryProvider)
    {
    }

    public VssMemoryCacheList(
      IVssCachePerformanceProvider cacheService,
      IEqualityComparer<TKey> comparer,
      long maxSize,
      ISizeProvider<TKey, TValue> sizeProvider,
      VssCacheExpiryProvider<TKey, TValue> expiryProvider = null)
      : this(cacheService, comparer, CaptureSize.Create(maxSize), sizeProvider, expiryProvider)
    {
    }

    public VssMemoryCacheList(
      IVssCachePerformanceProvider cacheService,
      IEqualityComparer<TKey> comparer,
      CaptureSize maxSize,
      ISizeProvider<TKey, TValue> sizeProvider,
      VssCacheExpiryProvider<TKey, TValue> expiryProvider = null)
      : this(cacheService, comparer, CaptureLength.Create(int.MaxValue), maxSize, sizeProvider, expiryProvider)
    {
    }

    public VssMemoryCacheList(
      IVssCachePerformanceProvider cacheService,
      int maxLength,
      long maxSize,
      ISizeProvider<TKey, TValue> sizeProvider,
      VssCacheExpiryProvider<TKey, TValue> expiryProvider = null)
      : this(cacheService, CaptureLength.Create(maxLength), CaptureSize.Create(maxSize), sizeProvider, expiryProvider)
    {
    }

    public VssMemoryCacheList(
      IVssCachePerformanceProvider cacheService,
      CaptureLength maxLength,
      CaptureSize maxSize,
      ISizeProvider<TKey, TValue> sizeProvider,
      VssCacheExpiryProvider<TKey, TValue> expiryProvider = null)
      : this(cacheService, (IEqualityComparer<TKey>) null, maxLength, maxSize, sizeProvider, expiryProvider)
    {
    }

    public VssMemoryCacheList(
      IVssCachePerformanceProvider cacheService,
      IEqualityComparer<TKey> comparer,
      int maxLength,
      long maxSize,
      ISizeProvider<TKey, TValue> sizeProvider,
      VssCacheExpiryProvider<TKey, TValue> expiryProvider = null)
      : this(cacheService, comparer, CaptureLength.Create(maxLength), CaptureSize.Create(maxSize), sizeProvider, expiryProvider, (ITimeProvider) null)
    {
    }

    public VssMemoryCacheList(
      IVssCachePerformanceProvider cacheService,
      IEqualityComparer<TKey> comparer,
      CaptureLength maxLength,
      CaptureSize maxSize,
      ISizeProvider<TKey, TValue> sizeProvider,
      VssCacheExpiryProvider<TKey, TValue> expiryProvider = null)
      : this(cacheService, comparer, maxLength, maxSize, sizeProvider, expiryProvider, (ITimeProvider) null)
    {
    }

    internal VssMemoryCacheList(
      IVssCachePerformanceProvider cacheService,
      IEqualityComparer<TKey> comparer,
      CaptureLength maxLength,
      CaptureSize maxSize,
      ISizeProvider<TKey, TValue> sizeProvider,
      VssCacheExpiryProvider<TKey, TValue> expiryProvider,
      ITimeProvider timeProvider)
    {
      this.Comparer = comparer ?? (IEqualityComparer<TKey>) EqualityComparer<TKey>.Default;
      this.m_sizeProvider = sizeProvider;
      this.m_timeProvider = timeProvider ?? (ITimeProvider) new DefaultTimeProvider();
      if (expiryProvider != null)
        this.m_defaultValidityPolicy = (IMemoryCacheValidityPolicy<TKey, VssMemoryCacheList<TKey, TValue>.ValueWithProperties>) new VssMemoryCacheList<TKey, TValue>.ExpiryMemoryCacheProvider((IExpiryProvider<TKey, TValue>) expiryProvider);
      this.m_performanceProvider = cacheService;
      this.m_cache = new MemoryCacheListUnsafe<TKey, VssMemoryCacheList<TKey, TValue>.ValueWithProperties>(this.Comparer, (IMemoryCacheCapacityPolicy<TKey, VssMemoryCacheList<TKey, TValue>.ValueWithProperties>) new LengthAndSizeMemoryCachePolicy<TKey, VssMemoryCacheList<TKey, TValue>.ValueWithProperties>(maxLength, maxSize, (ISizeProvider<TKey, VssMemoryCacheList<TKey, TValue>.ValueWithProperties>) new VssMemoryCacheList<TKey, TValue>.SizeProvider()), (IMemoryCacheEvictionPolicy<TKey, VssMemoryCacheList<TKey, TValue>.ValueWithProperties>) new ClockHandEvictionPolicy<TKey, VssMemoryCacheList<TKey, TValue>.ValueWithProperties>(), MemoryCacheMultiSubscriber.Create<TKey, VssMemoryCacheList<TKey, TValue>.ValueWithProperties>((IMemoryCacheSubscriber<TKey, VssMemoryCacheList<TKey, TValue>.ValueWithProperties>) new VssCacheSubscriber<TKey, VssMemoryCacheList<TKey, TValue>.ValueWithProperties>(cacheService), (IMemoryCacheSubscriber<TKey, VssMemoryCacheList<TKey, TValue>.ValueWithProperties>) new VssMemoryCacheList<TKey, TValue>.CacheSubscriber(this)));
    }

    public event EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheReplacementEventArgs> EntryReplaced;

    public event EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs> EntryEvicted;

    public event EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs> EntryInvalidated;

    public event EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs> EntryRemoved;

    public event EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs> EntryAdded;

    public event EventHandler Cleared;

    public IEqualityComparer<TKey> Comparer { get; private set; }

    public bool TryGetValue(
      TKey key,
      out TValue value,
      out DateTime modifiedOn,
      out DateTime accessedOn,
      bool updateTimestamp)
    {
      long timeStamp = HighResTimer.TimeStamp;
      this.m_readerWriterLock.EnterReadLock();
      try
      {
        VssMemoryCacheList<TKey, TValue>.ValueWithProperties valueWithProperties;
        if (this.m_cache.TryGetValue(key, out valueWithProperties))
        {
          value = valueWithProperties.Value;
          modifiedOn = valueWithProperties.ModifiedTimestamp;
          accessedOn = valueWithProperties.AccessedTimestamp;
          if (updateTimestamp)
            valueWithProperties.AccessedTimestamp = this.m_timeProvider.Now;
          return true;
        }
        value = default (TValue);
        accessedOn = new DateTime();
        modifiedOn = new DateTime();
        return false;
      }
      finally
      {
        this.m_readerWriterLock.ExitReadLock();
        this.m_performanceProvider.IncrementCacheRead(HighResTimer.ElapsedTime(HighResTimer.TimeStamp, timeStamp));
      }
    }

    public bool Add(
      TKey key,
      TValue value,
      DateTime timestamp,
      bool overwrite,
      VssCacheExpiryProvider<TKey, TValue> expiryProvider = null)
    {
      VssMemoryCacheList<TKey, TValue>.ValueWithProperties valueWithProps = new VssMemoryCacheList<TKey, TValue>.ValueWithProperties(value, this.m_sizeProvider.GetSize(key, value), timestamp);
      IMemoryCacheValidityPolicy<TKey, VssMemoryCacheList<TKey, TValue>.ValueWithProperties> validityPolicy = this.m_defaultValidityPolicy;
      if (expiryProvider != null)
        validityPolicy = (IMemoryCacheValidityPolicy<TKey, VssMemoryCacheList<TKey, TValue>.ValueWithProperties>) new VssMemoryCacheList<TKey, TValue>.ExpiryMemoryCacheProvider((IExpiryProvider<TKey, TValue>) expiryProvider);
      return this.Add(key, valueWithProps, overwrite, validityPolicy);
    }

    public bool Add(
      TKey key,
      TValue value,
      bool overwrite,
      VssCacheExpiryProvider<TKey, TValue> expiryProvider = null)
    {
      return this.Add(key, value, this.m_timeProvider.Now, overwrite, expiryProvider);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      DateTime dateTime;
      return this.TryGetValue(key, out value, out dateTime, out dateTime, true);
    }

    bool IMemoryCacheList<TKey, TValue>.Add(
      TKey key,
      TValue value,
      bool overwrite,
      IMemoryCacheValidityPolicy<TKey, TValue> validityPolicy)
    {
      VssMemoryCacheList<TKey, TValue>.ValueWithProperties valueWithProps = new VssMemoryCacheList<TKey, TValue>.ValueWithProperties(value, this.m_sizeProvider.GetSize(key, value), DateTime.UtcNow);
      IMemoryCacheValidityPolicy<TKey, VssMemoryCacheList<TKey, TValue>.ValueWithProperties> validityPolicy1 = this.m_defaultValidityPolicy;
      if (validityPolicy != null)
        validityPolicy1 = (IMemoryCacheValidityPolicy<TKey, VssMemoryCacheList<TKey, TValue>.ValueWithProperties>) new VssMemoryCacheList<TKey, TValue>.ExpiryMemoryCachePolicy(validityPolicy);
      return this.Add(key, valueWithProps, overwrite, validityPolicy1);
    }

    public bool Remove(TKey key)
    {
      long timeStamp = HighResTimer.TimeStamp;
      this.m_readerWriterLock.EnterWriteLock();
      try
      {
        return this.m_cache.Remove(key);
      }
      finally
      {
        this.m_readerWriterLock.ExitWriteLock();
        this.m_performanceProvider.IncrementCacheWrite(HighResTimer.ElapsedTime(HighResTimer.TimeStamp, timeStamp));
      }
    }

    public void Clear()
    {
      long timeStamp = HighResTimer.TimeStamp;
      this.m_readerWriterLock.EnterWriteLock();
      try
      {
        this.m_cache.Clear();
      }
      finally
      {
        this.m_readerWriterLock.ExitWriteLock();
        this.m_performanceProvider.IncrementCacheWrite(HighResTimer.ElapsedTime(HighResTimer.TimeStamp, timeStamp));
      }
    }

    public int Sweep()
    {
      long timeStamp = HighResTimer.TimeStamp;
      this.m_readerWriterLock.EnterWriteLock();
      try
      {
        return this.m_cache.Sweep();
      }
      finally
      {
        this.m_readerWriterLock.ExitWriteLock();
        this.m_performanceProvider.IncrementCacheWrite(HighResTimer.ElapsedTime(HighResTimer.TimeStamp, timeStamp));
      }
    }

    public int Count
    {
      get
      {
        this.m_readerWriterLock.EnterReadLock();
        try
        {
          return this.m_cache.Count;
        }
        finally
        {
          this.m_readerWriterLock.ExitReadLock();
        }
      }
    }

    public void Add(TKey key, TValue value)
    {
      if (!this.Add(key, value, false))
        throw new ArgumentException();
    }

    public TValue this[TKey key]
    {
      get
      {
        TValue obj;
        if (!this.TryGetValue(key, out obj))
          throw new KeyNotFoundException();
        return obj;
      }
      set => this.Add(key, value, true);
    }

    private bool Add(
      TKey key,
      VssMemoryCacheList<TKey, TValue>.ValueWithProperties valueWithProps,
      bool overwrite,
      IMemoryCacheValidityPolicy<TKey, VssMemoryCacheList<TKey, TValue>.ValueWithProperties> validityPolicy)
    {
      long timeStamp = HighResTimer.TimeStamp;
      this.m_readerWriterLock.EnterWriteLock();
      try
      {
        return this.m_cache.Add(key, valueWithProps, overwrite, validityPolicy);
      }
      finally
      {
        this.m_readerWriterLock.ExitWriteLock();
        this.m_performanceProvider.IncrementCacheWrite(HighResTimer.ElapsedTime(HighResTimer.TimeStamp, timeStamp));
      }
    }

    private class ValueWithProperties
    {
      private long m_timestamp;

      public ValueWithProperties(TValue value, long size, DateTime timestamp)
      {
        this.Value = value;
        this.Size = size;
        this.ModifiedTimestamp = timestamp;
        this.AccessedTimestamp = timestamp;
      }

      public TValue Value { get; set; }

      public long Size { get; set; }

      public DateTime ModifiedTimestamp { get; set; }

      public DateTime AccessedTimestamp
      {
        get => DateTime.FromBinary(Interlocked.Read(ref this.m_timestamp));
        set => Interlocked.Exchange(ref this.m_timestamp, value.ToBinary());
      }
    }

    private class SizeProvider : 
      ISizeProvider<TKey, VssMemoryCacheList<TKey, TValue>.ValueWithProperties>
    {
      public long GetSize(
        TKey key,
        VssMemoryCacheList<TKey, TValue>.ValueWithProperties value)
      {
        return value.Size;
      }
    }

    private class ExpiryMemoryCacheProvider : 
      IMemoryCacheValidityPolicy<TKey, VssMemoryCacheList<TKey, TValue>.ValueWithProperties>
    {
      private readonly IExpiryProvider<TKey, TValue> m_expiryProvider;

      public ExpiryMemoryCacheProvider(IExpiryProvider<TKey, TValue> expiryProvider) => this.m_expiryProvider = expiryProvider;

      public bool IsValid(
        TKey key,
        VssMemoryCacheList<TKey, TValue>.ValueWithProperties value)
      {
        return !this.m_expiryProvider.IsExpired(key, value.Value, value.ModifiedTimestamp, value.AccessedTimestamp);
      }
    }

    private class ExpiryMemoryCachePolicy : 
      IMemoryCacheValidityPolicy<TKey, VssMemoryCacheList<TKey, TValue>.ValueWithProperties>
    {
      private readonly IMemoryCacheValidityPolicy<TKey, TValue> m_validityPolicy;

      public ExpiryMemoryCachePolicy(
        IMemoryCacheValidityPolicy<TKey, TValue> validityPolicy)
      {
        this.m_validityPolicy = validityPolicy;
      }

      public bool IsValid(
        TKey key,
        VssMemoryCacheList<TKey, TValue>.ValueWithProperties value)
      {
        return this.m_validityPolicy.IsValid(key, value.Value);
      }
    }

    private class CacheSubscriber : 
      IMemoryCacheSubscriber<TKey, VssMemoryCacheList<TKey, TValue>.ValueWithProperties>
    {
      private readonly VssMemoryCacheList<TKey, TValue> m_owner;

      public CacheSubscriber(VssMemoryCacheList<TKey, TValue> owner) => this.m_owner = owner;

      public void OnEntryLookupSucceeded(
        TKey key,
        VssMemoryCacheList<TKey, TValue>.ValueWithProperties value)
      {
      }

      public void OnEntryLookupFailed(TKey key)
      {
      }

      public void OnEntryReplaced(
        TKey key,
        VssMemoryCacheList<TKey, TValue>.ValueWithProperties previousValue,
        VssMemoryCacheList<TKey, TValue>.ValueWithProperties newValue,
        MemoryCacheOperationStatistics stats)
      {
        EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheReplacementEventArgs> entryReplaced = this.m_owner.EntryReplaced;
        if (entryReplaced == null)
          return;
        entryReplaced((object) this.m_owner, new MemoryCacheList<TKey, TValue>.MemoryCacheReplacementEventArgs(key, previousValue.Value, newValue.Value));
      }

      public void OnEntryEvicted(
        TKey key,
        VssMemoryCacheList<TKey, TValue>.ValueWithProperties value,
        MemoryCacheOperationStatistics stats)
      {
        EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs> entryEvicted = this.m_owner.EntryEvicted;
        if (entryEvicted == null)
          return;
        entryEvicted((object) this.m_owner, new MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs(key, value.Value));
      }

      public void OnEntryInvalidated(
        TKey key,
        VssMemoryCacheList<TKey, TValue>.ValueWithProperties value,
        MemoryCacheOperationStatistics stats)
      {
        EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs> entryInvalidated = this.m_owner.EntryInvalidated;
        if (entryInvalidated == null)
          return;
        entryInvalidated((object) this.m_owner, new MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs(key, value.Value));
      }

      public void OnEntryRemoved(
        TKey key,
        VssMemoryCacheList<TKey, TValue>.ValueWithProperties value,
        MemoryCacheOperationStatistics stats)
      {
        EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs> entryRemoved = this.m_owner.EntryRemoved;
        if (entryRemoved == null)
          return;
        entryRemoved((object) this.m_owner, new MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs(key, value.Value));
      }

      public void OnEntryAdded(
        TKey key,
        VssMemoryCacheList<TKey, TValue>.ValueWithProperties value,
        MemoryCacheOperationStatistics stats)
      {
        EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs> entryAdded = this.m_owner.EntryAdded;
        if (entryAdded == null)
          return;
        entryAdded((object) this.m_owner, new MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs(key, value.Value));
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
