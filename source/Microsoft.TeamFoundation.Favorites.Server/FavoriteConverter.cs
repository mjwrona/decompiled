// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Favorites.FavoriteConverter
// Assembly: Microsoft.TeamFoundation.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4742830F-DF0E-4509-8C2D-2540DAED73F4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Favorites.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Favorites
{
  public static class FavoriteConverter
  {
    public static TfsFavorite ToLegacyFavorite(
      this Favorite favorite,
      IVssRequestContext requestContext)
    {
      string artifactProperties = (string) null;
      if (favorite.ArtifactProperties != null)
      {
        artifactProperties = favorite.ArtifactProperties.Serialize<ArtifactProperties>();
        Favorite.VerifyArtifactPropertyString(artifactProperties);
      }
      TfsFavorite legacyFavorite = new TfsFavorite();
      legacyFavorite.Data = favorite.ArtifactId;
      legacyFavorite.Id = favorite.Id;
      legacyFavorite.Metadata = artifactProperties;
      legacyFavorite.Name = favorite.ArtifactName;
      legacyFavorite.Type = favorite.ArtifactType;
      legacyFavorite.Scope = LegacyStorageRegistration.GetTypeToScopeMapping(requestContext)[favorite.ArtifactType];
      legacyFavorite.ArtifactIsDeleted = favorite.ArtifactIsDeleted;
      Guid result = Guid.Empty;
      Guid.TryParse(favorite.ArtifactScope.Id, out result);
      legacyFavorite.ProjectId = result;
      return legacyFavorite;
    }

    public static Favorite ToModernFavorite(this TfsFavorite favorite, IdentityRef owner)
    {
      ArtifactProperties artifactProperties = (ArtifactProperties) null;
      JsonUtilities.TryDeserialize<ArtifactProperties>(favorite.Metadata, out artifactProperties);
      return new Favorite()
      {
        ArtifactScope = new ArtifactScope()
        {
          Type = "Project",
          Id = favorite.ProjectId.ToString()
        },
        ArtifactId = favorite.Data,
        Id = favorite.Id,
        ArtifactProperties = artifactProperties,
        ArtifactName = favorite.Name,
        ArtifactType = favorite.Type,
        ArtifactIsDeleted = favorite.ArtifactIsDeleted,
        Owner = owner
      };
    }

    public static ArtifactScope MakeTfsArtifactScope(this Guid projectId, string name = null) => new ArtifactScope()
    {
      Type = "Project",
      Id = projectId.ToString(),
      Name = name
    };
  }
}
