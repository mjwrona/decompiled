// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedBatchController
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "Packaging", ResourceName = "FeedBatch")]
  [FeatureEnabled("Packaging.Feed.Service")]
  [ClientInternalUseOnly(true)]
  public class FeedBatchController : FeedApiController
  {
    [HttpPut]
    public void RunBatchAsync(Guid feedId, [FromBody] FeedBatchData data)
    {
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId.ToString();
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = feedService.GetFeed(tfsRequestContext, feedId1, projectReference);
      this.TfsRequestContext.GetService<IFeedBatchService>().RunBatch(this.TfsRequestContext, feed, data);
    }
  }
}
