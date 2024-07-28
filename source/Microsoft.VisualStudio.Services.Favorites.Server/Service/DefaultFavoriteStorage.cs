// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.Server.Service.DefaultFavoriteStorage
// Assembly: Microsoft.VisualStudio.Services.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EC45EF48-58C1-4A3C-8054-12C79AD0CC5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Favorites.Server.DataAccess;
using Microsoft.VisualStudio.Services.Favorites.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Favorites.Server.Service
{
  public class DefaultFavoriteStorage : IStoreFavorites, IAdministerFavorites
  {
    public void DeleteFavoriteByFavoriteId(
      IVssRequestContext requestContext,
      ArtifactScope artifactScope,
      string artifactType,
      Guid favoriteId,
      OwnerScope ownerScope)
    {
      ownerScope.ThrowIfUserHasInadequatePermissions(requestContext, FavoriteAccessMode.Write);
      using (FavoriteSqlResourceComponent component = requestContext.CreateComponent<FavoriteSqlResourceComponent>())
        component.DeleteFavoriteById(ownerScope.OwnerId, favoriteId);
    }

    public void DeleteFavoriteByArtifact(
      IVssRequestContext requestContext,
      string artifactType,
      string artifactId,
      OwnerScope ownerScope)
    {
      ownerScope.ThrowIfUserHasInadequatePermissions(requestContext, FavoriteAccessMode.Write);
      using (FavoriteSqlResourceComponent component = requestContext.CreateComponent<FavoriteSqlResourceComponent>())
      {
        IEnumerable<Favorite> source = component.GetFavoritesByOwner(ownerScope.OwnerId).Where<Favorite>((Func<Favorite, bool>) (o => o.ArtifactType == artifactType && o.ArtifactId == artifactId));
        if (source == null || !source.Any<Favorite>())
          return;
        if (source.Count<Favorite>() > 1)
          throw new AmbiguousFavoriteDeleteException(FavoritesWebApiResources.AmbiguousFavoriteDeleteExceptionMessage());
        component.DeleteFavoriteById(ownerScope.OwnerId, source.First<Favorite>().Id.Value);
      }
    }

    public IEnumerable<Favorite> GetFavorites(
      IVssRequestContext requestContext,
      OwnerScope ownerScope)
    {
      ownerScope.ThrowIfUserHasInadequatePermissions(requestContext, FavoriteAccessMode.Read);
      if (DefaultFavoriteStorage.CheckIfFavoriteServiceDoesntExists(requestContext))
        return Enumerable.Empty<Favorite>();
      using (FavoriteSqlResourceComponent component = requestContext.CreateComponent<FavoriteSqlResourceComponent>())
        return (IEnumerable<Favorite>) component.GetFavoritesByOwner(ownerScope.OwnerId);
    }

    public IEnumerable<Favorite> UpdateFavorites(
      IVssRequestContext requestContext,
      IEnumerable<Favorite> favoriteList,
      OwnerScope ownerScope)
    {
      ownerScope.ThrowIfUserHasInadequatePermissions(requestContext, FavoriteAccessMode.Write);
      FavoritesOwnerMismatchException.ThrowIfFavoritesAreNotOwnedByUser(ownerScope.OwnerId, favoriteList);
      return this.CreateUpdateFavorites(requestContext, ownerScope, favoriteList);
    }

    public IEnumerable<Favorite> CreateUpdateFavorites(
      IVssRequestContext requestContext,
      OwnerScope owner,
      IEnumerable<Favorite> favoriteList)
    {
      if (DefaultFavoriteStorage.CheckIfFavoriteServiceDoesntExists(requestContext))
        return Enumerable.Empty<Favorite>();
      using (FavoriteSqlResourceComponent component = requestContext.CreateComponent<FavoriteSqlResourceComponent>())
        return (IEnumerable<Favorite>) component.CreateUpdateFavorites(owner.OwnerId, owner.IsTeam, favoriteList);
    }

    private static bool CheckIfFavoriteServiceDoesntExists(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationResourceManagementService>().GetServiceVersion(requestContext, "Favorite", "Default").Version == 0;
  }
}
