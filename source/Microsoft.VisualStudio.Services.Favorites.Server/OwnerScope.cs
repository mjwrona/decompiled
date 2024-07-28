// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.Server.OwnerScope
// Assembly: Microsoft.VisualStudio.Services.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EC45EF48-58C1-4A3C-8054-12C79AD0CC5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Favorites.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Favorites.Server
{
  public class OwnerScope
  {
    public static OwnerScope Team(Guid teamId) => new OwnerScope()
    {
      OwnerId = teamId,
      IsTeam = true
    };

    public static OwnerScope SpecifiedUser(Guid userId) => new OwnerScope()
    {
      OwnerId = userId
    };

    public static OwnerScope Self(IVssRequestContext requestContext) => OwnerScope.SpecifiedUser(requestContext.GetUserId());

    public void ThrowIfUserHasInadequatePermissions(
      IVssRequestContext requestContext,
      FavoriteAccessMode accessMode)
    {
      if (!this.IsSelf(requestContext) && !this.IsTeam)
      {
        requestContext.Trace(10017001, TraceLevel.Info, "Favorites", nameof (OwnerScope), "A user identity other than the user's own was the target on Favorites modification. User:{0} Targeted User Identity:{1}", (object) requestContext.GetUserId(), (object) this.OwnerId);
        throw new FavoritesOwnerMismatchException(FavoritesWebApiResources.InadequateUserPermissionsExceptionMessage());
      }
      if (!this.IsTeam)
        return;
      ITeamFavoritesPermissionsProvider extension = requestContext.GetExtension<ITeamFavoritesPermissionsProvider>(ExtensionLifetime.Service, throwOnError: true);
      if (extension == null)
        throw new FavoritesOwnerMismatchException(FavoritesWebApiResources.TeamFavoritesUnsupportedExceptionMessage());
      if (!extension.CanUseFavoritesOfTeam(requestContext, accessMode, this.OwnerId))
      {
        requestContext.Trace(10017002, TraceLevel.Info, "Favorites", nameof (OwnerScope), "A user identity lacking team edit permissions attempted to modif favorites of this team. User:{0} Targeted Team Identity:{1}", (object) requestContext.GetUserId(), (object) this.OwnerId);
        throw new FavoritesOwnerMismatchException(FavoritesWebApiResources.InadequateTeamPermissionsExceptionMessage());
      }
    }

    internal static OwnerScope Create(
      IVssRequestContext requestContext,
      string ownerScopeType,
      Guid ownerScopeId)
    {
      switch (ownerScopeType)
      {
        case "Team":
          return OwnerScope.Team(ownerScopeId);
        case "User":
          return OwnerScope.SpecifiedUser(ownerScopeId);
        default:
          throw new UnsupportedOwnerScopeTypeException(FavoritesWebApiResources.UnrecognizedOwnerScopeTypeExceptionMessage());
      }
    }

    public Guid OwnerId { get; private set; }

    public bool IsTeam { get; private set; }

    public bool IsSelf(IVssRequestContext requestContext) => this.OwnerId == requestContext.GetUserId();

    public IdentityRef GetIdentityRef(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      return new IdentityRef()
      {
        Id = userIdentity.Id.ToString(),
        DisplayName = userIdentity.DisplayName.ToString(),
        UniqueName = userIdentity.ToString()
      };
    }
  }
}
