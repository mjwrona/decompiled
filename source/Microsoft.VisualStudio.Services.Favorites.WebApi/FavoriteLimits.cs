// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.WebApi.FavoriteLimits
// Assembly: Microsoft.VisualStudio.Services.Favorites.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86250CA5-6C66-4E9F-9014-5EE4DB12BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.WebApi.dll

namespace Microsoft.VisualStudio.Services.Favorites.WebApi
{
  public class FavoriteLimits
  {
    public const int MaxFavoritesPerScopedType = 200;
    public const int MaxScopeTypeLength = 256;
    public const int MaxScopeIdLength = 256;
    public const int MaxArtifactTypeLength = 128;
    public const int MaxArtifactIdLength = 256;
    public const int MaxArtifactNameLength = 256;
    public const int MaxArtifactPropertiesLength = 8000;
  }
}
