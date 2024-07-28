// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDataBinder17
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BuildDataBinder17 : BuildObjectBinder<BuildData>
  {
    private BuildDataBinderHelper17 m_buildDataBinderHelper = new BuildDataBinderHelper17();

    public BuildDataBinder17(
      IVssRequestContext requestContext,
      BuildSqlComponentBase resourceComponent)
      : base(requestContext, resourceComponent)
    {
    }

    protected override BuildData Bind() => this.m_buildDataBinderHelper.Bind(this.Reader, this.ResourceComponent);
  }
}
