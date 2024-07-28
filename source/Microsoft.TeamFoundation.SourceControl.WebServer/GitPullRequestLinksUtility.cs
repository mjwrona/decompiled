// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPullRequestLinksUtility
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class GitPullRequestLinksUtility
  {
    public static ReferenceLinks GetPullRequestReferenceLinks(
      this GitPullRequest gitPullRequest,
      IVssRequestContext requestContext,
      ISecuredObject securedObject)
    {
      RepoKey repoKey = new RepoKey(gitPullRequest.Repository.ProjectReference.Id, gitPullRequest.Repository.Id);
      ReferenceLinks baseReferenceLinks = GitReferenceLinksUtility.GetBaseReferenceLinks(requestContext, gitPullRequest.Url, repoKey, securedObject);
      baseReferenceLinks.AddLink("workItems", GitPullRequestLinksUtility.GetPullRequestWorkItemUrl(requestContext, repoKey, gitPullRequest.PullRequestId), securedObject);
      baseReferenceLinks.AddLink("sourceBranch", GitReferenceLinksUtility.GetRefUrl(requestContext, repoKey, gitPullRequest.SourceRefName), securedObject);
      baseReferenceLinks.AddLink("targetBranch", GitReferenceLinksUtility.GetRefUrl(requestContext, repoKey, gitPullRequest.TargetRefName), securedObject);
      baseReferenceLinks.AddLink("statuses", GitPullRequestLinksUtility.GetPullRequestStatusesUrl(requestContext, repoKey, gitPullRequest.PullRequestId), securedObject);
      if (gitPullRequest.LastMergeSourceCommit != null)
        baseReferenceLinks.AddLink("sourceCommit", gitPullRequest.LastMergeSourceCommit.Url, securedObject);
      if (gitPullRequest.LastMergeTargetCommit != null)
        baseReferenceLinks.AddLink("targetCommit", gitPullRequest.LastMergeTargetCommit.Url, securedObject);
      if (gitPullRequest.Status == PullRequestStatus.Completed && gitPullRequest.LastMergeCommit != null)
        baseReferenceLinks.AddLink("mergeCommit", gitPullRequest.LastMergeCommit.Url, securedObject);
      if (gitPullRequest.CreatedBy != null && gitPullRequest.CreatedBy.Url != null)
        baseReferenceLinks.AddLink("createdBy", gitPullRequest.CreatedBy.Url, securedObject);
      if (gitPullRequest.SupportsIterations)
        baseReferenceLinks.AddLink("iterations", GitPullRequestLinksUtility.GetPullRequestIterationUrl(requestContext, repoKey, gitPullRequest.PullRequestId), securedObject);
      return baseReferenceLinks;
    }

    public static string GetPullRequestWorkItemUrl(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int pullRequestId)
    {
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "git", GitWebApiConstants.PullRequestWorkItemsLocationId, RouteValuesFactory.PullRequest(repoKey, pullRequestId)).AbsoluteUri;
    }

    public static ReferenceLinks GetPullRequestIterationReferenceLinks(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int pullRequestId,
      int? iterationId = null)
    {
      string requestIterationUrl = GitPullRequestLinksUtility.GetPullRequestIterationUrl(requestContext, repoKey, pullRequestId, iterationId);
      string requestStatusesUrl = GitPullRequestLinksUtility.GetPullRequestStatusesUrl(requestContext, repoKey, pullRequestId, iterationId);
      ReferenceLinks baseReferenceLinks = GitReferenceLinksUtility.GetBaseReferenceLinks(requestContext, requestIterationUrl, repoKey);
      baseReferenceLinks.AddLink("statuses", requestStatusesUrl);
      return baseReferenceLinks;
    }

    public static string GetPullRequestIterationUrl(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int pullRequestId,
      int? iterationId = null)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      object obj = !iterationId.HasValue ? RouteValuesFactory.PullRequest(repoKey, pullRequestId) : RouteValuesFactory.Iteration(repoKey, pullRequestId, iterationId.Value);
      IVssRequestContext requestContext1 = requestContext;
      Guid iterationsLocationId = GitWebApiConstants.PullRequestIterationsLocationId;
      object routeValues = obj;
      return service.GetResourceUri(requestContext1, "git", iterationsLocationId, routeValues).AbsoluteUri;
    }

    public static ReferenceLinks GetPullRequestStatusReferenceLinks(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int pullRequestId,
      int statusId,
      int? iterationId = null)
    {
      string requestStatusUrl = GitPullRequestLinksUtility.GetPullRequestStatusUrl(requestContext, repoKey, pullRequestId, statusId, iterationId);
      return GitReferenceLinksUtility.GetBaseReferenceLinks(requestContext, requestStatusUrl, repoKey);
    }

    public static string GetPullRequestStatusUrl(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int pullRequestId,
      int statusId,
      int? iterationId = null)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      return (!iterationId.HasValue ? service.GetResourceUri(requestContext, "git", GitWebApiConstants.PullRequestStatusesLocationId, (object) new
      {
        repositoryId = repoKey.RepoId,
        pullRequestId = pullRequestId,
        resource = "statuses",
        statusId = statusId
      }) : service.GetResourceUri(requestContext, "git", GitWebApiConstants.PullRequestIterationStatusesLocationId, (object) new
      {
        repositoryId = repoKey.RepoId,
        pullRequestId = pullRequestId,
        iterationId = iterationId.Value,
        resource = "statuses",
        statusId = statusId
      })).AbsoluteUri;
    }

    public static string GetPullRequestStatusesUrl(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int pullRequestId,
      int? iterationId = null)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      return (!iterationId.HasValue ? service.GetResourceUri(requestContext, "git", GitWebApiConstants.PullRequestStatusesLocationId, RouteValuesFactory.PullRequest(repoKey, pullRequestId)) : service.GetResourceUri(requestContext, "git", GitWebApiConstants.PullRequestIterationStatusesLocationId, RouteValuesFactory.Iteration(repoKey, pullRequestId, iterationId.Value))).AbsoluteUri;
    }

    public static ReferenceLinks GetPullRequestCommentReferenceLinks(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int pullRequestId,
      int threadId,
      Comment comment)
    {
      string requestCommentUrl = GitPullRequestLinksUtility.GetPullRequestCommentUrl(requestContext, repoKey, pullRequestId, threadId, new int?((int) comment.Id));
      ReferenceLinks baseReferenceLinks = GitReferenceLinksUtility.GetBaseReferenceLinks(requestContext, requestCommentUrl, repoKey, (ISecuredObject) comment);
      baseReferenceLinks.AddLink("threads", GitPullRequestLinksUtility.GetPullRequestThreadUrl(requestContext, repoKey, pullRequestId, new int?(threadId)), (ISecuredObject) comment);
      baseReferenceLinks.AddLink("pullRequests", GitPullRequestLinksUtility.GetPullRequestUrl(requestContext, repoKey, pullRequestId), (ISecuredObject) comment);
      return baseReferenceLinks;
    }

    public static ReferenceLinks GetPullRequestThreadReferenceLinks(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int pullRequestId,
      GitPullRequestCommentThread commentThread)
    {
      string requestThreadUrl = GitPullRequestLinksUtility.GetPullRequestThreadUrl(requestContext, repoKey, pullRequestId, new int?(commentThread.Id));
      return GitReferenceLinksUtility.GetBaseReferenceLinks(requestContext, requestThreadUrl, repoKey, (ISecuredObject) commentThread);
    }

    public static string GetPullRequestCommentUrl(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int pullRequestId,
      int threadId,
      int? commentId = null)
    {
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "git", GitWebApiConstants.PullRequestThreadCommentsLocationId, (object) new
      {
        repositoryId = repoKey.RepoId,
        pullRequestId = pullRequestId,
        threadId = threadId,
        resource = "comments",
        commentId = commentId
      }).AbsoluteUri;
    }

    public static string GetPullRequestThreadUrl(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int pullRequestId,
      int? threadId = null)
    {
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "git", GitWebApiConstants.PullRequestThreadsLocationId, (object) new
      {
        repositoryId = repoKey.RepoId,
        pullRequestId = pullRequestId,
        resource = "threads",
        threadId = threadId
      }).AbsoluteUri;
    }

    public static string GetPullRequestUrl(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int pullRequestId)
    {
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "git", GitWebApiConstants.PullRequestByIdLocationId, (object) new
      {
        repositoryId = repoKey.RepoId,
        pullRequestId = pullRequestId,
        resource = "pullRequests"
      }).AbsoluteUri;
    }
  }
}
