// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Dashboards.DashboardsServerResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Dashboards, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1A53BFE3-D2EE-4259-A1B0-9683B82268B4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Dashboards.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Server.WebAccess.Dashboards
{
  internal static class DashboardsServerResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (DashboardsServerResources), typeof (DashboardsServerResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => DashboardsServerResources.s_resMgr;

    private static string Get(string resourceName) => DashboardsServerResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? DashboardsServerResources.Get(resourceName) : DashboardsServerResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) DashboardsServerResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? DashboardsServerResources.GetInt(resourceName) : (int) DashboardsServerResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) DashboardsServerResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? DashboardsServerResources.GetBool(resourceName) : (bool) DashboardsServerResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => DashboardsServerResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = DashboardsServerResources.Get(resourceName, culture);
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

    public static string AddWidgetButtonTitle() => DashboardsServerResources.Get(nameof (AddWidgetButtonTitle));

    public static string AddWidgetButtonTitle(CultureInfo culture) => DashboardsServerResources.Get(nameof (AddWidgetButtonTitle), culture);

    public static string RemoveFromDashboardTitle() => DashboardsServerResources.Get(nameof (RemoveFromDashboardTitle));

    public static string RemoveFromDashboardTitle(CultureInfo culture) => DashboardsServerResources.Get(nameof (RemoveFromDashboardTitle), culture);

    public static string ErrorCreateDashboardNameRequired() => DashboardsServerResources.Get(nameof (ErrorCreateDashboardNameRequired));

    public static string ErrorCreateDashboardNameRequired(CultureInfo culture) => DashboardsServerResources.Get(nameof (ErrorCreateDashboardNameRequired), culture);

    public static string WidgetHost_CreationError(object arg0, object arg1) => DashboardsServerResources.Format(nameof (WidgetHost_CreationError), arg0, arg1);

    public static string WidgetHost_CreationError(object arg0, object arg1, CultureInfo culture) => DashboardsServerResources.Format(nameof (WidgetHost_CreationError), culture, arg0, arg1);

    public static string WidgetHost_InitError(object arg0, object arg1) => DashboardsServerResources.Format(nameof (WidgetHost_InitError), arg0, arg1);

    public static string WidgetHost_InitError(object arg0, object arg1, CultureInfo culture) => DashboardsServerResources.Format(nameof (WidgetHost_InitError), culture, arg0, arg1);

    public static string ErrorMessage_AddWidgetToDashboard() => DashboardsServerResources.Get(nameof (ErrorMessage_AddWidgetToDashboard));

    public static string ErrorMessage_AddWidgetToDashboard(CultureInfo culture) => DashboardsServerResources.Get(nameof (ErrorMessage_AddWidgetToDashboard), culture);

    public static string ErrorMessage_DashboardList() => DashboardsServerResources.Get(nameof (ErrorMessage_DashboardList));

    public static string ErrorMessage_DashboardList(CultureInfo culture) => DashboardsServerResources.Get(nameof (ErrorMessage_DashboardList), culture);

    public static string ErrorMessage_DashboardMenuList() => DashboardsServerResources.Get(nameof (ErrorMessage_DashboardMenuList));

    public static string ErrorMessage_DashboardMenuList(CultureInfo culture) => DashboardsServerResources.Get(nameof (ErrorMessage_DashboardMenuList), culture);

    public static string ErrorMessage_RemoveWidgetFromAllDashboard() => DashboardsServerResources.Get(nameof (ErrorMessage_RemoveWidgetFromAllDashboard));

    public static string ErrorMessage_RemoveWidgetFromAllDashboard(CultureInfo culture) => DashboardsServerResources.Get(nameof (ErrorMessage_RemoveWidgetFromAllDashboard), culture);

    public static string ErrorMessage_RemoveWidgetFromDashboard() => DashboardsServerResources.Get(nameof (ErrorMessage_RemoveWidgetFromDashboard));

    public static string ErrorMessage_RemoveWidgetFromDashboard(CultureInfo culture) => DashboardsServerResources.Get(nameof (ErrorMessage_RemoveWidgetFromDashboard), culture);

    public static string MenuSaveNewDashboard() => DashboardsServerResources.Get(nameof (MenuSaveNewDashboard));

    public static string MenuSaveNewDashboard(CultureInfo culture) => DashboardsServerResources.Get(nameof (MenuSaveNewDashboard), culture);

    public static string ErrorMessage_GenericNetworkError() => DashboardsServerResources.Get(nameof (ErrorMessage_GenericNetworkError));

    public static string ErrorMessage_GenericNetworkError(CultureInfo culture) => DashboardsServerResources.Get(nameof (ErrorMessage_GenericNetworkError), culture);

    public static string ConfirmationMessage_Leaving() => DashboardsServerResources.Get(nameof (ConfirmationMessage_Leaving));

    public static string ConfirmationMessage_Leaving(CultureInfo culture) => DashboardsServerResources.Get(nameof (ConfirmationMessage_Leaving), culture);

    public static string ConfigureFromCatalog() => DashboardsServerResources.Get(nameof (ConfigureFromCatalog));

    public static string ConfigureFromCatalog(CultureInfo culture) => DashboardsServerResources.Get(nameof (ConfigureFromCatalog), culture);

    public static string WidgetCatalog_ConfigureButtonTitle() => DashboardsServerResources.Get(nameof (WidgetCatalog_ConfigureButtonTitle));

    public static string WidgetCatalog_ConfigureButtonTitle(CultureInfo culture) => DashboardsServerResources.Get(nameof (WidgetCatalog_ConfigureButtonTitle), culture);

    public static string InvalidJsonObject() => DashboardsServerResources.Get(nameof (InvalidJsonObject));

    public static string InvalidJsonObject(CultureInfo culture) => DashboardsServerResources.Get(nameof (InvalidJsonObject), culture);

    public static string WidgetCatalog_ChartingDiscoverabilityLinkText() => DashboardsServerResources.Get(nameof (WidgetCatalog_ChartingDiscoverabilityLinkText));

    public static string WidgetCatalog_ChartingDiscoverabilityLinkText(CultureInfo culture) => DashboardsServerResources.Get(nameof (WidgetCatalog_ChartingDiscoverabilityLinkText), culture);

    public static string WidgetCatalog_ChartingDiscoverabilityText() => DashboardsServerResources.Get(nameof (WidgetCatalog_ChartingDiscoverabilityText));

    public static string WidgetCatalog_ChartingDiscoverabilityText(CultureInfo culture) => DashboardsServerResources.Get(nameof (WidgetCatalog_ChartingDiscoverabilityText), culture);

    public static string Widget_Load_Invalid_Promise() => DashboardsServerResources.Get(nameof (Widget_Load_Invalid_Promise));

    public static string Widget_Load_Invalid_Promise(CultureInfo culture) => DashboardsServerResources.Get(nameof (Widget_Load_Invalid_Promise), culture);

    public static string WidgetHost_RenderedDataMethodNotImplemented() => DashboardsServerResources.Get(nameof (WidgetHost_RenderedDataMethodNotImplemented));

    public static string WidgetHost_RenderedDataMethodNotImplemented(CultureInfo culture) => DashboardsServerResources.Get(nameof (WidgetHost_RenderedDataMethodNotImplemented), culture);

    public static string BladeMenu_CustomCallbackWidgetStateRequest_BadSettings() => DashboardsServerResources.Get(nameof (BladeMenu_CustomCallbackWidgetStateRequest_BadSettings));

    public static string BladeMenu_CustomCallbackWidgetStateRequest_BadSettings(CultureInfo culture) => DashboardsServerResources.Get(nameof (BladeMenu_CustomCallbackWidgetStateRequest_BadSettings), culture);
  }
}
