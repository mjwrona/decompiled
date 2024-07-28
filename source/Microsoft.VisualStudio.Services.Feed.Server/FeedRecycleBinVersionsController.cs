// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedRecycleBinVersionsController
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("Recycle Bin")]
  [VersionedApiControllerCustomName(Area = "Packaging", ResourceName = "RecycleBinVersions")]
  [FeatureEnabled("Packaging.Feed.Service")]
  public class FeedRecycleBinVersionsController : FeedApiController
  {
    [HttpGet]
    public RecycleBinPackageVersion GetRecycleBinPackageVersion(
      string feedId,
      Guid packageId,
      Guid packageVersionId,
      bool includeUrls = true)
    {
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = feedService.GetFeed(tfsRequestContext, feedId1, projectReference);
      RecycleBinPackageVersion packageVersion = this.PackageRecycleBinService.GetPackageVersion(this.TfsRequestContext, feed, packageId, packageVersionId);
      if (includeUrls)
        packageVersion = FeedModelExtensions.IncludeUrls(packageVersion, this.TfsRequestContext, feed, packageId.ToString());
      return packageVersion;
    }

    [HttpGet]
    public IEnumerable<RecycleBinPackageVersion> GetRecycleBinPackageVersions(
      string feedId,
      Guid packageId,
      bool includeUrls = true)
    {
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = feedService.GetFeed(tfsRequestContext, feedId1, projectReference);
      IEnumerable<RecycleBinPackageVersion> source = this.PackageRecycleBinService.GetPackageVersions(this.TfsRequestContext, feed, packageId);
      if (includeUrls)
        source = source.Select<RecycleBinPackageVersion, RecycleBinPackageVersion>((Func<RecycleBinPackageVersion, RecycleBinPackageVersion>) (p => FeedModelExtensions.IncludeUrls(p, this.TfsRequestContext, feed, packageId.ToString())));
      return source;
    }

    [HttpDelete]
    [ClientInternalUseOnly(true)]
    public void PermanentlyDeletePackageVersion(
      string feedId,
      Guid packageId,
      Guid packageVersionId)
    {
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      this.PackageRecycleBinService.PermanentlyDeletePackageVersion(this.TfsRequestContext, feedService.GetFeed(tfsRequestContext, feedId1, projectReference).ThrowIfReadOnly(FeedSecurityHelper.CanBypassUnderMaintenance(this.TfsRequestContext)), packageId, packageVersionId);
    }

    [HttpPatch]
    [ClientRequestContentMediaType("application/json-patch+json")]
    [RequestContentTypeRestriction(AllowJson = false)]
    [ClientInternalUseOnly(true)]
    public void UpdateRecycleBinPackageVersion(
      string feedId,
      Guid packageId,
      Guid packageVersionId,
      [ClientParameterType(typeof (JsonPatchDocument), false)] PatchDocument<PackageVersion> patchJson)
    {
      patchJson.ThrowIfNull<PatchDocument<PackageVersion>>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (patchJson))));
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = feedService.GetFeed(tfsRequestContext, feedId1, projectReference).ThrowIfReadOnly(FeedSecurityHelper.CanBypassUnderMaintenance(this.TfsRequestContext));
      List<IPatchOperation<PackageVersion>> list = patchJson.Operations.ToList<IPatchOperation<PackageVersion>>();
      if (list.Count == 1 && !(list[0].Path != "/isDeleted") && list[0].Operation == Operation.Replace)
      {
        bool? nullable = list[0].Value as bool?;
        bool flag = false;
        if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
        {
          this.PackageRecycleBinService.RestorePackageVersionToFeed(this.TfsRequestContext, feed, packageId, packageVersionId);
          return;
        }
      }
      throw new NotSupportedException(Resources.Error_RecycleBinUpdateOnlySupportsRestoreToFeed());
    }
  }
}
