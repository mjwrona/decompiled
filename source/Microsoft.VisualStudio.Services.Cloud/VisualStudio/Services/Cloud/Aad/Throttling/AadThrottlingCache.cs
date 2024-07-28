// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.Aad.Throttling.AadThrottlingCache
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Cloud.Aad.Throttling
{
  public class AadThrottlingCache : VssCacheBase
  {
    private static readonly Capture<TimeSpan> NoExpiryTimeInterval = Capture.Create<TimeSpan>(VssCacheExpiryProvider.NoExpiry);
    private readonly VssMemoryCacheList<string, DateTime> cache;
    private const int defaultcleanupIntervalMilliSeconds = 3600000;

    public AadThrottlingCache(
      IVssRequestContext requestContext,
      int maxLength,
      TimeSpan cleanupInterval)
    {
      this.cache = new VssMemoryCacheList<string, DateTime>((IVssCachePerformanceProvider) this, maxLength);
      ITeamFoundationTaskService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>();
      int interval = cleanupInterval.TotalMilliseconds > (double) int.MaxValue || cleanupInterval.TotalMilliseconds < 0.0 ? 3600000 : (int) cleanupInterval.TotalMilliseconds;
      IVssRequestContext requestContext1 = requestContext;
      TeamFoundationTask task = new TeamFoundationTask(new TeamFoundationTaskCallback(this.EvictExpiredTenantIds), (object) null, interval);
      service.AddTask(requestContext1, task);
    }

    public int Count => this.cache.Count;

    public void Remove(string tenantId) => this.cache.Remove(tenantId);

    public void Set(string tenantId, TimeSpan expireTimeSpan)
    {
      VssCacheExpiryProvider<string, DateTime> expiryProvider = new VssCacheExpiryProvider<string, DateTime>(new Capture<TimeSpan>(expireTimeSpan), AadThrottlingCache.NoExpiryTimeInterval);
      DateTime dateTime = DateTime.UtcNow.Add(expireTimeSpan);
      this.cache.Add(tenantId, dateTime, true, expiryProvider);
    }

    public void Sweep() => this.cache.Sweep();

    public bool TryGetValue(string tenantId, out DateTime throttlingExpireTime) => this.cache.TryGetValue(tenantId, out throttlingExpireTime);

    private void EvictExpiredTenantIds(IVssRequestContext requestContext, object taskargs) => this.Sweep();
  }
}
