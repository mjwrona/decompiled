// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.Server.FavoriteService
// Assembly: Microsoft.VisualStudio.Services.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EC45EF48-58C1-4A3C-8054-12C79AD0CC5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Favorites.Server.Service;
using Microsoft.VisualStudio.Services.Favorites.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Favorites.Server
{
  public class FavoriteService : IFavoriteService, IVssFrameworkService, IFavoritesBackPropagator
  {
    private IStoreFavorites m_favoriteStorage;
    private IFavoriteMetadataEngine m_favoriteMetadataEngine;
    private IScopeProvider m_scopeNameProvider;

    public IEnumerable<Favorite> GetFavorites(
      IVssRequestContext requestContext,
      FavoriteFilter filter,
      bool includeExtendedDetails,
      OwnerScope ownerScope)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "FavoriteService.GetFavorites"))
      {
        if (filter == null)
          filter = new FavoriteFilter();
        if (ownerScope == null)
          ownerScope = OwnerScope.Self(requestContext);
        if (!ownerScope.IsSelf(requestContext) & includeExtendedDetails)
          throw new FavoriteServiceException(FavoritesWebApiResources.ExtendedInformationOnlyForOwnFavoritesExceptionMessage());
        using (TimedCiEvent ciEvent = new TimedCiEvent(requestContext, "Favorites", nameof (GetFavorites)))
        {
          int num1 = 0;
          int num2 = 0;
          requestContext.GetUserId();
          IEnumerable<Favorite> favorites = this.m_favoriteStorage.GetFavorites(requestContext, ownerScope);
          if (!favorites.IsNullOrEmpty<Favorite>())
          {
            favorites = filter.FilterFavorites(favorites);
            if (includeExtendedDetails)
            {
              IEnumerable<QueuedFavoriteEdit> queuedFavoriteEdits = this.m_favoriteMetadataEngine.ProcessMetadata(requestContext, favorites);
              if (!queuedFavoriteEdits.IsNullOrEmpty<QueuedFavoriteEdit>())
                this.BackPropagateEditsToFavorites(requestContext, this.m_favoriteStorage, queuedFavoriteEdits, ownerScope);
            }
            this.AppendReadonlyProperties(requestContext, favorites);
            num1 = favorites.Count<Favorite>();
            num2 = favorites.Select<Favorite, string>((Func<Favorite, string>) (o => o.ArtifactType)).Distinct<string>().Count<string>();
          }
          ciEvent["type"] = (object) filter.Type;
          ciEvent[nameof (includeExtendedDetails)] = (object) includeExtendedDetails;
          ciEvent["itemCount"] = (object) num1;
          ciEvent["typeCount"] = (object) num2;
          FavoriteService.AppendPerfInformation(requestContext, ciEvent);
          return (IEnumerable<Favorite>) favorites.ToArray<Favorite>();
        }
      }
    }

    private static void AppendPerfInformation(
      IVssRequestContext requestContext,
      TimedCiEvent ciEvent)
    {
      PerformanceTimingGroup performanceTimingGroup;
      if (!PerformanceTimer.GetAllTimings(requestContext).TryGetValue("SQL", out performanceTimingGroup))
        return;
      ciEvent["sqlCallCount"] = (object) performanceTimingGroup.Count;
    }

    public void DeleteFavorite(
      IVssRequestContext requestContext,
      ArtifactScope artifactScope,
      string artifactType,
      Guid favoriteId,
      OwnerScope ownerScope)
    {
      ArgumentUtility.CheckForNull<ArtifactScope>(artifactScope, nameof (artifactScope), "Favorite");
      ArgumentUtility.CheckForEmptyGuid(favoriteId, nameof (favoriteId), "Favorite");
      FavoriteMigrationState.ThrowIfMigrationActive(requestContext);
      artifactScope.Validate();
      using (TimedCiEvent timedCiEvent = new TimedCiEvent(requestContext, "Favorites", nameof (DeleteFavorite)))
      {
        timedCiEvent["type"] = (object) artifactType;
        timedCiEvent["addressingMode"] = (object) "byFavoriteId";
        requestContext.GetUserId();
        this.m_favoriteStorage.DeleteFavoriteByFavoriteId(requestContext, artifactScope, artifactType, favoriteId, ownerScope);
      }
    }

    public void DeleteFavorite(
      IVssRequestContext requestContext,
      string artifactType,
      string artifactId,
      OwnerScope ownerScope)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactType, nameof (artifactType), "Favorite");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId), "Favorite");
      FavoriteMigrationState.ThrowIfMigrationActive(requestContext);
      using (TimedCiEvent timedCiEvent = new TimedCiEvent(requestContext, "Favorites", nameof (DeleteFavorite)))
      {
        timedCiEvent["type"] = (object) artifactType;
        timedCiEvent["addressingMode"] = (object) "byArtifactId";
        this.m_favoriteStorage.DeleteFavoriteByArtifact(requestContext, artifactType, artifactId, ownerScope);
      }
    }

    public Favorite CreateFavorite(
      IVssRequestContext requestContext,
      FavoriteCreateParameters favorite,
      OwnerScope ownerScope)
    {
      ArgumentUtility.CheckForNull<FavoriteCreateParameters>(favorite, nameof (favorite), "Favorite");
      FavoriteMigrationState.ThrowIfMigrationActive(requestContext);
      Favorite newFavorite = favorite.ToFavorite();
      newFavorite.Validate();
      IEnumerable<Favorite> favorites1 = this.m_favoriteStorage.GetFavorites(requestContext, ownerScope);
      IEnumerable<Favorite> favorites2 = favorites1.Where<Favorite>((Func<Favorite, bool>) (o => o.ArtifactType == newFavorite.ArtifactType && o.ArtifactId == newFavorite.ArtifactId && o.ArtifactScope.IsSame(newFavorite.ArtifactScope)));
      if (!favorites2.IsNullOrEmpty<Favorite>())
        return favorites2.First<Favorite>();
      this.EnsureFavoriteTypeIsRecognizedHere(requestContext, newFavorite);
      this.EnsureFavoriteQuotaLimitIsSatisfied(requestContext, newFavorite, favorites1);
      using (TimedCiEvent timedCiEvent = new TimedCiEvent(requestContext, "Favorites", "UpdateFavorite"))
      {
        timedCiEvent["type"] = (object) newFavorite.ArtifactType;
        return this.m_favoriteStorage.UpdateFavorites(requestContext, (IEnumerable<Favorite>) new List<Favorite>()
        {
          newFavorite
        }, ownerScope).FirstOrDefault<Favorite>();
      }
    }

    public void BackPropagateEditsToFavorites(
      IVssRequestContext requestContext,
      IStoreFavorites favoriteStorage,
      IEnumerable<QueuedFavoriteEdit> updatedFavorites,
      OwnerScope ownerScope)
    {
      if (updatedFavorites == null || !updatedFavorites.Any<QueuedFavoriteEdit>() || FavoriteMigrationState.IsMigrationActive(requestContext))
        return;
      using (TimedCiEvent timedCiEvent = new TimedCiEvent(requestContext, "Favorites", nameof (BackPropagateEditsToFavorites)))
      {
        favoriteStorage.UpdateFavorites(requestContext, updatedFavorites.Where<QueuedFavoriteEdit>((Func<QueuedFavoriteEdit, bool>) (x => x.IsEdited)).Select<QueuedFavoriteEdit, Favorite>((Func<QueuedFavoriteEdit, Favorite>) (x => x.ConvertToFavoriteItem())), ownerScope);
        timedCiEvent["deletionCount"] = (object) updatedFavorites.Count<QueuedFavoriteEdit>((Func<QueuedFavoriteEdit, bool>) (x => x.UpdatedArtifactIsDeleted));
        timedCiEvent["updateCount"] = (object) updatedFavorites.Count<QueuedFavoriteEdit>((Func<QueuedFavoriteEdit, bool>) (x => x.IsEdited && !x.UpdatedArtifactIsDeleted));
      }
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_scopeNameProvider = this.m_scopeNameProvider ?? systemRequestContext.GetExtension<IScopeProvider>(ExtensionLifetime.Service, throwOnError: true);
      this.m_favoriteMetadataEngine = this.m_favoriteMetadataEngine ?? systemRequestContext.GetService<IFavoriteMetadataEngine>();
      this.m_favoriteStorage = (IStoreFavorites) new FavoritesStorageAdapter(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public FavoriteService()
    {
    }

    internal FavoriteService(
      IStoreFavorites favoriteStorage,
      IFavoriteMetadataEngine favoriteMetadataEngine,
      IScopeProvider scopeProvider)
    {
      this.m_favoriteStorage = favoriteStorage;
      this.m_favoriteMetadataEngine = favoriteMetadataEngine;
      this.m_scopeNameProvider = scopeProvider;
    }

    private void AppendReadonlyProperties(
      IVssRequestContext requestContext,
      IEnumerable<Favorite> favorites)
    {
      if (this.m_scopeNameProvider != null)
        favorites = this.m_favoriteMetadataEngine.AppendScopeInfo(requestContext, this.m_scopeNameProvider, favorites);
      this.m_favoriteMetadataEngine.AddArtifactLinks(requestContext, favorites);
    }

    private void EnsureFavoriteQuotaLimitIsSatisfied(
      IVssRequestContext requestContext,
      Favorite favoriteToCreate,
      IEnumerable<Favorite> existingFavorites)
    {
      IEnumerable<Favorite> source = existingFavorites.Where<Favorite>((Func<Favorite, bool>) (o => o.ArtifactScope.Type == favoriteToCreate.ArtifactScope.Type && o.ArtifactScope.Id == favoriteToCreate.ArtifactScope.Id && o.ArtifactType == favoriteToCreate.ArtifactType));
      int num = 200;
      if (source.Count<Favorite>() >= num)
        throw new FavoritesScopedQuotaExceededException(FavoritesWebApiResources.FavoritesQuotaExceeededExceptionMessage((object) num, (object) favoriteToCreate.ArtifactType, (object) favoriteToCreate.ArtifactScope.Type));
    }

    private void EnsureFavoriteTypeIsRecognizedHere(
      IVssRequestContext requestContext,
      Favorite favoriteToCreate)
    {
      IFavoriteProviderService service = requestContext.GetService<IFavoriteProviderService>();
      Guid currentServiceInstance = requestContext.ServiceInstanceType();
      IVssRequestContext requestContext1 = requestContext;
      IEnumerable<FavoriteProvider> source = service.GetFavoriteProviders(requestContext1, false);
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        source = source.Where<FavoriteProvider>((Func<FavoriteProvider, bool>) (o => o.ServiceIdentifier == currentServiceInstance));
      if (!source.Select<FavoriteProvider, string>((Func<FavoriteProvider, string>) (o => o.ArtifactType)).Contains<string>(favoriteToCreate.ArtifactType, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        throw new FavoriteServiceException(FavoritesWebApiResources.UnrecognizedArtifactTypeExceptionMessage((object) favoriteToCreate.ArtifactType));
    }
  }
}
