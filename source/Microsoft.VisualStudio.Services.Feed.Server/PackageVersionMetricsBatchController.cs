// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageVersionMetricsBatchController
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [ControllerApiVersion(5.0)]
  [ClientGroupByResource("Artifact Details")]
  [VersionedApiControllerCustomName(Area = "Packaging", ResourceName = "VersionMetricsBatch")]
  public class PackageVersionMetricsBatchController : FeedApiController
  {
    [HttpPost]
    [PublicProjectRequestRestrictions]
    public IEnumerable<PackageVersionMetrics> QueryPackageVersionMetrics(
      string feedId,
      Guid packageId,
      PackageVersionMetricsQuery packageVersionIdQuery)
    {
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = feedService.GetFeed(tfsRequestContext, feedId1, projectReference);
      return this.FeedMetricsService.QueryPackageVersionMetrics(this.TfsRequestContext, feed, packageId, packageVersionIdQuery).SetSecuredObject(feed);
    }
  }
}
