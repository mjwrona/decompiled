// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.GitHubConnectionController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [ValidateModel]
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "githubConnections", ResourceVersion = 1)]
  [ControllerApiVersion(7.1)]
  [ClientInternalUseOnly(true)]
  public class GitHubConnectionController : WorkItemTrackingApiController
  {
    private const string providerKey = "github.com";
    private const int TraceRange = 5904000;

    public override string TraceArea => "githubConnections";

    protected override bool ControllerSupportsIdentityRefForWorkItemFieldValues() => false;

    protected override bool ControllerSupportsProjectScopedUrls() => false;

    [TraceFilter(5904000, 5904020)]
    [HttpGet]
    [ClientLocationId("0cf95f86-6ce1-f410-ccf6-3d8c92b3a1ef")]
    [ClientExample("GET_githubConnections.json", "Get github connections list", null, null)]
    public IEnumerable<GitHubConnectionModel> GetGithubConnections()
    {
      if (this.ProjectInfo == null)
        throw new VssPropertyValidationException("Project", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.ProjectNotFound());
      IExternalConnectionService service = this.TfsRequestContext.GetService<IExternalConnectionService>();
      if (!service.HasPermission(this.TfsRequestContext, 1, new Guid?(this.ProjectInfo.Id)))
        throw new PermissionDeniedException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.ExternalConnectionNoReadPermissions());
      IEnumerable<GitHubConnectionModel> githubConnections = Enumerable.Empty<GitHubConnectionModel>();
      IReadOnlyCollection<ExternalConnection> externalConnections = service.GetExternalConnections(this.TfsRequestContext, new Guid?(this.ProjectInfo.Id), (string) null, true, includeInvalidConnections: true);
      if (externalConnections != null)
      {
        Dictionary<Guid, IdentityRef> identityRefMap = this.GetCreatorsIdentity(externalConnections.Select<ExternalConnection, Guid>((Func<ExternalConnection, Guid>) (c => c.CreatedBy)).ToList<Guid>());
        githubConnections = externalConnections.Select<ExternalConnection, GitHubConnectionModel>((Func<ExternalConnection, GitHubConnectionModel>) (ec =>
        {
          IdentityRef identityRef;
          if (!identityRefMap.TryGetValue(ec.CreatedBy, out identityRef))
            identityRef = (IdentityRef) null;
          return new GitHubConnectionModel()
          {
            Id = ec.Id,
            Name = ec.Name,
            AuthorizationType = ec.ServiceEndpoint.AuthorizationScheme,
            CreatedBy = identityRef,
            IsConnectionValid = new bool?(ec.IsConnectionValid)
          };
        }));
      }
      return githubConnections;
    }

    [TraceFilter(5904000, 5904020)]
    [HttpGet]
    [ClientLocationId("0b3a5212-f65b-2102-0d80-1dd77ce4c700")]
    [ClientExample("GET_githubConnections_repos.json", "Get github connection repos list", null, null)]
    public IEnumerable<GitHubConnectionRepoModel> GetGithubConnectionRepositories([FromUri] Guid connectionId)
    {
      if (this.ProjectInfo == null)
        throw new VssPropertyValidationException("Project", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.ProjectNotFound());
      IExternalConnectionService service = this.TfsRequestContext.GetService<IExternalConnectionService>();
      if (!service.HasPermission(this.TfsRequestContext, 1, new Guid?(this.ProjectInfo.Id)))
        throw new PermissionDeniedException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.ExternalConnectionNoReadPermissions());
      IEnumerable<ExternalGitRepo> externalConnectionRepos = service.GetExternalConnectionRepos(this.TfsRequestContext, this.ProjectInfo.Id, "github.com", connectionId, (IEnumerable<string>) null);
      IEnumerable<GitHubConnectionRepoModel> connectionRepositories = Enumerable.Empty<GitHubConnectionRepoModel>();
      if (externalConnectionRepos.Any<ExternalGitRepo>())
        connectionRepositories = externalConnectionRepos.Select<ExternalGitRepo, GitHubConnectionRepoModel>((Func<ExternalGitRepo, GitHubConnectionRepoModel>) (r => new GitHubConnectionRepoModel()
        {
          GitHubRepositoryUrl = r.WebUrl
        }));
      return connectionRepositories;
    }

    [TraceFilter(5904000, 5904020)]
    [HttpPost]
    [ClientLocationId("15b19676-8d9e-e224-d795-ca4d1a18024d")]
    [ClientExample("POST_githubConnections_reposBatch_add.json", "Add list of repos to the specified github connection", null, null)]
    [ClientExample("POST_githubConnections_reposBatch_remove.json", "Remove list of repos from the specified github connection", null, null)]
    public IEnumerable<GitHubConnectionRepoModel> UpdateGithubConnectionRepos(
      [FromUri] Guid connectionId,
      [FromBody] GitHubConnectionReposBatchRequest reposOperationData)
    {
      if (this.ProjectInfo == null)
        throw new VssPropertyValidationException("Project", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.ProjectNotFound());
      this.ValidateData(reposOperationData);
      this.EvaluateLimits(reposOperationData.GitHubRepositoryUrls.Count<GitHubConnectionRepoModel>());
      IExternalConnectionService service = this.TfsRequestContext.GetService<IExternalConnectionService>();
      if (!service.HasPermission(this.TfsRequestContext, 2, new Guid?(this.ProjectInfo.Id)))
        throw new PermissionDeniedException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.ExternalConnectionNoWritePermissions());
      Dictionary<string, string> repositoriesNameToUrlMap = this.ExtractOrganizationNamesAndRepoNames(reposOperationData.GitHubRepositoryUrls.Select<GitHubConnectionRepoModel, string>((Func<GitHubConnectionRepoModel, string>) (r => r.GitHubRepositoryUrl)));
      ExternalConnectionProvisionResult connectionProvisionResult = (ExternalConnectionProvisionResult) null;
      string normalizedOperationValue = GitHubConnectionRepoOperation.GetNormalizedOperationValue(reposOperationData.OperationType);
      if (normalizedOperationValue == GitHubConnectionRepoOperation.Add)
        connectionProvisionResult = service.AddExternalConnectionRepos(this.TfsRequestContext, this.ProjectInfo.Id, "github.com", connectionId, (IEnumerable<string>) repositoriesNameToUrlMap.Keys);
      else if (normalizedOperationValue == GitHubConnectionRepoOperation.Remove)
        connectionProvisionResult = service.RemoveExternalConnectionRepos(this.TfsRequestContext, this.ProjectInfo.Id, "github.com", connectionId, (IEnumerable<string>) repositoriesNameToUrlMap.Keys);
      IEnumerable<GitHubConnectionRepoModel> connectionRepoModels = Enumerable.Empty<GitHubConnectionRepoModel>();
      if (connectionProvisionResult != null)
        connectionRepoModels = connectionProvisionResult.RepoProvisionResult.Select<ExternalGitRepoProvisionResult, GitHubConnectionRepoModel>((Func<ExternalGitRepoProvisionResult, GitHubConnectionRepoModel>) (r => new GitHubConnectionRepoModel()
        {
          GitHubRepositoryUrl = r.Repo.WebUrl ?? repositoriesNameToUrlMap[r.Repo.Name],
          ErrorMessage = r.Message
        }));
      return connectionRepoModels;
    }

    private void ValidateData(
      GitHubConnectionReposBatchRequest reposOperationData)
    {
      if (reposOperationData == null)
      {
        string propertyName = nameof (reposOperationData);
        throw new VssPropertyValidationException(propertyName, Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) propertyName));
      }
      if (string.IsNullOrEmpty(reposOperationData.OperationType))
      {
        string propertyName = "OperationType";
        throw new VssPropertyValidationException(propertyName, Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) propertyName));
      }
      if (!GitHubConnectionRepoOperation.IsSupportedOperation(reposOperationData.OperationType))
        throw new VssPropertyValidationException("OperationType", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.UnsupportedBatchOperation());
      if (reposOperationData.GitHubRepositoryUrls == null)
      {
        string propertyName = "GitHubRepositoryUrls";
        throw new VssPropertyValidationException(propertyName, Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) propertyName));
      }
    }

    private void EvaluateLimits(int urlsCount)
    {
      if (urlsCount < 1 || urlsCount > ExternalConnectionService.MaxAllowedBulkRepoSize)
        throw new VssPropertyValidationException("length", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.QueryParameterOutOfRangeWithRangeValues((object) "length", (object) 1, (object) ExternalConnectionService.MaxAllowedBulkRepoSize));
    }

    private Dictionary<string, string> ExtractOrganizationNamesAndRepoNames(IEnumerable<string> urls)
    {
      Dictionary<string, string> repositoriesNameToUrlMap = new Dictionary<string, string>();
      urls.ForEach<string>((Action<string>) (url =>
      {
        if (url == null)
          throw new VssPropertyValidationException(nameof (url), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) "GitHubRepositoryUrl"));
        ArgumentUtility.CheckStringForInvalidCharacters(url, "Url");
        ArgumentUtility.CheckIsValidURI(url, UriKind.Absolute, url);
        if (string.IsNullOrEmpty(url))
          return;
        string[] strArray = url.Split('/');
        string key = (string) null;
        if (strArray.Length > 1)
          key = strArray[strArray.Length - 2] + "/" + strArray[strArray.Length - 1];
        repositoriesNameToUrlMap[key] = url;
      }));
      return repositoriesNameToUrlMap;
    }

    private Dictionary<Guid, IdentityRef> GetCreatorsIdentity(List<Guid> userList)
    {
      IList<Microsoft.VisualStudio.Services.Identity.Identity> collection = this.TfsRequestContext.GetService<IdentityService>().ReadIdentities(this.TfsRequestContext, (IList<Guid>) userList, QueryMembership.None, (IEnumerable<string>) null);
      Dictionary<Guid, IdentityRef> identityMap = new Dictionary<Guid, IdentityRef>();
      Action<Microsoft.VisualStudio.Services.Identity.Identity> action = (Action<Microsoft.VisualStudio.Services.Identity.Identity>) (identity =>
      {
        if (identity == null)
          return;
        IdentityRef identityRef = identity.ToIdentityRef(this.TfsRequestContext);
        identityMap[identity.Id] = identityRef;
      });
      collection.ForEach<Microsoft.VisualStudio.Services.Identity.Identity>(action);
      return identityMap;
    }
  }
}
