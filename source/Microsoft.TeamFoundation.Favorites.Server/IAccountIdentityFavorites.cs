// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Favorites.IAccountIdentityFavorites
// Assembly: Microsoft.TeamFoundation.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4742830F-DF0E-4509-8C2D-2540DAED73F4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Favorites.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Favorites
{
  public interface IAccountIdentityFavorites
  {
    Microsoft.VisualStudio.Services.Identity.Identity Identity { get; }

    IEnumerable<TfsFavorite> GetAccountScopedFavorites(
      IVssRequestContext requestContext,
      OwnerScope owner);

    void Update(IVssRequestContext requestContext);
  }
}
