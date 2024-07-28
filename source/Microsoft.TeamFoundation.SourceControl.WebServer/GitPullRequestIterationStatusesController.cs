// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPullRequestIterationStatusesController
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
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class GitPullRequestIterationStatusesController : GitPullRequestStatusesBaseController
  {
    [HttpGet]
    [ClientResponseType(typeof (List<GitPullRequestStatus>), null, null)]
    [ClientLocationId("75CF11C5-979F-4038-A76E-058A06ADF2BF")]
    [ClientExample("GET_git_pullRequestStatuses_iterationStatuses.json", null, null, null)]
    public virtual HttpResponseMessage GetPullRequestIterationStatuses(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int iterationId,
      [ClientIgnore] string projectId = null)
    {
      IEnumerable<GitPullRequestStatus> statusesInternal = this.GetPullRequestIterationStatusesInternal(repositoryId, pullRequestId, iterationId, projectId);
      GitStatusStateMapper.MapGitEntity<IEnumerable<GitPullRequestStatus>>(statusesInternal, this.TfsRequestContext);
      return this.GenerateResponse<GitPullRequestStatus>(statusesInternal);
    }

    [HttpGet]
    [ClientResponseType(typeof (GitPullRequestStatus), null, null)]
    [ClientLocationId("75CF11C5-979F-4038-A76E-058A06ADF2BF")]
    [ClientExample("GET_git_pullRequestStatuses_iterationStatus.json", null, null, null)]
    public virtual HttpResponseMessage GetPullRequestIterationStatus(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int iterationId,
      int statusId,
      [ClientIgnore] string projectId = null)
    {
      GitPullRequestStatus iterationStatusInternal = this.GetPullRequestIterationStatusInternal(repositoryId, pullRequestId, iterationId, statusId, projectId);
      GitStatusStateMapper.MapGitEntity<GitPullRequestStatus>(iterationStatusInternal, this.TfsRequestContext);
      return this.Request.CreateResponse<GitPullRequestStatus>(HttpStatusCode.OK, iterationStatusInternal);
    }

    [HttpPost]
    [ClientResponseType(typeof (GitPullRequestStatus), null, null)]
    [ClientLocationId("75CF11C5-979F-4038-A76E-058A06ADF2BF")]
    [ClientExample("POST_git_pullRequestStatuses_iterationStatus.json", null, null, null)]
    public virtual HttpResponseMessage CreatePullRequestIterationStatus(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int iterationId,
      GitPullRequestStatus status,
      [ClientIgnore] string projectId = null)
    {
      return this.CreatePullRequestStatusInternal(repositoryId, pullRequestId, status, new int?(iterationId), projectId);
    }

    [HttpPatch]
    [ClientRequestContentMediaType("application/json-patch+json")]
    [RequestContentTypeRestriction(AllowJson = false)]
    [ClientLocationId("75CF11C5-979F-4038-A76E-058A06ADF2BF")]
    [ClientResponseType(typeof (void), null, null)]
    [ClientExample("PATCH_git_pullRequestStatuses_iterationStatuses.json", null, null, null)]
    public virtual HttpResponseMessage UpdatePullRequestIterationStatuses(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int iterationId,
      [ClientParameterType(typeof (JsonPatchDocument), false)] PatchDocument<IDictionary<string, object>> patchDocument,
      [ClientIgnore] string projectId = null)
    {
      this.UpdatePullRequestStatusesInternal(repositoryId, pullRequestId, patchDocument, new int?(iterationId), projectId);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    [HttpDelete]
    [ClientLocationId("75CF11C5-979F-4038-A76E-058A06ADF2BF")]
    [ClientResponseType(typeof (void), null, null)]
    [ClientExample("DELETE_git_pullRequestStatuses_iterationStatus.json", null, null, null)]
    public virtual HttpResponseMessage DeletePullRequestIterationStatus(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int iterationId,
      int statusId,
      [ClientIgnore] string projectId = null)
    {
      string repositoryId1 = repositoryId;
      int pullRequestId1 = pullRequestId;
      int statusId1 = statusId;
      string str = projectId;
      int? iterationId1 = new int?();
      string projectId1 = str;
      this.DeletePullRequestStatusInternal(repositoryId1, pullRequestId1, statusId1, iterationId1, projectId1);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }
  }
}
