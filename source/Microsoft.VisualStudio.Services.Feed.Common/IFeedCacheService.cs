// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.IFeedCacheService
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Common
{
  [DefaultServiceImplementation(typeof (FeedCacheService))]
  public interface IFeedCacheService : IVssFrameworkService
  {
    FeedCore AddFeed(
      IVssRequestContext requestContext,
      Guid projectId,
      string feedNameOrId,
      FeedCore feed);

    FeedCore GetFeed(IVssRequestContext requestContext, Guid projectId, string feedNameOrId);

    Task<FeedCore> GetFeedAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      string feedNameOrId);

    void RemoveFeed(IVssRequestContext requestContext, Guid projectId, string feedNameOrId);

    Task<FeedCore> GetFeedByIdFromAnyScopeAsync(
      IVssRequestContext requestContext,
      Guid feedId,
      Func<FeedCore, bool> rejectCachedFeedIf = null);
  }
}
