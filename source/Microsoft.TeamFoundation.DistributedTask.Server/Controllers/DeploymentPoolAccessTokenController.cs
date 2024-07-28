// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.DeploymentPoolAccessTokenController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(4.1)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "deploymentpoolaccesstoken")]
  [OutputCache(NoStore = true, Duration = 0)]
  public sealed class DeploymentPoolAccessTokenController : DeploymentGroupApiController
  {
    [System.Web.Http.HttpPost]
    public string GenerateDeploymentPoolAccessToken(int poolId)
    {
      if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        return this.DeploymentGroupService.GeneratePersonalAccessTokenWithDeploymentPoolScope(this.TfsRequestContext, poolId);
      throw new NotSupportedException("Only supported for Azure DevOps Services");
    }
  }
}
