// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.Server.ClientFavoriteProviderService
// Assembly: Microsoft.VisualStudio.Services.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EC45EF48-58C1-4A3C-8054-12C79AD0CC5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Favorites.WebApi;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Favorites.Server
{
  public class ClientFavoriteProviderService : IClientFavoriteProviderService, IVssFrameworkService
  {
    private const string c_favoriteProvidersSharedDataKey = "_favoriteProviders";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void AddFavoriteProvider(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      string artifactType,
      string securityToken)
    {
      object obj;
      FavoritesSharedDataProviderSecuredList providerSecuredList;
      if (sharedData.TryGetValue("_favoriteProviders", out obj) && obj is Dictionary<string, FavoriteProvider>)
      {
        providerSecuredList = obj as FavoritesSharedDataProviderSecuredList;
      }
      else
      {
        providerSecuredList = new FavoritesSharedDataProviderSecuredList(securityToken);
        sharedData.Add("_favoriteProviders", (object) providerSecuredList);
      }
      if (providerSecuredList.ContainsKey(artifactType))
        return;
      FavoriteProvider favoriteProvider = requestContext.GetService<IFavoriteProviderService>().GetFavoriteProviders(requestContext, false, (ISet<string>) new HashSet<string>()
      {
        artifactType
      }).FirstOrDefault<FavoriteProvider>();
      if (favoriteProvider == null)
        return;
      favoriteProvider.Token = securityToken;
      providerSecuredList.Add(artifactType, favoriteProvider);
    }
  }
}
