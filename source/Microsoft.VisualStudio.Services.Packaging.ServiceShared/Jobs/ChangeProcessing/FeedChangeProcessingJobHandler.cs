// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ChangeProcessing.FeedChangeProcessingJobHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ChangeProcessing
{
  public class FeedChangeProcessingJobHandler : 
    IAsyncHandler<IFeedRequest, JobResult>,
    IHaveInputType<IFeedRequest>,
    IHaveOutputType<JobResult>
  {
    private readonly IFactory<IMigrationTransitionerInternal> transitionerFactory;
    private readonly IFactory<ICommitLogReader<CommitLogEntry>> commitLogService;
    private readonly IAggregationAccessorFactory aggregationAccessorFactory;
    private readonly IAggregationCommitApplier aggregationCommitApplier;
    private readonly IBookmarkTokenProvider<FeedCore, CommitLogBookmark> tokenProvider;
    private readonly IAsyncHandler<MigrationCatchupRequest, int> migrationCatchupHandler;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IAsyncHandler<FeedCore, bool> forceModeHandler;
    private readonly IAsyncHandler<MarkCommitAsCorruptRequest, bool> shouldMarkCommitAsCorruptHandler;
    private readonly IAsyncHandler<MarkCommitAsCorruptRequest> markCommitAsCorruptHandler;
    private readonly bool handleOnlyMigrationCatchup;
    private readonly ICommitApplier postAggregationCommitApplier;
    private readonly ICancellationFacade cancellationFacade;
    private readonly IPackagingTracesBasicInfo packagingTracesBasicInfo;

    public FeedChangeProcessingJobHandler(
      IFactory<IMigrationTransitionerInternal> transitionerFactory,
      IFactory<ICommitLogReader<CommitLogEntry>> commitLogService,
      IBookmarkTokenProvider<FeedCore, CommitLogBookmark> tokenProvider,
      IAggregationAccessorFactory aggregationAccessorFactory,
      IAsyncHandler<MigrationCatchupRequest, int> migrationCatchupHandler,
      IExecutionEnvironment executionEnvironment,
      IAsyncHandler<MarkCommitAsCorruptRequest, bool> shouldMarkCommitAsCorruptHandler,
      IAsyncHandler<MarkCommitAsCorruptRequest> markCommitAsCorruptHandler,
      IAsyncHandler<FeedCore, bool> forceModeHandler,
      IAggregationCommitApplier aggregationCommitApplier,
      ICommitApplier postAggregationCommitApplier,
      ICancellationFacade cancellationFacade,
      IPackagingTracesBasicInfo packagingTracesBasicInfo,
      bool handleOnlyMigrationCatchup = false)
    {
      this.transitionerFactory = transitionerFactory;
      this.commitLogService = commitLogService;
      this.tokenProvider = tokenProvider;
      this.aggregationAccessorFactory = aggregationAccessorFactory;
      this.migrationCatchupHandler = migrationCatchupHandler;
      this.executionEnvironment = executionEnvironment;
      this.shouldMarkCommitAsCorruptHandler = shouldMarkCommitAsCorruptHandler;
      this.markCommitAsCorruptHandler = markCommitAsCorruptHandler;
      this.forceModeHandler = forceModeHandler;
      this.aggregationCommitApplier = aggregationCommitApplier;
      this.postAggregationCommitApplier = postAggregationCommitApplier;
      this.handleOnlyMigrationCatchup = handleOnlyMigrationCatchup;
      this.cancellationFacade = cancellationFacade;
      this.packagingTracesBasicInfo = packagingTracesBasicInfo;
    }

    public async Task<JobResult> Handle(IFeedRequest feedRequest)
    {
      Guid id = feedRequest.Feed.Id;
      FeedChangeProcessingJobHandlerTelemetry handlerTelemetry1 = new FeedChangeProcessingJobHandlerTelemetry();
      handlerTelemetry1.FeedId = id;
      FeedChangeProcessingJobHandlerTelemetry telemetry = handlerTelemetry1;
      this.packagingTracesBasicInfo.SetFromFeedRequest((IProtocolAgnosticFeedRequest) feedRequest);
      try
      {
        MigrationEntry state = await this.transitionerFactory.Get().GetOrCreateState((CollectionId) this.executionEnvironment.HostId, id, feedRequest.Protocol);
        CommitLogBookmark bookmarkedCommitId = this.tokenProvider.GetToken(feedRequest.Feed);
        telemetry.BookmarkedCommitId = bookmarkedCommitId;
        if (state.VNextState == MigrationStateEnum.JobCatchup)
        {
          FeedChangeProcessingJobHandlerTelemetry handlerTelemetry = telemetry;
          handlerTelemetry.CatchupChangeCount = await this.migrationCatchupHandler.Handle(new MigrationCatchupRequest(feedRequest, bookmarkedCommitId.CommitId));
          handlerTelemetry = (FeedChangeProcessingJobHandlerTelemetry) null;
        }
        if (!this.handleOnlyMigrationCatchup)
        {
          CommitLogBookmark currentBookmark;
          if (bookmarkedCommitId.CommitId != PackagingCommitId.Empty)
            currentBookmark = (await this.commitLogService.Get().GetEntryAsync(feedRequest.Feed, bookmarkedCommitId.CommitId)).GetNextCommitLogBookmark();
          else
            currentBookmark = await this.commitLogService.Get().GetOldestCommitBookmarkAsync(feedRequest.Feed);
          telemetry.InitialCommit = currentBookmark;
          telemetry.CurrentCommitId = currentBookmark;
          bool isForceMode = await this.forceModeHandler.Handle(feedRequest.Feed);
          return await this.ProcessCommitLogChanges(feedRequest, currentBookmark, isForceMode, telemetry);
        }
        bookmarkedCommitId = new CommitLogBookmark();
      }
      catch (Exception ex)
      {
        FeedChangeProcessingJobHandlerTelemetry telemetry1 = telemetry;
        throw new JobFailedException(ex, (JobTelemetry) telemetry1);
      }
      return JobResult.Succeeded((JobTelemetry) telemetry);
    }

    private async Task<JobResult> ProcessCommitLogChanges(
      IFeedRequest feedRequest,
      CommitLogBookmark currentBookmark,
      bool isForceMode,
      FeedChangeProcessingJobHandlerTelemetry telemetry)
    {
      Dictionary<string, (int, long)> opTelemetry = new Dictionary<string, (int, long)>();
      AggregationApplyTimings timings = new AggregationApplyTimings();
      Stopwatch stopwatch = new Stopwatch();
      CommitLogBookmark processedCommitBookmark = CommitLogBookmark.Empty;
      ICommitLogReader<CommitLogEntry> commitLogReader = this.commitLogService.Get();
      IReadOnlyList<IAggregationAccessor> accessors = (IReadOnlyList<IAggregationAccessor>) null;
      try
      {
        while (currentBookmark != CommitLogBookmark.Empty)
        {
          if (this.cancellationFacade.IsCancellationRequested())
          {
            telemetry.Message = string.Format("The requestContext got canceled. Stopping job execution and rescheduling the job. Feed id: {0}, Processed count: {1}", (object) feedRequest.Feed.Id, (object) telemetry.ProcessedChangeCount);
            return JobResult.StoppedAndMoreWorkToDo((JobTelemetry) telemetry, TimeSpan.Zero);
          }
          CommitLogEntry commitEntry = await commitLogReader.GetEntryAsync(feedRequest.Feed, currentBookmark.CommitId);
          if (!commitEntry.CorruptEntry)
            await ProcessOneCommitLogChange(commitEntry);
          processedCommitBookmark = commitEntry.GetCommitLogBookmark();
          currentBookmark = commitEntry.GetNextCommitLogBookmark();
          telemetry.CurrentCommitId = currentBookmark;
          if (telemetry.OpCount >= 1000)
            return JobResult.SuccessButMoreWorkToDo((JobTelemetry) telemetry, TimeSpan.Zero);
          commitEntry = (CommitLogEntry) null;
        }
      }
      finally
      {
        if (CommitLogBookmark.Empty != processedCommitBookmark)
          this.tokenProvider.StoreToken(feedRequest.Feed, processedCommitBookmark);
        telemetry.Details = new Dictionary<string, long>();
        foreach (KeyValuePair<string, long> aggNameToElapsedMs in timings.AggNameToElapsedMsMap)
          telemetry.Details[aggNameToElapsedMs.Key + ".ElapsedMs"] = aggNameToElapsedMs.Value;
        foreach (KeyValuePair<string, (int, long)> keyValuePair in opTelemetry)
        {
          telemetry.Details["Op." + keyValuePair.Key + ".Count"] = (long) keyValuePair.Value.Item1;
          telemetry.Details["Op." + keyValuePair.Key + ".ElapsedMs"] = keyValuePair.Value.Item2;
        }
      }
      return JobResult.Succeeded((JobTelemetry) telemetry);

      async Task ProcessOneCommitLogChange(CommitLogEntry commitEntry)
      {
        string opType;
        int opCountInCommit;
        if (commitEntry.CommitOperationData is IBatchCommitOperationData commitOperationData)
        {
          opType = commitOperationData.Operations.First<ICommitOperationData>().GetType().Name;
          opCountInCommit = commitOperationData.Operations.Count<ICommitOperationData>();
        }
        else
        {
          opType = commitEntry.CommitOperationData.GetType().Name;
          opCountInCommit = 1;
        }
        stopwatch.Restart();
        if (accessors == null)
          accessors = await this.aggregationAccessorFactory.GetAccessorsFor(feedRequest);
        try
        {
          AggregationApplyTimings aggregationApplyTimings = timings;
          IAggregationCommitApplier aggregationCommitApplier1 = this.aggregationCommitApplier;
          IReadOnlyList<IAggregationAccessor> aggregationAccessors = accessors;
          IFeedRequest feedRequest1 = feedRequest;
          CommitLogEntry[] commitLogEntries1 = new CommitLogEntry[1]
          {
            commitEntry
          };
          aggregationApplyTimings.MergeWith(await aggregationCommitApplier1.ApplyCommitAsync(aggregationAccessors, feedRequest1, (IReadOnlyList<ICommitLogEntry>) commitLogEntries1));
          aggregationApplyTimings = (AggregationApplyTimings) null;
          aggregationApplyTimings = timings;
          ICommitApplier aggregationCommitApplier2 = this.postAggregationCommitApplier;
          IFeedRequest feedRequest2 = feedRequest;
          CommitLogEntry[] commitLogEntries2 = new CommitLogEntry[1]
          {
            commitEntry
          };
          aggregationApplyTimings.MergeWith(await aggregationCommitApplier2.ApplyCommitAsync(feedRequest2, (IReadOnlyList<ICommitLogEntry>) commitLogEntries2));
          aggregationApplyTimings = (AggregationApplyTimings) null;
          TimeSpan timeSpan = DateTime.UtcNow - commitEntry.CreatedDate;
          telemetry.TotalCommitLatencyMs += (long) timeSpan.TotalMilliseconds;
          ++telemetry.ProcessedChangeCount;
          telemetry.OpCount += opCountInCommit;
        }
        catch (Exception ex)
        {
          MarkCommitAsCorruptRequest markCorrupt = new MarkCommitAsCorruptRequest(feedRequest.Feed, currentBookmark.CommitId, isForceMode, ex);
          if (await this.shouldMarkCommitAsCorruptHandler.Handle(markCorrupt))
          {
            NullResult nullResult = await this.markCommitAsCorruptHandler.Handle(markCorrupt);
          }
          else
            throw;
          markCorrupt = (MarkCommitAsCorruptRequest) null;
        }
        finally
        {
          (int, long) valueTuple;
          if (opTelemetry.TryGetValue(opType, out valueTuple))
            opTelemetry[opType] = (valueTuple.Item1 + opCountInCommit, valueTuple.Item2 + stopwatch.ElapsedMilliseconds);
          else
            opTelemetry.Add(opType, (opCountInCommit, stopwatch.ElapsedMilliseconds));
        }
        opType = (string) null;
      }
    }
  }
}
