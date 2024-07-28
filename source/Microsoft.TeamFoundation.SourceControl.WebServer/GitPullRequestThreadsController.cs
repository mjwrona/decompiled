// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPullRequestThreadsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer.ObjectModel;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class GitPullRequestThreadsController : GitApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (GitPullRequestCommentThread), null, null)]
    [ClientLocationId("AB6E2E5D-A0B7-4153-B64A-A4EFE0D49449")]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetPullRequestThread(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int threadId,
      [FromUri(Name = "$iteration")] int? iteration = null,
      [FromUri(Name = "$baseIteration")] int? baseIteration = null,
      [ClientIgnore] string projectId = null)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        TfsGitPullRequest pullRequestDetails = service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId);
        if (pullRequestDetails == null)
          throw new GitPullRequestNotFoundException();
        int num = MediaTypeFormatUtility.GetExcludeUrlsAcceptHeaderOptionValue(this.Request) ? 1 : 0;
        GitPullRequestCommentThread thread = service.GetThread(this.TfsRequestContext, tfsGitRepository, pullRequestDetails, threadId, iteration, baseIteration);
        if (num == 0)
          this.AddReferenceLinks(thread, pullRequestDetails);
        return this.Request.CreateResponse<GitPullRequestCommentThread>(HttpStatusCode.OK, thread);
      }
    }

    [HttpGet]
    [ClientResponseType(typeof (List<GitPullRequestCommentThread>), null, null)]
    [ClientLocationId("AB6E2E5D-A0B7-4153-B64A-A4EFE0D49449")]
    [ClientExample("GET__git_repositories__repositoryId__pullRequests__pullRequestId__threads.json", null, null, null)]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetThreads(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      [FromUri(Name = "$iteration")] int? iteration = null,
      [FromUri(Name = "$baseIteration")] int? baseIteration = null,
      [ClientIgnore] string projectId = null)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        TfsGitPullRequest pullRequest = service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId);
        if (pullRequest == null)
          throw new GitPullRequestNotFoundException();
        bool excludeUrl = MediaTypeFormatUtility.GetExcludeUrlsAcceptHeaderOptionValue(this.Request);
        return this.GenerateResponse<GitPullRequestCommentThread>((IEnumerable<GitPullRequestCommentThread>) service.GetThreads(this.TfsRequestContext, tfsGitRepository, pullRequest, iteration, baseIteration).Select<GitPullRequestCommentThread, GitPullRequestCommentThread>((Func<GitPullRequestCommentThread, GitPullRequestCommentThread>) (thread =>
        {
          if (!excludeUrl)
            this.AddReferenceLinks(thread, pullRequest);
          return thread;
        })).ToArray<GitPullRequestCommentThread>(), GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key));
      }
    }

    [HttpPost]
    [ClientResponseType(typeof (GitPullRequestCommentThread), null, null)]
    [ClientExample("POST__git_repositories__repositoryId__pullRequests__pullRequestId__threads.json", "Comment on the pull request", null, null)]
    [ClientExample("POST__git_repositories__repositoryId__pullRequests__pullRequestId__threads2.json", "Comment on a specific file in the pull request", null, null)]
    public HttpResponseMessage CreateThread(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      GitPullRequestCommentThread commentThread,
      [ClientIgnore] string projectId = null)
    {
      if (commentThread == null)
        throw new ArgumentNullException(nameof (commentThread));
      if (commentThread.Comments == null || commentThread.Comments.Count == 0)
        throw new ArgumentNullException("comments");
      if (commentThread.Id > 0)
        throw new GitPullRequestCommentThreadCreateIdException(commentThread.Id);
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        TfsGitPullRequest pullRequestDetails = service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId);
        if (pullRequestDetails == null)
          throw new GitPullRequestNotFoundException();
        if (commentThread.PullRequestThreadContext == null && commentThread.ThreadContext != null && !string.IsNullOrEmpty(commentThread.ThreadContext.FilePath))
        {
          int num1 = 1;
          int num2 = 0;
          IEnumerable<GitPullRequestIteration> iterations = service.GetIterations(this.TfsRequestContext, tfsGitRepository, pullRequestDetails);
          if (iterations != null)
          {
            num1 = Math.Max(num1, iterations.Max<GitPullRequestIteration>((Func<GitPullRequestIteration, int>) (x => !x.Id.HasValue ? 1 : x.Id.Value)));
            GitPullRequestIterationChanges changes = service.GetChanges(this.TfsRequestContext, tfsGitRepository, pullRequestDetails, num1, out int _, new int?(0), new int?(0), new int?(0));
            if (changes != null)
            {
              GitPullRequestChange pullRequestChange = changes.ChangeEntries.FirstOrDefault<GitPullRequestChange>((Func<GitPullRequestChange, bool>) (change => change.Item != null && change.Item.Path != null && change.Item.Path.Equals(commentThread.ThreadContext.FilePath)));
              if (pullRequestChange != null)
                num2 = pullRequestChange.ChangeTrackingId;
            }
          }
          commentThread.PullRequestThreadContext = new GitPullRequestCommentThreadContext()
          {
            IterationContext = new CommentIterationContext()
            {
              FirstComparingIteration = (short) num1,
              SecondComparingIteration = (short) num1
            },
            ChangeTrackingId = num2
          };
        }
        int num = MediaTypeFormatUtility.GetExcludeUrlsAcceptHeaderOptionValue(this.Request) ? 1 : 0;
        GitPullRequestCommentThread thread = service.SaveThread(this.TfsRequestContext, tfsGitRepository, pullRequestDetails, commentThread);
        if (thread == null)
          throw new ArgumentNullException();
        if (num == 0)
          thread = this.AddReferenceLinks(thread, pullRequestDetails);
        return this.Request.CreateResponse<GitPullRequestCommentThread>(HttpStatusCode.OK, thread);
      }
    }

    [HttpPatch]
    [ClientResponseType(typeof (GitPullRequestCommentThread), null, null)]
    public HttpResponseMessage UpdateThread(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int threadId,
      GitPullRequestCommentThread commentThread,
      [ClientIgnore] string projectId = null)
    {
      if (commentThread == null)
        throw new ArgumentNullException(nameof (commentThread));
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        TfsGitPullRequest pullRequestDetails = service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId);
        if (pullRequestDetails == null)
          throw new GitPullRequestNotFoundException();
        int num = MediaTypeFormatUtility.GetExcludeUrlsAcceptHeaderOptionValue(this.Request) ? 1 : 0;
        commentThread.Id = threadId;
        GitPullRequestCommentThread thread = service.SaveThread(this.TfsRequestContext, tfsGitRepository, pullRequestDetails, commentThread);
        if (num == 0)
          thread = this.AddReferenceLinks(thread, pullRequestDetails);
        return this.Request.CreateResponse<GitPullRequestCommentThread>(HttpStatusCode.OK, thread);
      }
    }

    private GitPullRequestCommentThread AddReferenceLinks(
      GitPullRequestCommentThread thread,
      TfsGitPullRequest pullRequest)
    {
      if (thread != null)
      {
        thread.AddReferenceLinks(this.TfsRequestContext, pullRequest);
        if (thread.Comments != null)
        {
          foreach (Comment comment in (IEnumerable<Comment>) thread.Comments)
            comment.AddReferenceLinks(this.TfsRequestContext, pullRequest, thread.Id);
        }
      }
      return thread;
    }
  }
}
