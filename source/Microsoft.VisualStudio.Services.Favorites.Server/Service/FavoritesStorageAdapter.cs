// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.Server.Service.FavoritesStorageAdapter
// Assembly: Microsoft.VisualStudio.Services.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EC45EF48-58C1-4A3C-8054-12C79AD0CC5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Favorites.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Favorites.Server.Service
{
  internal class FavoritesStorageAdapter : IStoreFavorites
  {
    private IExtendFavoriteStorage m_pluginFavoriteStorage;
    private IStoreFavorites m_defaultFavoriteStorage;

    public FavoritesStorageAdapter(IVssRequestContext requestContext)
    {
      this.m_pluginFavoriteStorage = this.m_pluginFavoriteStorage ?? requestContext.GetExtension<IExtendFavoriteStorage>(ExtensionLifetime.Service, throwOnError: true);
      this.m_defaultFavoriteStorage = this.m_defaultFavoriteStorage ?? (IStoreFavorites) new DefaultFavoriteStorage();
    }

    public void DeleteFavoriteByArtifact(
      IVssRequestContext requestContext,
      string artifactType,
      string artifactId,
      OwnerScope ownerScope)
    {
      this.SelectStoreByType(requestContext, artifactType).DeleteFavoriteByArtifact(requestContext, artifactType, artifactId, ownerScope);
    }

    public void DeleteFavoriteByFavoriteId(
      IVssRequestContext requestContext,
      ArtifactScope artifactScope,
      string artifactType,
      Guid favoriteId,
      OwnerScope ownerScope)
    {
      this.SelectStoreByType(requestContext, artifactType).DeleteFavoriteByFavoriteId(requestContext, artifactScope, artifactType, favoriteId, ownerScope);
    }

    public IEnumerable<Favorite> GetFavorites(
      IVssRequestContext requestContext,
      OwnerScope ownerScope)
    {
      IEnumerable<Favorite> source = this.m_defaultFavoriteStorage.GetFavorites(requestContext, ownerScope);
      if (this.m_pluginFavoriteStorage != null)
        source = (IEnumerable<Favorite>) source.Where<Favorite>((Func<Favorite, bool>) (o => !this.m_pluginFavoriteStorage.IsSupportedType(requestContext, o.ArtifactType))).Union<Favorite>(this.m_pluginFavoriteStorage.GetFavorites(requestContext, ownerScope)).ToList<Favorite>();
      return source;
    }

    public IEnumerable<Favorite> UpdateFavorites(
      IVssRequestContext requestContext,
      IEnumerable<Favorite> favoritelist,
      OwnerScope ownerScope)
    {
      IEnumerable<Favorite> first = (IEnumerable<Favorite>) new List<Favorite>();
      foreach (IGrouping<IStoreFavorites, Favorite> favoritelist1 in favoritelist.GroupBy<Favorite, IStoreFavorites>((Func<Favorite, IStoreFavorites>) (o => this.SelectStoreByType(requestContext, o.ArtifactType))))
        first = first.Union<Favorite>(favoritelist1.Key.UpdateFavorites(requestContext, (IEnumerable<Favorite>) favoritelist1, ownerScope));
      return first;
    }

    private IStoreFavorites SelectStoreByType(IVssRequestContext requestContext, string type) => this.m_pluginFavoriteStorage != null && this.m_pluginFavoriteStorage.IsSupportedType(requestContext, type) ? (IStoreFavorites) this.m_pluginFavoriteStorage : this.m_defaultFavoriteStorage;
  }
}
