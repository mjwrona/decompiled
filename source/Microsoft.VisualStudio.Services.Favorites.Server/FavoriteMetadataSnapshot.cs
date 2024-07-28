// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.Server.FavoriteMetadataSnapshot
// Assembly: Microsoft.VisualStudio.Services.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EC45EF48-58C1-4A3C-8054-12C79AD0CC5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Favorites.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Favorites.Server
{
  internal class FavoriteMetadataSnapshot
  {
    public string Name;
    public ArtifactProperties ArtifactProperties;
    public bool ArtifactIsDeleted;

    public FavoriteMetadataSnapshot(Favorite favorite)
    {
      this.Name = favorite.ArtifactName;
      this.ArtifactProperties = favorite.ArtifactProperties;
      this.ArtifactIsDeleted = favorite.ArtifactIsDeleted;
    }

    internal bool IsEqual(Favorite favorite) => string.Equals(this.Name, favorite.ArtifactName, StringComparison.CurrentCulture) && FavoriteMetadataSnapshot.ArtifactPropertiesEquals(this.ArtifactProperties, favorite.ArtifactProperties) && this.ArtifactIsDeleted == favorite.ArtifactIsDeleted;

    internal static bool ArtifactPropertiesEquals(
      ArtifactProperties first,
      ArtifactProperties second)
    {
      if (first == second)
        return true;
      if (first == null || second == null || first.Count != second.Count)
        return false;
      foreach (KeyValuePair<string, object> keyValuePair in (Dictionary<string, object>) first)
      {
        object obj;
        if (!second.TryGetValue(keyValuePair.Key, out obj) || keyValuePair.Value != obj && (keyValuePair.Value == null || obj == null || string.Compare(JsonUtilities.Serialize(keyValuePair.Value), JsonUtilities.Serialize(obj), StringComparison.InvariantCulture) != 0))
          return false;
      }
      return true;
    }
  }
}
