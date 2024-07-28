// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.AspNetCoreResources
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class AspNetCoreResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (AspNetCoreResources), typeof (AspNetCoreResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => AspNetCoreResources.s_resMgr;

    private static string Get(string resourceName) => AspNetCoreResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? AspNetCoreResources.Get(resourceName) : AspNetCoreResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) AspNetCoreResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? AspNetCoreResources.GetInt(resourceName) : (int) AspNetCoreResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) AspNetCoreResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? AspNetCoreResources.GetBool(resourceName) : (bool) AspNetCoreResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => AspNetCoreResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = AspNetCoreResources.Get(resourceName, culture);
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

    public static string Description() => AspNetCoreResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => AspNetCoreResources.Get(nameof (Description), culture);

    public static string Name() => AspNetCoreResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => AspNetCoreResources.Get(nameof (Name), culture);

    public static string RestoreBuildProject() => AspNetCoreResources.Get(nameof (RestoreBuildProject));

    public static string RestoreBuildProject(CultureInfo culture) => AspNetCoreResources.Get(nameof (RestoreBuildProject), culture);

    public static string TestProject() => AspNetCoreResources.Get(nameof (TestProject));

    public static string TestProject(CultureInfo culture) => AspNetCoreResources.Get(nameof (TestProject), culture);

    public static string CsprojHelpMarkdown() => AspNetCoreResources.Get(nameof (CsprojHelpMarkdown));

    public static string CsprojHelpMarkdown(CultureInfo culture) => AspNetCoreResources.Get(nameof (CsprojHelpMarkdown), culture);
  }
}
