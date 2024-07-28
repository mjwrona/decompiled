// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ChangeProcessing.FeedChangeProcessingJobHandlerFactoryBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Migration;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeleteProcessingJob;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.StorageDeleters;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ChangeProcessing
{
  public class FeedChangeProcessingJobHandlerFactoryBootstrapper : 
    IBootstrapper<IFactory<JobId?, IAsyncHandler<IFeedRequest, JobResult>>>
  {
    private readonly IVssRequestContext requestContext;
    private readonly ICommitLogReader<CommitLogEntry> commitLogReader;
    private readonly ICommitLogWriter<ICommitLogEntry> commitLogWriter;
    private readonly BookmarkTokenKey changeProcessingBookmarkTokenKey;
    private readonly IMigrationDefinitionsProvider migrationDefinitionsProvider;
    private readonly IShouldMarkFactory shouldMarkFactory;
    private readonly Func<IStorageDeletionRequest<BlobStorageId>, IdBlobReference>? getBlobReferenceFunc;
    private readonly Func<IStorageDeletionRequest<DedupStoreStorageId>, IdBlobReference>? getDedupStoreReferenceFunc;
    private readonly bool handleOnlyMigrationCatchup;
    private readonly IEnumerable<IPduNotificationDetector> protocolSpecificPduDetectors;
    private readonly ICancellationFacade cancellationFacade;
    private readonly JobCreationInfo dpjCreationInfo;

    public FeedChangeProcessingJobHandlerFactoryBootstrapper(
      IVssRequestContext requestContext,
      ICommitLogReader<CommitLogEntry> commitLogReader,
      ICommitLogWriter<ICommitLogEntry> commitLogWriter,
      BookmarkTokenKey changeProcessingBookmarkTokenKey,
      IMigrationDefinitionsProvider migrationDefinitionsProvider,
      IShouldMarkFactory shouldMarkFactory,
      Func<IStorageDeletionRequest<BlobStorageId>, IdBlobReference>? getBlobReferenceFunc,
      Func<IStorageDeletionRequest<DedupStoreStorageId>, IdBlobReference>? getDedupStoreReferenceFunc,
      ICancellationFacade cancellationFacade,
      JobCreationInfo dpjCreationInfo,
      bool handleOnlyMigrationCatchup,
      IEnumerable<IPduNotificationDetector> protocolSpecificPduDetectors)
    {
      this.requestContext = requestContext;
      this.commitLogReader = commitLogReader;
      this.commitLogWriter = commitLogWriter;
      this.changeProcessingBookmarkTokenKey = changeProcessingBookmarkTokenKey;
      this.migrationDefinitionsProvider = migrationDefinitionsProvider;
      this.shouldMarkFactory = shouldMarkFactory;
      this.getBlobReferenceFunc = getBlobReferenceFunc;
      this.getDedupStoreReferenceFunc = getDedupStoreReferenceFunc;
      this.handleOnlyMigrationCatchup = handleOnlyMigrationCatchup;
      this.protocolSpecificPduDetectors = protocolSpecificPduDetectors;
      this.cancellationFacade = cancellationFacade;
      this.dpjCreationInfo = dpjCreationInfo;
    }

    public IFactory<JobId?, IAsyncHandler<IFeedRequest, JobResult>> Bootstrap()
    {
      IExecutionEnvironment environmentFacade = this.requestContext.GetExecutionEnvironmentFacade();
      IFactory<IAggregation, IAggregationAccessor> accessorFactory = new AggregationAccessorFactoryBootstrapper(this.requestContext).Bootstrap();
      ByFuncFactory<IMigrationTransitionerInternal> transitionerFactory = new ByFuncFactory<IMigrationTransitionerInternal>((Func<IMigrationTransitionerInternal>) (() => new NoCachingMigrationTransitionerBootstrapper(this.requestContext, this.migrationDefinitionsProvider).Bootstrap()));
      ReturnSameInstanceFactory<ICommitLogReader<CommitLogEntry>> commitLogService = new ReturnSameInstanceFactory<ICommitLogReader<CommitLogEntry>>(this.commitLogReader);
      ByAsyncFuncAsyncHandler<MarkCommitAsCorruptRequest> markCommitAsCorruptHandler = new ByAsyncFuncAsyncHandler<MarkCommitAsCorruptRequest>((Func<MarkCommitAsCorruptRequest, Task>) (async request => await this.commitLogWriter.MarkCommitLogEntryCorruptAsync(request.Feed, request.CommitId)));
      ByFuncAsyncHandler<MarkCommitAsCorruptRequest, bool> shouldMarkCommitAsCorruptHandler = new ByFuncAsyncHandler<MarkCommitAsCorruptRequest, bool>((Func<MarkCommitAsCorruptRequest, bool>) (request => this.shouldMarkFactory.Create(request.IsForceMode).ShouldMark(request.Exception)));
      RegistryForceMarkFactory forceMarkFactory = new RegistryForceMarkFactory();
      ByFuncAsyncHandler<FeedCore, bool> forceModeHandler = new ByFuncAsyncHandler<FeedCore, bool>((Func<FeedCore, bool>) (feed => forceMarkFactory.ComputeForceMode(this.requestContext, feed.Id)));
      IAggregationAccessorFactory aggregationAccessorFactory1 = new WriteAggregationAccessorFactoryBootstrapper(this.requestContext, this.migrationDefinitionsProvider, accessorFactory).Bootstrap();
      IAggregationAccessorFactory aggregationAccessorFactory2 = new MigrationAccessorFactoryBootstrapper(this.requestContext, this.migrationDefinitionsProvider, accessorFactory).Bootstrap();
      IAsyncHandler<IEnumerable<IStorageDeletionRequest>, NullResult> storageDeleter = new StorageDeleterHandlerBootstrapper(this.requestContext, this.getBlobReferenceFunc, this.getDedupStoreReferenceFunc).Bootstrap();
      DefaultTimeProvider defaultTimeProvider = new DefaultTimeProvider();
      CombiningCommitApplier postAggregationCommitApplier = new CombiningCommitApplier(new ICommitApplier[3]
      {
        (ICommitApplier) new CleanupStorageCommitApplier(storageDeleter),
        (ICommitApplier) PublishNotifierCommitApplier.Bootstrap(this.requestContext, (ITimeProvider) defaultTimeProvider, this.protocolSpecificPduDetectors),
        (ICommitApplier) new ScheduleDpjCommitApplier(new DeletedPackageJobQueuerBootstrapper(this.requestContext, this.dpjCreationInfo).Bootstrap(), (ITimeProvider) defaultTimeProvider, (IAsyncHandler<IDeleteOperationData, DateTime>) new ScheduledPermanentDeleteDateResolvingHandler(new ScheduledPermanentDeleteDateCalculatingHandlerBootstrapper(this.requestContext).Bootstrap()))
      });
      DependencyResolvingAggregationCommitApplier aggregationCommitApplier = new DependencyResolvingAggregationCommitApplierBootstrapper(this.requestContext, false).Bootstrap();
      FeedChangeProcessingJobHandler feedChangeProcessingJobHandler = new FeedChangeProcessingJobHandler((IFactory<IMigrationTransitionerInternal>) transitionerFactory, (IFactory<ICommitLogReader<CommitLogEntry>>) commitLogService, new CommitLogBookmarkTokenProviderBootstrapper(this.requestContext, this.changeProcessingBookmarkTokenKey).Bootstrap(), aggregationAccessorFactory1, (IAsyncHandler<MigrationCatchupRequest, int>) new MigrationCatchupHandler((IFactory<IMigrationTransitionerInternal>) transitionerFactory, (IFactory<ICommitLogReader<CommitLogEntry>>) commitLogService, aggregationAccessorFactory2, environmentFacade, (IAggregationCommitApplier) aggregationCommitApplier), environmentFacade, (IAsyncHandler<MarkCommitAsCorruptRequest, bool>) shouldMarkCommitAsCorruptHandler, (IAsyncHandler<MarkCommitAsCorruptRequest>) markCommitAsCorruptHandler, (IAsyncHandler<FeedCore, bool>) forceModeHandler, (IAggregationCommitApplier) aggregationCommitApplier, (ICommitApplier) postAggregationCommitApplier, this.cancellationFacade, (IPackagingTracesBasicInfo) new PackagingTracesBasicInfo(this.requestContext), this.handleOnlyMigrationCatchup);
      return (IFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>>) new ByFuncInputFactory<JobId, IAsyncHandler<IFeedRequest, JobResult>>((Func<JobId, IAsyncHandler<IFeedRequest, JobResult>>) (jobId => !((GuidBasedId) null == (GuidBasedId) jobId) ? new JobErrorHandlerBootstrapper<IFeedRequest>(this.requestContext, (IAsyncHandler<IFeedRequest, JobResult>) feedChangeProcessingJobHandler, jobId).Bootstrap() : (IAsyncHandler<IFeedRequest, JobResult>) feedChangeProcessingJobHandler));
    }
  }
}
