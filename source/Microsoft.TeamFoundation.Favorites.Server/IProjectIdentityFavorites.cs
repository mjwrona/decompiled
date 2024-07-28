// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Favorites.IProjectIdentityFavorites
// Assembly: Microsoft.TeamFoundation.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4742830F-DF0E-4509-8C2D-2540DAED73F4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Favorites
{
  public interface IProjectIdentityFavorites
  {
    void UpdateFavoriteItems(IVssRequestContext requestContext, IEnumerable<FavoriteItem> favItems);

    void DeleteFavoriteItems(IVssRequestContext requestContext, IEnumerable<Guid> favIds);

    void Update(IVssRequestContext requestContext);
  }
}
