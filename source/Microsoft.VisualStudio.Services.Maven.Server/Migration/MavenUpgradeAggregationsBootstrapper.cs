// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Migration.MavenUpgradeAggregationsBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.MigrationEngine;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Maven.Server.Migration
{
  public class MavenUpgradeAggregationsBootstrapper : IBootstrapper<IAsyncHandler>
  {
    private readonly IVssRequestContext collectionContext;
    private readonly IEnumerable<Guid> feedsToMigrate;
    private readonly string destinationMigration;
    private readonly IFeedService feedService;
    private readonly IServicingContextLogger servicingContextLogger;
    private readonly bool handleOnlyMigrationCatchup;
    private readonly IFactory<int> migrationJobNumberOfBatchesFactory;

    public MavenUpgradeAggregationsBootstrapper(
      IVssRequestContext collectionContext,
      IFeedService feedService,
      IServicingContextLogger servicingContextLogger,
      string destinationMigration = null,
      IEnumerable<Guid> feedsToMigrate = null,
      bool handleOnlyMigrationCatchup = false,
      IFactory<int> migrationJobNumberOfBatchesFactory = null)
    {
      collectionContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection, nameof (MavenUpgradeAggregationsBootstrapper));
      this.collectionContext = collectionContext;
      this.destinationMigration = destinationMigration;
      this.feedsToMigrate = feedsToMigrate ?? Enumerable.Empty<Guid>();
      this.feedService = feedService;
      this.servicingContextLogger = servicingContextLogger;
      this.handleOnlyMigrationCatchup = handleOnlyMigrationCatchup;
      this.migrationJobNumberOfBatchesFactory = migrationJobNumberOfBatchesFactory;
    }

    public IAsyncHandler Bootstrap() => new UpgradeAggregationsBootstrapper((IProtocol) Protocol.Maven, this.collectionContext, this.feedService, this.servicingContextLogger, new MavenMigrationDefinitionsProviderBootstrapper(this.collectionContext).Bootstrap(), new MavenMigrationJobHandlerFactoryBootstrapper(this.collectionContext, this.migrationJobNumberOfBatchesFactory).Bootstrap().Get((JobId) null), new MavenFeedChangeProcessingJobHandlerFactoryBootstrapper(this.collectionContext, this.handleOnlyMigrationCatchup).Bootstrap().Get((JobId) null), "MavenItemStoreMetadata", this.destinationMigration, this.feedsToMigrate).Bootstrap();
  }
}
