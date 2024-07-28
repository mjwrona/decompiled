// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssCacheBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class VssCacheBase : IVssCachePerformanceProvider
  {
    private readonly Guid m_cacheId;
    private readonly string m_cacheName;
    private readonly IVssCachePerformanceProvider m_perfProvider;

    public VssCacheBase()
    {
      this.m_cacheName = this.GetType().Name;
      this.m_cacheId = this.GetType().GUID;
      this.m_perfProvider = (IVssCachePerformanceProvider) new VssCachePerformanceProvider(this.Name);
    }

    public virtual Guid Id => this.m_cacheId;

    public virtual string Name => this.m_cacheName;

    public void NotifyCacheLookupSucceeded() => this.m_perfProvider.NotifyCacheLookupSucceeded();

    public void NotifyCacheLookupFailed() => this.m_perfProvider.NotifyCacheLookupFailed();

    public void NotifyCacheItemsAdded(int count, MemoryCacheOperationStatistics stats) => this.m_perfProvider.NotifyCacheItemsAdded(count, stats);

    public void NotifyCacheItemsReplaced(int count, MemoryCacheOperationStatistics stats) => this.m_perfProvider.NotifyCacheItemsReplaced(count, stats);

    public void NotifyCacheItemsRemoved(int count, MemoryCacheOperationStatistics stats) => this.m_perfProvider.NotifyCacheItemsRemoved(count, stats);

    public void NotifyCacheItemsEvicted(int count, MemoryCacheOperationStatistics stats) => this.m_perfProvider.NotifyCacheItemsEvicted(count, stats);

    public void NotifyCacheItemsInvalidated(int count, MemoryCacheOperationStatistics stats) => this.m_perfProvider.NotifyCacheItemsInvalidated(count, stats);

    public void NotifyCacheCleared(MemoryCacheOperationStatistics stats) => this.m_perfProvider.NotifyCacheCleared(stats);

    public void NotifyCacheReset() => this.m_perfProvider.NotifyCacheReset();

    public void IncrementCacheRead(long elapsedMicroseconds) => this.m_perfProvider.IncrementCacheRead(elapsedMicroseconds);

    public void IncrementCacheWrite(long elapsedMicroseconds) => this.m_perfProvider.IncrementCacheWrite(elapsedMicroseconds);
  }
}
