// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPullRequestStatuses2Controller
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
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
  [ControllerApiVersion(7.2)]
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "PullRequestStatuses", ResourceVersion = 2)]
  public class GitPullRequestStatuses2Controller : GitPullRequestStatusesController
  {
    [HttpGet]
    [ClientResponseType(typeof (List<GitPullRequestStatus>), null, null)]
    [ClientLocationId("B5F6BB4F-8D1E-4D79-8D11-4C9172C99C35")]
    [ClientExample("GET_git_pullRequestStatuses_statuses.json", null, null, null)]
    public override HttpResponseMessage GetPullRequestStatuses(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      [ClientIgnore] string projectId = null)
    {
      return this.GenerateResponse<GitPullRequestStatus>(this.GetPullRequestStatusesInternal(repositoryId, pullRequestId, projectId));
    }

    [HttpGet]
    [ClientResponseType(typeof (GitPullRequestStatus), null, null)]
    [ClientLocationId("B5F6BB4F-8D1E-4D79-8D11-4C9172C99C35")]
    [ClientExample("GET_git_pullRequestStatuses_status.json", null, null, null)]
    public override HttpResponseMessage GetPullRequestStatus(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int statusId,
      [ClientIgnore] string projectId = null)
    {
      return this.Request.CreateResponse<GitPullRequestStatus>(HttpStatusCode.OK, this.GetPullRequestStatusInternal(repositoryId, pullRequestId, statusId, projectId));
    }

    [HttpPost]
    [ClientResponseType(typeof (GitPullRequestStatus), null, null)]
    [ClientLocationId("B5F6BB4F-8D1E-4D79-8D11-4C9172C99C35")]
    [ClientExample("POST_git_pullRequestStatuses_status.json", "On pull request", null, null)]
    [ClientExample("POST_git_pullRequestStatuses_statusIterationInBody.json", "On iteration", null, null)]
    [ClientExample("POST_git_pullRequestStatuses_statusWithProperties.json", "With properties", "An optional property bag of key-value pairs is available when posting status on a PR. Include your properties in the request body, as shown below.", null)]
    public override HttpResponseMessage CreatePullRequestStatus(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      GitPullRequestStatus status,
      [ClientIgnore] string projectId = null)
    {
      return this.CreatePullRequestStatusInternal(repositoryId, pullRequestId, status, projectId: projectId);
    }
  }
}
