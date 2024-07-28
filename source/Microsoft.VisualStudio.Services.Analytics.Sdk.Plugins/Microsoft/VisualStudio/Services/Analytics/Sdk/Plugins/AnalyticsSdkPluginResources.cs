// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins.AnalyticsSdkPluginResources
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3E9FDCC8-8891-4D47-89A2-C972B6459647
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins
{
  internal static class AnalyticsSdkPluginResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (AnalyticsSdkPluginResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => AnalyticsSdkPluginResources.s_resMgr;

    private static string Get(string resourceName) => AnalyticsSdkPluginResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? AnalyticsSdkPluginResources.Get(resourceName) : AnalyticsSdkPluginResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) AnalyticsSdkPluginResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? AnalyticsSdkPluginResources.GetInt(resourceName) : (int) AnalyticsSdkPluginResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) AnalyticsSdkPluginResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? AnalyticsSdkPluginResources.GetBool(resourceName) : (bool) AnalyticsSdkPluginResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => AnalyticsSdkPluginResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = AnalyticsSdkPluginResources.Get(resourceName, culture);
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

    public static string TABLE_NOT_FOUND_IN_STAGE_TABLE_NAMES() => AnalyticsSdkPluginResources.Get(nameof (TABLE_NOT_FOUND_IN_STAGE_TABLE_NAMES));

    public static string TABLE_NOT_FOUND_IN_STAGE_TABLE_NAMES(CultureInfo culture) => AnalyticsSdkPluginResources.Get(nameof (TABLE_NOT_FOUND_IN_STAGE_TABLE_NAMES), culture);
  }
}
