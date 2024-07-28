// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.IFeedServiceFeedCache
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [DefaultServiceImplementation(typeof (FeedServiceFeedCache))]
  internal interface IFeedServiceFeedCache : IVssFrameworkService
  {
    Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeed(
      IVssRequestContext requestContext,
      string feedName,
      ProjectReference project = null,
      bool includeDeletedUpstreams = false);

    Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeed(
      IVssRequestContext requestContext,
      Guid id,
      ProjectReference project = null,
      bool includeDeletedUpstreams = false);

    Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeedByIdForAnyScope(
      IVssRequestContext requestContext,
      Guid id);

    IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeeds(
      IVssRequestContext requestContext,
      ProjectReference project,
      bool includeDeletedUpstreams = false);

    void Invalidate(IVssRequestContext requestContext);

    void Invalidate(IVssRequestContext requestContext, Guid feedId);
  }
}
