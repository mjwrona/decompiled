// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssMemoryCacheGrouping`4
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal abstract class VssMemoryCacheGrouping<TKey, TValue, TGroupingKey, TGroupingContainer> : 
    IVssMemoryCacheGrouping<TKey, TValue, TGroupingKey>
    where TGroupingContainer : IEnumerable<TKey>
  {
    protected readonly IEqualityComparer<TKey> m_keyComparer;
    protected readonly Dictionary<TGroupingKey, TGroupingContainer> m_grouping;
    protected readonly VssMemoryCacheGroupingBehavior m_groupingBehavior;
    private readonly Func<TKey, TValue, IEnumerable<TGroupingKey>> m_getGroupingKeys;
    private readonly ReaderWriterLockSlim m_readerWriterLock = new ReaderWriterLockSlim();
    private static readonly TGroupingKey[] s_empty = Array.Empty<TGroupingKey>();

    public VssMemoryCacheGrouping(
      VssMemoryCacheList<TKey, TValue> cache,
      Func<TKey, TValue, IEnumerable<TGroupingKey>> getGroupingKeys,
      IEqualityComparer<TGroupingKey> groupingComparer = null,
      VssMemoryCacheGroupingBehavior groupingBehavior = VssMemoryCacheGroupingBehavior.Append)
    {
      ArgumentUtility.CheckForNull<VssMemoryCacheList<TKey, TValue>>(cache, nameof (cache));
      ArgumentUtility.CheckForNull<Func<TKey, TValue, IEnumerable<TGroupingKey>>>(getGroupingKeys, nameof (getGroupingKeys));
      this.m_grouping = new Dictionary<TGroupingKey, TGroupingContainer>(groupingComparer);
      this.m_getGroupingKeys = getGroupingKeys;
      this.m_keyComparer = cache.Comparer;
      this.m_groupingBehavior = groupingBehavior;
      cache.EntryAdded += (EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs>) ((sender, args) => this.Add(args.Key, args.Value));
      cache.EntryReplaced += (EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheReplacementEventArgs>) ((sender, args) => this.Update(args.Key, args.OldValue, args.NewValue));
      cache.EntryRemoved += (EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs>) ((sender, args) => this.Remove(args.Key, args.Value));
      cache.EntryEvicted += (EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs>) ((sender, args) => this.Remove(args.Key, args.Value));
      cache.EntryInvalidated += (EventHandler<MemoryCacheList<TKey, TValue>.MemoryCacheEventArgs>) ((sender, args) => this.Remove(args.Key, args.Value));
      cache.Cleared += (EventHandler) ((sender, args) => this.Clear());
    }

    public int Count
    {
      get
      {
        this.m_readerWriterLock.EnterReadLock();
        try
        {
          return this.m_grouping.Count;
        }
        finally
        {
          this.m_readerWriterLock.ExitReadLock();
        }
      }
    }

    public bool TryGetKeys(TGroupingKey groupingKey, out IEnumerable<TKey> keys)
    {
      this.m_readerWriterLock.EnterReadLock();
      try
      {
        TGroupingContainer collection;
        if (this.m_grouping.TryGetValue(groupingKey, out collection))
        {
          keys = !this.IsSynchronized ? (IEnumerable<TKey>) new List<TKey>((IEnumerable<TKey>) collection) : (IEnumerable<TKey>) collection;
          return true;
        }
        keys = (IEnumerable<TKey>) null;
        return false;
      }
      finally
      {
        this.m_readerWriterLock.ExitReadLock();
      }
    }

    private void Add(TKey key, TValue value)
    {
      IEnumerable<TGroupingKey> groupingKeys = (IEnumerable<TGroupingKey>) ((object) this.m_getGroupingKeys(key, value) ?? (object) VssMemoryCacheGrouping<TKey, TValue, TGroupingKey, TGroupingContainer>.s_empty);
      this.m_readerWriterLock.EnterWriteLock();
      try
      {
        this.Add(key, value, groupingKeys);
      }
      finally
      {
        this.m_readerWriterLock.ExitWriteLock();
      }
    }

    private void Remove(TKey key, TValue value)
    {
      IEnumerable<TGroupingKey> groupingKeys = (IEnumerable<TGroupingKey>) ((object) this.m_getGroupingKeys(key, value) ?? (object) VssMemoryCacheGrouping<TKey, TValue, TGroupingKey, TGroupingContainer>.s_empty);
      this.m_readerWriterLock.EnterWriteLock();
      try
      {
        this.Remove(key, value, groupingKeys);
      }
      finally
      {
        this.m_readerWriterLock.ExitWriteLock();
      }
    }

    private void Update(TKey key, TValue oldValue, TValue newValue)
    {
      IEnumerable<TGroupingKey> groupingKeys1 = (IEnumerable<TGroupingKey>) ((object) this.m_getGroupingKeys(key, oldValue) ?? (object) VssMemoryCacheGrouping<TKey, TValue, TGroupingKey, TGroupingContainer>.s_empty);
      IEnumerable<TGroupingKey> groupingKeys2 = (IEnumerable<TGroupingKey>) ((object) this.m_getGroupingKeys(key, newValue) ?? (object) VssMemoryCacheGrouping<TKey, TValue, TGroupingKey, TGroupingContainer>.s_empty);
      this.m_readerWriterLock.EnterWriteLock();
      try
      {
        this.Remove(key, oldValue, groupingKeys1);
        this.Add(key, newValue, groupingKeys2);
      }
      finally
      {
        this.m_readerWriterLock.ExitWriteLock();
      }
    }

    private void Clear()
    {
      this.m_readerWriterLock.EnterWriteLock();
      try
      {
        this.m_grouping.Clear();
      }
      finally
      {
        this.m_readerWriterLock.ExitWriteLock();
      }
    }

    protected abstract bool IsSynchronized { get; }

    protected abstract void Add(TKey key, TValue value, IEnumerable<TGroupingKey> groupingKeys);

    protected abstract void Remove(TKey key, TValue value, IEnumerable<TGroupingKey> groupingKeys);
  }
}
