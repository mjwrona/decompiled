// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.Migration.MigrationJobHandlerFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.Migration
{
  public class MigrationJobHandlerFactoryBootstrapper : 
    IBootstrapper<IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly IMigrationDefinitionsProvider migrationsProvider;
    private readonly ICommitLogReader<CommitLogEntry> commitLogReader;
    private readonly IFeedJobQueuer changeProcessingJobQueuer;
    private readonly IFeedJobQueuer migrationProcessingJobQueuer;
    private readonly BookmarkTokenKey changeProcessingBookmarkTokenKey;
    private IFactory<int> numberOfBatchesFactory;

    public MigrationJobHandlerFactoryBootstrapper(
      IVssRequestContext requestContext,
      IMigrationDefinitionsProvider migrationsProvider,
      ICommitLogReader<CommitLogEntry> commitLogReader,
      IFeedJobQueuer changeProcessingJobQueuer,
      IFeedJobQueuer migrationProcessingJobQueuer,
      BookmarkTokenKey changeProcessingBookmarkTokenKey,
      IFactory<int> migrationJobNumberOfBatchesFactory = null)
    {
      this.requestContext = requestContext;
      this.migrationsProvider = migrationsProvider;
      this.commitLogReader = commitLogReader;
      this.changeProcessingJobQueuer = changeProcessingJobQueuer;
      this.migrationProcessingJobQueuer = migrationProcessingJobQueuer;
      this.changeProcessingBookmarkTokenKey = changeProcessingBookmarkTokenKey;
      this.numberOfBatchesFactory = migrationJobNumberOfBatchesFactory;
    }

    public IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>> Bootstrap()
    {
      IAggregationAccessorFactory aggregationAccessorFactory = new MigrationAccessorFactoryBootstrapper(this.requestContext, this.migrationsProvider, new AggregationAccessorFactoryBootstrapper(this.requestContext).Bootstrap()).Bootstrap();
      this.numberOfBatchesFactory = this.numberOfBatchesFactory ?? (IFactory<int>) new MigrationJobNumberOfBatchesToProcessFactory(this.requestContext.GetExecutionEnvironmentFacade());
      ReturnSameInstanceFactory<ICommitEnumeratingStrategy> commitEnumeratingStrategyFactory = new ReturnSameInstanceFactory<ICommitEnumeratingStrategy>((ICommitEnumeratingStrategy) new FlattenAndBatchCommitEnumeratingStrategy(this.commitLogReader, (IFactory<int>) new MigrationJobBatchSizeFactory((IRegistryService) new RegistryServiceFacade(this.requestContext))));
      MigrationJobHandler migrationJobHandler = new MigrationJobHandler((IFactory<IMigrationTransitionerInternal>) new ByFuncFactory<IMigrationTransitionerInternal>((Func<IMigrationTransitionerInternal>) (() => new NoCachingMigrationTransitionerBootstrapper(this.requestContext, this.migrationsProvider).Bootstrap())), new JobQueuerFactoryBootstrapperForMigrationJobs(this.requestContext, this.changeProcessingJobQueuer, this.migrationProcessingJobQueuer).Bootstrap(), new CommitLogBookmarkTokenProviderBootstrapper(this.requestContext, this.changeProcessingBookmarkTokenKey).Bootstrap(), this.numberOfBatchesFactory, aggregationAccessorFactory, new CollectionId(this.requestContext.ServiceHost.InstanceId), (IAggregationCommitApplier) new InParallelAndCacheSkippingCommitApplier(this.requestContext.GetTracerFacade()), (IFactory<ICommitEnumeratingStrategy>) commitEnumeratingStrategyFactory);
      return (IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>>) new ByFuncInputFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>>((Func<JobId, IAsyncHandler<IFeedRequest, JobResult>>) (jobId => !((GuidBasedId) null == (GuidBasedId) jobId) ? new JobErrorHandlerBootstrapper<IFeedRequest>(this.requestContext, (IAsyncHandler<IFeedRequest, JobResult>) migrationJobHandler, jobId).Bootstrap() : (IAsyncHandler<IFeedRequest, JobResult>) migrationJobHandler));
    }
  }
}
