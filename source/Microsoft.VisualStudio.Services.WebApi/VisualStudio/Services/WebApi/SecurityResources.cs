// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.SecurityResources
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.WebApi
{
  internal static class SecurityResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (SecurityResources), typeof (SecurityResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => SecurityResources.s_resMgr;

    private static string Get(string resourceName) => SecurityResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? SecurityResources.Get(resourceName) : SecurityResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) SecurityResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? SecurityResources.GetInt(resourceName) : (int) SecurityResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) SecurityResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? SecurityResources.GetBool(resourceName) : (bool) SecurityResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => SecurityResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = SecurityResources.Get(resourceName, culture);
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

    public static string InvalidAclStoreException(object arg0, object arg1) => SecurityResources.Format(nameof (InvalidAclStoreException), arg0, arg1);

    public static string InvalidAclStoreException(object arg0, object arg1, CultureInfo culture) => SecurityResources.Format(nameof (InvalidAclStoreException), culture, arg0, arg1);

    public static string InvalidPermissionsException(object arg0, object arg1) => SecurityResources.Format(nameof (InvalidPermissionsException), arg0, arg1);

    public static string InvalidPermissionsException(object arg0, object arg1, CultureInfo culture) => SecurityResources.Format(nameof (InvalidPermissionsException), culture, arg0, arg1);
  }
}
