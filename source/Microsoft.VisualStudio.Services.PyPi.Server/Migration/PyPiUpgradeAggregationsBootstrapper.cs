// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Migration.PyPiUpgradeAggregationsBootstrapper
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.MigrationEngine;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.PyPi.Server.JobManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Migration
{
  public class PyPiUpgradeAggregationsBootstrapper : IBootstrapper<IAsyncHandler>
  {
    private readonly IVssRequestContext collectionContext;
    private readonly IEnumerable<Guid> feedsToMigrate;
    private readonly string baselineMigration;
    private readonly string destinationMigration;
    private readonly IFeedService feedService;
    private readonly IServicingContextLogger servicingContextLogger;
    private readonly bool handleOnlyMigrationCatchup;
    private readonly IFactory<int> migrationJobNumberOfBatchesFactory;

    public PyPiUpgradeAggregationsBootstrapper(
      IVssRequestContext collectionContext,
      IFeedService feedService,
      IServicingContextLogger servicingContextLogger,
      string baselineMigration,
      string destinationMigration = null,
      IEnumerable<Guid> feedsToMigrate = null,
      bool handleOnlyMigrationCatchup = false,
      IFactory<int> migrationJobNumberOfBatchesFactory = null)
    {
      collectionContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection, nameof (PyPiUpgradeAggregationsBootstrapper));
      this.collectionContext = collectionContext;
      this.baselineMigration = baselineMigration;
      this.destinationMigration = destinationMigration;
      this.feedsToMigrate = feedsToMigrate ?? Enumerable.Empty<Guid>();
      this.feedService = feedService;
      this.servicingContextLogger = servicingContextLogger;
      this.handleOnlyMigrationCatchup = handleOnlyMigrationCatchup;
      this.migrationJobNumberOfBatchesFactory = migrationJobNumberOfBatchesFactory;
    }

    public IAsyncHandler Bootstrap() => new UpgradeAggregationsBootstrapper((IProtocol) Protocol.PyPi, this.collectionContext, this.feedService, this.servicingContextLogger, new PyPiMigrationDefinitionsProviderBootstrapper(this.collectionContext).Bootstrap(), new PyPiMigrationJobHandlerFactoryBootstrapper(this.collectionContext, this.migrationJobNumberOfBatchesFactory).Bootstrap().Get((JobId) null), new PyPiFeedChangeProcessingJobHandlerFactoryBootstrapper(this.collectionContext, this.handleOnlyMigrationCatchup).Bootstrap().Get((JobId) null), this.baselineMigration, this.destinationMigration, this.feedsToMigrate).Bootstrap();
  }
}
