// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPullRequestStatusesController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
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
  public class GitPullRequestStatusesController : GitPullRequestStatusesBaseController
  {
    [HttpGet]
    [ClientResponseType(typeof (List<GitPullRequestStatus>), null, null)]
    [ClientLocationId("B5F6BB4F-8D1E-4D79-8D11-4C9172C99C35")]
    [ClientExample("GET_git_pullRequestStatuses_statuses.json", null, null, null)]
    public virtual HttpResponseMessage GetPullRequestStatuses(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      [ClientIgnore] string projectId = null)
    {
      return this.GenerateResponse<GitPullRequestStatus>(this.GetPullRequestStatusesInternal(repositoryId, pullRequestId, projectId).Select<GitPullRequestStatus, GitPullRequestStatus>((Func<GitPullRequestStatus, GitPullRequestStatus>) (r =>
      {
        GitStatusStateMapper.MapGitEntity<GitPullRequestStatus>(r, this.TfsRequestContext);
        return r;
      })));
    }

    [HttpGet]
    [ClientResponseType(typeof (GitPullRequestStatus), null, null)]
    [ClientLocationId("B5F6BB4F-8D1E-4D79-8D11-4C9172C99C35")]
    [ClientExample("GET_git_pullRequestStatuses_status.json", null, null, null)]
    public virtual HttpResponseMessage GetPullRequestStatus(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int statusId,
      [ClientIgnore] string projectId = null)
    {
      GitPullRequestStatus requestStatusInternal = this.GetPullRequestStatusInternal(repositoryId, pullRequestId, statusId, projectId);
      GitStatusStateMapper.MapGitEntity<GitPullRequestStatus>(requestStatusInternal, this.TfsRequestContext);
      return this.Request.CreateResponse<GitPullRequestStatus>(HttpStatusCode.OK, requestStatusInternal);
    }

    [HttpPost]
    [ClientResponseType(typeof (GitPullRequestStatus), null, null)]
    [ClientLocationId("B5F6BB4F-8D1E-4D79-8D11-4C9172C99C35")]
    [ClientExample("POST_git_pullRequestStatuses_status.json", "On pull request", null, null)]
    [ClientExample("POST_git_pullRequestStatuses_statusIterationInBody.json", "On iteration", null, null)]
    [ClientExample("POST_git_pullRequestStatuses_statusWithProperties.json", "With properties", "An optional property bag of key-value pairs is available when posting status on a PR. Include your properties in the request body, as shown below.", null)]
    public virtual HttpResponseMessage CreatePullRequestStatus(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      GitPullRequestStatus status,
      [ClientIgnore] string projectId = null)
    {
      return this.CreatePullRequestStatusInternal(repositoryId, pullRequestId, status, projectId: projectId);
    }

    [HttpPatch]
    [ClientRequestContentMediaType("application/json-patch+json")]
    [RequestContentTypeRestriction(AllowJson = false)]
    [ClientLocationId("B5F6BB4F-8D1E-4D79-8D11-4C9172C99C35")]
    [ClientResponseType(typeof (void), null, null)]
    [ClientExample("PATCH_git_pullRequestStatuses_statuses.json", null, null, null)]
    public HttpResponseMessage UpdatePullRequestStatuses(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      [ClientParameterType(typeof (JsonPatchDocument), false)] PatchDocument<IDictionary<string, object>> patchDocument,
      [ClientIgnore] string projectId = null)
    {
      string repositoryId1 = repositoryId;
      int pullRequestId1 = pullRequestId;
      PatchDocument<IDictionary<string, object>> patchDocument1 = patchDocument;
      string str = projectId;
      int? iterationId = new int?();
      string projectId1 = str;
      this.UpdatePullRequestStatusesInternal(repositoryId1, pullRequestId1, patchDocument1, iterationId, projectId1);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    [HttpDelete]
    [ClientLocationId("B5F6BB4F-8D1E-4D79-8D11-4C9172C99C35")]
    [ClientResponseType(typeof (void), null, null)]
    [ClientExample("DELETE_git_pullRequestStatuses_status.json", null, null, null)]
    public HttpResponseMessage DeletePullRequestStatus(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int statusId,
      [ClientIgnore] string projectId = null)
    {
      string repositoryId1 = repositoryId;
      int pullRequestId1 = pullRequestId;
      int statusId1 = statusId;
      string str = projectId;
      int? iterationId = new int?();
      string projectId1 = str;
      this.DeletePullRequestStatusInternal(repositoryId1, pullRequestId1, statusId1, iterationId, projectId1);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }
  }
}
