// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPullRequests2Controller
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ControllerApiVersion(3.1)]
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "pullRequests")]
  public class GitPullRequests2Controller : GitPullRequestsController
  {
    [HttpPatch]
    [ClientLocationId("9946FD70-0D40-406E-B686-B4744CBBCC37")]
    [ClientResponseType(typeof (GitPullRequest), null, null)]
    public override HttpResponseMessage UpdatePullRequest(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientParameterType(typeof (int), false)] string pullRequestId,
      GitPullRequest gitPullRequestToUpdate,
      [ClientIgnore] string projectId = null)
    {
      GitPullRequest entity = this.UpdatePullRequestInternal(repositoryId, pullRequestId, gitPullRequestToUpdate, projectId);
      GitStatusStateMapper.MapGitEntity<GitPullRequest>(entity, this.TfsRequestContext);
      return this.Request.CreateResponse<GitPullRequest>(HttpStatusCode.OK, entity);
    }
  }
}
