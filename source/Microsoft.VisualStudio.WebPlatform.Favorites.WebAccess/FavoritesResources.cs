// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.WebPlatform.Favorites.WebAccess.FavoritesResources
// Assembly: Microsoft.VisualStudio.WebPlatform.Favorites.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B09BDB0-7575-41B5-8197-FCB4157B6C4A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.WebPlatform.Favorites.WebAccess.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.WebPlatform.Favorites.WebAccess
{
  internal static class FavoritesResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (FavoritesResources), typeof (FavoritesResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => FavoritesResources.s_resMgr;

    private static string Get(string resourceName) => FavoritesResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? FavoritesResources.Get(resourceName) : FavoritesResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) FavoritesResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? FavoritesResources.GetInt(resourceName) : (int) FavoritesResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) FavoritesResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? FavoritesResources.GetBool(resourceName) : (bool) FavoritesResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => FavoritesResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = FavoritesResources.Get(resourceName, culture);
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

    public static string FavoriteItemPickerBrowseAllText() => FavoritesResources.Get(nameof (FavoriteItemPickerBrowseAllText));

    public static string FavoriteItemPickerBrowseAllText(CultureInfo culture) => FavoritesResources.Get(nameof (FavoriteItemPickerBrowseAllText), culture);

    public static string FavoriteItemPickerLoadingMessage() => FavoritesResources.Get(nameof (FavoriteItemPickerLoadingMessage));

    public static string FavoriteItemPickerLoadingMessage(CultureInfo culture) => FavoritesResources.Get(nameof (FavoriteItemPickerLoadingMessage), culture);

    public static string FavoriteItemPickerNoItemsMessage(object arg0) => FavoritesResources.Format(nameof (FavoriteItemPickerNoItemsMessage), arg0);

    public static string FavoriteItemPickerNoItemsMessage(object arg0, CultureInfo culture) => FavoritesResources.Format(nameof (FavoriteItemPickerNoItemsMessage), culture, arg0);

    public static string FavoriteItemPickerSearchHeader() => FavoritesResources.Get(nameof (FavoriteItemPickerSearchHeader));

    public static string FavoriteItemPickerSearchHeader(CultureInfo culture) => FavoritesResources.Get(nameof (FavoriteItemPickerSearchHeader), culture);

    public static string FavoriteItemPickerSearchNoResults() => FavoritesResources.Get(nameof (FavoriteItemPickerSearchNoResults));

    public static string FavoriteItemPickerSearchNoResults(CultureInfo culture) => FavoritesResources.Get(nameof (FavoriteItemPickerSearchNoResults), culture);

    public static string FavoriteItemPickerSearchText() => FavoritesResources.Get(nameof (FavoriteItemPickerSearchText));

    public static string FavoriteItemPickerSearchText(CultureInfo culture) => FavoritesResources.Get(nameof (FavoriteItemPickerSearchText), culture);

    public static string FavoriteItemPickerStarAriaLabel() => FavoritesResources.Get(nameof (FavoriteItemPickerStarAriaLabel));

    public static string FavoriteItemPickerStarAriaLabel(CultureInfo culture) => FavoritesResources.Get(nameof (FavoriteItemPickerStarAriaLabel), culture);

    public static string FavoriteLabel() => FavoritesResources.Get(nameof (FavoriteLabel));

    public static string FavoriteLabel(CultureInfo culture) => FavoritesResources.Get(nameof (FavoriteLabel), culture);

    public static string FavoriteStarFavoritedTitle() => FavoritesResources.Get(nameof (FavoriteStarFavoritedTitle));

    public static string FavoriteStarFavoritedTitle(CultureInfo culture) => FavoritesResources.Get(nameof (FavoriteStarFavoritedTitle), culture);

    public static string FavoriteStarUnfavoritedTitle() => FavoritesResources.Get(nameof (FavoriteStarUnfavoritedTitle));

    public static string FavoriteStarUnfavoritedTitle(CultureInfo culture) => FavoritesResources.Get(nameof (FavoriteStarUnfavoritedTitle), culture);

    public static string FavoriteItemPickerAriaDescription() => FavoritesResources.Get(nameof (FavoriteItemPickerAriaDescription));

    public static string FavoriteItemPickerAriaDescription(CultureInfo culture) => FavoritesResources.Get(nameof (FavoriteItemPickerAriaDescription), culture);

    public static string FavoriteItemPickerGroupHeader() => FavoritesResources.Get(nameof (FavoriteItemPickerGroupHeader));

    public static string FavoriteItemPickerGroupHeader(CultureInfo culture) => FavoritesResources.Get(nameof (FavoriteItemPickerGroupHeader), culture);
  }
}
