// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.ObjectModel.GitPullRequestCodeReviewModelExtensions
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace Microsoft.TeamFoundation.SourceControl.WebServer.ObjectModel
{
  public static class GitPullRequestCodeReviewModelExtensions
  {
    public static void AddReferenceLinks(
      this GitPullRequestIteration iteration,
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest)
    {
      iteration.Links = GitPullRequestLinksUtility.GetPullRequestIterationReferenceLinks(requestContext, GitPullRequestCodeReviewModelExtensions.GetRepoKey(pullRequest), pullRequest.PullRequestId, iteration.Id);
    }

    public static void AddReferenceLinks(
      this GitPullRequestCommentThread commentThread,
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest)
    {
      commentThread.Links = GitPullRequestLinksUtility.GetPullRequestThreadReferenceLinks(requestContext, GitPullRequestCodeReviewModelExtensions.GetRepoKey(pullRequest), pullRequest.PullRequestId, commentThread);
    }

    public static void AddReferenceLinks(
      this Comment comment,
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      int threadId)
    {
      comment.Links = GitPullRequestLinksUtility.GetPullRequestCommentReferenceLinks(requestContext, GitPullRequestCodeReviewModelExtensions.GetRepoKey(pullRequest), pullRequest.PullRequestId, threadId, comment);
    }

    public static void AddReferenceLinks(
      this GitPullRequestStatus status,
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      int? iterationId = null)
    {
      status.Links = GitPullRequestLinksUtility.GetPullRequestStatusReferenceLinks(requestContext, GitPullRequestCodeReviewModelExtensions.GetRepoKey(pullRequest), pullRequest.PullRequestId, status.Id, iterationId);
    }

    private static RepoKey GetRepoKey(TfsGitPullRequest pullRequest) => new RepoKey(ProjectInfo.GetProjectId(pullRequest.ProjectUri), pullRequest.RepositoryId);
  }
}
