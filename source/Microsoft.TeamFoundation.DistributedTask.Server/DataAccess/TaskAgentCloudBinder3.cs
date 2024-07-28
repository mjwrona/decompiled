// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskAgentCloudBinder3
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class TaskAgentCloudBinder3 : ObjectBinder<TaskAgentCloud>
  {
    private SqlColumnBinder m_agentCloudId = new SqlColumnBinder("AgentCloudId");
    private SqlColumnBinder m_name = new SqlColumnBinder("Name");
    private SqlColumnBinder m_type = new SqlColumnBinder("Type");
    private SqlColumnBinder m_internal = new SqlColumnBinder("Internal");
    private SqlColumnBinder m_getAgentDefinition = new SqlColumnBinder("GetAgentDefinitionEndpoint");
    private SqlColumnBinder m_acquireAgent = new SqlColumnBinder("AcquireAgentEndpoint");
    private SqlColumnBinder m_getAgentRequestStatus = new SqlColumnBinder("GetAgentRequestStatusEndpoint");
    private SqlColumnBinder m_releaseAgent = new SqlColumnBinder("ReleaseAgentEndpoint");

    protected override TaskAgentCloud Bind() => new TaskAgentCloud()
    {
      AgentCloudId = this.m_agentCloudId.GetInt32((IDataReader) this.Reader),
      Name = this.m_name.GetString((IDataReader) this.Reader, false),
      GetAgentDefinitionEndpoint = this.m_getAgentDefinition.GetString((IDataReader) this.Reader, true),
      AcquireAgentEndpoint = this.m_acquireAgent.GetString((IDataReader) this.Reader, true),
      GetAgentRequestStatusEndpoint = this.m_getAgentRequestStatus.GetString((IDataReader) this.Reader, true),
      ReleaseAgentEndpoint = this.m_releaseAgent.GetString((IDataReader) this.Reader, true),
      Type = this.m_type.GetString((IDataReader) this.Reader, false),
      Internal = new bool?(this.m_internal.GetBoolean((IDataReader) this.Reader))
    };
  }
}
