// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildForRetentionBinder3
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BuildForRetentionBinder3 : BuildObjectBinder<BuildForRetention>
  {
    private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder m_buildId = new SqlColumnBinder("BuildId");
    private SqlColumnBinder m_sourceBranch = new SqlColumnBinder("SourceBranch");
    private SqlColumnBinder m_finishTime = new SqlColumnBinder("FinishTime");
    private SqlColumnBinder m_result = new SqlColumnBinder("Result");

    public BuildForRetentionBinder3(
      IVssRequestContext requestContext,
      BuildSqlComponentBase component)
      : base(requestContext, component)
    {
    }

    protected override BuildForRetention Bind()
    {
      BuildForRetention buildForRetention = new BuildForRetention();
      buildForRetention.Project = this.ResourceComponent.GetDataspaceIdentifier(this.m_dataspaceId.GetInt32((IDataReader) this.Reader));
      buildForRetention.Id = this.m_buildId.GetInt32((IDataReader) this.Reader);
      buildForRetention.SourceBranch = this.m_sourceBranch.GetString((IDataReader) this.Reader, true);
      buildForRetention.FinishTime = this.m_finishTime.GetDateTime((IDataReader) this.Reader);
      if (!this.m_result.IsNull((IDataReader) this.Reader))
        buildForRetention.Result = new BuildResult?((BuildResult) this.m_result.GetByte((IDataReader) this.Reader));
      return buildForRetention;
    }
  }
}
