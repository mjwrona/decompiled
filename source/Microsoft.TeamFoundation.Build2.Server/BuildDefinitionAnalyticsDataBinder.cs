// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDefinitionAnalyticsDataBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BuildDefinitionAnalyticsDataBinder : 
    BuildObjectBinder<BuildDefinitionAnalyticsData>
  {
    private SqlColumnBinder m_definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder m_definitionVersion = new SqlColumnBinder("DefinitionVersion");
    private SqlColumnBinder m_definitionName = new SqlColumnBinder("DefinitionName");
    private SqlColumnBinder m_processType = new SqlColumnBinder("ProcessType");
    private SqlColumnBinder m_deleted = new SqlColumnBinder("Deleted");
    private SqlColumnBinder m_lastUpdated = new SqlColumnBinder("LastUpdated");
    private Guid m_projectId;

    public BuildDefinitionAnalyticsDataBinder(IVssRequestContext requestContext, Guid projectId)
      : base(requestContext)
    {
      this.m_projectId = projectId;
    }

    protected override BuildDefinitionAnalyticsData Bind() => new BuildDefinitionAnalyticsData()
    {
      ProjectGuid = this.m_projectId,
      DefinitionId = this.m_definitionId.GetInt32((IDataReader) this.Reader),
      DefinitionVersion = this.m_definitionVersion.GetInt32((IDataReader) this.Reader),
      DefinitionName = DBHelper.DBPathToServerPath(this.m_definitionName.GetString((IDataReader) this.Reader, false)),
      ProcessType = this.m_processType.GetNullableInt32((IDataReader) this.Reader),
      Deleted = this.m_deleted.GetBoolean((IDataReader) this.Reader),
      LastUpdated = this.m_lastUpdated.GetNullableDateTime((IDataReader) this.Reader)
    };
  }
}
