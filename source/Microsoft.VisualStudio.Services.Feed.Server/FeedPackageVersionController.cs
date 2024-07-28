// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedPackageVersionController
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("Artifact Details")]
  [VersionedApiControllerCustomName(Area = "Packaging", ResourceName = "Versions")]
  [FeatureEnabled("Packaging.Feed.Service")]
  public class FeedPackageVersionController : FeedApiController
  {
    [HttpGet]
    [PublicProjectRequestRestrictions]
    public PackageVersion GetPackageVersion(
      string feedId,
      string packageId,
      string packageVersionId,
      bool includeUrls = true,
      bool? isListed = null,
      bool? isDeleted = null)
    {
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = feedService.GetFeed(tfsRequestContext, feedId1, projectReference);
      PackageVersion packageVersion = this.FeedIndexService.GetPackageVersion(this.TfsRequestContext, feed, packageId, packageVersionId, isListed, isDeleted);
      if (includeUrls)
        packageVersion = packageVersion.IncludeUrls(this.TfsRequestContext, feed, packageId);
      return FeedModelSecuredObjectExtensions.SetSecuredObject(packageVersion, feed);
    }

    [HttpPatch]
    [ClientRequestContentMediaType("application/json-patch+json")]
    [RequestContentTypeRestriction(AllowJson = false)]
    [ClientResponseType(typeof (void), null, null)]
    [ClientInternalUseOnly(true)]
    public HttpResponseMessage UpdatePackageVersion(
      string feedId,
      string packageId,
      string packageVersionId,
      [ClientParameterType(typeof (JsonPatchDocument), false)] PatchDocument<PackageVersion> patchJson)
    {
      patchJson.ThrowIfNull<PatchDocument<PackageVersion>>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (patchJson))));
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = feedService.GetFeed(tfsRequestContext, feedId1, projectReference).ThrowIfReadOnly(FeedSecurityHelper.CanBypassUnderMaintenance(this.TfsRequestContext));
      PackageVersion packageVersion = this.FeedIndexService.GetPackageVersion(this.TfsRequestContext, feed, packageId, packageVersionId);
      patchJson.Apply(packageVersion);
      this.FeedIndexService.UpdatePackageVersion(this.TfsRequestContext, feed, packageId, packageVersionId, packageVersion);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    [HttpGet]
    [PublicProjectRequestRestrictions]
    public IEnumerable<PackageVersion> GetPackageVersions(
      string feedId,
      string packageId,
      bool includeUrls = true,
      bool? isListed = null,
      bool? isDeleted = null)
    {
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = feedService.GetFeed(tfsRequestContext, feedId1, projectReference);
      IEnumerable<PackageVersion> packageVersions = this.FeedIndexService.GetPackageVersions(this.TfsRequestContext, feed, packageId, isListed, isDeleted);
      if (includeUrls)
        packageVersions = packageVersions.Select<PackageVersion, PackageVersion>((Func<PackageVersion, PackageVersion>) (x => x.IncludeUrls(this.TfsRequestContext, feed, packageId)));
      return packageVersions.SetSecuredObject(feed);
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientInternalUseOnly(true)]
    public HttpResponseMessage DeletePackageVersion(
      string feedId,
      string packageId,
      string packageVersionId,
      DateTime deletedDate,
      DateTime scheduledPermanentDeleteDate)
    {
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      this.FeedIndexService.DeletePackageVersion(this.TfsRequestContext, feedService.GetFeed(tfsRequestContext, feedId1, projectReference).ThrowIfReadOnly(FeedSecurityHelper.CanBypassUnderMaintenance(this.TfsRequestContext)), packageId, packageVersionId, deletedDate.ToUniversalTime(), scheduledPermanentDeleteDate.ToUniversalTime());
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }
  }
}
