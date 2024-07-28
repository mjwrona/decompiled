// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.WebApi.FavoritesWebApiResources
// Assembly: Microsoft.VisualStudio.Services.Favorites.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86250CA5-6C66-4E9F-9014-5EE4DB12BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Favorites.WebApi
{
  public static class FavoritesWebApiResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (FavoritesWebApiResources), typeof (FavoritesWebApiResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => FavoritesWebApiResources.s_resMgr;

    private static string Get(string resourceName) => FavoritesWebApiResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? FavoritesWebApiResources.Get(resourceName) : FavoritesWebApiResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) FavoritesWebApiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? FavoritesWebApiResources.GetInt(resourceName) : (int) FavoritesWebApiResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) FavoritesWebApiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? FavoritesWebApiResources.GetBool(resourceName) : (bool) FavoritesWebApiResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => FavoritesWebApiResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = FavoritesWebApiResources.Get(resourceName, culture);
      if (args == null)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }

    public static string FavoritesOwnerMismatchExceptionMessage() => FavoritesWebApiResources.Get(nameof (FavoritesOwnerMismatchExceptionMessage));

    public static string FavoritesOwnerMismatchExceptionMessage(CultureInfo culture) => FavoritesWebApiResources.Get(nameof (FavoritesOwnerMismatchExceptionMessage), culture);

    public static string FavoriteWritesDisabledDuringMigrationExceptionMessage() => FavoritesWebApiResources.Get(nameof (FavoriteWritesDisabledDuringMigrationExceptionMessage));

    public static string FavoriteWritesDisabledDuringMigrationExceptionMessage(CultureInfo culture) => FavoritesWebApiResources.Get(nameof (FavoriteWritesDisabledDuringMigrationExceptionMessage), culture);

    public static string AmbiguousFavoriteDeleteExceptionMessage() => FavoritesWebApiResources.Get(nameof (AmbiguousFavoriteDeleteExceptionMessage));

    public static string AmbiguousFavoriteDeleteExceptionMessage(CultureInfo culture) => FavoritesWebApiResources.Get(nameof (AmbiguousFavoriteDeleteExceptionMessage), culture);

    public static string UnsupportedLegacyFavoriteScopeDeletionExceptionMessage() => FavoritesWebApiResources.Get(nameof (UnsupportedLegacyFavoriteScopeDeletionExceptionMessage));

    public static string UnsupportedLegacyFavoriteScopeDeletionExceptionMessage(CultureInfo culture) => FavoritesWebApiResources.Get(nameof (UnsupportedLegacyFavoriteScopeDeletionExceptionMessage), culture);

    public static string UnsupportedLegacyFavoriteScopeUpdateExceptionMessage() => FavoritesWebApiResources.Get(nameof (UnsupportedLegacyFavoriteScopeUpdateExceptionMessage));

    public static string UnsupportedLegacyFavoriteScopeUpdateExceptionMessage(CultureInfo culture) => FavoritesWebApiResources.Get(nameof (UnsupportedLegacyFavoriteScopeUpdateExceptionMessage), culture);

    public static string UnrecognizedStorageScopeTypeExceptionMessage() => FavoritesWebApiResources.Get(nameof (UnrecognizedStorageScopeTypeExceptionMessage));

    public static string UnrecognizedStorageScopeTypeExceptionMessage(CultureInfo culture) => FavoritesWebApiResources.Get(nameof (UnrecognizedStorageScopeTypeExceptionMessage), culture);

    public static string InadequateTeamPermissionsExceptionMessage() => FavoritesWebApiResources.Get(nameof (InadequateTeamPermissionsExceptionMessage));

    public static string InadequateTeamPermissionsExceptionMessage(CultureInfo culture) => FavoritesWebApiResources.Get(nameof (InadequateTeamPermissionsExceptionMessage), culture);

    public static string InadequateUserPermissionsExceptionMessage() => FavoritesWebApiResources.Get(nameof (InadequateUserPermissionsExceptionMessage));

    public static string InadequateUserPermissionsExceptionMessage(CultureInfo culture) => FavoritesWebApiResources.Get(nameof (InadequateUserPermissionsExceptionMessage), culture);

    public static string TeamFavoritesUnsupportedExceptionMessage() => FavoritesWebApiResources.Get(nameof (TeamFavoritesUnsupportedExceptionMessage));

    public static string TeamFavoritesUnsupportedExceptionMessage(CultureInfo culture) => FavoritesWebApiResources.Get(nameof (TeamFavoritesUnsupportedExceptionMessage), culture);

    public static string UnrecognizedOwnerScopeTypeExceptionMessage() => FavoritesWebApiResources.Get(nameof (UnrecognizedOwnerScopeTypeExceptionMessage));

    public static string UnrecognizedOwnerScopeTypeExceptionMessage(CultureInfo culture) => FavoritesWebApiResources.Get(nameof (UnrecognizedOwnerScopeTypeExceptionMessage), culture);

    public static string FavoritesQuotaExceeededExceptionMessage(
      object arg0,
      object arg1,
      object arg2)
    {
      return FavoritesWebApiResources.Format(nameof (FavoritesQuotaExceeededExceptionMessage), arg0, arg1, arg2);
    }

    public static string FavoritesQuotaExceeededExceptionMessage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return FavoritesWebApiResources.Format(nameof (FavoritesQuotaExceeededExceptionMessage), culture, arg0, arg1, arg2);
    }

    public static string UnknownOwnerIdentityExceptionMessage() => FavoritesWebApiResources.Get(nameof (UnknownOwnerIdentityExceptionMessage));

    public static string UnknownOwnerIdentityExceptionMessage(CultureInfo culture) => FavoritesWebApiResources.Get(nameof (UnknownOwnerIdentityExceptionMessage), culture);

    public static string UnrecognizedArtifactTypeExceptionMessage(object arg0) => FavoritesWebApiResources.Format(nameof (UnrecognizedArtifactTypeExceptionMessage), arg0);

    public static string UnrecognizedArtifactTypeExceptionMessage(object arg0, CultureInfo culture) => FavoritesWebApiResources.Format(nameof (UnrecognizedArtifactTypeExceptionMessage), culture, arg0);

    public static string ExtendedInformationOnlyForOwnFavoritesExceptionMessage() => FavoritesWebApiResources.Get(nameof (ExtendedInformationOnlyForOwnFavoritesExceptionMessage));

    public static string ExtendedInformationOnlyForOwnFavoritesExceptionMessage(CultureInfo culture) => FavoritesWebApiResources.Get(nameof (ExtendedInformationOnlyForOwnFavoritesExceptionMessage), culture);
  }
}
