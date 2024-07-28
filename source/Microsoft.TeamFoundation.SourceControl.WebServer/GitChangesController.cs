// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitChangesController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ClientGroupByResource("Commits")]
  public class GitChangesController : GitApiController
  {
    private const int c_defaultTop = 1000;

    [HttpGet]
    [ClientResponseType(typeof (GitCommitChanges), null, null)]
    [ClientLocationId("5BF884F5-3E07-42E9-AFB8-1B872267BF16")]
    [ClientExample("GET__git_repositories__repositoryId__commits__commitId__changes_top-_top__skip-_skip_.json", "With changes", null, null)]
    public HttpResponseMessage GetChanges(
      string commitId,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null,
      int top = 1000,
      int skip = 0)
    {
      top = Math.Max(top, 1);
      skip = Math.Max(skip, 0);
      Sha1Id sha1Id = GitCommitUtility.ParseSha1Id(commitId);
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        GitVersionParser.GetCommitById(tfsGitRepository, sha1Id);
        CommitMetadataAndChanges commitManifest = this.TfsRequestContext.GetService<ITeamFoundationGitCommitService>().GetCommitManifest(this.TfsRequestContext, tfsGitRepository, sha1Id);
        if (commitManifest == null)
          throw new GitCommitDoesNotExistException(sha1Id);
        bool allChangesIncluded = true;
        HttpResponseMessage response = this.Request.CreateResponse<GitCommitChanges>(HttpStatusCode.OK, commitManifest.Changes.ToGitCommitChanges(this.TfsRequestContext, tfsGitRepository.Key, top, skip, (ISecuredObject) null, out allChangesIncluded));
        if (!allChangesIncluded)
        {
          string empty = string.Empty;
          response.Headers.Add("Link", empty);
        }
        return response;
      }
    }
  }
}
