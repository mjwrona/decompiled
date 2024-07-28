// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.NugetUpgradeAggregationsBootstrapper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NuGet.Server.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.MigrationEngine;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class NugetUpgradeAggregationsBootstrapper : IBootstrapper<IAsyncHandler>
  {
    private readonly IVssRequestContext collectionContext;
    private readonly IEnumerable<Guid> feedsToMigrate;
    private readonly string destinationMigration;
    private readonly IFeedService feedService;
    private readonly IServicingContextLogger servicingContextLogger;
    private readonly bool handleOnlyMigrationCatchup;
    private readonly IFactory<int> migrationJobNumberOfBatchesFactory;

    public NugetUpgradeAggregationsBootstrapper(
      IVssRequestContext collectionContext,
      IFeedService feedService,
      IServicingContextLogger servicingContextLogger,
      string destinationMigration = null,
      IEnumerable<Guid> feedsToMigrate = null,
      bool handleOnlyMigrationCatchup = false,
      IFactory<int> migrationJobNumberOfBatchesFactory = null)
    {
      collectionContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection, nameof (NugetUpgradeAggregationsBootstrapper));
      this.collectionContext = collectionContext;
      this.destinationMigration = destinationMigration;
      this.feedsToMigrate = feedsToMigrate ?? Enumerable.Empty<Guid>();
      this.feedService = feedService;
      this.servicingContextLogger = servicingContextLogger;
      this.handleOnlyMigrationCatchup = handleOnlyMigrationCatchup;
      this.migrationJobNumberOfBatchesFactory = migrationJobNumberOfBatchesFactory;
    }

    public IAsyncHandler Bootstrap() => new UpgradeAggregationsBootstrapper((IProtocol) Protocol.NuGet, this.collectionContext, this.feedService, this.servicingContextLogger, new NuGetMigrationDefinitionsProviderBootstrapper(this.collectionContext).Bootstrap(), new NuGetMigrationJobHandlerFactoryBootstrapper(this.collectionContext, this.migrationJobNumberOfBatchesFactory).Bootstrap().Get((JobId) null), new NuGetFeedChangeProcessingJobHandlerFactoryBootstrapper(this.collectionContext, this.handleOnlyMigrationCatchup).Bootstrap().Get((JobId) null), "NuGetItemStoreMetadata", this.destinationMigration, this.feedsToMigrate).Bootstrap();
  }
}
