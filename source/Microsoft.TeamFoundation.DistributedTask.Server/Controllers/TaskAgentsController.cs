// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskAgentsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "agents")]
  public class TaskAgentsController : TaskAgentsBaseController
  {
    [HttpPost]
    public override TaskAgent AddAgent(int poolId, TaskAgent agent)
    {
      try
      {
        return base.AddAgent(poolId, agent);
      }
      catch (AccessDeniedException ex)
      {
        TaskAgentPool agentPool = this.ResourceService.GetAgentPool(this.TfsRequestContext.Elevate(), poolId);
        if (agentPool != null && agentPool.PoolType == TaskAgentPoolType.Deployment)
          throw new InvalidTaskAgentVersionException(TaskResources.UnableToAddDeploymentAgent());
        throw;
      }
    }

    [HttpDelete]
    public override void DeleteAgent(int poolId, int agentId)
    {
      try
      {
        base.DeleteAgent(poolId, agentId);
      }
      catch (AccessDeniedException ex)
      {
        TaskAgentPool agentPool = this.ResourceService.GetAgentPool(this.TfsRequestContext.Elevate(), poolId);
        if (agentPool != null && agentPool.PoolType == TaskAgentPoolType.Deployment)
        {
          TaskAgent agent = this.ResourceService.GetAgent(this.TfsRequestContext.Elevate(), poolId, agentId);
          throw new InvalidTaskAgentVersionException(TaskResources.UnableToRemoveDeploymentAgent(agent != null ? (object) agent.Name : (object) string.Empty));
        }
        throw;
      }
    }
  }
}
