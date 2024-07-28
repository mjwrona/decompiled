// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssCacheSubscriber`2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VssCacheSubscriber<TKey, TValue> : IMemoryCacheSubscriber<TKey, TValue>
  {
    private readonly IVssCachePerformanceProvider m_cache;

    public VssCacheSubscriber(IVssCachePerformanceProvider cache) => this.m_cache = cache;

    public void OnEntryLookupSucceeded(TKey key, TValue value) => this.m_cache.NotifyCacheLookupSucceeded();

    public void OnEntryLookupFailed(TKey key) => this.m_cache.NotifyCacheLookupFailed();

    public void OnEntryAdded(TKey key, TValue value, MemoryCacheOperationStatistics stats) => this.m_cache.NotifyCacheItemsAdded(1, stats);

    public void OnEntryReplaced(
      TKey key,
      TValue previousValue,
      TValue newValue,
      MemoryCacheOperationStatistics stats)
    {
      this.m_cache.NotifyCacheItemsReplaced(1, stats);
    }

    public void OnEntryRemoved(TKey key, TValue value, MemoryCacheOperationStatistics stats) => this.m_cache.NotifyCacheItemsRemoved(1, stats);

    public void OnEntryEvicted(TKey key, TValue value, MemoryCacheOperationStatistics stats) => this.m_cache.NotifyCacheItemsEvicted(1, stats);

    public void OnEntryInvalidated(TKey key, TValue value, MemoryCacheOperationStatistics stats) => this.m_cache.NotifyCacheItemsInvalidated(1, stats);

    public void OnCleared(MemoryCacheOperationStatistics stats) => this.m_cache.NotifyCacheCleared(stats);
  }
}
