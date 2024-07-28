// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeleteProcessingJob.FeedLevelDeletedPackageJobHandler`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DeleteProcessingJob
{
  public class FeedLevelDeletedPackageJobHandler<TPackageIdentity> : 
    IAsyncHandler<IFeedRequest, JobResult>,
    IHaveInputType<IFeedRequest>,
    IHaveOutputType<JobResult>
    where TPackageIdentity : class, IPackageIdentity
  {
    private readonly ITimeProvider timeProvider;
    private readonly IBookmarkTokenProvider<FeedCore, CommitLogBookmark> deleteProcessingBookmarkTokenProvider;
    private readonly IBookmarkTokenProvider<FeedCore, CommitLogBookmark> changeProcessingBookmarkProvider;
    private readonly IUnflattenedCommitEnumeratingStrategy commitEnumeratingStrategy;
    private readonly IAsyncHandler<PackageRequest<TPackageIdentity>, IPermanentDeleteOperationData> permanentOpDataGeneratingHandler;
    private readonly IAsyncHandler<PackageRequest<TPackageIdentity>, PackageDeletionState> deletionStateFetchingHandler;
    private readonly ICommitLogWriter<ICommitLogEntry> commitLogWriter;
    private readonly ITracerService tracerService;
    private readonly IAsyncHandler<IDeleteOperationData, DateTime> scheduledPermanentDeleteDateResolvingHandler;
    private readonly IFeedJobQueuer changeProcessingJobQueuer;
    private readonly IFeedJobQueuer dpjQueuer;
    private readonly ICancellationFacade cancellationFacade;
    private readonly IFactory<TimeSpan> maxRunTimeFactory;
    private readonly IFactory<TimeSpan> flushIntervalFactory;
    private readonly IFactory<int> maxPendingPermDeleteOpsFactory;
    private readonly IFactory<int> maxPermDeleteBatchSizeFactory;

    public FeedLevelDeletedPackageJobHandler(
      ITimeProvider timeProvider,
      IBookmarkTokenProvider<FeedCore, CommitLogBookmark> deleteProcessingBookmarkTokenProvider,
      IBookmarkTokenProvider<FeedCore, CommitLogBookmark> changeProcessingBookmarkProvider,
      IUnflattenedCommitEnumeratingStrategy commitEnumeratingStrategy,
      IAsyncHandler<PackageRequest<TPackageIdentity>, IPermanentDeleteOperationData> permanentOpDataGeneratingHandler,
      IAsyncHandler<PackageRequest<TPackageIdentity>, PackageDeletionState> deletionStateFetchingHandler,
      ICommitLogWriter<ICommitLogEntry> commitLogWriter,
      ITracerService tracerService,
      IAsyncHandler<IDeleteOperationData, DateTime> scheduledPermanentDeleteDateResolvingHandler,
      IFeedJobQueuer changeProcessingJobQueuer,
      IFeedJobQueuer dpjQueuer,
      ICancellationFacade cancellationFacade,
      IFactory<TimeSpan> maxRunTimeFactory,
      IFactory<TimeSpan> flushIntervalFactory,
      IFactory<int> maxPendingPermDeleteOpsFactory,
      IFactory<int> maxPermDeleteBatchSizeFactory)
    {
      this.timeProvider = timeProvider;
      this.deleteProcessingBookmarkTokenProvider = deleteProcessingBookmarkTokenProvider;
      this.changeProcessingBookmarkProvider = changeProcessingBookmarkProvider;
      this.commitEnumeratingStrategy = commitEnumeratingStrategy;
      this.permanentOpDataGeneratingHandler = permanentOpDataGeneratingHandler;
      this.deletionStateFetchingHandler = deletionStateFetchingHandler;
      this.commitLogWriter = commitLogWriter;
      this.tracerService = tracerService;
      this.scheduledPermanentDeleteDateResolvingHandler = scheduledPermanentDeleteDateResolvingHandler;
      this.changeProcessingJobQueuer = changeProcessingJobQueuer;
      this.dpjQueuer = dpjQueuer;
      this.cancellationFacade = cancellationFacade;
      this.maxRunTimeFactory = maxRunTimeFactory;
      this.flushIntervalFactory = flushIntervalFactory;
      this.maxPendingPermDeleteOpsFactory = maxPendingPermDeleteOpsFactory;
      this.maxPermDeleteBatchSizeFactory = maxPermDeleteBatchSizeFactory;
    }

    public async Task<JobResult> Handle(IFeedRequest feedRequest)
    {
      FeedLevelDeletedPackageJobHandler<TPackageIdentity> sendInTheThisObject = this;
      FeedCore feed = feedRequest.Feed;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (Handle)))
      {
        FeedLevelDeletedPackageJobTelemetry packageJobTelemetry = new FeedLevelDeletedPackageJobTelemetry();
        packageJobTelemetry.FeedId = feed.Id;
        FeedLevelDeletedPackageJobTelemetry telemetry = packageJobTelemetry;
        try
        {
          if (await sendInTheThisObject.IsChangeProcessingBookmarkTooFarOutOfDate(feed, sendInTheThisObject.changeProcessingBookmarkProvider.GetToken(feed), telemetry))
          {
            await sendInTheThisObject.QueueChangeProcessingJob(feedRequest);
            await sendInTheThisObject.DelayQueueCurrentJob(feedRequest, FeedLevelDeletedPackageJobConstants.RequeueDelayIfCpjOutOfDate);
            return JobResult.Succeeded((JobTelemetry) telemetry);
          }
          CommitLogBookmark lastProcessedCommitBookmark = sendInTheThisObject.deleteProcessingBookmarkTokenProvider.GetToken(feed);
          telemetry.InitialBookmark = lastProcessedCommitBookmark;
          TimeSpan maxRunTime = sendInTheThisObject.maxRunTimeFactory.Get();
          TimeSpan flushInterval = sendInTheThisObject.flushIntervalFactory.Get();
          int maxPendingOps = sendInTheThisObject.maxPendingPermDeleteOpsFactory.Get();
          DateTime startTime = sendInTheThisObject.timeProvider.Now;
          DateTime lastFlushTime = startTime;
          List<IPermanentDeleteOperationData> permDeletesToCommit = new List<IPermanentDeleteOperationData>();
          IConcurrentIterator<ICommitLogEntry> commits = await sendInTheThisObject.commitEnumeratingStrategy.EnumerateCommitsAsync(feed, lastProcessedCommitBookmark.CommitId, PackagingCommitId.Empty);
          try
          {
            ICommitLogEntry outerCommit;
            DateTime dateTime;
            TimeSpan timeSpan;
            while (true)
            {
              if (await commits.MoveNextAsync(CancellationToken.None))
              {
                outerCommit = commits.Current;
                List<IPermanentDeleteOperationData> newPermDeletesToCommit = new List<IPermanentDeleteOperationData>();
                IReadOnlyList<ICommitLogEntry> subCommits = AggregationAccessorCommonUtils.FlattenCommitLogEntry(outerCommit);
                if (subCommits.Any<ICommitLogEntry>())
                {
                  bool flag;
                  (flag, dateTime) = await sendInTheThisObject.ShouldStopProcessing(subCommits.First<ICommitLogEntry>());
                  if (!flag)
                  {
                    foreach (ICommitLogEntry commit in (IEnumerable<ICommitLogEntry>) subCommits)
                    {
                      IPermanentDeleteOperationData deleteOperation = await sendInTheThisObject.GetDeleteOperation(feedRequest, commit);
                      if (deleteOperation != null)
                        newPermDeletesToCommit.Add(deleteOperation);
                    }
                  }
                  else
                    break;
                }
                if (sendInTheThisObject.timeProvider.Now - lastFlushTime > flushInterval || permDeletesToCommit.Count + newPermDeletesToCommit.Count > maxPendingOps)
                  await Flush();
                permDeletesToCommit.AddRange((IEnumerable<IPermanentDeleteOperationData>) newPermDeletesToCommit);
                lastProcessedCommitBookmark = outerCommit.GetCommitLogBookmark();
                if (!sendInTheThisObject.cancellationFacade.IsCancellationRequested())
                {
                  timeSpan = sendInTheThisObject.timeProvider.Now - startTime;
                  if (!(timeSpan > maxRunTime))
                  {
                    outerCommit = (ICommitLogEntry) null;
                    newPermDeletesToCommit = (List<IPermanentDeleteOperationData>) null;
                    subCommits = (IReadOnlyList<ICommitLogEntry>) null;
                  }
                  else
                    goto label_27;
                }
                else
                  goto label_25;
              }
              else
                goto label_32;
            }
            telemetry.ExitReason = DeleteProcessingJobHandlerExitReason.ReachedCommitInGracePeriod;
            telemetry.Message = string.Format("Reached commit in grace period. Id: {0}", (object) outerCommit.CommitId);
            telemetry.NextKnownScheduledPermanentDeleteTime = new DateTime?(dateTime);
            goto label_32;
label_25:
            telemetry.Message = string.Format("The requestContext got canceled. Stopping processing the batch and rescheduling the job. Feed id: {0}, last processed commit: {1}", (object) feed.Id, (object) lastProcessedCommitBookmark.CommitId);
            telemetry.ExitReason = DeleteProcessingJobHandlerExitReason.RequestContextCanceled;
            goto label_32;
label_27:
            telemetry.Message = string.Format("Reached max allowed run time ({0}). Elapsed time: {1}", (object) maxRunTime, (object) timeSpan);
            telemetry.ExitReason = DeleteProcessingJobHandlerExitReason.ReachedMaxRunTime;
          }
          finally
          {
label_32:
            await Flush();
          }
          if (telemetry.ExitReason == DeleteProcessingJobHandlerExitReason.Unknown)
          {
            telemetry.Message = "Reached end of commit log.";
            telemetry.ExitReason = DeleteProcessingJobHandlerExitReason.ReachedEndOfCommitLog;
          }
          switch (telemetry.ExitReason)
          {
            case DeleteProcessingJobHandlerExitReason.ReachedMaxRunTime:
              return JobResult.SuccessButMoreWorkToDo((JobTelemetry) telemetry, FeedLevelDeletedPackageJobConstants.RequeueDelayIfStoppedEarly);
            case DeleteProcessingJobHandlerExitReason.ReachedCommitInGracePeriod:
              DateTime? permanentDeleteTime = telemetry.NextKnownScheduledPermanentDeleteTime;
              if (permanentDeleteTime.HasValue)
              {
                permanentDeleteTime = telemetry.NextKnownScheduledPermanentDeleteTime;
                TimeSpan delay = permanentDeleteTime.Value + FeedLevelDeletedPackageJobConstants.RequeueDelayAfterNextScheduledPermanentDelete - sendInTheThisObject.timeProvider.Now;
                if (delay < FeedLevelDeletedPackageJobConstants.MinRequeueDelayIfFutureDeleteReached)
                  delay = FeedLevelDeletedPackageJobConstants.MinRequeueDelayIfFutureDeleteReached;
                await sendInTheThisObject.DelayQueueCurrentJob(feedRequest, delay);
              }
              return JobResult.Succeeded((JobTelemetry) telemetry);
            case DeleteProcessingJobHandlerExitReason.ReachedEndOfCommitLog:
              return JobResult.Succeeded((JobTelemetry) telemetry);
            case DeleteProcessingJobHandlerExitReason.RequestContextCanceled:
              return JobResult.StoppedAndMoreWorkToDo((JobTelemetry) telemetry, FeedLevelDeletedPackageJobConstants.RequeueDelayIfStoppedEarly);
            default:
              throw new Exception(string.Format("Unexpected exit reason at end of DPJ loop: {0}", (object) telemetry.ExitReason));
          }
          // ISSUE: variable of a compiler-generated type
          FeedLevelDeletedPackageJobHandler<TPackageIdentity>.\u003C\u003Ec__DisplayClass17_0 cDisplayClass170;

          async Task Flush()
          {
            // ISSUE: reference to a compiler-generated field
            CommitLogBookmark commitLog = await cDisplayClass170.\u003C\u003E4__this.AppendCommitsToCommitLog(feedRequest.Feed, (IReadOnlyCollection<IPermanentDeleteOperationData>) permDeletesToCommit, telemetry);
            // ISSUE: reference to a compiler-generated field
            cDisplayClass170.\u003C\u003E4__this.StoreLastProcessedBookmark(feedRequest.Feed, lastProcessedCommitBookmark, telemetry);
            CommitLogBookmark empty = CommitLogBookmark.Empty;
            if (commitLog != empty)
            {
              // ISSUE: reference to a compiler-generated field
              await cDisplayClass170.\u003C\u003E4__this.QueueChangeProcessingJob(feedRequest);
            }
            permDeletesToCommit.Clear();
            // ISSUE: reference to a compiler-generated field
            lastFlushTime = cDisplayClass170.\u003C\u003E4__this.timeProvider.Now;
          }
        }
        catch (Exception ex)
        {
          FeedLevelDeletedPackageJobTelemetry telemetry1 = telemetry;
          throw new JobFailedException(ex, (JobTelemetry) telemetry1);
        }
      }
    }

    private async Task<CommitLogBookmark> AppendCommitsToCommitLog(
      FeedCore feed,
      IReadOnlyCollection<IPermanentDeleteOperationData> permDeletesToCommit,
      FeedLevelDeletedPackageJobTelemetry telemetry)
    {
      if (!permDeletesToCommit.Any<IPermanentDeleteOperationData>())
        return CommitLogBookmark.Empty;
      int configuredOpsPerBatchCommit = this.maxPermDeleteBatchSizeFactory.Get();
      int initialOpsPerBatchCommit = Math.Min(permDeletesToCommit.Count + 1, configuredOpsPerBatchCommit);
      int opsPerBatchCommit = initialOpsPerBatchCommit;
      while (true)
      {
        try
        {
          List<BatchCommitOperationData> list = permDeletesToCommit.GetPages<IPermanentDeleteOperationData>(opsPerBatchCommit).Select<List<IPermanentDeleteOperationData>, BatchCommitOperationData>((Func<List<IPermanentDeleteOperationData>, BatchCommitOperationData>) (page => new BatchCommitOperationData((IReadOnlyCollection<ICommitOperationData>) page))).ToList<BatchCommitOperationData>();
          if (list.Count > 98)
            throw new TooManyBatchesAtOnceException(string.Join("\n", new string[3]
            {
              string.Format("Cannot commit {0} batch ops at the same time. Maximum is {1}.", (object) list.Count, (object) 98),
              string.Format("There are {0} permanent delete ops to commit.", (object) permDeletesToCommit.Count),
              string.Format("Ops per batch commit: {0} (initially {1}, configured {2})", (object) opsPerBatchCommit, (object) initialOpsPerBatchCommit, (object) configuredOpsPerBatchCommit)
            }));
          IReadOnlyCollection<ICommitLogEntry> source = await this.commitLogWriter.AppendEntriesAsync(feed, (IReadOnlyCollection<ICommitOperationData>) list);
          CommitLogBookmark commitLogBookmark = source.Last<ICommitLogEntry>().GetCommitLogBookmark();
          telemetry.LastCommitAddedBookmark = commitLogBookmark;
          telemetry.OperationsAdded += permDeletesToCommit.Count;
          telemetry.CommitsAdded += source.Count;
          return commitLogBookmark;
        }
        catch (ItemTooBigException ex)
        {
          if (opsPerBatchCommit <= 1)
            throw new SingleOpItemTooBigException("The operation data for a single permanent delete could not fit in an ItemStore item", (Exception) ex);
          opsPerBatchCommit /= 2;
        }
      }
    }

    private async Task QueueChangeProcessingJob(IFeedRequest feedRequest)
    {
      FeedLevelDeletedPackageJobHandler<TPackageIdentity> sendInTheThisObject = this;
      FeedCore feed = feedRequest.Feed;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (QueueChangeProcessingJob)))
      {
        Guid guid = await sendInTheThisObject.changeProcessingJobQueuer.QueueJob(feed, feedRequest.Protocol, JobPriorityLevel.Idle);
        tracer.TraceInfo(string.Format("Queued change processing job for feed {0} and protocol {1}.", (object) feed.Id, (object) feedRequest.Protocol));
      }
      feed = (FeedCore) null;
    }

    private async Task DelayQueueCurrentJob(IFeedRequest feedRequest, TimeSpan delay)
    {
      FeedLevelDeletedPackageJobHandler<TPackageIdentity> sendInTheThisObject = this;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (DelayQueueCurrentJob)))
      {
        Guid guid = await sendInTheThisObject.dpjQueuer.QueueJob(feedRequest.Feed, feedRequest.Protocol, JobPriorityLevel.Idle, (int) delay.TotalSeconds);
        tracer.TraceInfo(string.Format("Queued DPJ for feed {0} and protocol {1} with delay {2}.", (object) feedRequest.Feed.Id, (object) feedRequest.Protocol, (object) delay));
      }
    }

    private async Task<bool> IsChangeProcessingBookmarkTooFarOutOfDate(
      FeedCore feed,
      CommitLogBookmark changeProcessingBookmark,
      FeedLevelDeletedPackageJobTelemetry telemetry)
    {
      ICommitLogEntry newestCommitAsync = await this.commitEnumeratingStrategy.GetNewestCommitAsync(feed);
      if (newestCommitAsync == null)
        return false;
      if (changeProcessingBookmark == CommitLogBookmark.Empty)
      {
        telemetry.Message = "Change processing bookmark is empty yet newest commit exists so change processing job not up to date.";
        telemetry.ExitReason = DeleteProcessingJobHandlerExitReason.NoChangeProcessingBookmark;
        return true;
      }
      if (newestCommitAsync.CommitId == changeProcessingBookmark.CommitId)
        return false;
      if (!((await this.commitEnumeratingStrategy.GetCommitAsync(feed, changeProcessingBookmark)).CreatedDate + FeedLevelDeletedPackageJobConstants.LimboPeriod < this.timeProvider.Now))
        return false;
      telemetry.Message = "Change processing bookmark commit is older than the limbo period from now.";
      telemetry.ExitReason = DeleteProcessingJobHandlerExitReason.ChangeProcessingBookmarkOlderThanLimboPeriod;
      return true;
    }

    private void StoreLastProcessedBookmark(
      FeedCore feed,
      CommitLogBookmark lastProcessedBookmark,
      FeedLevelDeletedPackageJobTelemetry telemetry)
    {
      using (this.tracerService.Enter((object) this, nameof (StoreLastProcessedBookmark)))
      {
        if (CommitLogBookmark.Empty == lastProcessedBookmark)
          return;
        this.deleteProcessingBookmarkTokenProvider.StoreToken(feed, lastProcessedBookmark);
        telemetry.LastProcessedBookmark = lastProcessedBookmark;
      }
    }

    private async Task<IPermanentDeleteOperationData> GetDeleteOperation(
      IFeedRequest feedRequest,
      ICommitLogEntry commit)
    {
      FeedLevelDeletedPackageJobHandler<TPackageIdentity> sendInTheThisObject = this;
      if (!(commit.CommitOperationData is IDeleteOperationData deleteOperationData))
        return (IPermanentDeleteOperationData) null;
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetDeleteOperation)))
      {
        if (!(deleteOperationData.Identity is TPackageIdentity identity))
          throw new Exception(string.Format("Cannot process commit {0} since its identity is not the expected type: {1}", (object) commit.CommitId, (object) typeof (TPackageIdentity)));
        PackageRequest<TPackageIdentity> packageRequest = new PackageRequest<TPackageIdentity>(feedRequest, identity);
        CommitLogBookmark currentDeletingCommit = new CommitLogBookmark(commit.CommitId, new long?(commit.SequenceNumber));
        PackageDeletionState packageDeletionState = await sendInTheThisObject.deletionStateFetchingHandler.Handle(packageRequest);
        if (packageDeletionState.PermanentDeletedDate.HasValue)
        {
          tracer.TraceInfo(string.Format("Skipped permanent delete for commit {0} due to having been permanently deleted.", (object) currentDeletingCommit));
          return (IPermanentDeleteOperationData) null;
        }
        if (!packageDeletionState.DeletedDate.HasValue)
        {
          tracer.TraceInfo(string.Format("Skipped permanent delete for commit {0} due to this package having already been restored.", (object) currentDeletingCommit));
          return (IPermanentDeleteOperationData) null;
        }
        if (packageDeletionState.ScheduledPermanentDeleteDate.HasValue)
        {
          DateTime? permanentDeleteDate1 = packageDeletionState.ScheduledPermanentDeleteDate;
          DateTime? permanentDeleteDate2 = deleteOperationData.ScheduledPermanentDeleteDate;
          if ((permanentDeleteDate1.HasValue & permanentDeleteDate2.HasValue ? (permanentDeleteDate1.GetValueOrDefault() > permanentDeleteDate2.GetValueOrDefault() ? 1 : 0) : 0) != 0)
          {
            tracer.TraceInfo(string.Format("Skipped permanent delete for commit {0} due to having been re-restored and re-deleted.", (object) currentDeletingCommit));
            return (IPermanentDeleteOperationData) null;
          }
        }
        IPermanentDeleteOperationData deleteOperation = await sendInTheThisObject.permanentOpDataGeneratingHandler.Handle(packageRequest);
        if (deleteOperation != null)
          return deleteOperation;
        tracer.TraceError(string.Format("Skipped permanent delete for commit {0} because it no longer has metadata (i.e. was permanently deleted). This should only occur in a fairly rare race.", (object) currentDeletingCommit));
        return (IPermanentDeleteOperationData) null;
      }
    }

    private async Task<(bool ShouldStop, DateTime nextKnownScheduledPermanentDeleteTime)> ShouldStopProcessing(
      ICommitLogEntry commit)
    {
      if (!(commit.CommitOperationData is IDeleteOperationData commitOperationData))
        return (false, new DateTime());
      DateTime dateTime = await this.scheduledPermanentDeleteDateResolvingHandler.Handle(commitOperationData);
      return (this.timeProvider.Now < dateTime + FeedLevelDeletedPackageJobConstants.LimboPeriod, dateTime);
    }
  }
}
