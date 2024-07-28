// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmFavoritesController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(3.1)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "favorites")]
  public class RmFavoritesController : ReleaseManagementProjectControllerBase
  {
    [HttpGet]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.ViewReleaseDefinition)]
    public IEnumerable<FavoriteItem> GetFavorites(string scope, string identityId = null)
    {
      identityId = FavoritesHelper.GetIdentityIdOrCurrentUser(this.TfsRequestContext, identityId, false);
      return new List<string>() { identityId }.GetIdentitiesWithFavoriteProperties(this.TfsRequestContext).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>().GetFavoriteItemPropertyValues(this.ProjectId.ToString("D"), scope);
    }

    [HttpPost]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.ViewReleaseDefinition)]
    public IEnumerable<FavoriteItem> CreateFavorites(
      [FromBody] IEnumerable<FavoriteItem> favoriteItems,
      string scope,
      string identityId = null)
    {
      if (favoriteItems == null || !favoriteItems.Any<FavoriteItem>())
        throw new InvalidRequestException(Resources.FavoriteItemsListEmpty);
      if (favoriteItems.Any<FavoriteItem>((Func<FavoriteItem, bool>) (favItem => favItem == null)))
        throw new InvalidRequestException(Resources.FavoriteItemsListInvalid);
      identityId = FavoritesHelper.GetIdentityIdOrCurrentUser(this.TfsRequestContext, identityId, true);
      return identityId.CreateNewFavoriteProperties(this.TfsRequestContext, this.ProjectId.ToString("D"), scope, favoriteItems);
    }

    [HttpDelete]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.ViewReleaseDefinition)]
    public void DeleteFavorites(string scope, string identityId = null, string favoriteItemIds = null)
    {
      IEnumerable<string> source = !string.IsNullOrWhiteSpace(favoriteItemIds) ? ((IEnumerable<string>) favoriteItemIds.Split(',')).Where<string>((Func<string, bool>) (id => !string.IsNullOrWhiteSpace(id))) : throw new InvalidRequestException(Resources.NoFavoriteIdsWereSpecified);
      if (!source.Any<string>((Func<string, bool>) (id => Guid.TryParse(id, out Guid _))))
        throw new InvalidRequestException(Resources.DeleteFavoriteItemIdsInvalid);
      identityId = FavoritesHelper.GetIdentityIdOrCurrentUser(this.TfsRequestContext, identityId, true);
      IEnumerable<Guid> favoriteItemIds1 = source.Select<string, Guid>((Func<string, Guid>) (item => new Guid(item)));
      identityId.DeleteFavoriteProperties(this.TfsRequestContext, this.ProjectId.ToString("D"), scope, favoriteItemIds1);
    }
  }
}
