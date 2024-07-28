// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildRetentionSampleBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BuildRetentionSampleBinder : BuildObjectBinder<BuildRetentionSample>
  {
    private SqlColumnBinder m_sampleTime = new SqlColumnBinder("SampleTime");
    private SqlColumnBinder m_definitions = new SqlColumnBinder("Definitions");
    private SqlColumnBinder m_builds = new SqlColumnBinder("Builds");
    private SqlColumnBinder m_files = new SqlColumnBinder("Files");

    public BuildRetentionSampleBinder(IVssRequestContext requestContext)
      : base(requestContext)
    {
    }

    protected override BuildRetentionSample Bind() => new BuildRetentionSample()
    {
      SampleTime = this.m_sampleTime.GetDateTime((IDataReader) this.Reader),
      Definitions = this.m_definitions.GetString((IDataReader) this.Reader, true),
      Builds = this.m_builds.GetString((IDataReader) this.Reader, true),
      Files = this.m_files.GetString((IDataReader) this.Reader, true)
    };
  }
}
