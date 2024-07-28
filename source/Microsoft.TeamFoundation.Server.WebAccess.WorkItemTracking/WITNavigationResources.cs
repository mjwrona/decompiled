// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.WITNavigationResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 74AD14A4-225D-46D2-B154-945941A2D167
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking
{
  public static class WITNavigationResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (WITNavigationResources), typeof (WITNavigationResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => WITNavigationResources.s_resMgr;

    private static string Get(string resourceName) => WITNavigationResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? WITNavigationResources.Get(resourceName) : WITNavigationResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) WITNavigationResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? WITNavigationResources.GetInt(resourceName) : (int) WITNavigationResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) WITNavigationResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? WITNavigationResources.GetBool(resourceName) : (bool) WITNavigationResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => WITNavigationResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = WITNavigationResources.Get(resourceName, culture);
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

    public static string NewWorkItemActionTitle(object arg0) => WITNavigationResources.Format(nameof (NewWorkItemActionTitle), arg0);

    public static string NewWorkItemActionTitle(object arg0, CultureInfo culture) => WITNavigationResources.Format(nameof (NewWorkItemActionTitle), culture, arg0);

    public static string AllWorkItemsTitle() => WITNavigationResources.Get(nameof (AllWorkItemsTitle));

    public static string AllWorkItemsTitle(CultureInfo culture) => WITNavigationResources.Get(nameof (AllWorkItemsTitle), culture);

    public static string NewWorkItemSubmenuTitle() => WITNavigationResources.Get(nameof (NewWorkItemSubmenuTitle));

    public static string NewWorkItemSubmenuTitle(CultureInfo culture) => WITNavigationResources.Get(nameof (NewWorkItemSubmenuTitle), culture);
  }
}
