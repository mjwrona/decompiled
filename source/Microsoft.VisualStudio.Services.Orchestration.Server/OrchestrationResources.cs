// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.OrchestrationResources
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  internal static class OrchestrationResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (OrchestrationResources), typeof (OrchestrationResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => OrchestrationResources.s_resMgr;

    private static string Get(string resourceName) => OrchestrationResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? OrchestrationResources.Get(resourceName) : OrchestrationResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) OrchestrationResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? OrchestrationResources.GetInt(resourceName) : (int) OrchestrationResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) OrchestrationResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? OrchestrationResources.GetBool(resourceName) : (bool) OrchestrationResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => OrchestrationResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = OrchestrationResources.Get(resourceName, culture);
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

    public static string OrchestrationDispatcherName() => OrchestrationResources.Get(nameof (OrchestrationDispatcherName));

    public static string OrchestrationDispatcherName(CultureInfo culture) => OrchestrationResources.Get(nameof (OrchestrationDispatcherName), culture);

    public static string ActivityDispatcherName() => OrchestrationResources.Get(nameof (ActivityDispatcherName));

    public static string ActivityDispatcherName(CultureInfo culture) => OrchestrationResources.Get(nameof (ActivityDispatcherName), culture);

    public static string HubNotFound(object arg0) => OrchestrationResources.Format(nameof (HubNotFound), arg0);

    public static string HubNotFound(object arg0, CultureInfo culture) => OrchestrationResources.Format(nameof (HubNotFound), culture, arg0);

    public static string SessionExceededThreshold(object arg0, object arg1) => OrchestrationResources.Format(nameof (SessionExceededThreshold), arg0, arg1);

    public static string SessionExceededThreshold(object arg0, object arg1, CultureInfo culture) => OrchestrationResources.Format(nameof (SessionExceededThreshold), culture, arg0, arg1);

    public static string HubExists(object arg0) => OrchestrationResources.Format(nameof (HubExists), arg0);

    public static string HubExists(object arg0, CultureInfo culture) => OrchestrationResources.Format(nameof (HubExists), culture, arg0);

    public static string SessionExists(object arg0, object arg1) => OrchestrationResources.Format(nameof (SessionExists), arg0, arg1);

    public static string SessionExists(object arg0, object arg1, CultureInfo culture) => OrchestrationResources.Format(nameof (SessionExists), culture, arg0, arg1);

    public static string SessionNotFound(object arg0, object arg1) => OrchestrationResources.Format(nameof (SessionNotFound), arg0, arg1);

    public static string SessionNotFound(object arg0, object arg1, CultureInfo culture) => OrchestrationResources.Format(nameof (SessionNotFound), culture, arg0, arg1);

    public static string CustomActivityDispatcherName(object arg0) => OrchestrationResources.Format(nameof (CustomActivityDispatcherName), arg0);

    public static string CustomActivityDispatcherName(object arg0, CultureInfo culture) => OrchestrationResources.Format(nameof (CustomActivityDispatcherName), culture, arg0);

    public static string MissingActivityMessageContent(object arg0) => OrchestrationResources.Format(nameof (MissingActivityMessageContent), arg0);

    public static string MissingActivityMessageContent(object arg0, CultureInfo culture) => OrchestrationResources.Format(nameof (MissingActivityMessageContent), culture, arg0);
  }
}
