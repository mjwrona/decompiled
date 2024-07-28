// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DeploymentGroupManager
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public class DeploymentGroupManager
  {
    public async Task<IList<DeploymentMachine>> GetDeploymentTargetsAsync(
      IVssRequestContext requestContext,
      IDistributedTaskPoolService poolService,
      Guid projectId,
      int deploymentGroupId,
      IList<string> tagFilters,
      DeploymentTargetExpands expands = DeploymentTargetExpands.None,
      bool? enabled = null,
      IList<string> propertyFilters = null)
    {
      IList<DeploymentMachine> deploymentMachines = (IList<DeploymentMachine>) new List<DeploymentMachine>();
      string continuationToken = (string) null;
      do
      {
        IPagedList<DeploymentMachine> deploymentMachinesAsync = await poolService.GetDeploymentMachinesAsync(requestContext, projectId, deploymentGroupId, tagFilters, continuationToken, expands, enabled, propertyFilters);
        if (deploymentMachinesAsync != null)
        {
          deploymentMachines.AddRange<DeploymentMachine, IList<DeploymentMachine>>((IEnumerable<DeploymentMachine>) deploymentMachinesAsync);
          continuationToken = deploymentMachinesAsync.ContinuationToken;
        }
        else
          break;
      }
      while (!string.IsNullOrEmpty(continuationToken));
      IList<DeploymentMachine> deploymentTargetsAsync = deploymentMachines;
      deploymentMachines = (IList<DeploymentMachine>) null;
      return deploymentTargetsAsync;
    }
  }
}
