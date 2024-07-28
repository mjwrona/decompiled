// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DeploymentGroupExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class DeploymentGroupExtensions
  {
    public static DeploymentGroupReference AsReference(this DeploymentGroup deploymentGroup) => new DeploymentGroupReference()
    {
      Id = deploymentGroup.Id,
      Name = deploymentGroup.Name,
      Pool = deploymentGroup.Pool,
      Project = deploymentGroup.Project
    };

    public static DeploymentGroup PopulatePoolReference(
      this DeploymentGroup deploymentGroup,
      IVssRequestContext requestContext)
    {
      if (deploymentGroup?.Pool == null)
        return deploymentGroup;
      TaskAgentPool agentPool = requestContext.GetService<IDistributedTaskResourceService>().GetAgentPool(requestContext.Elevate(), deploymentGroup.Pool.Id);
      if (agentPool != null)
      {
        deploymentGroup.Pool.Name = agentPool.Name;
        deploymentGroup.Pool.Scope = agentPool.Scope;
        deploymentGroup.Pool.Size = agentPool.Size;
        deploymentGroup.Pool.PoolType = agentPool.PoolType;
      }
      return deploymentGroup;
    }

    public static DeploymentGroup PopulateAgentReferences(
      this DeploymentGroup deploymentGroup,
      IVssRequestContext requestContext,
      bool addMachinesForNewAgents = true,
      bool removeMachinesForDeletedAgents = true)
    {
      if (deploymentGroup?.Pool == null)
        return deploymentGroup;
      IList<TaskAgent> agents = DeploymentGroupExtensions.GetAgents(deploymentGroup, requestContext);
      deploymentGroup.Machines = DeploymentGroupHelper.MapAgentsToMachines((IEnumerable<DeploymentMachine>) deploymentGroup.Machines, (IEnumerable<TaskAgent>) agents, addMachinesForNewAgents, removeMachinesForDeletedAgents);
      return deploymentGroup;
    }

    public static DeploymentGroup PopulateAgentCount(
      this DeploymentGroup deploymentGroup,
      Dictionary<int, TaskAgentPool> poolLookUp)
    {
      TaskAgentPool taskAgentPool;
      if (deploymentGroup?.Pool == null || !poolLookUp.TryGetValue(deploymentGroup.Pool.Id, out taskAgentPool))
        return deploymentGroup;
      deploymentGroup.MachineCount = taskAgentPool.Size;
      return deploymentGroup;
    }

    public static DeploymentGroup PopulateProjectName(
      this DeploymentGroup deploymentGroup,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      if (deploymentGroup.Project != null)
      {
        try
        {
          deploymentGroup.Project.Name = requestContext.GetService<IProjectService>().GetProjectName(requestContext.Elevate(), projectId);
        }
        catch (ProjectDoesNotExistException ex)
        {
          requestContext.TraceInfo(10015146, "DistributedTask", "Getting project name for project Id {0} failed with error {1}", (object) projectId, (object) ex.ToString());
          deploymentGroup.Project.Name = string.Empty;
        }
      }
      return deploymentGroup;
    }

    private static IList<TaskAgent> GetAgents(
      DeploymentGroup deploymentGroup,
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<IDistributedTaskResourceService>().GetAgents(requestContext.Elevate(), deploymentGroup.Pool.Id);
    }
  }
}
