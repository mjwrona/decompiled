// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedChangeController
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("Change Tracking")]
  [VersionedApiControllerCustomName(Area = "Packaging", ResourceName = "FeedChanges")]
  [FeatureEnabled("Packaging.Feed.Service")]
  public class FeedChangeController : FeedApiController
  {
    [HttpGet]
    public async Task<FeedChange> GetFeedChange(string feedId)
    {
      FeedChangeController changeController = this;
      IFeedChangeService feedChangeService = changeController.FeedChangeService;
      IVssRequestContext tfsRequestContext = changeController.TfsRequestContext;
      // ISSUE: explicit non-virtual call
      ProjectInfo projectInfo = __nonvirtual (changeController.ProjectInfo);
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      string feedId1 = feedId;
      FeedChange feedChange = feedChangeService.GetFeedChange(tfsRequestContext, projectReference, feedId1);
      if (feedChange == null)
        throw FeedIdNotFoundException.Create(feedId);
      return await feedChange.IncludeUrlsAsync(changeController.TfsRequestContext);
    }

    [HttpGet]
    public async Task<FeedChangesResponse> GetFeedChanges(
      bool includeDeleted = false,
      long continuationToken = 0,
      int batchSize = 1000)
    {
      FeedChangeController changeController = this;
      IFeedChangeService feedChangeService = changeController.FeedChangeService;
      IVssRequestContext tfsRequestContext = changeController.TfsRequestContext;
      // ISSUE: explicit non-virtual call
      ProjectInfo projectInfo = __nonvirtual (changeController.ProjectInfo);
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      int num = includeDeleted ? 1 : 0;
      long continuationToken1 = continuationToken;
      int batchSize1 = batchSize;
      return await feedChangeService.GetFeedChanges(tfsRequestContext, projectReference, num != 0, continuationToken1, batchSize1).ToResponseAsync(changeController.TfsRequestContext, batchSize);
    }
  }
}
