// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.EnvironmentBinder
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
  internal class EnvironmentBinder : ObjectBinder<EnvironmentInstance>
  {
    private SqlColumnBinder m_environmentId = new SqlColumnBinder("Id");
    private SqlColumnBinder m_environmentName = new SqlColumnBinder("Name");
    private SqlColumnBinder m_description = new SqlColumnBinder("Description");
    private SqlColumnBinder m_createdBy = new SqlColumnBinder("CreatedBy");
    private SqlColumnBinder m_createdOn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder m_modifiedBy = new SqlColumnBinder("LastModifiedBy");
    private SqlColumnBinder m_modifiedOn = new SqlColumnBinder("LastModifiedOn");

    protected override EnvironmentInstance Bind()
    {
      EnvironmentInstance environmentInstance = new EnvironmentInstance();
      environmentInstance.Id = this.m_environmentId.GetInt32((IDataReader) this.Reader);
      environmentInstance.Name = this.m_environmentName.GetString((IDataReader) this.Reader, false);
      environmentInstance.Description = this.m_description.GetString((IDataReader) this.Reader, true);
      Guid? nullableGuid1 = this.m_createdBy.GetNullableGuid((IDataReader) this.Reader);
      environmentInstance.CreatedBy = new IdentityRef()
      {
        Id = nullableGuid1.Value.ToString("D")
      };
      environmentInstance.CreatedOn = this.m_createdOn.GetDateTime((IDataReader) this.Reader);
      Guid? nullableGuid2 = this.m_modifiedBy.GetNullableGuid((IDataReader) this.Reader);
      environmentInstance.LastModifiedBy = new IdentityRef()
      {
        Id = nullableGuid2.Value.ToString("D")
      };
      environmentInstance.LastModifiedOn = this.m_modifiedOn.GetDateTime((IDataReader) this.Reader);
      return environmentInstance;
    }
  }
}
