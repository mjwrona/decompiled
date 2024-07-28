// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Internal.CachedPackagesController
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Feed.Server.Internal
{
  [ControllerApiVersion(3.1)]
  [VersionedApiControllerCustomName(Area = "Packaging", ResourceName = "CachedPackages")]
  [FeatureEnabled("Packaging.Feed.Service")]
  public class CachedPackagesController : FeedApiController
  {
    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientInternalUseOnly(false)]
    public HttpResponseMessage ClearPackageCache(
      string feedId,
      string protocolType,
      string upstreamId)
    {
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      this.FeedIndexService.ClearCachedPackages(this.TfsRequestContext, feedService.GetFeed(tfsRequestContext, feedId1, projectReference).ThrowIfReadOnly(FeedSecurityHelper.CanBypassUnderMaintenance(this.TfsRequestContext)), protocolType);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }
  }
}
