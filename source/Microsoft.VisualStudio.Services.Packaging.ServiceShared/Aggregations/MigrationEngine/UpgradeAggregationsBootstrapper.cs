// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.MigrationEngine.UpgradeAggregationsBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.MigrationEngine
{
  public class UpgradeAggregationsBootstrapper : IBootstrapper<IAsyncHandler>
  {
    private readonly IProtocol protocol;
    private readonly IVssRequestContext collectionContext;
    private readonly IEnumerable<Guid> feedsToMigrate;
    private readonly string destinationMigration;
    private readonly IFeedService feedService;
    private readonly IServicingContextLogger servicingContextLogger;
    private readonly IMigrationDefinitionsProvider migrationsDefinitionsProvider;
    private readonly IAsyncHandler<IFeedRequest, JobResult> migrationJobHandler;
    private readonly IAsyncHandler<IFeedRequest, JobResult> changeProcessingJobHandler;
    private readonly string itemStoreMetadataMigration;

    public UpgradeAggregationsBootstrapper(
      IProtocol protocol,
      IVssRequestContext collectionContext,
      IFeedService feedService,
      IServicingContextLogger servicingContextLogger,
      IMigrationDefinitionsProvider migrationsDefinitionsProvider,
      IAsyncHandler<IFeedRequest, JobResult> migrationJobHandler,
      IAsyncHandler<IFeedRequest, JobResult> changeProcessingJobHandler,
      string itemStoreMetadataMigration,
      string destinationMigration = null,
      IEnumerable<Guid> feedsToMigrate = null)
    {
      collectionContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection, nameof (UpgradeAggregationsBootstrapper));
      this.protocol = protocol;
      this.collectionContext = collectionContext;
      this.destinationMigration = destinationMigration;
      this.feedsToMigrate = feedsToMigrate ?? Enumerable.Empty<Guid>();
      this.feedService = feedService;
      this.servicingContextLogger = servicingContextLogger;
      this.migrationsDefinitionsProvider = migrationsDefinitionsProvider;
      this.migrationJobHandler = migrationJobHandler;
      this.changeProcessingJobHandler = changeProcessingJobHandler;
      this.itemStoreMetadataMigration = itemStoreMetadataMigration;
    }

    public IAsyncHandler Bootstrap()
    {
      GivenMigrationAsDefaultMigrationsProvider migrationsProvider = new GivenMigrationAsDefaultMigrationsProvider(this.migrationsDefinitionsProvider, this.destinationMigration);
      ByFuncFactory<IMigrationTransitionerInternal> transitionerFactory = new ByFuncFactory<IMigrationTransitionerInternal>((Func<IMigrationTransitionerInternal>) (() => new NoCachingMigrationTransitionerBootstrapper(this.collectionContext, (IMigrationDefinitionsProvider) migrationsProvider).Bootstrap()));
      FeedMigrationAndChangeProcessingInlinePerformerAsQueuer inlineFeedJobsPerformer = new FeedMigrationAndChangeProcessingInlinePerformerAsQueuer(this.feedService, this.migrationJobHandler, this.changeProcessingJobHandler);
      ByFuncInputFactory<CollectionId, IDisposingFeedJobQueuer> jobQueuerFactory = new ByFuncInputFactory<CollectionId, IDisposingFeedJobQueuer>((Func<CollectionId, IDisposingFeedJobQueuer>) (id => (IDisposingFeedJobQueuer) new DontDisposeFeedJobQueuer((IFeedJobQueuer) inlineFeedJobsPerformer)));
      IProtocol protocol = this.protocol;
      ITracerService tracerFacade = this.collectionContext.GetTracerFacade();
      MigrationKickerJobHandler kickerJobHandler = new MigrationKickerJobHandler((IFactory<IMigrationTransitionerInternal>) transitionerFactory, (IFactory<CollectionId, IDisposingFeedJobQueuer>) jobQueuerFactory, protocol, tracerFacade);
      return (IAsyncHandler) new UpgradeAggregations(this.collectionContext.GetExecutionEnvironmentFacade(), new MigrationTransitionerWithGivenMigrationAsDefaultBootstrapper(this.collectionContext, this.migrationsDefinitionsProvider, this.itemStoreMetadataMigration).Bootstrap(), (IAsyncHandler<MigrationKickerRequest, JobResult>) kickerJobHandler, (IAsyncHandler<NullRequest, IEnumerable<FeedCore>>) new ByAsyncFuncAsyncHandler<NullRequest, IEnumerable<FeedCore>>((Func<NullRequest, Task<IEnumerable<FeedCore>>>) (async r =>
      {
        IEnumerable<FeedCore> feedsAsync = await this.feedService.GetFeedsAsync();
        if (!this.feedsToMigrate.Any<Guid>())
          return feedsAsync;
        Dictionary<Guid, FeedCore> allFeedsDictionary = feedsAsync.ToDictionary<FeedCore, Guid>((Func<FeedCore, Guid>) (x => x.Id));
        if (this.feedsToMigrate.Any<Guid>((Func<Guid, bool>) (feedId => !allFeedsDictionary.ContainsKey(feedId))))
          throw new Exception("One or more feedIds might be invalid: " + string.Join<Guid>(",", this.feedsToMigrate));
        return this.feedsToMigrate.Select<Guid, FeedCore>((Func<Guid, FeedCore>) (feed => allFeedsDictionary[feed]));
      })), (IFactory<IProtocol, string>) new GivenOrDefaultMigrationNameFactory(this.migrationsDefinitionsProvider, this.destinationMigration), this.servicingContextLogger, this.protocol);
    }
  }
}
