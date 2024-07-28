// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Favorites.FavoriteExtension
// Assembly: Microsoft.TeamFoundation.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4742830F-DF0E-4509-8C2D-2540DAED73F4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Favorites
{
  public static class FavoriteExtension
  {
    public static FavoriteItem ConvertToFavoriteItem(this TfsFavorite favorite) => new FavoriteItem()
    {
      Id = favorite.Id.Value,
      Name = favorite.Name,
      Type = favorite.Type,
      Data = favorite.Data,
      Metadata = favorite.Metadata,
      ArtifactIsDeleted = favorite.ArtifactIsDeleted
    };
  }
}
