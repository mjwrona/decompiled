// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.Controllers.BuildTimelinesController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer.Controllers
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "Timeline", ResourceVersion = 2)]
  public class BuildTimelinesController : BuildApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (Timeline), null, null)]
    public virtual HttpResponseMessage GetBuildTimeline(
      int buildId,
      Guid? timelineId = null,
      int? changeId = null,
      Guid? planId = null)
    {
      return this.GetBuildTimeline(this.InternalBuildService.GetBuildById(this.TfsRequestContext, buildId, includeDeleted: true) ?? throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId)), timelineId, changeId, planId);
    }

    private HttpResponseMessage GetBuildTimeline(
      BuildData build,
      Guid? timelineId,
      int? changeId,
      Guid? planId)
    {
      TimelineData? timelineData = build.GetTimelineData(this.TfsRequestContext, timelineId, changeId, planId);
      return !timelineData.HasValue || timelineData.Value.Timeline == null ? this.Request.CreateResponse(timelineId.HasValue ? HttpStatusCode.NotFound : HttpStatusCode.NoContent) : this.Request.CreateResponse<Timeline>(HttpStatusCode.OK, timelineData.Value.Timeline);
    }
  }
}
