// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedPackageController
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("Artifact Details")]
  [VersionedApiControllerCustomName(Area = "Packaging", ResourceName = "Packages")]
  [FeatureEnabled("Packaging.Feed.Service")]
  public class FeedPackageController : FeedApiController
  {
    [HttpGet]
    [PublicProjectRequestRestrictions]
    public Package GetPackage(
      string feedId,
      string packageId,
      bool includeAllVersions = false,
      bool includeUrls = true,
      bool? isListed = null,
      bool? isRelease = null,
      bool includeDeleted = false,
      bool includeDescription = false)
    {
      bool? nullable = includeDeleted ? new bool?() : new bool?(false);
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext1 = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed1 = feedService.GetFeed(tfsRequestContext1, feedId1, projectReference);
      IFeedIndexService feedIndexService = this.FeedIndexService;
      IVssRequestContext tfsRequestContext2 = this.TfsRequestContext;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed2 = feed1;
      string packageId1 = packageId;
      ResultOptions resultOptions = new ResultOptions();
      resultOptions.IncludeAllVersions = includeAllVersions;
      resultOptions.IncludeDescriptions = includeDescription;
      bool? isListed1 = isListed;
      bool? isRelease1 = isRelease;
      bool? isDeleted = nullable;
      Package package = feedIndexService.GetPackage(tfsRequestContext2, feed2, packageId1, resultOptions, isListed1, isRelease1, isDeleted);
      if (includeUrls)
        package.IncludeUrls(this.TfsRequestContext, feed1);
      return FeedModelSecuredObjectExtensions.SetSecuredObject(package, feed1);
    }

    [HttpGet]
    [PublicProjectRequestRestrictions]
    public IEnumerable<Package> GetPackages(
      string feedId,
      string protocolType = null,
      string packageNameQuery = null,
      string normalizedPackageName = null,
      bool includeUrls = true,
      bool includeAllVersions = false,
      bool? isListed = null,
      bool getTopPackageVersions = false,
      bool? isRelease = null,
      bool includeDescription = false,
      [FromUri(Name = "$top")] int top = 1000,
      [FromUri(Name = "$skip")] int skip = 0,
      bool includeDeleted = false,
      bool? isCached = null,
      Guid? directUpstreamId = null)
    {
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext1 = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = feedService.GetFeed(tfsRequestContext1, feedId1, projectReference);
      bool? nullable = includeDeleted ? new bool?() : new bool?(false);
      IFeedIndexService feedIndexService = this.FeedIndexService;
      IVssRequestContext tfsRequestContext2 = this.TfsRequestContext;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed1 = feed;
      string protocolType1 = protocolType;
      string nameQuery = packageNameQuery;
      string normalizedPackageName1 = normalizedPackageName;
      PagingOptions pagingOptions = new PagingOptions();
      pagingOptions.Top = top;
      pagingOptions.Skip = skip;
      ResultOptions resultOptions = new ResultOptions();
      resultOptions.IncludeAllVersions = includeAllVersions;
      resultOptions.IncludeDescriptions = includeDescription;
      bool? isListed1 = isListed;
      int num = getTopPackageVersions ? 1 : 0;
      bool? isRelease1 = isRelease;
      bool? isDeleted = nullable;
      bool? isCached1 = isCached;
      Guid? directUpstreamSourceId = directUpstreamId;
      IEnumerable<Package> packages = feedIndexService.GetPackages(tfsRequestContext2, feed1, protocolType1, nameQuery, normalizedPackageName1, pagingOptions, resultOptions, isListed1, num != 0, isRelease1, isDeleted, isCached1, directUpstreamSourceId);
      if (includeUrls)
        packages = packages.Select<Package, Package>((Func<Package, Package>) (package => package.IncludeUrls(this.TfsRequestContext, feed)));
      return packages.SetSecuredObject(feed);
    }
  }
}
