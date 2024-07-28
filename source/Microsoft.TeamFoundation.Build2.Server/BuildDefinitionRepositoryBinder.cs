// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDefinitionRepositoryBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BuildDefinitionRepositoryBinder : 
    BuildObjectBinder<BuildDefinitionRepositoryMap>
  {
    private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder m_definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");

    public BuildDefinitionRepositoryBinder(
      IVssRequestContext requestContext,
      BuildSqlComponentBase resourceComponent)
      : base(requestContext, resourceComponent)
    {
    }

    protected override BuildDefinitionRepositoryMap Bind() => new BuildDefinitionRepositoryMap(this.m_dataspaceId.GetInt32((IDataReader) this.Reader), this.m_definitionId.GetInt32((IDataReader) this.Reader), this.m_repositoryId.GetInt32((IDataReader) this.Reader));
  }
}
