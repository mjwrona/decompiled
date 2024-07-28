// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Tfs.WebPlatform.TFSUI.TFSUIResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Tfs.WebPlatform.TFSUI, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDA8DF6C-700E-486B-89AF-56EBC173977C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Tfs.WebPlatform.TFSUI.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Server.WebAccess.Tfs.WebPlatform.TFSUI
{
  internal static class TFSUIResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (TFSUIResources), typeof (TFSUIResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => TFSUIResources.s_resMgr;

    private static string Get(string resourceName) => TFSUIResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? TFSUIResources.Get(resourceName) : TFSUIResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) TFSUIResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? TFSUIResources.GetInt(resourceName) : (int) TFSUIResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) TFSUIResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? TFSUIResources.GetBool(resourceName) : (bool) TFSUIResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => TFSUIResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = TFSUIResources.Get(resourceName, culture);
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

    public static string AddToDashboardPanel_FavoritesGroupHeader() => TFSUIResources.Get(nameof (AddToDashboardPanel_FavoritesGroupHeader));

    public static string AddToDashboardPanel_FavoritesGroupHeader(CultureInfo culture) => TFSUIResources.Get(nameof (AddToDashboardPanel_FavoritesGroupHeader), culture);

    public static string AddToDashboardPanel_AllTeamDashboardsGroupHeader() => TFSUIResources.Get(nameof (AddToDashboardPanel_AllTeamDashboardsGroupHeader));

    public static string AddToDashboardPanel_AllTeamDashboardsGroupHeader(CultureInfo culture) => TFSUIResources.Get(nameof (AddToDashboardPanel_AllTeamDashboardsGroupHeader), culture);

    public static string AddToDashboardPanel_Header() => TFSUIResources.Get(nameof (AddToDashboardPanel_Header));

    public static string AddToDashboardPanel_Header(CultureInfo culture) => TFSUIResources.Get(nameof (AddToDashboardPanel_Header), culture);

    public static string AddToDashboardPanel_SubHeaderPreText() => TFSUIResources.Get(nameof (AddToDashboardPanel_SubHeaderPreText));

    public static string AddToDashboardPanel_SubHeaderPreText(CultureInfo culture) => TFSUIResources.Get(nameof (AddToDashboardPanel_SubHeaderPreText), culture);

    public static string AddToDashboardPanel_SubHeaderPostText() => TFSUIResources.Get(nameof (AddToDashboardPanel_SubHeaderPostText));

    public static string AddToDashboardPanel_SubHeaderPostText(CultureInfo culture) => TFSUIResources.Get(nameof (AddToDashboardPanel_SubHeaderPostText), culture);

    public static string AddToDashboardPanel_OKButtonText() => TFSUIResources.Get(nameof (AddToDashboardPanel_OKButtonText));

    public static string AddToDashboardPanel_OKButtonText(CultureInfo culture) => TFSUIResources.Get(nameof (AddToDashboardPanel_OKButtonText), culture);

    public static string AddToDashboardPanel_CancelButtonText() => TFSUIResources.Get(nameof (AddToDashboardPanel_CancelButtonText));

    public static string AddToDashboardPanel_CancelButtonText(CultureInfo culture) => TFSUIResources.Get(nameof (AddToDashboardPanel_CancelButtonText), culture);

    public static string AddToDashboardPanel_CloseButtonText() => TFSUIResources.Get(nameof (AddToDashboardPanel_CloseButtonText));

    public static string AddToDashboardPanel_CloseButtonText(CultureInfo culture) => TFSUIResources.Get(nameof (AddToDashboardPanel_CloseButtonText), culture);

    public static string AddToDashboardPanel_NoDashboardsFound() => TFSUIResources.Get(nameof (AddToDashboardPanel_NoDashboardsFound));

    public static string AddToDashboardPanel_NoDashboardsFound(CultureInfo culture) => TFSUIResources.Get(nameof (AddToDashboardPanel_NoDashboardsFound), culture);

    public static string AddToDashboard_LoadingText() => TFSUIResources.Get(nameof (AddToDashboard_LoadingText));

    public static string AddToDashboard_LoadingText(CultureInfo culture) => TFSUIResources.Get(nameof (AddToDashboard_LoadingText), culture);

    public static string AddToDashboard_GetDashboardsFailedError() => TFSUIResources.Get(nameof (AddToDashboard_GetDashboardsFailedError));

    public static string AddToDashboard_GetDashboardsFailedError(CultureInfo culture) => TFSUIResources.Get(nameof (AddToDashboard_GetDashboardsFailedError), culture);

    public static string AddToDashboard_SuccessMessage(object arg0, object arg1) => TFSUIResources.Format(nameof (AddToDashboard_SuccessMessage), arg0, arg1);

    public static string AddToDashboard_SuccessMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TFSUIResources.Format(nameof (AddToDashboard_SuccessMessage), culture, arg0, arg1);
    }

    public static string AddToDashboard_FailureMessage(object arg0, object arg1) => TFSUIResources.Format(nameof (AddToDashboard_FailureMessage), arg0, arg1);

    public static string AddToDashboard_FailureMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return TFSUIResources.Format(nameof (AddToDashboard_FailureMessage), culture, arg0, arg1);
    }

    public static string AddToCurrentDashboard_SuccessMessage(object arg0) => TFSUIResources.Format(nameof (AddToCurrentDashboard_SuccessMessage), arg0);

    public static string AddToCurrentDashboard_SuccessMessage(object arg0, CultureInfo culture) => TFSUIResources.Format(nameof (AddToCurrentDashboard_SuccessMessage), culture, arg0);

    public static string AddToCurrentDashboard_FailureMessage(object arg0) => TFSUIResources.Format(nameof (AddToCurrentDashboard_FailureMessage), arg0);

    public static string AddToCurrentDashboard_FailureMessage(object arg0, CultureInfo culture) => TFSUIResources.Format(nameof (AddToCurrentDashboard_FailureMessage), culture, arg0);

    public static string AddToDashboardPanel_SearchTextPlaceholder() => TFSUIResources.Get(nameof (AddToDashboardPanel_SearchTextPlaceholder));

    public static string AddToDashboardPanel_SearchTextPlaceholder(CultureInfo culture) => TFSUIResources.Get(nameof (AddToDashboardPanel_SearchTextPlaceholder), culture);

    public static string AddToDashboardPanel_SearchNoResultsFound() => TFSUIResources.Get(nameof (AddToDashboardPanel_SearchNoResultsFound));

    public static string AddToDashboardPanel_SearchNoResultsFound(CultureInfo culture) => TFSUIResources.Get(nameof (AddToDashboardPanel_SearchNoResultsFound), culture);

    public static string AddToDashboardPanel_AllProjectDashboardsGroupHeader() => TFSUIResources.Get(nameof (AddToDashboardPanel_AllProjectDashboardsGroupHeader));

    public static string AddToDashboardPanel_AllProjectDashboardsGroupHeader(CultureInfo culture) => TFSUIResources.Get(nameof (AddToDashboardPanel_AllProjectDashboardsGroupHeader), culture);
  }
}
