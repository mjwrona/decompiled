// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Environments.Server.EnvironmentResources
// Assembly: Microsoft.Azure.Pipelines.Environments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4A0C9A0D-816B-442F-8D21-CE0F4EA438AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Environments.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.Azure.Pipelines.Environments.Server
{
  internal static class EnvironmentResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (EnvironmentResources), typeof (EnvironmentResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => EnvironmentResources.s_resMgr;

    private static string Get(string resourceName) => EnvironmentResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? EnvironmentResources.Get(resourceName) : EnvironmentResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) EnvironmentResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? EnvironmentResources.GetInt(resourceName) : (int) EnvironmentResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) EnvironmentResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? EnvironmentResources.GetBool(resourceName) : (bool) EnvironmentResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => EnvironmentResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = EnvironmentResources.Get(resourceName, culture);
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

    public static string RequestedMoreThanMaxSupport() => EnvironmentResources.Get(nameof (RequestedMoreThanMaxSupport));

    public static string RequestedMoreThanMaxSupport(CultureInfo culture) => EnvironmentResources.Get(nameof (RequestedMoreThanMaxSupport), culture);
  }
}
