// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.IFeedViewService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [DefaultServiceImplementation(typeof (FeedViewService))]
  public interface IFeedViewService : IVssFrameworkService
  {
    FeedView CreateView(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, FeedView view);

    FeedView GetView(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, string viewId);

    IEnumerable<FeedView> GetViews(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed);

    FeedView UpdateView(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string viewId,
      FeedView updatedView);

    void DeleteView(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, string viewId);

    IReadOnlyDictionary<Guid, IReadOnlyList<FeedView>> GetAllNonDeletedFeedViewsForCollection(
      IVssRequestContext requestContext);

    IReadOnlyDictionary<Guid, IReadOnlyList<FeedView>> GetFeedViewsNoSecurityCheck(
      IVssRequestContext requestContext,
      IReadOnlyCollection<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> feeds);
  }
}
