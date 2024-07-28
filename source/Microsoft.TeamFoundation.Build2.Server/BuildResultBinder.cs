// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildResultBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BuildResultBinder : BuildObjectBinder<BuildResult?>
  {
    private SqlColumnBinder m_result = new SqlColumnBinder("Result");

    public BuildResultBinder(
      IVssRequestContext requestContext,
      BuildSqlComponentBase resourceComponent)
      : base(requestContext, resourceComponent)
    {
    }

    protected override BuildResult? Bind() => !this.m_result.IsNull((IDataReader) this.Reader) ? new BuildResult?((BuildResult) this.m_result.GetByte((IDataReader) this.Reader)) : new BuildResult?();
  }
}
