// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.DeploymentGroupAccessTokenController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(3.2)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "deploymentgroupaccesstoken")]
  [OutputCache(NoStore = true, Duration = 0)]
  public sealed class DeploymentGroupAccessTokenController : DeploymentGroupApiController
  {
    [System.Web.Http.HttpPost]
    public string GenerateDeploymentGroupAccessToken(int deploymentGroupId) => this.DeploymentGroupService.GeneratePersonalAccessTokenWithDeploymentGroupScope(this.TfsRequestContext, this.ProjectId, deploymentGroupId);
  }
}
