// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.CargoUpgradeAggregationsBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.MigrationEngine;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Aggregations
{
  public class CargoUpgradeAggregationsBootstrapper : IBootstrapper<IAsyncHandler>
  {
    private readonly IVssRequestContext collectionContext;
    private readonly IEnumerable<Guid> feedsToMigrate;
    private readonly string? baselineMigration;
    private readonly string? destinationMigration;
    private readonly IFeedService feedService;
    private readonly IServicingContextLogger servicingContextLogger;
    private readonly bool handleOnlyMigrationCatchup;
    private readonly IFactory<int>? migrationJobNumberOfBatchesFactory;

    public CargoUpgradeAggregationsBootstrapper(
      IVssRequestContext collectionContext,
      IFeedService feedService,
      IServicingContextLogger servicingContextLogger,
      string baselineMigration,
      string? destinationMigration = null,
      IEnumerable<Guid>? feedsToMigrate = null,
      bool handleOnlyMigrationCatchup = false,
      IFactory<int>? migrationJobNumberOfBatchesFactory = null)
    {
      collectionContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection, nameof (CargoUpgradeAggregationsBootstrapper));
      this.collectionContext = collectionContext;
      this.baselineMigration = baselineMigration;
      this.destinationMigration = destinationMigration;
      this.feedsToMigrate = feedsToMigrate ?? Enumerable.Empty<Guid>();
      this.feedService = feedService;
      this.servicingContextLogger = servicingContextLogger;
      this.handleOnlyMigrationCatchup = handleOnlyMigrationCatchup;
      this.migrationJobNumberOfBatchesFactory = migrationJobNumberOfBatchesFactory;
    }

    public IAsyncHandler Bootstrap() => new UpgradeAggregationsBootstrapper((IProtocol) Protocol.Cargo, this.collectionContext, this.feedService, this.servicingContextLogger, new CargoMigrationDefinitionsProviderBootstrapper(this.collectionContext).Bootstrap(), new CargoMigrationJobHandlerFactoryBootstrapper(this.collectionContext, this.migrationJobNumberOfBatchesFactory).Bootstrap().Get((JobId) null), new CargoFeedChangeProcessingJobHandlerFactoryBootstrapper(this.collectionContext, this.handleOnlyMigrationCatchup).Bootstrap().Get((JobId) null), this.baselineMigration, this.destinationMigration, this.feedsToMigrate).Bootstrap();
  }
}
