// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.RecentActivity.Resources
// Assembly: Microsoft.Azure.Boards.RecentActivity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 684DCFA4-4764-4794-94A6-960AF811434C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.RecentActivity.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.Azure.Boards.RecentActivity
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.Azure.Boards.RecentActivity.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.Azure.Boards.RecentActivity.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.Azure.Boards.RecentActivity.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.Azure.Boards.RecentActivity.Resources.Get(resourceName) : Microsoft.Azure.Boards.RecentActivity.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.Azure.Boards.RecentActivity.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.Azure.Boards.RecentActivity.Resources.GetInt(resourceName) : (int) Microsoft.Azure.Boards.RecentActivity.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.Azure.Boards.RecentActivity.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.Azure.Boards.RecentActivity.Resources.GetBool(resourceName) : (bool) Microsoft.Azure.Boards.RecentActivity.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.Azure.Boards.RecentActivity.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.Azure.Boards.RecentActivity.Resources.Get(resourceName, culture);
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

    public static string InvalidActivity() => Microsoft.Azure.Boards.RecentActivity.Resources.Get(nameof (InvalidActivity));

    public static string InvalidActivity(CultureInfo culture) => Microsoft.Azure.Boards.RecentActivity.Resources.Get(nameof (InvalidActivity), culture);

    public static string CouldNotFindArtifactProvider(object arg0, object arg1) => Microsoft.Azure.Boards.RecentActivity.Resources.Format(nameof (CouldNotFindArtifactProvider), arg0, arg1);

    public static string CouldNotFindArtifactProvider(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return Microsoft.Azure.Boards.RecentActivity.Resources.Format(nameof (CouldNotFindArtifactProvider), culture, arg0, arg1);
    }

    public static string AllActivitiesShouldBeOfSameKind() => Microsoft.Azure.Boards.RecentActivity.Resources.Get(nameof (AllActivitiesShouldBeOfSameKind));

    public static string AllActivitiesShouldBeOfSameKind(CultureInfo culture) => Microsoft.Azure.Boards.RecentActivity.Resources.Get(nameof (AllActivitiesShouldBeOfSameKind), culture);
  }
}
