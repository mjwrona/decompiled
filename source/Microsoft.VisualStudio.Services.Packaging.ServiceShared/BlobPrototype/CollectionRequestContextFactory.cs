// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CollectionRequestContextFactory
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class CollectionRequestContextFactory : IFactory<CollectionId, IVssRequestContext>
  {
    private readonly IVssRequestContext deploymentRequestContext;
    private readonly ITeamFoundationHostManagementService hostService;

    public CollectionRequestContextFactory(
      IVssRequestContext deploymentContext,
      ITeamFoundationHostManagementService hostService)
    {
      this.deploymentRequestContext = deploymentContext;
      this.hostService = hostService;
    }

    public IVssRequestContext Get(CollectionId collectionId)
    {
      this.deploymentRequestContext.CheckDeploymentRequestContext();
      this.deploymentRequestContext.CheckSystemRequestContext();
      return this.hostService.BeginRequest(this.deploymentRequestContext, collectionId.Guid, RequestContextType.SystemContext);
    }
  }
}
