// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitForkSyncRequestsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Git.Server.Forks;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ClientGroupByResource("Forks")]
  [VersionedApiControllerCustomName(Area = "git", ResourceName = "forkSyncRequests")]
  public class GitForkSyncRequestsController : GitApiController
  {
    [HttpGet]
    [ClientLocationId("1703F858-B9D1-46AF-AB62-483E9E1055B5")]
    [ClientResponseType(typeof (IEnumerable<GitForkSyncRequest>), null, null)]
    public HttpResponseMessage GetForkSyncRequests(
      [ClientParameterType(typeof (Guid), true)] string repositoryNameOrId,
      bool includeAbandoned = false,
      bool includeLinks = false)
    {
      RepoKey repoKey = this.ResolveRepoKey(repositoryNameOrId);
      return this.GenerateResponse<GitForkSyncRequest>(this.TfsRequestContext.GetService<IGitForkService>().QueryFetchRequests(this.TfsRequestContext, repoKey, includeAbandoned).Select<ForkFetchAsyncOp, GitForkSyncRequest>((Func<ForkFetchAsyncOp, GitForkSyncRequest>) (fo => this.ToWebApiForkSyncRequest(repoKey, fo, includeLinks))));
    }

    [HttpGet]
    [ClientLocationId("1703F858-B9D1-46AF-AB62-483E9E1055B5")]
    [ClientResponseType(typeof (GitForkSyncRequest), null, null)]
    [ClientSwaggerOperationId("Get fork sync request")]
    public HttpResponseMessage GetForkSyncRequest(
      [ClientParameterType(typeof (Guid), true)] string repositoryNameOrId,
      int forkSyncOperationId,
      bool includeLinks = false)
    {
      RepoKey repoKey = this.ResolveRepoKey(repositoryNameOrId);
      ForkFetchAsyncOp fetchRequestById = this.TfsRequestContext.GetService<IGitForkService>().GetFetchRequestById(this.TfsRequestContext, repoKey, forkSyncOperationId);
      return this.Request.CreateResponse<GitForkSyncRequest>(HttpStatusCode.OK, this.ToWebApiForkSyncRequest(repoKey, fetchRequestById, includeLinks));
    }

    [HttpPost]
    [ClientLocationId("1703F858-B9D1-46AF-AB62-483E9E1055B5")]
    [ClientResponseType(typeof (GitForkSyncRequest), null, null)]
    [ClientSwaggerOperationId("Create fork sync request")]
    public HttpResponseMessage CreateForkSyncRequest(
      [ClientParameterType(typeof (Guid), true)] string repositoryNameOrId,
      GitForkSyncRequestParameters syncParams,
      bool includeLinks = false)
    {
      ArgumentUtility.CheckForNull<GitForkSyncRequestParameters>(syncParams, nameof (syncParams), "git");
      ArgumentUtility.CheckForNull<GlobalGitRepositoryKey>(syncParams.Source, "Source", "git");
      RepoKey repoKey = this.ResolveRepoKey(repositoryNameOrId);
      IGitForkService service = this.TfsRequestContext.GetService<IGitForkService>();
      return this.Request.CreateResponse<GitForkSyncRequest>(HttpStatusCode.Created, this.ToWebApiForkSyncRequest(repoKey, service.SyncFork(this.TfsRequestContext, syncParams, repoKey), includeLinks));
    }

    private GitForkSyncRequest ToWebApiForkSyncRequest(
      RepoKey repoKey,
      ForkFetchAsyncOp fetchOp,
      bool includeLinks)
    {
      ReferenceLinks referenceLinks = (ReferenceLinks) null;
      if (includeLinks)
        referenceLinks = GitReferenceLinksUtility.GetBaseReferenceLinks(this.TfsRequestContext, this.TfsRequestContext.GetService<ILocationService>().GetResourceUri(this.TfsRequestContext, "git", GitWebApiConstants.ForkSyncRequestsLocationId, (object) new
        {
          forkSyncOperationId = fetchOp.OperationId,
          repositoryNameOrId = fetchOp.RepositoryId
        }).AbsoluteUri, repoKey);
      return new GitForkSyncRequest()
      {
        OperationId = fetchOp.OperationId,
        Source = fetchOp.Parameters.Source,
        SourceToTargetRefs = fetchOp.Parameters.SourceToTargetRefs,
        Status = fetchOp.Status,
        DetailedStatus = fetchOp.DetailedStatus,
        Links = referenceLinks
      };
    }
  }
}
