// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskAgentUserCapabilitiesController
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
  [ControllerApiVersion(1.0)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "usercapabilities")]
  public class TaskAgentUserCapabilitiesController : DistributedTaskApiController
  {
    [HttpPut]
    public TaskAgent UpdateAgentUserCapabilities(
      int poolId,
      int agentId,
      Dictionary<string, string> userCapabilities)
    {
      ArgumentUtility.CheckForNull<Dictionary<string, string>>(userCapabilities, nameof (userCapabilities), "DistributedTask");
      TaskAgent agent = this.ResourceService.GetAgent(this.TfsRequestContext, poolId, agentId, true);
      if (agent == null)
        throw new TaskAgentNotFoundException(TaskResources.AgentNotFound((object) poolId, (object) agentId));
      agent.Name = (string) null;
      agent.Authorization = (TaskAgentAuthorization) null;
      agent.Version = (string) null;
      agent.UserCapabilities.Clear();
      foreach (KeyValuePair<string, string> userCapability in userCapabilities)
        agent.UserCapabilities.Add(userCapability);
      return this.ResourceService.UpdateAgent(this.TfsRequestContext, poolId, agent, TaskAgentCapabilityType.User);
    }
  }
}
