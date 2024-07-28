// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MigrationStateWriterFromCollectionFactory
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  internal class MigrationStateWriterFromCollectionFactory : 
    IFactory<CollectionId, IMigrationStateWriter>
  {
    private readonly IFactory<ContainerAddress, IBlobService> blobServiceFactory;
    private readonly ISerializer<IEnumerable<MigrationEntry>> serializer;
    private readonly ICache<string, MigrationState> migrationStateCacheService;
    private readonly bool isHosted;
    private readonly ICache<IProtocol, EtagValue<IEnumerable<MigrationEntry>>> migrationStateDocumentCacheService;
    private readonly bool useMigrationStateDocumentCache;

    public MigrationStateWriterFromCollectionFactory(
      IFactory<ContainerAddress, IBlobService> blobServiceFactory,
      ISerializer<IEnumerable<MigrationEntry>> serializer,
      ICache<string, MigrationState> migrationStateCacheService,
      bool isHosted,
      ICache<IProtocol, EtagValue<IEnumerable<MigrationEntry>>> migrationStateDocumentCacheService,
      bool useMigrationStateDocumentCache)
    {
      this.blobServiceFactory = blobServiceFactory;
      this.serializer = serializer;
      this.migrationStateCacheService = migrationStateCacheService;
      this.isHosted = isHosted;
      this.migrationStateDocumentCacheService = migrationStateDocumentCacheService;
      this.useMigrationStateDocumentCache = useMigrationStateDocumentCache;
    }

    public IMigrationStateWriter Get(CollectionId collectionId) => (IMigrationStateWriter) new MigrationStateWriter(this.blobServiceFactory.Get(new ContainerAddress(collectionId, new Locator(new string[1]
    {
      "migrationstate"
    }))), this.serializer, collectionId, (IInvalidatableCache<string>) this.migrationStateCacheService, this.isHosted, this.migrationStateDocumentCacheService, this.useMigrationStateDocumentCache);
  }
}
