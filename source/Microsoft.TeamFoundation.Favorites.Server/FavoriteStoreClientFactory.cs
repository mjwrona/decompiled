// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Favorites.FavoriteStoreClientFactory
// Assembly: Microsoft.TeamFoundation.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4742830F-DF0E-4509-8C2D-2540DAED73F4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Favorites.Server;
using System;
using System.Text;

namespace Microsoft.TeamFoundation.Favorites
{
  public class FavoriteStoreClientFactory : IFavoriteStoreClientFactory
  {
    public IAccountIdentityFavorites GetAccountStore(
      IVssRequestContext tfsRequestContext,
      OwnerScope owner)
    {
      return (IAccountIdentityFavorites) IdentityPropertiesView.CreateView<AccountIdentityFavorites>(tfsRequestContext, owner.OwnerId);
    }

    public IProjectIdentityFavorites GetProjectStore(
      IVssRequestContext tfsRequestContext,
      OwnerScope owner,
      Guid projectGuid,
      string scope)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append('.');
      stringBuilder.Append((object) projectGuid);
      if (owner.IsTeam)
      {
        stringBuilder.Append('.');
        stringBuilder.Append((object) owner.OwnerId);
      }
      stringBuilder.Append('.');
      stringBuilder.Append(scope);
      return (IProjectIdentityFavorites) IdentityPropertiesView.CreateView<ProjectIdentityFavorites>(tfsRequestContext, owner.OwnerId, stringBuilder.ToString());
    }
  }
}
