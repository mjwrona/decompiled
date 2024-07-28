// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.Server.Service.FavoritesPermissionHelper
// Assembly: Microsoft.VisualStudio.Services.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EC45EF48-58C1-4A3C-8054-12C79AD0CC5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Favorites.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Favorites.Server.Service
{
  public class FavoritesPermissionHelper
  {
    public static string GetMyFavoritesSecurityToken(
      IVssRequestContext requestContext,
      Guid? projectId)
    {
      return string.Format("{0}/{1}", (object) FavoritesPrivileges.Root, (object) projectId);
    }

    public static bool CanUseMyFavorites(IVssRequestContext requestContext, Guid? projectId)
    {
      string favoritesSecurityToken = FavoritesPermissionHelper.GetMyFavoritesSecurityToken(requestContext, projectId);
      return requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FavoritesPrivileges.NamespaceId).HasPermission(requestContext, favoritesSecurityToken, 1);
    }

    public static void CheckCanUseMyFavorites(IVssRequestContext requestContext, Guid? projectId)
    {
      string favoritesSecurityToken = FavoritesPermissionHelper.GetMyFavoritesSecurityToken(requestContext, projectId);
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FavoritesPrivileges.NamespaceId).CheckPermission(requestContext, favoritesSecurityToken, 1);
    }
  }
}
