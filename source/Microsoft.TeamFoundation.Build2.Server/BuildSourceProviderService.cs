// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildSourceProviderService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public sealed class BuildSourceProviderService : IBuildSourceProviderService, IVssFrameworkService
  {
    private readonly IBuildSourceProviderFactory m_sourceProviderFactory;

    public BuildSourceProviderService()
      : this((IBuildSourceProviderFactory) new BuildSourceProviderFactory())
    {
    }

    internal BuildSourceProviderService(IBuildSourceProviderFactory sourceProviderFactory) => this.m_sourceProviderFactory = sourceProviderFactory;

    public IBuildSourceProvider GetSourceProvider(
      IVssRequestContext requestContext,
      string repositoryType,
      bool throwIfNotFound = true)
    {
      using (requestContext.TraceScope("Service", nameof (GetSourceProvider)))
      {
        requestContext.AddCIEntry("SourceProvider", (object) repositoryType);
        return this.m_sourceProviderFactory.GetSourceProvider(requestContext, repositoryType, throwIfNotFound);
      }
    }

    public IEnumerable<IBuildSourceProvider> GetSourceProviders(IVssRequestContext requestContext)
    {
      using (requestContext.TraceScope("Service", nameof (GetSourceProviders)))
        return this.m_sourceProviderFactory.GetSourceProviders(requestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => this.m_sourceProviderFactory.Dispose();

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
