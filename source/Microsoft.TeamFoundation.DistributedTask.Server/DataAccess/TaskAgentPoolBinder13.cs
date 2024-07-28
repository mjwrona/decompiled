// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskAgentPoolBinder13
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
  internal sealed class TaskAgentPoolBinder13 : ObjectBinder<TaskAgentPoolData>
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
    private SqlColumnBinder m_autoSize = new SqlColumnBinder("AutoSize");
    private SqlColumnBinder m_poolMetadataFileId = new SqlColumnBinder("PoolMetadataFileId");
    private SqlColumnBinder m_ownerId = new SqlColumnBinder("OwnerId");
    private SqlColumnBinder m_agentCloudId = new SqlColumnBinder("agentCloudId");
    private SqlColumnBinder m_targetSize = new SqlColumnBinder("TargetSize");
    private SqlColumnBinder m_isLegacy = new SqlColumnBinder("IsLegacy");
    private SqlColumnBinder m_autoUpdate = new SqlColumnBinder("AutoUpdate");

    public TaskAgentPoolBinder13(IVssRequestContext requestContext) => this.m_scopeId = requestContext.ServiceHost.InstanceId;

    protected override TaskAgentPoolData Bind()
    {
      TaskAgentPool taskAgentPool1 = new TaskAgentPool();
      taskAgentPool1.Scope = this.m_scopeId;
      taskAgentPool1.Id = this.m_id.GetInt32((IDataReader) this.Reader);
      taskAgentPool1.Name = this.m_name.GetString((IDataReader) this.Reader, false);
      if (this.m_poolType.ColumnExists((IDataReader) this.Reader))
        taskAgentPool1.PoolType = (TaskAgentPoolType) this.m_poolType.GetByte((IDataReader) this.Reader);
      taskAgentPool1.Size = this.m_size.GetInt32((IDataReader) this.Reader);
      Guid? nullableGuid1 = this.m_createdBy.GetNullableGuid((IDataReader) this.Reader);
      Guid guid;
      if (nullableGuid1.HasValue)
      {
        TaskAgentPool taskAgentPool2 = taskAgentPool1;
        IdentityRef identityRef = new IdentityRef();
        guid = nullableGuid1.Value;
        identityRef.Id = guid.ToString("D");
        taskAgentPool2.CreatedBy = identityRef;
      }
      taskAgentPool1.CreatedOn = this.m_createdOn.GetDateTime((IDataReader) this.Reader);
      taskAgentPool1.IsHosted = this.m_isHosted.GetBoolean((IDataReader) this.Reader, false);
      taskAgentPool1.AutoProvision = new bool?(this.m_autoProvision.GetBoolean((IDataReader) this.Reader, false));
      taskAgentPool1.AutoSize = new bool?(this.m_autoSize.GetBoolean((IDataReader) this.Reader, false));
      taskAgentPool1.AgentCloudId = this.m_agentCloudId.GetNullableInt32((IDataReader) this.Reader);
      taskAgentPool1.TargetSize = this.m_targetSize.GetNullableInt32((IDataReader) this.Reader);
      taskAgentPool1.IsLegacy = new bool?(this.m_isLegacy.GetBoolean((IDataReader) this.Reader));
      taskAgentPool1.AutoUpdate = new bool?(this.m_autoUpdate.GetBoolean((IDataReader) this.Reader));
      Guid? nullableGuid2 = this.m_ownerId.GetNullableGuid((IDataReader) this.Reader);
      if (nullableGuid2.HasValue)
      {
        TaskAgentPool taskAgentPool3 = taskAgentPool1;
        IdentityRef identityRef = new IdentityRef();
        guid = nullableGuid2.Value;
        identityRef.Id = guid.ToString("D");
        taskAgentPool3.Owner = identityRef;
      }
      return new TaskAgentPoolData()
      {
        Pool = taskAgentPool1,
        ServiceAccountId = this.m_serviceAccountId.GetGuid((IDataReader) this.Reader),
        ServiceIdentityId = this.m_serviceIdentityId.GetNullableGuid((IDataReader) this.Reader),
        PoolMetadataFileId = this.m_poolMetadataFileId.GetNullableInt32((IDataReader) this.Reader)
      };
    }
  }
}
