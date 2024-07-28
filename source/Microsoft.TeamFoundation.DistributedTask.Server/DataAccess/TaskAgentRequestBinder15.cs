// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskAgentRequestBinder15
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
  internal sealed class TaskAgentRequestBinder15 : ObjectBinder<TaskAgentJobRequest>
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
    private SqlColumnBinder m_agentId = new SqlColumnBinder("AgentId");
    private SqlColumnBinder m_agentName = new SqlColumnBinder("AgentName");
    private SqlColumnBinder m_agentVersion = new SqlColumnBinder("AgentVersion");
    private SqlColumnBinder m_agentEnabled = new SqlColumnBinder("AgentEnabled");
    private SqlColumnBinder m_agentStatus = new SqlColumnBinder("AgentStatus");
    private SqlColumnBinder m_data = new SqlColumnBinder("Data");
    private SqlColumnBinder m_agentProvisioningState = new SqlColumnBinder("AgentProvisioningState");
    private SqlColumnBinder m_osDescription = new SqlColumnBinder("OSDescription");
    private SqlColumnBinder m_accessPoint = new SqlColumnBinder("AccessPoint");
    private SqlColumnBinder m_orchestrationId = new SqlColumnBinder("OrchestrationId");
    private SqlColumnBinder m_matchesAllAgents = new SqlColumnBinder("MatchesAllAgents");

    internal TaskAgentRequestBinder15()
    {
    }

    protected override TaskAgentJobRequest Bind()
    {
      TaskAgentJobRequest result = new TaskAgentJobRequest();
      result.PoolId = this.m_poolId.GetInt32((IDataReader) this.Reader);
      result.QueueId = this.m_queueId.GetNullableInt32((IDataReader) this.Reader);
      result.RequestId = this.m_requestId.GetInt64((IDataReader) this.Reader);
      result.QueueTime = this.m_queueTime.GetDateTime((IDataReader) this.Reader);
      result.AssignTime = this.m_assignTime.GetNullableDateTime((IDataReader) this.Reader);
      result.ReceiveTime = this.m_startTime.GetNullableDateTime((IDataReader) this.Reader);
      result.FinishTime = this.m_finishTime.GetNullableDateTime((IDataReader) this.Reader);
      TaskAgentJobRequest taskAgentJobRequest = result;
      byte? nullableByte = this.m_result.GetNullableByte((IDataReader) this.Reader);
      TaskResult? nullable = nullableByte.HasValue ? new TaskResult?((TaskResult) nullableByte.GetValueOrDefault()) : new TaskResult?();
      taskAgentJobRequest.Result = nullable;
      result.LockedUntil = this.m_lockedUntil.GetNullableDateTime((IDataReader) this.Reader);
      result.ServiceOwner = this.m_serviceOwner.GetGuid((IDataReader) this.Reader, false);
      result.HostId = this.m_hostId.GetGuid((IDataReader) this.Reader);
      result.ScopeId = this.m_scopeId.GetGuid((IDataReader) this.Reader, true, Guid.Empty);
      result.PlanType = this.m_planType.GetString((IDataReader) this.Reader, true);
      result.PlanId = this.m_planId.GetGuid((IDataReader) this.Reader);
      result.JobId = this.m_jobId.GetGuid((IDataReader) this.Reader);
      result.JobName = this.m_jobName.GetString((IDataReader) this.Reader, true);
      result.Demands = JsonUtility.Deserialize<IList<Demand>>(this.m_demands.GetBytes((IDataReader) this.Reader, true));
      result.AgentSpecification = JsonUtility.Deserialize<JObject>(this.m_agentSpec.GetBytes((IDataReader) this.Reader, true));
      result.Definition = JsonUtility.Deserialize<TaskOrchestrationOwner>(this.m_definitionReference.GetBytes((IDataReader) this.Reader, true));
      result.Owner = JsonUtility.Deserialize<TaskOrchestrationOwner>(this.m_ownerReference.GetBytes((IDataReader) this.Reader, true));
      if (this.m_orchestrationId.ColumnExists((IDataReader) this.Reader))
        result.OrchestrationId = this.m_orchestrationId.GetString((IDataReader) this.Reader, true);
      result.MatchesAllAgentsInPool = this.m_matchesAllAgents.GetBoolean((IDataReader) this.Reader);
      IDictionary<string, string> data = JsonUtility.Deserialize<IDictionary<string, string>>(this.m_data.GetBytes((IDataReader) this.Reader, true));
      this.AddData(result, data);
      if (!this.m_agentId.IsNull((IDataReader) this.Reader))
        result.ReservedAgent = new TaskAgentReference()
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
      return result;
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
