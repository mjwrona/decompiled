// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.IFeedService
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public interface IFeedService
  {
    IEnumerable<FeedCore> GetFeeds(bool includeSoftDeletedFeeds = false);

    Task<IEnumerable<FeedCore>> GetFeedsAsync(bool includeSoftDeletedFeeds = false);

    FeedView GetView(FeedCore feed, string viewId);

    FeedCore GetFeed(Guid projectId, string feedId);

    Task<FeedCore> GetFeedAsync(Guid projectId, string feedId);

    FeedCore GetFeedByIdForAnyScope(Guid feedId, bool includeSoftDeletedFeeds = false);

    Task<FeedCore> GetFeedByIdForAnyScopeAsync(
      Guid feedId,
      bool includeSoftDeletedFeeds = false,
      Func<FeedCore, bool> rejectCachedFeedIf = null);

    Task<FeedView> GetLocalViewOrDefaultAsync(FeedCore feed);

    Task<IReadOnlyList<FeedView>> GetViewsAsync(FeedCore feed);
  }
}
