// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.OrphanedBuildContainerBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class OrphanedBuildContainerBinder : ObjectBinder<OrphanedBuildContainer>
  {
    private SqlColumnBinder m_projectId = new SqlColumnBinder("ProjectId");
    private SqlColumnBinder m_containerId = new SqlColumnBinder("ContainerId");
    private SqlColumnBinder m_dateCreated = new SqlColumnBinder("DateCreated");

    protected override OrphanedBuildContainer Bind() => new OrphanedBuildContainer(this.m_projectId.GetGuid((IDataReader) this.Reader), this.m_containerId.GetInt64((IDataReader) this.Reader), this.m_dateCreated.GetDateTime((IDataReader) this.Reader));
  }
}
