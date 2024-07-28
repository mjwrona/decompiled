// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPullRequestThreadCommentsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
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
  public class GitPullRequestThreadCommentsController : GitApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (Comment), null, null)]
    [ClientLocationId("965A3EC7-5ED8-455A-BDCB-835A5EA7FE7B")]
    public HttpResponseMessage GetComment(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      [ClientIgnore] string projectId = null)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        TfsGitPullRequest pullRequestDetails = service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId);
        if (pullRequestDetails == null)
          throw new GitPullRequestNotFoundException();
        bool headerOptionValue = MediaTypeFormatUtility.GetExcludeUrlsAcceptHeaderOptionValue(this.Request);
        GitPullRequestCommentThread thread = service.GetThread(this.TfsRequestContext, tfsGitRepository, pullRequestDetails, threadId, new int?(), new int?());
        if (thread == null)
          throw new GitPullRequestCommentThreadNotFoundException(threadId);
        if (thread.Comments != null)
        {
          foreach (Comment comment in (IEnumerable<Comment>) thread.Comments)
          {
            if ((int) comment.Id == commentId)
            {
              if (!headerOptionValue)
                comment.AddReferenceLinks(this.TfsRequestContext, pullRequestDetails, thread.Id);
              return this.Request.CreateResponse<Comment>(HttpStatusCode.OK, comment);
            }
          }
        }
        throw new GitPullRequestCommentNotFoundException(threadId, commentId);
      }
    }

    [HttpGet]
    [ClientResponseType(typeof (List<Comment>), null, null)]
    [ClientLocationId("965A3EC7-5ED8-455A-BDCB-835A5EA7FE7B")]
    public HttpResponseMessage GetComments(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int threadId,
      [ClientIgnore] string projectId = null)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        TfsGitPullRequest pullRequestDetails = service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId);
        if (pullRequestDetails == null)
          throw new GitPullRequestNotFoundException();
        int num = MediaTypeFormatUtility.GetExcludeUrlsAcceptHeaderOptionValue(this.Request) ? 1 : 0;
        GitPullRequestCommentThread thread = service.GetThread(this.TfsRequestContext, tfsGitRepository, pullRequestDetails, threadId, new int?(), new int?());
        if (num == 0 && thread.Comments != null)
        {
          foreach (Comment comment in (IEnumerable<Comment>) thread.Comments)
            comment.AddReferenceLinks(this.TfsRequestContext, pullRequestDetails, thread.Id);
        }
        return this.GenerateResponse<Comment>((IEnumerable<Comment>) thread.Comments ?? Enumerable.Empty<Comment>());
      }
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("965A3EC7-5ED8-455A-BDCB-835A5EA7FE7B")]
    [ClientExample("DELETE__git_repositories__repositoryId__pullRequests__pullRequestId__threads__threadId__comments__commentId_.json", null, null, null)]
    public HttpResponseMessage DeleteComment(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      [ClientIgnore] string projectId = null)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        service.DeleteComment(this.TfsRequestContext, tfsGitRepository, service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId) ?? throw new GitPullRequestNotFoundException(), threadId, commentId);
        return this.Request.CreateResponse(HttpStatusCode.OK);
      }
    }

    [HttpPost]
    [ClientResponseType(typeof (Comment), null, null)]
    [ClientExample("POST__git_repositories__repositoryId__pullRequests__pullRequestId__threads__threadId__comments.json", null, null, null)]
    public HttpResponseMessage CreateComment(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int threadId,
      Comment comment,
      [ClientIgnore] string projectId = null)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        TfsGitPullRequest pullRequestDetails = service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId);
        if (pullRequestDetails == null)
          throw new GitPullRequestNotFoundException();
        bool headerOptionValue = MediaTypeFormatUtility.GetExcludeUrlsAcceptHeaderOptionValue(this.Request);
        Comment comment1 = service.AddComment(this.TfsRequestContext, tfsGitRepository, pullRequestDetails, threadId, comment);
        if (comment1 != null && !headerOptionValue)
          comment1.AddReferenceLinks(this.TfsRequestContext, pullRequestDetails, threadId);
        return this.Request.CreateResponse<Comment>(HttpStatusCode.OK, comment1);
      }
    }

    [HttpPatch]
    [ClientResponseType(typeof (Comment), null, null)]
    public HttpResponseMessage UpdateComment(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      Comment comment,
      [ClientIgnore] string projectId = null)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        TfsGitPullRequest pullRequestDetails = service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId);
        if (pullRequestDetails == null)
          throw new GitPullRequestNotFoundException();
        int num = MediaTypeFormatUtility.GetExcludeUrlsAcceptHeaderOptionValue(this.Request) ? 1 : 0;
        Comment comment1 = service.UpdateComment(this.TfsRequestContext, tfsGitRepository, pullRequestDetails, threadId, commentId, comment);
        if (num == 0)
          comment1.AddReferenceLinks(this.TfsRequestContext, pullRequestDetails, threadId);
        return this.Request.CreateResponse<Comment>(HttpStatusCode.OK, comment1);
      }
    }
  }
}
