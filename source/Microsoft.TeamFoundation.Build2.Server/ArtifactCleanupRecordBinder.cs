// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ArtifactCleanupRecordBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class ArtifactCleanupRecordBinder : BuildObjectBinder<ArtifactCleanupRecord>
  {
    private BuildDataBinderHelper16 m_buildDataBinderHelper = new BuildDataBinderHelper16();
    private BuildArtifactBinderHelper6 m_artifactBinderHelper = new BuildArtifactBinderHelper6();

    public ArtifactCleanupRecordBinder(
      IVssRequestContext requestContext,
      BuildSqlComponentBase resourceComponent)
      : base(requestContext, resourceComponent)
    {
    }

    protected override ArtifactCleanupRecord Bind() => new ArtifactCleanupRecord()
    {
      Build = this.m_buildDataBinderHelper.Bind(this.Reader, this.ResourceComponent),
      Artifact = this.m_artifactBinderHelper.Bind(this.Reader)
    };
  }
}
