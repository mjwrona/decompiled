// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.xamariniosResources
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class xamariniosResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (xamariniosResources), typeof (xamariniosResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => xamariniosResources.s_resMgr;

    private static string Get(string resourceName) => xamariniosResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? xamariniosResources.Get(resourceName) : xamariniosResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) xamariniosResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? xamariniosResources.GetInt(resourceName) : (int) xamariniosResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) xamariniosResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? xamariniosResources.GetBool(resourceName) : (bool) xamariniosResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => xamariniosResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = xamariniosResources.Get(resourceName, culture);
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

    public static string AppFileDescription() => xamariniosResources.Get(nameof (AppFileDescription));

    public static string AppFileDescription(CultureInfo culture) => xamariniosResources.Get(nameof (AppFileDescription), culture);

    public static string AppFileLabel() => xamariniosResources.Get(nameof (AppFileLabel));

    public static string AppFileLabel(CultureInfo culture) => xamariniosResources.Get(nameof (AppFileLabel), culture);

    public static string Description() => xamariniosResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => xamariniosResources.Get(nameof (Description), culture);

    public static string Name() => xamariniosResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => xamariniosResources.Get(nameof (Name), culture);

    public static string SolutionDescription() => xamariniosResources.Get(nameof (SolutionDescription));

    public static string SolutionDescription(CultureInfo culture) => xamariniosResources.Get(nameof (SolutionDescription), culture);

    public static string SolutionLabel() => xamariniosResources.Get(nameof (SolutionLabel));

    public static string SolutionLabel(CultureInfo culture) => xamariniosResources.Get(nameof (SolutionLabel), culture);
  }
}
