// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.VirtualMachineGroupBinder
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
  internal sealed class VirtualMachineGroupBinder : ObjectBinder<VirtualMachineGroup>
  {
    private SqlColumnBinder m_resourceId = new SqlColumnBinder("Id");
    private SqlColumnBinder m_resourceName = new SqlColumnBinder("Name");
    private SqlColumnBinder m_resourceType = new SqlColumnBinder("Type");
    private SqlColumnBinder m_poolId = new SqlColumnBinder("PoolId");
    private SqlColumnBinder m_createdBy = new SqlColumnBinder("CreatedBy");
    private SqlColumnBinder m_createdOn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder m_modifiedBy = new SqlColumnBinder("LastModifiedBy");
    private SqlColumnBinder m_modifiedOn = new SqlColumnBinder("LastModifiedOn");
    private SqlColumnBinder m_environmentId = new SqlColumnBinder("EnvironmentId");
    private SqlColumnBinder m_environmentName = new SqlColumnBinder("EnvironmentName");

    protected override VirtualMachineGroup Bind()
    {
      VirtualMachineGroup virtualMachineGroup = new VirtualMachineGroup();
      virtualMachineGroup.Id = this.m_resourceId.GetInt32((IDataReader) this.Reader);
      virtualMachineGroup.Name = this.m_resourceName.GetString((IDataReader) this.Reader, false);
      virtualMachineGroup.Type = (EnvironmentResourceType) this.m_resourceType.GetByte((IDataReader) this.Reader);
      virtualMachineGroup.PoolId = this.m_poolId.GetInt32((IDataReader) this.Reader);
      Guid guid1 = this.m_createdBy.GetGuid((IDataReader) this.Reader);
      virtualMachineGroup.CreatedBy = new IdentityRef()
      {
        Id = guid1.ToString()
      };
      virtualMachineGroup.CreatedOn = this.m_createdOn.GetDateTime((IDataReader) this.Reader);
      Guid guid2 = this.m_modifiedBy.GetGuid((IDataReader) this.Reader);
      virtualMachineGroup.LastModifiedBy = new IdentityRef()
      {
        Id = guid2.ToString()
      };
      virtualMachineGroup.LastModifiedOn = this.m_modifiedOn.GetDateTime((IDataReader) this.Reader);
      virtualMachineGroup.EnvironmentReference.Id = this.m_environmentId.GetInt32((IDataReader) this.Reader);
      virtualMachineGroup.EnvironmentReference.Name = this.m_environmentName.GetString((IDataReader) this.Reader, true);
      return virtualMachineGroup;
    }
  }
}
