// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement.CollectionDeletedFeedsProcessingJobHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.StorageDeleters;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement
{
  public class CollectionDeletedFeedsProcessingJobHandler : 
    IAsyncHandler<TeamFoundationJobDefinition, JobResult>,
    IHaveInputType<TeamFoundationJobDefinition>,
    IHaveOutputType<JobResult>
  {
    internal const string FeedCountBatchSizeRegistryPath = "/Configuration/Packaging/DeletedFeedsProcessing/FeedCountBatchSize";
    internal const string FeedGracePeriodHoursRegistryPath = "/Configuration/Packaging/DeletedFeedsProcessing/FeedDeletionGraceHours";
    internal const string DFPJRequeueTimeMinutesRegistryPath = "/Configuration/Packaging/DeletedFeedsProcessing/RequeueTimeOnPartialSuccessInMinutes";
    private const int DefaultFeedCountBatchSize = 1000;
    private readonly TimeSpan DefaultFeedDeleteGracePeriod = TimeSpan.Zero;
    private readonly TimeSpan DefaultRequeueTime = TimeSpan.Zero;
    private readonly ITimeProvider timeProvider;
    private readonly IFeatureFlagService featureFlagService;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IRegistryService registryService;
    private readonly IBookmarkTokenProvider<CollectionId, long> tokenProvider;
    private readonly IFeedChangesService feedChangesService;
    private readonly IMigrationTransitioner migrationTransitioner;
    private readonly IFactory<ICommitLog> commitLogService;
    private readonly IProtocol protocol;
    private readonly ITracerService tracerService;
    private readonly IAsyncHandler<IEnumerable<IStorageDeletionRequest>, NullResult> storageDeleter;
    private readonly ICommitOperationData finishedCleanupCommitData;
    private TimeSpan gracePeriod;
    private TimeSpan requeueTime;
    private int batchSize;
    private DateTime jobStartTime;
    private Guid collectionId;
    private Stack<Guid> feedsProcessed;
    private IAsyncHandler<FeedRequest<IAddOperationData>, IStorageDeletionRequest> storageDeletionRequestCreator;

    public CollectionDeletedFeedsProcessingJobHandler(
      ITimeProvider timeProvider,
      IFeatureFlagService featureFlagService,
      IExecutionEnvironment executionEnvironment,
      IRegistryService registryService,
      IBookmarkTokenProvider<CollectionId, long> tokenProvider,
      IFeedChangesService feedChangesService,
      IMigrationTransitioner migrationTransitioner,
      IFactory<ICommitLog> commitLogService,
      IProtocol protocol,
      ITracerService tracerService,
      IAsyncHandler<IEnumerable<IStorageDeletionRequest>, NullResult> storageDeleter,
      IAsyncHandler<FeedRequest<IAddOperationData>, IStorageDeletionRequest> storageDeletionRequestCreator,
      ICommitOperationData finishedCleanupCommitData)
    {
      this.timeProvider = timeProvider;
      this.featureFlagService = featureFlagService;
      this.executionEnvironment = executionEnvironment;
      this.registryService = registryService;
      this.tokenProvider = tokenProvider;
      this.feedChangesService = feedChangesService;
      this.migrationTransitioner = migrationTransitioner;
      this.commitLogService = commitLogService;
      this.protocol = protocol;
      this.tracerService = tracerService;
      this.storageDeleter = storageDeleter;
      this.storageDeletionRequestCreator = storageDeletionRequestCreator;
      this.finishedCleanupCommitData = finishedCleanupCommitData;
    }

    public async Task<JobResult> Handle(TeamFoundationJobDefinition request)
    {
      CollectionDeletedFeedsProcessingJobHandler sendInTheThisObject = this;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (Handle)))
      {
        CollectionDeletedFeedProcessingJobDataModel processingJobDataModel = request?.Data == null ? (CollectionDeletedFeedProcessingJobDataModel) null : TeamFoundationSerializationUtility.Deserialize<CollectionDeletedFeedProcessingJobDataModel>(request.Data);
        CollectionDeletedFeedsProcessingJobTelemetry telemetry = new CollectionDeletedFeedsProcessingJobTelemetry()
        {
          MigrationStateDeletionEnabled = true,
          CollectionId = sendInTheThisObject.executionEnvironment.HostId
        };
        if (!sendInTheThisObject.featureFlagService.IsEnabled("Packaging.DeletedFeeds.EnableProcessingJob"))
        {
          telemetry.Message = "Feature 'Packaging.DeletedFeeds.EnableProcessingJob' disabled for the collection.";
          return JobResult.Succeeded((JobTelemetry) telemetry);
        }
        long initialContinuationToken = 0;
        long currentContinuationToken = 0;
        try
        {
          sendInTheThisObject.Initialize();
          initialContinuationToken = sendInTheThisObject.tokenProvider.GetToken((CollectionId) sendInTheThisObject.collectionId);
          currentContinuationToken = initialContinuationToken;
          telemetry.InitialContinuationToken = new long?(currentContinuationToken);
          IAsyncEnumerable<DfpjFilteredFeedChange> asyncEnumerable;
          if (processingJobDataModel != null)
          {
            DfpjFilteredFeedChange.FeedToDelete[] source = new DfpjFilteredFeedChange.FeedToDelete[1];
            Microsoft.VisualStudio.Services.Feed.WebApi.Feed Feed = new Microsoft.VisualStudio.Services.Feed.WebApi.Feed();
            Feed.Id = processingJobDataModel.FeedId;
            source[0] = new DfpjFilteredFeedChange.FeedToDelete(Feed, initialContinuationToken);
            asyncEnumerable = (IAsyncEnumerable<DfpjFilteredFeedChange>) ((IEnumerable<DfpjFilteredFeedChange.FeedToDelete>) source).AsAsyncEnumerable<DfpjFilteredFeedChange.FeedToDelete>();
          }
          else
            asyncEnumerable = DfpjChangeFilter.FilterFeedsToDeleteAsyncEnumerable(sendInTheThisObject.feedChangesService.EnumerateFeedChangesAsyncEnumerable(initialContinuationToken, sendInTheThisObject.batchSize, true), sendInTheThisObject.jobStartTime - sendInTheThisObject.gracePeriod, initialContinuationToken);
          IAsyncEnumerator<DfpjFilteredFeedChange> asyncEnumerator = asyncEnumerable.GetAsyncEnumerator();
          try
          {
            do
            {
              if (await asyncEnumerator.MoveNextAsync())
              {
                DfpjFilteredFeedChange current = asyncEnumerator.Current;
                DfpjFilteredFeedChange.FeedToDelete feedToDelete = current as DfpjFilteredFeedChange.FeedToDelete;
                Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed;
                if ((object) feedToDelete != null)
                {
                  long continuationToken;
                  (feed, continuationToken) = feedToDelete;
                  await sendInTheThisObject.ProcessDeletedFeedCommitLogChanges((FeedCore) feed, telemetry);
                  ICommitLogEntry commitLogEntry = await sendInTheThisObject.commitLogService.Get().AppendEntryAsync((FeedCore) feed, sendInTheThisObject.finishedCleanupCommitData);
                  await sendInTheThisObject.DeleteMigrationState(feed.Id);
                  currentContinuationToken = continuationToken;
                  sendInTheThisObject.feedsProcessed.Push(feed.Id);
                }
                else
                {
                  DfpjFilteredFeedChange.LastContinuationToken continuationToken = current as DfpjFilteredFeedChange.LastContinuationToken;
                  if ((object) continuationToken != null)
                  {
                    long ContinuationToken;
                    continuationToken.Deconstruct(out ContinuationToken);
                    currentContinuationToken = ContinuationToken;
                  }
                  else
                    goto label_16;
                }
                feed = (Microsoft.VisualStudio.Services.Feed.WebApi.Feed) null;
              }
              else
                break;
            }
            while (sendInTheThisObject.feedsProcessed.Count < sendInTheThisObject.batchSize);
            goto label_21;
label_16:
            throw new ArgumentOutOfRangeException("feedChange");
          }
          finally
          {
label_21:
            if (asyncEnumerator != null)
              await asyncEnumerator.DisposeAsync();
          }
          asyncEnumerator = (IAsyncEnumerator<DfpjFilteredFeedChange>) null;
        }
        catch (Exception ex)
        {
          CollectionDeletedFeedsProcessingJobTelemetry telemetry1 = telemetry;
          throw new JobFailedException(ex, (JobTelemetry) telemetry1);
        }
        finally
        {
          telemetry = sendInTheThisObject.UpdateTelemetryData(telemetry);
          sendInTheThisObject.StoreToken(initialContinuationToken, currentContinuationToken, telemetry);
        }
        return sendInTheThisObject.feedsProcessed.Count < sendInTheThisObject.batchSize ? JobResult.Succeeded((JobTelemetry) telemetry) : JobResult.SuccessButMoreWorkToDo((JobTelemetry) telemetry, sendInTheThisObject.requeueTime);
      }
    }

    private void StoreToken(
      long initialContinuationToken,
      long currentContinuationToken,
      CollectionDeletedFeedsProcessingJobTelemetry telemetry)
    {
      try
      {
        if (currentContinuationToken <= initialContinuationToken)
          return;
        this.tokenProvider.StoreToken((CollectionId) this.collectionId, currentContinuationToken);
        telemetry.FinalContinuationToken = new long?(currentContinuationToken);
      }
      catch (Exception ex)
      {
        CollectionDeletedFeedsProcessingJobTelemetry telemetry1 = telemetry;
        throw new JobFailedException(ex, (JobTelemetry) telemetry1);
      }
    }

    private void Initialize()
    {
      this.feedsProcessed = new Stack<Guid>();
      this.collectionId = this.executionEnvironment.HostId;
      this.jobStartTime = this.timeProvider.Now;
      this.batchSize = this.registryService.GetValue<int>((RegistryQuery) "/Configuration/Packaging/DeletedFeedsProcessing/FeedCountBatchSize", 1000);
      this.gracePeriod = TimeSpan.FromHours(this.registryService.GetValue<double>((RegistryQuery) "/Configuration/Packaging/DeletedFeedsProcessing/FeedDeletionGraceHours", this.DefaultFeedDeleteGracePeriod.TotalHours));
      this.requeueTime = TimeSpan.FromMinutes(this.registryService.GetValue<double>((RegistryQuery) "/Configuration/Packaging/DeletedFeedsProcessing/RequeueTimeOnPartialSuccessInMinutes", this.DefaultRequeueTime.TotalMinutes));
    }

    private async Task DeleteMigrationState(Guid feedId)
    {
      CollectionDeletedFeedsProcessingJobHandler sendInTheThisObject = this;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (DeleteMigrationState)))
      {
        tracer.TraceInfo(string.Format("Deleting migration state for deleted feed with Id: {0}.", (object) feedId));
        if (!sendInTheThisObject.featureFlagService.IsEnabled("Packaging.DeletedFeeds.DeletePackageContent"))
          return;
        tracer.TraceInfo(string.Format("Deleted migration state for deleted feed with id: {0}. Deleted state: {1}", (object) feedId, (object) JsonConvert.SerializeObject((object) await sendInTheThisObject.migrationTransitioner.Delete(new CollectionId(sendInTheThisObject.executionEnvironment.HostId), feedId, sendInTheThisObject.protocol))));
      }
    }

    private async Task ProcessDeletedFeedCommitLogChanges(
      FeedCore feed,
      CollectionDeletedFeedsProcessingJobTelemetry telemetry)
    {
      CollectionDeletedFeedsProcessingJobHandler sendInTheThisObject = this;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (ProcessDeletedFeedCommitLogChanges)))
      {
        PackagingCommitId commitId = (await sendInTheThisObject.commitLogService.Get().GetOldestCommitBookmarkAsync(feed)).CommitId;
        while (commitId != PackagingCommitId.Empty)
        {
          CommitLogEntry commitEntry = await sendInTheThisObject.commitLogService.Get().GetEntryAsync(feed, commitId);
          await sendInTheThisObject.ProcessCommitLogChange(feed, commitEntry);
          ++telemetry.ProcessedChangeCount;
          commitId = commitEntry.NextCommitId;
          commitEntry = (CommitLogEntry) null;
        }
      }
    }

    private async Task ProcessCommitLogChange(FeedCore feed, CommitLogEntry entry)
    {
      CollectionDeletedFeedsProcessingJobHandler sendInTheThisObject = this;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (ProcessCommitLogChange)))
      {
        if (!(entry.CommitOperationData is IAddOperationData commitOperationData))
          return;
        tracer.TraceInfo(string.Format("Deleting package content from feed with Id: {0}. PackageStorageId = {1}", (object) feed.Id, (object) commitOperationData.PackageStorageId));
        if (!sendInTheThisObject.featureFlagService.IsEnabled("Packaging.DeletedFeeds.DeletePackageContent"))
          return;
        IStorageDeletionRequest storageDeletionRequest = await sendInTheThisObject.storageDeletionRequestCreator.Handle(new FeedRequest<IAddOperationData>(feed, sendInTheThisObject.protocol, commitOperationData));
        NullResult nullResult = await sendInTheThisObject.storageDeleter.Handle((IEnumerable<IStorageDeletionRequest>) new IStorageDeletionRequest[1]
        {
          storageDeletionRequest
        });
      }
    }

    private CollectionDeletedFeedsProcessingJobTelemetry UpdateTelemetryData(
      CollectionDeletedFeedsProcessingJobTelemetry telemetry)
    {
      telemetry.CollectionId = this.collectionId;
      telemetry.FeedUpdateGracePeriod = this.gracePeriod;
      telemetry.FeedCountBatchSize = this.batchSize;
      telemetry.JobStartTime = this.jobStartTime;
      telemetry.ProcessedFeedCount = this.feedsProcessed.Count;
      telemetry.SetFeedsProcessed(this.feedsProcessed);
      return telemetry;
    }
  }
}
