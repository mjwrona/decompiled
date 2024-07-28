// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.RunAgentEventBinderHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Tasks;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal static class RunAgentEventBinderHelper
  {
    private static SqlColumnBinder s_agentCloudId = new SqlColumnBinder("AgentCloudId");
    private static SqlColumnBinder s_agentCloudRequestId = new SqlColumnBinder("AgentCloudRequestId");
    private static SqlColumnBinder s_poolId = new SqlColumnBinder("PoolId");
    private static SqlColumnBinder s_agentId = new SqlColumnBinder("AgentId");

    public static void BindRunAgentEvent(RunAgentEvent agentEvent, SqlDataReader reader)
    {
      agentEvent.AgentCloudId = RunAgentEventBinderHelper.s_agentCloudId.GetInt32((IDataReader) reader);
      agentEvent.PoolId = RunAgentEventBinderHelper.s_poolId.GetInt32((IDataReader) reader);
      agentEvent.AgentId = RunAgentEventBinderHelper.s_agentId.GetInt32((IDataReader) reader);
      if (!RunAgentEventBinderHelper.s_agentCloudRequestId.ColumnExists((IDataReader) reader))
        return;
      agentEvent.AgentCloudRequestId = RunAgentEventBinderHelper.s_agentCloudRequestId.GetGuid((IDataReader) reader);
    }
  }
}
