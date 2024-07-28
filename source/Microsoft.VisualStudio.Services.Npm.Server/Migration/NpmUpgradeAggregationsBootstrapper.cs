// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Migration.NpmUpgradeAggregationsBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.MigrationEngine;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Npm.Server.Migration
{
  public class NpmUpgradeAggregationsBootstrapper
  {
    private readonly IVssRequestContext collectionContext;
    private readonly IEnumerable<Guid> feedsToMigrate;
    private readonly string destinationMigration;
    private readonly IFeedService feedService;
    private readonly IServicingContextLogger servicingContextLogger;
    private readonly bool handleOnlyMigrationCatchup;
    private readonly IFactory<int> migrationJobNumberOfBatchesFactory;

    public NpmUpgradeAggregationsBootstrapper(
      IVssRequestContext collectionContext,
      IFeedService feedService,
      IServicingContextLogger servicingContextLogger,
      string destinationMigration = null,
      IEnumerable<Guid> feedsToMigrate = null,
      bool handleOnlyMigrationCatchup = false,
      IFactory<int> migrationJobNumberOfBatchesFactory = null)
    {
      collectionContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection, nameof (NpmUpgradeAggregationsBootstrapper));
      this.collectionContext = collectionContext;
      this.feedsToMigrate = feedsToMigrate;
      this.destinationMigration = destinationMigration;
      this.feedService = feedService;
      this.servicingContextLogger = servicingContextLogger;
      this.handleOnlyMigrationCatchup = handleOnlyMigrationCatchup;
      this.migrationJobNumberOfBatchesFactory = migrationJobNumberOfBatchesFactory;
    }

    public IAsyncHandler Bootstrap() => new UpgradeAggregationsBootstrapper((IProtocol) Protocol.npm, this.collectionContext, this.feedService, this.servicingContextLogger, new NpmMigrationDefinitionsProviderBootstrapper(this.collectionContext).Bootstrap(), new NpmMigrationJobHandlerFactoryBootstrapper(this.collectionContext, this.migrationJobNumberOfBatchesFactory).Bootstrap().Get((JobId) null), new NpmFeedChangeProcessingJobHandlerFactoryBootstrapper(this.collectionContext, this.handleOnlyMigrationCatchup).Bootstrap().Get((JobId) null), "NpmItemStoreMetadata", this.destinationMigration, this.feedsToMigrate).Bootstrap();
  }
}
