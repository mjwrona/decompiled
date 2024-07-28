// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskAgentPoolsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "pools")]
  public sealed class TaskAgentPoolsController : DistributedTaskApiController
  {
    [HttpPost]
    public TaskAgentPool AddAgentPool(TaskAgentPool pool) => this.ResourceService.AddAgentPool(this.TfsRequestContext, pool);

    [HttpDelete]
    public void DeleteAgentPool(int poolId) => this.ResourceService.DeleteAgentPool(this.TfsRequestContext, poolId);

    [HttpGet]
    public TaskAgentPool GetAgentPool(
      int poolId,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string properties = null,
      [ClientQueryParameter] TaskAgentPoolActionFilter actionFilter = TaskAgentPoolActionFilter.None)
    {
      return this.ResourceService.GetAgentPool(this.TfsRequestContext, poolId, ArtifactPropertyKinds.AsPropertyFilters(properties), actionFilter) ?? throw new TaskAgentPoolNotFoundException(TaskResources.AgentPoolNotFound((object) poolId));
    }

    [HttpGet]
    public IList<TaskAgentPool> GetAgentPools(
      [ClientQueryParameter] string poolName = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string properties = null,
      [ClientQueryParameter] TaskAgentPoolType poolType = TaskAgentPoolType.Automation,
      [ClientQueryParameter] TaskAgentPoolActionFilter actionFilter = TaskAgentPoolActionFilter.None)
    {
      return (IList<TaskAgentPool>) this.ResourceService.GetAgentPools(this.TfsRequestContext, poolName, ArtifactPropertyKinds.AsPropertyFilters(properties), poolType, actionFilter);
    }

    [HttpGet]
    public async Task<IList<TaskAgentPool>> GetAgentPoolsByIds(
      [ClientParameterAsIEnumerable(typeof (int), ',')] string poolIds,
      [ClientQueryParameter] TaskAgentPoolActionFilter actionFilter = TaskAgentPoolActionFilter.None)
    {
      TaskAgentPoolsController agentPoolsController = this;
      IList<int> array = DistributedTaskApiControllerHelper.ParseArray(poolIds);
      return (IList<TaskAgentPool>) await agentPoolsController.ResourceService.GetAgentPoolsByIdsAsync(agentPoolsController.TfsRequestContext, array, actionFilter: actionFilter);
    }

    [HttpPatch]
    public TaskAgentPool UpdateAgentPool(int poolId, TaskAgentPool pool) => this.ResourceService.UpdateAgentPool(this.TfsRequestContext, poolId, pool);
  }
}
