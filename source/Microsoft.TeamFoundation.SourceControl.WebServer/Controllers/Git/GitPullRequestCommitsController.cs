// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.Controllers.Git.GitPullRequestCommitsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer.Controllers.Git
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class GitPullRequestCommitsController : GitApiController
  {
    private const int c_defaultTopCommits = 100;
    private const int c_maxTopCommits = 500;

    [HttpGet]
    [ClientLocationId("52823034-34a8-4576-922c-8d8b77e9e4c4")]
    [ClientResponseType(typeof (IPagedList<GitCommitRef>), null, null)]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetPullRequestCommits(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientParameterType(typeof (int), false)] string pullRequestId,
      [ClientIgnore] string projectId = null,
      [ClientInclude(RestClientLanguages.Swagger2 | RestClientLanguages.Python), FromUri(Name = "$top")] int? top = null,
      [ClientInclude(RestClientLanguages.Swagger2 | RestClientLanguages.Python)] string continuationToken = null)
    {
      int result;
      if (string.IsNullOrEmpty(pullRequestId) || !int.TryParse(pullRequestId, out result))
        throw new InvalidArgumentValueException(Microsoft.TeamFoundation.SourceControl.WebServer.Resources.Get("InvalidPullRequestId"));
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        TfsGitPullRequest pullRequest;
        this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>().TryGetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, result, out pullRequest);
        if (pullRequest == null)
          throw new GitPullRequestNotFoundException();
        PullRequestCommitsContinuationToken token = (PullRequestCommitsContinuationToken) null;
        PullRequestCommitsContinuationToken currentToken = (PullRequestCommitsContinuationToken) null;
        if (continuationToken != null && !PullRequestCommitsContinuationToken.TryParseContinuationToken(continuationToken, out token))
          throw new ArgumentException(Microsoft.TeamFoundation.SourceControl.WebServer.Resources.Format("InvalidContinuationToken", (object) continuationToken)).Expected("git");
        int top1 = Math.Max(0, top ?? 100);
        HttpResponseMessage response = this.Request.CreateResponse<GitCommitRef[]>(HttpStatusCode.OK, pullRequest.GenerateCommitsWithContinuationToken(this.TfsRequestContext, tfsGitRepository, top1, token, out currentToken));
        if (currentToken != null)
          response.Headers.Add("x-ms-continuationtoken", currentToken.ToString());
        return response;
      }
    }

    [HttpGet]
    [ClientLocationId("E7EA0883-095F-4926-B5FB-F24691C26FB9")]
    [ClientResponseType(typeof (IList<GitCommitRef>), null, null)]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetPullRequestIterationCommits(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      int iterationId,
      [ClientIgnore] string projectId = null,
      int top = 100,
      int skip = 0)
    {
      int top1 = Math.Min(500, Math.Max(0, top));
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
        TfsGitPullRequest pullRequest;
        service.TryGetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId, out pullRequest);
        if (pullRequest == null)
          throw new GitPullRequestNotFoundException();
        GitPullRequestIteration iteration = service.GetIteration(this.TfsRequestContext, tfsGitRepository, pullRequest, iterationId);
        return this.GenerateResponse<GitCommitRef>((IEnumerable<GitCommitRef>) pullRequest.GenerateCommits(this.TfsRequestContext, tfsGitRepository, top1, skip, iteration?.SourceRefCommit?.CommitId, iteration?.TargetRefCommit?.CommitId), GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key));
      }
    }
  }
}
