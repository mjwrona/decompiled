// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskAgentsBaseController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  public abstract class TaskAgentsBaseController : DistributedTaskApiController
  {
    [HttpPost]
    public virtual TaskAgent AddAgent(int poolId, TaskAgent agent)
    {
      TaskAgentsBaseController.FixAgentVersion(agent, true);
      DistributedTaskApiController.FixAgentPlatform((TaskAgentReference) agent, this.TfsRequestContext.UserAgent);
      return this.ResourceService.AddAgent(this.TfsRequestContext, poolId, agent);
    }

    [HttpDelete]
    public virtual void DeleteAgent(int poolId, int agentId)
    {
      if (this.TfsRequestContext.IsFeatureEnabled("DistributedTask.ValidatePoolIdAndAgentIdOnAgentDelete"))
      {
        TaskAgentPool agentPool = this.ResourceService.GetAgentPool(this.TfsRequestContext, poolId);
        if (agentPool == null)
          throw new TaskAgentPoolNotFoundException(TaskResources.AgentPoolNotFound((object) poolId));
        if (agentPool.Size == 0)
          throw new TaskAgentNotFoundException(TaskResources.AgentNotFoundPoolEmpty((object) poolId));
        if (this.ResourceService.GetAgent(this.TfsRequestContext, poolId, agentId) == null)
          throw new TaskAgentNotFoundException(TaskResources.AgentNotFound((object) poolId, (object) agentId));
      }
      this.ResourceService.DeleteAgents(this.TfsRequestContext, poolId, (IEnumerable<int>) new int[1]
      {
        agentId
      });
    }

    [HttpGet]
    public TaskAgent GetAgent(
      int poolId,
      int agentId,
      [ClientQueryParameter] bool includeCapabilities = false,
      [ClientQueryParameter] bool includeAssignedRequest = false,
      [ClientQueryParameter] bool includeLastCompletedRequest = false,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string propertyFilters = null)
    {
      return this.ResourceService.GetAgent(this.TfsRequestContext, poolId, agentId, includeCapabilities, includeAssignedRequest, includeLastCompletedRequest, ArtifactPropertyKinds.AsPropertyFilters(propertyFilters)) ?? throw new TaskAgentNotFoundException(TaskResources.AgentNotFound((object) poolId, (object) agentId));
    }

    [HttpGet]
    public IList<TaskAgent> GetAgents(
      int poolId,
      [ClientQueryParameter] string agentName = null,
      [ClientQueryParameter] bool includeCapabilities = false,
      [ClientQueryParameter] bool includeAssignedRequest = false,
      [ClientQueryParameter] bool includeLastCompletedRequest = false,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string propertyFilters = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string demands = null)
    {
      if (string.IsNullOrEmpty(demands))
        return this.ResourceService.GetAgents(this.TfsRequestContext, poolId, agentName, includeCapabilities, includeAssignedRequest, includeLastCompletedRequest: includeLastCompletedRequest, propertyFilters: ArtifactPropertyKinds.AsPropertyFilters(propertyFilters));
      IList<Demand> demands1 = TaskAgentsBaseController.AsDemands(demands);
      if (!string.IsNullOrEmpty(agentName))
        demands1.Add((Demand) new DemandEquals(PipelineConstants.AgentName, agentName));
      return this.ResourceService.GetAgents(this.TfsRequestContext, poolId, demands1, ArtifactPropertyKinds.AsPropertyFilters(propertyFilters)).MatchedAgents;
    }

    [HttpPut]
    public TaskAgent ReplaceAgent(int poolId, int agentId, TaskAgent agent)
    {
      TaskAgentsBaseController.FixAgentVersion(agent);
      DistributedTaskApiController.FixAgentPlatform((TaskAgentReference) agent, this.TfsRequestContext.UserAgent);
      return this.ResourceService.UpdateAgent(this.TfsRequestContext, poolId, agent, TaskAgentCapabilityType.System);
    }

    [HttpPatch]
    public TaskAgent UpdateAgent(int poolId, int agentId, TaskAgent agent)
    {
      TaskAgentsBaseController.FixAgentVersion(agent);
      DistributedTaskApiController.FixAgentPlatform((TaskAgentReference) agent, this.TfsRequestContext.UserAgent);
      return this.ResourceService.UpdateAgent(this.TfsRequestContext, poolId, agent, TaskAgentCapabilityType.None);
    }

    private static IList<Demand> AsDemands(string value)
    {
      if (string.IsNullOrEmpty(value))
        return (IList<Demand>) new List<Demand>();
      List<Demand> demandList = new List<Demand>();
      foreach (string input in ((IEnumerable<string>) value.Split(',')).Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x))))
      {
        Demand demand;
        if (!Demand.TryParse(input, out demand))
          throw new ArgumentException(TaskResources.InvalidDemand((object) input), "demands").Expected("DistributedTask");
        demandList.Add(demand);
      }
      return (IList<Demand>) demandList;
    }

    private static void FixAgentVersion(TaskAgent agent, bool isAdd = false)
    {
      if (agent == null || !string.IsNullOrEmpty(agent.Version))
        return;
      string str;
      if (agent.SystemCapabilities.TryGetValue(PipelineConstants.AgentVersionDemandName, out str))
      {
        agent.Version = str;
      }
      else
      {
        if (!isAdd)
          return;
        agent.Version = "1.0.0";
      }
    }
  }
}
