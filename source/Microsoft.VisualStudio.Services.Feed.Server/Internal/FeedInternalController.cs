// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Internal.FeedInternalController
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Feed.Server.Internal
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "Packaging", ResourceName = "FeedsInternal")]
  [FeatureEnabled("Packaging.Feed.Service")]
  public class FeedInternalController : FeedApiController
  {
    [HttpGet]
    public async Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeedByIdForAnyScope(
      Guid feedId,
      bool includeSoftDeletedFeeds = false)
    {
      FeedInternalController internalController = this;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feedByIdForAnyScope1 = internalController.FeedService.GetFeedByIdForAnyScope(internalController.TfsRequestContext, feedId, includeSoftDeletedFeeds);
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feedByIdForAnyScope2;
      if (MediaTypeFormatUtility.GetExcludeUrlsAcceptHeaderOptionValue(internalController.Request))
        feedByIdForAnyScope2 = feedByIdForAnyScope1;
      else
        feedByIdForAnyScope2 = await feedByIdForAnyScope1.IncludeUrlsAsync(internalController.TfsRequestContext);
      return feedByIdForAnyScope2;
    }
  }
}
