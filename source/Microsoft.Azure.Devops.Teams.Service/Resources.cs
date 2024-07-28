// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Teams.Service.Resources
// Assembly: Microsoft.Azure.Devops.Teams.Service, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 643BB522-27FA-4CE7-8A1E-31D95F8AC409
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Teams.Service.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.Azure.Devops.Teams.Service
{
  internal static class Resources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Resources), typeof (Microsoft.Azure.Devops.Teams.Service.Resources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Microsoft.Azure.Devops.Teams.Service.Resources.s_resMgr;

    private static string Get(string resourceName) => Microsoft.Azure.Devops.Teams.Service.Resources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Microsoft.Azure.Devops.Teams.Service.Resources.Get(resourceName) : Microsoft.Azure.Devops.Teams.Service.Resources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Microsoft.Azure.Devops.Teams.Service.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Microsoft.Azure.Devops.Teams.Service.Resources.GetInt(resourceName) : (int) Microsoft.Azure.Devops.Teams.Service.Resources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Microsoft.Azure.Devops.Teams.Service.Resources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Microsoft.Azure.Devops.Teams.Service.Resources.GetBool(resourceName) : (bool) Microsoft.Azure.Devops.Teams.Service.Resources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Microsoft.Azure.Devops.Teams.Service.Resources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Microsoft.Azure.Devops.Teams.Service.Resources.Get(resourceName, culture);
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

    public static string TeamNotFound(object arg0) => Microsoft.Azure.Devops.Teams.Service.Resources.Format(nameof (TeamNotFound), arg0);

    public static string TeamNotFound(object arg0, CultureInfo culture) => Microsoft.Azure.Devops.Teams.Service.Resources.Format(nameof (TeamNotFound), culture, arg0);

    public static string DefaultTeamNotFound(object arg0) => Microsoft.Azure.Devops.Teams.Service.Resources.Format(nameof (DefaultTeamNotFound), arg0);

    public static string DefaultTeamNotFound(object arg0, CultureInfo culture) => Microsoft.Azure.Devops.Teams.Service.Resources.Format(nameof (DefaultTeamNotFound), culture, arg0);

    public static string TeamPermission_ReadError() => Microsoft.Azure.Devops.Teams.Service.Resources.Get(nameof (TeamPermission_ReadError));

    public static string TeamPermission_ReadError(CultureInfo culture) => Microsoft.Azure.Devops.Teams.Service.Resources.Get(nameof (TeamPermission_ReadError), culture);

    public static string TeamPermission_WriteError() => Microsoft.Azure.Devops.Teams.Service.Resources.Get(nameof (TeamPermission_WriteError));

    public static string TeamPermission_WriteError(CultureInfo culture) => Microsoft.Azure.Devops.Teams.Service.Resources.Get(nameof (TeamPermission_WriteError), culture);

    public static string TeamServiceSafeguardBlockCreateTeamAndAdminOperations_Error() => Microsoft.Azure.Devops.Teams.Service.Resources.Get(nameof (TeamServiceSafeguardBlockCreateTeamAndAdminOperations_Error));

    public static string TeamServiceSafeguardBlockCreateTeamAndAdminOperations_Error(
      CultureInfo culture)
    {
      return Microsoft.Azure.Devops.Teams.Service.Resources.Get(nameof (TeamServiceSafeguardBlockCreateTeamAndAdminOperations_Error), culture);
    }

    public static string ProcessTemplateStatusNotReady(object arg0) => Microsoft.Azure.Devops.Teams.Service.Resources.Format(nameof (ProcessTemplateStatusNotReady), arg0);

    public static string ProcessTemplateStatusNotReady(object arg0, CultureInfo culture) => Microsoft.Azure.Devops.Teams.Service.Resources.Format(nameof (ProcessTemplateStatusNotReady), culture, arg0);

    public static string TeamNotFoundInUrl() => Microsoft.Azure.Devops.Teams.Service.Resources.Get(nameof (TeamNotFoundInUrl));

    public static string TeamNotFoundInUrl(CultureInfo culture) => Microsoft.Azure.Devops.Teams.Service.Resources.Get(nameof (TeamNotFoundInUrl), culture);

    public static string NoTeamsFoundForProject(object arg0) => Microsoft.Azure.Devops.Teams.Service.Resources.Format(nameof (NoTeamsFoundForProject), arg0);

    public static string NoTeamsFoundForProject(object arg0, CultureInfo culture) => Microsoft.Azure.Devops.Teams.Service.Resources.Format(nameof (NoTeamsFoundForProject), culture, arg0);
  }
}
