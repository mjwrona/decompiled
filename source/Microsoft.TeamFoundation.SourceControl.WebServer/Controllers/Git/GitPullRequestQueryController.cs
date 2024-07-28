// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.Controllers.Git.GitPullRequestQueryController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer.Controllers.Git
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class GitPullRequestQueryController : GitApiController
  {
    private const int c_maxItems = 1000;
    private const int c_maxQueries = 10;

    [HttpPost]
    [ClientResponseType(typeof (GitPullRequestQuery), null, null)]
    [ClientLocationId("B3A6EEBE-9CF0-49EA-B6CB-1A4C5F5007B0")]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetPullRequestQuery(
      GitPullRequestQuery queries,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientIgnore] string projectId = null)
    {
      if (queries == null || queries.QueryInputs == null || !queries.QueryInputs.Any<GitPullRequestQueryInput>())
        throw new InvalidArgumentValueException(nameof (queries));
      if (queries.QueryInputs.Count == 0 || queries.QueryInputs.Count > 10)
        throw new InvalidArgumentValueException(nameof (queries), Microsoft.TeamFoundation.SourceControl.WebServer.Resources.Format("InvalidPullRequestQueryTooManyQueries", (object) 10));
      foreach (GitPullRequestQueryInput queryInput in queries.QueryInputs)
      {
        if (queryInput.Items == null)
          throw new InvalidArgumentValueException("items");
        if (queryInput.Type == GitPullRequestQueryType.NotSet)
          throw new InvalidArgumentValueException("type", Microsoft.TeamFoundation.SourceControl.WebServer.Resources.Format("InvalidPullRequestQueryType", (object) queryInput.Type.ToString()));
        if (queryInput.Items.Count > 1000)
          throw new InvalidArgumentValueException("items", Microsoft.TeamFoundation.SourceControl.WebServer.Resources.Format("InvalidPullRequestQueryTooManyItems", (object) "items", (object) 1000));
      }
      RepoKey repoKey;
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
        repoKey = tfsGitRepository.Key;
      queries.Results = new List<IDictionary<string, List<GitPullRequest>>>();
      IDictionary<Guid, TeamFoundationIdentity> guidToIdentity = (IDictionary<Guid, TeamFoundationIdentity>) new Dictionary<Guid, TeamFoundationIdentity>();
      ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
      ISecuredObject securedObject = GitSecuredObjectFactory.CreateRepositoryReadOnly(repoKey);
      queries.SetSecuredObject(securedObject);
      foreach (GitPullRequestQueryInput queryInput in queries.QueryInputs)
      {
        ILookup<Sha1Id, TfsGitPullRequest> source = (ILookup<Sha1Id, TfsGitPullRequest>) null;
        if (queryInput.Type == GitPullRequestQueryType.LastMergeCommit)
          source = service.QueryPullRequestsByMergeCommits(this.TfsRequestContext, queryInput.Items.Select<string, Sha1Id>((Func<string, Sha1Id>) (commitId => GitCommitUtility.ParseSha1Id(commitId))).Distinct<Sha1Id>(), repoKey);
        else if (queryInput.Type == GitPullRequestQueryType.Commit)
          source = service.QueryPullRequestsByCommits(this.TfsRequestContext, queryInput.Items.Select<string, Sha1Id>((Func<string, Sha1Id>) (commitId => GitCommitUtility.ParseSha1Id(commitId))).Distinct<Sha1Id>(), repoKey);
        IdentityLookupHelper.LoadIdentities(this.TfsRequestContext, source.SelectMany<IGrouping<Sha1Id, TfsGitPullRequest>, TfsGitPullRequest>((Func<IGrouping<Sha1Id, TfsGitPullRequest>, IEnumerable<TfsGitPullRequest>>) (x => (IEnumerable<TfsGitPullRequest>) x)).Select<TfsGitPullRequest, Guid>((Func<TfsGitPullRequest, Guid>) (pr => pr.Creator)), guidToIdentity);
        IDictionary<string, List<GitPullRequest>> securedDictionary = source.Select(prs => new
        {
          Key = prs.Key.ToString(),
          Value = prs.Select<TfsGitPullRequest, GitPullRequest>((Func<TfsGitPullRequest, GitPullRequest>) (pr => pr.ToShallowWebApiItem(this.TfsRequestContext, repoKey, guidToIdentity, securedObject))).Where<GitPullRequest>((Func<GitPullRequest, bool>) (pr => pr != null)).ToList<GitPullRequest>()
        }).ToSecuredDictionary(securedObject, prs => prs.Key, prs => prs.Value);
        queries.Results.Add(securedDictionary);
      }
      return this.Request.CreateResponse<GitPullRequestQuery>(HttpStatusCode.Created, queries);
    }
  }
}
