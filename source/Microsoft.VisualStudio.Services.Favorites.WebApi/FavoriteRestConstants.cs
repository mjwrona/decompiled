// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.WebApi.FavoriteRestConstants
// Assembly: Microsoft.VisualStudio.Services.Favorites.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86250CA5-6C66-4E9F-9014-5EE4DB12BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Favorites.WebApi
{
  public static class FavoriteRestConstants
  {
    public const string AreaId = "67349C8B-6425-42F2-97B6-0843CB037473";
    public const string Area = "Favorite";
    public const string ActivityLogArea = "Favorites";

    public static class FavoriteResource
    {
      public const string Name = "Favorites";
      public const int Version = 1;
      public const string RouteName = "Favorites";
      public const string RouteTemplate = "{area}/{resource}/{favoriteId}";
      public const string LocationString = "6F13E9A6-AAE2-4B89-B683-131CA9564CEC";
      public static Guid LocationId = new Guid("6F13E9A6-AAE2-4B89-B683-131CA9564CEC");
    }

    public static class FavoriteProvidersResource
    {
      public const string Name = "FavoriteProviders";
      public const int Version = 1;
      public const string RouteName = "FavoriteProviders";
      public const string RouteTemplate = "{area}/{resource}";
      public static Guid LocationId = new Guid("0C04D86B-E315-464F-8125-4D6222D306C2");
    }

    public static class FavoriteArtifactResource
    {
      public const string Name = "FavoriteArtifacts";
      public const int Version = 1;
      public const string RouteName = "FavoriteArtifact";
      public const string RouteTemplate = "{area}/Favorites/{artifactType}/{artifactId}";
      public const string LocationString = "98F78F0F-5988-4858-8FB4-468EE9ABEB8B";
      public static Guid LocationId = new Guid("98F78F0F-5988-4858-8FB4-468EE9ABEB8B");
    }

    public static class TeamFavoriteResource
    {
      public const string Name = "TeamFavorites";
      public const int Version = 1;
      public const string RouteName = "TeamFavorites";
      public const string RouteTemplate = "{area}/{resource}/{favoriteId}";
      public const string LocationString = "ED9A188E-213F-4331-BF62-8AA10D135CA3";
      public static Guid LocationId = new Guid("ED9A188E-213F-4331-BF62-8AA10D135CA3");
    }
  }
}
