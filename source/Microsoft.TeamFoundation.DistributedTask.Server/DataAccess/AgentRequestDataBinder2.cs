// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.AgentRequestDataBinder2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class AgentRequestDataBinder2 : ObjectBinder<AgentRequestData>
  {
    private SqlColumnBinder requestId = new SqlColumnBinder("RequestId");
    private SqlColumnBinder poolId = new SqlColumnBinder("PoolId");
    private SqlColumnBinder planId = new SqlColumnBinder("PlanId");
    private SqlColumnBinder jobId = new SqlColumnBinder("JobId");
    private SqlColumnBinder jobName = new SqlColumnBinder("JobName");
    private SqlColumnBinder queueTime = new SqlColumnBinder("QueueTime");
    private SqlColumnBinder assignTime = new SqlColumnBinder("AssignTime");
    private SqlColumnBinder startTime = new SqlColumnBinder("StartTime");
    private SqlColumnBinder finishTime = new SqlColumnBinder("FinishTime");
    private SqlColumnBinder result = new SqlColumnBinder("Result");
    private SqlColumnBinder scopeId = new SqlColumnBinder("ScopeId");
    private SqlColumnBinder definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder planType = new SqlColumnBinder("PlanType");
    private SqlColumnBinder reservedAgentId = new SqlColumnBinder("ReservedAgentId");
    private SqlColumnBinder lastUpdated = new SqlColumnBinder("LastUpdated");

    protected override AgentRequestData Bind()
    {
      AgentRequestData agentRequestData1 = new AgentRequestData();
      agentRequestData1.RequestId = this.requestId.GetInt64((IDataReader) this.Reader);
      agentRequestData1.PoolId = this.poolId.GetInt32((IDataReader) this.Reader);
      agentRequestData1.PlanId = this.planId.GetGuid((IDataReader) this.Reader);
      agentRequestData1.JobId = this.jobId.GetGuid((IDataReader) this.Reader);
      agentRequestData1.JobName = this.jobName.GetString((IDataReader) this.Reader, true);
      agentRequestData1.QueueTime = this.queueTime.GetDateTime((IDataReader) this.Reader);
      agentRequestData1.AssignTime = this.assignTime.GetNullableDateTime((IDataReader) this.Reader);
      agentRequestData1.StartTime = this.startTime.GetNullableDateTime((IDataReader) this.Reader);
      agentRequestData1.FinishTime = this.finishTime.GetNullableDateTime((IDataReader) this.Reader);
      AgentRequestData agentRequestData2 = agentRequestData1;
      byte? nullableByte = this.result.GetNullableByte((IDataReader) this.Reader);
      TaskResult? nullable = nullableByte.HasValue ? new TaskResult?((TaskResult) nullableByte.GetValueOrDefault()) : new TaskResult?();
      agentRequestData2.Result = nullable;
      agentRequestData1.ProjectGuid = this.scopeId.GetNullableGuid((IDataReader) this.Reader);
      agentRequestData1.PipelineId = this.definitionId.GetNullableInt32((IDataReader) this.Reader);
      agentRequestData1.PipelineType = this.planType.GetString((IDataReader) this.Reader, true);
      if (!this.reservedAgentId.IsNull((IDataReader) this.Reader))
        agentRequestData1.AgentId = new int?(this.reservedAgentId.GetInt32((IDataReader) this.Reader));
      agentRequestData1.LastUpdated = this.lastUpdated.GetDateTime((IDataReader) this.Reader);
      return agentRequestData1;
    }
  }
}
