// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.AzureRepoPRMetricsPublisher
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class AzureRepoPRMetricsPublisher : ICoverageMetricsPublisher
  {
    private const string IterationIdMetaData = "IterationId";

    public void PublishCoverageMetrics(
      TestManagementRequestContext requestContext,
      PipelineContext pipelineContext,
      CoverageMetrics coverageMetrics,
      IVersionControlProvider versionControlProvider,
      CancellationToken cancellationToken,
      Dictionary<string, FolderCoverageResult> folderCoverageResults = null)
    {
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("projectId", (object) pipelineContext.ProjectId.ToString());
      data.Add("repositoryId", (object) pipelineContext.RepositoryId);
      data.Add("pullRequestId", (object) pipelineContext.PullRequestId);
      data.Add("iterationId", (object) pipelineContext.PullRequestIterationId);
      try
      {
        using (new SimpleTimer(requestContext.RequestContext, nameof (PublishCoverageMetrics)))
        {
          string comment = (folderCoverageResults == null || folderCoverageResults.Count <= 0 ? new MarkDownComment(requestContext, coverageMetrics, pipelineContext.PullRequestIterationId, pipelineContext.PullRequestIterationUrl, pipelineContext.CoverageReportUrl) : new MarkDownComment(requestContext, coverageMetrics, pipelineContext.PullRequestIterationId, pipelineContext.PullRequestIterationUrl, pipelineContext.CoverageReportUrl, folderCoverageResults)).CreateComment();
          if (string.IsNullOrEmpty(comment))
          {
            data.Add("CommentValue", (object) "Empty");
            return;
          }
          CommentThreadStatus status = this.GetCommentDefaultStatus(requestContext);
          int num1 = requestContext.IsFeatureEnabled("TestManagement.Server.EnableActiveCommentOnCodeCoveragePolicyFailure") ? 1 : 0;
          bool flag = !folderCoverageResults.IsNullOrEmpty<KeyValuePair<string, FolderCoverageResult>>();
          CoverageStatusCheckState statusCheckState = CoverageStatusCheckState.Succeeded;
          if (num1 != 0)
          {
            if (flag && folderCoverageResults.Where<KeyValuePair<string, FolderCoverageResult>>((Func<KeyValuePair<string, FolderCoverageResult>, bool>) (x => x.Value.CoverageStatusCheck == CoverageStatusCheckState.Failed)).Any<KeyValuePair<string, FolderCoverageResult>>())
              statusCheckState = CoverageStatusCheckState.Failed;
            if (CoverageStatusCheckState.Failed == coverageMetrics.CoverageStatusCheckResult.State || statusCheckState == CoverageStatusCheckState.Failed)
              status = CommentThreadStatus.Active;
          }
          data.Add("CoverageStatusCheckResult", (object) coverageMetrics.CoverageStatusCheckResult.State.ToString());
          data.Add("CommentStatus", (object) status.ToString());
          GitPullRequestCommentThread currentIteration = this.GetPreviouslyPostedCommentForCurrentIteration(requestContext, pipelineContext, versionControlProvider, cancellationToken);
          if (currentIteration != null)
          {
            versionControlProvider.UpdatePullRequestThreadComment(requestContext, pipelineContext, currentIteration, comment, currentIteration.Id, cancellationToken);
            data.Add("threadId", (object) currentIteration.Id);
          }
          else
          {
            if (pipelineContext.PullRequestIterationId > 1)
            {
              int num2 = this.ClosePreviousActiveComments(requestContext, pipelineContext, versionControlProvider, cancellationToken);
              data.Add("closedCommentsCount", (object) num2);
            }
            GitPullRequestCommentThread requestCommentThread = this.PublishNewComment(requestContext, pipelineContext, versionControlProvider, comment, status, cancellationToken);
            data.Add("threadId", (object) requestCommentThread.Id);
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.Logger.Error(10983217, string.Format("Failed to publish coverage comment for pull request: {0} with Exception: {1}.", (object) pipelineContext.PullRequestId, (object) ex));
        data.Add("exception", (object) ex.Message);
        throw;
      }
      finally
      {
        if (data.Count > 0)
        {
          CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) data);
          TelemetryLogger.Instance.PublishData(requestContext.RequestContext, nameof (AzureRepoPRMetricsPublisher), cid);
        }
      }
      requestContext.Logger.Info(10983218, string.Format("Published coverage comment for pull request:{0}, iteration: {1}", (object) pipelineContext.PullRequestId, (object) pipelineContext.PullRequestIterationId));
    }

    private GitPullRequestCommentThread PublishNewComment(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      IVersionControlProvider versionControlProvider,
      string coverageComment,
      CommentThreadStatus status,
      CancellationToken cancellationToken)
    {
      GitPullRequestCommentThread discussionCommentThread = AzureRepoPRMetricsPublisher.CreateDiscussionCommentThread(coverageComment, status);
      discussionCommentThread.Properties.Add("IterationId", (object) pipelineContext.PullRequestIterationId);
      return versionControlProvider.CreateThread(tcmRequestContext, pipelineContext, discussionCommentThread, CancellationToken.None);
    }

    private GitPullRequestCommentThread GetPreviouslyPostedCommentForCurrentIteration(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      IVersionControlProvider versionControlProvider,
      CancellationToken cancellationToken)
    {
      List<GitPullRequestCommentThread> requestCommentThreads = versionControlProvider.GetPullRequestCommentThreads(tcmRequestContext, pipelineContext, new int?(), CancellationToken.None);
      if (requestCommentThreads == null)
        return (GitPullRequestCommentThread) null;
      List<GitPullRequestCommentThread> all = requestCommentThreads.FindAll((Predicate<GitPullRequestCommentThread>) (x => (x.Status == CommentThreadStatus.Active || x.Status == CommentThreadStatus.Closed) && x.Comments != null && x.Comments.Count > 0 && string.Equals(x.Comments[0].Author.Id, TestManagementServerConstants.TCMServiceInstanceType.ToString(), StringComparison.OrdinalIgnoreCase))).FindAll((Predicate<GitPullRequestCommentThread>) (x => x.Properties != null && x.Properties.ContainsKey("IterationId") && Convert.ToInt32(x.Properties["IterationId"]) == pipelineContext.PullRequestIterationId));
      return all == null || !all.Any<GitPullRequestCommentThread>() ? (GitPullRequestCommentThread) null : all.Last<GitPullRequestCommentThread>();
    }

    private int ClosePreviousActiveComments(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      IVersionControlProvider versionControlProvider,
      CancellationToken cancellationToken)
    {
      List<GitPullRequestCommentThread> requestCommentThreads = versionControlProvider.GetPullRequestCommentThreads(tcmRequestContext, pipelineContext, new int?(), CancellationToken.None);
      if (requestCommentThreads == null)
        return 0;
      List<GitPullRequestCommentThread> all = requestCommentThreads.FindAll((Predicate<GitPullRequestCommentThread>) (x => x.Status == CommentThreadStatus.Active && x.Comments != null && x.Comments.Count > 0 && string.Equals(x.Comments[0].Author.Id, TestManagementServerConstants.TCMServiceInstanceType.ToString(), StringComparison.OrdinalIgnoreCase)));
      foreach (GitPullRequestCommentThread requestCommentThread1 in all)
      {
        GitPullRequestCommentThread requestCommentThread2 = new GitPullRequestCommentThread((ISecuredObject) requestCommentThread1);
        requestCommentThread2.Status = CommentThreadStatus.Closed;
        GitPullRequestCommentThread commentThread = requestCommentThread2;
        versionControlProvider.UpdatePullRequestThread(tcmRequestContext, pipelineContext, commentThread, requestCommentThread1.Id, cancellationToken);
      }
      return all.Count;
    }

    private static GitPullRequestCommentThread CreateDiscussionCommentThread(
      string commentText,
      CommentThreadStatus status)
    {
      GitPullRequestCommentThread discussionCommentThread = new GitPullRequestCommentThread();
      discussionCommentThread.Status = status;
      discussionCommentThread.Comments = (IList<Comment>) new Comment[1]
      {
        new Comment()
        {
          Content = commentText,
          CommentType = CommentType.Text
        }
      };
      discussionCommentThread.Properties = new PropertiesCollection();
      return discussionCommentThread;
    }

    private CommentThreadStatus GetCommentDefaultStatus(TestManagementRequestContext requestContext) => new CoverageConfiguration().GetCommentDefaultStatus(requestContext.RequestContext);
  }
}
