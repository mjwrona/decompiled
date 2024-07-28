// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.Controllers.ProjectActivityMetricsController
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.ProjectAnalysis.WebApi;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server.Controllers
{
  [ControllerApiVersion(4.0)]
  [VersionedApiControllerCustomName(Area = "projectanalysis", ResourceName = "projectactivitymetrics")]
  public class ProjectActivityMetricsController : ProjectAnalysisApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (ProjectActivityMetrics), null, null)]
    public HttpResponseMessage GetProjectActivityMetrics(
      DateTime fromDate,
      AggregationType aggregationType)
    {
      return this.Request.CreateResponse<ProjectActivityMetrics>(HttpStatusCode.OK, new ProjectActivityMetricsProvider().GetProjectActivityMetrics(this.TfsRequestContext, this.ProjectId, fromDate, aggregationType));
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ArgumentOutOfRangeException>(HttpStatusCode.BadRequest);
    }
  }
}
