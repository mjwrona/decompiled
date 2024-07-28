// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DistributedTaskAgentCloudPoolsHelper
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
  internal static class DistributedTaskAgentCloudPoolsHelper
  {
    private const string c_layer = "DistributedTaskAgentCloudPoolsHelper";

    public static bool ResizeAgentCloudPools(
      IVssRequestContext requestContext,
      string agentPoolName = null)
    {
      List<TaskAgentPool> agentPools = requestContext.GetService<IDistributedTaskResourceService>().GetAgentPools(requestContext);
      List<TaskAgentPool> taskAgentPoolList = new List<TaskAgentPool>();
      if (string.IsNullOrEmpty(agentPoolName))
      {
        taskAgentPoolList.AddRange(agentPools.Where<TaskAgentPool>((Func<TaskAgentPool, bool>) (p => !p.IsHosted && p.AgentCloudId.HasValue)));
      }
      else
      {
        TaskAgentPool taskAgentPool = agentPools.FirstOrDefault<TaskAgentPool>((Func<TaskAgentPool, bool>) (p => !p.IsHosted && p.AgentCloudId.HasValue && p.Name.Equals(agentPoolName, StringComparison.OrdinalIgnoreCase)));
        if (taskAgentPool != null)
          taskAgentPoolList.Add(taskAgentPool);
      }
      if (taskAgentPoolList.Count == 0)
      {
        requestContext.TraceWarning(nameof (DistributedTaskAgentCloudPoolsHelper), "There was no Agent Cloud backed Pool for HostId {0}, cannot add an additional Task Agent.", (object) requestContext.ServiceHost.InstanceId);
        return true;
      }
      IInternalAgentCloudService service = requestContext.GetService<IInternalAgentCloudService>();
      Dictionary<int, TaskAgentCloud> dictionary1 = new Dictionary<int, TaskAgentCloud>();
      IVssRequestContext requestContext1 = requestContext;
      foreach (TaskAgentCloud agentCloud in (IEnumerable<TaskAgentCloud>) service.GetAgentClouds(requestContext1))
        dictionary1.Add(agentCloud.AgentCloudId, agentCloud);
      bool flag1 = true;
      PackageVersion agentPackageVersion = requestContext.GetRecommendedAgentPackageVersion();
      foreach (TaskAgentPool pool in taskAgentPoolList)
      {
        int? nullable = pool.TargetSize;
        int targetSize = nullable ?? 1;
        bool? autoSize = pool.AutoSize;
        bool flag2 = true;
        if (autoSize.GetValueOrDefault() == flag2 & autoSize.HasValue)
        {
          nullable = pool.AgentCloudId;
          if (nullable.HasValue)
          {
            Dictionary<int, TaskAgentCloud> dictionary2 = dictionary1;
            nullable = pool.AgentCloudId;
            int key = nullable.Value;
            TaskAgentCloud taskAgentCloud;
            ref TaskAgentCloud local = ref taskAgentCloud;
            if (dictionary2.TryGetValue(key, out local))
            {
              nullable = taskAgentCloud.MaxParallelism;
              targetSize = nullable ?? targetSize;
            }
          }
        }
        flag1 &= DistributedTaskAgentCloudPoolsHelper.AddOrRemoveTaskAgents(requestContext, pool, agentPackageVersion, targetSize);
      }
      DistributedTaskResourceService.QueueAgentRematchJob(requestContext);
      return flag1;
    }

    private static bool AddOrRemoveTaskAgents(
      IVssRequestContext requestContext,
      TaskAgentPool pool,
      PackageVersion agentVersion,
      int targetSize)
    {
      string name = pool.Name;
      Dictionary<string, TaskAgent> source = new Dictionary<string, TaskAgent>();
      IDistributedTaskResourceService service = requestContext.GetService<IDistributedTaskResourceService>();
      foreach (TaskAgent agent in (IEnumerable<TaskAgent>) service.GetAgents(requestContext, pool.Id))
        source.Add(agent.Name, agent);
      for (int index = 1; index <= targetSize; ++index)
      {
        string str = string.Format("{0} {1}", (object) name, (object) index);
        TaskAgent agent1;
        if (source.TryGetValue(str, out agent1))
        {
          source.Remove(str);
          int? maxParallelism = agent1.MaxParallelism;
          int num = 0;
          if (maxParallelism.GetValueOrDefault() == num & maxParallelism.HasValue)
          {
            agent1.MaxParallelism = new int?(1);
            service.UpdateAgent(requestContext, pool.Id, agent1, TaskAgentCapabilityType.None);
          }
        }
        else
        {
          DistributedTaskAgentCloudPoolsHelper.TraceAgentCloudPoolScaleOut(requestContext, pool, str);
          TaskAgent taskAgent1 = new TaskAgent(str);
          taskAgent1.Enabled = new bool?(true);
          taskAgent1.MaxParallelism = new int?(1);
          taskAgent1.Version = agentVersion.ToString();
          taskAgent1.ProvisioningState = "Deallocated";
          TaskAgent agent2 = taskAgent1;
          try
          {
            TaskAgent taskAgent2 = service.AddAgent(requestContext, pool.Id, agent2);
            if (taskAgent2 != null)
              requestContext.TraceInfo(nameof (DistributedTaskAgentCloudPoolsHelper), "Successfully created agent {0} with id {1}", (object) taskAgent2.Name, (object) taskAgent2.Id);
          }
          catch (TaskAgentExistsException ex)
          {
            requestContext.TraceException(nameof (DistributedTaskAgentCloudPoolsHelper), (Exception) ex);
          }
        }
      }
      bool flag = true;
      try
      {
        int[] array = source.Values.Select<TaskAgent, int>((Func<TaskAgent, int>) (x => x.Id)).ToArray<int>();
        int deletedAgentsCount = service.DeleteAgents(requestContext, pool.Id, (IEnumerable<int>) array);
        DistributedTaskAgentCloudPoolsHelper.TraceAgentCloudPoolScaleIn(requestContext, pool, array, deletedAgentsCount);
        if (deletedAgentsCount < source.Count<KeyValuePair<string, TaskAgent>>())
          flag = false;
      }
      catch (TaskAgentJobStillRunningException ex)
      {
        flag = false;
      }
      return flag;
    }

    public static void TraceAgentCloudPoolScaleOut(
      IVssRequestContext requestContext,
      TaskAgentPool pool,
      string agentName)
    {
      if (!requestContext.IsFeatureEnabled("DistributedTask.AgentPoolTelemetry"))
        return;
      try
      {
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("AgentCloudId", (object) pool.AgentCloudId);
        properties.Add("PoolName", pool.Name);
        properties.Add("PoolSize", (double) pool.Size);
        properties.Add("PoolTargetSize", (object) pool.TargetSize);
        properties.Add("NewAgentName", agentName);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "DistributedTask", "AgentCloud backed AgentPool scale-out", properties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(nameof (DistributedTaskAgentCloudPoolsHelper), ex);
      }
    }

    public static void TraceAgentCloudPoolScaleIn(
      IVssRequestContext requestContext,
      TaskAgentPool pool,
      int[] agentsToDelete,
      int deletedAgentsCount)
    {
      if (!requestContext.IsFeatureEnabled("DistributedTask.AgentPoolTelemetry"))
        return;
      try
      {
        if (agentsToDelete.Length == 0)
          return;
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("AgentCloudId", (object) pool.AgentCloudId);
        properties.Add("PoolName", pool.Name);
        properties.Add("PoolSize", (double) pool.Size);
        properties.Add("PoolTargetSize", (object) pool.TargetSize);
        properties.Add("AgentsToDelete", agentsToDelete.Serialize<int[]>());
        properties.Add("SuccessfullyDeletedCount", (double) deletedAgentsCount);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "DistributedTask", "AgentCloud backed AgentPool scale-in", properties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(nameof (DistributedTaskAgentCloudPoolsHelper), ex);
      }
    }
  }
}
