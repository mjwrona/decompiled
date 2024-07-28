// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskAgentBinder7
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
  internal sealed class TaskAgentBinder7 : ObjectBinder<TaskAgent>
  {
    private SqlColumnBinder m_id = new SqlColumnBinder("AgentId");
    private SqlColumnBinder m_name = new SqlColumnBinder("AgentName");
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

    protected override TaskAgent Bind()
    {
      TaskAgent taskAgent = new TaskAgent();
      taskAgent.Id = this.m_id.GetInt32((IDataReader) this.Reader);
      taskAgent.Name = this.m_name.GetString((IDataReader) this.Reader, false);
      taskAgent.Version = this.m_version.GetString((IDataReader) this.Reader, false);
      taskAgent.MaxParallelism = new int?(this.m_maxParallelism.GetInt32((IDataReader) this.Reader));
      taskAgent.CreatedOn = this.m_createdOn.GetDateTime((IDataReader) this.Reader);
      taskAgent.Enabled = new bool?(this.m_enabled.GetBoolean((IDataReader) this.Reader));
      taskAgent.Status = (TaskAgentStatus) this.m_status.GetInt32((IDataReader) this.Reader);
      taskAgent.Authorization = new TaskAgentAuthorization()
      {
        ClientId = this.m_clientId.GetGuid((IDataReader) this.Reader),
        PublicKey = JsonUtility.Deserialize<TaskAgentPublicKey>(this.m_publicKey.GetBytes((IDataReader) this.Reader, true))
      };
      if (!this.m_targetVersion.IsNull((IDataReader) this.Reader))
      {
        taskAgent.PendingUpdate = new TaskAgentUpdate()
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
            taskAgent.PendingUpdate.Reason = (TaskAgentUpdateReason) new TaskAgentManualUpdate();
            break;
          case TaskAgentUpdateReasonType.MinAgentVersionRequired:
            taskAgent.PendingUpdate.Reason = (TaskAgentUpdateReason) new TaskAgentMinAgentVersionRequiredUpdate()
            {
              MinAgentVersion = JsonUtility.FromString<Demand>(this.m_data.GetString((IDataReader) this.Reader, true)),
              JobDefinition = JsonUtility.Deserialize<TaskOrchestrationOwner>(this.m_definitionReference.GetBytes((IDataReader) this.Reader, true)),
              JobOwner = JsonUtility.Deserialize<TaskOrchestrationOwner>(this.m_ownerReference.GetBytes((IDataReader) this.Reader, true))
            };
            break;
        }
      }
      return taskAgent;
    }
  }
}
