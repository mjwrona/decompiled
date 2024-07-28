// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.MemoryCacheMultiSubscriber`2
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class MemoryCacheMultiSubscriber<TKey, TValue> : IMemoryCacheSubscriber<TKey, TValue>
  {
    private readonly IMemoryCacheSubscriber<TKey, TValue>[] m_subscribers;

    public MemoryCacheMultiSubscriber(
      IEnumerable<IMemoryCacheSubscriber<TKey, TValue>> subscribers)
    {
      this.m_subscribers = subscribers.ToArray<IMemoryCacheSubscriber<TKey, TValue>>();
    }

    public void OnEntryLookupSucceeded(TKey key, TValue value)
    {
      for (int index = 0; index < this.m_subscribers.Length; ++index)
        this.m_subscribers[index].OnEntryLookupSucceeded(key, value);
    }

    public void OnEntryLookupFailed(TKey key)
    {
      for (int index = 0; index < this.m_subscribers.Length; ++index)
        this.m_subscribers[index].OnEntryLookupFailed(key);
    }

    public void OnEntryAdded(TKey key, TValue value, MemoryCacheOperationStatistics stats)
    {
      for (int index = 0; index < this.m_subscribers.Length; ++index)
        this.m_subscribers[index].OnEntryAdded(key, value, stats);
    }

    public void OnEntryReplaced(
      TKey key,
      TValue previousValue,
      TValue newValue,
      MemoryCacheOperationStatistics stats)
    {
      for (int index = 0; index < this.m_subscribers.Length; ++index)
        this.m_subscribers[index].OnEntryReplaced(key, previousValue, newValue, stats);
    }

    public void OnEntryRemoved(TKey key, TValue value, MemoryCacheOperationStatistics stats)
    {
      for (int index = 0; index < this.m_subscribers.Length; ++index)
        this.m_subscribers[index].OnEntryRemoved(key, value, stats);
    }

    public void OnEntryEvicted(TKey key, TValue value, MemoryCacheOperationStatistics stats)
    {
      for (int index = 0; index < this.m_subscribers.Length; ++index)
        this.m_subscribers[index].OnEntryEvicted(key, value, stats);
    }

    public void OnEntryInvalidated(TKey key, TValue value, MemoryCacheOperationStatistics stats)
    {
      for (int index = 0; index < this.m_subscribers.Length; ++index)
        this.m_subscribers[index].OnEntryInvalidated(key, value, stats);
    }

    public void OnCleared(MemoryCacheOperationStatistics stats)
    {
      for (int index = 0; index < this.m_subscribers.Length; ++index)
        this.m_subscribers[index].OnCleared(stats);
    }
  }
}
