// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.ClientServices.PlatformFeedClientService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common.ClientServices;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Server.ClientServices
{
  public class PlatformFeedClientService : IFeedClientService, IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public async Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeedAsync(
      IVssRequestContext requestContext,
      ProjectReference project,
      string feedId,
      bool includeDeletedUpstreams = false)
    {
      IFeedService service = requestContext.GetService<IFeedService>();
      IVssRequestContext requestContext1 = requestContext;
      ProjectReference projectReference = project;
      string feedId1 = feedId;
      ProjectReference project1 = projectReference;
      int num = includeDeletedUpstreams ? 1 : 0;
      return await service.GetFeed(requestContext1, feedId1, project1, includeDeletedUpstreams: num != 0).IncludeUrlsAsync(requestContext);
    }

    public async Task<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>> GetFeedsAsync(
      IVssRequestContext requestContext,
      ProjectReference project = null,
      FeedRole? feedRole = FeedRole.Reader,
      bool includeDeletedUpstreams = false)
    {
      return await requestContext.GetService<IFeedService>().GetFeeds(requestContext, project, (FeedRole) ((int) feedRole ?? 2), includeDeletedUpstreams).IncludeUrlsAsync(requestContext);
    }

    public Task<FeedView> GetFeedViewAsync(
      IVssRequestContext requestContext,
      FeedCore feedCore,
      string viewId)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = requestContext.GetService<IFeedService>().GetFeed(requestContext, feedCore.Id.ToString(), feedCore.Project).ThrowIfViewNotation();
      return Task.FromResult<FeedView>(requestContext.GetService<IFeedViewService>().GetView(requestContext, feed, viewId).IncludeUrls(requestContext, feed));
    }

    public async Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> CreateFeedAsync(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      return await requestContext.GetService<IFeedService>().CreateFeed(requestContext, feed.Name, feed.Description, feed.UpstreamEnabled, feed.AllowUpstreamNameConflict, new bool?(feed.HideDeletedPackageVersions), new bool?(feed.BadgesEnabled), feed.Permissions, feed.UpstreamSources, feed.Project);
    }

    public Task DeleteFeedViewAsync(
      IVssRequestContext requestContext,
      FeedCore feedCore,
      string viewId)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = requestContext.GetService<IFeedService>().GetFeed(requestContext, feedCore.Id.ToString(), feedCore.Project).ThrowIfViewNotation();
      requestContext.GetService<IFeedViewService>().DeleteView(requestContext, feed, viewId);
      return Task.CompletedTask;
    }

    public Task<List<FeedView>> GetFeedViewsAsync(
      IVssRequestContext requestContext,
      FeedCore feedCore)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = requestContext.GetService<IFeedService>().GetFeed(requestContext, feedCore.Id.ToString(), feedCore.Project).ThrowIfViewNotation();
      return Task.FromResult<List<FeedView>>(requestContext.GetService<IFeedViewService>().GetViews(requestContext, feed).Select<FeedView, FeedView>((Func<FeedView, FeedView>) (v => v.IncludeUrls(requestContext, feed))).ToList<FeedView>());
    }
  }
}
