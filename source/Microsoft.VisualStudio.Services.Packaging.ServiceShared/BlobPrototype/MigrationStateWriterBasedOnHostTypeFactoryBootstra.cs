// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MigrationStateWriterBasedOnHostTypeFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class MigrationStateWriterBasedOnHostTypeFactoryBootstrapper : 
    IBootstrapper<IFactory<IMigrationStateWriter>>
  {
    private readonly IVssRequestContext requestContext;

    public MigrationStateWriterBasedOnHostTypeFactoryBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IFactory<IMigrationStateWriter> Bootstrap()
    {
      IExecutionEnvironment environmentFacade = this.requestContext.GetExecutionEnvironmentFacade();
      IHostInfoService hostInfoService = new HostInfoServiceFacadeBootstrapper(this.requestContext).Bootstrap();
      return (IFactory<IMigrationStateWriter>) new MigrationStateWriterBasedOnHostTypeFactory((IFactory<IVssRequestContext, IFactory<CollectionId, IMigrationStateWriter>>) new MigrationStateWriterFromRequestContextFactory(this.requestContext, (Func<IVssRequestContext, IFactory<CollectionId, IMigrationStateWriter>>) (collectionContext => new MigrationStateWriterFromCollectionFactoryBootstrapper(collectionContext).Bootstrap())), (IFactory<CollectionId, IVssRequestContext>) new CollectionRequestContextFactory(this.requestContext, (ITeamFoundationHostManagementService) this.requestContext.GetService<TeamFoundationHostManagementService>()), environmentFacade, hostInfoService);
    }
  }
}
