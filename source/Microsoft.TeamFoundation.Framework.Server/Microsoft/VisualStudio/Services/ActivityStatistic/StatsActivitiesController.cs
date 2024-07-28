// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ActivityStatistic.StatsActivitiesController
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ActivityStatistic
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Stats", ResourceName = "Activities")]
  public class StatsActivitiesController : TfsApiController
  {
    [HttpGet]
    public HttpResponseMessage GetActivityStatistics(Guid activityId)
    {
      if (!this.TfsRequestContext.ExecutionEnvironment.IsDevFabricDeployment && !this.IsDebugBits() && !this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.WebAccess.StatsCollection"))
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      ArgumentUtility.CheckForEmptyGuid(activityId, nameof (activityId));
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      return this.Request.CreateResponse<IEnumerable<Microsoft.TeamFoundation.Framework.Server.ActivityStatistic>>(HttpStatusCode.OK, vssRequestContext.GetService<TeamFoundationActivityStatisticService>().QueryActivityLogEntries(vssRequestContext, activityId));
    }

    public bool IsDebugBits() => false;

    public override string TraceArea => "ActivityStatisticsService";

    public override string ActivityLogArea => "Framework";
  }
}
