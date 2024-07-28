// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Internal.FeedIndexEntryController
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Feed.Server.Internal
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("Artifact Details")]
  [VersionedApiControllerCustomName(Area = "Packaging", ResourceName = "IndexEntries")]
  [FeatureEnabled("Packaging.Feed.Service")]
  public class FeedIndexEntryController : FeedApiController
  {
    [HttpPost]
    [ClientResponseType(typeof (PackageIndexEntryResponse), null, null)]
    public HttpResponseMessage SetPackage(string feedId, PackageIndexEntry indexEntry)
    {
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      return this.Request.CreateResponse<PackageIndexEntryResponse>(HttpStatusCode.OK, this.FeedIndexService.SetIndexEntry(this.TfsRequestContext, feedService.GetFeed(tfsRequestContext, feedId1, projectReference).ThrowIfReadOnly(FeedSecurityHelper.CanBypassUnderMaintenance(this.TfsRequestContext)), indexEntry));
    }

    [HttpPatch]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage UpdatePackages(
      string feedId,
      List<PackageVersionIndexEntryUpdate> updates)
    {
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      this.FeedIndexService.UpdatePackageVersions(this.TfsRequestContext, feedService.GetFeed(tfsRequestContext, feedId1, projectReference).ThrowIfReadOnly(FeedSecurityHelper.CanBypassUnderMaintenance(this.TfsRequestContext)), updates.Select<PackageVersionIndexEntryUpdate, PackageVersionUpdate>((Func<PackageVersionIndexEntryUpdate, PackageVersionUpdate>) (x => new PackageVersionUpdate(x.PackageId, x.NormalizedVersion, x.SortableVersion, x.Metadata))));
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }
  }
}
