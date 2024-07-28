// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedCapabilitiesController
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "Packaging", ResourceName = "Capabilities")]
  [FeatureEnabled("Packaging.Feed.Service")]
  [ClientInternalUseOnly(true)]
  public class FeedCapabilitiesController : FeedApiController
  {
    [HttpGet]
    public FeedCapabilities GetCapabilities(Guid feedId)
    {
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId.ToString();
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = feedService.GetFeed(tfsRequestContext, feedId1, projectReference);
      return this.TfsRequestContext.GetService<IFeedCapabilitiesService>().GetCapabilities(this.TfsRequestContext, feed);
    }

    [HttpPut]
    [ClientResponseType(typeof (void), null, null)]
    public async Task<HttpResponseMessage> UpdateCapabilityAsync(
      Guid feedId,
      [FromBody] FeedCapabilities capabilities)
    {
      FeedCapabilitiesController capabilitiesController = this;
      IFeedService feedService = capabilitiesController.FeedService;
      IVssRequestContext tfsRequestContext = capabilitiesController.TfsRequestContext;
      string feedId1 = feedId.ToString();
      // ISSUE: explicit non-virtual call
      ProjectInfo projectInfo = __nonvirtual (capabilitiesController.ProjectInfo);
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = feedService.GetFeed(tfsRequestContext, feedId1, projectReference);
      if (await capabilitiesController.TfsRequestContext.GetService<IFeedCapabilitiesService>().UpdateCapabilityAsync(capabilitiesController.TfsRequestContext, feed, capabilities))
        return capabilitiesController.Request.CreateResponse(HttpStatusCode.Accepted);
      capabilitiesController.TfsRequestContext.TraceAlways(10019125, TraceLevel.Error, "Feed", "Service", string.Format("UPGRADEFEEDFAIL -  {0} Capabilities {1} service feed {2} service capabilities {3}", (object) feedId.ToString(), (object) ((uint) capabilities).ToString("X4"), (object) feed.Id, (object) ((uint) feed.Capabilities).ToString("X4")));
      return capabilitiesController.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Resources.UpgradeNotSupported());
    }
  }
}
