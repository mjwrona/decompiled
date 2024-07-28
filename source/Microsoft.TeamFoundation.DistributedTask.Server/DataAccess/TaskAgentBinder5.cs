// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskAgentBinder5
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class TaskAgentBinder5 : ObjectBinder<TaskAgent>
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
      return taskAgent;
    }
  }
}
