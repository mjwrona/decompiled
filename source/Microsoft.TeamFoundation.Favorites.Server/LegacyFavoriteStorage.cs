// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Favorites.LegacyFavoriteStorage
// Assembly: Microsoft.TeamFoundation.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4742830F-DF0E-4509-8C2D-2540DAED73F4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Favorites.Server;
using Microsoft.VisualStudio.Services.Favorites.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Favorites
{
  public class LegacyFavoriteStorage : IExtendFavoriteStorage, IStoreFavorites
  {
    private FavoriteStoreClientFactory m_legacyStoreClientFactory;

    public LegacyFavoriteStorage() => this.m_legacyStoreClientFactory = new FavoriteStoreClientFactory();

    internal LegacyFavoriteStorage(FavoriteStoreClientFactory legacyStoreFacade) => this.m_legacyStoreClientFactory = legacyStoreFacade;

    public bool IsSupportedScope(IVssRequestContext requestContext, string scope) => LegacyStorageRegistration.GetLegacyFavoriteScopes(requestContext).Contains<string>(scope);

    public bool IsSupportedType(IVssRequestContext requestContext, string type) => LegacyStorageRegistration.GetLegacyFavoriteTypes(requestContext).Contains<string>(type);

    public string GetScopeOfType(IVssRequestContext requestContext, string type) => LegacyStorageRegistration.GetTypeToScopeMapping(requestContext)[type];

    public IEnumerable<Favorite> GetFavorites(
      IVssRequestContext requestContext,
      OwnerScope ownerScope)
    {
      ownerScope.ThrowIfUserHasInadequatePermissions(requestContext, FavoriteAccessMode.Read);
      return this.m_legacyStoreClientFactory.GetAccountStore(requestContext, ownerScope).GetAccountScopedFavorites(requestContext, ownerScope).Where<TfsFavorite>((Func<TfsFavorite, bool>) (o => this.IsSupportedType(requestContext, o.Type) && this.IsSupportedScope(requestContext, o.Scope))).Select<TfsFavorite, Favorite>((Func<TfsFavorite, Favorite>) (o => o.ToModernFavorite(ownerScope.GetIdentityRef(requestContext))));
    }

    public void DeleteFavoriteByFavoriteId(
      IVssRequestContext requestContext,
      ArtifactScope artifactScope,
      string artifactType,
      Guid favoriteId,
      OwnerScope ownerScope)
    {
      ownerScope.ThrowIfUserHasInadequatePermissions(requestContext, FavoriteAccessMode.Write);
      if (!this.IsSupportedType(requestContext, artifactType))
        throw new UnsupportedLegacyFavoriteScopeException(FavoritesWebApiResources.UnsupportedLegacyFavoriteScopeDeletionExceptionMessage());
      List<Guid> favIds = new List<Guid>();
      favIds.Add(favoriteId);
      Guid projectGuid = Guid.Parse(artifactScope.Id);
      string scopeOfType = this.GetScopeOfType(requestContext, artifactType);
      IProjectIdentityFavorites projectStore = this.m_legacyStoreClientFactory.GetProjectStore(requestContext, ownerScope, projectGuid, scopeOfType);
      projectStore.DeleteFavoriteItems(requestContext, (IEnumerable<Guid>) favIds);
      projectStore.Update(requestContext);
    }

    public void DeleteFavoriteByArtifact(
      IVssRequestContext requestContext,
      string artifactType,
      string artifactId,
      OwnerScope ownerScope)
    {
      ownerScope.ThrowIfUserHasInadequatePermissions(requestContext, FavoriteAccessMode.Write);
      if (!this.IsSupportedType(requestContext, artifactType))
        throw new UnsupportedLegacyFavoriteScopeException(FavoritesWebApiResources.UnsupportedLegacyFavoriteScopeDeletionExceptionMessage());
      IEnumerable<TfsFavorite> source = this.m_legacyStoreClientFactory.GetAccountStore(requestContext, ownerScope).GetAccountScopedFavorites(requestContext, ownerScope).Where<TfsFavorite>((Func<TfsFavorite, bool>) (o => o.Type == artifactType && o.Data == artifactId));
      if (source == null)
        return;
      TfsFavorite tfsFavorite = source.Count<TfsFavorite>() <= 1 ? source.First<TfsFavorite>() : throw new AmbiguousFavoriteDeleteException(FavoritesWebApiResources.AmbiguousFavoriteDeleteExceptionMessage());
      List<Guid> favIds = new List<Guid>();
      favIds.Add(tfsFavorite.Id.Value);
      IProjectIdentityFavorites projectStore = this.m_legacyStoreClientFactory.GetProjectStore(requestContext, ownerScope, tfsFavorite.ProjectId, tfsFavorite.Scope);
      projectStore.DeleteFavoriteItems(requestContext, (IEnumerable<Guid>) favIds);
      projectStore.Update(requestContext);
    }

    private IEnumerable<TfsFavorite> UpdateFavoritesPartitionedByProjectScope(
      IVssRequestContext requestContext,
      OwnerScope owner,
      Guid projectGuid,
      string scopeKey,
      IEnumerable<TfsFavorite> favoriteList)
    {
      if (!favoriteList.Any<TfsFavorite>((Func<TfsFavorite, bool>) (f => this.IsSupportedScope(requestContext, f.Scope))))
        throw new UnsupportedLegacyFavoriteScopeException(FavoritesWebApiResources.UnsupportedLegacyFavoriteScopeUpdateExceptionMessage());
      foreach (TfsFavorite favorite in favoriteList)
      {
        if (favorite.Id.HasValue)
        {
          Guid? id = favorite.Id;
          Guid empty = Guid.Empty;
          if ((id.HasValue ? (id.HasValue ? (id.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
            continue;
        }
        favorite.Id = new Guid?(Guid.NewGuid());
      }
      IProjectIdentityFavorites projectStore = this.m_legacyStoreClientFactory.GetProjectStore(requestContext, owner, projectGuid, scopeKey);
      projectStore.UpdateFavoriteItems(requestContext, favoriteList.Select<TfsFavorite, FavoriteItem>((Func<TfsFavorite, FavoriteItem>) (o => o.ConvertToFavoriteItem())));
      projectStore.Update(requestContext);
      return favoriteList;
    }

    public IEnumerable<Favorite> UpdateFavorites(
      IVssRequestContext requestContext,
      IEnumerable<Favorite> favoriteList,
      OwnerScope ownerScope)
    {
      ownerScope.ThrowIfUserHasInadequatePermissions(requestContext, FavoriteAccessMode.Write);
      FavoritesOwnerMismatchException.ThrowIfFavoritesAreNotOwnedByUser(ownerScope.OwnerId, favoriteList);
      IEnumerable<TfsFavorite> source1 = favoriteList.Select<Favorite, TfsFavorite>((Func<Favorite, TfsFavorite>) (o => o.ToLegacyFavorite(requestContext)));
      foreach (TfsFavorite tfsFavorite in source1)
        tfsFavorite.Validate();
      List<TfsFavorite> source2 = new List<TfsFavorite>();
      foreach (IGrouping<Guid, TfsFavorite> source3 in source1.GroupBy<TfsFavorite, Guid>((Func<TfsFavorite, Guid>) (x => x.ProjectId)))
      {
        foreach (IGrouping<string, TfsFavorite> favoriteList1 in source3.GroupBy<TfsFavorite, string>((Func<TfsFavorite, string>) (x => x.Scope)))
        {
          string key = favoriteList1.Key;
          source2.AddRange(this.UpdateFavoritesPartitionedByProjectScope(requestContext, ownerScope, source3.Key, key, (IEnumerable<TfsFavorite>) favoriteList1));
        }
      }
      return source2.Select<TfsFavorite, Favorite>((Func<TfsFavorite, Favorite>) (o => o.ToModernFavorite(ownerScope.GetIdentityRef(requestContext))));
    }
  }
}
