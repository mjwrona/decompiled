// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.ClientServices.FrameworkFeedClientService
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Common.ClientServices
{
  public class FrameworkFeedClientService : IFeedClientService, IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeedAsync(
      IVssRequestContext requestContext,
      ProjectReference project,
      string feedId,
      bool includeDeletedUpstreams = false)
    {
      FeedHttpClient client = requestContext.GetClient<FeedHttpClient>();
      client.ExcludeUrlsHeader = true;
      Guid projectIdOrEmptyGuid = project.ToProjectIdOrEmptyGuid();
      return !(projectIdOrEmptyGuid != Guid.Empty) ? client.GetFeedAsync(feedId, new bool?(includeDeletedUpstreams), (object) null, new CancellationToken()) : client.GetFeedAsync(projectIdOrEmptyGuid, feedId, new bool?(includeDeletedUpstreams));
    }

    public Task<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>> GetFeedsAsync(
      IVssRequestContext requestContext,
      ProjectReference project = null,
      FeedRole? feedRole = FeedRole.Reader,
      bool includeDeletedUpstreams = false)
    {
      FeedHttpClient client = requestContext.GetClient<FeedHttpClient>();
      client.ExcludeUrlsHeader = true;
      Guid projectIdOrEmptyGuid = project.ToProjectIdOrEmptyGuid();
      return !(projectIdOrEmptyGuid != Guid.Empty) ? client.GetFeedsAsync(feedRole, new bool?(includeDeletedUpstreams)) : client.GetFeedsAsync(projectIdOrEmptyGuid, feedRole, new bool?(includeDeletedUpstreams));
    }

    public Task<FeedView> GetFeedViewAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string viewId)
    {
      return requestContext.GetClient<FeedHttpClient>().GetFeedViewAsync(feed.GetProjectIdentifierString(), feed.Id.ToString(), viewId);
    }

    public Task<List<FeedView>> GetFeedViewsAsync(IVssRequestContext requestContext, FeedCore feed) => requestContext.GetClient<FeedHttpClient>().GetFeedViewsAsync(feed.GetProjectIdentifierString(), feed.Id.ToString());

    public async Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> CreateFeedAsync(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      return await requestContext.GetClient<FeedHttpClient>().CreateFeedAsync(feed, feed.GetProjectIdentifierString());
    }

    public async Task DeleteFeedViewAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string viewId)
    {
      await requestContext.GetClient<FeedHttpClient>().DeleteFeedViewAsync(feed.GetProjectIdentifierString(), feed.Id.ToString(), viewId);
    }
  }
}
