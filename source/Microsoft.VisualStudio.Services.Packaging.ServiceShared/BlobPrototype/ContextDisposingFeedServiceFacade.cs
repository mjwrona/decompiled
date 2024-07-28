// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.ContextDisposingFeedServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class ContextDisposingFeedServiceFacade : IDisposableFeedService, IFeedService, IDisposable
  {
    private readonly IVssRequestContext requestContextToDispose;
    private readonly IFeedService backingFeedService;

    public ContextDisposingFeedServiceFacade(
      IVssRequestContext requestContextToDispose,
      IFeedService backingFeedService)
    {
      this.requestContextToDispose = requestContextToDispose;
      this.backingFeedService = backingFeedService;
    }

    public FeedCore GetFeed(Guid projectId, string feedId) => this.backingFeedService.GetFeed(projectId, feedId);

    public async Task<FeedCore> GetFeedAsync(Guid projectId, string feedId) => await this.backingFeedService.GetFeedAsync(projectId, feedId);

    public FeedCore GetFeedByIdForAnyScope(Guid feedId, bool includeSoftDeletedFeeds = false) => this.backingFeedService.GetFeedByIdForAnyScope(feedId, includeSoftDeletedFeeds);

    public async Task<FeedCore> GetFeedByIdForAnyScopeAsync(
      Guid feedId,
      bool includeSoftDeletedFeeds = false,
      Func<FeedCore, bool> rejectCachedFeedIf = null)
    {
      return await this.backingFeedService.GetFeedByIdForAnyScopeAsync(feedId, includeSoftDeletedFeeds, rejectCachedFeedIf);
    }

    public async Task<FeedView> GetLocalViewOrDefaultAsync(FeedCore feed) => await this.backingFeedService.GetLocalViewOrDefaultAsync(feed);

    public async Task<IReadOnlyList<FeedView>> GetViewsAsync(FeedCore feed) => await this.backingFeedService.GetViewsAsync(feed);

    public IEnumerable<FeedCore> GetFeeds(bool includeSoftDeletedFeeds = false) => this.backingFeedService.GetFeeds(includeSoftDeletedFeeds);

    public Task<IEnumerable<FeedCore>> GetFeedsAsync(bool includeSoftDeletedFeeds = false) => this.backingFeedService.GetFeedsAsync(includeSoftDeletedFeeds);

    public FeedView GetView(FeedCore feed, string viewId) => this.backingFeedService.GetView(feed, viewId);

    public void Dispose() => this.requestContextToDispose.Dispose();
  }
}
