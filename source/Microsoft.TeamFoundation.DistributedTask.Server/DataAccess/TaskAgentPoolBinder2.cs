// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskAgentPoolBinder2
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
  internal sealed class TaskAgentPoolBinder2 : ObjectBinder<TaskAgentPoolData>
  {
    private Guid m_scopeId;
    private SqlColumnBinder m_id = new SqlColumnBinder("PoolId");
    private SqlColumnBinder m_name = new SqlColumnBinder("PoolName");
    private SqlColumnBinder m_size = new SqlColumnBinder("Size");
    private SqlColumnBinder m_createdBy = new SqlColumnBinder("CreatedBy");
    private SqlColumnBinder m_createdOn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder m_groupScopeId = new SqlColumnBinder("GroupScopeId");
    private SqlColumnBinder m_administratorsGroupId = new SqlColumnBinder("AdministratorsGroupId");
    private SqlColumnBinder m_serviceAccountsGroupId = new SqlColumnBinder("ServiceAccountsGroupId");
    private SqlColumnBinder m_serviceIdentityId = new SqlColumnBinder("ServiceIdentityId");

    public TaskAgentPoolBinder2(IVssRequestContext requestContext) => this.m_scopeId = requestContext.ServiceHost.InstanceId;

    protected override TaskAgentPoolData Bind()
    {
      TaskAgentPool taskAgentPool1 = new TaskAgentPool();
      taskAgentPool1.Scope = this.m_scopeId;
      taskAgentPool1.Id = this.m_id.GetInt32((IDataReader) this.Reader);
      taskAgentPool1.Name = this.m_name.GetString((IDataReader) this.Reader, false);
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
      Guid? nullableGuid2 = this.m_groupScopeId.GetNullableGuid((IDataReader) this.Reader);
      if (nullableGuid2.HasValue)
        taskAgentPool1.GroupScopeId = nullableGuid2.Value;
      Guid? nullableGuid3 = this.m_administratorsGroupId.GetNullableGuid((IDataReader) this.Reader);
      if (nullableGuid3.HasValue)
      {
        TaskAgentPool taskAgentPool3 = taskAgentPool1;
        IdentityRef identityRef = new IdentityRef();
        guid = nullableGuid3.Value;
        identityRef.Id = guid.ToString("D");
        taskAgentPool3.AdministratorsGroup = identityRef;
      }
      Guid? nullableGuid4 = this.m_serviceAccountsGroupId.GetNullableGuid((IDataReader) this.Reader);
      if (nullableGuid4.HasValue)
      {
        TaskAgentPool taskAgentPool4 = taskAgentPool1;
        IdentityRef identityRef = new IdentityRef();
        guid = nullableGuid4.Value;
        identityRef.Id = guid.ToString("D");
        taskAgentPool4.ServiceAccountsGroup = identityRef;
      }
      return new TaskAgentPoolData()
      {
        Pool = taskAgentPool1,
        ServiceIdentityId = this.m_serviceIdentityId.GetNullableGuid((IDataReader) this.Reader)
      };
    }
  }
}
