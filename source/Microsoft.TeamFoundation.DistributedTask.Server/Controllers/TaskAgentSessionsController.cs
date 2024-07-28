// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskAgentSessionsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ClientInternalUseOnly(true)]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "sessions")]
  public sealed class TaskAgentSessionsController : DistributedTaskApiController
  {
    [HttpPost]
    public TaskAgentSession CreateAgentSession(int poolId, TaskAgentSession session)
    {
      TaskAgentSessionsController.FixAgentVersion(session.Agent, session.SystemCapabilities);
      DistributedTaskApiController.FixAgentPlatform(session.Agent, this.TfsRequestContext.UserAgent);
      return this.ResourceService.CreateSession(this.TfsRequestContext, poolId, session);
    }

    [HttpDelete]
    public void DeleteAgentSession(int poolId, Guid sessionId) => this.ResourceService.DeleteSession(this.TfsRequestContext, poolId, sessionId);

    private static void FixAgentVersion(
      TaskAgentReference agent,
      IDictionary<string, string> systemCapabilities)
    {
      string str;
      if (agent == null || !string.IsNullOrEmpty(agent.Version) || !systemCapabilities.TryGetValue(PipelineConstants.AgentVersionDemandName, out str))
        return;
      agent.Version = str;
    }
  }
}
