// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskAgentMachineBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class TaskAgentMachineBinder : ObjectBinder<TaskAgentMachine>
  {
    private SqlColumnBinder m_name = new SqlColumnBinder("MachineName");
    private SqlColumnBinder m_agentCount = new SqlColumnBinder("AgentCount");
    private SqlColumnBinder m_activeAgentCount = new SqlColumnBinder("ActiveAgentCount");

    protected override TaskAgentMachine Bind() => new TaskAgentMachine()
    {
      Name = this.m_name.GetString((IDataReader) this.Reader, false),
      AgentCount = this.m_agentCount.GetInt32((IDataReader) this.Reader),
      ActiveAgentCount = this.m_activeAgentCount.GetInt32((IDataReader) this.Reader)
    };
  }
}
