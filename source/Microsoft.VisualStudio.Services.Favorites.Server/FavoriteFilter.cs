// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.Server.FavoriteFilter
// Assembly: Microsoft.VisualStudio.Services.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EC45EF48-58C1-4A3C-8054-12C79AD0CC5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.Server.dll

using Microsoft.VisualStudio.Services.Favorites.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Favorites.Server
{
  public class FavoriteFilter
  {
    public Guid? FavoriteId { get; set; }

    public string Type { get; set; }

    public string ArtifactId { get; set; }

    public ArtifactScope ArtifactScope { get; set; }

    public IEnumerable<Favorite> FilterFavorites(IEnumerable<Favorite> favorites)
    {
      IEnumerable<Favorite> source = favorites;
      if (this.FavoriteId.HasValue)
        source = source.Where<Favorite>((Func<Favorite, bool>) (x =>
        {
          Guid guid = this.FavoriteId.Value;
          Guid? id = x.Id;
          return id.HasValue && guid == id.GetValueOrDefault();
        }));
      if (!string.IsNullOrWhiteSpace(this.Type))
        source = source.Where<Favorite>((Func<Favorite, bool>) (x => x.ArtifactType == this.Type));
      if (!string.IsNullOrWhiteSpace(this.ArtifactId))
        source = source.Where<Favorite>((Func<Favorite, bool>) (x => x.ArtifactId == this.ArtifactId));
      if (this.ArtifactScope != null)
        source = source.Where<Favorite>((Func<Favorite, bool>) (x => x.ArtifactScope.IsSame(this.ArtifactScope)));
      return source;
    }
  }
}
