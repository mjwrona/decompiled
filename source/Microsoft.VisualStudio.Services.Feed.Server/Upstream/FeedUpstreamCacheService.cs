// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Upstream.FeedUpstreamCacheService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Feed.Server.Upstream
{
  public class FeedUpstreamCacheService : 
    VssMemoryCacheService<RawUpstreamSourceIdentity, IEnumerable<DownstreamFeeds>>,
    IFeedUpstreamCacheService,
    IFeedUpstreamService,
    IVssFrameworkService
  {
    private const double DefaultEvictionTimeInMinutes = 1.0;
    private const double DefaultCleanupTimeInMinutes = 1.0;

    public FeedUpstreamCacheService()
      : this(1.0, 1.0)
    {
    }

    public FeedUpstreamCacheService(double evictionTimeInMinutes, double cleanupTimeInMinutes)
      : base(TimeSpan.FromMinutes(cleanupTimeInMinutes))
    {
      this.InactivityInterval.Value = (TimeSpan) Capture.Create<TimeSpan>(TimeSpan.FromMinutes(evictionTimeInMinutes));
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => requestContext.TraceBlock(10019000, 10019001, 10019002, "Feed", "Service", (Action) (() =>
    {
      // ISSUE: reference to a compiler-generated method
      this.\u003C\u003En__0(requestContext);
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
    }), "ServiceStart");

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext) => requestContext.TraceBlock(10019003, 10019004, 10019005, "Feed", "Service", (Action) (() =>
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      // ISSUE: reference to a compiler-generated method
      this.\u003C\u003En__1(requestContext);
    }), "ServiceEnd");

    public IEnumerable<DownstreamFeeds> GetFeedsWithUpstream(
      IVssRequestContext requestContext,
      Guid collectionId,
      Guid? projectId,
      Guid feedId,
      Guid viewId,
      string protocolType,
      string normalizedPackageName)
    {
      IEnumerable<DownstreamFeeds> feeds;
      if (!this.TryGetValueWrapper(requestContext, collectionId, projectId, feedId, viewId, protocolType, out feeds))
        feeds = this.AddFeedToCacheFromServiceAsync(requestContext, collectionId, projectId, feedId, viewId, protocolType, normalizedPackageName);
      return feeds;
    }

    private IEnumerable<DownstreamFeeds> AddFeedToCacheFromServiceAsync(
      IVssRequestContext requestContext,
      Guid collectionId,
      Guid? projectId,
      Guid feedId,
      Guid viewId,
      string protocolType,
      string normalizedPackageName)
    {
      IEnumerable<DownstreamFeeds> feedsWithUpstream = requestContext.GetService<IFeedUpstreamSQLService>().GetFeedsWithUpstream(requestContext, collectionId, projectId, feedId, viewId, protocolType, normalizedPackageName);
      return this.AddFeed(requestContext, collectionId, projectId, feedId, viewId, protocolType, feedsWithUpstream);
    }

    private IEnumerable<DownstreamFeeds> AddFeed(
      IVssRequestContext requestContext,
      Guid collectionId,
      Guid? projectId,
      Guid feedId,
      Guid viewId,
      string protocolType,
      IEnumerable<DownstreamFeeds> downstreamFeeds)
    {
      RawUpstreamSourceIdentity key = this.BuildKey(collectionId, projectId, feedId, viewId, protocolType);
      bool flag = this.TryAdd(requestContext, key, downstreamFeeds);
      IVssRequestContext requestContext1 = requestContext;
      string message;
      if (!flag)
        message = string.Format("Feeds list could not be added to cache: Key= {0}, CollectionId={1}, Project= {2}, FeedId= {3}, ViewId= {4}", (object) key, (object) collectionId, (object) projectId, (object) feedId, (object) viewId);
      else
        message = string.Format("Feeds list added: Key= {0}, CollectionId={1}, Project= {2}, FeedId= {3}, ViewId= {4}", (object) key, (object) collectionId, (object) projectId, (object) feedId, (object) viewId);
      requestContext1.Trace(10019139, TraceLevel.Info, "Feed", "UpstreamCacheService", message);
      return downstreamFeeds;
    }

    private bool TryGetValueWrapper(
      IVssRequestContext requestContext,
      Guid collectionId,
      Guid? projectId,
      Guid feedId,
      Guid viewId,
      string protocolType,
      out IEnumerable<DownstreamFeeds> feeds)
    {
      if (requestContext.IsFeatureEnabled("Packaging.Feed.DownstreamNotificationRequiresLocalVersion"))
      {
        feeds = (IEnumerable<DownstreamFeeds>) null;
        return false;
      }
      RawUpstreamSourceIdentity key = this.BuildKey(collectionId, projectId, feedId, viewId, protocolType);
      bool valueWrapper = this.TryGetValue(requestContext, key, out feeds);
      IVssRequestContext requestContext1 = requestContext;
      string message;
      if (!valueWrapper)
        message = string.Format("Cache miss for feed: Key= {0}, CollectionId={1}, Project= {2}, FeedId= {3}, ViewId= {4}", (object) key, (object) collectionId, (object) projectId, (object) feedId, (object) viewId);
      else
        message = string.Format("Cache hit: Key= {0}, CollectionId={1}, Project= {2}, FeedId= {3}, ViewId= {4}", (object) key, (object) collectionId, (object) projectId, (object) feedId, (object) viewId);
      requestContext1.Trace(10019138, TraceLevel.Info, "Feed", "UpstreamCacheService", message);
      return valueWrapper;
    }

    public RawUpstreamSourceIdentity BuildKey(
      Guid collectionId,
      Guid? projectId,
      Guid feedId,
      Guid viewId,
      string protocolType)
    {
      return new RawUpstreamSourceIdentity(collectionId, feedId, projectId, viewId, protocolType);
    }
  }
}
