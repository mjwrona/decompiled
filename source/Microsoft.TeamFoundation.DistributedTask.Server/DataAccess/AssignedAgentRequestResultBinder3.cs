// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.AssignedAgentRequestResultBinder3
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class AssignedAgentRequestResultBinder3 : ObjectBinder<AssignedAgentRequestResult>
  {
    private SqlColumnBinder m_poolId = new SqlColumnBinder("PoolId");
    private SqlColumnBinder m_queueId = new SqlColumnBinder("QueueId");
    private SqlColumnBinder m_requestId = new SqlColumnBinder("RequestId");
    private SqlColumnBinder m_queueTime = new SqlColumnBinder("QueueTime");
    private SqlColumnBinder m_assignTime = new SqlColumnBinder("AssignTime");
    private SqlColumnBinder m_startTime = new SqlColumnBinder("StartTime");
    private SqlColumnBinder m_finishTime = new SqlColumnBinder("FinishTime");
    private SqlColumnBinder m_result = new SqlColumnBinder("Result");
    private SqlColumnBinder m_lockedUntil = new SqlColumnBinder("LockedUntil");
    private SqlColumnBinder m_serviceOwner = new SqlColumnBinder("ServiceOwner");
    private SqlColumnBinder m_hostId = new SqlColumnBinder("HostId");
    private SqlColumnBinder m_scopeId = new SqlColumnBinder("ScopeId");
    private SqlColumnBinder m_planType = new SqlColumnBinder("PlanType");
    private SqlColumnBinder m_planId = new SqlColumnBinder("PlanId");
    private SqlColumnBinder m_jobId = new SqlColumnBinder("JobId");
    private SqlColumnBinder m_jobName = new SqlColumnBinder("JobName");
    private SqlColumnBinder m_demands = new SqlColumnBinder("Demands");
    private SqlColumnBinder m_agentSpec = new SqlColumnBinder("AgentSpecification");
    private SqlColumnBinder m_definitionReference = new SqlColumnBinder("DefinitionReference");
    private SqlColumnBinder m_ownerReference = new SqlColumnBinder("OwnerReference");
    private SqlColumnBinder m_orchestrationId = new SqlColumnBinder("OrchestrationId");
    private SqlColumnBinder m_agentId = new SqlColumnBinder("AgentId");
    private SqlColumnBinder m_agentName = new SqlColumnBinder("AgentName");
    private SqlColumnBinder m_agentVersion = new SqlColumnBinder("AgentVersion");
    private SqlColumnBinder m_agentEnabled = new SqlColumnBinder("AgentEnabled");
    private SqlColumnBinder m_agentStatus = new SqlColumnBinder("AgentStatus");
    private SqlColumnBinder m_data = new SqlColumnBinder("Data");
    private SqlColumnBinder m_agentProvisioningState = new SqlColumnBinder("AgentProvisioningState");
    private SqlColumnBinder m_osDescription = new SqlColumnBinder("OSDescription");
    private SqlColumnBinder m_accessPoint = new SqlColumnBinder("AccessPoint");
    private SqlColumnBinder m_matchestAllAgents = new SqlColumnBinder("MatchesAllAgents");
    private SqlColumnBinder m_previousAgentCloudId = new SqlColumnBinder("PreviousAgentCloudId");
    private SqlColumnBinder m_previousName = new SqlColumnBinder("PreviousAgentCloudName");
    private SqlColumnBinder m_previousType = new SqlColumnBinder("PreviousAgentCloudType");
    private SqlColumnBinder m_previousInternal = new SqlColumnBinder("PreviousAgentCloudInternal");
    private SqlColumnBinder m_previousGetAgentDefinition = new SqlColumnBinder("PreviousAgentCloudGetAgentDefinitionEndpoint");
    private SqlColumnBinder m_previousAcquireAgent = new SqlColumnBinder("PreviousAgentCloudAcquireAgentEndpoint");
    private SqlColumnBinder m_previousGetAgentRequestStatus = new SqlColumnBinder("PreviousAgentCloudGetAgentRequestStatusEndpoint");
    private SqlColumnBinder m_previousReleaseAgent = new SqlColumnBinder("PreviousAgentCloudReleaseAgentEndpoint");
    private SqlColumnBinder m_previousAcquisitionTimeout = new SqlColumnBinder("PreviousAgentCloudAcquisitionTimeout");
    private SqlColumnBinder m_currentAgentCloudId = new SqlColumnBinder("CurrentAgentCloudId");
    private SqlColumnBinder m_currentName = new SqlColumnBinder("CurrentAgentCloudName");
    private SqlColumnBinder m_currentType = new SqlColumnBinder("CurrentAgentCloudType");
    private SqlColumnBinder m_currentInternal = new SqlColumnBinder("CurrentAgentCloudInternal");
    private SqlColumnBinder m_currentGetAgentDefinition = new SqlColumnBinder("CurrentAgentCloudGetAgentDefinitionEndpoint");
    private SqlColumnBinder m_currentAcquireAgent = new SqlColumnBinder("CurrentAgentCloudAcquireAgentEndpoint");
    private SqlColumnBinder m_currentGetAgentRequestStatus = new SqlColumnBinder("CurrentAgentCloudGetAgentRequestStatusEndpoint");
    private SqlColumnBinder m_currentReleaseAgent = new SqlColumnBinder("CurrentAgentCloudReleaseAgentEndpoint");
    private SqlColumnBinder m_currentAcquisitionTimeout = new SqlColumnBinder("CurrentAgentCloudAcquisitionTimeout");

    protected override AssignedAgentRequestResult Bind()
    {
      TaskAgentJobRequest taskAgentJobRequest1 = new TaskAgentJobRequest();
      taskAgentJobRequest1.PoolId = this.m_poolId.GetInt32((IDataReader) this.Reader);
      taskAgentJobRequest1.QueueId = this.m_queueId.GetNullableInt32((IDataReader) this.Reader);
      taskAgentJobRequest1.RequestId = this.m_requestId.GetInt64((IDataReader) this.Reader);
      taskAgentJobRequest1.QueueTime = this.m_queueTime.GetDateTime((IDataReader) this.Reader);
      taskAgentJobRequest1.AssignTime = this.m_assignTime.GetNullableDateTime((IDataReader) this.Reader);
      taskAgentJobRequest1.ReceiveTime = this.m_startTime.GetNullableDateTime((IDataReader) this.Reader);
      taskAgentJobRequest1.FinishTime = this.m_finishTime.GetNullableDateTime((IDataReader) this.Reader);
      TaskAgentJobRequest taskAgentJobRequest2 = taskAgentJobRequest1;
      byte? nullableByte = this.m_result.GetNullableByte((IDataReader) this.Reader);
      TaskResult? nullable = nullableByte.HasValue ? new TaskResult?((TaskResult) nullableByte.GetValueOrDefault()) : new TaskResult?();
      taskAgentJobRequest2.Result = nullable;
      taskAgentJobRequest1.LockedUntil = this.m_lockedUntil.GetNullableDateTime((IDataReader) this.Reader);
      taskAgentJobRequest1.ServiceOwner = this.m_serviceOwner.GetGuid((IDataReader) this.Reader, false);
      taskAgentJobRequest1.HostId = this.m_hostId.GetGuid((IDataReader) this.Reader);
      taskAgentJobRequest1.ScopeId = this.m_scopeId.GetGuid((IDataReader) this.Reader, true, Guid.Empty);
      taskAgentJobRequest1.PlanType = this.m_planType.GetString((IDataReader) this.Reader, true);
      taskAgentJobRequest1.PlanId = this.m_planId.GetGuid((IDataReader) this.Reader);
      taskAgentJobRequest1.JobId = this.m_jobId.GetGuid((IDataReader) this.Reader);
      taskAgentJobRequest1.JobName = this.m_jobName.GetString((IDataReader) this.Reader, true);
      taskAgentJobRequest1.Demands = JsonUtility.Deserialize<IList<Demand>>(this.m_demands.GetBytes((IDataReader) this.Reader, true));
      taskAgentJobRequest1.AgentSpecification = JsonUtility.Deserialize<JObject>(this.m_agentSpec.GetBytes((IDataReader) this.Reader, true));
      taskAgentJobRequest1.Definition = JsonUtility.Deserialize<TaskOrchestrationOwner>(this.m_definitionReference.GetBytes((IDataReader) this.Reader, true));
      taskAgentJobRequest1.Owner = JsonUtility.Deserialize<TaskOrchestrationOwner>(this.m_ownerReference.GetBytes((IDataReader) this.Reader, true));
      if (this.m_orchestrationId.ColumnExists((IDataReader) this.Reader))
        taskAgentJobRequest1.OrchestrationId = this.m_orchestrationId.GetString((IDataReader) this.Reader, true);
      IDictionary<string, string> data = JsonUtility.Deserialize<IDictionary<string, string>>(this.m_data.GetBytes((IDataReader) this.Reader, true));
      this.AddData(taskAgentJobRequest1, data);
      taskAgentJobRequest1.MatchesAllAgentsInPool = this.m_matchestAllAgents.GetBoolean((IDataReader) this.Reader);
      if (!this.m_agentId.IsNull((IDataReader) this.Reader))
        taskAgentJobRequest1.ReservedAgent = new TaskAgentReference()
        {
          Id = this.m_agentId.GetInt32((IDataReader) this.Reader),
          Name = this.m_agentName.GetString((IDataReader) this.Reader, false),
          Version = this.m_agentVersion.GetString((IDataReader) this.Reader, false),
          Enabled = new bool?(this.m_agentEnabled.GetBoolean((IDataReader) this.Reader)),
          Status = (TaskAgentStatus) this.m_agentStatus.GetInt32((IDataReader) this.Reader),
          ProvisioningState = this.m_agentProvisioningState.GetString((IDataReader) this.Reader, false),
          OSDescription = this.m_osDescription.GetString((IDataReader) this.Reader, true),
          AccessPoint = this.m_accessPoint.GetString((IDataReader) this.Reader, true)
        };
      TaskAgentCloud previousAgentCloud = (TaskAgentCloud) null;
      int int32_1 = this.m_previousAgentCloudId.GetInt32((IDataReader) this.Reader, 0);
      if (int32_1 > 0)
      {
        previousAgentCloud = new TaskAgentCloud();
        previousAgentCloud.AgentCloudId = int32_1;
        previousAgentCloud.Name = this.m_previousName.GetString((IDataReader) this.Reader, false);
        previousAgentCloud.GetAgentDefinitionEndpoint = this.m_previousGetAgentDefinition.GetString((IDataReader) this.Reader, true);
        previousAgentCloud.AcquireAgentEndpoint = this.m_previousAcquireAgent.GetString((IDataReader) this.Reader, true);
        previousAgentCloud.GetAgentRequestStatusEndpoint = this.m_previousGetAgentRequestStatus.GetString((IDataReader) this.Reader, true);
        previousAgentCloud.ReleaseAgentEndpoint = this.m_previousReleaseAgent.GetString((IDataReader) this.Reader, true);
        previousAgentCloud.Type = this.m_previousType.GetString((IDataReader) this.Reader, false);
        previousAgentCloud.Internal = new bool?(this.m_previousInternal.GetBoolean((IDataReader) this.Reader));
        previousAgentCloud.AcquisitionTimeout = new int?(this.m_previousAcquisitionTimeout.GetInt32((IDataReader) this.Reader, 0));
      }
      TaskAgentCloud currentAgentCloud = (TaskAgentCloud) null;
      int int32_2 = this.m_currentAgentCloudId.GetInt32((IDataReader) this.Reader, 0);
      if (int32_2 > 0)
      {
        currentAgentCloud = new TaskAgentCloud();
        currentAgentCloud.AgentCloudId = int32_2;
        currentAgentCloud.Name = this.m_currentName.GetString((IDataReader) this.Reader, false);
        currentAgentCloud.GetAgentDefinitionEndpoint = this.m_currentGetAgentDefinition.GetString((IDataReader) this.Reader, true);
        currentAgentCloud.AcquireAgentEndpoint = this.m_currentAcquireAgent.GetString((IDataReader) this.Reader, true);
        currentAgentCloud.GetAgentRequestStatusEndpoint = this.m_currentGetAgentRequestStatus.GetString((IDataReader) this.Reader, true);
        currentAgentCloud.ReleaseAgentEndpoint = this.m_currentReleaseAgent.GetString((IDataReader) this.Reader, true);
        currentAgentCloud.Type = this.m_currentType.GetString((IDataReader) this.Reader, false);
        currentAgentCloud.Internal = new bool?(this.m_currentInternal.GetBoolean((IDataReader) this.Reader));
        currentAgentCloud.AcquisitionTimeout = new int?(this.m_currentAcquisitionTimeout.GetInt32((IDataReader) this.Reader, 0));
      }
      return new AssignedAgentRequestResult(taskAgentJobRequest1, previousAgentCloud, currentAgentCloud);
    }

    private void AddData(TaskAgentJobRequest result, IDictionary<string, string> data)
    {
      if (data == null)
        return;
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) data)
        result.Data[keyValuePair.Key] = keyValuePair.Value;
    }
  }
}
