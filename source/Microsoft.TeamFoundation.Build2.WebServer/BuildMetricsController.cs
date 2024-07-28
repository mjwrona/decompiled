// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildMetricsController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "metrics", ResourceVersion = 1)]
  public class BuildMetricsController : BuildApiController
  {
    [HttpGet]
    [ClientLocationId("D973B939-0CE0-4FEC-91D8-DA3940FA1827")]
    public List<Microsoft.TeamFoundation.Build.WebApi.BuildMetric> GetDefinitionMetrics(
      int definitionId,
      DateTime? minMetricsTime = null)
    {
      minMetricsTime = new DateTime?(minMetricsTime ?? DateTime.UtcNow.AddDays(-8.0));
      return this.DefinitionService.GetMetrics(this.TfsRequestContext, this.ProjectId, definitionId, minMetricsTime).MergePullRequestMetrics(this.TfsRequestContext).Select<Microsoft.TeamFoundation.Build2.Server.BuildMetric, Microsoft.TeamFoundation.Build.WebApi.BuildMetric>((Func<Microsoft.TeamFoundation.Build2.Server.BuildMetric, Microsoft.TeamFoundation.Build.WebApi.BuildMetric>) (x => x.ToWebApiBuildMetric((this.DefinitionService.GetDefinition(this.TfsRequestContext, this.ProjectId, definitionId) ?? throw new DefinitionNotFoundException(Resources.DefinitionNotFound((object) definitionId))).ToSecuredObject()))).ToList<Microsoft.TeamFoundation.Build.WebApi.BuildMetric>();
    }
  }
}
