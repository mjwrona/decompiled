// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.EnvironmentResourceDataBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class EnvironmentResourceDataBinder : ObjectBinder<EnvironmentResourceData>
  {
    private SqlColumnBinder m_resourceId = new SqlColumnBinder("Id");
    private SqlColumnBinder m_resourceName = new SqlColumnBinder("Name");
    private SqlColumnBinder m_resourceType = new SqlColumnBinder("Type");
    private SqlColumnBinder m_environmentId = new SqlColumnBinder("EnvironmentId");

    protected override EnvironmentResourceData Bind() => new EnvironmentResourceData()
    {
      Id = this.m_resourceId.GetInt32((IDataReader) this.Reader),
      Name = this.m_resourceName.GetString((IDataReader) this.Reader, false),
      Type = (EnvironmentResourceType) this.m_resourceType.GetByte((IDataReader) this.Reader),
      EnvironmentId = this.m_environmentId.GetInt32((IDataReader) this.Reader)
    };
  }
}
