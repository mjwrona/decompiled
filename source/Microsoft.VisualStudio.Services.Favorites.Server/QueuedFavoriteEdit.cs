// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.Server.QueuedFavoriteEdit
// Assembly: Microsoft.VisualStudio.Services.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EC45EF48-58C1-4A3C-8054-12C79AD0CC5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.Server.dll

using Microsoft.VisualStudio.Services.Favorites.WebApi;

namespace Microsoft.VisualStudio.Services.Favorites.Server
{
  public class QueuedFavoriteEdit
  {
    public Favorite Item;
    public bool IsEdited;
    public string UpdatedArtifactName;
    public ArtifactProperties UpdatedArtifactProperties;
    public bool UpdatedArtifactIsDeleted;

    public Favorite ConvertToFavoriteItem()
    {
      Favorite favoriteItem = this.Item;
      if (this.IsEdited)
      {
        favoriteItem.ArtifactName = this.UpdatedArtifactName;
        favoriteItem.ArtifactProperties = this.UpdatedArtifactProperties;
        favoriteItem.ArtifactIsDeleted = this.UpdatedArtifactIsDeleted;
      }
      return favoriteItem;
    }
  }
}
