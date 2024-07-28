// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.DeploymentTargetJobRequestsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(4.1)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "deploymentTargetJobRequests")]
  public sealed class DeploymentTargetJobRequestsController : DeploymentGroupApiController
  {
    [HttpGet]
    public IList<TaskAgentJobRequest> GetAgentRequestsForDeploymentTarget(
      int deploymentGroupId,
      [ClientQueryParameter] int targetId,
      [ClientQueryParameter] int completedRequestCount = 50)
    {
      return this.DeploymentGroupService.GetAgentRequestsForDeploymentTarget(this.TfsRequestContext, this.ProjectId, deploymentGroupId, targetId, completedRequestCount);
    }

    [HttpGet]
    public IList<TaskAgentJobRequest> GetAgentRequestsForDeploymentTargets(
      int deploymentGroupId,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string targetIds = null,
      [ClientQueryParameter] int? ownerId = null,
      [ClientQueryParameter] DateTime? completedOn = null,
      [ClientQueryParameter] int completedRequestCount = 50)
    {
      return this.DeploymentGroupService.GetAgentRequestsForDeploymentTargets(this.TfsRequestContext, this.ProjectId, deploymentGroupId, this.ParseArray(targetIds), completedRequestCount, ownerId, completedOn);
    }
  }
}
