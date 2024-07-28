// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.Controllers.Git.PullRequest.GitPullRequestShareController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer.Controllers.Git.PullRequest
{
  public class GitPullRequestShareController : GitApiController
  {
    [HttpPost]
    [ClientLocationId("696F3A82-47C9-487F-9117-B9D00972CA84")]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage SharePullRequest(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      ShareNotificationContext userMessage)
    {
      ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId))
        service.SharePullRequest(this.TfsRequestContext, tfsGitRepository, service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId) ?? throw new GitPullRequestNotFoundException(), userMessage);
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }
  }
}
