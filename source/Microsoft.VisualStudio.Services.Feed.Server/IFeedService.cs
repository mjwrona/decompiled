// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.IFeedService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [DefaultServiceImplementation(typeof (FeedService))]
  public interface IFeedService : IVssFrameworkService
  {
    Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> CreateFeed(
      IVssRequestContext requestContext,
      string feedName,
      string feedDescription,
      bool upstreamEnabled = false,
      bool allowUpstreamNameConflict = false,
      bool? hideDeletedPackageVersions = false,
      bool? badgesEnabled = false,
      IEnumerable<FeedPermission> feedPermission = null,
      IList<UpstreamSource> upstreamSources = null,
      ProjectReference project = null);

    IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeeds(
      IVssRequestContext requestContext,
      ProjectReference project,
      FeedRole feedRole = FeedRole.Reader,
      bool includeDeletedUpstreams = false);

    Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeed(
      IVssRequestContext requestContext,
      string feedId,
      ProjectReference project = null,
      bool includeDeleted = false,
      bool includeDeletedUpstreams = false);

    Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeedById(
      IVssRequestContext requestContext,
      Guid feedId,
      ProjectReference project,
      bool includeDeletedUpstreams = false);

    Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeedByIdForAnyScope(
      IVssRequestContext requestContext,
      Guid feedId,
      bool includeSoftDeletedFeeds = false);

    Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> UpdateFeed(
      IVssRequestContext requestContext,
      string feedId,
      FeedUpdate updatedFeed,
      ProjectReference project = null);

    void DeleteFeed(IVssRequestContext requestContext, string feedId, ProjectReference project);

    void DeleteAllFeedsInProject(IVssRequestContext requestContext, ProjectReference project);

    FeedInternalState GetFeedInternalState(
      IVssRequestContext requestContext,
      Guid feedId,
      ProjectReference project = null);
  }
}
