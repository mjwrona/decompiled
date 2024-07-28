// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildTagFilterBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BuildTagFilterBinder : ObjectBinder<BuildTagFilter>
  {
    private SqlColumnBinder m_definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder m_definitionPath = new SqlColumnBinder("DefinitionPath");
    private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder m_tag = new SqlColumnBinder("Tag");
    private BuildSqlComponentBase m_resourceComponent;

    public BuildTagFilterBinder(BuildSqlComponentBase component) => this.m_resourceComponent = component;

    protected override BuildTagFilter Bind() => new BuildTagFilter()
    {
      Tag = this.m_tag.GetString((IDataReader) this.Reader, false),
      Definition = new MinimalBuildDefinition()
      {
        Id = this.m_definitionId.GetInt32((IDataReader) this.Reader),
        Path = this.m_definitionPath.GetString((IDataReader) this.Reader, false),
        ProjectId = this.m_resourceComponent.GetDataspaceIdentifier(this.m_dataspaceId.GetInt32((IDataReader) this.Reader))
      }
    };
  }
}
