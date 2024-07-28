// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildArtifactProviderService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public sealed class BuildArtifactProviderService : 
    IBuildArtifactProviderService,
    IVssFrameworkService
  {
    private readonly IArtifactProviderFactory m_artifactProviderFactory;

    public BuildArtifactProviderService()
      : this((IArtifactProviderFactory) new DefaultArtifactProviderFactory())
    {
    }

    internal BuildArtifactProviderService(IArtifactProviderFactory artifactProviderFactory) => this.m_artifactProviderFactory = artifactProviderFactory;

    public bool TryGetArtifactProvider(
      IVssRequestContext requestContext,
      string artifactResourceType,
      out IArtifactProvider artifactProvider)
    {
      using (requestContext.TraceScope("Service", nameof (TryGetArtifactProvider)))
        return this.m_artifactProviderFactory.TryGetArtifactProvider(requestContext, artifactResourceType, out artifactProvider);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_artifactProviderFactory == null || !(this.m_artifactProviderFactory is IDisposable artifactProviderFactory))
        return;
      artifactProviderFactory.Dispose();
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
