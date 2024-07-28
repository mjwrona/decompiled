// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPullRequestIterationStatuses2Controller
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
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "PullRequestIterationStatuses", ResourceVersion = 2)]
  public class GitPullRequestIterationStatuses2Controller : GitPullRequestIterationStatusesController
  {
    [HttpGet]
    [ClientResponseType(typeof (List<GitPullRequestStatus>), null, null)]
    [ClientLocationId("75CF11C5-979F-4038-A76E-058A06ADF2BF")]
    [ClientExample("GET_git_pullRequestStatuses_iterationStatuses.json", null, null, null)]
    public override HttpResponseMessage GetPullRequestIterationStatuses(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int iterationId,
      [ClientIgnore] string projectId = null)
    {
      return this.GenerateResponse<GitPullRequestStatus>(this.GetPullRequestIterationStatusesInternal(repositoryId, pullRequestId, iterationId, projectId));
    }

    [HttpGet]
    [ClientResponseType(typeof (GitPullRequestStatus), null, null)]
    [ClientLocationId("75CF11C5-979F-4038-A76E-058A06ADF2BF")]
    [ClientExample("GET_git_pullRequestStatuses_iterationStatus.json", null, null, null)]
    public override HttpResponseMessage GetPullRequestIterationStatus(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int iterationId,
      int statusId,
      [ClientIgnore] string projectId = null)
    {
      return this.Request.CreateResponse<GitPullRequestStatus>(HttpStatusCode.OK, this.GetPullRequestIterationStatusInternal(repositoryId, pullRequestId, iterationId, statusId, projectId));
    }
  }
}
