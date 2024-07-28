// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MigrationStateWriterFromCollectionFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  internal class MigrationStateWriterFromCollectionFactoryBootstrapper : 
    IBootstrapper<IFactory<CollectionId, IMigrationStateWriter>>
  {
    private readonly IVssRequestContext requestContext;

    public MigrationStateWriterFromCollectionFactoryBootstrapper(IVssRequestContext requestContext)
    {
      this.requestContext = requestContext;
      requestContext.CheckProjectCollectionRequestContext();
    }

    public IFactory<CollectionId, IMigrationStateWriter> Bootstrap() => (IFactory<CollectionId, IMigrationStateWriter>) new MigrationStateWriterFromCollectionFactory(BlobServiceFactoryBootstrapper.CreateLegacyUnsharded(this.requestContext).Bootstrap(), (ISerializer<IEnumerable<MigrationEntry>>) new JsonSerializer<IEnumerable<MigrationEntry>>(new JsonSerializerSettings()
    {
      DefaultValueHandling = DefaultValueHandling.Ignore
    }), (ICache<string, MigrationState>) new MigrationStateCacheServiceFacade(this.requestContext, this.requestContext.GetTracerFacade()), (this.requestContext.ExecutionEnvironment.IsHostedDeployment ? 1 : 0) != 0, (ICache<IProtocol, EtagValue<IEnumerable<MigrationEntry>>>) new MigrationStateCacheOrgProtocolServiceFacade(this.requestContext), (this.requestContext.IsFeatureEnabled("Packaging.UseMigrationStateDocumentPerOrgCache") ? 1 : 0) != 0);
  }
}
