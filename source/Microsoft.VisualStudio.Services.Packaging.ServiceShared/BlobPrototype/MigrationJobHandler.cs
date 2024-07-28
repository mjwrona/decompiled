// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MigrationJobHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class MigrationJobHandler : 
    IAsyncHandler<IFeedRequest, JobResult>,
    IHaveInputType<IFeedRequest>,
    IHaveOutputType<JobResult>
  {
    private readonly IFactory<IMigrationTransitionerInternal> transitionerFactory;
    private readonly IFactory<JobType, IFeedJobQueuer> jobQueuerFactory;
    private readonly IBookmarkTokenProvider<FeedCore, CommitLogBookmark> bookmarkTokenProvider;
    private readonly IAggregationAccessorFactory aggregationAccessorFactory;
    private readonly IAggregationCommitApplier aggregationCommitApplier;
    private readonly IFactory<ICommitEnumeratingStrategy> commitEnumeratingStrategyFactory;
    private readonly CollectionId collectionId;
    private readonly IFactory<int> numBatchesToProcessFactory;

    public MigrationJobHandler(
      IFactory<IMigrationTransitionerInternal> transitionerFactory,
      IFactory<JobType, IFeedJobQueuer> jobQueuerFactory,
      IBookmarkTokenProvider<FeedCore, CommitLogBookmark> bookmarkTokenProvider,
      IFactory<int> numBatchesToProcessFactory,
      IAggregationAccessorFactory aggregationAccessorFactory,
      CollectionId collectionId,
      IAggregationCommitApplier aggregationCommitApplier,
      IFactory<ICommitEnumeratingStrategy> commitEnumeratingStrategyFactory)
    {
      this.transitionerFactory = transitionerFactory;
      this.jobQueuerFactory = jobQueuerFactory;
      this.bookmarkTokenProvider = bookmarkTokenProvider;
      this.numBatchesToProcessFactory = numBatchesToProcessFactory;
      this.aggregationAccessorFactory = aggregationAccessorFactory;
      this.collectionId = collectionId;
      this.aggregationCommitApplier = aggregationCommitApplier;
      this.commitEnumeratingStrategyFactory = commitEnumeratingStrategyFactory;
    }

    public async Task<JobResult> Handle(IFeedRequest feedRequest)
    {
      MigrationProcessingJobHandlerTelemetry handlerTelemetry = new MigrationProcessingJobHandlerTelemetry();
      handlerTelemetry.FeedId = feedRequest.Feed.Id;
      MigrationProcessingJobHandlerTelemetry telemetry = handlerTelemetry;
      FeedCore feed = feedRequest.Feed;
      try
      {
        ICommitEnumeratingStrategy commitEnumeratingStrategy = this.commitEnumeratingStrategyFactory.Get();
        int numBatchesToProcess = this.numBatchesToProcessFactory.Get();
        MigrationState state = (MigrationState) await this.transitionerFactory.Get().GetOrCreateState(this.collectionId, feed.Id, feedRequest.Protocol);
        MigrationProcessingStatus processingStatus1 = state.MigrationProgress;
        PackagingCommitId packagingCommitId;
        if (processingStatus1 == null)
        {
          MigrationProcessingStatus processingStatus2 = new MigrationProcessingStatus();
          packagingCommitId = PackagingCommitId.Empty;
          processingStatus2.CommitToken = packagingCommitId.ToString();
          processingStatus1 = processingStatus2;
        }
        MigrationProcessingStatus migrationProcessingStatus = processingStatus1;
        if (state.VNextState != MigrationStateEnum.Computing)
        {
          telemetry.Message = string.Format("Migration state not Computing. Current State: {0}", (object) state.VNextState);
          if (state.VNextState == MigrationStateEnum.JobCatchup)
          {
            Guid guid = await this.jobQueuerFactory.Get(ChangeProcessingJobConstants.ChangeProcessingJobType).QueueJob(feed, feedRequest.Protocol, JobPriorityLevel.Normal);
            telemetry.Message += " Change processing kicked off.";
          }
          telemetry.ExitReason = MigrationProcessingJobHandlerExitReason.NotInComputingState;
          return JobResult.Succeeded((JobTelemetry) telemetry);
        }
        PackagingCommitId lastCommitIdToProcess = this.bookmarkTokenProvider.GetToken(feed).CommitId;
        telemetry.DestinationCommitId = lastCommitIdToProcess;
        if (lastCommitIdToProcess == PackagingCommitId.Empty)
        {
          await this.MarkStateAndQueueChangeProcessingJob(state, feedRequest, migrationProcessingStatus);
          telemetry.Message = "No change processing bookmark found. No migration necessary";
          telemetry.ExitReason = MigrationProcessingJobHandlerExitReason.NoChangeProcessingBookmark;
          return JobResult.Succeeded((JobTelemetry) telemetry);
        }
        if (migrationProcessingStatus.FinalCommitSequenceNumber <= 0L)
          migrationProcessingStatus.FinalCommitSequenceNumber = (await commitEnumeratingStrategy.GetNewestCommitAsync(feed)).SequenceNumber;
        for (int batchNum = 1; batchNum <= numBatchesToProcess; ++batchNum)
        {
          CommitEntryBatch commitEntryBatch = await commitEnumeratingStrategy.GetNextBatchAsync(feed, PackagingCommitId.Parse(migrationProcessingStatus.CommitToken), lastCommitIdToProcess);
          CommitLogBookmark consultedCommitBookmark = commitEntryBatch.LastConsultedCommitBookmark;
          if (consultedCommitBookmark.CommitId == PackagingCommitId.Empty)
          {
            telemetry.Message = "Migration job was started with migration progress at end of commit log.";
            telemetry.ExitReason = MigrationProcessingJobHandlerExitReason.MigrationProgressAtEndOfCommitLog;
            return JobResult.Failed((JobTelemetry) telemetry);
          }
          if (commitEntryBatch.CommitLogEntries.Any<ICommitLogEntry>())
            await this.ProcessCommitLogBatch(feedRequest, commitEntryBatch.CommitLogEntries, telemetry);
          ICommitLogEntry commitLogEntry = commitEntryBatch.CommitLogEntries.LastOrDefault<ICommitLogEntry>();
          MigrationProcessingStatus processingStatus3 = migrationProcessingStatus;
          consultedCommitBookmark = commitEntryBatch.LastConsultedCommitBookmark;
          packagingCommitId = consultedCommitBookmark.CommitId;
          string str = packagingCommitId.ToString();
          processingStatus3.CommitToken = str;
          if (commitLogEntry != null)
            migrationProcessingStatus.CurrentCommitSequenceNumber = commitLogEntry.SequenceNumber;
          consultedCommitBookmark = commitEntryBatch.LastConsultedCommitBookmark;
          packagingCommitId = consultedCommitBookmark.CommitId;
          if (packagingCommitId.Equals(lastCommitIdToProcess))
          {
            await this.MarkStateAndQueueChangeProcessingJob(state, feedRequest, migrationProcessingStatus);
            telemetry.Message = "Migration completed.";
            telemetry.ExitReason = MigrationProcessingJobHandlerExitReason.ReachedTargetCommit;
            return JobResult.Succeeded((JobTelemetry) telemetry);
          }
          await this.transitionerFactory.Get().Apply((IReadOnlyCollection<IMigrationTransition>) new MarkAsComputingTransition[1]
          {
            new MarkAsComputingTransition(state.VNextMigration, this.collectionId, feed.Id, feedRequest.Protocol, this.TransformOutbound(migrationProcessingStatus))
          });
          commitEntryBatch = (CommitEntryBatch) null;
        }
        Guid guid1 = await this.jobQueuerFactory.Get(MigrationJobConstants.MigrationProcessingJobType).QueueJob(feed, feedRequest.Protocol, JobPriorityLevel.Normal);
        telemetry.Message = "BatchSize limit reached. Processing status: " + JsonConvert.SerializeObject((object) migrationProcessingStatus);
        telemetry.ExitReason = MigrationProcessingJobHandlerExitReason.ReachedEndOfBatch;
        return JobResult.Succeeded((JobTelemetry) telemetry);
      }
      catch (Exception ex)
      {
        MigrationProcessingJobHandlerTelemetry telemetry1 = telemetry;
        throw new JobFailedException(ex, (JobTelemetry) telemetry1);
      }
    }

    private async Task ProcessCommitLogBatch(
      IFeedRequest feedRequest,
      IReadOnlyList<ICommitLogEntry> commitLogBatch,
      MigrationProcessingJobHandlerTelemetry telemetry)
    {
      telemetry.InitialCommit = telemetry.InitialCommit == CommitLogBookmark.Empty ? commitLogBatch.First<ICommitLogEntry>().GetCommitLogBookmark() : telemetry.InitialCommit;
      telemetry.CurrentCommitId = commitLogBatch.Last<ICommitLogEntry>().GetCommitLogBookmark();
      IReadOnlyList<IAggregationAccessor> accessorsFor = await this.aggregationAccessorFactory.GetAccessorsFor(feedRequest);
      telemetry.Accessors = accessorsFor.Select<IAggregationAccessor, string>((Func<IAggregationAccessor, string>) (a => a.Aggregation.Definition.Name + "." + a.Aggregation.VersionName));
      AggregationApplyTimings aggregationApplyTimings = await this.aggregationCommitApplier.ApplyCommitAsync(accessorsFor, feedRequest, commitLogBatch);
      telemetry.ProcessedChangeCount += commitLogBatch.Select<ICommitLogEntry, PackagingCommitId>((Func<ICommitLogEntry, PackagingCommitId>) (c => c.CommitId)).Distinct<PackagingCommitId>().Count<PackagingCommitId>();
    }

    private async Task MarkStateAndQueueChangeProcessingJob(
      MigrationState state,
      IFeedRequest feedRequest,
      MigrationProcessingStatus migrationProcessingStatus)
    {
      Guid id = feedRequest.Feed.Id;
      IProtocol protocol = feedRequest.Protocol;
      if (state.Instruction > MigrationInstructionEnum.Compute)
      {
        await this.transitionerFactory.Get().Apply((IReadOnlyCollection<IMigrationTransition>) new MarkAsJobCatchupTransition[1]
        {
          new MarkAsJobCatchupTransition(state.VNextMigration, this.collectionId, id, protocol, this.TransformOutbound(migrationProcessingStatus))
        });
        Guid guid = await this.jobQueuerFactory.Get(ChangeProcessingJobConstants.ChangeProcessingJobType).QueueJob(feedRequest.Feed, protocol, JobPriorityLevel.Normal);
        protocol = (IProtocol) null;
      }
      else
      {
        await this.transitionerFactory.Get().Apply((IReadOnlyCollection<IMigrationTransition>) new MarkAsComputingTransition[1]
        {
          new MarkAsComputingTransition(state.VNextMigration, this.collectionId, id, protocol, this.TransformOutbound(migrationProcessingStatus))
        });
        protocol = (IProtocol) null;
      }
    }

    private MigrationProcessingStatus TransformOutbound(
      MigrationProcessingStatus migrationProcessingStatus)
    {
      return migrationProcessingStatus != null && migrationProcessingStatus.CommitToken == PackagingCommitId.Empty.ToString() ? (MigrationProcessingStatus) null : migrationProcessingStatus;
    }
  }
}
