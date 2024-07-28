// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskAgentPoolBinder7
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
  internal sealed class TaskAgentPoolBinder7 : ObjectBinder<TaskAgentPoolData>
  {
    private Guid m_scopeId;
    private SqlColumnBinder m_id = new SqlColumnBinder("PoolId");
    private SqlColumnBinder m_name = new SqlColumnBinder("PoolName");
    private SqlColumnBinder m_poolType = new SqlColumnBinder("PoolType");
    private SqlColumnBinder m_size = new SqlColumnBinder("Size");
    private SqlColumnBinder m_createdBy = new SqlColumnBinder("CreatedBy");
    private SqlColumnBinder m_createdOn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder m_serviceAccountId = new SqlColumnBinder("ServiceAccountId");
    private SqlColumnBinder m_serviceIdentityId = new SqlColumnBinder("ServiceIdentityId");
    private SqlColumnBinder m_isHosted = new SqlColumnBinder("IsHosted");
    private SqlColumnBinder m_autoProvision = new SqlColumnBinder("AutoProvision");

    public TaskAgentPoolBinder7(IVssRequestContext requestContext) => this.m_scopeId = requestContext.ServiceHost.InstanceId;

    protected override TaskAgentPoolData Bind()
    {
      TaskAgentPool taskAgentPool = new TaskAgentPool();
      taskAgentPool.Scope = this.m_scopeId;
      taskAgentPool.Id = this.m_id.GetInt32((IDataReader) this.Reader);
      taskAgentPool.Name = this.m_name.GetString((IDataReader) this.Reader, false);
      if (this.m_poolType.ColumnExists((IDataReader) this.Reader))
        taskAgentPool.PoolType = (TaskAgentPoolType) this.m_poolType.GetByte((IDataReader) this.Reader);
      taskAgentPool.Size = this.m_size.GetInt32((IDataReader) this.Reader);
      Guid? nullableGuid = this.m_createdBy.GetNullableGuid((IDataReader) this.Reader);
      if (nullableGuid.HasValue)
        taskAgentPool.CreatedBy = new IdentityRef()
        {
          Id = nullableGuid.Value.ToString("D")
        };
      taskAgentPool.CreatedOn = this.m_createdOn.GetDateTime((IDataReader) this.Reader);
      taskAgentPool.IsHosted = this.m_isHosted.GetBoolean((IDataReader) this.Reader, false);
      taskAgentPool.AutoProvision = new bool?(this.m_autoProvision.GetBoolean((IDataReader) this.Reader, false));
      return new TaskAgentPoolData()
      {
        Pool = taskAgentPool,
        ServiceAccountId = this.m_serviceAccountId.GetGuid((IDataReader) this.Reader),
        ServiceIdentityId = this.m_serviceIdentityId.GetNullableGuid((IDataReader) this.Reader)
      };
    }
  }
}
