// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedRecycleBinPackagesController
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("Recycle Bin")]
  [VersionedApiControllerCustomName(Area = "Packaging", ResourceName = "RecycleBinPackages")]
  [FeatureEnabled("Packaging.Feed.Service")]
  public class FeedRecycleBinPackagesController : FeedApiController
  {
    [HttpGet]
    public Package GetRecycleBinPackage(string feedId, Guid packageId, bool includeUrls = true)
    {
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = feedService.GetFeed(tfsRequestContext, feedId1, projectReference).ThrowIfViewNotation();
      Package package = this.PackageRecycleBinService.GetPackage(this.TfsRequestContext, feed, packageId);
      if (includeUrls)
        package.IncludeRecycleBinUrls(this.TfsRequestContext, feed);
      return package;
    }

    [HttpGet]
    public IEnumerable<Package> GetRecycleBinPackages(
      string feedId,
      string protocolType = null,
      string packageNameQuery = null,
      bool includeUrls = true,
      [FromUri(Name = "$top")] int top = 1000,
      [FromUri(Name = "$skip")] int skip = 0,
      bool includeAllVersions = false)
    {
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = feedService.GetFeed(tfsRequestContext, feedId1, projectReference);
      IEnumerable<Package> source = this.PackageRecycleBinService.GetPackages(this.TfsRequestContext, feed, protocolType, packageNameQuery, top, skip, includeAllVersions);
      if (includeUrls)
        source = source.Select<Package, Package>((Func<Package, Package>) (p => p.IncludeRecycleBinUrls(this.TfsRequestContext, feed)));
      return source;
    }

    [HttpDelete]
    [ClientResponseType(typeof (OperationReference), null, null)]
    public HttpResponseMessage EmptyRecycleBin(string feedId)
    {
      if (this.TfsRequestContext.IsFeatureEnabled("Packaging.Feed.DisableEmptyRecycleBin"))
        return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "The Empty Recycle Bin Feature is currently disabled.");
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      return this.Request.CreateResponse<OperationReference>(HttpStatusCode.Accepted, JobOperationsUtility.GetOperationReference(this.TfsRequestContext, this.PackageRecycleBinService.QueueEmptyRecycleBin(this.TfsRequestContext, feedService.GetFeed(tfsRequestContext, feedId1, projectReference))));
    }
  }
}
