// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildObjectBinder`1
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal abstract class BuildObjectBinder<T> : ObjectBinder<T>
  {
    private IVssRequestContext m_requestContext;
    private BuildSqlComponentBase m_resourceComponent;

    protected BuildObjectBinder()
    {
    }

    protected BuildObjectBinder(IVssRequestContext requestContext) => this.m_requestContext = requestContext;

    protected BuildObjectBinder(IVssRequestContext requestContext, BuildSqlComponentBase component)
      : this(requestContext)
    {
      this.m_resourceComponent = component;
    }

    protected BuildSqlComponentBase ResourceComponent => this.m_resourceComponent;

    protected IdentityRef ToIdentityRef(Guid identityId) => new IdentityRef()
    {
      Id = identityId.ToString("D")
    };

    protected TeamProjectReference ToTeamProjectReference(Guid projectId) => new TeamProjectReference()
    {
      Id = projectId
    };

    protected BuildLogReference GetLogLocationReference(Microsoft.TeamFoundation.Build.WebApi.Build build) => new BuildLogReference((ISecuredObject) build)
    {
      Type = "Container"
    };

    protected void TraceException(string layer, Exception exception) => this.m_requestContext.TraceException(0, "Build2", layer, exception);
  }
}
