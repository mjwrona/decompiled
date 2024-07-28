// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Elastic.ElasticHelpers
// Assembly: Microsoft.TeamFoundation.DistributedTask.Elastic, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6202E83A-3164-4101-8FDA-8C4FB25E62EC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Elastic.dll

using Microsoft.TeamFoundation.DistributedTask.Azure.Models;
using Microsoft.TeamFoundation.DistributedTask.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Elastic
{
  internal class ElasticHelpers
  {
    public static void CheckViewAndOtherPermissionsForPool(
      IVssRequestContext requestContext,
      int poolId,
      int otherPermissions = 0)
    {
      requestContext = requestContext.ToPoolRequestContext();
      IAgentPoolSecurityProvider agentPoolSecurity = requestContext.GetService<IInternalDistributedTaskResourceService>().GetAgentPoolSecurity(requestContext, poolId);
      if (!agentPoolSecurity.HasPoolPermission(requestContext, poolId, 1, true))
        throw new ElasticPoolDoesNotExistException(ElasticResources.ElasticPoolDoesNotExist((object) poolId));
      if (otherPermissions == 0 || agentPoolSecurity.HasPoolPermission(requestContext, poolId, otherPermissions, true))
        return;
      PoolSecurityProvider.ThrowPoolAccessDeniedException(requestContext, otherPermissions, poolId);
    }

    public static async Task<bool> CanReimageVMs(
      IVssRequestContext requestContext,
      ElasticPool elasticPool,
      VirtualMachineScaleSet vmss = null)
    {
      if (requestContext.IsFeatureEnabled("DistributedTask.ElasticPoolDisableRecycleForNonEphemeral") && elasticPool.RecycleAfterEachUse)
      {
        IAzureVirtualMachineScaleSetResourceServiceInternal service = requestContext.GetService<IAzureVirtualMachineScaleSetResourceServiceInternal>();
        if (vmss == null)
          vmss = await service.GetScaleSetAsync(requestContext, elasticPool);
        if (vmss.OrchestrationType() == OrchestrationType.Flexible && !vmss.AllowReimage())
          return false;
      }
      return true;
    }
  }
}
