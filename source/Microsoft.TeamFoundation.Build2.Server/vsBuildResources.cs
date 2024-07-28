// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.vsBuildResources
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class vsBuildResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (vsBuildResources), typeof (vsBuildResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => vsBuildResources.s_resMgr;

    private static string Get(string resourceName) => vsBuildResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? vsBuildResources.Get(resourceName) : vsBuildResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) vsBuildResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? vsBuildResources.GetInt(resourceName) : (int) vsBuildResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) vsBuildResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? vsBuildResources.GetBool(resourceName) : (bool) vsBuildResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => vsBuildResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = vsBuildResources.Get(resourceName, culture);
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

    public static string Description() => vsBuildResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => vsBuildResources.Get(nameof (Description), culture);

    public static string Name() => vsBuildResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => vsBuildResources.Get(nameof (Name), culture);

    public static string SolutionDescription() => vsBuildResources.Get(nameof (SolutionDescription));

    public static string SolutionDescription(CultureInfo culture) => vsBuildResources.Get(nameof (SolutionDescription), culture);

    public static string SolutionLabel() => vsBuildResources.Get(nameof (SolutionLabel));

    public static string SolutionLabel(CultureInfo culture) => vsBuildResources.Get(nameof (SolutionLabel), culture);
  }
}
