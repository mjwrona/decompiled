// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Admin.FeatureDisplayName
// Assembly: Microsoft.TeamFoundation.Admin.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4DC7473-FE52-49C1-BB5D-1E769BB5001D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Admin.Common.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Admin
{
  internal static class FeatureDisplayName
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (FeatureDisplayName), typeof (FeatureDisplayName).GetTypeInfo().Assembly);

    public static ResourceManager Manager => FeatureDisplayName.s_resMgr;

    private static string Get(string resourceName) => FeatureDisplayName.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? FeatureDisplayName.Get(resourceName) : FeatureDisplayName.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) FeatureDisplayName.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? FeatureDisplayName.GetInt(resourceName) : (int) FeatureDisplayName.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) FeatureDisplayName.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? FeatureDisplayName.GetBool(resourceName) : (bool) FeatureDisplayName.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => FeatureDisplayName.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = FeatureDisplayName.Get(resourceName, culture);
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

    public static string ApplicationTier() => FeatureDisplayName.Get(nameof (ApplicationTier));

    public static string ApplicationTier(CultureInfo culture) => FeatureDisplayName.Get(nameof (ApplicationTier), culture);

    public static string ObjectModel() => FeatureDisplayName.Get(nameof (ObjectModel));

    public static string ObjectModel(CultureInfo culture) => FeatureDisplayName.Get(nameof (ObjectModel), culture);

    public static string Tools() => FeatureDisplayName.Get(nameof (Tools));

    public static string Tools(CultureInfo culture) => FeatureDisplayName.Get(nameof (Tools), culture);

    public static string VersionControlProxy() => FeatureDisplayName.Get(nameof (VersionControlProxy));

    public static string VersionControlProxy(CultureInfo culture) => FeatureDisplayName.Get(nameof (VersionControlProxy), culture);

    public static string Search() => FeatureDisplayName.Get(nameof (Search));

    public static string Search(CultureInfo culture) => FeatureDisplayName.Get(nameof (Search), culture);
  }
}
