// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DistributedTaskCommerceHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class DistributedTaskCommerceHelper
  {
    private const string c_defaultHostedPoolName = "Hosted";
    private const string c_defaultHostedAgentName = "Hosted Agent";
    private const string c_layer = "DistributedTaskCommerceHelper";

    public static bool ResizeHostedPools(
      IVssRequestContext requestContext,
      int targetParallelism,
      string agentPoolName = null,
      bool includeNonAutoSizePools = false)
    {
      List<TaskAgentPool> agentPools = requestContext.GetService<IDistributedTaskResourceService>().GetAgentPools(requestContext);
      List<TaskAgentPool> taskAgentPoolList = new List<TaskAgentPool>();
      if (string.IsNullOrEmpty(agentPoolName))
      {
        taskAgentPoolList.AddRange(agentPools.Where<TaskAgentPool>((Func<TaskAgentPool, bool>) (p =>
        {
          if (!p.IsHosted)
            return false;
          if (includeNonAutoSizePools || !p.AutoSize.HasValue)
            return true;
          return p.AutoSize.HasValue && p.AutoSize.Value;
        })));
      }
      else
      {
        TaskAgentPool taskAgentPool = agentPools.FirstOrDefault<TaskAgentPool>((Func<TaskAgentPool, bool>) (p => p.IsHosted && p.Name.Equals(agentPoolName, StringComparison.OrdinalIgnoreCase)));
        if (taskAgentPool != null)
          taskAgentPoolList.Add(taskAgentPool);
      }
      if (taskAgentPoolList.Count == 0)
      {
        requestContext.TraceError(nameof (DistributedTaskCommerceHelper), "There was no Hosted Agent Pool for HostId {0}, cannot add an additional Hosted Task Agent.", (object) requestContext.ServiceHost.InstanceId);
        return true;
      }
      bool flag1 = true;
      bool flag2 = false;
      PackageVersion agentPackageVersion = requestContext.GetRecommendedAgentPackageVersion();
      foreach (TaskAgentPool pool in taskAgentPoolList)
      {
        (bool poolAtCorrectSize, bool agentsAdded) tuple = DistributedTaskCommerceHelper.AddOrRemoveHostedTaskAgents(requestContext, pool, targetParallelism, agentPackageVersion);
        flag1 &= tuple.poolAtCorrectSize;
        flag2 |= tuple.agentsAdded;
      }
      if (flag2)
        DistributedTaskResourceService.QueueAgentRematchJob(requestContext);
      return flag1;
    }

    private static (bool poolAtCorrectSize, bool agentsAdded) AddOrRemoveHostedTaskAgents(
      IVssRequestContext requestContext,
      TaskAgentPool pool,
      int newAgentCount,
      PackageVersion agentVersion)
    {
      string str1 = "Hosted Agent";
      if (!pool.Name.Equals("Hosted", StringComparison.OrdinalIgnoreCase))
        str1 = pool.Name;
      Dictionary<string, TaskAgent> source = new Dictionary<string, TaskAgent>();
      IDistributedTaskResourceService service = requestContext.GetService<IDistributedTaskResourceService>();
      foreach (TaskAgent agent in (IEnumerable<TaskAgent>) service.GetAgents(requestContext, pool.Id))
        source.Add(agent.Name, agent);
      bool flag1 = false;
      int num = 1;
      TaskAgent agent1;
      if (source.TryGetValue("Hosted Agent", out agent1))
      {
        source.Remove("Hosted Agent");
        if (!requestContext.IsFeatureEnabled("DistributedTask.HostedAgent.EnsureMaxParallelism.Disabled"))
        {
          bool agentParallelismAdded;
          DistributedTaskCommerceHelper.EnsureAgentParallelism(requestContext, service, pool, agent1, out agentParallelismAdded);
          flag1 |= agentParallelismAdded;
        }
        num = 2;
      }
      for (int index = num; index <= newAgentCount; ++index)
      {
        string str2 = string.Format("{0} {1}", (object) str1, (object) index);
        if (source.TryGetValue(str2, out agent1))
        {
          source.Remove(str2);
          bool agentParallelismAdded;
          DistributedTaskCommerceHelper.EnsureAgentParallelism(requestContext, service, pool, agent1, out agentParallelismAdded);
          flag1 |= agentParallelismAdded;
        }
        else
        {
          DistributedTaskAgentCloudPoolsHelper.TraceAgentCloudPoolScaleOut(requestContext, pool, str2);
          TaskAgent taskAgent1 = new TaskAgent(str2);
          taskAgent1.Enabled = new bool?(true);
          taskAgent1.MaxParallelism = new int?(1);
          taskAgent1.Version = agentVersion.ToString();
          TaskAgent agent2 = taskAgent1;
          if (pool.AgentCloudId.HasValue)
            agent2.ProvisioningState = "Deallocated";
          try
          {
            TaskAgent taskAgent2 = service.AddAgent(requestContext, pool.Id, agent2);
            if (taskAgent2 != null)
            {
              requestContext.TraceInfo(nameof (DistributedTaskCommerceHelper), "Successfully created agent {0} with id {1}", (object) taskAgent2.Name, (object) taskAgent2.Id);
              flag1 = true;
            }
          }
          catch (TaskAgentExistsException ex)
          {
            requestContext.TraceException(nameof (DistributedTaskCommerceHelper), (Exception) ex);
          }
        }
      }
      bool flag2 = true;
      try
      {
        int[] array = source.Values.Select<TaskAgent, int>((Func<TaskAgent, int>) (x => x.Id)).ToArray<int>();
        int deletedAgentsCount = service.DeleteAgents(requestContext, pool.Id, (IEnumerable<int>) array);
        DistributedTaskAgentCloudPoolsHelper.TraceAgentCloudPoolScaleIn(requestContext, pool, array, deletedAgentsCount);
        if (deletedAgentsCount < source.Count<KeyValuePair<string, TaskAgent>>())
          flag2 = false;
      }
      catch (TaskAgentJobStillRunningException ex)
      {
        flag2 = false;
      }
      return (flag2, flag1);
    }

    private static void EnsureAgentParallelism(
      IVssRequestContext requestContext,
      IDistributedTaskResourceService resourceService,
      TaskAgentPool pool,
      TaskAgent agent,
      out bool agentParallelismAdded)
    {
      int? maxParallelism = agent.MaxParallelism;
      int num = 0;
      if (maxParallelism.GetValueOrDefault() > num & maxParallelism.HasValue)
      {
        agentParallelismAdded = false;
      }
      else
      {
        agent.MaxParallelism = new int?(1);
        resourceService.UpdateAgent(requestContext, pool.Id, agent, TaskAgentCapabilityType.None);
        agentParallelismAdded = true;
      }
    }
  }
}
