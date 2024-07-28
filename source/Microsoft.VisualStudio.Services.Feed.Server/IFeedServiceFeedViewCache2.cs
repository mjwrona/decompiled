// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.IFeedServiceFeedViewCache2
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [DefaultServiceImplementation(typeof (FeedServiceFeedViewCache2))]
  internal interface IFeedServiceFeedViewCache2 : IVssFrameworkService
  {
    FeedView? GetFeedView(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, string viewName);

    FeedView? GetFeedView(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, Guid viewId);

    IEnumerable<FeedView> GetFeedViews(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed);

    void InvalidateFeedViews(IVssRequestContext requestContext, Guid feedId);

    IReadOnlyDictionary<Guid, IReadOnlyList<FeedView>> GetAllNonDeletedViewsByFeedForCollection(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>? knownExistingFeeds = null);

    IReadOnlyDictionary<Guid, IReadOnlyList<FeedView>> GetFeedViews(
      IVssRequestContext requestContext,
      IReadOnlyCollection<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> feeds);
  }
}
