// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.DeprovisionedAgentResultBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class DeprovisionedAgentResultBinder : ObjectBinder<DeprovisioningAgentResult>
  {
    private SqlColumnBinder m_id = new SqlColumnBinder("AgentId");
    private SqlColumnBinder m_agentName = new SqlColumnBinder("AgentName");
    private SqlColumnBinder m_version = new SqlColumnBinder("AgentVersion");
    private SqlColumnBinder m_maxParallelism = new SqlColumnBinder("MaxParallelism");
    private SqlColumnBinder m_createdOn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder m_enabled = new SqlColumnBinder("Enabled");
    private SqlColumnBinder m_status = new SqlColumnBinder("Status");
    private SqlColumnBinder m_clientId = new SqlColumnBinder("ClientId");
    private SqlColumnBinder m_publicKey = new SqlColumnBinder("PublicKey");
    private SqlColumnBinder m_sourceVersion = new SqlColumnBinder("SourceVersion");
    private SqlColumnBinder m_targetVersion = new SqlColumnBinder("TargetVersion");
    private SqlColumnBinder m_currentState = new SqlColumnBinder("CurrentState");
    private SqlColumnBinder m_requestTime = new SqlColumnBinder("RequestTime");
    private SqlColumnBinder m_requestedBy = new SqlColumnBinder("RequestedBy");
    private SqlColumnBinder m_reason = new SqlColumnBinder("Reason");
    private SqlColumnBinder m_data = new SqlColumnBinder("Data");
    private SqlColumnBinder m_definitionReference = new SqlColumnBinder("DefinitionReference");
    private SqlColumnBinder m_ownerReference = new SqlColumnBinder("OwnerReference");
    private SqlColumnBinder m_osDescription = new SqlColumnBinder("OSDescription");
    private SqlColumnBinder m_provisioningState = new SqlColumnBinder("ProvisioningState");
    private SqlColumnBinder m_accessPoint = new SqlColumnBinder("AccessPoint");
    private SqlColumnBinder m_agentCloudId = new SqlColumnBinder("AgentCloudId");
    private SqlColumnBinder m_agentCloudName = new SqlColumnBinder("Name");
    private SqlColumnBinder m_type = new SqlColumnBinder("Type");
    private SqlColumnBinder m_internal = new SqlColumnBinder("Internal");
    private SqlColumnBinder m_getAgentDefinition = new SqlColumnBinder("GetAgentDefinitionEndpoint");
    private SqlColumnBinder m_acquireAgent = new SqlColumnBinder("AcquireAgentEndpoint");
    private SqlColumnBinder m_getAgentRequestStatus = new SqlColumnBinder("GetAgentRequestStatusEndpoint");
    private SqlColumnBinder m_releaseAgent = new SqlColumnBinder("ReleaseAgentEndpoint");

    protected override DeprovisioningAgentResult Bind()
    {
      TaskAgent agent = new TaskAgent();
      agent.Id = this.m_id.GetInt32((IDataReader) this.Reader);
      agent.Name = this.m_agentName.GetString((IDataReader) this.Reader, false);
      agent.Version = this.m_version.GetString((IDataReader) this.Reader, false);
      agent.OSDescription = this.m_osDescription.GetString((IDataReader) this.Reader, true);
      agent.AccessPoint = this.m_accessPoint.GetString((IDataReader) this.Reader, true);
      agent.MaxParallelism = new int?(this.m_maxParallelism.GetInt32((IDataReader) this.Reader));
      agent.CreatedOn = this.m_createdOn.GetDateTime((IDataReader) this.Reader);
      agent.Enabled = new bool?(this.m_enabled.GetBoolean((IDataReader) this.Reader));
      agent.Status = (TaskAgentStatus) this.m_status.GetInt32((IDataReader) this.Reader);
      agent.ProvisioningState = this.m_provisioningState.GetString((IDataReader) this.Reader, false);
      agent.Authorization = new TaskAgentAuthorization()
      {
        ClientId = this.m_clientId.GetGuid((IDataReader) this.Reader),
        PublicKey = JsonUtility.Deserialize<TaskAgentPublicKey>(this.m_publicKey.GetBytes((IDataReader) this.Reader, true))
      };
      if (!this.m_targetVersion.IsNull((IDataReader) this.Reader))
      {
        agent.PendingUpdate = new TaskAgentUpdate()
        {
          SourceVersion = new PackageVersion(this.m_sourceVersion.GetString((IDataReader) this.Reader, false)),
          TargetVersion = new PackageVersion(this.m_targetVersion.GetString((IDataReader) this.Reader, false)),
          CurrentState = this.m_currentState.GetString((IDataReader) this.Reader, true),
          RequestTime = new DateTime?(this.m_requestTime.GetDateTime((IDataReader) this.Reader)),
          RequestedBy = new IdentityRef()
          {
            Id = this.m_requestedBy.GetGuid((IDataReader) this.Reader).ToString("D")
          }
        };
        switch ((TaskAgentUpdateReasonType) this.m_reason.GetByte((IDataReader) this.Reader))
        {
          case TaskAgentUpdateReasonType.Manual:
            agent.PendingUpdate.Reason = (TaskAgentUpdateReason) new TaskAgentManualUpdate();
            break;
          case TaskAgentUpdateReasonType.MinAgentVersionRequired:
            agent.PendingUpdate.Reason = (TaskAgentUpdateReason) new TaskAgentMinAgentVersionRequiredUpdate()
            {
              MinAgentVersion = JsonUtility.FromString<Demand>(this.m_data.GetString((IDataReader) this.Reader, true)),
              JobDefinition = JsonUtility.Deserialize<TaskOrchestrationOwner>(this.m_definitionReference.GetBytes((IDataReader) this.Reader, true)),
              JobOwner = JsonUtility.Deserialize<TaskOrchestrationOwner>(this.m_ownerReference.GetBytes((IDataReader) this.Reader, true))
            };
            break;
        }
      }
      TaskAgentCloud agentCloud = (TaskAgentCloud) null;
      if (this.m_agentCloudId.GetInt32((IDataReader) this.Reader, 0) > 0)
      {
        agentCloud = new TaskAgentCloud();
        agentCloud.AgentCloudId = this.m_agentCloudId.GetInt32((IDataReader) this.Reader);
        agentCloud.Name = this.m_agentCloudName.GetString((IDataReader) this.Reader, false);
        agentCloud.GetAgentDefinitionEndpoint = this.m_getAgentDefinition.GetString((IDataReader) this.Reader, true);
        agentCloud.AcquireAgentEndpoint = this.m_acquireAgent.GetString((IDataReader) this.Reader, true);
        agentCloud.GetAgentRequestStatusEndpoint = this.m_getAgentRequestStatus.GetString((IDataReader) this.Reader, true);
        agentCloud.ReleaseAgentEndpoint = this.m_releaseAgent.GetString((IDataReader) this.Reader, true);
        agentCloud.Type = this.m_type.GetString((IDataReader) this.Reader, false);
        agentCloud.Internal = new bool?(this.m_internal.GetBoolean((IDataReader) this.Reader));
      }
      return new DeprovisioningAgentResult(agent, agentCloud);
    }
  }
}
