// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SecurityRoles.SecurityRolesResources
// Assembly: Microsoft.VisualStudio.Services.SecurityRoles.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BBA245E2-CEA0-4262-9E17-EB6FDFC84F54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SecurityRoles.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.SecurityRoles
{
  internal static class SecurityRolesResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (SecurityRolesResources).GetTypeInfo().Assembly);

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

    public static string CannotModifySelfRole() => SecurityRolesResources.Get(nameof (CannotModifySelfRole));

    public static string CannotModifySelfRole(CultureInfo culture) => SecurityRolesResources.Get(nameof (CannotModifySelfRole), culture);

    public static string RoleNotFoundException(object arg0) => SecurityRolesResources.Format(nameof (RoleNotFoundException), arg0);

    public static string RoleNotFoundException(object arg0, CultureInfo culture) => SecurityRolesResources.Format(nameof (RoleNotFoundException), culture, arg0);

    public static string ScopeNotFoundException(object arg0) => SecurityRolesResources.Format(nameof (ScopeNotFoundException), arg0);

    public static string ScopeNotFoundException(object arg0, CultureInfo culture) => SecurityRolesResources.Format(nameof (ScopeNotFoundException), culture, arg0);
  }
}
