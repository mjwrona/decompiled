// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDefinitionForRetentionBinder3
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BuildDefinitionForRetentionBinder3 : 
    BuildObjectBinder<BuildDefinitionForRetention>
  {
    private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder m_definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder m_repository = new SqlColumnBinder("Repository");
    private SqlColumnBinder m_retentionRules = new SqlColumnBinder("RetentionPolicy");
    private SqlColumnBinder m_definitionName = new SqlColumnBinder("DefinitionName");
    private SqlColumnBinder m_deleted = new SqlColumnBinder("Deleted");

    public BuildDefinitionForRetentionBinder3(
      IVssRequestContext requestContext,
      BuildSqlComponentBase component)
      : base(requestContext, component)
    {
    }

    protected override BuildDefinitionForRetention Bind() => new BuildDefinitionForRetention()
    {
      ProjectId = this.ResourceComponent.GetDataspaceIdentifier(this.m_dataspaceId.GetInt32((IDataReader) this.Reader)),
      DefinitionId = this.m_definitionId.GetInt32((IDataReader) this.Reader),
      RepositoryString = this.m_repository.GetString((IDataReader) this.Reader, true),
      RetentionRulesString = this.m_retentionRules.GetString((IDataReader) this.Reader, true),
      DefinitionName = this.m_definitionName.GetString((IDataReader) this.Reader, true),
      Deleted = this.m_deleted.GetBoolean((IDataReader) this.Reader, false),
      ProcessType = 1
    };
  }
}
