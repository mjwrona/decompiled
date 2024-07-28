// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitMergeController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [FeatureEnabled("Git.Merges")]
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "merges")]
  public class GitMergeController : GitApiController
  {
    [HttpPost]
    [ClientExample("CreateMergeRequestAsync.json", "Create a merge request.", null, null)]
    [ClientLocationId("985F7AE9-844F-4906-9897-7EF41516C0E2")]
    [ClientResponseType(typeof (GitMerge), null, null)]
    public HttpResponseMessage CreateMergeRequest(
      [ClientParameterType(typeof (Guid), true)] string repositoryNameOrId,
      [FromBody] GitMergeParameters mergeParameters,
      bool includeLinks = false)
    {
      RepoKey key;
      this.ValidateInput(mergeParameters, repositoryNameOrId, out key);
      GitMergeAsyncOp mergeRequest = this.TfsRequestContext.GetService<IGitMergeService>().CreateMergeRequest(this.TfsRequestContext, key, mergeParameters);
      return this.Request.CreateResponse<GitMerge>(HttpStatusCode.Created, this.ToWebApiMergeRequest(key, mergeRequest, includeLinks));
    }

    [HttpGet]
    [ClientExample("GetMergeRequestAsync.json", "Get the details of the merge request.", null, null)]
    [ClientLocationId("985F7AE9-844F-4906-9897-7EF41516C0E2")]
    [ClientResponseType(typeof (GitMerge), null, null)]
    public HttpResponseMessage GetMergeRequest(
      [ClientParameterType(typeof (Guid), true)] string repositoryNameOrId,
      int mergeOperationId,
      bool includeLinks = false)
    {
      RepoKey repoKey = this.ResolveRepoKey(repositoryNameOrId);
      GitMergeAsyncOp mergeRequestById = this.TfsRequestContext.GetService<IGitMergeService>().GetMergeRequestById(this.TfsRequestContext, repoKey, mergeOperationId);
      return this.Request.CreateResponse<GitMerge>(HttpStatusCode.OK, this.ToWebApiMergeRequest(repoKey, mergeRequestById, includeLinks));
    }

    private GitMerge ToWebApiMergeRequest(
      RepoKey repoKey,
      GitMergeAsyncOp mergeAsyncOp,
      bool includeLinks)
    {
      ReferenceLinks referenceLinks = (ReferenceLinks) null;
      if (includeLinks)
        referenceLinks = GitReferenceLinksUtility.GetBaseReferenceLinks(this.TfsRequestContext, this.TfsRequestContext.GetService<ILocationService>().GetResourceUri(this.TfsRequestContext, "git", GitWebApiConstants.MergeLocationId, (object) new
        {
          repositoryNameOrId = mergeAsyncOp.RepositoryId,
          mergeOperationId = mergeAsyncOp.OperationId
        }).AbsoluteUri, repoKey);
      GitMerge webApiMergeRequest = new GitMerge();
      webApiMergeRequest.OperationId = mergeAsyncOp.OperationId;
      webApiMergeRequest.Status = mergeAsyncOp.Status;
      webApiMergeRequest.DetailedStatus = mergeAsyncOp.DetailedStatus;
      webApiMergeRequest.Links = referenceLinks;
      webApiMergeRequest.Comment = mergeAsyncOp.Parameters.Comment;
      webApiMergeRequest.Parents = mergeAsyncOp.Parameters.Parents;
      return webApiMergeRequest;
    }

    private void ValidateInput(
      GitMergeParameters mergeParameters,
      string repositoryNameOrId,
      out RepoKey key)
    {
      ArgumentUtility.CheckForNull<GitMergeParameters>(mergeParameters, nameof (mergeParameters), "git");
      ArgumentUtility.CheckForNull<string>(mergeParameters.Comment, "Comment", "git");
      List<string> parents = mergeParameters.Parents;
      ArgumentUtility.CheckForNull<List<string>>(parents, "Parents", "git");
      if (parents.Count != 2)
        throw new ArgumentException(Resources.Get("UnsupportedNumberOfCommitsForMerge")).Expected("git");
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryNameOrId, this.ProjectInfo?.Id.ToString()))
      {
        key = tfsGitRepository.Key;
        tfsGitRepository.LookupObject<TfsGitCommit>(GitCommitUtility.ParseSha1Id(parents[0]));
        tfsGitRepository.LookupObject<TfsGitCommit>(GitCommitUtility.ParseSha1Id(parents[1]));
      }
    }
  }
}
