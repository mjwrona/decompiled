// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskAgentPoolMaintenanceDefinitionBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class TaskAgentPoolMaintenanceDefinitionBinder : 
    ObjectBinder<TaskAgentPoolMaintenanceDefinition>
  {
    private SqlColumnBinder m_enabled = new SqlColumnBinder("Enabled");
    private SqlColumnBinder m_id = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder m_poolId = new SqlColumnBinder("PoolId");
    private SqlColumnBinder m_timeout = new SqlColumnBinder("JobTimeout");
    private SqlColumnBinder m_agentsConcurrent = new SqlColumnBinder("AgentsConcurrent");
    private SqlColumnBinder m_options = new SqlColumnBinder("Options");
    private SqlColumnBinder m_scheduleSetting = new SqlColumnBinder("ScheduleSetting");
    private SqlColumnBinder m_retentionPolicy = new SqlColumnBinder("RetentionPolicy");

    protected override TaskAgentPoolMaintenanceDefinition Bind() => new TaskAgentPoolMaintenanceDefinition()
    {
      Enabled = this.m_enabled.GetBoolean((IDataReader) this.Reader),
      Id = this.m_id.GetInt32((IDataReader) this.Reader),
      Pool = new TaskAgentPoolReference()
      {
        Id = this.m_poolId.GetInt32((IDataReader) this.Reader)
      },
      JobTimeoutInMinutes = this.m_timeout.GetInt32((IDataReader) this.Reader),
      MaxConcurrentAgentsPercentage = this.m_agentsConcurrent.GetInt32((IDataReader) this.Reader),
      Options = JsonUtility.FromString<TaskAgentPoolMaintenanceOptions>(this.m_options.GetString((IDataReader) this.Reader, true)),
      ScheduleSetting = JsonUtility.FromString<TaskAgentPoolMaintenanceSchedule>(this.m_scheduleSetting.GetString((IDataReader) this.Reader, true)),
      RetentionPolicy = JsonUtility.FromString<TaskAgentPoolMaintenanceRetentionPolicy>(this.m_retentionPolicy.GetString((IDataReader) this.Reader, true))
    };
  }
}
