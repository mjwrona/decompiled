// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitAsyncOperationModelExtensions
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class GitAsyncOperationModelExtensions
  {
    public static GitCherryPick ToWebApiCherryPickItem(
      this Microsoft.TeamFoundation.Git.Server.GitAsyncRefOperation operation,
      ITfsGitRepository repo,
      IVssRequestContext rc,
      UrlHelper urlHelper,
      bool includeLinks = false)
    {
      if (operation.Type != GitAsyncOperationType.CherryPick)
        throw new GitAsyncRefOperationNotFoundException("cherry-pick", operation.OperationId);
      GitCherryPick operation1 = new GitCherryPick();
      operation1.CherryPickId = operation.OperationId;
      operation1.Parameters = operation.Parameters.ToWebApiItem(rc, repo);
      operation1.Status = operation.Status;
      operation1.DetailedStatus = operation.DetailedStatus;
      operation1.Url = urlHelper.RestLink(rc, GitWebApiConstants.CherryPickLocationId, (object) new
      {
        project = repo.Key.ProjectId,
        repositoryId = repo.Key.RepoId,
        cherryPickId = operation.OperationId
      });
      operation1.Links = operation1.GetCherryPickOperationReferenceLinks(rc, urlHelper, repo.Key);
      return operation1;
    }

    public static Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest ToWebApiImportItem(
      this Microsoft.TeamFoundation.Git.Server.GitImportRequest importRequest,
      ITfsGitRepository repo,
      IVssRequestContext rc,
      UrlHelper urlHelper,
      bool includeLinks = false)
    {
      Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest webApiImportItem = new Microsoft.TeamFoundation.SourceControl.WebApi.GitImportRequest()
      {
        ImportRequestId = importRequest.OperationId,
        Parameters = new GitImportRequestParameters((GitImportRequestParameters) importRequest.Parameters),
        Status = importRequest.Status,
        DetailedStatus = importRequest.DetailedStatus,
        Repository = repo.ToWebApiItem(rc)
      };
      webApiImportItem.Parameters.ServiceEndpointId = Guid.Empty;
      if (urlHelper != null)
        webApiImportItem.Url = urlHelper.RestLink(rc, GitWebApiConstants.ImportRequestsLocationId, (object) new
        {
          repositoryId = repo.Key.RepoId,
          importRequestId = importRequest.OperationId
        });
      if (includeLinks)
        webApiImportItem.Links = GitReferenceLinksUtility.GetBaseReferenceLinks(rc, webApiImportItem.Url, repo.Key);
      return webApiImportItem;
    }

    public static GitRevert ToWebApiRevertItem(
      this Microsoft.TeamFoundation.Git.Server.GitAsyncRefOperation operation,
      ITfsGitRepository repo,
      IVssRequestContext rc,
      UrlHelper urlHelper,
      bool includeLinks = false)
    {
      if (operation.Type != GitAsyncOperationType.Revert)
        throw new GitAsyncRefOperationNotFoundException("revert", operation.OperationId);
      GitRevert operation1 = new GitRevert();
      operation1.RevertId = operation.OperationId;
      operation1.Parameters = operation.Parameters.ToWebApiItem(rc, repo);
      operation1.Status = operation.Status;
      operation1.DetailedStatus = operation.DetailedStatus;
      operation1.Url = urlHelper.RestLink(rc, GitWebApiConstants.RevertLocationId, (object) new
      {
        repositoryId = repo.Key.RepoId,
        revertId = operation.OperationId
      });
      operation1.Links = operation1.GetRevertOperationReferenceLinks(rc, urlHelper, repo.Key);
      return operation1;
    }

    public static Microsoft.TeamFoundation.SourceControl.WebApi.GitAsyncRefOperationSource ToWebApiItem(
      this Microsoft.TeamFoundation.Git.Server.GitAsyncRefOperationSource source,
      IVssRequestContext requestContext,
      RepoKey repoKey)
    {
      Microsoft.TeamFoundation.SourceControl.WebApi.GitAsyncRefOperationSource webApiItem = new Microsoft.TeamFoundation.SourceControl.WebApi.GitAsyncRefOperationSource();
      webApiItem.PullRequestId = source.PullRequestId;
      Sha1Id[] commitList = source.CommitList;
      webApiItem.CommitList = commitList != null ? ((IEnumerable<Sha1Id>) commitList).Select<Sha1Id, GitCommitRef>((Func<Sha1Id, GitCommitRef>) (commitId => GitCommitUtility.CreateMinimalGitCommitRef(requestContext, commitId, repoKey))).ToArray<GitCommitRef>() : (GitCommitRef[]) null;
      return webApiItem;
    }

    public static Microsoft.TeamFoundation.Git.Server.GitAsyncRefOperationSource ToServerItem(
      this Microsoft.TeamFoundation.SourceControl.WebApi.GitAsyncRefOperationSource source)
    {
      if (source == null)
        return (Microsoft.TeamFoundation.Git.Server.GitAsyncRefOperationSource) null;
      int? pullRequestId = source.PullRequestId;
      if (pullRequestId.HasValue)
      {
        pullRequestId = source.PullRequestId;
        return new Microsoft.TeamFoundation.Git.Server.GitAsyncRefOperationSource(pullRequestId.Value);
      }
      GitCommitRef[] commitList = source.CommitList;
      return new Microsoft.TeamFoundation.Git.Server.GitAsyncRefOperationSource(commitList != null ? ((IEnumerable<GitCommitRef>) commitList).Select<GitCommitRef, Sha1Id>((Func<GitCommitRef, Sha1Id>) (commitRef => new Sha1Id(commitRef.CommitId))).ToArray<Sha1Id>() : (Sha1Id[]) null);
    }

    public static Microsoft.TeamFoundation.SourceControl.WebApi.GitAsyncRefOperationParameters ToWebApiItem(
      this Microsoft.TeamFoundation.Git.Server.GitAsyncRefOperationParameters parameters,
      IVssRequestContext requestContext,
      ITfsGitRepository repo)
    {
      return new Microsoft.TeamFoundation.SourceControl.WebApi.GitAsyncRefOperationParameters()
      {
        Repository = repo.ToWebApiItem(requestContext),
        OntoRefName = parameters.OntoRefName,
        GeneratedRefName = parameters.GeneratedRefName,
        Source = parameters.Source.ToWebApiItem(requestContext, repo.Key)
      };
    }

    public static Microsoft.TeamFoundation.Git.Server.GitAsyncRefOperationParameters ToServerItem(
      this Microsoft.TeamFoundation.SourceControl.WebApi.GitAsyncRefOperationParameters parameters)
    {
      return new Microsoft.TeamFoundation.Git.Server.GitAsyncRefOperationParameters()
      {
        OntoRefName = parameters.OntoRefName,
        GeneratedRefName = parameters.GeneratedRefName,
        Source = parameters.Source.ToServerItem()
      };
    }
  }
}
