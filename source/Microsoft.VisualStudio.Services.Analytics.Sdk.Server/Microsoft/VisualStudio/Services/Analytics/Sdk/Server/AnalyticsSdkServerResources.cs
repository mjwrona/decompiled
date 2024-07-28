// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Sdk.Server.AnalyticsSdkServerResources
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E5A0742E-601C-4AD5-8902-781963AA7C5A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Analytics.Sdk.Server
{
  internal static class AnalyticsSdkServerResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (AnalyticsSdkServerResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => AnalyticsSdkServerResources.s_resMgr;

    private static string Get(string resourceName) => AnalyticsSdkServerResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? AnalyticsSdkServerResources.Get(resourceName) : AnalyticsSdkServerResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) AnalyticsSdkServerResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? AnalyticsSdkServerResources.GetInt(resourceName) : (int) AnalyticsSdkServerResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) AnalyticsSdkServerResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? AnalyticsSdkServerResources.GetBool(resourceName) : (bool) AnalyticsSdkServerResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => AnalyticsSdkServerResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = AnalyticsSdkServerResources.Get(resourceName, culture);
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

    public static string JOB_DEFINITIONS_JOB_IDS_MIS_MATCH(object arg0, object arg1) => AnalyticsSdkServerResources.Format(nameof (JOB_DEFINITIONS_JOB_IDS_MIS_MATCH), arg0, arg1);

    public static string JOB_DEFINITIONS_JOB_IDS_MIS_MATCH(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AnalyticsSdkServerResources.Format(nameof (JOB_DEFINITIONS_JOB_IDS_MIS_MATCH), culture, arg0, arg1);
    }

    public static string MISSING_JOB_DEFINITIONS(object arg0) => AnalyticsSdkServerResources.Format(nameof (MISSING_JOB_DEFINITIONS), arg0);

    public static string MISSING_JOB_DEFINITIONS(object arg0, CultureInfo culture) => AnalyticsSdkServerResources.Format(nameof (MISSING_JOB_DEFINITIONS), culture, arg0);
  }
}
