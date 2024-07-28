// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.FavoritesServiceShim
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Favorites.Server;
using Microsoft.VisualStudio.Services.Favorites.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class FavoritesServiceShim
  {
    private static Dictionary<string, string> ScopeToTypeMap = new Dictionary<string, string>();
    private const string LoggedAreaName = "Favorites";
    private const string BuildScope = "Build.Definitions";

    public static bool IsFavoritesReadShimNeeded(
      IVssRequestContext requestContext,
      string[] expressions,
      TeamFoundationIdentity[] identities)
    {
      return expressions != null && expressions.Length == 1 && identities != null && identities.Length == 1 && FavoritesServiceShim.IsKeyTargetingFavorites(expressions[0]);
    }

    public static bool IsFavoritesWriteShimNeeded(
      IVssRequestContext requestContext,
      List<PropertyValue> propertyValues,
      IdentityPropertyScope propertyScope)
    {
      return propertyValues.Count >= 1 && propertyScope == IdentityPropertyScope.Local && propertyValues.All<PropertyValue>((Func<PropertyValue, bool>) (o => FavoritesServiceShim.IsKeyTargetingFavorites(o.PropertyName)));
    }

    private static bool IsKeyTargetingFavorites(string keyName) => keyName.StartsWith("Microsoft.TeamFoundation.Framework.Server.IdentityFavorites", StringComparison.InvariantCultureIgnoreCase);

    public static void OverrideQueriedFavorites(
      IVssRequestContext requestContext,
      string[] expressions,
      TeamFoundationIdentity[] queriedIdentities)
    {
      ArgumentUtility.CheckForNull<string[]>(expressions, nameof (expressions));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(expressions[0], "expressions[0]");
      ArgumentUtility.CheckForNull<TeamFoundationIdentity[]>(queriedIdentities, nameof (queriedIdentities));
      ArgumentUtility.CheckForNull<TeamFoundationIdentity>(queriedIdentities[0], "queriedIdentities[0]");
      TeamFoundationIdentity queriedIdentity = queriedIdentities[0];
      string expression = expressions[0];
      using (TimedCiEvent timedCiEvent = new TimedCiEvent(requestContext, "Favorites", nameof (OverrideQueriedFavorites)))
      {
        try
        {
          List<string> stringList = new List<string>();
          foreach (KeyValuePair<string, object> property in queriedIdentity.GetProperties(IdentityPropertyScope.Local))
          {
            if (FavoritesServiceShim.IsKeyTargetingFavorites(property.Key))
              stringList.Add(property.Key);
          }
          IdentityFavoriteKey identityFavoriteKey1 = IdentityFavoriteKey.Parse(requestContext, expression);
          Guid teamFoundationId1 = queriedIdentity.TeamFoundationId;
          string type = FavoritesServiceShim.MapLegacyScopeToType(identityFavoriteKey1.Scope);
          if (type == null)
            return;
          timedCiEvent.Properties["type"] = (object) type;
          timedCiEvent.Properties["isTeam"] = (object) identityFavoriteKey1.IsTeamFavorite;
          FavoriteFilter filter = new FavoriteFilter()
          {
            ArtifactScope = new ArtifactScope()
            {
              Id = identityFavoriteKey1.ProjectGuid,
              Type = "Project"
            },
            Type = type
          };
          Guid teamFoundationId2 = queriedIdentity.TeamFoundationId;
          OwnerScope ownerScope = identityFavoriteKey1.IsTeamFavorite ? OwnerScope.Team(teamFoundationId2) : OwnerScope.SpecifiedUser(teamFoundationId2);
          IFavoriteService service = requestContext.GetService<IFavoriteService>();
          IEnumerable<Favorite> favorites = service.GetFavorites(requestContext, filter, false, ownerScope);
          if (identityFavoriteKey1.Scope == "Build.Definitions")
          {
            filter.Type = "Microsoft.TeamFoundation.Build.BuildDefinitionInitialized";
            favorites = favorites.Union<Favorite>(service.GetFavorites(requestContext, filter, false, ownerScope));
          }
          IEnumerable<FavoriteItem> favoriteItems = favorites.Select<Favorite, FavoriteItem>((Func<Favorite, FavoriteItem>) (o => FavoritesServiceShim.ConvertToFavorite(o)));
          foreach (string name in stringList)
            queriedIdentity.RemoveProperty(IdentityPropertyScope.Local, name);
          foreach (FavoriteItem favoriteItem in favoriteItems)
          {
            IdentityFavoriteKey identityFavoriteKey2 = identityFavoriteKey1.IsTeamFavorite ? IdentityFavoriteKey.CreateTeamkey(identityFavoriteKey1.ProjectGuid, identityFavoriteKey1.TeamId, identityFavoriteKey1.Scope, favoriteItem.Id) : IdentityFavoriteKey.CreateUserkey(identityFavoriteKey1.ProjectGuid, identityFavoriteKey1.Scope, favoriteItem.Id);
            queriedIdentity.SetProperty(IdentityPropertyScope.Local, identityFavoriteKey2.ToString(), (object) favoriteItem.Serialize());
          }
        }
        catch (Exception ex)
        {
          requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Favorites", "OverrideQueriedFavorites_Exception", "Summary", ex.ToString());
          throw;
        }
      }
    }

    public static void UpdateFavoriteItems(
      IVssRequestContext requestContext,
      TeamFoundationIdentity owner,
      List<PropertyValue> propertyValues)
    {
      ArgumentUtility.CheckForNull<TeamFoundationIdentity>(owner, nameof (owner));
      ArgumentUtility.CheckForNull<List<PropertyValue>>(propertyValues, nameof (propertyValues));
      using (TimedCiEvent timedCiEvent = new TimedCiEvent(requestContext, "Favorites", nameof (UpdateFavoriteItems)))
      {
        try
        {
          IFavoriteService favoriteService = !FavoriteMigrationState.IsMigrationActive(requestContext) ? requestContext.GetService<IFavoriteService>() : throw new FavoriteWritesDisabledDuringMigrationException(FavoritesWebApiResources.FavoriteWritesDisabledDuringMigrationExceptionMessage());
          int num1 = 0;
          int num2 = 0;
          foreach (PropertyValue propertyValue in propertyValues)
          {
            string propertyName = propertyValue.PropertyName;
            string str = propertyValue.Value as string;
            FavoriteItem o = (FavoriteItem) null;
            if (str != null)
              o = FavoriteItem.Deserialize(str);
            IdentityFavoriteKey identityFavoriteKey = IdentityFavoriteKey.Parse(requestContext, propertyName);
            ArtifactScope artifactScope = new ArtifactScope()
            {
              Id = identityFavoriteKey.ProjectGuid,
              Type = "Project"
            };
            string type = FavoritesServiceShim.MapLegacyScopeToType(identityFavoriteKey.Scope);
            Guid favoriteId = Guid.Parse(identityFavoriteKey.Id);
            OwnerScope ownerScope = identityFavoriteKey.IsTeamFavorite ? OwnerScope.Team(owner.TeamFoundationId) : OwnerScope.SpecifiedUser(owner.TeamFoundationId);
            if (o == null)
            {
              favoriteService.DeleteFavorite(requestContext, artifactScope, type, favoriteId, ownerScope);
              ++num2;
            }
            else
            {
              FavoriteCreateParameters favorite = FavoritesServiceShim.ConvertToFavorite(o, identityFavoriteKey.ProjectGuid);
              favoriteService.CreateFavorite(requestContext, favorite, ownerScope);
              ++num1;
            }
          }
          timedCiEvent.Properties["updateCount"] = (object) num1;
          timedCiEvent.Properties["deleteCount"] = (object) num2;
        }
        catch (Exception ex)
        {
          requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Favorites", "UpdateFavoriteItems_Exception", "Summary", ex.ToString());
          throw;
        }
      }
    }

    private static FavoriteItem ConvertToFavorite(Favorite o) => new FavoriteItem()
    {
      Id = o.Id.Value,
      Data = o.ArtifactId,
      Name = o.ArtifactName,
      Type = o.ArtifactType
    };

    private static FavoriteCreateParameters ConvertToFavorite(FavoriteItem o, string projectGuid) => new FavoriteCreateParameters()
    {
      ArtifactId = o.Data,
      ArtifactName = o.Name,
      ArtifactType = o.Type,
      ArtifactScope = new ArtifactScope()
      {
        Id = projectGuid,
        Type = "Project"
      }
    };

    private static string MapLegacyScopeToType(string scope)
    {
      if (scope.Equals("WorkItemTracking.Queries", StringComparison.InvariantCultureIgnoreCase))
        return "Microsoft.TeamFoundation.WorkItemTracking.QueryItem";
      return scope.Equals("Build.Definitions", StringComparison.InvariantCultureIgnoreCase) ? "Microsoft.TeamFoundation.Build.Definition" : (string) null;
    }
  }
}
