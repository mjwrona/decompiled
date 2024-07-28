// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SecurityRoles.WebApi.SecurityRolesResources
// Assembly: Microsoft.VisualStudio.Services.SecurityRoles.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 74D9BC5A-4C7E-4BC3-9331-A0A75718A098
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.SecurityRoles.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.SecurityRoles.WebApi
{
  internal static class SecurityRolesResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (SecurityRolesResources), typeof (SecurityRolesResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => SecurityRolesResources.s_resMgr;

    private static string Get(string resourceName) => SecurityRolesResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? SecurityRolesResources.Get(resourceName) : SecurityRolesResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) SecurityRolesResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? SecurityRolesResources.GetInt(resourceName) : (int) SecurityRolesResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) SecurityRolesResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? SecurityRolesResources.GetBool(resourceName) : (bool) SecurityRolesResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => SecurityRolesResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = SecurityRolesResources.Get(resourceName, culture);
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

    public static string AccessAssigned() => SecurityRolesResources.Get(nameof (AccessAssigned));

    public static string AccessAssigned(CultureInfo culture) => SecurityRolesResources.Get(nameof (AccessAssigned), culture);

    public static string AccessInherited() => SecurityRolesResources.Get(nameof (AccessInherited));

    public static string AccessInherited(CultureInfo culture) => SecurityRolesResources.Get(nameof (AccessInherited), culture);
  }
}
