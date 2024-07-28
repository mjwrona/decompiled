// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.RefFavoritesManager
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Favorites.Server.Service;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class RefFavoritesManager : IRefFavoritesManager
  {
    private readonly IVssRequestContext m_requestContext;

    public RefFavoritesManager(IVssRequestContext requestContext) => this.m_requestContext = requestContext;

    public TfsGitRefFavorite CreateFavorite(
      Guid identityId,
      ITfsGitRepository repo,
      string name,
      bool isFolder)
    {
      ArgumentUtility.CheckForEmptyGuid(identityId, nameof (identityId));
      ArgumentUtility.CheckForNull<ITfsGitRepository>(repo, nameof (repo));
      ArgumentUtility.CheckForNull<string>(name, nameof (name));
      this.CheckFavoritePermission(identityId, repo);
      using (GitCoreComponent gitCoreComponent = this.m_requestContext.CreateGitCoreComponent())
        return new TfsGitRefFavorite(gitCoreComponent.CreateRefFavorite(identityId, repo.Key, name, isFolder), identityId, repo.Key, name, isFolder);
    }

    public void DeleteFavorite(Guid projectId, int favoriteId)
    {
      this.GetFavorite(projectId, favoriteId);
      using (GitCoreComponent gitCoreComponent = this.m_requestContext.CreateGitCoreComponent())
        gitCoreComponent.DeleteRefFavorite(projectId, favoriteId);
    }

    public TfsGitRefFavorite GetFavorite(Guid projectId, int favoriteId)
    {
      FavoritesPermissionHelper.CheckCanUseMyFavorites(this.m_requestContext, new Guid?(projectId));
      TfsGitRefFavorite refFavorite;
      using (GitCoreComponent gitCoreComponent = this.m_requestContext.CreateGitCoreComponent())
        refFavorite = gitCoreComponent.GetRefFavorite(projectId, favoriteId);
      if (refFavorite != null)
      {
        using (ITfsGitRepository repositoryById = this.m_requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(this.m_requestContext, refFavorite.RepoKey.RepoId))
          this.CheckFavoritePermission(refFavorite.IdentityId, repositoryById);
      }
      return refFavorite;
    }

    public List<TfsGitRefFavorite> GetFavorites(Guid favoritesIdentity, ITfsGitRepository repo)
    {
      this.CheckFavoritePermission(favoritesIdentity, repo);
      using (GitCoreComponent gitCoreComponent = this.m_requestContext.CreateGitCoreComponent())
        return gitCoreComponent.GetRefFavorites(favoritesIdentity, repo.Key);
    }

    public List<TfsGitRefFavorite> GetFavoritesForProject(Guid projectId, Guid favoritesIdentity)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(favoritesIdentity, nameof (favoritesIdentity));
      FavoritesPermissionHelper.CheckCanUseMyFavorites(this.m_requestContext, new Guid?(projectId));
      using (GitCoreComponent gitCoreComponent = this.m_requestContext.CreateGitCoreComponent())
        return gitCoreComponent.GetRefFavoritesForProject(projectId, favoritesIdentity);
    }

    private void CheckFavoritePermission(Guid favoritesIdentity, ITfsGitRepository repo)
    {
      FavoritesPermissionHelper.CheckCanUseMyFavorites(this.m_requestContext, new Guid?(repo.Key.ProjectId));
      Guid userId = this.m_requestContext.GetUserId();
      if (!(favoritesIdentity != userId))
        return;
      SecurityHelper.Instance.CheckEditPoliciesPermission(this.m_requestContext, (RepoScope) repo.Key);
    }
  }
}
