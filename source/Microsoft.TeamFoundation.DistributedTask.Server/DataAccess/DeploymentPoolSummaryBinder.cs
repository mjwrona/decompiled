// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.DeploymentPoolSummaryBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class DeploymentPoolSummaryBinder : ObjectBinder<DeploymentPoolSummary>
  {
    private Guid m_scopeId;
    private SqlColumnBinder m_id = new SqlColumnBinder("PoolId");
    private SqlColumnBinder m_name = new SqlColumnBinder("PoolName");
    private SqlColumnBinder m_poolType = new SqlColumnBinder("PoolType");
    private SqlColumnBinder m_size = new SqlColumnBinder("Size");
    private SqlColumnBinder m_isHosted = new SqlColumnBinder("IsHosted");
    private SqlColumnBinder m_onlineCount = new SqlColumnBinder("OnlineCount");
    private SqlColumnBinder m_offlineCount = new SqlColumnBinder("OfflineCount");

    public DeploymentPoolSummaryBinder(IVssRequestContext requestContext) => this.m_scopeId = requestContext.ServiceHost.InstanceId;

    protected override DeploymentPoolSummary Bind() => new DeploymentPoolSummary()
    {
      Pool = new TaskAgentPoolReference()
      {
        Scope = this.m_scopeId,
        Id = this.m_id.GetInt32((IDataReader) this.Reader),
        Name = this.m_name.GetString((IDataReader) this.Reader, false),
        PoolType = (TaskAgentPoolType) this.m_poolType.GetByte((IDataReader) this.Reader),
        Size = this.m_size.GetInt32((IDataReader) this.Reader),
        IsHosted = this.m_isHosted.GetBoolean((IDataReader) this.Reader, false)
      },
      OnlineAgentsCount = this.m_onlineCount.GetInt32((IDataReader) this.Reader),
      OfflineAgentsCount = this.m_offlineCount.GetInt32((IDataReader) this.Reader)
    };
  }
}
