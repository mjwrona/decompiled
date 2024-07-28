// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitRepositoriesController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Git.Server.Forks;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class GitRepositoriesController : GitApiController
  {
    internal static Dictionary<Type, HttpStatusCode> s_httpIdentityServiceExceptions;

    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ClientLocationId("225F7195-F9C7-4D14-AB28-A83F7FF77E1F")]
    [ClientResponseType(typeof (IEnumerable<GitRepository>), null, null)]
    [ClientExample("GET__git_repositories.json", null, null, null)]
    public HttpResponseMessage GetRepositories(
      [ClientIgnore] string projectId = null,
      bool includeLinks = false,
      bool includeAllUrls = false,
      bool includeHidden = false)
    {
      return this.GenerateResponse<GitRepository>(this.GetProjectFilteredRepos(projectId, includeLinks, includeAllUrls, includeHidden), this.GetProjectSecuredObject(), this.TfsRequestContext.IsFeatureEnabled("Git.GetRepositories.AsyncResponse"));
    }

    private ISecuredObject GetProjectSecuredObject() => this.ProjectInfo != null ? SharedSecuredObjectFactory.CreateTeamProjectReadOnly(this.ProjectInfo.Id) : (ISecuredObject) null;

    private IEnumerable<GitRepository> GetProjectFilteredRepos(
      string projectId,
      bool includeLinks,
      bool includeAllUrls,
      bool includeHidden)
    {
      GitRepositoriesController repositoriesController = this;
      ProjectInfo projectInfo1;
      IList<TfsGitRepositoryInfo> gitRepositoryInfoList = repositoriesController.TfsRequestContext.GetService<ITeamFoundationGitRepositoryService>().QueryRepositories(repositoriesController.TfsRequestContext, repositoriesController.GetProjectFilter(projectId, out projectInfo1), !includeHidden);
      IDictionary<string, ProjectInfo> projectMap;
      if (projectInfo1 == null)
        projectMap = (IDictionary<string, ProjectInfo>) repositoriesController.TfsRequestContext.GetService<IProjectService>().GetProjects(repositoriesController.TfsRequestContext).ToDictionary<ProjectInfo, string, ProjectInfo>((Func<ProjectInfo, string>) (p => p.Uri), (Func<ProjectInfo, ProjectInfo>) (p => p));
      else
        projectMap = (IDictionary<string, ProjectInfo>) new Dictionary<string, ProjectInfo>()
        {
          {
            projectInfo1.Uri,
            projectInfo1
          }
        };
      foreach (TfsGitRepositoryInfo repo in (IEnumerable<TfsGitRepositoryInfo>) gitRepositoryInfoList)
      {
        ProjectInfo projectInfo2;
        if (projectMap.TryGetValue(repo.Key.GetProjectUri(), out projectInfo2))
        {
          GitRepository webApiItem = repo.ToWebApiItem(repositoriesController.TfsRequestContext, projectInfo2, includeLinks, includeAllUrls);
          ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(repo.Key);
          webApiItem.SetSecuredObject(repositoryReadOnly);
          yield return webApiItem;
        }
      }
    }

    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ClientLocationId("225F7195-F9C7-4D14-AB28-A83F7FF77E1F")]
    [ClientExample("GET__git_repositories__repositoryId_.json", "Get a repository by repositoryId", null, null)]
    [ClientExample("GET__git_repositories__remoteurl.json", "Get a repository by remote URL", null, null)]
    public GitRepository GetRepository([ClientParameterType(typeof (Guid), true)] string repositoryId, [ClientIgnore] string projectId = null)
    {
      using (ITfsGitRepository repositoryByFilters = GitServerUtils.FindRepositoryByFilters(this.TfsRequestContext, repositoryId, this.GetProjectFilter(projectId)))
        return repositoryByFilters.ToWebApiItem(this.TfsRequestContext, getDefaultBranch: true, includeLinks: true);
    }

    [HttpGet]
    [ClientLocationId("225F7195-F9C7-4D14-AB28-A83F7FF77E1F")]
    public GitRepository GetRepositoryWithParent(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      bool includeParent,
      [ClientIgnore] string projectId = null)
    {
      using (ITfsGitRepository repositoryByFilters = GitServerUtils.FindRepositoryByFilters(this.TfsRequestContext, repositoryId, this.GetProjectFilter(projectId)))
        return repositoryByFilters.ToWebApiItem(this.TfsRequestContext, getDefaultBranch: true, includeLinks: true, includeParent: includeParent);
    }

    [HttpPost]
    [ClientResponseCode(HttpStatusCode.Created, null, false)]
    [ClientLocationId("225F7195-F9C7-4D14-AB28-A83F7FF77E1F")]
    [ClientResponseType(typeof (GitRepository), null, null)]
    [ClientExample("POST__git_repositories.json", "Create a repository", null, null)]
    [ClientExample("POST__git_repositories_fork.json", "Create a fork of a parent repository", null, null)]
    [ClientExample("POST__git_repositories_fork_source_ref.json", "Create a fork of a parent repository syncing only the provided refs", null, null)]
    public HttpResponseMessage CreateRepository(
      GitRepositoryCreateOptions gitRepositoryToCreate,
      string sourceRef = null)
    {
      if (gitRepositoryToCreate == null)
        throw new InvalidArgumentValueException(nameof (gitRepositoryToCreate), Resources.Get("MalformedGitRepositoryData"));
      if (string.IsNullOrWhiteSpace(gitRepositoryToCreate.Name))
        throw new InvalidArgumentValueException("Name", Resources.Get("RepositoryNameRequired"));
      if (this.ProjectId == Guid.Empty && (gitRepositoryToCreate.ProjectReference == null || gitRepositoryToCreate.ProjectReference.Id == Guid.Empty))
        throw new InvalidArgumentValueException("ProjectReference", Resources.Get("TeamProjectRequired"));
      if (this.ProjectId != Guid.Empty && gitRepositoryToCreate.ProjectReference != null && gitRepositoryToCreate.ProjectReference.Id != this.ProjectId)
        throw new InvalidArgumentValueException(Resources.Get("MismatchedProjectId"));
      Guid guid = this.ProjectId != Guid.Empty ? this.ProjectId : gitRepositoryToCreate.ProjectReference.Id;
      ProjectInfo project = this.TfsRequestContext.GetService<IProjectService>().GetProject(this.TfsRequestContext, guid);
      project.PopulateProperties(this.TfsRequestContext, "System.SourceControlGitEnabled", ProcessTemplateIdPropertyNames.ProcessTemplateType);
      List<AccessControlEntry> repositoryPermissions = !this.IsProjectGitEnabled(project) ? VersionControlProcessTemplateUtility.GetGitRepositoryPermissions(this.TfsRequestContext, project, true) : (List<AccessControlEntry>) null;
      if (gitRepositoryToCreate.ParentRepository != null)
      {
        GitForkSyncRequestParameters syncParams = createSyncParams(gitRepositoryToCreate.ParentRepository);
        ForkFetchAsyncOp fetchOp;
        using (ITfsGitRepository andSyncFork = this.TfsRequestContext.GetService<IGitForkService>().CreateAndSyncFork(this.TfsRequestContext, syncParams, guid, gitRepositoryToCreate.Name, out fetchOp))
        {
          GitRepository webApiItem = andSyncFork.ToWebApiItem(this.TfsRequestContext, project, includeLinks: true);
          string absoluteUri = this.TfsRequestContext.GetService<ILocationService>().GetResourceUri(this.TfsRequestContext, "git", GitWebApiConstants.ForkSyncRequestsLocationId, (object) new
          {
            forkSyncOperationId = fetchOp.OperationId,
            repositoryNameOrId = andSyncFork.Key.RepoId
          }).AbsoluteUri;
          webApiItem.Links.AddLink("forkSyncOperation", absoluteUri);
          return this.Request.CreateResponse<GitRepository>(HttpStatusCode.Created, webApiItem);
        }
      }
      else
      {
        ITeamFoundationGitRepositoryService service = this.TfsRequestContext.GetService<ITeamFoundationGitRepositoryService>();
        ITfsGitRepository repository1;
        if (service.TryFindRepositoryByName(this.TfsRequestContext, project.Name, gitRepositoryToCreate.Name, out repository1, true))
        {
          using (repository1)
          {
            if (repository1 != null)
            {
              if (repository1.IsDisabled)
                throw new GitRepositoryNameAlreadyExistsException(gitRepositoryToCreate.Name);
            }
          }
        }
        using (ITfsGitRepository repository2 = service.CreateRepository(this.TfsRequestContext, guid, gitRepositoryToCreate.Name, (IEnumerable<IAccessControlEntry>) null, (IEnumerable<IAccessControlEntry>) repositoryPermissions))
          return this.Request.CreateResponse<GitRepository>(HttpStatusCode.Created, repository2.ToWebApiItem(this.TfsRequestContext, project));
      }

      GitForkSyncRequestParameters createSyncParams(GitRepositoryRef source)
      {
        if (source.ProjectReference == null || source.ProjectReference.Id == Guid.Empty)
          throw new InvalidArgumentValueException("gitRepositoryToCreate.ParentRepository.ProjectReference", Resources.Get("TeamProjectRequired"));
        List<SourceToTargetRef> sourceToTargetRefList = (List<SourceToTargetRef>) null;
        if (sourceRef != null)
          sourceToTargetRefList = new List<SourceToTargetRef>()
          {
            new SourceToTargetRef()
            {
              SourceRef = sourceRef,
              TargetRef = sourceRef
            }
          };
        GitForkSyncRequestParameters syncParams = new GitForkSyncRequestParameters();
        GlobalGitRepositoryKey gitRepositoryKey = new GlobalGitRepositoryKey();
        TeamProjectCollectionReference collection = source.Collection;
        gitRepositoryKey.CollectionId = collection != null ? collection.Id : this.TfsRequestContext.ServiceHost.InstanceId;
        gitRepositoryKey.ProjectId = source.ProjectReference.Id;
        gitRepositoryKey.RepositoryId = source.Id;
        syncParams.Source = gitRepositoryKey;
        syncParams.SourceToTargetRefs = sourceToTargetRefList;
        return syncParams;
      }
    }

    [HttpDelete]
    [ClientExample("DELETE__git_repositories__repositoryId_.json", null, null, null)]
    [ClientLocationId("225F7195-F9C7-4D14-AB28-A83F7FF77E1F")]
    public void DeleteRepository(Guid repositoryId)
    {
      if (repositoryId == Guid.Empty)
        throw new ArgumentException("ErrorRespositoryIdInvalid").Expected("git");
      ITeamFoundationGitRepositoryService service = this.TfsRequestContext.GetService<ITeamFoundationGitRepositoryService>();
      using (ITfsGitRepository repositoryById = service.FindRepositoryById(this.TfsRequestContext, repositoryId))
      {
        if (this.ProjectId != Guid.Empty && this.ProjectId != repositoryById.Key.ProjectId)
          throw new GitRepositoryNotFoundException(repositoryId);
        this.TfsRequestContext.GetService<IProjectService>().GetProject(this.TfsRequestContext, repositoryById.Key.ProjectId);
        service.DeleteRepositories(this.TfsRequestContext, (RepoScope) repositoryById.Key);
      }
    }

    [HttpPatch]
    [ClientResponseCode(HttpStatusCode.OK, "The operation succeeded. The response contains the updated repository information.", false)]
    [ClientLocationId("225F7195-F9C7-4D14-AB28-A83F7FF77E1F")]
    [ClientExample("PATCH__git_repositories__disable_.json", "Disable repository", null, null)]
    [ClientExample("PATCH__git_repositories__repositoryId_.json", "Update a respository without specifying the project", null, null)]
    [ClientExample("PATCH__git_repositories__project_.json", "Update a respository while specifying the project", null, null)]
    public GitRepository UpdateRepository(Guid repositoryId, GitRepository newRepositoryInfo)
    {
      if (repositoryId == Guid.Empty || newRepositoryInfo == null || newRepositoryInfo.Id != Guid.Empty && newRepositoryInfo.Id != repositoryId)
        throw new ArgumentException(Resources.Get("UnsupportedRepoChangeType")).Expected("git");
      ITeamFoundationGitRepositoryService service = this.TfsRequestContext.GetService<ITeamFoundationGitRepositoryService>();
      using (ITfsGitRepository repositoryById1 = service.FindRepositoryById(this.TfsRequestContext, repositoryId, includeDisabled: newRepositoryInfo.IsDisabled.HasValue))
      {
        if (this.ProjectId != Guid.Empty && this.ProjectId != repositoryById1.Key.ProjectId)
          throw new GitRepositoryNotFoundException(repositoryId);
        bool? isDisabled1 = newRepositoryInfo.IsDisabled;
        int num1;
        if (isDisabled1.HasValue)
        {
          isDisabled1 = newRepositoryInfo.IsDisabled;
          bool isDisabled2 = repositoryById1.IsDisabled;
          num1 = !(isDisabled1.GetValueOrDefault() == isDisabled2 & isDisabled1.HasValue) ? 1 : 0;
        }
        else
          num1 = 0;
        int num2 = !string.IsNullOrWhiteSpace(newRepositoryInfo.Name) ? 1 : (!string.IsNullOrWhiteSpace(newRepositoryInfo.DefaultBranch) ? 1 : 0);
        if (num1 == num2)
          throw new ArgumentException(Resources.Get("UnsupportedRepoChangeType")).Expected("git");
        ProjectInfo project = this.TfsRequestContext.GetService<IProjectService>().GetProject(this.TfsRequestContext, repositoryById1.Key.ProjectId);
        if (newRepositoryInfo.ProjectReference != null && newRepositoryInfo.ProjectReference.Id != repositoryById1.Key.ProjectId)
          throw new ArgumentException(Resources.Get("UnsupportedRepoChangeType")).Expected("git");
        GitRepository gitRepository = (GitRepository) null;
        if (!string.IsNullOrWhiteSpace(newRepositoryInfo.Name) || !string.IsNullOrWhiteSpace(newRepositoryInfo.DefaultBranch))
        {
          ITfsGitRepository repo;
          using (repo = service.UpdateRepository(this.TfsRequestContext, repositoryById1.Key, newRepositoryInfo.Name, newRepositoryInfo.DefaultBranch))
            gitRepository = repo.ToWebApiItem(this.TfsRequestContext, project, true);
        }
        else
        {
          isDisabled1 = newRepositoryInfo.IsDisabled;
          if (isDisabled1.HasValue)
          {
            isDisabled1 = newRepositoryInfo.IsDisabled;
            bool flag = true;
            if (isDisabled1.GetValueOrDefault() == flag & isDisabled1.HasValue)
              service.DisableRepository(this.TfsRequestContext, (RepoScope) repositoryById1.Key);
            else
              service.EnableRepository(this.TfsRequestContext, (RepoScope) repositoryById1.Key);
            ITfsGitRepository repositoryById2;
            using (repositoryById2 = service.FindRepositoryById(this.TfsRequestContext, repositoryById1.Key.RepoId, includeDisabled: true))
              gitRepository = repositoryById2.ToWebApiItem(this.TfsRequestContext, project, true);
          }
        }
        return gitRepository;
      }
    }

    private bool IsProjectGitEnabled(ProjectInfo project)
    {
      ProjectProperty projectProperty = project.Properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (x => x.Name.Equals("System.SourceControlGitEnabled")));
      return projectProperty != null && string.Equals((string) projectProperty.Value, bool.TrueString);
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions
    {
      get
      {
        if (GitRepositoriesController.s_httpIdentityServiceExceptions == null)
          GitRepositoriesController.s_httpIdentityServiceExceptions = new Dictionary<Type, HttpStatusCode>(base.HttpExceptions)
          {
            [typeof (GitRepositoryPerProjectThresholdExceededException)] = HttpStatusCode.BadRequest,
            [typeof (GitRefLockDeniedException)] = HttpStatusCode.Forbidden,
            [typeof (GitRefUnlockDeniedException)] = HttpStatusCode.Forbidden,
            [typeof (GitRepositoryNameAlreadyExistsException)] = HttpStatusCode.Conflict
          };
        return (IDictionary<Type, HttpStatusCode>) GitRepositoriesController.s_httpIdentityServiceExceptions;
      }
    }
  }
}
