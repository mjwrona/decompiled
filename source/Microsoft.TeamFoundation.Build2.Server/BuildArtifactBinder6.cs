// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildArtifactBinder6
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BuildArtifactBinder6 : BuildObjectBinder<BuildArtifact>
  {
    private BuildArtifactBinderHelper6 m_buildArtifactBinderHelper = new BuildArtifactBinderHelper6();

    public BuildArtifactBinder6(IVssRequestContext requestContext)
      : base(requestContext)
    {
    }

    protected override BuildArtifact Bind() => this.m_buildArtifactBinderHelper.Bind(this.Reader);
  }
}
