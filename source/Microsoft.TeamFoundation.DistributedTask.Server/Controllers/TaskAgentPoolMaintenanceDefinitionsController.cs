// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskAgentPoolMaintenanceDefinitionsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(3.2)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "maintenancedefinitions")]
  public sealed class TaskAgentPoolMaintenanceDefinitionsController : DistributedTaskApiController
  {
    [HttpGet]
    public IList<TaskAgentPoolMaintenanceDefinition> GetAgentPoolMaintenanceDefinitions(int poolId)
    {
      ArgumentUtility.CheckGreaterThanZero((float) poolId, nameof (poolId));
      return this.ResourceService.GetAgentPoolMaintenanceDefinitions(this.TfsRequestContext, poolId);
    }

    [HttpPost]
    public TaskAgentPoolMaintenanceDefinition CreateAgentPoolMaintenanceDefinition(
      int poolId,
      TaskAgentPoolMaintenanceDefinition definition)
    {
      ArgumentUtility.CheckGreaterThanZero((float) poolId, nameof (poolId));
      ArgumentUtility.CheckForNull<TaskAgentPoolMaintenanceDefinition>(definition, nameof (definition));
      return this.ResourceService.CreateAgentPoolMaintenanceDefinition(this.TfsRequestContext, poolId, definition);
    }

    [HttpPut]
    public TaskAgentPoolMaintenanceDefinition UpdateAgentPoolMaintenanceDefinition(
      int poolId,
      int definitionId,
      TaskAgentPoolMaintenanceDefinition definition)
    {
      ArgumentUtility.CheckGreaterThanZero((float) poolId, nameof (poolId));
      ArgumentUtility.CheckGreaterThanZero((float) definitionId, nameof (definitionId));
      ArgumentUtility.CheckForNull<TaskAgentPoolMaintenanceDefinition>(definition, nameof (definition));
      return this.ResourceService.UpdateAgentPoolMaintenanceDefinition(this.TfsRequestContext, poolId, definitionId, definition);
    }

    [HttpGet]
    public TaskAgentPoolMaintenanceDefinition GetAgentPoolMaintenanceDefinition(
      int poolId,
      int definitionId)
    {
      ArgumentUtility.CheckGreaterThanZero((float) poolId, nameof (poolId));
      ArgumentUtility.CheckGreaterThanZero((float) definitionId, nameof (definitionId));
      return this.ResourceService.GetAgentPoolMaintenanceDefinition(this.TfsRequestContext, poolId, definitionId);
    }

    [HttpDelete]
    public void DeleteAgentPoolMaintenanceDefinition(int poolId, int definitionId)
    {
      ArgumentUtility.CheckGreaterThanZero((float) poolId, nameof (poolId));
      ArgumentUtility.CheckGreaterThanZero((float) definitionId, nameof (definitionId));
      this.ResourceService.DeleteAgentPoolMaintenanceDefinition(this.TfsRequestContext, poolId, definitionId);
    }
  }
}
