// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeleteProcessingJob.FeedDeletedPackageJobHandlerBootstrapper`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.DeleteProcessingJob;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeleteProcessingJob
{
  public class FeedDeletedPackageJobHandlerBootstrapper<TPackageIdentity> : 
    IBootstrapper<IAsyncHandler<IFeedRequest, JobResult>>
    where TPackageIdentity : class, IPackageIdentity
  {
    private readonly IVssRequestContext requestContext;
    private readonly Guid jobId;
    private readonly ICommitLogWriter<ICommitLogEntry> commitLogWriter;
    private readonly ICommitLogReader<CommitLogEntry> commitLogReader;
    private readonly IAsyncHandler<PackageRequest<TPackageIdentity>, IPermanentDeleteOperationData> permanentOpDataGeneratingHandler;
    private readonly IAsyncHandler<PackageRequest<TPackageIdentity>, PackageDeletionState> deletionStateFetchingHandler;
    private readonly IFeedJobQueuer changeProcessingJobQueuer;
    private readonly JobCreationInfo deletedPackageJobCreationInfo;
    private readonly BookmarkTokenKey changeProcessingBookmarkTokenKey;
    private readonly BookmarkTokenKey deleteProcessingBookmarkTokenKey;

    public FeedDeletedPackageJobHandlerBootstrapper(
      IVssRequestContext requestContext,
      Guid jobId,
      ICommitLogWriter<ICommitLogEntry> commitLogWriter,
      ICommitLogReader<CommitLogEntry> commitLogReader,
      IAsyncHandler<PackageRequest<TPackageIdentity>, IPermanentDeleteOperationData> permanentOpDataGeneratingHandler,
      IAsyncHandler<PackageRequest<TPackageIdentity>, PackageDeletionState> deletionStateFetchingHandler,
      IFeedJobQueuer changeProcessingJobQueuer,
      JobCreationInfo deletedPackageJobCreationInfo,
      BookmarkTokenKey changeProcessingBookmarkTokenKey,
      BookmarkTokenKey deleteProcessingBookmarkTokenKey)
    {
      this.requestContext = requestContext;
      this.jobId = jobId;
      this.commitLogWriter = commitLogWriter;
      this.commitLogReader = commitLogReader;
      this.permanentOpDataGeneratingHandler = permanentOpDataGeneratingHandler;
      this.deletionStateFetchingHandler = deletionStateFetchingHandler;
      this.changeProcessingJobQueuer = changeProcessingJobQueuer;
      this.deletedPackageJobCreationInfo = deletedPackageJobCreationInfo;
      this.changeProcessingBookmarkTokenKey = changeProcessingBookmarkTokenKey;
      this.deleteProcessingBookmarkTokenKey = deleteProcessingBookmarkTokenKey;
    }

    public IAsyncHandler<IFeedRequest, JobResult> Bootstrap() => new JobErrorHandlerBootstrapper<IFeedRequest>(this.requestContext, (IAsyncHandler<IFeedRequest, JobResult>) new FeedLevelDeletedPackageJobHandler<TPackageIdentity>((ITimeProvider) new DefaultTimeProvider(), new CommitLogBookmarkTokenProviderBootstrapper(this.requestContext, this.deleteProcessingBookmarkTokenKey).Bootstrap(), new CommitLogBookmarkTokenProviderBootstrapper(this.requestContext, this.changeProcessingBookmarkTokenKey).Bootstrap(), (IUnflattenedCommitEnumeratingStrategy) new UnflattenedCommitEnumeratingStrategy(this.commitLogReader), this.permanentOpDataGeneratingHandler, this.deletionStateFetchingHandler, this.commitLogWriter, this.requestContext.GetTracerFacade(), (IAsyncHandler<IDeleteOperationData, DateTime>) new ScheduledPermanentDeleteDateResolvingHandler(new ScheduledPermanentDeleteDateCalculatingHandlerBootstrapper(this.requestContext).Bootstrap()), this.changeProcessingJobQueuer, new DeletedPackageJobQueuerBootstrapper(this.requestContext, this.deletedPackageJobCreationInfo).Bootstrap(), (ICancellationFacade) new CancellationFacade(this.requestContext), new DeletedPackageJobMaxRunTimeFactoryBootstrapper(this.requestContext).Bootstrap(), new DeletedPackageJobFlushIntervalFactoryBootstrapper(this.requestContext).Bootstrap(), new DeletedPackageJobMaxPendingPermDeleteOpsFactoryBootstrapper(this.requestContext).Bootstrap(), new DeletedPackageJobMaxPermDeleteBatchSizeFactoryBootstrapper(this.requestContext).Bootstrap()), (JobId) this.jobId).Bootstrap();
  }
}
