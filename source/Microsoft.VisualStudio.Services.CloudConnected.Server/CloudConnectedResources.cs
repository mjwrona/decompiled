// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CloudConnected.CloudConnectedResources
// Assembly: Microsoft.VisualStudio.Services.CloudConnected.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6AAFC756-39E6-4247-9102-7DC33B035E4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CloudConnected.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.CloudConnected
{
  internal static class CloudConnectedResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (CloudConnectedResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => CloudConnectedResources.s_resMgr;

    private static string Get(string resourceName) => CloudConnectedResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? CloudConnectedResources.Get(resourceName) : CloudConnectedResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) CloudConnectedResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? CloudConnectedResources.GetInt(resourceName) : (int) CloudConnectedResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) CloudConnectedResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? CloudConnectedResources.GetBool(resourceName) : (bool) CloudConnectedResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => CloudConnectedResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = CloudConnectedResources.Get(resourceName, culture);
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

    public static string InvalidClientId(object arg0, object arg1) => CloudConnectedResources.Format(nameof (InvalidClientId), arg0, arg1);

    public static string InvalidClientId(object arg0, object arg1, CultureInfo culture) => CloudConnectedResources.Format(nameof (InvalidClientId), culture, arg0, arg1);
  }
}
