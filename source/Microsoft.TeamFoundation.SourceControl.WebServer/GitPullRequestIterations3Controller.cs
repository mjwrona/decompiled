// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPullRequestIterations3Controller
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ControllerApiVersion(7.1)]
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "pullRequestIterations", ResourceVersion = 2)]
  public class GitPullRequestIterations3Controller : GitPullRequestIterations2Controller
  {
    protected static readonly IEnumerable<IterationReason> s_version3SupportedReasons = (IEnumerable<IterationReason>) GitPullRequestIterations2Controller.s_version2SupportedReasons.Union<IterationReason>((IEnumerable<IterationReason>) new List<IterationReason>()
    {
      IterationReason.ResolveConflicts
    }).ToList<IterationReason>();

    [HttpGet]
    [ClientResponseType(typeof (List<GitPullRequestIteration>), null, null)]
    [ClientLocationId("D43911EE-6958-46B0-A42B-8445B8A0D004")]
    public override HttpResponseMessage GetPullRequestIterations(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      [ClientIgnore] string projectId = null,
      [FromUri(Name = "includeCommits")] bool includeCommits = false)
    {
      return this.GenerateResponse<GitPullRequestIteration>((IEnumerable<GitPullRequestIteration>) this.GetPullRequestIterationsInternal(repositoryId, pullRequestId, projectId, includeCommits, GitPullRequestIterations3Controller.s_version3SupportedReasons));
    }

    [HttpGet]
    [ClientResponseType(typeof (GitPullRequestIteration), null, null)]
    [ClientLocationId("D43911EE-6958-46B0-A42B-8445B8A0D004")]
    public override HttpResponseMessage GetPullRequestIteration(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int iterationId,
      [ClientIgnore] string projectId = null)
    {
      return this.Request.CreateResponse<GitPullRequestIteration>(HttpStatusCode.OK, this.GetPullRequestIterationInternal(repositoryId, pullRequestId, iterationId, projectId, GitPullRequestIterations3Controller.s_version3SupportedReasons));
    }
  }
}
