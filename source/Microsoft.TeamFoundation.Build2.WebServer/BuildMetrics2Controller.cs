// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildMetrics2Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(3.1)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "Metrics", ResourceVersion = 1)]
  public class BuildMetrics2Controller : BuildMetricsController
  {
    [HttpGet]
    [ClientLocationId("7433FAE7-A6BC-41DC-A6E2-EEF9005CE41A")]
    public virtual List<Microsoft.TeamFoundation.Build.WebApi.BuildMetric> GetProjectMetrics(
      string metricAggregationType = "Daily",
      DateTime? minMetricsTime = null)
    {
      TeamProjectReference project = this.ProjectInfo.ToTeamProjectReference(this.TfsRequestContext);
      return this.TfsRequestContext.GetService<ITeamFoundationBuildService2>().GetProjectMetrics(this.TfsRequestContext, this.ProjectId, metricAggregationType, minMetricsTime).Select<Microsoft.TeamFoundation.Build2.Server.BuildMetric, Microsoft.TeamFoundation.Build.WebApi.BuildMetric>((Func<Microsoft.TeamFoundation.Build2.Server.BuildMetric, Microsoft.TeamFoundation.Build.WebApi.BuildMetric>) (x => x.ToWebApiBuildMetric((ISecuredObject) project))).ToList<Microsoft.TeamFoundation.Build.WebApi.BuildMetric>();
    }
  }
}
