// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskAgentPoolMaintenanceJobTargetAgentBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class TaskAgentPoolMaintenanceJobTargetAgentBinder : 
    ObjectBinder<TaskAgentPoolMaintenanceJobTargetAgent>
  {
    private SqlColumnBinder m_jobId = new SqlColumnBinder("JobId");
    private SqlColumnBinder m_agentId = new SqlColumnBinder("AgentId");
    private SqlColumnBinder m_name = new SqlColumnBinder("AgentName");

    protected override TaskAgentPoolMaintenanceJobTargetAgent Bind() => new TaskAgentPoolMaintenanceJobTargetAgent()
    {
      JobId = this.m_jobId.GetInt32((IDataReader) this.Reader),
      Agent = new TaskAgentReference()
      {
        Id = this.m_agentId.GetInt32((IDataReader) this.Reader),
        Name = this.m_name.GetString((IDataReader) this.Reader, false)
      }
    };
  }
}
