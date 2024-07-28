// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ShallowBuildAnalyticsDataBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class ShallowBuildAnalyticsDataBinder : 
    BuildObjectBinder<ShallowBuildAnalyticsData>
  {
    private SqlColumnBinder m_buildId = new SqlColumnBinder("BuildId");
    private SqlColumnBinder m_changedOn = new SqlColumnBinder("ChangedOn");
    private Guid m_projectId;

    public ShallowBuildAnalyticsDataBinder(IVssRequestContext requestContext, Guid projectId)
      : base(requestContext)
    {
      this.m_projectId = projectId;
    }

    protected override ShallowBuildAnalyticsData Bind() => new ShallowBuildAnalyticsData()
    {
      ProjectGuid = this.m_projectId,
      BuildId = this.m_buildId.GetInt32((IDataReader) this.Reader),
      ChangedOn = this.m_changedOn.GetDateTime((IDataReader) this.Reader)
    };
  }
}
