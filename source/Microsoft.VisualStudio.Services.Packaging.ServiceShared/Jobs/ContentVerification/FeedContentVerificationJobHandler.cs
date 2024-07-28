// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ContentVerification.FeedContentVerificationJobHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ContentVerification;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.ContentVerification
{
  public class FeedContentVerificationJobHandler : 
    IAsyncHandler<IFeedRequest, JobResult>,
    IHaveInputType<IFeedRequest>,
    IHaveOutputType<JobResult>
  {
    private readonly ICommitEnumeratingStrategy commitEnumerator;
    private readonly IBookmarkTokenProvider<FeedCore, CommitLogBookmark> contentVerificationBookmarkTokenProvider;
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IContentVerificationScanner contentVerificationScanner;
    private readonly ITracerService tracerService;

    public FeedContentVerificationJobHandler(
      ICommitEnumeratingStrategy commitEnumerator,
      IBookmarkTokenProvider<FeedCore, CommitLogBookmark> contentVerificationBookmarkTokenProvider,
      IExecutionEnvironment executionEnvironment,
      IContentVerificationScanner contentVerificationScanner,
      ITracerService tracerService)
    {
      this.commitEnumerator = commitEnumerator;
      this.contentVerificationBookmarkTokenProvider = contentVerificationBookmarkTokenProvider;
      this.executionEnvironment = executionEnvironment;
      this.contentVerificationScanner = contentVerificationScanner;
      this.tracerService = tracerService;
    }

    public async Task<JobResult> Handle(IFeedRequest feedRequest)
    {
      FeedContentVerificationJobHandler sendInTheThisObject = this;
      JobResult jobResult;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (Handle)))
      {
        FeedCore feed = feedRequest.Feed;
        FeedContentVerificationJobTelemetry verificationJobTelemetry = new FeedContentVerificationJobTelemetry();
        verificationJobTelemetry.FeedId = feed.Id;
        FeedContentVerificationJobTelemetry telemetry = verificationJobTelemetry;
        try
        {
          CommitLogBookmark token = sendInTheThisObject.contentVerificationBookmarkTokenProvider.GetToken(feed);
          telemetry.InitialCommit = token;
          telemetry.BookmarkedCommitId = token.CommitId;
          ICommitLogEntry lastReadCommit = (ICommitLogEntry) null;
          CommitEntryBatch nextBatchAsync = await sendInTheThisObject.commitEnumerator.GetNextBatchAsync(feed, token.CommitId, PackagingCommitId.Empty);
          telemetry.ProcessedChangeCount = 0;
          CommitLogBookmark lastSuccessfullyProcessedBookmark = CommitLogBookmark.Empty;
          try
          {
            for (; nextBatchAsync.CommitLogEntries.Any<ICommitLogEntry>(); nextBatchAsync = await sendInTheThisObject.commitEnumerator.GetNextBatchAsync(feed, lastReadCommit.CommitId, PackagingCommitId.Empty))
            {
              foreach (ICommitLogEntry commit in (IEnumerable<ICommitLogEntry>) nextBatchAsync.CommitLogEntries)
              {
                telemetry.CurrentCommitId = commit.GetCommitLogBookmark();
                await sendInTheThisObject.ProcessOneCommitLogChange(feedRequest, commit, telemetry);
                lastSuccessfullyProcessedBookmark = commit.GetCommitLogBookmark();
                lastReadCommit = commit;
                ++telemetry.ProcessedChangeCount;
              }
              sendInTheThisObject.BookmarkLastProcessed(feed, lastSuccessfullyProcessedBookmark, telemetry);
            }
          }
          catch (Exception ex)
          {
            sendInTheThisObject.BookmarkLastProcessed(feed, lastSuccessfullyProcessedBookmark, telemetry);
            throw;
          }
          if (lastSuccessfullyProcessedBookmark != CommitLogBookmark.Empty)
            sendInTheThisObject.BookmarkLastProcessed(feed, lastSuccessfullyProcessedBookmark, telemetry);
          lastReadCommit = (ICommitLogEntry) null;
          lastSuccessfullyProcessedBookmark = new CommitLogBookmark();
        }
        catch (Exception ex)
        {
          FeedContentVerificationJobTelemetry telemetry1 = telemetry;
          throw new JobFailedException(ex, (JobTelemetry) telemetry1);
        }
        jobResult = JobResult.Succeeded((JobTelemetry) telemetry);
      }
      return jobResult;
    }

    private async Task ProcessOneCommitLogChange(
      IFeedRequest feedRequest,
      ICommitLogEntry commitEntry,
      FeedContentVerificationJobTelemetry telemetry)
    {
      if (commitEntry.CorruptEntry)
        return;
      await this.contentVerificationScanner.ScanCommitAsync((CollectionId) this.executionEnvironment.HostId, feedRequest.Feed, (IReadOnlyList<ICommitLogEntry>) new ICommitLogEntry[1]
      {
        commitEntry
      });
    }

    private void BookmarkLastProcessed(
      FeedCore feed,
      CommitLogBookmark lastProcessedBookmark,
      FeedContentVerificationJobTelemetry telemetry)
    {
      using (this.tracerService.Enter((object) this, nameof (BookmarkLastProcessed)))
      {
        if (lastProcessedBookmark == CommitLogBookmark.Empty)
          return;
        CommitLogBookmark token = new CommitLogBookmark(lastProcessedBookmark.CommitId, lastProcessedBookmark.SequenceNumber);
        this.contentVerificationBookmarkTokenProvider.StoreToken(feed, token);
        telemetry.BookmarkedCommitId = lastProcessedBookmark.CommitId;
      }
    }
  }
}
