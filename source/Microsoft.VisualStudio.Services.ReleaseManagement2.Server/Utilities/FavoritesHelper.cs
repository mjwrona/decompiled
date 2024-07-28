// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.FavoritesHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  internal static class FavoritesHelper
  {
    private const string FavoriteItemPropertyNameFormat = "{0}.{1}{2}";
    private const string FavoriteItemPropertyNamePrefixFormat = "{0}.{1}";
    private const int CISamplingRateForGet = 3;
    private const int CISamplingRateForCreateDelete = 1;

    public static IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> GetIdentitiesWithFavoriteProperties(
      this IEnumerable<string> teamFoundationIds,
      IVssRequestContext requestContext)
    {
      using (ReleaseManagementTimer releaseManagementTimer = ReleaseManagementTimer.Create(requestContext, "Service", "FavoritesHelper.GetIdentitiesWithFavoriteProperties", 1961224, 3, true))
      {
        IdentityService service = requestContext.GetService<IdentityService>();
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identities = Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions.IdentityExtensions.GetIdentities(requestContext, service, (IList<Guid>) teamFoundationIds.Select<string, Guid>((Func<string, Guid>) (id => new Guid(id))).ToList<Guid>());
        releaseManagementTimer.RecordLap("Service", "Identities.IdentityFetchFromService", 1961224);
        new IdentityPropertyHelper().ReadExtendedProperties(requestContext, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities, (IEnumerable<string>) null, IdentityPropertyScope.Local);
        return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities;
      }
    }

    public static IEnumerable<FavoriteItem> CreateNewFavoriteProperties(
      this string teamFoundationId,
      IVssRequestContext requestContext,
      string projectId,
      string scope,
      IEnumerable<FavoriteItem> favoriteItems)
    {
      using (ReleaseManagementTimer releaseManagementTimer = ReleaseManagementTimer.Create(requestContext, "Service", "FavoritesHelper.CreateFavoriteProperties", 1961224, 1, true))
      {
        Microsoft.VisualStudio.Services.Identity.Identity extendedProperties = FavoritesHelper.GetIdentityWithoutExtendedProperties(teamFoundationId, requestContext);
        releaseManagementTimer.RecordLap("Service", "Identities.IdentityFetchFromService", 1961224);
        foreach (FavoriteItem favoriteItem in favoriteItems)
        {
          if (favoriteItem != null)
          {
            favoriteItem.Id = Guid.NewGuid();
            string str = FavoritesHelper.Serialize(favoriteItem);
            string fromFavoriteItemId = FavoritesHelper.GetPropertyNameFromFavoriteItemId(projectId, scope, favoriteItem.Id);
            extendedProperties.SetProperty(fromFavoriteItemId, (object) str);
          }
        }
        return extendedProperties.UpdateFavoritesFromExtendendProperties(requestContext, projectId, scope);
      }
    }

    public static void DeleteFavoriteProperties(
      this string teamFoundationId,
      IVssRequestContext requestContext,
      string projectId,
      string scope,
      IEnumerable<Guid> favoriteItemIds)
    {
      using (ReleaseManagementTimer releaseManagementTimer = ReleaseManagementTimer.Create(requestContext, "Service", "FavoritesHelper.DeleteFavoriteProperties", 1961224, 1, true))
      {
        Microsoft.VisualStudio.Services.Identity.Identity extendedProperties = FavoritesHelper.GetIdentityWithoutExtendedProperties(teamFoundationId, requestContext);
        releaseManagementTimer.RecordLap("Service", "Identities.IdentityFetchFromService", 1961224);
        foreach (Guid favoriteItemId in favoriteItemIds)
        {
          string fromFavoriteItemId = FavoritesHelper.GetPropertyNameFromFavoriteItemId(projectId, scope, favoriteItemId);
          extendedProperties.SetProperty(fromFavoriteItemId, (object) null);
        }
        extendedProperties.UpdateFavoritesFromExtendendProperties(requestContext, projectId, scope);
      }
    }

    public static IEnumerable<FavoriteItem> GetFavoriteItemPropertyValues(
      this Microsoft.VisualStudio.Services.Identity.Identity identity,
      string projectId,
      string scope)
    {
      string prefix = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}.{1}", (object) projectId, (object) scope);
      return identity.Properties.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (kvp =>
      {
        object obj = kvp.Value;
        return !string.IsNullOrWhiteSpace(kvp.Key) && kvp.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
      })).Select<KeyValuePair<string, object>, FavoriteItem>((Func<KeyValuePair<string, object>, FavoriteItem>) (kvp =>
      {
        object obj = kvp.Value;
        return FavoritesHelper.Deserialize((string) kvp.Value);
      }));
    }

    public static string GetIdentityIdOrCurrentUser(
      IVssRequestContext requestContext,
      string identityId,
      bool checkManagePermissions)
    {
      string str = requestContext.GetUserId().ToString("D");
      if (string.IsNullOrWhiteSpace(identityId))
        identityId = str;
      if (checkManagePermissions && !identityId.Equals(str))
      {
        using (ReleaseManagementTimer.Create(requestContext, "Service", "Identities.CheckTeamPermissions", 1961224, 1, true))
        {
          Microsoft.VisualStudio.Services.Identity.Identity extendedProperties = FavoritesHelper.GetIdentityWithoutExtendedProperties(identityId, requestContext);
          FavoritesHelper.CheckUserManagePermissions(requestContext, extendedProperties);
        }
      }
      return identityId;
    }

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
    public static FavoriteItem Deserialize(string value)
    {
      using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(value)))
        return new DataContractJsonSerializer(typeof (FavoriteItem)).ReadObject((Stream) memoryStream) as FavoriteItem;
    }

    public static string Serialize(FavoriteItem item)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        new DataContractJsonSerializer(typeof (FavoriteItem)).WriteObject((Stream) memoryStream, (object) item);
        return Encoding.UTF8.GetString(memoryStream.ToArray()).ToString();
      }
    }

    private static string GetPropertyNameFromFavoriteItemId(
      string projectId,
      string scope,
      Guid favItemId)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}{2}", (object) projectId, (object) scope, (object) favItemId.ToString("D"));
    }

    private static IEnumerable<FavoriteItem> UpdateFavoritesFromExtendendProperties(
      this Microsoft.VisualStudio.Services.Identity.Identity identity,
      IVssRequestContext requestContext,
      string projectId,
      string scope)
    {
      new IdentityPropertyHelper().UpdateExtendedProperties(requestContext, IdentityPropertyScope.Local, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>()
      {
        identity
      });
      return identity.GetFavoriteItemPropertyValues(projectId, scope);
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity GetIdentityWithoutExtendedProperties(
      string teamFoundationId,
      IVssRequestContext requestContext)
    {
      IEnumerable<string> source = (IEnumerable<string>) new List<string>()
      {
        teamFoundationId
      };
      IdentityService service = requestContext.GetService<IdentityService>();
      return Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions.IdentityExtensions.GetIdentities(requestContext, service, (IList<Guid>) source.Select<string, Guid>((Func<string, Guid>) (id => new Guid(id))).ToList<Guid>()).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    private static void CheckUserManagePermissions(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      string token = identity.IsContainer ? FavoritesHelper.CreateSecurityToken(identity) : throw new UnauthorizedAccessException(Resources.UnauthorizedToModifyFavoritesUser);
      if (!requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.IdentitiesNamespaceId).HasPermission(requestContext, token, 8))
        throw new UnauthorizedAccessException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.UnauthorizedToModifyFavoritesGroup, (object) identity.DisplayName));
    }

    private static string CreateSecurityToken(Microsoft.VisualStudio.Services.Identity.Identity group) => group.GetProperty<Guid>("LocalScopeId", Guid.Empty).ToString() + (object) FrameworkSecurity.IdentitySecurityPathSeparator + group.Id.ToString();
  }
}
