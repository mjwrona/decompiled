// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.EnvironmentBinder2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class EnvironmentBinder2 : EnvironmentBinder
  {
    private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");

    public EnvironmentBinder2(EnvironmentComponent resourceComponent) => this.m_resourceComponent = resourceComponent;

    protected override EnvironmentInstance Bind()
    {
      EnvironmentInstance environmentInstance = base.Bind();
      environmentInstance.Project = new ProjectReference()
      {
        Id = this.m_resourceComponent.GetDataspaceIdentifier(this.m_dataspaceId.GetInt32((IDataReader) this.Reader))
      };
      return environmentInstance;
    }

    private EnvironmentComponent m_resourceComponent { get; }
  }
}
