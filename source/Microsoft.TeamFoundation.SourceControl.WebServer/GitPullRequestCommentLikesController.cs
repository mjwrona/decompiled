// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPullRequestCommentLikesController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class GitPullRequestCommentLikesController : GitApiController
  {
    [HttpPost]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("5F2E2851-1389-425B-A00B-FB2ADB3EF31B")]
    public HttpResponseMessage CreateLike(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      [ClientIgnore] string projectId = null)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        service.LikeComment(this.TfsRequestContext, tfsGitRepository, service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId) ?? throw new GitPullRequestNotFoundException(), threadId, commentId);
        return this.Request.CreateResponse(HttpStatusCode.OK);
      }
    }

    [HttpGet]
    [ClientResponseType(typeof (List<IdentityRef>), null, null)]
    [ClientLocationId("5F2E2851-1389-425B-A00B-FB2ADB3EF31B")]
    public HttpResponseMessage GetLikes(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      [ClientIgnore] string projectId = null)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        return this.Request.CreateResponse<List<IdentityRef>>(HttpStatusCode.OK, service.GetLikes(this.TfsRequestContext, tfsGitRepository, service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId) ?? throw new GitPullRequestNotFoundException(), threadId, commentId));
      }
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("5F2E2851-1389-425B-A00B-FB2ADB3EF31B")]
    public HttpResponseMessage DeleteLike(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int threadId,
      int commentId,
      [ClientIgnore] string projectId = null)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        service.WithdrawLikeComment(this.TfsRequestContext, tfsGitRepository, service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId) ?? throw new GitPullRequestNotFoundException(), threadId, commentId);
        return this.Request.CreateResponse(HttpStatusCode.OK);
      }
    }
  }
}
