// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Favorites.FavoritesResources
// Assembly: Microsoft.TeamFoundation.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4742830F-DF0E-4509-8C2D-2540DAED73F4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Favorites.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Favorites
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
  }
}
