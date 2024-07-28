// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DistributedTaskEventSource
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  [EventSource(Name = "Microsoft-Visual Studio Services-DistributedTask")]
  internal class DistributedTaskEventSource : EventSource
  {
    private static readonly Lazy<DistributedTaskEventSource> s_log = new Lazy<DistributedTaskEventSource>((Func<DistributedTaskEventSource>) (() => new DistributedTaskEventSource()));

    public static DistributedTaskEventSource Log => DistributedTaskEventSource.s_log.Value;

    [NonEvent]
    public void PublishAgentPoolRequestHistory(
      Guid hostId,
      string poolName,
      string projectName,
      TaskAgentJobRequest request,
      TaskAgentCloud cloud)
    {
      string str1 = (string) null;
      string str2 = (string) null;
      string str3 = (string) null;
      if (cloud != null)
      {
        str1 = cloud.Name;
        str2 = cloud.Type;
        str3 = JsonUtility.ToString<JToken>((IList<JToken>) request.AgentSpecification);
      }
      else if (request.AgentSpecification != null)
      {
        str3 = JsonUtility.ToString<JToken>((IList<JToken>) request.AgentSpecification);
      }
      else
      {
        IList<Demand> demands = request.Demands;
        if ((demands != null ? (demands.Count > 0 ? 1 : 0) : 0) != 0)
          str3 = JsonUtility.ToString<Demand>(request.Demands);
      }
      Guid hostId1 = hostId;
      string orchestrationId = request.OrchestrationId;
      long requestId = request.RequestId;
      string poolName1 = poolName;
      DateTime queueTime = request.QueueTime;
      DateTime assignTime = request.AssignTime ?? DateTime.MinValue;
      DateTime? nullable = request.ReceiveTime;
      DateTime startTime = nullable ?? DateTime.MinValue;
      nullable = request.FinishTime;
      DateTime finishTime = nullable ?? DateTime.MinValue;
      TaskResult? result1 = request.Result;
      ref TaskResult? local = ref result1;
      string result2 = local.HasValue ? local.GetValueOrDefault().ToString() : (string) null;
      string planType = request.PlanType;
      Guid scopeId = request.ScopeId;
      string projectName1 = projectName;
      Guid planId = request.PlanId;
      Guid jobId = request.JobId;
      TaskOrchestrationOwner definition = request.Definition;
      int id1 = definition != null ? definition.Id : 0;
      string name1 = request.Definition?.Name;
      TaskOrchestrationOwner owner = request.Owner;
      int id2 = owner != null ? owner.Id : 0;
      string name2 = request.Owner?.Name;
      string agentCloudType = str2;
      string agentCloudName = str1;
      string agentSpecification = str3;
      TaskAgentReference reservedAgent = request.ReservedAgent;
      int id3 = reservedAgent != null ? reservedAgent.Id : 0;
      string name3 = request.ReservedAgent?.Name;
      string version = request.ReservedAgent?.Version;
      int poolId = request.PoolId;
      this.AgentPoolRequestHistory(hostId1, orchestrationId, requestId, poolName1, queueTime, assignTime, startTime, finishTime, result2, planType, scopeId, projectName1, planId, jobId, id1, name1, id2, name2, agentCloudType, agentCloudName, agentSpecification, id3, name3, version, poolId);
    }

    [NonEvent]
    public void PublishOrchestrationPlanContext(
      string orchestrationId,
      Guid authorizationId,
      Guid hostId,
      Guid projectId,
      string projectName,
      string planType,
      Guid planId,
      Guid jobId,
      string jobRefName,
      TaskOrchestrationOwner definition,
      TaskOrchestrationOwner owner,
      TaskInstance task = null)
    {
      this.OrchestrationPlanContext(authorizationId, hostId, orchestrationId, projectId, projectName, planType, planId, jobId, jobRefName, task != null ? task.InstanceId : Guid.Empty, task?.RefName, definition != null ? definition.Id : 0, definition?.Name, owner != null ? owner.Id : 0, owner?.Name);
    }

    [NonEvent]
    public void PublishTaskDefinitionInstallHistory(
      Guid hostId,
      string installType,
      string contributionIdentifier,
      Guid taskId,
      string task,
      string version)
    {
      this.TaskDefinitionInstallHistory(hostId, installType, contributionIdentifier, taskId, task, version);
    }

    public void TaskDefinitionInstallHistory(
      Guid hostId,
      string installType,
      string contributionIdentifier,
      Guid taskId,
      string task,
      string version)
    {
      var data = new
      {
        HostId = hostId,
        InstallType = installType,
        ContributionIdentifier = contributionIdentifier,
        TaskId = taskId,
        Task = task,
        Version = version
      };
      this.Write(nameof (TaskDefinitionInstallHistory), data);
    }

    public void AgentPoolRequestHistory(
      Guid hostId,
      string orchestrationId,
      long requestId,
      string poolName,
      DateTime queueTime,
      DateTime assignTime,
      DateTime startTime,
      DateTime finishTime,
      string result,
      string planType,
      Guid projectId,
      string projectName,
      Guid planId,
      Guid jobId,
      int definitionId,
      string definitionName,
      int ownerId,
      string ownerName,
      string agentCloudType,
      string agentCloudName,
      string agentSpecification,
      int agentId,
      string agentName,
      string agentVersion,
      int poolId)
    {
      var data = new
      {
        HostId = hostId,
        OrchestrationId = orchestrationId,
        RequestId = requestId,
        PoolName = poolName,
        QueueTime = queueTime,
        AssignTime = assignTime,
        StartTime = startTime,
        FinishTime = finishTime,
        Result = result,
        PlanType = planType,
        ProjectId = projectId,
        ProjectName = projectName,
        DefinitionId = definitionId,
        DefinitionName = definitionName,
        OwnerId = ownerId,
        OwnerName = ownerName,
        PlanId = planId,
        JobId = jobId,
        AgentCloudType = agentCloudType,
        AgentCloudName = agentCloudName,
        AgentSpecification = agentSpecification,
        AgentId = agentId,
        AgentName = agentName,
        AgentVersion = agentVersion,
        PoolId = poolId
      };
      this.Write(nameof (AgentPoolRequestHistory), data);
    }

    public void OrchestrationPlanContext(
      Guid authorizationId,
      Guid hostId,
      string orchestrationId,
      Guid projectId,
      string projectName,
      string planType,
      Guid planId,
      Guid jobId,
      string jobRefName,
      Guid taskId,
      string taskRefName,
      int definitionId,
      string definitionName,
      int ownerId,
      string ownerName)
    {
      var data = new
      {
        AuthorizationId = authorizationId,
        HostId = hostId,
        OrchestrationId = orchestrationId,
        ProjectId = projectId,
        ProjectName = projectName,
        PlanType = planType,
        PlanId = planId,
        JobId = jobId,
        JobRefName = jobRefName,
        TaskId = taskId,
        TaskRefName = taskRefName,
        DefinitionId = definitionId,
        DefinitionName = definitionName,
        OwnerId = ownerId,
        OwnerName = ownerName
      };
      this.Write(nameof (OrchestrationPlanContext), data);
    }
  }
}
