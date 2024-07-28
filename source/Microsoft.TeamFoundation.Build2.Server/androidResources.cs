// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.androidResources
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class androidResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (androidResources), typeof (androidResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => androidResources.s_resMgr;

    private static string Get(string resourceName) => androidResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? androidResources.Get(resourceName) : androidResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) androidResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? androidResources.GetInt(resourceName) : (int) androidResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) androidResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? androidResources.GetBool(resourceName) : (bool) androidResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => androidResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = androidResources.Get(resourceName, culture);
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

    public static string Description() => androidResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => androidResources.Get(nameof (Description), culture);

    public static string GradleTasks() => androidResources.Get(nameof (GradleTasks));

    public static string GradleTasks(CultureInfo culture) => androidResources.Get(nameof (GradleTasks), culture);

    public static string GradleWrapper() => androidResources.Get(nameof (GradleWrapper));

    public static string GradleWrapper(CultureInfo culture) => androidResources.Get(nameof (GradleWrapper), culture);

    public static string Name() => androidResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => androidResources.Get(nameof (Name), culture);
  }
}
