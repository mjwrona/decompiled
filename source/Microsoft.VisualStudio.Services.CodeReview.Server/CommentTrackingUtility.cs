// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CommentTrackingUtility
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.CodeReview.Server.Common;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal static class CommentTrackingUtility
  {
    internal static string c_Layer = "CodeReviewCommentTrackingUtility";

    internal static void TrackCommentThreads(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      List<CommentThread> threadList,
      CommentTrackingCriteria trackingCriteria)
    {
      if (threadList.Count == 0 || trackingCriteria.FirstComparingIteration < 0 || trackingCriteria.SecondComparingIteration <= 0 || trackingCriteria.FirstComparingIteration > trackingCriteria.SecondComparingIteration)
        return;
      if (trackingCriteria.FirstComparingIteration == 0)
        trackingCriteria.FirstComparingIteration = trackingCriteria.SecondComparingIteration;
      Guid correlationId = Guid.NewGuid();
      ClientTraceData ctData = new ClientTraceData();
      Stopwatch stopwatch = Stopwatch.StartNew();
      ctData.Add("CommentTrackingProjectId", (object) projectId);
      ctData.Add("CommentTrackingReviewId", (object) reviewId);
      ctData.Add("CommentTrackingIsDiffCacheJob", (object) false);
      ctData.Add("CommentTrackingCorrelationId", (object) correlationId);
      ctData.Add("CommentTrackingFirstComparingIteration", (object) trackingCriteria.FirstComparingIteration);
      ctData.Add("CommentTrackingSecondComparingIteration", (object) trackingCriteria.SecondComparingIteration);
      TrackingManager trackingManager = new TrackingManager(trackingCriteria);
      List<CommentThread> threadsFromThreadsList = CommentTrackingUtility.GetFileThreadsFromThreadsList(threadList);
      ctData.Add("CommentTrackingApplicableThreadIds", (object) threadsFromThreadsList.Select<CommentThread, int>((Func<CommentThread, int>) (t => t.DiscussionId)).ToArray<int>());
      if (threadsFromThreadsList.Count<CommentThread>() > 0)
      {
        trackingManager.AddTrackingPairs(threadsFromThreadsList);
        trackingManager.AddTrackingPairs(threadsFromThreadsList, TrackingTarget.Secondary);
        TelemetryHelper.ExecuteAndMeasure(ctData, "CommentTrackingTimeToGetChanges", (Action) (() => CommentTrackingUtility.PopulateMetadata(requestContext, projectId, reviewId, trackingManager, ctData)));
        trackingManager.CleanUpTrackingPairs(threadsFromThreadsList, ctData);
      }
      if (trackingManager.GetHashes().Count<string>() >= 2)
      {
        ICodeReviewDiffCacheService diffCacheService = requestContext.GetService<ICodeReviewDiffCacheService>();
        TelemetryHelper.ExecuteAndMeasure(ctData, "CommentTrackingTimeToReadCache", (Action) (() => trackingManager.PopulateDiffDataFromCache(diffCacheService.GetCachedDiffData(requestContext, projectId, reviewId), ctData)));
        if (!trackingManager.IsAllDataCached(ctData))
        {
          CachedRegistryService service = requestContext.GetService<CachedRegistryService>();
          int fileLimit = service.GetValue<int>(requestContext, (RegistryQuery) "/CodeReview/CommentTracking/FileLimit", true, 20);
          int fileSizeLimit = service.GetValue<int>(requestContext, (RegistryQuery) "/CodeReview/CommentTracking/FileSizeLimit", true, 6000000);
          CommentTrackingUtility.PopulateContentAndDiffs(requestContext, projectId, reviewId, trackingManager, fileLimit, fileSizeLimit, ctData);
          TelemetryHelper.ExecuteAndMeasure(ctData, "CommentTrackingTimeToWriteCache", (Action) (() => diffCacheService.CacheDiffData(requestContext, projectId, reviewId, trackingManager.GetDiffDataAsCache(), ctData)));
        }
      }
      int num = 0;
      foreach (CommentThread thread in threadsFromThreadsList)
      {
        bool flag = CommentTrackingUtility.TrackThread(requestContext, trackingManager, thread, correlationId);
        num += flag ? 1 : 0;
      }
      ctData.Add("CommentTrackingNumThreadsTracked", (object) num);
      ctData.Add("CommentTrackingTotalTimeMs", (object) stopwatch.ElapsedMilliseconds);
      CommentTrackingUtility.TryPublishCtData(requestContext, ctData);
    }

    internal static DiffChangeType? TrackPosition(
      List<IDiffChange> diffData,
      ref int line,
      ref int offset,
      bool trackBackward = false,
      bool isSpanStart = true)
    {
      bool flag1 = false;
      DiffChangeType? nullable = new DiffChangeType?();
      if (diffData.IsNullOrEmpty<IDiffChange>())
        return new DiffChangeType?();
      int num1 = 0;
      int index1 = diffData.Count - 1;
      int sourceChangeStart;
      int sourceChangeLength;
      int sourceChangeEnd;
      int targetChangeStart;
      int targetChangeLength;
      int targetChangeEnd;
      while (num1 <= index1)
      {
        int index2 = (num1 + index1) / 2;
        IDiffChange change = diffData[index2];
        CommentTrackingUtility.GetSourceTargetFromChanges(change, trackBackward, out sourceChangeStart, out sourceChangeLength, out sourceChangeEnd, out targetChangeStart, out targetChangeLength, out targetChangeEnd);
        bool flag2 = change.ChangeType == DiffChangeType.Change;
        int num2 = trackBackward || change.ChangeType != DiffChangeType.Insert ? (!trackBackward ? 0 : (change.ChangeType == DiffChangeType.Delete ? 1 : 0)) : 1;
        bool flag3 = !trackBackward && change.ChangeType == DiffChangeType.Delete || trackBackward && change.ChangeType == DiffChangeType.Insert;
        int num3 = (uint) num2 > 0U ? 1 : 0;
        if (line < sourceChangeStart + num3)
          index1 = index2 - 1;
        else if (line > sourceChangeEnd)
        {
          num1 = index2 + 1;
        }
        else
        {
          if (flag2)
          {
            nullable = new DiffChangeType?(DiffChangeType.Change);
            line = Math.Min(targetChangeStart + (line - sourceChangeStart), targetChangeEnd);
            if (!isSpanStart && line == sourceChangeEnd)
              line = targetChangeEnd;
          }
          else
          {
            nullable = new DiffChangeType?(flag3 ? DiffChangeType.Delete : DiffChangeType.Insert);
            line = isSpanStart ? targetChangeStart : targetChangeEnd;
          }
          flag1 = true;
          break;
        }
      }
      if (index1 < num1 && num1 > 0)
      {
        CommentTrackingUtility.GetSourceTargetFromChanges(diffData[index1], trackBackward, out sourceChangeStart, out sourceChangeLength, out sourceChangeEnd, out targetChangeStart, out targetChangeLength, out targetChangeEnd);
        int num4 = targetChangeEnd - sourceChangeEnd;
        flag1 = num4 != 0;
        line += num4;
      }
      if (flag1)
        offset = isSpanStart || line <= 0 ? 0 : 2147483646;
      return nullable;
    }

    internal static void GetSourceTargetFromChanges(
      IDiffChange change,
      bool trackBackward,
      out int sourceChangeStart,
      out int sourceChangeLength,
      out int sourceChangeEnd,
      out int targetChangeStart,
      out int targetChangeLength,
      out int targetChangeEnd)
    {
      bool flag1 = !trackBackward && change.ChangeType == DiffChangeType.Insert || trackBackward && change.ChangeType == DiffChangeType.Delete;
      bool flag2 = change.ChangeType == DiffChangeType.Change;
      if (trackBackward)
      {
        sourceChangeStart = change.ModifiedStart;
        sourceChangeLength = change.ModifiedLength;
        targetChangeStart = change.OriginalStart;
        targetChangeLength = change.OriginalLength;
      }
      else
      {
        sourceChangeStart = change.OriginalStart;
        sourceChangeLength = change.OriginalLength;
        targetChangeStart = change.ModifiedStart;
        targetChangeLength = change.ModifiedLength;
      }
      sourceChangeEnd = sourceChangeStart + sourceChangeLength;
      targetChangeEnd = targetChangeStart + targetChangeLength;
      sourceChangeStart = flag2 || !flag1 ? sourceChangeStart + 1 : sourceChangeStart;
      targetChangeStart = flag2 | flag1 ? targetChangeStart + 1 : targetChangeStart;
    }

    internal static List<CommentThread> GetFileThreadsFromThreadsList(List<CommentThread> threads)
    {
      List<CommentThread> applicableThreads = new List<CommentThread>();
      if (!threads.IsNullOrEmpty<CommentThread>())
        threads.ForEach((Action<CommentThread>) (thread =>
        {
          int num1 = thread.IsReviewLevel ? 0 : (!thread.IsDeleted ? 1 : 0);
          bool flag1 = thread.ThreadContext != null && thread.ThreadContext.IterationContext != null && thread.ThreadContext.ChangeTrackingId != 0;
          bool flag2 = thread.ThreadContext?.FilePath != null;
          bool flag3 = thread.ThreadContext != null && (thread.ThreadContext.LeftFileStart != null && thread.ThreadContext.LeftFileEnd != null || thread.ThreadContext.RightFileStart != null && thread.ThreadContext.RightFileEnd != null);
          int num2 = flag1 ? 1 : 0;
          if ((num1 & num2 & (flag2 ? 1 : 0) & (flag3 ? 1 : 0)) == 0)
            return;
          applicableThreads.Add(thread);
        }));
      return applicableThreads;
    }

    internal static void PopulateMetadata(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      TrackingManager trackingManager,
      ClientTraceData ctData = null)
    {
      if (trackingManager.GetSnapshotPairs().Count<TrackingFileSnapshotPair>() < 1)
      {
        trackingManager.Clear();
      }
      else
      {
        List<int> iterations = trackingManager.GetIterations();
        ctData?.Add("CommentTrackingIterationMetaDataRequested", (object) iterations);
        List<ChangeEntry> list = requestContext.GetService<ICodeReviewIterationService>().GetChangeList(requestContext, projectId, reviewId, iterations).ToList<ChangeEntry>();
        trackingManager.AddChangeEntries(list);
      }
    }

    internal static bool PopulateContentAndDiffs(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      TrackingManager trackingManager,
      int fileLimit = 20,
      int fileSizeLimit = 6000000,
      ClientTraceData ctData = null)
    {
      if (trackingManager.GetHashes(true).Count < 2)
      {
        trackingManager.Clear();
        return true;
      }
      ICodeReviewService reviewService = requestContext.GetService<ICodeReviewService>();
      List<string> uncachedContentHashes = trackingManager.GetHashes(true);
      ctData?.Add("CommentTrackingUncachedFiles", (object) uncachedContentHashes);
      Dictionary<string, ChangeEntryStream> contentHashToStream = (Dictionary<string, ChangeEntryStream>) null;
      TelemetryHelper.ExecuteAndMeasure(ctData, "CommentTrackingTimeToGetStreamsForEncodingMs", (Action) (() => contentHashToStream = reviewService.DownloadFilesStreams(requestContext, projectId, reviewId, uncachedContentHashes)));
      bool flag = uncachedContentHashes.Count > fileLimit;
      if (flag)
        uncachedContentHashes = uncachedContentHashes.Take<string>(fileLimit).ToList<string>();
      ctData?.Add("CommentTrackingFileLimitReached", (object) flag);
      TelemetryHelper.ExecuteAndMeasure(ctData, "CommentTrackingTimeToDetermineEncodingMs", (Action) (() => trackingManager.SetFileStreams(contentHashToStream, true, (long) fileSizeLimit)));
      TelemetryHelper.ExecuteAndMeasure(ctData, "CommentTrackingTimeToGetStreamsForContentMs", (Action) (() => contentHashToStream = reviewService.DownloadFilesStreams(requestContext, projectId, reviewId, uncachedContentHashes)));
      TelemetryHelper.ExecuteAndMeasure(ctData, "CommentTrackingTimeToReadStreamsMs", (Action) (() => trackingManager.SetFileStreams(contentHashToStream, fileSizeLimit: (long) fileSizeLimit)));
      TelemetryHelper.ExecuteAndMeasure(ctData, "CommentTrackingTimeToDiffFilesMs", (Action) (() =>
      {
        int fileDiffs = trackingManager.TakeFileDiffs();
        ctData?.Add("CommentTrackingNumDiffsTaken", (object) fileDiffs);
      }));
      return !flag;
    }

    internal static bool TrackThread(TrackingManager trackingManager, CommentThread thread) => CommentTrackingUtility.TrackThread((IVssRequestContext) null, trackingManager, thread, Guid.Empty);

    internal static bool TrackThread(
      IVssRequestContext requestContext,
      TrackingManager trackingManager,
      CommentThread thread,
      Guid correlationId)
    {
      ClientTraceData ctData = new ClientTraceData();
      ctData.Add("TrackThreadCorrelationId", (object) correlationId);
      ctData.Add("TrackThreadProjectId", (object) thread.ProjectId);
      ctData.Add("TrackThreadCodeReviewId", (object) thread.ReviewId);
      ctData.Add("TrackThreadId", (object) thread.DiscussionId);
      ctData.Add("TrackThreadChangeTrackingId", (object) thread.ThreadContext.ChangeTrackingId);
      CommentThreadContext threadContext = thread.ThreadContext;
      CommentTrackingCriteria trackingCriteria = new CommentTrackingCriteria()
      {
        FirstComparingIteration = trackingManager.TrackingCriteria.FirstComparingIteration,
        SecondComparingIteration = trackingManager.TrackingCriteria.SecondComparingIteration
      };
      TrackingContext contextForThread1 = trackingManager.GetTrackingContextForThread(thread);
      TrackingContext contextForThread2 = trackingManager.GetTrackingContextForThread(thread, TrackingTarget.Secondary);
      TrackingContext trackingContext1 = contextForThread1.TargetSide == TrackingSide.Right ? contextForThread1 : contextForThread2;
      TrackingContext trackingContext2 = contextForThread1;
      ctData.Add("TrackPrimaryContext", contextForThread1.FormatCI());
      ctData.Add("TrackSecondaryContext", contextForThread2.FormatCI());
      List<IDiffChange> diffChangeList = (List<IDiffChange>) null;
      List<IDiffChange> diffData1 = trackingManager.GetDiffData(contextForThread1);
      List<IDiffChange> diffData2 = trackingManager.GetDiffData(contextForThread2);
      if (diffData1 != null)
      {
        diffChangeList = diffData1;
        ctData.Add("TrackPrimaryDiffDataUsed", (object) true);
        ctData.Add("TrackSecondaryDiffDataUsed", (object) false);
      }
      else if (diffData2 != null)
      {
        trackingContext2 = contextForThread2;
        diffChangeList = diffData2;
        ctData.Add("TrackPrimaryDiffDataUsed", (object) false);
        ctData.Add("TrackSecondaryDiffDataUsed", (object) true);
      }
      else
      {
        ctData.Add("TrackPrimaryDiffDataUsed", (object) false);
        ctData.Add("TrackSecondaryDiffDataUsed", (object) false);
      }
      int origStartLine = trackingContext2.OrigStartLine;
      int origStartOffset = trackingContext2.OrigStartOffset;
      int origEndLine = trackingContext2.OrigEndLine;
      int offset = trackingContext2.OrigEndOffset;
      DiffChangeType? nullable1 = new DiffChangeType?();
      DiffChangeType? nullable2 = new DiffChangeType?();
      if (diffChangeList != null && !diffChangeList.IsNullOrEmpty<IDiffChange>())
      {
        DiffChangeType? nullable3 = CommentTrackingUtility.TrackPosition(diffChangeList, ref origStartLine, ref origStartOffset, trackingContext2.TrackBackward);
        DiffChangeType? nullable4 = CommentTrackingUtility.TrackPosition(diffChangeList, ref origEndLine, ref offset, trackingContext2.TrackBackward, false);
        DiffChangeType? nullable5 = nullable3;
        DiffChangeType diffChangeType1 = DiffChangeType.Delete;
        if (nullable5.GetValueOrDefault() == diffChangeType1 & nullable5.HasValue)
        {
          nullable5 = nullable4;
          DiffChangeType diffChangeType2 = DiffChangeType.Delete;
          if (nullable5.GetValueOrDefault() == diffChangeType2 & nullable5.HasValue)
          {
            offset = 0;
            goto label_11;
          }
        }
        nullable5 = nullable3;
        DiffChangeType diffChangeType3 = DiffChangeType.Delete;
        if (nullable5.GetValueOrDefault() == diffChangeType3 & nullable5.HasValue)
          ++origStartLine;
      }
label_11:
      ctData.Add("TrackNewStartLine", (object) origStartLine);
      ctData.Add("TrackNewStartOffset", (object) origStartOffset);
      ctData.Add("TrackNewEndLine", (object) origEndLine);
      ctData.Add("TrackNewEndOffset", (object) offset);
      bool flag1 = trackingContext2.SourceSide != trackingContext2.TargetSide;
      bool flag2 = trackingContext2.SourceVersion != trackingContext2.TargetVersion;
      bool flag3 = trackingContext1.SourceFilename != null && trackingContext1.TargetFilename != null && !trackingContext1.SourceFilename.Equals(trackingContext1.TargetFilename, StringComparison.Ordinal);
      bool flag4 = origStartLine != trackingContext2.OrigStartLine || origStartOffset != trackingContext2.OrigStartOffset || origEndLine != trackingContext2.OrigEndLine || offset != trackingContext2.OrigEndOffset;
      ctData.Add("TrackSideChanged", (object) flag1);
      ctData.Add("TrackVersionChanged", (object) flag2);
      ctData.Add("TrackFilenameChanged", (object) flag3);
      ctData.Add("TrackPositionChanged", (object) flag4);
      bool flag5 = false;
      if (flag1 | flag4 | flag2 | flag3)
      {
        trackingCriteria.OrigFilePath = threadContext.FilePath;
        trackingCriteria.OrigLeftFileStart = threadContext.LeftFileStart;
        trackingCriteria.OrigLeftFileEnd = threadContext.LeftFileEnd;
        trackingCriteria.OrigRightFileStart = threadContext.RightFileStart;
        trackingCriteria.OrigRightFileEnd = threadContext.RightFileEnd;
        flag5 = true;
        threadContext.TrackingCriteria = trackingCriteria;
      }
      if (flag1 | flag4)
      {
        threadContext.LeftFileStart = threadContext.LeftFileEnd = threadContext.RightFileStart = threadContext.RightFileEnd = (Microsoft.VisualStudio.Services.CodeReview.WebApi.Position) null;
        if (trackingContext2.TargetSide == TrackingSide.Left)
        {
          threadContext.LeftFileStart = new Microsoft.VisualStudio.Services.CodeReview.WebApi.Position();
          threadContext.LeftFileStart.Line = origStartLine;
          threadContext.LeftFileStart.Offset = origStartOffset;
          threadContext.LeftFileEnd = new Microsoft.VisualStudio.Services.CodeReview.WebApi.Position();
          threadContext.LeftFileEnd.Line = origEndLine;
          threadContext.LeftFileEnd.Offset = offset;
        }
        else
        {
          threadContext.RightFileStart = new Microsoft.VisualStudio.Services.CodeReview.WebApi.Position();
          threadContext.RightFileStart.Line = origStartLine;
          threadContext.RightFileStart.Offset = origStartOffset;
          threadContext.RightFileEnd = new Microsoft.VisualStudio.Services.CodeReview.WebApi.Position();
          threadContext.RightFileEnd.Line = origEndLine;
          threadContext.RightFileEnd.Offset = offset;
        }
      }
      if (flag3)
        threadContext.FilePath = trackingContext1.TargetFilename;
      ctData.Add("TrackWasTracked", (object) flag5);
      if (requestContext != null && correlationId != Guid.Empty)
      {
        CommentTrackingUtility.TryTraceDiffData(requestContext, (IEnumerable<IDiffChange>) diffChangeList, correlationId);
        CommentTrackingUtility.TryPublishCtData(requestContext, ctData);
      }
      return flag5;
    }

    internal static void TryPublishCtData(IVssRequestContext requestContext, ClientTraceData ctData)
    {
      try
      {
        requestContext.GetService<ClientTraceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.CodeReview.Server", "CommentTracking", ctData);
      }
      catch
      {
      }
    }

    internal static void TryTraceDiffData(
      IVssRequestContext requestContext,
      IEnumerable<IDiffChange> diffData,
      Guid correlationId)
    {
      if (diffData == null)
        return;
      requestContext.TraceDataConditionally(1384400, TraceLevel.Verbose, "CodeReview", CommentTrackingUtility.c_Layer, "Tracking diff data for correlation id " + correlationId.ToString(), (Func<object>) (() => (object) new
      {
        diffData = diffData
      }), "TrackThread");
    }
  }
}
