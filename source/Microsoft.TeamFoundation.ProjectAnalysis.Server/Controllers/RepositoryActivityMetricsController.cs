// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.Controllers.RepositoryActivityMetricsController
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.ProjectAnalysis.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server.Controllers
{
  [ControllerApiVersion(4.0)]
  [VersionedApiControllerCustomName(Area = "projectanalysis", ResourceName = "repositoryactivitymetrics")]
  public class RepositoryActivityMetricsController : ProjectAnalysisApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (RepositoryActivityMetrics), null, null)]
    public HttpResponseMessage GetRepositoryActivityMetrics(
      Guid repositoryId,
      DateTime fromDate,
      AggregationType aggregationType)
    {
      return this.Request.CreateResponse<RepositoryActivityMetrics>(HttpStatusCode.OK, new RepositoryActivityMetricsProvider().GetRepositoryActivityMetrics(this.TfsRequestContext, this.ProjectId, repositoryId, fromDate, aggregationType));
    }

    [HttpGet]
    [ClientResponseType(typeof (IList<RepositoryActivityMetrics>), null, null)]
    public HttpResponseMessage GetGitRepositoriesActivityMetrics(
      DateTime fromDate,
      AggregationType aggregationType,
      [FromUri(Name = "$skip")] int skip,
      [FromUri(Name = "$top")] int top)
    {
      return this.Request.CreateResponse<IList<RepositoryActivityMetrics>>(HttpStatusCode.OK, new RepositoryActivityMetricsProvider().GetGitRepositoriesActivityMetrics(this.TfsRequestContext, this.ProjectId, fromDate, aggregationType, skip, top));
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ArgumentOutOfRangeException>(HttpStatusCode.BadRequest);
    }
  }
}
