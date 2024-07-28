// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.ServicingOrchestrationResources
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  internal static class ServicingOrchestrationResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ServicingOrchestrationResources), typeof (ServicingOrchestrationResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ServicingOrchestrationResources.s_resMgr;

    private static string Get(string resourceName) => ServicingOrchestrationResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ServicingOrchestrationResources.Get(resourceName) : ServicingOrchestrationResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ServicingOrchestrationResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ServicingOrchestrationResources.GetInt(resourceName) : (int) ServicingOrchestrationResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ServicingOrchestrationResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ServicingOrchestrationResources.GetBool(resourceName) : (bool) ServicingOrchestrationResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ServicingOrchestrationResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ServicingOrchestrationResources.Get(resourceName, culture);
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

    public static string ServicingOrchestrationAlreadyExistsException(object arg0) => ServicingOrchestrationResources.Format(nameof (ServicingOrchestrationAlreadyExistsException), arg0);

    public static string ServicingOrchestrationAlreadyExistsException(
      object arg0,
      CultureInfo culture)
    {
      return ServicingOrchestrationResources.Format(nameof (ServicingOrchestrationAlreadyExistsException), culture, arg0);
    }

    public static string ServicingOrchestrationDoesNotExistException(object arg0) => ServicingOrchestrationResources.Format(nameof (ServicingOrchestrationDoesNotExistException), arg0);

    public static string ServicingOrchestrationDoesNotExistException(
      object arg0,
      CultureInfo culture)
    {
      return ServicingOrchestrationResources.Format(nameof (ServicingOrchestrationDoesNotExistException), culture, arg0);
    }
  }
}
