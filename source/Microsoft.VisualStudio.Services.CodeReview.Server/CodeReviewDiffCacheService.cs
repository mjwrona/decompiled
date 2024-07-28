// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CodeReviewDiffCacheService
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Xml;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal class CodeReviewDiffCacheService : 
    CodeReviewServiceBase,
    ICodeReviewDiffCacheService,
    IVssFrameworkService
  {
    internal const int c_numWriteAttempts = 3;

    public override void ServiceStart(IVssRequestContext systemRequestContext) => base.ServiceStart(systemRequestContext);

    public override void ServiceEnd(IVssRequestContext systemRequestContext) => base.ServiceEnd(systemRequestContext);

    public bool CacheDiffData(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      DiffCacheObject cache,
      ClientTraceData ctData = null)
    {
      bool cacheSuccess = false;
      this.ExecuteAndTrace(requestContext, nameof (CacheDiffData), 1380131, 1380132, 1380133, (Action) (() =>
      {
        requestContext.Trace(1380134, TraceLevel.Verbose, this.Area, this.Layer, "Caching diff data for: review id: '{0}', project id: '{1}'", (object) reviewId, (object) projectId);
        cacheSuccess = this.CacheDiffDataInternal(requestContext, projectId, reviewId, cache, ctData);
      }));
      return cacheSuccess;
    }

    private bool CacheDiffDataInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      DiffCacheObject cache,
      ClientTraceData ctData = null)
    {
      ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
      if (cache == null || cache.IsEmpty())
        return false;
      ICodeReviewService service = requestContext.GetService<ICodeReviewService>();
      bool flag1 = false;
      int? diffFileId = new int?();
      int? nullable1 = new int?();
      int? nullable2;
      for (int index = 0; index < 3 && !flag1; ++index)
      {
        DiffCacheObject cachedDiffData = this.GetCachedDiffData(requestContext, projectId, reviewId, out diffFileId);
        ctData?.Add("CommentTrackingCacheWriteAttemptNumber", (object) index);
        ctData?.Add("CommentTrackingCacheWriteAttemptStartTimeUtc", (object) DateTime.UtcNow);
        ctData?.Add("CommentTrackingCacheWriteAttemptOldFileId", (object) diffFileId);
        bool flag2 = cache.IsSubsetOf(cachedDiffData);
        ctData?.Add("CommentTrackingCacheWriteAttemptWasSubset", (object) flag2);
        if (flag2)
        {
          flag1 = false;
          break;
        }
        cache.CombineWith(cachedDiffData);
        nullable1 = new int?(this.StoreCacheFile(requestContext, cache));
        ctData?.Add("CommentTrackingCacheWriteAttemptNewFileId", (object) nullable1);
        if (nullable1.HasValue)
        {
          nullable2 = nullable1;
          int num = 0;
          if (nullable2.GetValueOrDefault() > num & nullable2.HasValue)
          {
            try
            {
              ctData?.Add("CommentTrackingCacheWriteAttemptWriteTimeUtc", (object) DateTime.UtcNow);
              service.SaveReview(requestContext, projectId, new Review()
              {
                Id = reviewId,
                DiffFileId = nullable1,
                ExpectedDiffFileId = diffFileId
              });
              flag1 = true;
            }
            catch (Exception ex)
            {
              ctData?.Add("CommentTrackingCacheWriteAttemptFailedUnexpectedFileId", (object) (ex is CodeReviewUnexpectedDiffFileIdException));
              this.DeleteCacheFile(requestContext, nullable1.Value);
            }
          }
        }
      }
      if (flag1 && diffFileId.HasValue)
      {
        nullable2 = diffFileId;
        int num = 0;
        if (nullable2.GetValueOrDefault() > num & nullable2.HasValue)
          this.DeleteCacheFile(requestContext, diffFileId.Value);
      }
      ctData?.Add("CommentTrackingCacheSuccess", (object) flag1);
      return flag1;
    }

    public DiffCacheObject GetCachedDiffData(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId)
    {
      return this.GetCachedDiffData(requestContext, projectId, reviewId, out int? _);
    }

    private DiffCacheObject GetCachedDiffData(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      out int? diffFileId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
      DiffCacheObject cache = (DiffCacheObject) null;
      diffFileId = new int?();
      if (this.CheckResourceVersion(requestContext))
      {
        Review review = this.GetReviewRaw(requestContext, projectId, reviewId);
        if (review != null)
        {
          int? diffFileId1 = review.DiffFileId;
          if (diffFileId1.HasValue)
          {
            diffFileId1 = review.DiffFileId;
            int num = 0;
            if (diffFileId1.GetValueOrDefault() > num & diffFileId1.HasValue)
            {
              diffFileId = review.DiffFileId;
              this.ExecuteAndTrace(requestContext, nameof (GetCachedDiffData), 1380141, 1380142, 1380143, (Action) (() =>
              {
                requestContext.Trace(1380144, TraceLevel.Verbose, this.Area, this.Layer, "Getting cached diff data for: review id: '{0}', project id: '{1}', diff file id: '{2}'", (object) reviewId, (object) projectId, (object) review.DiffFileId.Value);
                cache = this.GetCachedDiffDataInternal(requestContext, review.DiffFileId.Value);
              }));
            }
          }
        }
      }
      return cache;
    }

    private DiffCacheObject GetCachedDiffDataInternal(
      IVssRequestContext requestContext,
      int diffFileId)
    {
      diffDataInternal = (DiffCacheObject) null;
      if (this.CheckResourceVersion(requestContext))
      {
        try
        {
          using (Stream stream1 = requestContext.GetService<ITeamFoundationFileService>().RetrieveFile(requestContext, (long) diffFileId, false, out byte[] _, out long _, out CompressionType _))
          {
            DataContractJsonSerializer contractJsonSerializer = new DataContractJsonSerializer(typeof (DiffCacheObject));
            stream1.Position = 0L;
            Stream stream2 = stream1;
            if (contractJsonSerializer.ReadObject(stream2) is DiffCacheObject diffDataInternal)
            {
              if (diffDataInternal.IsEmpty())
                diffDataInternal = (DiffCacheObject) null;
            }
          }
        }
        catch (FileIdNotFoundException ex)
        {
          requestContext.Trace(1380132, TraceLevel.Verbose, this.Area, this.Layer, "Cannot find diff cache file for: file id: '{0}'", (object) diffFileId);
        }
        catch (FileNotFoundException ex)
        {
          requestContext.Trace(1380132, TraceLevel.Verbose, this.Area, this.Layer, "Cannot find diff cache file for: file id: '{0}'", (object) diffFileId);
        }
        catch (Exception ex)
        {
          requestContext.Trace(1380132, TraceLevel.Verbose, this.Area, this.Layer, "Error retrieving diff cache file for: file id: '{0}'", (object) diffFileId);
        }
      }
      return diffDataInternal;
    }

    public void QueueDiffCacheJob(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int latestIterationId)
    {
      if (!this.CheckResourceVersion(requestContext))
        return;
      ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
      ArgumentUtility.CheckForOutOfRange(latestIterationId, nameof (latestIterationId), 1);
      this.ExecuteAndTrace(requestContext, nameof (QueueDiffCacheJob), 1380151, 1380152, 1380153, (Action) (() =>
      {
        requestContext.Trace(1380154, TraceLevel.Verbose, this.Area, this.Layer, "Queueing diff cache job for: review id: '{0}', project id: '{1}', latest iteration id: '{2}'", (object) reviewId, (object) projectId, (object) latestIterationId);
        XmlNode diffCacheJobDataXml = this.CreateDiffCacheJobDataXml(projectId, reviewId, latestIterationId);
        requestContext.GetService<ITeamFoundationJobService>().QueueOneTimeJob(requestContext, string.Format("Code Review Diff Cache Job, projectId={0}, reviewId={1}, latestIterationId={2}", (object) projectId.ToString("N"), (object) reviewId, (object) latestIterationId), "Microsoft.VisualStudio.Services.CodeReview.Server.Plugins.CodeReviewDiffCacheJob", diffCacheJobDataXml, JobPriorityLevel.Highest, JobPriorityClass.Normal, TimeSpan.Zero);
      }));
    }

    private bool CheckResourceVersion(IVssRequestContext requestContext)
    {
      ITeamFoundationResourceManagementService service = requestContext.GetService<ITeamFoundationResourceManagementService>();
      return service != null && service.GetServiceVersion(requestContext, "CodeReview", "CodeReview").Version >= 8;
    }

    private int StoreCacheFile(IVssRequestContext requestContext, DiffCacheObject cache)
    {
      int num = 0;
      try
      {
        using (MemoryStream content = new MemoryStream())
        {
          new DataContractJsonSerializer(typeof (DiffCacheObject)).WriteObject((Stream) content, (object) cache);
          if (content != null)
          {
            content.Position = 0L;
            using (requestContext.AllowAnonymousOrPublicUserWrites())
              num = requestContext.GetService<ITeamFoundationFileService>().UploadFile(requestContext, (Stream) content, OwnerId.Generic, Guid.Empty);
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.Trace(1380132, TraceLevel.Verbose, this.Area, this.Layer, "Exception storing cache file: message: '{0}'", (object) ex.Message);
      }
      return num;
    }

    private void DeleteCacheFile(IVssRequestContext requestContext, int fileId)
    {
      try
      {
        requestContext.GetService<TeamFoundationFileService>().DeleteFile(requestContext, (long) fileId);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1380132, TraceLevel.Verbose, this.Area, this.Layer, "Exception deleting cache file for: file id: '{0}', message: '{1}'", (object) fileId, (object) ex.Message);
      }
    }

    internal void PerformDiffCacheJob(
      IVssRequestContext requestContext,
      Guid jobId,
      XmlNode jobDataXml,
      ClientTraceData ctData)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      Guid projectId = Guid.Empty;
      int reviewId = 0;
      int latestIterationId = 0;
      this.ParseDiffCacheDataXml(jobDataXml, out projectId, out reviewId, out latestIterationId);
      ctData.Add("CommentTrackingProjectId", (object) projectId);
      ctData.Add("CommentTrackingReviewId", (object) reviewId);
      ctData.Add("CommentTrackingIsDiffCacheJob", (object) true);
      ctData.Add("CommentTrackingCorrelationId", (object) Guid.Empty);
      ctData.Add("CommentTrackingFirstComparingIteration", (object) latestIterationId);
      ctData.Add("CommentTrackingSecondComparingIteration", (object) latestIterationId);
      List<CommentThread> threadsFromThreadsList = CommentTrackingUtility.GetFileThreadsFromThreadsList(requestContext.GetService<ICodeReviewCommentService>().GetCommentThreads(requestContext, projectId, reviewId));
      ctData.Add("CommentTrackingApplicableThreadIds", (object) threadsFromThreadsList.Select<CommentThread, int>((Func<CommentThread, int>) (t => t.DiscussionId)).ToArray<int>());
      if (threadsFromThreadsList.Count<CommentThread>() > 0)
      {
        TrackingManager trackingManager = new TrackingManager(new CommentTrackingCriteria(latestIterationId, latestIterationId));
        trackingManager.AddTrackingPairs(threadsFromThreadsList);
        trackingManager.AddTrackingPairs(threadsFromThreadsList, TrackingTarget.Secondary);
        TelemetryHelper.ExecuteAndMeasure(ctData, "CommentTrackingTimeToGetChanges", (Action) (() => CommentTrackingUtility.PopulateMetadata(requestContext, projectId, reviewId, trackingManager)));
        trackingManager.CleanUpTrackingPairs(threadsFromThreadsList, ctData);
        CachedRegistryService service = requestContext.GetService<CachedRegistryService>();
        int fileLimit = service.GetValue<int>(requestContext, (RegistryQuery) "/CodeReview/CommentTracking/JobFileLimit", true, 1000);
        int fileSizeLimit = service.GetValue<int>(requestContext, (RegistryQuery) "/CodeReview/CommentTracking/FileSizeLimit", true, 6000000);
        CommentTrackingUtility.PopulateContentAndDiffs(requestContext, projectId, reviewId, trackingManager, fileLimit, fileSizeLimit, ctData);
        TelemetryHelper.ExecuteAndMeasure(ctData, "CommentTrackingTimeToWriteCache", (Action) (() => requestContext.GetService<ICodeReviewDiffCacheService>().CacheDiffData(requestContext, projectId, reviewId, trackingManager.GetDiffDataAsCache(), ctData)));
      }
      ctData.Add("CommentTrackingTotalTimeMs", (object) stopwatch.ElapsedMilliseconds);
    }

    private XmlNode CreateDiffCacheJobDataXml(Guid projectId, int reviewId, int latestIterationId) => TeamFoundationSerializationUtility.SerializeToXml((object) new DiffCacheJobData()
    {
      ProjectId = projectId,
      ReviewId = reviewId,
      LatestIterationId = latestIterationId
    });

    private void ParseDiffCacheDataXml(
      XmlNode xmlData,
      out Guid projectId,
      out int reviewId,
      out int latestIterationId)
    {
      ArgumentUtility.CheckForNull<XmlNode>(xmlData, nameof (xmlData));
      DiffCacheJobData diffCacheJobData = TeamFoundationSerializationUtility.Deserialize<DiffCacheJobData>(xmlData);
      projectId = diffCacheJobData.ProjectId;
      reviewId = diffCacheJobData.ReviewId;
      latestIterationId = diffCacheJobData.LatestIterationId;
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
      ArgumentUtility.CheckForOutOfRange(latestIterationId, nameof (latestIterationId), 1);
    }
  }
}
