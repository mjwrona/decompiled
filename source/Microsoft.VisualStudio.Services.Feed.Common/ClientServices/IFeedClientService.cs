// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.ClientServices.IFeedClientService
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Common.ClientServices
{
  [DefaultServiceImplementation(typeof (FrameworkFeedClientService))]
  public interface IFeedClientService : IVssFrameworkService
  {
    Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeedAsync(
      IVssRequestContext requestContext,
      ProjectReference project,
      string feedId,
      bool includeDeletedUpstreams = false);

    Task<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>> GetFeedsAsync(
      IVssRequestContext requestContext,
      ProjectReference project = null,
      FeedRole? feedRole = FeedRole.Reader,
      bool includeDeletedUpstreams = false);

    Task<List<FeedView>> GetFeedViewsAsync(IVssRequestContext requestContext, FeedCore feed);

    Task<FeedView> GetFeedViewAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string viewId);

    Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> CreateFeedAsync(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed);

    Task DeleteFeedViewAsync(IVssRequestContext requestContext, FeedCore feed, string viewId);
  }
}
