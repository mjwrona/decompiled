// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TeamFoundationGitRepositoryService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Policy.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using Microsoft.VisualStudio.Services.Settings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class TeamFoundationGitRepositoryService : 
    ITeamFoundationGitRepositoryService,
    IVssFrameworkService,
    IInternalGitRepoService
  {
    private const string RepoCacheKey = "TfsGitServer.TfsGitRepositoryCache";
    private IDisposableReadOnlyList<GitKeyedJob<RepoJobKey>> m_repoScopedJobs;
    private IDisposableReadOnlyList<GitKeyedJob<OdbJobKey>> m_odbScopedJobs;
    private readonly IGitDependencyRoot m_dependencyRoot;
    private const string c_layer = "TeamFoundationGitRepositoryService";
    private const string MaxRepositoriesRegistryKey = "/Service/Git/Settings/MaxRepositoriesPerTeamProject";

    internal TeamFoundationGitRepositoryService()
      : this(DefaultGitDependencyRoot.Instance)
    {
    }

    internal TeamFoundationGitRepositoryService(IGitDependencyRoot dependencyRoot) => this.m_dependencyRoot = dependencyRoot;

    public ITfsGitRepository CreateTeamProjectRepository(
      IVssRequestContext rc,
      Guid teamProjectId,
      string repoName,
      IEnumerable<IAccessControlEntry> permissions)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(repoName, "repositoryName", "git");
      ArgumentUtility.CheckForEmptyGuid(teamProjectId, nameof (teamProjectId), "git");
      string projectUri = ProjectInfo.GetProjectUri(teamProjectId);
      rc.Trace(1013051, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (TeamFoundationGitRepositoryService), "Creating Team Project Repository in Team Project {0} with name {1}", (object) teamProjectId, (object) repoName);
      GitServerUtils.ValidateAndNormalizeGitRepositoryName(ref repoName, nameof (repoName));
      SecurityHelper.Instance.CheckCreateTeamProjectPermission(rc);
      rc.GetService<IDataspaceService>().CreateDataspaces(rc, new string[2]
      {
        "VersionControl",
        "Git"
      }, teamProjectId);
      GitServerUtils.ConfigureTeamProjectForGit(rc, projectUri, setCapability: true);
      if (permissions != null)
        SecurityHelper.Instance.SetPermissions(rc, new RepoScope(teamProjectId, Guid.Empty), (string) null, permissions);
      return this.CreateRepository(rc, teamProjectId, repoName, (IEnumerable<IAccessControlEntry>) null, (IEnumerable<IAccessControlEntry>) null, false, new OdbId?(), new MinimalGlobalRepoKey?());
    }

    ITfsGitRepository IInternalGitRepoService.CreateRepositoryWithExistingOdb(
      IVssRequestContext rc,
      Guid teamProjectId,
      string repositoryName,
      OdbId existingOdbId,
      MinimalGlobalRepoKey forkSourceRepo)
    {
      return this.CreateRepository(rc, teamProjectId, repositoryName, (IEnumerable<IAccessControlEntry>) null, (IEnumerable<IAccessControlEntry>) null, false, new OdbId?(existingOdbId), new MinimalGlobalRepoKey?(forkSourceRepo));
    }

    public ITfsGitRepository CreateRepository(
      IVssRequestContext rc,
      Guid teamProjectId,
      string repositoryName,
      IEnumerable<IAccessControlEntry> permissions,
      IEnumerable<IAccessControlEntry> permissionsForAllReposIfNotSet,
      bool isHidden)
    {
      return this.CreateRepository(rc, teamProjectId, repositoryName, permissions, permissionsForAllReposIfNotSet, isHidden, new OdbId?(), new MinimalGlobalRepoKey?());
    }

    private ITfsGitRepository CreateRepository(
      IVssRequestContext rc,
      Guid teamProjectId,
      string repoName,
      IEnumerable<IAccessControlEntry> permissions,
      IEnumerable<IAccessControlEntry> permissionsForAllReposIfNotSet,
      bool isHidden,
      OdbId? existingOdbId,
      MinimalGlobalRepoKey? forkSourceRepo)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(repoName, "repositoryName", "git");
      ArgumentUtility.CheckForEmptyGuid(teamProjectId, nameof (teamProjectId), "git");
      if (existingOdbId.HasValue != forkSourceRepo.HasValue)
        throw new ArgumentException("Either both or neither of existingOdbId and forkSourceRepo are required.");
      string projectUri = ProjectInfo.GetProjectUri(teamProjectId);
      rc.Trace(1013052, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (TeamFoundationGitRepositoryService), "Creating Repository in Team Project {0} with Name {1}", (object) teamProjectId, (object) repoName);
      GitServerUtils.ValidateAndNormalizeGitRepositoryName(ref repoName, nameof (repoName));
      GitServerUtils.ConfigureTeamProjectForGit(rc, projectUri, !isHidden, permissions: permissionsForAllReposIfNotSet);
      SecurityHelper.Instance.CheckCreateRepositoryPermission(rc, teamProjectId);
      RepoKey repoKey = new RepoKey(teamProjectId, Guid.NewGuid(), existingOdbId.HasValue ? existingOdbId.GetValueOrDefault().Value : Guid.NewGuid());
      if (permissions != null)
        SecurityHelper.Instance.SetPermissions(rc, (RepoScope) repoKey, (string) null, permissions);
      else
        SecurityHelper.Instance.SetDefaultRepositoryPermissions(rc, (RepoScope) repoKey);
      int result;
      if (!int.TryParse(rc.GetService<IVssRegistryService>().GetValue(rc, (RegistryQuery) "/Service/Git/Settings/MaxRepositoriesPerTeamProject", false, (string) null), out result))
        result = int.MaxValue;
      IDataspaceService dataspaceService = (IDataspaceService) null;
      if (!existingOdbId.HasValue)
      {
        dataspaceService = rc.GetService<IDataspaceService>();
        dataspaceService.CreateDataspace(rc, "GitOdb", repoKey.OdbId.Value, DataspaceState.Creating);
        ITeamFoundationCounterService service = rc.GetService<ITeamFoundationCounterService>();
        string[] strArray = new string[3]
        {
          "CommitUserId",
          "InternalForkId",
          "LfsObjectIndex"
        };
        foreach (string counterName in strArray)
          service.CreateCounter(rc, counterName, 1L, "GitOdb", repoKey.OdbId.Value, true);
      }
      using (GitOdbComponent gitOdbComponent = rc.CreateGitOdbComponent(repoKey))
        gitOdbComponent.CreateFork(new MinimalGlobalRepoKey(Guid.Empty, repoKey.RepoId));
      if (forkSourceRepo.HasValue)
      {
        using (GitOdbComponent gitOdbComponent = rc.CreateGitOdbComponent(repoKey))
          gitOdbComponent.CreateForkRelationship(forkSourceRepo.Value, new MinimalGlobalRepoKey(Guid.Empty, repoKey.RepoId));
      }
      bool flag = false;
      try
      {
        if (!existingOdbId.HasValue)
          this.m_dependencyRoot.GetBlobProvider(rc).CreateRepository(rc, repoKey.OdbId);
        using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        {
          gitCoreComponent.CreateRepository(new RepoKey(teamProjectId, repoKey.RepoId), repoName, result, isHidden, forkSourceRepo.HasValue, repoKey.OdbId.Value);
          flag = true;
        }
        try
        {
          GitAuditLogHelper.RepositoryCreated(rc, repoKey, repoName, forkSourceRepo);
        }
        catch (Exception ex)
        {
          rc.TraceException(1013881, GitServerUtils.TraceArea, nameof (TeamFoundationGitRepositoryService), ex);
        }
        if (!existingOdbId.HasValue)
        {
          DateTime scheduleTime = DateTime.UtcNow.Date.AddDays(1.0);
          uint num = (uint) repoKey.OdbId.GetHashCode() % 1440U;
          scheduleTime = scheduleTime.AddMinutes((double) num);
          KeyScopedJobUtil.CreateInitialSchedules<OdbJobKey>(rc, new OdbJobKey(repoKey.OdbId), scheduleTime);
        }
        if (!rc.GetService<ISettingsService>().GetValue<bool>(rc, SettingsUserScope.AllUsers, "Project", teamProjectId.ToString(), "VersionControl/Projects/{0}/CreatedBranchesManagePermissionsEnabled", true))
          rc.GetService<IVssRegistryService>().WriteEntries(rc, (IEnumerable<RegistryEntry>) new RegistryEntry[1]
          {
            new RegistryEntry(WebAccessRegistryConstants.Prefix + string.Format("/VersionControl/Repositories/{0}/CreatedBranchesManagePermissionsEnabled", (object) repoKey.RepoId), "false")
          });
        rc.GetService<IGitAdvSecService>().OnRepositoryCreate(rc, repoKey);
        ITfsGitRepository repository = (ITfsGitRepository) null;
        try
        {
          repository = this.FindRepositoryById(rc, repoKey.RepoId, false, false, false);
          TeamFoundationGitRepositoryService.PublishClientTraces(rc, repoName, projectUri, repoKey);
          TeamFoundationGitRepositoryService.PublishNotifications(rc, repoName, forkSourceRepo, projectUri, repoKey);
          return repository;
        }
        catch
        {
          repository?.Dispose();
          throw;
        }
      }
      finally
      {
        dataspaceService?.UpdateDataspaces(rc, "GitOdb", new Guid?(repoKey.OdbId.Value), new int?(), new DataspaceState?(flag ? DataspaceState.Active : DataspaceState.Deleted));
      }
    }

    public ITfsGitRepository RenameRepository(
      IVssRequestContext rc,
      RepoKey repoKey,
      string newRepositoryName)
    {
      ArgumentUtility.CheckForNull<RepoKey>(repoKey, nameof (repoKey));
      ArgumentUtility.CheckStringForNullOrEmpty(newRepositoryName, "repositoryName");
      return this.UpdateRepository(rc, repoKey, newRepositoryName, (string) null);
    }

    public ITfsGitRepository UpdateRepository(
      IVssRequestContext rc,
      RepoKey repoKey,
      string newRepositoryName = null,
      string newDefaultBranch = null)
    {
      ArgumentUtility.CheckForNull<RepoKey>(repoKey, nameof (repoKey));
      if (string.IsNullOrEmpty(newRepositoryName) && string.IsNullOrEmpty(newDefaultBranch))
        throw new ArgumentException(CommonResources.BothStringsCannotBeNull((object) nameof (newRepositoryName), (object) nameof (newDefaultBranch)));
      rc.Trace(1013052, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (TeamFoundationGitRepositoryService), "Updating Repository {0} to Name: {1} DefaultBranch: {2}", (object) repoKey, (object) newRepositoryName, (object) newDefaultBranch);
      if (!string.IsNullOrEmpty(newRepositoryName))
        GitServerUtils.ValidateAndNormalizeGitRepositoryName(ref newRepositoryName, nameof (newRepositoryName));
      SecurityHelper.Instance.CheckRenameRepositoryPermission(rc, (RepoScope) repoKey);
      string name1;
      string name2;
      using (ITfsGitRepository repositoryById = this.FindRepositoryById(rc, repoKey.RepoId, false, false, false))
      {
        name1 = repositoryById.Name;
        name2 = repositoryById.Refs.GetDefault()?.Name;
      }
      newRepositoryName = string.IsNullOrEmpty(newRepositoryName) ? (string) null : newRepositoryName;
      newDefaultBranch = string.IsNullOrEmpty(newDefaultBranch) ? (string) null : newDefaultBranch;
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        gitCoreComponent.UpdateRepository(repoKey, newRepositoryName, newDefaultBranch);
      TeamFoundationGitRepositoryService.InvalidateRepositoryCache(rc, repoKey.RepoId);
      try
      {
        GitAuditLogHelper.RepositoryModified(rc, repoKey, name1, newRepositoryName, name2, newDefaultBranch);
      }
      catch (Exception ex)
      {
        rc.TraceException(1013881, GitServerUtils.TraceArea, nameof (TeamFoundationGitRepositoryService), ex);
      }
      ITfsGitRepository tfsGitRepository = (ITfsGitRepository) null;
      try
      {
        tfsGitRepository = this.FindRepositoryById(rc, repoKey.RepoId, false, false, false);
        ClientTraceData properties = new ClientTraceData();
        properties.Add("Action", (object) "RenameRepository");
        properties.Add("TeamProjectUri", (object) repoKey.GetProjectUri());
        properties.Add("RepositoryId", (object) repoKey.RepoId.ToString());
        properties.Add("RepositoryName", (object) newRepositoryName);
        properties.Add("RepositoryDefaultBranch", (object) newDefaultBranch);
        rc.GetService<ClientTraceService>().Publish(rc, "Microsoft.TeamFoundation.Git.Server", "TeamProject", properties);
        rc.GetService<ITeamFoundationEventService>().PublishNotification(rc, (object) new RepositoryRenamedNotification(repoKey.GetProjectUri(), repoKey.RepoId, newRepositoryName, rc.UserContext)
        {
          OldName = name1
        });
        return tfsGitRepository;
      }
      catch
      {
        tfsGitRepository?.Dispose();
        throw;
      }
    }

    public void DeleteRepositories(IVssRequestContext rc, RepoScope toDelete)
    {
      ArgumentUtility.CheckForNull<RepoScope>(toDelete, nameof (toDelete));
      ArgumentUtility.CheckForEmptyGuid(toDelete.ProjectId, "ProjectId");
      rc.Trace(1013053, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (TeamFoundationGitRepositoryService), "Soft Deleting repository {0}", (object) toDelete);
      string projectUri = toDelete.GetProjectUri();
      if (toDelete.RepoId == Guid.Empty)
        SecurityHelper.Instance.CheckDestroyAllPermission(rc, projectUri);
      SecurityHelper.Instance.CheckDeleteRepositoryPermission(rc, toDelete);
      Guid userId = rc.GetUserId();
      ICollection<TfsGitRepositoryInfo> repositoryInfos;
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        repositoryInfos = gitCoreComponent.DeleteRepositories(toDelete, userId);
      try
      {
        GitAuditLogHelper.RepositoriesDeleted(rc, repositoryInfos, false);
      }
      catch (Exception ex)
      {
        rc.TraceException(1013881, GitServerUtils.TraceArea, nameof (TeamFoundationGitRepositoryService), ex);
      }
      IGitAdvSecService service = rc.GetService<IGitAdvSecService>();
      string repositoryName = string.Empty;
      foreach (TfsGitRepositoryInfo gitRepositoryInfo in (IEnumerable<TfsGitRepositoryInfo>) repositoryInfos)
      {
        if (string.IsNullOrEmpty(repositoryName))
          repositoryName = gitRepositoryInfo.Name.Trim();
        using (GitOdbComponent gitOdbComponent = rc.CreateGitOdbComponent(gitRepositoryInfo.Key))
          gitOdbComponent.DeleteForkRelationships(new MinimalGlobalRepoKey(Guid.Empty, gitRepositoryInfo.Key.RepoId));
        service.OnRepositoryDisable(rc, new RepoKey(gitRepositoryInfo.Key.ProjectId, gitRepositoryInfo.Key.RepoId));
      }
      rc.GetService<ITeamFoundationEventService>().PublishNotification(rc, (object) new RepositoryDeletedNotification(projectUri, toDelete.RepoId, repositoryName, rc.UserContext)
      {
        IsHardDelete = false
      });
    }

    public ITfsGitRepository UndeleteRepository(IVssRequestContext rc, RepoKey toUndelete)
    {
      ArgumentUtility.CheckForNull<RepoKey>(toUndelete, nameof (toUndelete));
      SecurityHelper.Instance.CheckDeleteRepositoryPermission(rc, (RepoScope) toUndelete);
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        gitCoreComponent.UndeleteRepository(toUndelete);
      TeamFoundationGitRepositoryService.InvalidateRepositoryCache(rc, toUndelete.RepoId);
      ITfsGitRepository repositoryById = this.FindRepositoryById(rc.Elevate(), toUndelete.RepoId, false, false, false);
      try
      {
        GitAuditLogHelper.RepositoriesUndeleted(rc, repositoryById);
      }
      catch (Exception ex)
      {
        rc.TraceException(1013881, GitServerUtils.TraceArea, nameof (TeamFoundationGitRepositoryService), ex);
      }
      rc.GetService<IGitAdvSecService>().OnRepositoryCreate(rc, new RepoKey(toUndelete.ProjectId, toUndelete.RepoId));
      rc.GetService<ITeamFoundationEventService>().PublishNotification(rc, (object) new RepositoryUndeletedNotification(toUndelete.GetProjectUri(), toUndelete.RepoId, rc.UserContext));
      return repositoryById;
    }

    public void DestroyRepositories(IVssRequestContext rc, RepoScope toDestroy)
    {
      string projectUri = ProjectInfo.GetProjectUri(toDestroy.ProjectId);
      SecurityHelper.Instance.CheckDeleteRepositoryPermission(rc, toDestroy);
      if (toDestroy.RepoId == Guid.Empty)
        SecurityHelper.Instance.CheckDestroyAllPermission(rc, projectUri);
      Guid userId = rc.GetUserId();
      ICollection<TfsGitRepositoryInfo> gitRepositoryInfos;
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        gitRepositoryInfos = gitCoreComponent.DestroyRepositories(toDestroy, userId);
      try
      {
        GitAuditLogHelper.RepositoriesDeleted(rc, gitRepositoryInfos, true);
      }
      catch (Exception ex)
      {
        rc.TraceException(1013881, GitServerUtils.TraceArea, nameof (TeamFoundationGitRepositoryService), ex);
      }
      IGitAdvSecService service1 = rc.GetService<IGitAdvSecService>();
      string repositoryName = string.Empty;
      IDataspaceService service2 = rc.GetService<IDataspaceService>();
      foreach (TfsGitRepositoryInfo gitRepositoryInfo in (IEnumerable<TfsGitRepositoryInfo>) gitRepositoryInfos)
      {
        if (string.IsNullOrEmpty(repositoryName))
          repositoryName = gitRepositoryInfo.Name.Trim();
        RepoKey key = gitRepositoryInfo.Key;
        bool flag = false;
        using (GitOdbComponent gitOdbComponent = rc.CreateGitOdbComponent(key))
        {
          gitOdbComponent.DeleteFork(new MinimalGlobalRepoKey(Guid.Empty, key.RepoId));
          List<MinimalGlobalRepoKey> minimalGlobalRepoKeyList = gitOdbComponent.QueryForksByOdb();
          if (minimalGlobalRepoKeyList != null)
          {
            if (minimalGlobalRepoKeyList.Count != 0)
              goto label_20;
          }
          flag = true;
        }
label_20:
        List<Guid> jobsToDelete = new List<Guid>();
        foreach (GitKeyedJob<RepoJobKey> repoScopedJob in (IEnumerable<GitKeyedJob<RepoJobKey>>) this.m_repoScopedJobs)
        {
          repoScopedJob.GetType();
          Guid guid = KeyScopedJobUtil.JobIdForKeyScopedJob<RepoJobKey>(new RepoJobKey()
          {
            RepositoryId = gitRepositoryInfo.Key.RepoId
          }, repoScopedJob.GetType().Name);
          jobsToDelete.Add(guid);
        }
        if (flag)
        {
          foreach (GitKeyedJob<OdbJobKey> odbScopedJob in (IEnumerable<GitKeyedJob<OdbJobKey>>) this.m_odbScopedJobs)
          {
            odbScopedJob.GetType();
            Guid guid = KeyScopedJobUtil.JobIdForKeyScopedJob<OdbJobKey>(new OdbJobKey(gitRepositoryInfo.Key.OdbId), odbScopedJob.GetType().Name);
            jobsToDelete.Add(guid);
          }
          service2.UpdateDataspaces(rc, "GitOdb", new Guid?(key.OdbId.Value), new int?(), new DataspaceState?(DataspaceState.Deleted));
          using (GitOdbComponent gitOdbComponent = rc.CreateGitOdbComponent(key))
            gitOdbComponent.DeleteOdb();
          this.m_dependencyRoot.GetBlobProvider(rc).DeleteContainer(rc, key.OdbId, false);
        }
        if (jobsToDelete.Count > 0)
          rc.GetService<ITeamFoundationJobService>().DeleteJobDefinitions(rc, (IEnumerable<Guid>) jobsToDelete);
        service1.OnRepositoryDestroy(rc, new RepoKey(gitRepositoryInfo.Key.ProjectId, gitRepositoryInfo.Key.RepoId));
      }
      if (gitRepositoryInfos.Any<TfsGitRepositoryInfo>())
        SecurityHelper.Instance.RemoveACLs(rc, toDestroy, (string) null);
      ClientTraceData properties = new ClientTraceData();
      properties.Add("Action", (object) "DeleteRepository");
      properties.Add("TeamProjectUri", (object) projectUri);
      properties.Add("RepositoryId", (object) toDestroy.RepoId.ToString());
      rc.GetService<ClientTraceService>().Publish(rc, "Microsoft.TeamFoundation.Git.Server", "TeamProject", properties);
      rc.GetService<ITeamFoundationEventService>().PublishNotification(rc, (object) new RepositoryDeletedNotification(projectUri, toDestroy.RepoId, repositoryName, rc.UserContext)
      {
        IsHardDelete = true
      });
    }

    public void DisableRepository(IVssRequestContext rc, RepoScope toDisable)
    {
      ArgumentUtility.CheckForNull<RepoScope>(toDisable, nameof (toDisable));
      ArgumentUtility.CheckForEmptyGuid(toDisable.ProjectId, "ProjectId");
      rc.Trace(1013891, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (TeamFoundationGitRepositoryService), "Disabling repository {0}", (object) toDisable);
      SecurityHelper.Instance.CheckDeleteRepositoryPermission(rc, toDisable);
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        gitCoreComponent.DisableRepository(toDisable);
      TeamFoundationGitRepositoryService.InvalidateRepositoryCache(rc, toDisable.RepoId);
      TeamFoundationGitRepositoryService.PublishDisabledRepoCiEvent(rc, toDisable, nameof (DisableRepository));
      try
      {
        GitAuditLogHelper.RepositoryIsDisabledModified(rc, toDisable, true);
      }
      catch (Exception ex)
      {
        rc.TraceException(1013881, GitServerUtils.TraceArea, nameof (TeamFoundationGitRepositoryService), ex);
      }
      string projectUri = toDisable.GetProjectUri();
      rc.GetService<ITeamFoundationEventService>().PublishNotification(rc, (object) new RepositoryDisabledNotification(projectUri, toDisable.RepoId, true, rc.UserContext));
      rc.GetService<IGitAdvSecService>().OnRepositoryDisable(rc, new RepoKey(toDisable.ProjectId, toDisable.RepoId));
    }

    public void EnableRepository(IVssRequestContext rc, RepoScope toEnable)
    {
      ArgumentUtility.CheckForNull<RepoScope>(toEnable, nameof (toEnable));
      rc.Trace(1013892, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (TeamFoundationGitRepositoryService), "Enabling repository {0}", (object) toEnable);
      SecurityHelper.Instance.CheckDeleteRepositoryPermission(rc, toEnable);
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        gitCoreComponent.EnableRepository(toEnable);
      TeamFoundationGitRepositoryService.InvalidateRepositoryCache(rc, toEnable.RepoId);
      TeamFoundationGitRepositoryService.PublishDisabledRepoCiEvent(rc, toEnable, nameof (EnableRepository));
      try
      {
        GitAuditLogHelper.RepositoryIsDisabledModified(rc, toEnable, false);
      }
      catch (Exception ex)
      {
        rc.TraceException(1013881, GitServerUtils.TraceArea, nameof (TeamFoundationGitRepositoryService), ex);
      }
      string projectUri = toEnable.GetProjectUri();
      rc.GetService<ITeamFoundationEventService>().PublishNotification(rc, (object) new RepositoryDisabledNotification(projectUri, toEnable.RepoId, false, rc.UserContext));
      rc.GetService<IGitAdvSecService>().OnRepositoryCreate(rc, new RepoKey(toEnable.ProjectId, toEnable.RepoId));
    }

    private static void PublishDisabledRepoCiEvent(
      IVssRequestContext rc,
      RepoScope repo,
      string action)
    {
      try
      {
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("Action", action);
        properties.Add("ProjectId", (object) repo.ProjectId);
        properties.Add("RepositoryId", (object) repo.RepoId);
        rc.GetService<CustomerIntelligenceService>().Publish(rc, "Microsoft.TeamFoundation.Git.Server", "DisabledRepositories", properties);
      }
      catch (Exception ex)
      {
        rc.TraceException(1013894, GitServerUtils.TraceArea, nameof (TeamFoundationGitRepositoryService), ex);
      }
    }

    public ITfsGitRepository FindRepositoryByName(
      IVssRequestContext rc,
      string projectName,
      string repositoryName,
      bool includeDisabled = false,
      bool useCache = false)
    {
      IProjectService service = rc.GetService<IProjectService>();
      string teamProjectUri = (string) null;
      try
      {
        Guid result;
        if (Guid.TryParse(projectName, out result))
        {
          try
          {
            teamProjectUri = service.GetProject(rc, result).Uri;
          }
          catch (ProjectDoesNotExistException ex)
          {
          }
        }
        if (teamProjectUri == null)
          teamProjectUri = service.GetProject(rc, projectName).Uri;
      }
      catch (ProjectDoesNotExistException ex)
      {
        throw new GitRepositoryNotFoundException(repositoryName);
      }
      return this.FindRepositoryByNameAndUri(rc, teamProjectUri, repositoryName, false, includeDisabled, useCache);
    }

    public bool TryFindRepositoryByName(
      IVssRequestContext rc,
      string projectName,
      string repositoryName,
      out ITfsGitRepository repository,
      bool includeDisabled = false)
    {
      try
      {
        repository = this.FindRepositoryByName(rc, projectName, repositoryName, includeDisabled, false);
        return true;
      }
      catch (ProjectDoesNotExistWithNameException ex)
      {
        repository = (ITfsGitRepository) null;
        return false;
      }
      catch (GitRepositoryNotFoundException ex)
      {
        repository = (ITfsGitRepository) null;
        return false;
      }
    }

    public ITfsGitRepository FindRepositoryByNameAndUri(
      IVssRequestContext rc,
      string teamProjectUri,
      string repositoryName,
      bool allowReadByAdmins = false,
      bool includeDisabled = false,
      bool useCache = false)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(repositoryName, nameof (repositoryName), "git");
      if (teamProjectUri == null)
        throw new GitRepositoryNotFoundException(repositoryName);
      rc.Trace(1013054, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (TeamFoundationGitRepositoryService), "Finding Repository {0} With Name {1}", (object) teamProjectUri, (object) repositoryName);
      Guid result;
      if (Guid.TryParse(repositoryName, out result))
      {
        bool flag = false;
        ITfsGitRepository repositoryByNameAndUri = (ITfsGitRepository) null;
        try
        {
          repositoryByNameAndUri = this.FindRepositoryById(rc, result, allowReadByAdmins, includeDisabled, useCache);
          if (repositoryByNameAndUri.Key.GetProjectUri().Equals(teamProjectUri, StringComparison.OrdinalIgnoreCase))
          {
            flag = true;
            return repositoryByNameAndUri;
          }
        }
        catch (Exception ex)
        {
        }
        finally
        {
          if (repositoryByNameAndUri != null && !flag)
            repositoryByNameAndUri.Dispose();
        }
      }
      string canonicalName;
      bool createdByForking;
      long compressedSize;
      bool disabled;
      bool isInMaintenance;
      RepoKey repoKey;
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        repoKey = gitCoreComponent.RepoKeyFromName(ProjectInfo.GetProjectId(teamProjectUri), repositoryName, includeDisabled, out canonicalName, out createdByForking, out compressedSize, out disabled, out isInMaintenance);
      if ((RepoScope) repoKey == (RepoScope) null || string.IsNullOrEmpty(canonicalName))
        throw new GitRepositoryNotFoundException(repositoryName);
      SecurityHelper.Instance.CheckReadPermission(rc, (RepoScope) repoKey, repositoryName, allowReadByAdmins);
      return (ITfsGitRepository) this.m_dependencyRoot.CreateRepo(rc, canonicalName, repoKey, createdByForking, compressedSize, disabled, isInMaintenance);
    }

    public void UpdateRepositorySettingsById(
      IVssRequestContext rc,
      Guid repositoryId,
      GitRepoSettings repoSettings)
    {
      using (ITfsGitRepository repositoryById = this.FindRepositoryById(rc, repositoryId, false, true, false))
      {
        ITeamFoundationPolicyService service1 = rc.GetService<ITeamFoundationPolicyService>();
        IVssRegistryService service2 = rc.GetService<IVssRegistryService>();
        IContributedFeatureService service3 = rc.GetService<IContributedFeatureService>();
        new GitRepoSettingsProvider(rc, service2, service1, service3, repositoryById.Key).UpdateRepositorySettings(repoSettings);
      }
      TeamFoundationGitRepositoryService.InvalidateRepositoryCache(rc, repositoryId);
    }

    public bool GetIsRepositoryDisabledById(IVssRequestContext rc, Guid repositoryId)
    {
      ArgumentUtility.CheckForEmptyGuid(repositoryId, nameof (repositoryId), "git");
      rc.Trace(1013893, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (TeamFoundationGitRepositoryService), "Finding if repository with id {0} is disabled", (object) repositoryId);
      bool disabled;
      string str;
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        str = gitCoreComponent.RepositoryNameFromId(repositoryId, true, out RepoKey _, out bool _, out long _, out disabled, out bool _);
      if (string.IsNullOrEmpty(str))
        throw new GitRepositoryNotFoundException(repositoryId);
      return disabled;
    }

    public ITfsGitRepository FindRepositoryById(
      IVssRequestContext rc,
      Guid repositoryId,
      bool allowReadByAdmins = false,
      bool includeDisabled = false,
      bool useCache = false)
    {
      ArgumentUtility.CheckForEmptyGuid(repositoryId, nameof (repositoryId), "git");
      rc.Trace(1013055, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (TeamFoundationGitRepositoryService), "Finding Repository with id {0}", (object) repositoryId);
      TfsGitRepositoryBasicInfo repositoryBasicInfoById;
      if (rc.IsFeatureEnabled("Git.Server.EnableFindRepositoryByIdCaching") & useCache)
      {
        ConcurrentDictionary<Guid, TfsGitRepositoryBasicInfo> repositoryCache = TeamFoundationGitRepositoryService.GetRepositoryCache(rc);
        if (!repositoryCache.TryGetValue(repositoryId, out repositoryBasicInfoById))
        {
          rc.Trace(10130551, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (TeamFoundationGitRepositoryService), "FindRepositoryById cache miss. RepositoryId: {0}", (object) repositoryId);
          repositoryBasicInfoById = TeamFoundationGitRepositoryService.GetRepositoryBasicInfoById(rc, repositoryId, includeDisabled);
          repositoryCache[repositoryId] = repositoryBasicInfoById;
        }
        else
          rc.Trace(10130551, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (TeamFoundationGitRepositoryService), "FindRepositoryById cache hit. RepositoryId: {0}", (object) repositoryId);
      }
      else
        repositoryBasicInfoById = TeamFoundationGitRepositoryService.GetRepositoryBasicInfoById(rc, repositoryId, includeDisabled);
      if (string.IsNullOrEmpty(repositoryBasicInfoById.Name))
        throw new GitRepositoryNotFoundException(repositoryId);
      SecurityHelper.Instance.CheckReadPermission(rc, (RepoScope) repositoryBasicInfoById.Key, repositoryId.ToString(), allowReadByAdmins);
      SecurityHelper.Instance.CheckReadProjectPermission(rc, repositoryBasicInfoById.Key.GetProjectUri());
      return (ITfsGitRepository) this.m_dependencyRoot.CreateRepo(rc, repositoryBasicInfoById.Name, repositoryBasicInfoById.Key, repositoryBasicInfoById.IsFork, repositoryBasicInfoById.Size, repositoryBasicInfoById.IsDisabled, repositoryBasicInfoById.IsInMaintenance);
    }

    private static TfsGitRepositoryBasicInfo GetRepositoryBasicInfoById(
      IVssRequestContext rc,
      Guid repositoryId,
      bool includeDisabled)
    {
      RepoKey repoKey;
      bool createdByForking;
      long compressedSize;
      bool disabled;
      bool isInMaintenance;
      string name;
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        name = gitCoreComponent.RepositoryNameFromId(repositoryId, includeDisabled, out repoKey, out createdByForking, out compressedSize, out disabled, out isInMaintenance);
      return new TfsGitRepositoryBasicInfo(name, repoKey, createdByForking, compressedSize, disabled, isInMaintenance);
    }

    private static ConcurrentDictionary<Guid, TfsGitRepositoryBasicInfo> GetRepositoryCache(
      IVssRequestContext rc)
    {
      if (!rc.IsFeatureEnabled("Git.Server.EnableFindRepositoryByIdCaching"))
        return new ConcurrentDictionary<Guid, TfsGitRepositoryBasicInfo>();
      ConcurrentDictionary<Guid, TfsGitRepositoryBasicInfo> repositoryCache;
      if (!rc.TryGetItem<ConcurrentDictionary<Guid, TfsGitRepositoryBasicInfo>>("TfsGitServer.TfsGitRepositoryCache", out repositoryCache))
      {
        repositoryCache = new ConcurrentDictionary<Guid, TfsGitRepositoryBasicInfo>();
        rc.Items.TryAdd<string, object>("TfsGitServer.TfsGitRepositoryCache", (object) repositoryCache);
      }
      return repositoryCache;
    }

    private static bool InvalidateRepositoryCache(IVssRequestContext rc, Guid repositoryId) => !rc.IsFeatureEnabled("Git.Server.EnableFindRepositoryByIdCaching") || TeamFoundationGitRepositoryService.GetRepositoryCache(rc).TryRemove(repositoryId, out TfsGitRepositoryBasicInfo _);

    public bool TryFindRepositoryById(
      IVssRequestContext rc,
      Guid repositoryId,
      bool allowReadByAdmins,
      out ITfsGitRepository repo)
    {
      try
      {
        repo = this.FindRepositoryById(rc, repositoryId, allowReadByAdmins, false, false);
        return true;
      }
      catch (GitRepositoryNotFoundException ex)
      {
        repo = (ITfsGitRepository) null;
        return false;
      }
    }

    public IList<TfsGitRepositoryInfo> QueryRepositories(
      IVssRequestContext rc,
      string projectUriFilter = null,
      bool excludeHiddenRepos = false)
    {
      rc.Trace(1013056, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (TeamFoundationGitRepositoryService), "Looking for repositories");
      ICollection<TfsGitRepositoryInfo> gitRepositoryInfos;
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        gitRepositoryInfos = (ICollection<TfsGitRepositoryInfo>) gitCoreComponent.QueryRepositories(excludeHiddenRepos);
      Dictionary<string, bool> cachedVisibleProjects = new Dictionary<string, bool>();
      List<TfsGitRepositoryInfo> gitRepositoryInfoList = new List<TfsGitRepositoryInfo>();
      foreach (TfsGitRepositoryInfo gitRepositoryInfo in (IEnumerable<TfsGitRepositoryInfo>) gitRepositoryInfos)
      {
        if (projectUriFilter == null || projectUriFilter.Equals(gitRepositoryInfo.Key.GetProjectUri(), StringComparison.InvariantCultureIgnoreCase))
        {
          string projectUri = gitRepositoryInfo.Key.GetProjectUri();
          if (SecurityHelper.Instance.HasReadPermission(rc, (RepoScope) gitRepositoryInfo.Key, true) && this.HasReadProjectPermission((IDictionary<string, bool>) cachedVisibleProjects, rc, projectUri))
          {
            rc.Trace(1013057, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (TeamFoundationGitRepositoryService), "Found repository in Team Project {0} with id {1}", (object) gitRepositoryInfo.Key.ProjectId, (object) gitRepositoryInfo.Key.RepoId);
            gitRepositoryInfoList.Add(gitRepositoryInfo);
          }
        }
      }
      return (IList<TfsGitRepositoryInfo>) gitRepositoryInfoList;
    }

    public IEnumerable<TfsGitRepositoryInfo> QueryRepositoriesAcrossProjects(
      IVssRequestContext rc,
      IEnumerable<Guid> repoIds)
    {
      IReadOnlyList<TfsGitRepositoryInfo> repoInfos;
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        repoInfos = gitCoreComponent.QueryRepositoriesAcrossProjects(repoIds);
      return this.FilterUserVisibleRepositories(rc, repoInfos);
    }

    public IEnumerable<TfsGitRepositoryInfo> QueryRepositories(
      IVssRequestContext rc,
      Guid projectId,
      IEnumerable<Guid> repoIds)
    {
      IReadOnlyList<TfsGitRepositoryInfo> repoInfos;
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        repoInfos = gitCoreComponent.QueryRepositories(projectId, repoIds);
      return this.FilterUserVisibleRepositories(rc, repoInfos);
    }

    IEnumerable<TfsGitRepositoryInfo> IInternalGitRepoService.QueryRepositoriesByOdbId(
      IVssRequestContext rc,
      OdbId odb)
    {
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        return gitCoreComponent.QueryRepositories().Where<TfsGitRepositoryInfo>((Func<TfsGitRepositoryInfo, bool>) (x => x.Key.OdbId == odb));
    }

    private IEnumerable<TfsGitRepositoryInfo> FilterUserVisibleRepositories(
      IVssRequestContext rc,
      IReadOnlyList<TfsGitRepositoryInfo> repoInfos)
    {
      Dictionary<string, bool> cachedVisibleProjects = new Dictionary<string, bool>();
      foreach (TfsGitRepositoryInfo repoInfo in (IEnumerable<TfsGitRepositoryInfo>) repoInfos)
      {
        if (SecurityHelper.Instance.HasReadPermission(rc, (RepoScope) repoInfo.Key, true) && this.HasReadProjectPermission((IDictionary<string, bool>) cachedVisibleProjects, rc, repoInfo.Key.GetProjectUri()))
          yield return repoInfo;
      }
    }

    private bool HasReadProjectPermission(
      IDictionary<string, bool> cachedVisibleProjects,
      IVssRequestContext rc,
      string projectUri)
    {
      bool flag;
      if (!cachedVisibleProjects.TryGetValue(projectUri, out flag))
      {
        flag = SecurityHelper.Instance.HasReadProjectPermission(rc, projectUri);
        cachedVisibleProjects.Add(projectUri, flag);
      }
      return flag;
    }

    public IList<TfsGitDeletedRepositoryInfo> QueryDeletedRepositories(
      IVssRequestContext rc,
      Guid? projectId,
      bool isSoftDeletedOnly = false)
    {
      rc.GetService<IProjectService>().CheckProjectPermission(rc, projectId.HasValue ? ProjectInfo.GetProjectUri(projectId.Value) : (string) null, TeamProjectPermissions.GenericWrite);
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        return gitCoreComponent.QueryDeletedRepositories(projectId, isSoftDeletedOnly);
    }

    public ITfsGitRepository RepositoryFromRepositoryInfo(
      IVssRequestContext rc,
      TfsGitRepositoryInfo repositoryInfo)
    {
      ArgumentUtility.CheckForNull<TfsGitRepositoryInfo>(repositoryInfo, nameof (repositoryInfo));
      return (ITfsGitRepository) this.m_dependencyRoot.CreateRepo(rc, repositoryInfo.Name, repositoryInfo.Key, repositoryInfo.IsFork, repositoryInfo.Size, isInMaintenance: repositoryInfo.IsInMaintenance);
    }

    public IEnumerable<TfsGitRepositoryInfo> QueryRepositoriesUpdatedSinceLastWatermark(
      IVssRequestContext rc,
      DateTime updatedTime,
      int batchSize)
    {
      ArgumentUtility.CheckForOutOfRange(batchSize, nameof (batchSize), 0, 10000);
      IReadOnlyList<TfsGitRepositoryInfo> repoInfos;
      using (GitCoreComponent gitCoreComponent = rc.CreateGitCoreComponent())
        repoInfos = (IReadOnlyList<TfsGitRepositoryInfo>) gitCoreComponent.QueryRepositoriesUpdatedSinceLastWatermark(updatedTime, batchSize);
      return this.FilterUserVisibleRepositories(rc, repoInfos);
    }

    public void SetGitRepoMaintenanceFlagByObdId(
      IVssRequestContext requestContext,
      Guid odbId,
      bool state)
    {
      using (GitCoreComponent gitCoreComponent = requestContext.CreateGitCoreComponent())
        gitCoreComponent.SetGitRepoMaintenanceFlag(odbId, state);
    }

    public IRepoPermissionsManager GetOrgLevelRepoPermissionsManager(
      IVssRequestContext requestContext)
    {
      return (IRepoPermissionsManager) new RepoPermissionsManager(requestContext, new RepoScope(Guid.Empty, Guid.Empty));
    }

    public IRepoPermissionsManager GetProjectLevelRepoPermissionsManager(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return (IRepoPermissionsManager) new RepoPermissionsManager(requestContext, new RepoScope(projectId, Guid.Empty));
    }

    private static void PublishNotifications(
      IVssRequestContext rc,
      string repoName,
      MinimalGlobalRepoKey? forkSourceRepo,
      string teamProjectUri,
      RepoKey repoKey)
    {
      rc.GetService<ITeamFoundationEventService>().PublishNotification(rc, (object) new RepositoryCreatedNotification(teamProjectUri, repoKey.RepoId, repoName, rc.UserContext));
    }

    private static void PublishClientTraces(
      IVssRequestContext rc,
      string repoName,
      string teamProjectUri,
      RepoKey repoKey)
    {
      ClientTraceData properties = new ClientTraceData();
      properties.Add("Action", (object) "CreateRepository");
      properties.Add("TeamProjectUri", (object) teamProjectUri);
      properties.Add("RepositoryId", (object) repoKey.RepoId.ToString());
      properties.Add("RepositoryName", (object) repoName);
      rc.GetService<ClientTraceService>().Publish(rc, "Microsoft.TeamFoundation.Git.Server", "TeamProject", properties);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_repoScopedJobs != null)
      {
        this.m_repoScopedJobs.Dispose();
        this.m_repoScopedJobs = (IDisposableReadOnlyList<GitKeyedJob<RepoJobKey>>) null;
      }
      if (this.m_odbScopedJobs == null)
        return;
      this.m_odbScopedJobs.Dispose();
      this.m_odbScopedJobs = (IDisposableReadOnlyList<GitKeyedJob<OdbJobKey>>) null;
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_repoScopedJobs = systemRequestContext.GetExtensions<GitKeyedJob<RepoJobKey>>();
      this.m_odbScopedJobs = systemRequestContext.GetExtensions<GitKeyedJob<OdbJobKey>>();
    }
  }
}
