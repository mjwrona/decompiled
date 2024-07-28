// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.FeedServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Common.ClientServices;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class FeedServiceFacade : IFeedService
  {
    private readonly IVssRequestContext requestContext;

    public FeedServiceFacade(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IEnumerable<FeedCore> GetFeeds(bool includeSoftDeletedFeeds = false) => this.requestContext.RunSynchronously<IEnumerable<FeedCore>>((Func<Task<IEnumerable<FeedCore>>>) (async () => await this.GetFeedsAsync(includeSoftDeletedFeeds)));

    public async Task<IEnumerable<FeedCore>> GetFeedsAsync(bool includeSoftDeletedFeeds = false)
    {
      List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> softDeletedFeeds = new List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>();
      if (includeSoftDeletedFeeds)
      {
        try
        {
          softDeletedFeeds = await this.requestContext.GetService<IFeedRecycleBinClientService>().GetFeedsFromRecycleBinAsync(this.requestContext);
        }
        catch (Exception ex) when (
        {
          // ISSUE: unable to correctly present filter
          int num;
          switch (ex)
          {
            case FeatureDisabledException _:
            case FeatureOffException _:
            case MissingFeatureException _:
            case InvalidFeatureFlagStateValueException _:
              num = 1;
              break;
            default:
              num = ex is VssResourceNotFoundException ? 1 : 0;
              break;
          }
          if ((uint) num > 0U)
          {
            SuccessfulFiltering;
          }
          else
            throw;
        }
        )
        {
        }
      }
      IEnumerable<FeedCore> feedsAsync = (IEnumerable<FeedCore>) (await this.requestContext.GetService<IFeedClientService>().GetFeedsAsync(this.requestContext)).Concat<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) softDeletedFeeds);
      softDeletedFeeds = (List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) null;
      return feedsAsync;
    }

    public async Task<FeedView> GetLocalViewOrDefaultAsync(FeedCore feed)
    {
      try
      {
        FeedCore feedAsync = await this.GetFeedAsync(feed.Project.ToProjectIdOrEmptyGuid(), string.Format("{0}@{1}", (object) feed.Id, (object) "Local"));
        return feedAsync.View.Type == FeedViewType.Implicit ? feedAsync.View : (FeedView) null;
      }
      catch (FeedViewNotFoundException ex)
      {
        return (FeedView) null;
      }
    }

    public FeedView GetView(FeedCore feed, string viewId) => this.requestContext.RunSynchronously<FeedView>((Func<Task<FeedView>>) (async () => await this.requestContext.GetService<IFeedClientService>().GetFeedViewAsync(this.requestContext, feed, viewId)));

    public async Task<IReadOnlyList<FeedView>> GetViewsAsync(FeedCore feed) => (IReadOnlyList<FeedView>) await this.requestContext.GetService<IFeedClientService>().GetFeedViewsAsync(this.requestContext, feed);

    public FeedCore GetFeed(Guid projectId, string feedId) => this.requestContext.GetService<IFeedCacheService>().GetFeed(this.requestContext, projectId, feedId);

    public FeedCore GetFeedByIdForAnyScope(Guid feedId, bool includeSoftDeletedFeeds = false) => AsyncPump.Run<FeedCore>((Func<Task<FeedCore>>) (() => this.GetFeedByIdForAnyScopeAsync(feedId, includeSoftDeletedFeeds, (Func<FeedCore, bool>) null)));

    public async Task<FeedCore> GetFeedByIdForAnyScopeAsync(
      Guid feedId,
      bool includeSoftDeletedFeeds = false,
      Func<FeedCore, bool> rejectCachedFeedIf = null)
    {
      if (!includeSoftDeletedFeeds)
        return await this.requestContext.GetService<IFeedCacheService>().GetFeedByIdFromAnyScopeAsync(this.requestContext, feedId, rejectCachedFeedIf);
      try
      {
        return (FeedCore) await this.requestContext.GetService<IFeedInternalClientService>().GetFeedByIdForAnyScopeAsync(this.requestContext, feedId, includeSoftDeletedFeeds);
      }
      catch (VssServiceResponseException ex) when (ex.HttpStatusCode == HttpStatusCode.Forbidden)
      {
        throw new UnauthorizedAccessException("GetFeedByIdForAnyScopeAsync returned status code Forbidden", (Exception) ex);
      }
    }

    public Task<FeedCore> GetFeedAsync(Guid projectId, string feedId) => this.requestContext.GetService<IFeedCacheService>().GetFeedAsync(this.requestContext, projectId, feedId);
  }
}
