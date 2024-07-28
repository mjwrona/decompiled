// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageVersionProvenanceController
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [ControllerApiVersion(5.0)]
  [ClientGroupByResource("Artifact Details")]
  [VersionedApiControllerCustomName(Area = "Packaging", ResourceName = "Provenance")]
  [FeatureEnabled("Packaging.Feed.Service")]
  public class PackageVersionProvenanceController : FeedApiController
  {
    private ITimeProvider timeProvider;

    public PackageVersionProvenanceController()
      : this((ITimeProvider) new DefaultTimeProvider())
    {
    }

    public PackageVersionProvenanceController(ITimeProvider timeProvider) => this.timeProvider = timeProvider;

    [HttpGet]
    [ClientSwaggerOperationId("GetPackageVersionProvenance")]
    [PublicProjectRequestRestrictions]
    public PackageVersionProvenance GetPackageVersionProvenance(
      string feedId,
      Guid packageId,
      Guid packageVersionId)
    {
      feedId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feedId))));
      if (packageId.Equals(Guid.Empty))
        throw InvalidUserInputException.Create(nameof (packageId));
      if (packageVersionId.Equals(Guid.Empty))
        throw InvalidUserInputException.Create(nameof (packageVersionId));
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = feedService.GetFeed(tfsRequestContext, feedId1, projectReference);
      return this.ProvenanceService.GetPackageVersionProvenance(this.TfsRequestContext, feed, packageId, packageVersionId).SetSecuredObject(feed);
    }
  }
}
