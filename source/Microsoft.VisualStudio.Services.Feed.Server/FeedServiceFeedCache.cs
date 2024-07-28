// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedServiceFeedCache
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  internal class FeedServiceFeedCache : 
    VssVersionedCacheService<FeedCacheData>,
    IFeedServiceFeedCache,
    IVssFrameworkService
  {
    public static readonly Guid FeedChangedNotificationId = new Guid("B054E17F-E082-4F67-9186-D3DAE24CB6D4");

    public IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeeds(
      IVssRequestContext requestContext,
      ProjectReference project,
      bool includeDeletedUpstreams = false)
    {
      if (!requestContext.IsFeatureEnabled("Packaging.Feed.InvalidateFeedCacheAllAtOnce"))
        this.Invalidate(requestContext);
      return this.CloneFeeds(this.Read<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>>(requestContext, (Func<FeedCacheData, List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>>) (feedCacheData => feedCacheData.Values.ToList<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>())).Where<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Microsoft.VisualStudio.Services.Feed.WebApi.Feed, bool>) (feed => FeedServiceFeedCache.ShouldIncludeForAllFeedsProjectScopedRequest(feed, project))), includeDeletedUpstreams);
    }

    public virtual Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeed(
      IVssRequestContext requestContext,
      Guid id,
      ProjectReference project = null,
      bool includeDeletedUpstreams = false)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed;
      return FeedServiceFeedCache.CloneFeed(this.Read<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(requestContext, (Func<FeedCacheData, Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) (feedCacheData => feedCacheData.TryGetValue(id, out feed) && FeedServiceFeedCache.ShouldIncludeForFeedProjectScopedRequest(feed, project) ? feed : (Microsoft.VisualStudio.Services.Feed.WebApi.Feed) null)) ?? this.FaultInFeed(requestContext, (Func<FeedSqlResourceComponent, Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) (component => component.GetFeed(new FeedIdentity(project?.Id, id), true))), includeDeletedUpstreams);
    }

    public virtual Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeedByIdForAnyScope(
      IVssRequestContext requestContext,
      Guid id)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed;
      return FeedServiceFeedCache.CloneFeed(this.Read<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(requestContext, (Func<FeedCacheData, Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) (feedCacheData => !feedCacheData.TryGetValue(id, out feed) ? (Microsoft.VisualStudio.Services.Feed.WebApi.Feed) null : feed)) ?? this.FaultInFeed(requestContext, (Func<FeedSqlResourceComponent, Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) (component => component.GetFeedByIdForAnyScope(id, false))));
    }

    public Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeed(
      IVssRequestContext requestContext,
      string feedName,
      ProjectReference project = null,
      bool includeDeletedUpstreams = false)
    {
      return FeedServiceFeedCache.CloneFeed(this.Read<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(requestContext, (Func<FeedCacheData, Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) (feedCacheData =>
      {
        Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed1 = feedCacheData.Values.FirstOrDefault<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Microsoft.VisualStudio.Services.Feed.WebApi.Feed, bool>) (feed => StringComparer.InvariantCultureIgnoreCase.Equals(feed.Name, feedName)));
        return feed1 == null || !FeedServiceFeedCache.ShouldIncludeForFeedProjectScopedRequest(feed1, project) ? (Microsoft.VisualStudio.Services.Feed.WebApi.Feed) null : feed1;
      })) ?? this.FaultInFeed(requestContext, (Func<FeedSqlResourceComponent, Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) (component => component.GetFeed(feedName, project?.Id, includeDeletedUpstreams: true))), includeDeletedUpstreams);
    }

    public void Invalidate(IVssRequestContext requestContext) => this.Reset(requestContext);

    public void Invalidate(IVssRequestContext requestContext, Guid feedId)
    {
      if (requestContext.IsFeatureEnabled("Packaging.Feed.InvalidateFeedCacheAllAtOnce"))
        this.Invalidate(requestContext);
      else
        this.Invalidate<bool>(requestContext, (Func<FeedCacheData, bool>) (feedCacheData => feedCacheData.Remove(feedId)));
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      base.ServiceStart(systemRequestContext);
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", FeedServiceFeedCache.FeedChangedNotificationId, new SqlNotificationHandler(this.OnFeedChanged), false);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Default", FeedServiceFeedCache.FeedChangedNotificationId, new SqlNotificationHandler(this.OnFeedChanged), false);
      base.ServiceEnd(systemRequestContext);
    }

    protected override FeedCacheData InitializeCache(IVssRequestContext requestContext) => new FeedCacheData(this.GetFeedsFromDatabase(requestContext, (ProjectReference) null, true));

    private IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeedsFromDatabase(
      IVssRequestContext requestContext,
      ProjectReference project,
      bool includeDeletedUpstreams)
    {
      using (FeedSqlResourceComponent component = requestContext.CreateComponent<FeedSqlResourceComponent>())
        return component.GetFeeds(project?.Id, includeDeletedUpstreams);
    }

    private static bool ShouldIncludeForAllFeedsProjectScopedRequest(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      ProjectReference project)
    {
      if (project == (ProjectReference) null)
        return true;
      return feed.Project != (ProjectReference) null && project.Id == feed.Project.Id;
    }

    private static bool ShouldIncludeForFeedProjectScopedRequest(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      ProjectReference project)
    {
      if (project == (ProjectReference) null)
        return feed.Project == (ProjectReference) null;
      return feed.Project != (ProjectReference) null && project.Id == feed.Project.Id;
    }

    private static Microsoft.VisualStudio.Services.Feed.WebApi.Feed CloneFeed(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      bool includeDeletedUpstreams = false)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed1 = FeedSqlResourceComponent.DeepCloneFeed(feed);
      if (feed1 != null && !includeDeletedUpstreams)
        feed1.UpstreamSources.RemoveDeletedUpstreamSources();
      return feed1;
    }

    private IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> CloneFeeds(
      IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> feedList,
      bool includeDeletedUpstreams = false)
    {
      return feedList.Select<Microsoft.VisualStudio.Services.Feed.WebApi.Feed, Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Microsoft.VisualStudio.Services.Feed.WebApi.Feed, Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) (feed => FeedServiceFeedCache.CloneFeed(feed, includeDeletedUpstreams)));
    }

    private static Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeedFromDatabase(
      IVssRequestContext requestContext,
      Func<FeedSqlResourceComponent, Microsoft.VisualStudio.Services.Feed.WebApi.Feed> getFeed)
    {
      using (FeedSqlResourceComponent component = requestContext.CreateComponent<FeedSqlResourceComponent>())
        return getFeed(component);
    }

    private static void UpdateCacheWithFeed(FeedCacheData cache, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      if (feed == null)
        return;
      cache[feed.Id] = feed;
    }

    private Microsoft.VisualStudio.Services.Feed.WebApi.Feed FaultInFeed(
      IVssRequestContext requestContext,
      Func<FeedSqlResourceComponent, Microsoft.VisualStudio.Services.Feed.WebApi.Feed> getFeed)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return this.Synchronize<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(requestContext, (Func<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) (() => FeedServiceFeedCache.GetFeedFromDatabase(requestContext, getFeed)), FeedServiceFeedCache.\u003C\u003EO.\u003C0\u003E__UpdateCacheWithFeed ?? (FeedServiceFeedCache.\u003C\u003EO.\u003C0\u003E__UpdateCacheWithFeed = new Action<FeedCacheData, Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(FeedServiceFeedCache.UpdateCacheWithFeed)));
    }

    private void OnFeedChanged(IVssRequestContext requestContext, NotificationEventArgs args)
    {
      if (string.IsNullOrEmpty(args.Data))
      {
        this.Invalidate(requestContext);
      }
      else
      {
        Guid result;
        if (Guid.TryParse(args.Data, out result))
        {
          this.Invalidate(requestContext, result);
          new FeedMessageBus().SendCachedFeedNoLongerValidNotification(requestContext, result);
        }
        else
        {
          requestContext.Trace(10019091, TraceLevel.Error, "Feed", "FeedServiceCache", "Invalid Feed Id: " + (args.Data ?? string.Empty));
          this.Invalidate(requestContext);
        }
      }
    }
  }
}
