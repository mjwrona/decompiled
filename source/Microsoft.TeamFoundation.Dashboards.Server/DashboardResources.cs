// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.DashboardResources
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Dashboards
{
  internal static class DashboardResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (DashboardResources), typeof (DashboardResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => DashboardResources.s_resMgr;

    private static string Get(string resourceName) => DashboardResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? DashboardResources.Get(resourceName) : DashboardResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) DashboardResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? DashboardResources.GetInt(resourceName) : (int) DashboardResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) DashboardResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? DashboardResources.GetBool(resourceName) : (bool) DashboardResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => DashboardResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = DashboardResources.Get(resourceName, culture);
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

    public static string DefaultTeamDashboardName() => DashboardResources.Get(nameof (DefaultTeamDashboardName));

    public static string DefaultTeamDashboardName(CultureInfo culture) => DashboardResources.Get(nameof (DefaultTeamDashboardName), culture);

    public static string DefaultAccoutDashboardName() => DashboardResources.Get(nameof (DefaultAccoutDashboardName));

    public static string DefaultAccoutDashboardName(CultureInfo culture) => DashboardResources.Get(nameof (DefaultAccoutDashboardName), culture);

    public static string DefaultDashboardName() => DashboardResources.Get(nameof (DefaultDashboardName));

    public static string DefaultDashboardName(CultureInfo culture) => DashboardResources.Get(nameof (DefaultDashboardName), culture);

    public static string DefaultQueryForAgileProject() => DashboardResources.Get(nameof (DefaultQueryForAgileProject));

    public static string DefaultQueryForAgileProject(CultureInfo culture) => DashboardResources.Get(nameof (DefaultQueryForAgileProject), culture);

    public static string DefaultQueryForCMMIProject() => DashboardResources.Get(nameof (DefaultQueryForCMMIProject));

    public static string DefaultQueryForCMMIProject(CultureInfo culture) => DashboardResources.Get(nameof (DefaultQueryForCMMIProject), culture);

    public static string DefaultQueryForScrumProject() => DashboardResources.Get(nameof (DefaultQueryForScrumProject));

    public static string DefaultQueryForScrumProject(CultureInfo culture) => DashboardResources.Get(nameof (DefaultQueryForScrumProject), culture);

    public static string ErrorTeamAdminOnlyAccess() => DashboardResources.Get(nameof (ErrorTeamAdminOnlyAccess));

    public static string ErrorTeamAdminOnlyAccess(CultureInfo culture) => DashboardResources.Get(nameof (ErrorTeamAdminOnlyAccess), culture);

    public static string ErrorEmptyContributionId() => DashboardResources.Get(nameof (ErrorEmptyContributionId));

    public static string ErrorEmptyContributionId(CultureInfo culture) => DashboardResources.Get(nameof (ErrorEmptyContributionId), culture);

    public static string ErrorWidgetTypeDoesNotExistFormat(object arg0) => DashboardResources.Format(nameof (ErrorWidgetTypeDoesNotExistFormat), arg0);

    public static string ErrorWidgetTypeDoesNotExistFormat(object arg0, CultureInfo culture) => DashboardResources.Format(nameof (ErrorWidgetTypeDoesNotExistFormat), culture, arg0);

    public static string AxService() => DashboardResources.Get(nameof (AxService));

    public static string AxService(CultureInfo culture) => DashboardResources.Get(nameof (AxService), culture);

    public static string SprintBurndownLegacyCatalogName() => DashboardResources.Get(nameof (SprintBurndownLegacyCatalogName));

    public static string SprintBurndownLegacyCatalogName(CultureInfo culture) => DashboardResources.Get(nameof (SprintBurndownLegacyCatalogName), culture);

    public static string SharedQueriesString() => DashboardResources.Get(nameof (SharedQueriesString));

    public static string SharedQueriesString(CultureInfo culture) => DashboardResources.Get(nameof (SharedQueriesString), culture);
  }
}
