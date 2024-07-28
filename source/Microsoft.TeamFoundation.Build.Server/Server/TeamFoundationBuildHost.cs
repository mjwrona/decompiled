// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.TeamFoundationBuildHost
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal sealed class TeamFoundationBuildHost : IVssFrameworkService
  {
    private SecurityManager m_securityManager;
    private ArtifactKind m_buildAgentArtifactKind;
    private ArtifactKind m_buildControllerArtifactKind;
    private ArtifactKind m_buildDefinitionArtifactKind;

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(0, "Build", "Service", "ServiceStart");
      try
      {
        this.m_securityManager = new SecurityManager(systemRequestContext);
      }
      finally
      {
        systemRequestContext.TraceLeave(0, "Build", "Service", "ServiceStart");
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(0, "Build", "Service", "ServiceEnd");
      try
      {
        this.m_securityManager = (SecurityManager) null;
      }
      finally
      {
        systemRequestContext.TraceLeave(0, "Build", "Service", "ServiceEnd");
      }
    }

    internal ArtifactKind GetBuildAgentArtifactKind(IVssRequestContext requestContext)
    {
      if (this.m_buildAgentArtifactKind == null)
        this.m_buildAgentArtifactKind = requestContext.Elevate().GetService<TeamFoundationPropertyService>().GetArtifactKind(requestContext.Elevate(), BuildPropertyKinds.BuildAgent);
      return this.m_buildAgentArtifactKind;
    }

    internal ArtifactKind GetBuildControllerArtifactKind(IVssRequestContext requestContext)
    {
      if (this.m_buildControllerArtifactKind == null)
        this.m_buildControllerArtifactKind = requestContext.Elevate().GetService<TeamFoundationPropertyService>().GetArtifactKind(requestContext.Elevate(), BuildPropertyKinds.BuildController);
      return this.m_buildControllerArtifactKind;
    }

    internal ArtifactKind GetBuildDefinitionArtifactKind(IVssRequestContext requestContext)
    {
      if (this.m_buildDefinitionArtifactKind == null)
        this.m_buildDefinitionArtifactKind = requestContext.Elevate().GetService<TeamFoundationPropertyService>().GetArtifactKind(requestContext.Elevate(), BuildPropertyKinds.BuildDefinition);
      return this.m_buildDefinitionArtifactKind;
    }

    internal SecurityManager SecurityManager => this.m_securityManager;
  }
}
