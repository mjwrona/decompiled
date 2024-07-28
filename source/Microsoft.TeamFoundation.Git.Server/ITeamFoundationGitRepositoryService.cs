// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ITeamFoundationGitRepositoryService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationGitRepositoryService))]
  public interface ITeamFoundationGitRepositoryService : IVssFrameworkService
  {
    ITfsGitRepository CreateTeamProjectRepository(
      IVssRequestContext requestContext,
      Guid teamProjectId,
      string repositoryName,
      IEnumerable<IAccessControlEntry> permissions);

    ITfsGitRepository CreateRepository(
      IVssRequestContext requestContext,
      Guid teamProjectId,
      string repositoryName,
      IEnumerable<IAccessControlEntry> permissions,
      IEnumerable<IAccessControlEntry> permissionsForAllReposIfNotSet = null,
      bool isHidden = false);

    ITfsGitRepository RenameRepository(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      string newRepositoryName);

    ITfsGitRepository UpdateRepository(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      string newRepositoryName = null,
      string newDefaultBranch = null);

    void DeleteRepositories(IVssRequestContext requestContext, RepoScope toDelete);

    ITfsGitRepository UndeleteRepository(IVssRequestContext requestContext, RepoKey toRecover);

    void DestroyRepositories(IVssRequestContext requestContext, RepoScope toDestory);

    void DisableRepository(IVssRequestContext rc, RepoScope toDisable);

    void EnableRepository(IVssRequestContext rc, RepoScope toEnable);

    ITfsGitRepository FindRepositoryByName(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      bool includeDisabled = false,
      bool useCache = false);

    bool TryFindRepositoryByName(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryName,
      out ITfsGitRepository repository,
      bool includeDisabled = false);

    ITfsGitRepository FindRepositoryByNameAndUri(
      IVssRequestContext requestContext,
      string teamProjectUri,
      string repositoryName,
      bool allowReadByAdmins = false,
      bool includeDisabled = false,
      bool useCache = false);

    void UpdateRepositorySettingsById(
      IVssRequestContext requestContext,
      Guid repositoryId,
      GitRepoSettings repoSettings);

    void SetGitRepoMaintenanceFlagByObdId(
      IVssRequestContext requestContext,
      Guid odbId,
      bool isInMaintenance);

    bool GetIsRepositoryDisabledById(IVssRequestContext rc, Guid repositoryId);

    ITfsGitRepository FindRepositoryById(
      IVssRequestContext requestContext,
      Guid repositoryId,
      bool allowReadByAdmins = false,
      bool includeDisabled = false,
      bool useCache = false);

    bool TryFindRepositoryById(
      IVssRequestContext requestContext,
      Guid repositoryId,
      bool allowReadByAdmins,
      out ITfsGitRepository repo);

    IList<TfsGitRepositoryInfo> QueryRepositories(
      IVssRequestContext requestContext,
      string projectUriFilter = null,
      bool excludeHiddenRepos = false);

    IEnumerable<TfsGitRepositoryInfo> QueryRepositories(
      IVssRequestContext rc,
      Guid projectId,
      IEnumerable<Guid> repoIds);

    IEnumerable<TfsGitRepositoryInfo> QueryRepositoriesAcrossProjects(
      IVssRequestContext rc,
      IEnumerable<Guid> repoIds);

    IList<TfsGitDeletedRepositoryInfo> QueryDeletedRepositories(
      IVssRequestContext requestContext,
      Guid? projectId,
      bool isSoftDeletedOnly = false);

    ITfsGitRepository RepositoryFromRepositoryInfo(
      IVssRequestContext requestContext,
      TfsGitRepositoryInfo repositoryInfo);

    IEnumerable<TfsGitRepositoryInfo> QueryRepositoriesUpdatedSinceLastWatermark(
      IVssRequestContext requestContext,
      DateTime updatedTime,
      int batchSize);

    IRepoPermissionsManager GetOrgLevelRepoPermissionsManager(IVssRequestContext requestConext);

    IRepoPermissionsManager GetProjectLevelRepoPermissionsManager(
      IVssRequestContext requestContext,
      Guid projectId);
  }
}
