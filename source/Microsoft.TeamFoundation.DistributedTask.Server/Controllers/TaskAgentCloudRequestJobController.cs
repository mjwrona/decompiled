// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskAgentCloudRequestJobController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.Pipelines.PoolProvider.Contracts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "agentCloudRequestJob")]
  [ClientIgnore]
  public sealed class TaskAgentCloudRequestJobController : DistributedTaskApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (AgentRequestJob), null, null)]
    public Task<AgentRequestJob> GetTaskAgentCloudRequestJobAsync(
      int agentCloudId,
      Guid agentCloudRequestId)
    {
      return this.AgentCloudService.GetTaskAgentCloudRequestJobAsync(this.TfsRequestContext, agentCloudId, agentCloudRequestId);
    }
  }
}
