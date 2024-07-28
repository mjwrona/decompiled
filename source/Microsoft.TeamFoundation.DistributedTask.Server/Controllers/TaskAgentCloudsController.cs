// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskAgentCloudsController
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
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "agentclouds")]
  public sealed class TaskAgentCloudsController : DistributedTaskApiController
  {
    [HttpPost]
    [ClientResponseType(typeof (TaskAgentCloud), null, null)]
    public Task<TaskAgentCloud> AddAgentCloud(TaskAgentCloud agentCloud) => this.AgentCloudService.AddAgentCloudAsync(this.TfsRequestContext, agentCloud);

    [HttpGet]
    [ClientResponseType(typeof (TaskAgentCloud), null, null)]
    public async Task<TaskAgentCloud> GetAgentCloud(int agentCloudId)
    {
      TaskAgentCloudsController cloudsController = this;
      return await cloudsController.AgentCloudService.GetAgentCloudAsync(cloudsController.TfsRequestContext, agentCloudId) ?? throw new TaskAgentCloudNotFoundException(TaskResources.AgentCloudNotFound((object) agentCloudId));
    }

    [HttpGet]
    [ClientResponseType(typeof (IList<TaskAgentCloud>), null, null)]
    public Task<IList<TaskAgentCloud>> GetAgentClouds() => this.AgentCloudService.GetAgentCloudsAsync(this.TfsRequestContext);

    [HttpDelete]
    [ClientResponseType(typeof (TaskAgentCloud), null, null)]
    public Task<TaskAgentCloud> DeleteAgentCloud(int agentCloudId) => this.AgentCloudService.DeleteAgentCloudAsync(this.TfsRequestContext, agentCloudId);

    [HttpPatch]
    [ClientResponseType(typeof (TaskAgentCloud), null, null)]
    public Task<TaskAgentCloud> UpdateAgentCloud(int agentCloudId, TaskAgentCloud updatedCloud) => this.AgentCloudService.UpdateAgentCloudAsync(this.TfsRequestContext, agentCloudId, updatedCloud);
  }
}
