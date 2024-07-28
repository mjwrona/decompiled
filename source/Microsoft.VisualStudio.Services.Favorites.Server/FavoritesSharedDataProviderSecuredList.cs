// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.Server.FavoritesSharedDataProviderSecuredList
// Assembly: Microsoft.VisualStudio.Services.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EC45EF48-58C1-4A3C-8054-12C79AD0CC5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.Server.dll

using Microsoft.VisualStudio.Services.Favorites.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Favorites.Server
{
  internal class FavoritesSharedDataProviderSecuredList : 
    Dictionary<string, FavoriteProvider>,
    ISecuredObject
  {
    private string Token;

    public FavoritesSharedDataProviderSecuredList(string securityToken) => this.Token = securityToken;

    Guid ISecuredObject.NamespaceId => FavoritesPrivileges.NamespaceId;

    int ISecuredObject.RequiredPermissions => 1;

    string ISecuredObject.GetToken() => this.Token;
  }
}
