// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.Server.FavoriteController
// Assembly: Microsoft.VisualStudio.Services.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EC45EF48-58C1-4A3C-8054-12C79AD0CC5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Favorites.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Favorites.Server
{
  [VersionedApiControllerCustomName("Favorite", "Favorites", 1)]
  public class FavoriteController : TfsApiController, IOverrideLoggingMethodNames
  {
    public override string TraceArea => "Favorites";

    public override string ActivityLogArea => "Favorites";

    [TraceFilter(15160001, 15160002)]
    [HttpGet]
    [ClientExample("GET__favorites.json", null, null, null)]
    public IEnumerable<Favorite> GetFavorites(
      string artifactType = null,
      string artifactScopeType = null,
      string artifactScopeId = null,
      bool includeExtendedDetails = false)
    {
      return this.GetFavoritesImpl(OwnerScope.Self(this.TfsRequestContext), artifactType, artifactScopeType, artifactScopeId, includeExtendedDetails);
    }

    [TraceFilter(15160001, 15160002)]
    [HttpGet]
    [ClientExample("GET__favoritesOfOwner.json", null, null, null)]
    public IEnumerable<Favorite> GetFavoritesOfOwner(
      string ownerScopeType,
      Guid ownerScopeId,
      string artifactType = null,
      string artifactScopeType = null,
      string artifactScopeId = null,
      bool includeExtendedDetails = false)
    {
      return this.GetFavoritesImpl(OwnerScope.Create(this.TfsRequestContext, ownerScopeType, ownerScopeId), artifactType, artifactScopeType, artifactScopeId, includeExtendedDetails);
    }

    private IEnumerable<Favorite> GetFavoritesImpl(
      OwnerScope ownerScope,
      string artifactType = null,
      string artifactScopeType = null,
      string artifactScopeId = null,
      bool includeExtendedDetails = false)
    {
      FavoriteFilter filter = new FavoriteFilter()
      {
        Type = artifactType
      };
      if (artifactScopeType != null && artifactScopeId != null)
        filter.ArtifactScope = new ArtifactScope(artifactScopeType, artifactScopeId);
      return this.TfsRequestContext.GetService<IFavoriteService>().GetFavorites(this.TfsRequestContext, filter, includeExtendedDetails, ownerScope);
    }

    [TraceFilter(15160003, 15160004)]
    [HttpDelete]
    [ClientExample("DELETE__favoriteById.json", null, null, null)]
    public void DeleteFavoriteById(
      Guid favoriteId,
      string artifactType,
      string artifactScopeType,
      string artifactScopeId = null)
    {
      this.DeleteFavoriteByIdImpl(OwnerScope.Self(this.TfsRequestContext), favoriteId, artifactType, artifactScopeType, artifactScopeId);
    }

    [TraceFilter(15160003, 15160004)]
    [HttpDelete]
    [ClientExample("DELETE__favoriteOfOwnerById.json", null, null, null)]
    public void DeleteFavoriteOfOwnerById(
      string ownerScopeType,
      Guid ownerScopeId,
      Guid favoriteId,
      string artifactType,
      string artifactScopeType,
      string artifactScopeId = null)
    {
      this.DeleteFavoriteByIdImpl(OwnerScope.Create(this.TfsRequestContext, ownerScopeType, ownerScopeId), favoriteId, artifactType, artifactScopeType, artifactScopeId);
    }

    private void DeleteFavoriteByIdImpl(
      OwnerScope ownerScope,
      Guid favoriteId,
      string artifactType,
      string artifactScopeType,
      string artifactScopeId = null)
    {
      IFavoriteService service = this.TfsRequestContext.GetService<IFavoriteService>();
      ArtifactScope artifactScope1 = new ArtifactScope(artifactScopeType, artifactScopeId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      ArtifactScope artifactScope2 = artifactScope1;
      string artifactType1 = artifactType;
      Guid favoriteId1 = favoriteId;
      OwnerScope ownerScope1 = ownerScope;
      service.DeleteFavorite(tfsRequestContext, artifactScope2, artifactType1, favoriteId1, ownerScope1);
    }

    [TraceFilter(15160016, 15160016)]
    [HttpGet]
    [ClientExample("GET__favoriteById.json", null, null, null)]
    public Favorite GetFavoriteById(
      Guid favoriteId,
      string artifactScopeType,
      string artifactType,
      string artifactScopeId = null,
      bool includeExtendedDetails = false)
    {
      return this.GetFavoriteByIdImpl(OwnerScope.Self(this.TfsRequestContext), favoriteId, artifactScopeType, artifactType, artifactScopeId, includeExtendedDetails);
    }

    [TraceFilter(15160016, 15160016)]
    [HttpGet]
    [ClientExample("GET__favoritesOfOwnerById.json", null, null, null)]
    public Favorite GetFavoriteOfOwnerById(
      string ownerScopeType,
      Guid ownerScopeId,
      Guid favoriteId,
      string artifactScopeType,
      string artifactType,
      string artifactScopeId = null,
      bool includeExtendedDetails = false)
    {
      return this.GetFavoriteByIdImpl(OwnerScope.Create(this.TfsRequestContext, ownerScopeType, ownerScopeId), favoriteId, artifactScopeType, artifactType, artifactScopeId, includeExtendedDetails);
    }

    private Favorite GetFavoriteByIdImpl(
      OwnerScope ownerScope,
      Guid favoriteId,
      string artifactScopeType,
      string artifactType,
      string artifactScopeId = null,
      bool includeExtendedDetails = false)
    {
      IFavoriteService service = this.TfsRequestContext.GetService<IFavoriteService>();
      ArtifactScope artifactScope = new ArtifactScope(artifactScopeType, artifactScopeId);
      FavoriteFilter favoriteFilter = new FavoriteFilter()
      {
        Type = artifactType,
        ArtifactScope = artifactScope
      };
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      FavoriteFilter filter = favoriteFilter;
      int num = includeExtendedDetails ? 1 : 0;
      OwnerScope ownerScope1 = ownerScope;
      return service.GetFavorites(tfsRequestContext, filter, num != 0, ownerScope1).FirstOrDefault<Favorite>();
    }

    [TraceFilter(15160007, 15160008)]
    [HttpPost]
    [ClientExample("POST__createFavorite.json", null, null, null)]
    public Favorite CreateFavorite(FavoriteCreateParameters favorite) => this.CreateFavoriteImpl(OwnerScope.Self(this.TfsRequestContext), favorite);

    [TraceFilter(15160007, 15160008)]
    [HttpPost]
    [ClientExample("POST__createFavoriteOfOwner.json", null, null, null)]
    public Favorite CreateFavoriteOfOwner(
      string ownerScopeType,
      Guid ownerScopeId,
      FavoriteCreateParameters favorite)
    {
      return this.CreateFavoriteImpl(OwnerScope.Create(this.TfsRequestContext, ownerScopeType, ownerScopeId), favorite);
    }

    private Favorite CreateFavoriteImpl(OwnerScope ownerScope, FavoriteCreateParameters favorite) => this.TfsRequestContext.GetService<IFavoriteService>().CreateFavorite(this.TfsRequestContext, favorite, ownerScope);

    [TraceFilter(15160011, 15160012)]
    [HttpGet]
    [ClientExample("GET__favoriteByArtifact.json", null, null, null)]
    public Favorite GetFavoriteByArtifact(
      string artifactType,
      string artifactId,
      string artifactScopeType,
      string artifactScopeId = null,
      bool includeExtendedDetails = false)
    {
      FavoriteFilter filter = new FavoriteFilter()
      {
        Type = artifactType,
        ArtifactId = artifactId
      };
      return this.TfsRequestContext.GetService<IFavoriteService>().GetFavorites(this.TfsRequestContext, filter, includeExtendedDetails, OwnerScope.Self(this.TfsRequestContext)).FirstOrDefault<Favorite>();
    }

    [ClientIgnore]
    public string GetLoggingMethodName(string originalMethodName, HttpActionContext actionContext)
    {
      string loggingMethodName = originalMethodName;
      string str = "includeExtendedDetails";
      string query = actionContext?.Request?.RequestUri?.Query;
      if ((loggingMethodName.EndsWith("GetFavorites") || loggingMethodName.EndsWith("GetFavoritesOfOwner") ? 1 : (loggingMethodName.EndsWith("GetFavoriteByArtifact") ? 1 : 0)) != 0 && query != null && query.Contains(str))
        loggingMethodName = loggingMethodName + "." + str;
      return loggingMethodName;
    }
  }
}
