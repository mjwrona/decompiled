// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskAgentCloudRequestBinder3
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class TaskAgentCloudRequestBinder3 : ObjectBinder<TaskAgentCloudRequest>
  {
    private SqlColumnBinder m_agentCloudId = new SqlColumnBinder("AgentCloudId");
    private SqlColumnBinder m_requestId = new SqlColumnBinder("RequestId");
    private SqlColumnBinder m_poolId = new SqlColumnBinder("PoolId");
    private SqlColumnBinder m_agentId = new SqlColumnBinder("AgentId");
    private SqlColumnBinder m_agentSpec = new SqlColumnBinder("AgentSpecification");
    private SqlColumnBinder m_agentData = new SqlColumnBinder("AgentData");
    private SqlColumnBinder m_provisionRequestTime = new SqlColumnBinder("ProvisionRequestTime");
    private SqlColumnBinder m_provisionedTime = new SqlColumnBinder("ProvisionedTime");
    private SqlColumnBinder m_agentConnectedTime = new SqlColumnBinder("AgentConnectedTime");
    private SqlColumnBinder m_releaseRequestTime = new SqlColumnBinder("ReleaseRequestTime");

    protected override TaskAgentCloudRequest Bind()
    {
      TaskAgentCloudRequest agentCloudRequest = new TaskAgentCloudRequest()
      {
        AgentCloudId = this.m_agentCloudId.GetInt32((IDataReader) this.Reader),
        RequestId = this.m_requestId.GetGuid((IDataReader) this.Reader),
        AgentSpecification = JsonUtility.Deserialize<JObject>(this.m_agentSpec.GetBytes((IDataReader) this.Reader, true)),
        AgentData = JsonUtility.Deserialize<JObject>(this.m_agentData.GetBytes((IDataReader) this.Reader, true)),
        ProvisionRequestTime = this.m_provisionRequestTime.GetNullableDateTime((IDataReader) this.Reader),
        ProvisionedTime = this.m_provisionedTime.GetNullableDateTime((IDataReader) this.Reader),
        AgentConnectedTime = this.m_agentConnectedTime.GetNullableDateTime((IDataReader) this.Reader),
        ReleaseRequestTime = this.m_releaseRequestTime.GetNullableDateTime((IDataReader) this.Reader),
        Pool = new TaskAgentPoolReference()
      };
      agentCloudRequest.Pool.Id = this.m_poolId.GetInt32((IDataReader) this.Reader);
      agentCloudRequest.Agent = new TaskAgentReference();
      agentCloudRequest.Agent.Id = this.m_agentId.GetInt32((IDataReader) this.Reader);
      return agentCloudRequest;
    }
  }
}
