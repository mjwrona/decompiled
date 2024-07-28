// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildLeasesController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build2.WebApiConverters;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(6.1)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "buildleases", ResourceVersion = 1)]
  [CheckWellFormedProject(Required = true)]
  [ClientGroupByResource("builds")]
  public class BuildLeasesController : BuildApiController
  {
    [HttpGet]
    [ClientLocationId("3DA19A6A-F088-45C4-83CE-2AD3A87BE6C4")]
    [ClientSwaggerOperationId("Get Retention Leases For Build")]
    public virtual async Task<IEnumerable<Microsoft.TeamFoundation.Build.WebApi.RetentionLease>> GetRetentionLeasesForBuild(
      int buildId)
    {
      BuildLeasesController leasesController = this;
      // ISSUE: explicit non-virtual call
      ProjectInfo projectInfo = __nonvirtual (leasesController.ProjectInfo);
      TeamProjectReference securedObject = projectInfo != null ? projectInfo.ToTeamProjectReference(leasesController.TfsRequestContext) : (TeamProjectReference) null;
      return (IEnumerable<Microsoft.TeamFoundation.Build.WebApi.RetentionLease>) (await leasesController.BuildService.GetRetentionLeasesForRuns(leasesController.TfsRequestContext, leasesController.ProjectId, (IEnumerable<int>) new int[1]
      {
        buildId
      })).Select<Microsoft.TeamFoundation.Build2.Server.RetentionLease, Microsoft.TeamFoundation.Build.WebApi.RetentionLease>((Func<Microsoft.TeamFoundation.Build2.Server.RetentionLease, Microsoft.TeamFoundation.Build.WebApi.RetentionLease>) (serverLease => serverLease.ToWebApiRetentionLease((ISecuredObject) securedObject))).ToList<Microsoft.TeamFoundation.Build.WebApi.RetentionLease>();
    }
  }
}
