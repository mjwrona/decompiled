// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Dashboards.DashboardsResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Dashboards, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1A53BFE3-D2EE-4259-A1B0-9683B82268B4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Dashboards.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Server.WebAccess.Dashboards
{
  internal static class DashboardsResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (DashboardsResources), typeof (DashboardsResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => DashboardsResources.s_resMgr;

    private static string Get(string resourceName) => DashboardsResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? DashboardsResources.Get(resourceName) : DashboardsResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) DashboardsResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? DashboardsResources.GetInt(resourceName) : (int) DashboardsResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) DashboardsResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? DashboardsResources.GetBool(resourceName) : (bool) DashboardsResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => DashboardsResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = DashboardsResources.Get(resourceName, culture);
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

    public static string AddWidgetButtonText() => DashboardsResources.Get(nameof (AddWidgetButtonText));

    public static string AddWidgetButtonText(CultureInfo culture) => DashboardsResources.Get(nameof (AddWidgetButtonText), culture);

    public static string WidgetEditMenuConfigureButtonTooltipText() => DashboardsResources.Get(nameof (WidgetEditMenuConfigureButtonTooltipText));

    public static string WidgetEditMenuConfigureButtonTooltipText(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetEditMenuConfigureButtonTooltipText), culture);

    public static string HubTitle() => DashboardsResources.Get(nameof (HubTitle));

    public static string HubTitle(CultureInfo culture) => DashboardsResources.Get(nameof (HubTitle), culture);

    public static string ManageDashboardTooltip() => DashboardsResources.Get(nameof (ManageDashboardTooltip));

    public static string ManageDashboardTooltip(CultureInfo culture) => DashboardsResources.Get(nameof (ManageDashboardTooltip), culture);

    public static string LoadingMessage() => DashboardsResources.Get(nameof (LoadingMessage));

    public static string LoadingMessage(CultureInfo culture) => DashboardsResources.Get(nameof (LoadingMessage), culture);

    public static string WidgetEditMenuDeleteButtonTooltipText() => DashboardsResources.Get(nameof (WidgetEditMenuDeleteButtonTooltipText));

    public static string WidgetEditMenuDeleteButtonTooltipText(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetEditMenuDeleteButtonTooltipText), culture);

    public static string ErrorCreateDashboardNameAlreadyExists() => DashboardsResources.Get(nameof (ErrorCreateDashboardNameAlreadyExists));

    public static string ErrorCreateDashboardNameAlreadyExists(CultureInfo culture) => DashboardsResources.Get(nameof (ErrorCreateDashboardNameAlreadyExists), culture);

    public static string ManageDashboardsDialogCancelText() => DashboardsResources.Get(nameof (ManageDashboardsDialogCancelText));

    public static string ManageDashboardsDialogCancelText(CultureInfo culture) => DashboardsResources.Get(nameof (ManageDashboardsDialogCancelText), culture);

    public static string ManageDashboardsDialogDoneText() => DashboardsResources.Get(nameof (ManageDashboardsDialogDoneText));

    public static string ManageDashboardsDialogDoneText(CultureInfo culture) => DashboardsResources.Get(nameof (ManageDashboardsDialogDoneText), culture);

    public static string ManageDashboardsDialogTitleText() => DashboardsResources.Get(nameof (ManageDashboardsDialogTitleText));

    public static string ManageDashboardsDialogTitleText(CultureInfo culture) => DashboardsResources.Get(nameof (ManageDashboardsDialogTitleText), culture);

    public static string ManageDashboardsDialogNewDashboardPlaceholderText() => DashboardsResources.Get(nameof (ManageDashboardsDialogNewDashboardPlaceholderText));

    public static string ManageDashboardsDialogNewDashboardPlaceholderText(CultureInfo culture) => DashboardsResources.Get(nameof (ManageDashboardsDialogNewDashboardPlaceholderText), culture);

    public static string AddWidgetDialogSearchBoxWatermark() => DashboardsResources.Get(nameof (AddWidgetDialogSearchBoxWatermark));

    public static string AddWidgetDialogSearchBoxWatermark(CultureInfo culture) => DashboardsResources.Get(nameof (AddWidgetDialogSearchBoxWatermark), culture);

    public static string WidgetNotificationErrorTitle() => DashboardsResources.Get(nameof (WidgetNotificationErrorTitle));

    public static string WidgetNotificationErrorTitle(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetNotificationErrorTitle), culture);

    public static string ErrorMessage_CreateNewDashboard(object arg0, object arg1, object arg2) => DashboardsResources.Format(nameof (ErrorMessage_CreateNewDashboard), arg0, arg1, arg2);

    public static string ErrorMessage_CreateNewDashboard(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return DashboardsResources.Format(nameof (ErrorMessage_CreateNewDashboard), culture, arg0, arg1, arg2);
    }

    public static string ErrorMessage_DashboardGroup() => DashboardsResources.Get(nameof (ErrorMessage_DashboardGroup));

    public static string ErrorMessage_DashboardGroup(CultureInfo culture) => DashboardsResources.Get(nameof (ErrorMessage_DashboardGroup), culture);

    public static string ErrorMessage_UpdateWidgetPosition() => DashboardsResources.Get(nameof (ErrorMessage_UpdateWidgetPosition));

    public static string ErrorMessage_UpdateWidgetPosition(CultureInfo culture) => DashboardsResources.Get(nameof (ErrorMessage_UpdateWidgetPosition), culture);

    public static string ErrorCreateDashboardTooManyDashboard(object arg0) => DashboardsResources.Format(nameof (ErrorCreateDashboardTooManyDashboard), arg0);

    public static string ErrorCreateDashboardTooManyDashboard(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (ErrorCreateDashboardTooManyDashboard), culture, arg0);

    public static string MenuAddNewDashboard() => DashboardsResources.Get(nameof (MenuAddNewDashboard));

    public static string MenuAddNewDashboard(CultureInfo culture) => DashboardsResources.Get(nameof (MenuAddNewDashboard), culture);

    public static string MenuCancelNewDashboard() => DashboardsResources.Get(nameof (MenuCancelNewDashboard));

    public static string MenuCancelNewDashboard(CultureInfo culture) => DashboardsResources.Get(nameof (MenuCancelNewDashboard), culture);

    public static string MenuDashboardNamePlaceHolder() => DashboardsResources.Get(nameof (MenuDashboardNamePlaceHolder));

    public static string MenuDashboardNamePlaceHolder(CultureInfo culture) => DashboardsResources.Get(nameof (MenuDashboardNamePlaceHolder), culture);

    public static string ErrorMessage_ErrorDetails() => DashboardsResources.Get(nameof (ErrorMessage_ErrorDetails));

    public static string ErrorMessage_ErrorDetails(CultureInfo culture) => DashboardsResources.Get(nameof (ErrorMessage_ErrorDetails), culture);

    public static string ErrorMessage_LoadDashboard() => DashboardsResources.Get(nameof (ErrorMessage_LoadDashboard));

    public static string ErrorMessage_LoadDashboard(CultureInfo culture) => DashboardsResources.Get(nameof (ErrorMessage_LoadDashboard), culture);

    public static string ErrorWidgetCountExceeded(object arg0) => DashboardsResources.Format(nameof (ErrorWidgetCountExceeded), arg0);

    public static string ErrorWidgetCountExceeded(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (ErrorWidgetCountExceeded), culture, arg0);

    public static string MenuOverflowTooltip() => DashboardsResources.Get(nameof (MenuOverflowTooltip));

    public static string MenuOverflowTooltip(CultureInfo culture) => DashboardsResources.Get(nameof (MenuOverflowTooltip), culture);

    public static string ConfigureWidgetOnDashboardTitle() => DashboardsResources.Get(nameof (ConfigureWidgetOnDashboardTitle));

    public static string ConfigureWidgetOnDashboardTitle(CultureInfo culture) => DashboardsResources.Get(nameof (ConfigureWidgetOnDashboardTitle), culture);

    public static string CancelConfiguration() => DashboardsResources.Get(nameof (CancelConfiguration));

    public static string CancelConfiguration(CultureInfo culture) => DashboardsResources.Get(nameof (CancelConfiguration), culture);

    public static string SaveConfiguration() => DashboardsResources.Get(nameof (SaveConfiguration));

    public static string SaveConfiguration(CultureInfo culture) => DashboardsResources.Get(nameof (SaveConfiguration), culture);

    public static string WidgetConfiguration_ContextError() => DashboardsResources.Get(nameof (WidgetConfiguration_ContextError));

    public static string WidgetConfiguration_ContextError(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetConfiguration_ContextError), culture);

    public static string ErrorMessage_Configuration_NameWidget(object arg0, object arg1) => DashboardsResources.Format(nameof (ErrorMessage_Configuration_NameWidget), arg0, arg1);

    public static string ErrorMessage_Configuration_NameWidget(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return DashboardsResources.Format(nameof (ErrorMessage_Configuration_NameWidget), culture, arg0, arg1);
    }

    public static string CloseCatalog() => DashboardsResources.Get(nameof (CloseCatalog));

    public static string CloseCatalog(CultureInfo culture) => DashboardsResources.Get(nameof (CloseCatalog), culture);

    public static string WidgetCatalog_AddButtonTitle() => DashboardsResources.Get(nameof (WidgetCatalog_AddButtonTitle));

    public static string WidgetCatalog_AddButtonTitle(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetCatalog_AddButtonTitle), culture);

    public static string WidgetCatalog_SortAscending() => DashboardsResources.Get(nameof (WidgetCatalog_SortAscending));

    public static string WidgetCatalog_SortAscending(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetCatalog_SortAscending), culture);

    public static string WidgetCatalog_SortByTitle() => DashboardsResources.Get(nameof (WidgetCatalog_SortByTitle));

    public static string WidgetCatalog_SortByTitle(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetCatalog_SortByTitle), culture);

    public static string WidgetCatalog_SortDescending() => DashboardsResources.Get(nameof (WidgetCatalog_SortDescending));

    public static string WidgetCatalog_SortDescending(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetCatalog_SortDescending), culture);

    public static string WidgetNotificationError_MoreDetails() => DashboardsResources.Get(nameof (WidgetNotificationError_MoreDetails));

    public static string WidgetNotificationError_MoreDetails(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetNotificationError_MoreDetails), culture);

    public static string BladeConfigurationWidgetName() => DashboardsResources.Get(nameof (BladeConfigurationWidgetName));

    public static string BladeConfigurationWidgetName(CultureInfo culture) => DashboardsResources.Get(nameof (BladeConfigurationWidgetName), culture);

    public static string BladeConfigurationWidgetSize() => DashboardsResources.Get(nameof (BladeConfigurationWidgetSize));

    public static string BladeConfigurationWidgetSize(CultureInfo culture) => DashboardsResources.Get(nameof (BladeConfigurationWidgetSize), culture);

    public static string BladeConfigurationWidgetSizeLabel(object arg0, object arg1) => DashboardsResources.Format(nameof (BladeConfigurationWidgetSizeLabel), arg0, arg1);

    public static string BladeConfigurationWidgetSizeLabel(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return DashboardsResources.Format(nameof (BladeConfigurationWidgetSizeLabel), culture, arg0, arg1);
    }

    public static string WidgetNotificationDialogClose() => DashboardsResources.Get(nameof (WidgetNotificationDialogClose));

    public static string WidgetNotificationDialogClose(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetNotificationDialogClose), culture);

    public static string WidgetNotificationErrorDialogTitle() => DashboardsResources.Get(nameof (WidgetNotificationErrorDialogTitle));

    public static string WidgetNotificationErrorDialogTitle(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetNotificationErrorDialogTitle), culture);

    public static string WidgetNotificationConfigureDialogMessage() => DashboardsResources.Get(nameof (WidgetNotificationConfigureDialogMessage));

    public static string WidgetNotificationConfigureDialogMessage(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetNotificationConfigureDialogMessage), culture);

    public static string WidgetNotificationConfigureDialogTitle() => DashboardsResources.Get(nameof (WidgetNotificationConfigureDialogTitle));

    public static string WidgetNotificationConfigureDialogTitle(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetNotificationConfigureDialogTitle), culture);

    public static string BladeCatalogTitle() => DashboardsResources.Get(nameof (BladeCatalogTitle));

    public static string BladeCatalogTitle(CultureInfo culture) => DashboardsResources.Get(nameof (BladeCatalogTitle), culture);

    public static string BladeConfigurationTitle() => DashboardsResources.Get(nameof (BladeConfigurationTitle));

    public static string BladeConfigurationTitle(CultureInfo culture) => DashboardsResources.Get(nameof (BladeConfigurationTitle), culture);

    public static string CloseConfiguration() => DashboardsResources.Get(nameof (CloseConfiguration));

    public static string CloseConfiguration(CultureInfo culture) => DashboardsResources.Get(nameof (CloseConfiguration), culture);

    public static string ConfirmCancelDialogText() => DashboardsResources.Get(nameof (ConfirmCancelDialogText));

    public static string ConfirmCancelDialogText(CultureInfo culture) => DashboardsResources.Get(nameof (ConfirmCancelDialogText), culture);

    public static string ConfigHost_NoOnSave() => DashboardsResources.Get(nameof (ConfigHost_NoOnSave));

    public static string ConfigHost_NoOnSave(CultureInfo culture) => DashboardsResources.Get(nameof (ConfigHost_NoOnSave), culture);

    public static string Dashboard_Timer_Initialized() => DashboardsResources.Get(nameof (Dashboard_Timer_Initialized));

    public static string Dashboard_Timer_Initialized(CultureInfo culture) => DashboardsResources.Get(nameof (Dashboard_Timer_Initialized), culture);

    public static string ManageDashboardsDialogAutoRefresh() => DashboardsResources.Get(nameof (ManageDashboardsDialogAutoRefresh));

    public static string ManageDashboardsDialogAutoRefresh(CultureInfo culture) => DashboardsResources.Get(nameof (ManageDashboardsDialogAutoRefresh), culture);

    public static string Dashboard_Timer_Updated_minutes(object arg0) => DashboardsResources.Format(nameof (Dashboard_Timer_Updated_minutes), arg0);

    public static string Dashboard_Timer_Updated_minutes(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (Dashboard_Timer_Updated_minutes), culture, arg0);

    public static string Dashboard_Timer_Updating() => DashboardsResources.Get(nameof (Dashboard_Timer_Updating));

    public static string Dashboard_Timer_Updating(CultureInfo culture) => DashboardsResources.Get(nameof (Dashboard_Timer_Updating), culture);

    public static string Dashboard_Timer_Updated_hours() => DashboardsResources.Get(nameof (Dashboard_Timer_Updated_hours));

    public static string Dashboard_Timer_Updated_hours(CultureInfo culture) => DashboardsResources.Get(nameof (Dashboard_Timer_Updated_hours), culture);

    public static string Dashboard_Timer_Updated_one_minute() => DashboardsResources.Get(nameof (Dashboard_Timer_Updated_one_minute));

    public static string Dashboard_Timer_Updated_one_minute(CultureInfo culture) => DashboardsResources.Get(nameof (Dashboard_Timer_Updated_one_minute), culture);

    public static string AutoRefreshMinutesTooltip(object arg0) => DashboardsResources.Format(nameof (AutoRefreshMinutesTooltip), arg0);

    public static string AutoRefreshMinutesTooltip(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (AutoRefreshMinutesTooltip), culture, arg0);

    public static string AutoRefreshOneMinuteTooltip() => DashboardsResources.Get(nameof (AutoRefreshOneMinuteTooltip));

    public static string AutoRefreshOneMinuteTooltip(CultureInfo culture) => DashboardsResources.Get(nameof (AutoRefreshOneMinuteTooltip), culture);

    public static string AutoRefreshLessThanOneMinuteTooltip() => DashboardsResources.Get(nameof (AutoRefreshLessThanOneMinuteTooltip));

    public static string AutoRefreshLessThanOneMinuteTooltip(CultureInfo culture) => DashboardsResources.Get(nameof (AutoRefreshLessThanOneMinuteTooltip), culture);

    public static string DashboardEditMenuOpenEditMode() => DashboardsResources.Get(nameof (DashboardEditMenuOpenEditMode));

    public static string DashboardEditMenuOpenEditMode(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardEditMenuOpenEditMode), culture);

    public static string DashboardEditMenuCloseEditMode() => DashboardsResources.Get(nameof (DashboardEditMenuCloseEditMode));

    public static string DashboardEditMenuCloseEditMode(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardEditMenuCloseEditMode), culture);

    public static string WidgetConfiguration_NoReload() => DashboardsResources.Get(nameof (WidgetConfiguration_NoReload));

    public static string WidgetConfiguration_NoReload(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetConfiguration_NoReload), culture);

    public static string WidgetHost_MissingContributionData(object arg0) => DashboardsResources.Format(nameof (WidgetHost_MissingContributionData), arg0);

    public static string WidgetHost_MissingContributionData(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (WidgetHost_MissingContributionData), culture, arg0);

    public static string Widget_Noload() => DashboardsResources.Get(nameof (Widget_Noload));

    public static string Widget_Noload(CultureInfo culture) => DashboardsResources.Get(nameof (Widget_Noload), culture);

    public static string Widget_Load_Invalid_WidgetStatus(object arg0) => DashboardsResources.Format(nameof (Widget_Load_Invalid_WidgetStatus), arg0);

    public static string Widget_Load_Invalid_WidgetStatus(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (Widget_Load_Invalid_WidgetStatus), culture, arg0);

    public static string BladeMenu_ServerSaveFail(object arg0) => DashboardsResources.Format(nameof (BladeMenu_ServerSaveFail), arg0);

    public static string BladeMenu_ServerSaveFail(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (BladeMenu_ServerSaveFail), culture, arg0);

    public static string ConfigHost_NoLoadImplemented() => DashboardsResources.Get(nameof (ConfigHost_NoLoadImplemented));

    public static string ConfigHost_NoLoadImplemented(CultureInfo culture) => DashboardsResources.Get(nameof (ConfigHost_NoLoadImplemented), culture);

    public static string ConfigHost_onSaveRejectedPromise() => DashboardsResources.Get(nameof (ConfigHost_onSaveRejectedPromise));

    public static string ConfigHost_onSaveRejectedPromise(CultureInfo culture) => DashboardsResources.Get(nameof (ConfigHost_onSaveRejectedPromise), culture);

    public static string ValidationErrorOnConfigurationSave() => DashboardsResources.Get(nameof (ValidationErrorOnConfigurationSave));

    public static string ValidationErrorOnConfigurationSave(CultureInfo culture) => DashboardsResources.Get(nameof (ValidationErrorOnConfigurationSave), culture);

    public static string ConfigHost_onSaveBadData() => DashboardsResources.Get(nameof (ConfigHost_onSaveBadData));

    public static string ConfigHost_onSaveBadData(CultureInfo culture) => DashboardsResources.Get(nameof (ConfigHost_onSaveBadData), culture);

    public static string ErrorWidgetSettingsVersionDowngrade(object arg0, object arg1) => DashboardsResources.Format(nameof (ErrorWidgetSettingsVersionDowngrade), arg0, arg1);

    public static string ErrorWidgetSettingsVersionDowngrade(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return DashboardsResources.Format(nameof (ErrorWidgetSettingsVersionDowngrade), culture, arg0, arg1);
    }

    public static string ErrorWidgetSettingsVersionInvalid(object arg0) => DashboardsResources.Format(nameof (ErrorWidgetSettingsVersionInvalid), arg0);

    public static string ErrorWidgetSettingsVersionInvalid(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (ErrorWidgetSettingsVersionInvalid), culture, arg0);

    public static string BladeMenu_CustomCallbackWidgetStateRequest_OldRequest() => DashboardsResources.Get(nameof (BladeMenu_CustomCallbackWidgetStateRequest_OldRequest));

    public static string BladeMenu_CustomCallbackWidgetStateRequest_OldRequest(CultureInfo culture) => DashboardsResources.Get(nameof (BladeMenu_CustomCallbackWidgetStateRequest_OldRequest), culture);

    public static string Config_NotifyChangeEvent_BadData(object arg0) => DashboardsResources.Format(nameof (Config_NotifyChangeEvent_BadData), arg0);

    public static string Config_NotifyChangeEvent_BadData(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (Config_NotifyChangeEvent_BadData), culture, arg0);

    public static string ConfigHost_EventNotSupported(object arg0) => DashboardsResources.Format(nameof (ConfigHost_EventNotSupported), arg0);

    public static string ConfigHost_EventNotSupported(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (ConfigHost_EventNotSupported), culture, arg0);

    public static string ExternalContentErrorFormat(object arg0) => DashboardsResources.Format(nameof (ExternalContentErrorFormat), arg0);

    public static string ExternalContentErrorFormat(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (ExternalContentErrorFormat), culture, arg0);

    public static string WidgetCatalog_LearnMore() => DashboardsResources.Get(nameof (WidgetCatalog_LearnMore));

    public static string WidgetCatalog_LearnMore(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetCatalog_LearnMore), culture);

    public static string WidgetCatalog_PublisherTemplate(object arg0) => DashboardsResources.Format(nameof (WidgetCatalog_PublisherTemplate), arg0);

    public static string WidgetCatalog_PublisherTemplate(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (WidgetCatalog_PublisherTemplate), culture, arg0);

    public static string WidgetCatalog_GalleryLink(object arg0) => DashboardsResources.Format(nameof (WidgetCatalog_GalleryLink), arg0);

    public static string WidgetCatalog_GalleryLink(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (WidgetCatalog_GalleryLink), culture, arg0);

    public static string WidgetCatalog_NetworkError() => DashboardsResources.Get(nameof (WidgetCatalog_NetworkError));

    public static string WidgetCatalog_NetworkError(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetCatalog_NetworkError), culture);

    public static string WidgetCatalog_GalleryLinkText() => DashboardsResources.Get(nameof (WidgetCatalog_GalleryLinkText));

    public static string WidgetCatalog_GalleryLinkText(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetCatalog_GalleryLinkText), culture);

    public static string ErrorMessage_UpdateWidgetPositionConflict(object arg0, object arg1) => DashboardsResources.Format(nameof (ErrorMessage_UpdateWidgetPositionConflict), arg0, arg1);

    public static string ErrorMessage_UpdateWidgetPositionConflict(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return DashboardsResources.Format(nameof (ErrorMessage_UpdateWidgetPositionConflict), culture, arg0, arg1);
    }

    public static string ErrorMessage_UpdateWidgetPositionConflict_Refresh() => DashboardsResources.Get(nameof (ErrorMessage_UpdateWidgetPositionConflict_Refresh));

    public static string ErrorMessage_UpdateWidgetPositionConflict_Refresh(CultureInfo culture) => DashboardsResources.Get(nameof (ErrorMessage_UpdateWidgetPositionConflict_Refresh), culture);

    public static string WidgetHost_NotifyBeforePreview() => DashboardsResources.Get(nameof (WidgetHost_NotifyBeforePreview));

    public static string WidgetHost_NotifyBeforePreview(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetHost_NotifyBeforePreview), culture);

    public static string BuildPicker_EmptyWatermark() => DashboardsResources.Get(nameof (BuildPicker_EmptyWatermark));

    public static string BuildPicker_EmptyWatermark(CultureInfo culture) => DashboardsResources.Get(nameof (BuildPicker_EmptyWatermark), culture);

    public static string BuildDefinitions() => DashboardsResources.Get(nameof (BuildDefinitions));

    public static string BuildDefinitions(CultureInfo culture) => DashboardsResources.Get(nameof (BuildDefinitions), culture);

    public static string XamlDefinitions() => DashboardsResources.Get(nameof (XamlDefinitions));

    public static string XamlDefinitions(CultureInfo culture) => DashboardsResources.Get(nameof (XamlDefinitions), culture);

    public static string StakeholderView_DialogHeading() => DashboardsResources.Get(nameof (StakeholderView_DialogHeading));

    public static string StakeholderView_DialogHeading(CultureInfo culture) => DashboardsResources.Get(nameof (StakeholderView_DialogHeading), culture);

    public static string StakeholderView_DialogLicenseTypesLinkText(object arg0) => DashboardsResources.Format(nameof (StakeholderView_DialogLicenseTypesLinkText), arg0);

    public static string StakeholderView_DialogLicenseTypesLinkText(
      object arg0,
      CultureInfo culture)
    {
      return DashboardsResources.Format(nameof (StakeholderView_DialogLicenseTypesLinkText), culture, arg0);
    }

    public static string StakeholderView_DialogMessage() => DashboardsResources.Get(nameof (StakeholderView_DialogMessage));

    public static string StakeholderView_DialogMessage(CultureInfo culture) => DashboardsResources.Get(nameof (StakeholderView_DialogMessage), culture);

    public static string StakeholderView_DialogSubHeading() => DashboardsResources.Get(nameof (StakeholderView_DialogSubHeading));

    public static string StakeholderView_DialogSubHeading(CultureInfo culture) => DashboardsResources.Get(nameof (StakeholderView_DialogSubHeading), culture);

    public static string WidgetNotificationConfigure_ScreenReaderSupportText(object arg0) => DashboardsResources.Format(nameof (WidgetNotificationConfigure_ScreenReaderSupportText), arg0);

    public static string WidgetNotificationConfigure_ScreenReaderSupportText(
      object arg0,
      CultureInfo culture)
    {
      return DashboardsResources.Format(nameof (WidgetNotificationConfigure_ScreenReaderSupportText), culture, arg0);
    }

    public static string WidgetNotificationConfigure_ScreenReaderSupportText_Admin(object arg0) => DashboardsResources.Format(nameof (WidgetNotificationConfigure_ScreenReaderSupportText_Admin), arg0);

    public static string WidgetNotificationConfigure_ScreenReaderSupportText_Admin(
      object arg0,
      CultureInfo culture)
    {
      return DashboardsResources.Format(nameof (WidgetNotificationConfigure_ScreenReaderSupportText_Admin), culture, arg0);
    }

    public static string WidgetNotificationError_ScreenReaderSupportText(object arg0) => DashboardsResources.Format(nameof (WidgetNotificationError_ScreenReaderSupportText), arg0);

    public static string WidgetNotificationError_ScreenReaderSupportText(
      object arg0,
      CultureInfo culture)
    {
      return DashboardsResources.Format(nameof (WidgetNotificationError_ScreenReaderSupportText), culture, arg0);
    }

    public static string WidgetNotificationStakeholder_ScreenReaderSupportText(object arg0) => DashboardsResources.Format(nameof (WidgetNotificationStakeholder_ScreenReaderSupportText), arg0);

    public static string WidgetNotificationStakeholder_ScreenReaderSupportText(
      object arg0,
      CultureInfo culture)
    {
      return DashboardsResources.Format(nameof (WidgetNotificationStakeholder_ScreenReaderSupportText), culture, arg0);
    }

    public static string WidgetError_Timeout() => DashboardsResources.Get(nameof (WidgetError_Timeout));

    public static string WidgetError_Timeout(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetError_Timeout), culture);

    public static string ErrorMessage_UpdateWidgetPositionConflict_AddAction() => DashboardsResources.Get(nameof (ErrorMessage_UpdateWidgetPositionConflict_AddAction));

    public static string ErrorMessage_UpdateWidgetPositionConflict_AddAction(CultureInfo culture) => DashboardsResources.Get(nameof (ErrorMessage_UpdateWidgetPositionConflict_AddAction), culture);

    public static string ErrorMessage_UpdateWidgetPositionConflict_DeleteAction() => DashboardsResources.Get(nameof (ErrorMessage_UpdateWidgetPositionConflict_DeleteAction));

    public static string ErrorMessage_UpdateWidgetPositionConflict_DeleteAction(CultureInfo culture) => DashboardsResources.Get(nameof (ErrorMessage_UpdateWidgetPositionConflict_DeleteAction), culture);

    public static string ErrorMessage_UpdateWidgetPositionConflict_MoveAction() => DashboardsResources.Get(nameof (ErrorMessage_UpdateWidgetPositionConflict_MoveAction));

    public static string ErrorMessage_UpdateWidgetPositionConflict_MoveAction(CultureInfo culture) => DashboardsResources.Get(nameof (ErrorMessage_UpdateWidgetPositionConflict_MoveAction), culture);

    public static string ErrorMessage_UpdateWidgetPositionConflict_UpdateWidgetAction() => DashboardsResources.Get(nameof (ErrorMessage_UpdateWidgetPositionConflict_UpdateWidgetAction));

    public static string ErrorMessage_UpdateWidgetPositionConflict_UpdateWidgetAction(
      CultureInfo culture)
    {
      return DashboardsResources.Get(nameof (ErrorMessage_UpdateWidgetPositionConflict_UpdateWidgetAction), culture);
    }

    public static string InvalidWidget_Message() => DashboardsResources.Get(nameof (InvalidWidget_Message));

    public static string InvalidWidget_Message(CultureInfo culture) => DashboardsResources.Get(nameof (InvalidWidget_Message), culture);

    public static string ExtentionDisabledWidget_DialogContentText() => DashboardsResources.Get(nameof (ExtentionDisabledWidget_DialogContentText));

    public static string ExtentionDisabledWidget_DialogContentText(CultureInfo culture) => DashboardsResources.Get(nameof (ExtentionDisabledWidget_DialogContentText), culture);

    public static string ExtensionDisabledWidget_DialogHeading() => DashboardsResources.Get(nameof (ExtensionDisabledWidget_DialogHeading));

    public static string ExtensionDisabledWidget_DialogHeading(CultureInfo culture) => DashboardsResources.Get(nameof (ExtensionDisabledWidget_DialogHeading), culture);

    public static string DisabledWidget_DialogHeading() => DashboardsResources.Get(nameof (DisabledWidget_DialogHeading));

    public static string DisabledWidget_DialogHeading(CultureInfo culture) => DashboardsResources.Get(nameof (DisabledWidget_DialogHeading), culture);

    public static string DisabledWidget_DialogContentText_DialogSubHeading() => DashboardsResources.Get(nameof (DisabledWidget_DialogContentText_DialogSubHeading));

    public static string DisabledWidget_DialogContentText_DialogSubHeading(CultureInfo culture) => DashboardsResources.Get(nameof (DisabledWidget_DialogContentText_DialogSubHeading), culture);

    public static string DisabledWidget_DialogMessage() => DashboardsResources.Get(nameof (DisabledWidget_DialogMessage));

    public static string DisabledWidget_DialogMessage(CultureInfo culture) => DashboardsResources.Get(nameof (DisabledWidget_DialogMessage), culture);

    public static string DisabledWidgets_DialogLicenseTypesLinkText_VSTS() => DashboardsResources.Get(nameof (DisabledWidgets_DialogLicenseTypesLinkText_VSTS));

    public static string DisabledWidgets_DialogLicenseTypesLinkText_VSTS(CultureInfo culture) => DashboardsResources.Get(nameof (DisabledWidgets_DialogLicenseTypesLinkText_VSTS), culture);

    public static string DisabledWidgets_DialogLicenseTypesLinkText_AzureDevOps() => DashboardsResources.Get(nameof (DisabledWidgets_DialogLicenseTypesLinkText_AzureDevOps));

    public static string DisabledWidgets_DialogLicenseTypesLinkText_AzureDevOps(CultureInfo culture) => DashboardsResources.Get(nameof (DisabledWidgets_DialogLicenseTypesLinkText_AzureDevOps), culture);

    public static string DisabledWidget_ScreenReaderSupportText(object arg0) => DashboardsResources.Format(nameof (DisabledWidget_ScreenReaderSupportText), arg0);

    public static string DisabledWidget_ScreenReaderSupportText(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (DisabledWidget_ScreenReaderSupportText), culture, arg0);

    public static string Dashboards_Title() => DashboardsResources.Get(nameof (Dashboards_Title));

    public static string Dashboards_Title(CultureInfo culture) => DashboardsResources.Get(nameof (Dashboards_Title), culture);

    public static string ErrorMessage_CreateNewDashboard_Details(object arg0) => DashboardsResources.Format(nameof (ErrorMessage_CreateNewDashboard_Details), arg0);

    public static string ErrorMessage_CreateNewDashboard_Details(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (ErrorMessage_CreateNewDashboard_Details), culture, arg0);

    public static string Navigation_NewDashboardActionText() => DashboardsResources.Get(nameof (Navigation_NewDashboardActionText));

    public static string Navigation_NewDashboardActionText(CultureInfo culture) => DashboardsResources.Get(nameof (Navigation_NewDashboardActionText), culture);

    public static string NewDashboardLabelName() => DashboardsResources.Get(nameof (NewDashboardLabelName));

    public static string NewDashboardLabelName(CultureInfo culture) => DashboardsResources.Get(nameof (NewDashboardLabelName), culture);

    public static string ManageDashboardsDialogInsufficientPermissions() => DashboardsResources.Get(nameof (ManageDashboardsDialogInsufficientPermissions));

    public static string ManageDashboardsDialogInsufficientPermissions(CultureInfo culture) => DashboardsResources.Get(nameof (ManageDashboardsDialogInsufficientPermissions), culture);

    public static string Error_DashboardOutOfSync(object arg0) => DashboardsResources.Format(nameof (Error_DashboardOutOfSync), arg0);

    public static string Error_DashboardOutOfSync(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (Error_DashboardOutOfSync), culture, arg0);

    public static string ManageDashboardsDialog_RefreshLink() => DashboardsResources.Get(nameof (ManageDashboardsDialog_RefreshLink));

    public static string ManageDashboardsDialog_RefreshLink(CultureInfo culture) => DashboardsResources.Get(nameof (ManageDashboardsDialog_RefreshLink), culture);

    public static string ManagePermissionsDialogDescriptionEdit() => DashboardsResources.Get(nameof (ManagePermissionsDialogDescriptionEdit));

    public static string ManagePermissionsDialogDescriptionEdit(CultureInfo culture) => DashboardsResources.Get(nameof (ManagePermissionsDialogDescriptionEdit), culture);

    public static string AdminDashboardsSecurityDescription() => DashboardsResources.Get(nameof (AdminDashboardsSecurityDescription));

    public static string AdminDashboardsSecurityDescription(CultureInfo culture) => DashboardsResources.Get(nameof (AdminDashboardsSecurityDescription), culture);

    public static string AdminDashboardsSecuritySubTitle() => DashboardsResources.Get(nameof (AdminDashboardsSecuritySubTitle));

    public static string AdminDashboardsSecuritySubTitle(CultureInfo culture) => DashboardsResources.Get(nameof (AdminDashboardsSecuritySubTitle), culture);

    public static string AdminDashboardsHub() => DashboardsResources.Get(nameof (AdminDashboardsHub));

    public static string AdminDashboardsHub(CultureInfo culture) => DashboardsResources.Get(nameof (AdminDashboardsHub), culture);

    public static string ManagePermissionsDialogDescriptionManage() => DashboardsResources.Get(nameof (ManagePermissionsDialogDescriptionManage));

    public static string ManagePermissionsDialogDescriptionManage(CultureInfo culture) => DashboardsResources.Get(nameof (ManagePermissionsDialogDescriptionManage), culture);

    public static string ManagePermissionsDialogDescriptionNone() => DashboardsResources.Get(nameof (ManagePermissionsDialogDescriptionNone));

    public static string ManagePermissionsDialogDescriptionNone(CultureInfo culture) => DashboardsResources.Get(nameof (ManagePermissionsDialogDescriptionNone), culture);

    public static string ManagePermissionsDialogOptionEdit() => DashboardsResources.Get(nameof (ManagePermissionsDialogOptionEdit));

    public static string ManagePermissionsDialogOptionEdit(CultureInfo culture) => DashboardsResources.Get(nameof (ManagePermissionsDialogOptionEdit), culture);

    public static string ManagePermissionsDialogOptionManage() => DashboardsResources.Get(nameof (ManagePermissionsDialogOptionManage));

    public static string ManagePermissionsDialogOptionManage(CultureInfo culture) => DashboardsResources.Get(nameof (ManagePermissionsDialogOptionManage), culture);

    public static string ManagePermissionsDialogOptionNone() => DashboardsResources.Get(nameof (ManagePermissionsDialogOptionNone));

    public static string ManagePermissionsDialogOptionNone(CultureInfo culture) => DashboardsResources.Get(nameof (ManagePermissionsDialogOptionNone), culture);

    public static string ManagePermissionsLegend() => DashboardsResources.Get(nameof (ManagePermissionsLegend));

    public static string ManagePermissionsLegend(CultureInfo culture) => DashboardsResources.Get(nameof (ManagePermissionsLegend), culture);

    public static string DashboardManager_CreateNewDashboardLabel() => DashboardsResources.Get(nameof (DashboardManager_CreateNewDashboardLabel));

    public static string DashboardManager_CreateNewDashboardLabel(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardManager_CreateNewDashboardLabel), culture);

    public static string DashboardManager_CreateNewDashboard_ButtonText() => DashboardsResources.Get(nameof (DashboardManager_CreateNewDashboard_ButtonText));

    public static string DashboardManager_CreateNewDashboard_ButtonText(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardManager_CreateNewDashboard_ButtonText), culture);

    public static string DashboardManager_List_AutorefreshTitle() => DashboardsResources.Get(nameof (DashboardManager_List_AutorefreshTitle));

    public static string DashboardManager_List_AutorefreshTitle(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardManager_List_AutorefreshTitle), culture);

    public static string DashboardManager_List_NameTitle() => DashboardsResources.Get(nameof (DashboardManager_List_NameTitle));

    public static string DashboardManager_List_NameTitle(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardManager_List_NameTitle), culture);

    public static string DashboardManager_SaveButtonText() => DashboardsResources.Get(nameof (DashboardManager_SaveButtonText));

    public static string DashboardManager_SaveButtonText(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardManager_SaveButtonText), culture);

    public static string ManagePermissionsDialogDescriptionCreate() => DashboardsResources.Get(nameof (ManagePermissionsDialogDescriptionCreate));

    public static string ManagePermissionsDialogDescriptionCreate(CultureInfo culture) => DashboardsResources.Get(nameof (ManagePermissionsDialogDescriptionCreate), culture);

    public static string ManagePermissionsDialogDescriptionDelete() => DashboardsResources.Get(nameof (ManagePermissionsDialogDescriptionDelete));

    public static string ManagePermissionsDialogDescriptionDelete(CultureInfo culture) => DashboardsResources.Get(nameof (ManagePermissionsDialogDescriptionDelete), culture);

    public static string ManagePermissionsDialogOptionCreate() => DashboardsResources.Get(nameof (ManagePermissionsDialogOptionCreate));

    public static string ManagePermissionsDialogOptionCreate(CultureInfo culture) => DashboardsResources.Get(nameof (ManagePermissionsDialogOptionCreate), culture);

    public static string ManagePermissionsDialogOptionDelete() => DashboardsResources.Get(nameof (ManagePermissionsDialogOptionDelete));

    public static string ManagePermissionsDialogOptionDelete(CultureInfo culture) => DashboardsResources.Get(nameof (ManagePermissionsDialogOptionDelete), culture);

    public static string SavingConfiguration() => DashboardsResources.Get(nameof (SavingConfiguration));

    public static string SavingConfiguration(CultureInfo culture) => DashboardsResources.Get(nameof (SavingConfiguration), culture);

    public static string LightboxButton_AriaLabel() => DashboardsResources.Get(nameof (LightboxButton_AriaLabel));

    public static string LightboxButton_AriaLabel(CultureInfo culture) => DashboardsResources.Get(nameof (LightboxButton_AriaLabel), culture);

    public static string CopyToDashboardTitle() => DashboardsResources.Get(nameof (CopyToDashboardTitle));

    public static string CopyToDashboardTitle(CultureInfo culture) => DashboardsResources.Get(nameof (CopyToDashboardTitle), culture);

    public static string AutoRefreshAltText() => DashboardsResources.Get(nameof (AutoRefreshAltText));

    public static string AutoRefreshAltText(CultureInfo culture) => DashboardsResources.Get(nameof (AutoRefreshAltText), culture);

    public static string AutoRefreshAriaLabelFormat(object arg0) => DashboardsResources.Format(nameof (AutoRefreshAriaLabelFormat), arg0);

    public static string AutoRefreshAriaLabelFormat(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (AutoRefreshAriaLabelFormat), culture, arg0);

    public static string Manager_Input_AriaLabel() => DashboardsResources.Get(nameof (Manager_Input_AriaLabel));

    public static string Manager_Input_AriaLabel(CultureInfo culture) => DashboardsResources.Get(nameof (Manager_Input_AriaLabel), culture);

    public static string ErrorMessage_PermissionDenied(object arg0) => DashboardsResources.Format(nameof (ErrorMessage_PermissionDenied), arg0);

    public static string ErrorMessage_PermissionDenied(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (ErrorMessage_PermissionDenied), culture, arg0);

    public static string Refresh_Link() => DashboardsResources.Get(nameof (Refresh_Link));

    public static string Refresh_Link(CultureInfo culture) => DashboardsResources.Get(nameof (Refresh_Link), culture);

    public static string ManagePermissionsLearnMoreFwLink() => DashboardsResources.Get(nameof (ManagePermissionsLearnMoreFwLink));

    public static string ManagePermissionsLearnMoreFwLink(CultureInfo culture) => DashboardsResources.Get(nameof (ManagePermissionsLearnMoreFwLink), culture);

    public static string DashboardManager_DashboardsTabName() => DashboardsResources.Get(nameof (DashboardManager_DashboardsTabName));

    public static string DashboardManager_DashboardsTabName(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardManager_DashboardsTabName), culture);

    public static string DashboardManager_PermissionsTabName() => DashboardsResources.Get(nameof (DashboardManager_PermissionsTabName));

    public static string DashboardManager_PermissionsTabName(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardManager_PermissionsTabName), culture);

    public static string DeleteWidgetOnDashboardTitle() => DashboardsResources.Get(nameof (DeleteWidgetOnDashboardTitle));

    public static string DeleteWidgetOnDashboardTitle(CultureInfo culture) => DashboardsResources.Get(nameof (DeleteWidgetOnDashboardTitle), culture);

    public static string LightboxButtonTooltipText() => DashboardsResources.Get(nameof (LightboxButtonTooltipText));

    public static string LightboxButtonTooltipText(CultureInfo culture) => DashboardsResources.Get(nameof (LightboxButtonTooltipText), culture);

    public static string ManagerRemoveDashboardAriaLabelFormat(object arg0) => DashboardsResources.Format(nameof (ManagerRemoveDashboardAriaLabelFormat), arg0);

    public static string ManagerRemoveDashboardAriaLabelFormat(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (ManagerRemoveDashboardAriaLabelFormat), culture, arg0);

    public static string WidgetCatalog_Instructions() => DashboardsResources.Get(nameof (WidgetCatalog_Instructions));

    public static string WidgetCatalog_Instructions(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetCatalog_Instructions), culture);

    public static string WidgetCatalog_AnnouncePluralSearchResultFormat(object arg0, object arg1) => DashboardsResources.Format(nameof (WidgetCatalog_AnnouncePluralSearchResultFormat), arg0, arg1);

    public static string WidgetCatalog_AnnouncePluralSearchResultFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return DashboardsResources.Format(nameof (WidgetCatalog_AnnouncePluralSearchResultFormat), culture, arg0, arg1);
    }

    public static string WidgetCatalog_AnnounceSingleSearchResultFormat(object arg0) => DashboardsResources.Format(nameof (WidgetCatalog_AnnounceSingleSearchResultFormat), arg0);

    public static string WidgetCatalog_AnnounceSingleSearchResultFormat(
      object arg0,
      CultureInfo culture)
    {
      return DashboardsResources.Format(nameof (WidgetCatalog_AnnounceSingleSearchResultFormat), culture, arg0);
    }

    public static string WidgetEditMenuConfigureButtonAriaLabelFormat(object arg0) => DashboardsResources.Format(nameof (WidgetEditMenuConfigureButtonAriaLabelFormat), arg0);

    public static string WidgetEditMenuConfigureButtonAriaLabelFormat(
      object arg0,
      CultureInfo culture)
    {
      return DashboardsResources.Format(nameof (WidgetEditMenuConfigureButtonAriaLabelFormat), culture, arg0);
    }

    public static string WidgetEditMenuDeleteButtonAriaLabelFormat(object arg0) => DashboardsResources.Format(nameof (WidgetEditMenuDeleteButtonAriaLabelFormat), arg0);

    public static string WidgetEditMenuDeleteButtonAriaLabelFormat(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (WidgetEditMenuDeleteButtonAriaLabelFormat), culture, arg0);

    public static string WidgetCatalog_AnnounceWidgetAddedFormat(
      object arg0,
      object arg1,
      object arg2)
    {
      return DashboardsResources.Format(nameof (WidgetCatalog_AnnounceWidgetAddedFormat), arg0, arg1, arg2);
    }

    public static string WidgetCatalog_AnnounceWidgetAddedFormat(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return DashboardsResources.Format(nameof (WidgetCatalog_AnnounceWidgetAddedFormat), culture, arg0, arg1, arg2);
    }

    public static string Grid_AnnounceWidgetRemovedFormat(object arg0, object arg1, object arg2) => DashboardsResources.Format(nameof (Grid_AnnounceWidgetRemovedFormat), arg0, arg1, arg2);

    public static string Grid_AnnounceWidgetRemovedFormat(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return DashboardsResources.Format(nameof (Grid_AnnounceWidgetRemovedFormat), culture, arg0, arg1, arg2);
    }

    public static string BladeDialogClosedAnnouncement() => DashboardsResources.Get(nameof (BladeDialogClosedAnnouncement));

    public static string BladeDialogClosedAnnouncement(CultureInfo culture) => DashboardsResources.Get(nameof (BladeDialogClosedAnnouncement), culture);

    public static string DashboardNameForPageTitle(object arg0) => DashboardsResources.Format(nameof (DashboardNameForPageTitle), arg0);

    public static string DashboardNameForPageTitle(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (DashboardNameForPageTitle), culture, arg0);

    public static string DashboardInlineEditor_AnnounceSuccessfullyCreatedMessage() => DashboardsResources.Get(nameof (DashboardInlineEditor_AnnounceSuccessfullyCreatedMessage));

    public static string DashboardInlineEditor_AnnounceSuccessfullyCreatedMessage(
      CultureInfo culture)
    {
      return DashboardsResources.Get(nameof (DashboardInlineEditor_AnnounceSuccessfullyCreatedMessage), culture);
    }

    public static string Configuration_MoreInfoLabelFormat(object arg0) => DashboardsResources.Format(nameof (Configuration_MoreInfoLabelFormat), arg0);

    public static string Configuration_MoreInfoLabelFormat(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (Configuration_MoreInfoLabelFormat), culture, arg0);

    public static string Announce_DashboardHasNoWidgets() => DashboardsResources.Get(nameof (Announce_DashboardHasNoWidgets));

    public static string Announce_DashboardHasNoWidgets(CultureInfo culture) => DashboardsResources.Get(nameof (Announce_DashboardHasNoWidgets), culture);

    public static string Announce_LoadingSingleWidget() => DashboardsResources.Get(nameof (Announce_LoadingSingleWidget));

    public static string Announce_LoadingSingleWidget(CultureInfo culture) => DashboardsResources.Get(nameof (Announce_LoadingSingleWidget), culture);

    public static string Announce_LoadingWidgets(object arg0) => DashboardsResources.Format(nameof (Announce_LoadingWidgets), arg0);

    public static string Announce_LoadingWidgets(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (Announce_LoadingWidgets), culture, arg0);

    public static string InvalidWidgetSizeText() => DashboardsResources.Get(nameof (InvalidWidgetSizeText));

    public static string InvalidWidgetSizeText(CultureInfo culture) => DashboardsResources.Get(nameof (InvalidWidgetSizeText), culture);

    public static string DashboardInlineEditor_AnnounceSuccessfullyAddedMessage() => DashboardsResources.Get(nameof (DashboardInlineEditor_AnnounceSuccessfullyAddedMessage));

    public static string DashboardInlineEditor_AnnounceSuccessfullyAddedMessage(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardInlineEditor_AnnounceSuccessfullyAddedMessage), culture);

    public static string MoveWidgetDown() => DashboardsResources.Get(nameof (MoveWidgetDown));

    public static string MoveWidgetDown(CultureInfo culture) => DashboardsResources.Get(nameof (MoveWidgetDown), culture);

    public static string MoveWidgetLeft() => DashboardsResources.Get(nameof (MoveWidgetLeft));

    public static string MoveWidgetLeft(CultureInfo culture) => DashboardsResources.Get(nameof (MoveWidgetLeft), culture);

    public static string MoveWidgetRight() => DashboardsResources.Get(nameof (MoveWidgetRight));

    public static string MoveWidgetRight(CultureInfo culture) => DashboardsResources.Get(nameof (MoveWidgetRight), culture);

    public static string MoveWidgetUp() => DashboardsResources.Get(nameof (MoveWidgetUp));

    public static string MoveWidgetUp(CultureInfo culture) => DashboardsResources.Get(nameof (MoveWidgetUp), culture);

    public static string NewDashboardExperience_AddWidget() => DashboardsResources.Get(nameof (NewDashboardExperience_AddWidget));

    public static string NewDashboardExperience_AddWidget(CultureInfo culture) => DashboardsResources.Get(nameof (NewDashboardExperience_AddWidget), culture);

    public static string NewDashboardExperience_DashboardsBreadcrumb() => DashboardsResources.Get(nameof (NewDashboardExperience_DashboardsBreadcrumb));

    public static string NewDashboardExperience_DashboardsBreadcrumb(CultureInfo culture) => DashboardsResources.Get(nameof (NewDashboardExperience_DashboardsBreadcrumb), culture);

    public static string NewDashboardExperience_DoneEditingDashboard() => DashboardsResources.Get(nameof (NewDashboardExperience_DoneEditingDashboard));

    public static string NewDashboardExperience_DoneEditingDashboard(CultureInfo culture) => DashboardsResources.Get(nameof (NewDashboardExperience_DoneEditingDashboard), culture);

    public static string NewDashboardExperience_EditDashboard() => DashboardsResources.Get(nameof (NewDashboardExperience_EditDashboard));

    public static string NewDashboardExperience_EditDashboard(CultureInfo culture) => DashboardsResources.Get(nameof (NewDashboardExperience_EditDashboard), culture);

    public static string NewDashboardExperience_NewDashboard() => DashboardsResources.Get(nameof (NewDashboardExperience_NewDashboard));

    public static string NewDashboardExperience_NewDashboard(CultureInfo culture) => DashboardsResources.Get(nameof (NewDashboardExperience_NewDashboard), culture);

    public static string NewDashboardsExperience_ManageDashboards_Label() => DashboardsResources.Get(nameof (NewDashboardsExperience_ManageDashboards_Label));

    public static string NewDashboardsExperience_ManageDashboards_Label(CultureInfo culture) => DashboardsResources.Get(nameof (NewDashboardsExperience_ManageDashboards_Label), culture);

    public static string BladeCatalog_NoResultsFound() => DashboardsResources.Get(nameof (BladeCatalog_NoResultsFound));

    public static string BladeCatalog_NoResultsFound(CultureInfo culture) => DashboardsResources.Get(nameof (BladeCatalog_NoResultsFound), culture);

    public static string AllDashboardsHeading() => DashboardsResources.Get(nameof (AllDashboardsHeading));

    public static string AllDashboardsHeading(CultureInfo culture) => DashboardsResources.Get(nameof (AllDashboardsHeading), culture);

    public static string DirectoryView_SearchWatermark() => DashboardsResources.Get(nameof (DirectoryView_SearchWatermark));

    public static string DirectoryView_SearchWatermark(CultureInfo culture) => DashboardsResources.Get(nameof (DirectoryView_SearchWatermark), culture);

    public static string MyDashboardsHeading() => DashboardsResources.Get(nameof (MyDashboardsHeading));

    public static string MyDashboardsHeading(CultureInfo culture) => DashboardsResources.Get(nameof (MyDashboardsHeading), culture);

    public static string DashboardDialogCancelButtonText() => DashboardsResources.Get(nameof (DashboardDialogCancelButtonText));

    public static string DashboardDialogCancelButtonText(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardDialogCancelButtonText), culture);

    public static string NewDashboardDialogOKButtonText() => DashboardsResources.Get(nameof (NewDashboardDialogOKButtonText));

    public static string NewDashboardDialogOKButtonText(CultureInfo culture) => DashboardsResources.Get(nameof (NewDashboardDialogOKButtonText), culture);

    public static string NewDashboardDialogTitle() => DashboardsResources.Get(nameof (NewDashboardDialogTitle));

    public static string NewDashboardDialogTitle(CultureInfo culture) => DashboardsResources.Get(nameof (NewDashboardDialogTitle), culture);

    public static string AutoRefreshSelected(object arg0) => DashboardsResources.Format(nameof (AutoRefreshSelected), arg0);

    public static string AutoRefreshSelected(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (AutoRefreshSelected), culture, arg0);

    public static string DashboardDialogDescriptionField() => DashboardsResources.Get(nameof (DashboardDialogDescriptionField));

    public static string DashboardDialogDescriptionField(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardDialogDescriptionField), culture);

    public static string DashboardDialogDescriptionFieldDescription() => DashboardsResources.Get(nameof (DashboardDialogDescriptionFieldDescription));

    public static string DashboardDialogDescriptionFieldDescription(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardDialogDescriptionFieldDescription), culture);

    public static string DashboardDialogDescriptionFieldErrorMessage() => DashboardsResources.Get(nameof (DashboardDialogDescriptionFieldErrorMessage));

    public static string DashboardDialogDescriptionFieldErrorMessage(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardDialogDescriptionFieldErrorMessage), culture);

    public static string DashboardDialogNameField() => DashboardsResources.Get(nameof (DashboardDialogNameField));

    public static string DashboardDialogNameField(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardDialogNameField), culture);

    public static string DashboardDialogNameFieldDescription() => DashboardsResources.Get(nameof (DashboardDialogNameFieldDescription));

    public static string DashboardDialogNameFieldDescription(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardDialogNameFieldDescription), culture);

    public static string DashboardDialogNameFieldErrorMessage() => DashboardsResources.Get(nameof (DashboardDialogNameFieldErrorMessage));

    public static string DashboardDialogNameFieldErrorMessage(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardDialogNameFieldErrorMessage), culture);

    public static string DirectoryTabs_AllTabKey() => DashboardsResources.Get(nameof (DirectoryTabs_AllTabKey));

    public static string DirectoryTabs_AllTabKey(CultureInfo culture) => DashboardsResources.Get(nameof (DirectoryTabs_AllTabKey), culture);

    public static string DirectoryTabs_MineTabKey() => DashboardsResources.Get(nameof (DirectoryTabs_MineTabKey));

    public static string DirectoryTabs_MineTabKey(CultureInfo culture) => DashboardsResources.Get(nameof (DirectoryTabs_MineTabKey), culture);

    public static string MyDashboards_DescriptionColumn() => DashboardsResources.Get(nameof (MyDashboards_DescriptionColumn));

    public static string MyDashboards_DescriptionColumn(CultureInfo culture) => DashboardsResources.Get(nameof (MyDashboards_DescriptionColumn), culture);

    public static string MyDashboards_NameColumn() => DashboardsResources.Get(nameof (MyDashboards_NameColumn));

    public static string MyDashboards_NameColumn(CultureInfo culture) => DashboardsResources.Get(nameof (MyDashboards_NameColumn), culture);

    public static string MyDashboards_TeamColumn() => DashboardsResources.Get(nameof (MyDashboards_TeamColumn));

    public static string MyDashboards_TeamColumn(CultureInfo culture) => DashboardsResources.Get(nameof (MyDashboards_TeamColumn), culture);

    public static string NoResultsFound() => DashboardsResources.Get(nameof (NoResultsFound));

    public static string NoResultsFound(CultureInfo culture) => DashboardsResources.Get(nameof (NoResultsFound), culture);

    public static string MyEmptyMessage(object arg0) => DashboardsResources.Format(nameof (MyEmptyMessage), arg0);

    public static string MyEmptyMessage(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (MyEmptyMessage), culture, arg0);

    public static string NoDashboardFound() => DashboardsResources.Get(nameof (NoDashboardFound));

    public static string NoDashboardFound(CultureInfo culture) => DashboardsResources.Get(nameof (NoDashboardFound), culture);

    public static string MyFavoritesText() => DashboardsResources.Get(nameof (MyFavoritesText));

    public static string MyFavoritesText(CultureInfo culture) => DashboardsResources.Get(nameof (MyFavoritesText), culture);

    public static string NewDashboardDialogSelectATeam() => DashboardsResources.Get(nameof (NewDashboardDialogSelectATeam));

    public static string NewDashboardDialogSelectATeam(CultureInfo culture) => DashboardsResources.Get(nameof (NewDashboardDialogSelectATeam), culture);

    public static string NewDashboardDialogTeamsLoading() => DashboardsResources.Get(nameof (NewDashboardDialogTeamsLoading));

    public static string NewDashboardDialogTeamsLoading(CultureInfo culture) => DashboardsResources.Get(nameof (NewDashboardDialogTeamsLoading), culture);

    public static string NewDashboardDialogTeamFieldLabel() => DashboardsResources.Get(nameof (NewDashboardDialogTeamFieldLabel));

    public static string NewDashboardDialogTeamFieldLabel(CultureInfo culture) => DashboardsResources.Get(nameof (NewDashboardDialogTeamFieldLabel), culture);

    public static string NewDashboardExperience_BrowseAllText() => DashboardsResources.Get(nameof (NewDashboardExperience_BrowseAllText));

    public static string NewDashboardExperience_BrowseAllText(CultureInfo culture) => DashboardsResources.Get(nameof (NewDashboardExperience_BrowseAllText), culture);

    public static string NewDashboardExperience_RefreshDashboard() => DashboardsResources.Get(nameof (NewDashboardExperience_RefreshDashboard));

    public static string NewDashboardExperience_RefreshDashboard(CultureInfo culture) => DashboardsResources.Get(nameof (NewDashboardExperience_RefreshDashboard), culture);

    public static string PivotDataLoadFailed() => DashboardsResources.Get(nameof (PivotDataLoadFailed));

    public static string PivotDataLoadFailed(CultureInfo culture) => DashboardsResources.Get(nameof (PivotDataLoadFailed), culture);

    public static string DashboardContextMenu_Delete() => DashboardsResources.Get(nameof (DashboardContextMenu_Delete));

    public static string DashboardContextMenu_Delete(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardContextMenu_Delete), culture);

    public static string DashboardContextMenu_Manage() => DashboardsResources.Get(nameof (DashboardContextMenu_Manage));

    public static string DashboardContextMenu_Manage(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardContextMenu_Manage), culture);

    public static string DashboardContextMenu_Security() => DashboardsResources.Get(nameof (DashboardContextMenu_Security));

    public static string DashboardContextMenu_Security(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardContextMenu_Security), culture);

    public static string DeleteDashboardDialogCancelButtonText() => DashboardsResources.Get(nameof (DeleteDashboardDialogCancelButtonText));

    public static string DeleteDashboardDialogCancelButtonText(CultureInfo culture) => DashboardsResources.Get(nameof (DeleteDashboardDialogCancelButtonText), culture);

    public static string DeleteDashboardDialogConfirmationMessage() => DashboardsResources.Get(nameof (DeleteDashboardDialogConfirmationMessage));

    public static string DeleteDashboardDialogConfirmationMessage(CultureInfo culture) => DashboardsResources.Get(nameof (DeleteDashboardDialogConfirmationMessage), culture);

    public static string DeleteDashboardDialogDeleteButtonText() => DashboardsResources.Get(nameof (DeleteDashboardDialogDeleteButtonText));

    public static string DeleteDashboardDialogDeleteButtonText(CultureInfo culture) => DashboardsResources.Get(nameof (DeleteDashboardDialogDeleteButtonText), culture);

    public static string DeleteDashboardDialogTitle() => DashboardsResources.Get(nameof (DeleteDashboardDialogTitle));

    public static string DeleteDashboardDialogTitle(CultureInfo culture) => DashboardsResources.Get(nameof (DeleteDashboardDialogTitle), culture);

    public static string DirectoryViewAllPivotName() => DashboardsResources.Get(nameof (DirectoryViewAllPivotName));

    public static string DirectoryViewAllPivotName(CultureInfo culture) => DashboardsResources.Get(nameof (DirectoryViewAllPivotName), culture);

    public static string DirectoryViewMinePivotName() => DashboardsResources.Get(nameof (DirectoryViewMinePivotName));

    public static string DirectoryViewMinePivotName(CultureInfo culture) => DashboardsResources.Get(nameof (DirectoryViewMinePivotName), culture);

    public static string EditModeBannerButtonText() => DashboardsResources.Get(nameof (EditModeBannerButtonText));

    public static string EditModeBannerButtonText(CultureInfo culture) => DashboardsResources.Get(nameof (EditModeBannerButtonText), culture);

    public static string EditModeBannerText() => DashboardsResources.Get(nameof (EditModeBannerText));

    public static string EditModeBannerText(CultureInfo culture) => DashboardsResources.Get(nameof (EditModeBannerText), culture);

    public static string WidgetLightboxConfigureButtonTooltipText() => DashboardsResources.Get(nameof (WidgetLightboxConfigureButtonTooltipText));

    public static string WidgetLightboxConfigureButtonTooltipText(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetLightboxConfigureButtonTooltipText), culture);

    public static string TeamFilterPlaceholder() => DashboardsResources.Get(nameof (TeamFilterPlaceholder));

    public static string TeamFilterPlaceholder(CultureInfo culture) => DashboardsResources.Get(nameof (TeamFilterPlaceholder), culture);

    public static string TeamFilterNoTeamsFound() => DashboardsResources.Get(nameof (TeamFilterNoTeamsFound));

    public static string TeamFilterNoTeamsFound(CultureInfo culture) => DashboardsResources.Get(nameof (TeamFilterNoTeamsFound), culture);

    public static string NewDashboardExperience_EditDashboardLabel() => DashboardsResources.Get(nameof (NewDashboardExperience_EditDashboardLabel));

    public static string NewDashboardExperience_EditDashboardLabel(CultureInfo culture) => DashboardsResources.Get(nameof (NewDashboardExperience_EditDashboardLabel), culture);

    public static string NewDashboardExperience_RefreshDashboardLabel() => DashboardsResources.Get(nameof (NewDashboardExperience_RefreshDashboardLabel));

    public static string NewDashboardExperience_RefreshDashboardLabel(CultureInfo culture) => DashboardsResources.Get(nameof (NewDashboardExperience_RefreshDashboardLabel), culture);

    public static string DashboardDialogSaveButtonText() => DashboardsResources.Get(nameof (DashboardDialogSaveButtonText));

    public static string DashboardDialogSaveButtonText(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardDialogSaveButtonText), culture);

    public static string ManageDashboardDialogSubtext() => DashboardsResources.Get(nameof (ManageDashboardDialogSubtext));

    public static string ManageDashboardDialogSubtext(CultureInfo culture) => DashboardsResources.Get(nameof (ManageDashboardDialogSubtext), culture);

    public static string ManageDashboardDialogTitle(object arg0) => DashboardsResources.Format(nameof (ManageDashboardDialogTitle), arg0);

    public static string ManageDashboardDialogTitle(object arg0, CultureInfo culture) => DashboardsResources.Format(nameof (ManageDashboardDialogTitle), culture, arg0);

    public static string ManageDashboardDialog_EditDisabled() => DashboardsResources.Get(nameof (ManageDashboardDialog_EditDisabled));

    public static string ManageDashboardDialog_EditDisabled(CultureInfo culture) => DashboardsResources.Get(nameof (ManageDashboardDialog_EditDisabled), culture);

    public static string ManageDashboardDialog_SecurityDialogLink() => DashboardsResources.Get(nameof (ManageDashboardDialog_SecurityDialogLink));

    public static string ManageDashboardDialog_SecurityDialogLink(CultureInfo culture) => DashboardsResources.Get(nameof (ManageDashboardDialog_SecurityDialogLink), culture);

    public static string ManageDashboardDialog_SecuritySectionHeading() => DashboardsResources.Get(nameof (ManageDashboardDialog_SecuritySectionHeading));

    public static string ManageDashboardDialog_SecuritySectionHeading(CultureInfo culture) => DashboardsResources.Get(nameof (ManageDashboardDialog_SecuritySectionHeading), culture);

    public static string ManageDashboardDialog_SecuritySectionLabel() => DashboardsResources.Get(nameof (ManageDashboardDialog_SecuritySectionLabel));

    public static string ManageDashboardDialog_SecuritySectionLabel(CultureInfo culture) => DashboardsResources.Get(nameof (ManageDashboardDialog_SecuritySectionLabel), culture);

    public static string FavoriteNameSeperator() => DashboardsResources.Get(nameof (FavoriteNameSeperator));

    public static string FavoriteNameSeperator(CultureInfo culture) => DashboardsResources.Get(nameof (FavoriteNameSeperator), culture);

    public static string TeamProfileCard_Prefix() => DashboardsResources.Get(nameof (TeamProfileCard_Prefix));

    public static string TeamProfileCard_Prefix(CultureInfo culture) => DashboardsResources.Get(nameof (TeamProfileCard_Prefix), culture);

    public static string DeleteDashboardAccessDeniedError() => DashboardsResources.Get(nameof (DeleteDashboardAccessDeniedError));

    public static string DeleteDashboardAccessDeniedError(CultureInfo culture) => DashboardsResources.Get(nameof (DeleteDashboardAccessDeniedError), culture);

    public static string ManageDashboardsDialogAllOperationsNotPermitted() => DashboardsResources.Get(nameof (ManageDashboardsDialogAllOperationsNotPermitted));

    public static string ManageDashboardsDialogAllOperationsNotPermitted(CultureInfo culture) => DashboardsResources.Get(nameof (ManageDashboardsDialogAllOperationsNotPermitted), culture);

    public static string SearchTextPlaceholder() => DashboardsResources.Get(nameof (SearchTextPlaceholder));

    public static string SearchTextPlaceholder(CultureInfo culture) => DashboardsResources.Get(nameof (SearchTextPlaceholder), culture);

    public static string BrowseAllDashboardsText() => DashboardsResources.Get(nameof (BrowseAllDashboardsText));

    public static string BrowseAllDashboardsText(CultureInfo culture) => DashboardsResources.Get(nameof (BrowseAllDashboardsText), culture);

    public static string LoadingDashboardsText() => DashboardsResources.Get(nameof (LoadingDashboardsText));

    public static string LoadingDashboardsText(CultureInfo culture) => DashboardsResources.Get(nameof (LoadingDashboardsText), culture);

    public static string SearchResultsLoadingText() => DashboardsResources.Get(nameof (SearchResultsLoadingText));

    public static string SearchResultsLoadingText(CultureInfo culture) => DashboardsResources.Get(nameof (SearchResultsLoadingText), culture);

    public static string SearchNoResultsText() => DashboardsResources.Get(nameof (SearchNoResultsText));

    public static string SearchNoResultsText(CultureInfo culture) => DashboardsResources.Get(nameof (SearchNoResultsText), culture);

    public static string ViewContextMenu_Security() => DashboardsResources.Get(nameof (ViewContextMenu_Security));

    public static string ViewContextMenu_Security(CultureInfo culture) => DashboardsResources.Get(nameof (ViewContextMenu_Security), culture);

    public static string DashboardZeroData_PrimaryText() => DashboardsResources.Get(nameof (DashboardZeroData_PrimaryText));

    public static string DashboardZeroData_PrimaryText(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardZeroData_PrimaryText), culture);

    public static string DashboardZeroData_SecondaryText() => DashboardsResources.Get(nameof (DashboardZeroData_SecondaryText));

    public static string DashboardZeroData_SecondaryText(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardZeroData_SecondaryText), culture);

    public static string DashboardsCommandBar_DisabledEditTooltip() => DashboardsResources.Get(nameof (DashboardsCommandBar_DisabledEditTooltip));

    public static string DashboardsCommandBar_DisabledEditTooltip(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardsCommandBar_DisabledEditTooltip), culture);

    public static string DashboardsCommandBar_DisabledConfigureTooltip() => DashboardsResources.Get(nameof (DashboardsCommandBar_DisabledConfigureTooltip));

    public static string DashboardsCommandBar_DisabledConfigureTooltip(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardsCommandBar_DisabledConfigureTooltip), culture);

    public static string WidgetContextMenu_DisabledConfigureTooltip() => DashboardsResources.Get(nameof (WidgetContextMenu_DisabledConfigureTooltip));

    public static string WidgetContextMenu_DisabledConfigureTooltip(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetContextMenu_DisabledConfigureTooltip), culture);

    public static string WidgetContextMenu_DisabledCopyTooltip() => DashboardsResources.Get(nameof (WidgetContextMenu_DisabledCopyTooltip));

    public static string WidgetContextMenu_DisabledCopyTooltip(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetContextMenu_DisabledCopyTooltip), culture);

    public static string WidgetContextMenu_DisabledDeleteTooltip() => DashboardsResources.Get(nameof (WidgetContextMenu_DisabledDeleteTooltip));

    public static string WidgetContextMenu_DisabledDeleteTooltip(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetContextMenu_DisabledDeleteTooltip), culture);

    public static string DashboardDirectoryPicker_DisabledNewDashboardTooltip() => DashboardsResources.Get(nameof (DashboardDirectoryPicker_DisabledNewDashboardTooltip));

    public static string DashboardDirectoryPicker_DisabledNewDashboardTooltip(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardDirectoryPicker_DisabledNewDashboardTooltip), culture);

    public static string DashboardDirectoryContextMenu_DisabledSecurityTooltip() => DashboardsResources.Get(nameof (DashboardDirectoryContextMenu_DisabledSecurityTooltip));

    public static string DashboardDirectoryContextMenu_DisabledSecurityTooltip(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardDirectoryContextMenu_DisabledSecurityTooltip), culture);

    public static string DashboardDirectoryContextMenu_DisabledDeleteTooltip() => DashboardsResources.Get(nameof (DashboardDirectoryContextMenu_DisabledDeleteTooltip));

    public static string DashboardDirectoryContextMenu_DisabledDeleteTooltip(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardDirectoryContextMenu_DisabledDeleteTooltip), culture);

    public static string AllGroupHeader() => DashboardsResources.Get(nameof (AllGroupHeader));

    public static string AllGroupHeader(CultureInfo culture) => DashboardsResources.Get(nameof (AllGroupHeader), culture);

    public static string DefaultGroupHeader() => DashboardsResources.Get(nameof (DefaultGroupHeader));

    public static string DefaultGroupHeader(CultureInfo culture) => DashboardsResources.Get(nameof (DefaultGroupHeader), culture);

    public static string FavoriteGroupHeader() => DashboardsResources.Get(nameof (FavoriteGroupHeader));

    public static string FavoriteGroupHeader(CultureInfo culture) => DashboardsResources.Get(nameof (FavoriteGroupHeader), culture);

    public static string MineGroupHeader() => DashboardsResources.Get(nameof (MineGroupHeader));

    public static string MineGroupHeader(CultureInfo culture) => DashboardsResources.Get(nameof (MineGroupHeader), culture);

    public static string VisualHostMenu_CheckDataQuality() => DashboardsResources.Get(nameof (VisualHostMenu_CheckDataQuality));

    public static string VisualHostMenu_CheckDataQuality(CultureInfo culture) => DashboardsResources.Get(nameof (VisualHostMenu_CheckDataQuality), culture);

    public static string VisualHostMenu_ViewAsTabularData() => DashboardsResources.Get(nameof (VisualHostMenu_ViewAsTabularData));

    public static string VisualHostMenu_ViewAsTabularData(CultureInfo culture) => DashboardsResources.Get(nameof (VisualHostMenu_ViewAsTabularData), culture);

    public static string VisualHostMenu_GetQueryLinks() => DashboardsResources.Get(nameof (VisualHostMenu_GetQueryLinks));

    public static string VisualHostMenu_GetQueryLinks(CultureInfo culture) => DashboardsResources.Get(nameof (VisualHostMenu_GetQueryLinks), culture);

    public static string TabularData_Title() => DashboardsResources.Get(nameof (TabularData_Title));

    public static string TabularData_Title(CultureInfo culture) => DashboardsResources.Get(nameof (TabularData_Title), culture);

    public static string DataQuality_Title() => DashboardsResources.Get(nameof (DataQuality_Title));

    public static string DataQuality_Title(CultureInfo culture) => DashboardsResources.Get(nameof (DataQuality_Title), culture);

    public static string QueryLinks_Title() => DashboardsResources.Get(nameof (QueryLinks_Title));

    public static string QueryLinks_Title(CultureInfo culture) => DashboardsResources.Get(nameof (QueryLinks_Title), culture);

    public static string DataQuality_Entity() => DashboardsResources.Get(nameof (DataQuality_Entity));

    public static string DataQuality_Entity(CultureInfo culture) => DashboardsResources.Get(nameof (DataQuality_Entity), culture);

    public static string DataQuality_Latency() => DashboardsResources.Get(nameof (DataQuality_Latency));

    public static string DataQuality_Latency(CultureInfo culture) => DashboardsResources.Get(nameof (DataQuality_Latency), culture);

    public static string QueryLinks_QueryName() => DashboardsResources.Get(nameof (QueryLinks_QueryName));

    public static string QueryLinks_QueryName(CultureInfo culture) => DashboardsResources.Get(nameof (QueryLinks_QueryName), culture);

    public static string QueryLinks_Description() => DashboardsResources.Get(nameof (QueryLinks_Description));

    public static string QueryLinks_Description(CultureInfo culture) => DashboardsResources.Get(nameof (QueryLinks_Description), culture);

    public static string DashboardZeroData_NoWidgets_PrimaryText() => DashboardsResources.Get(nameof (DashboardZeroData_NoWidgets_PrimaryText));

    public static string DashboardZeroData_NoWidgets_PrimaryText(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardZeroData_NoWidgets_PrimaryText), culture);

    public static string DashboardZeroData_NoWidgets_SecondaryText() => DashboardsResources.Get(nameof (DashboardZeroData_NoWidgets_SecondaryText));

    public static string DashboardZeroData_NoWidgets_SecondaryText(CultureInfo culture) => DashboardsResources.Get(nameof (DashboardZeroData_NoWidgets_SecondaryText), culture);

    public static string WidgetHostAdapter_ContributionLoadError(object arg0, object arg1) => DashboardsResources.Format(nameof (WidgetHostAdapter_ContributionLoadError), arg0, arg1);

    public static string WidgetHostAdapter_ContributionLoadError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return DashboardsResources.Format(nameof (WidgetHostAdapter_ContributionLoadError), culture, arg0, arg1);
    }

    public static string WidgetHostAdapter_PreloadError(object arg0, object arg1) => DashboardsResources.Format(nameof (WidgetHostAdapter_PreloadError), arg0, arg1);

    public static string WidgetHostAdapter_PreloadError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return DashboardsResources.Format(nameof (WidgetHostAdapter_PreloadError), culture, arg0, arg1);
    }

    public static string WidgetHostAdapter_LoadError(object arg0, object arg1) => DashboardsResources.Format(nameof (WidgetHostAdapter_LoadError), arg0, arg1);

    public static string WidgetHostAdapter_LoadError(object arg0, object arg1, CultureInfo culture) => DashboardsResources.Format(nameof (WidgetHostAdapter_LoadError), culture, arg0, arg1);

    public static string WidgetHostAdapter_ReloadError(object arg0, object arg1) => DashboardsResources.Format(nameof (WidgetHostAdapter_ReloadError), arg0, arg1);

    public static string WidgetHostAdapter_ReloadError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return DashboardsResources.Format(nameof (WidgetHostAdapter_ReloadError), culture, arg0, arg1);
    }

    public static string ConfigHostAdapter_ContributionLoadError(object arg0, object arg1) => DashboardsResources.Format(nameof (ConfigHostAdapter_ContributionLoadError), arg0, arg1);

    public static string ConfigHostAdapter_ContributionLoadError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return DashboardsResources.Format(nameof (ConfigHostAdapter_ContributionLoadError), culture, arg0, arg1);
    }

    public static string ConfigHostAdapter_LoadFailure(object arg0, object arg1) => DashboardsResources.Format(nameof (ConfigHostAdapter_LoadFailure), arg0, arg1);

    public static string ConfigHostAdapter_LoadFailure(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return DashboardsResources.Format(nameof (ConfigHostAdapter_LoadFailure), culture, arg0, arg1);
    }

    public static string ConfigHostAdapter_UnsupportedAPI() => DashboardsResources.Get(nameof (ConfigHostAdapter_UnsupportedAPI));

    public static string ConfigHostAdapter_UnsupportedAPI(CultureInfo culture) => DashboardsResources.Get(nameof (ConfigHostAdapter_UnsupportedAPI), culture);

    public static string WidgetNotificationErrorAltText() => DashboardsResources.Get(nameof (WidgetNotificationErrorAltText));

    public static string WidgetNotificationErrorAltText(CultureInfo culture) => DashboardsResources.Get(nameof (WidgetNotificationErrorAltText), culture);

    public static string AnalyticsNotEnabled_DialogHeading() => DashboardsResources.Get(nameof (AnalyticsNotEnabled_DialogHeading));

    public static string AnalyticsNotEnabled_DialogHeading(CultureInfo culture) => DashboardsResources.Get(nameof (AnalyticsNotEnabled_DialogHeading), culture);

    public static string AnalyticsNotEnabled_DialogContentTextFirstPart() => DashboardsResources.Get(nameof (AnalyticsNotEnabled_DialogContentTextFirstPart));

    public static string AnalyticsNotEnabled_DialogContentTextFirstPart(CultureInfo culture) => DashboardsResources.Get(nameof (AnalyticsNotEnabled_DialogContentTextFirstPart), culture);

    public static string AnalyticsNotEnabled_SettingsPageLinkText() => DashboardsResources.Get(nameof (AnalyticsNotEnabled_SettingsPageLinkText));

    public static string AnalyticsNotEnabled_SettingsPageLinkText(CultureInfo culture) => DashboardsResources.Get(nameof (AnalyticsNotEnabled_SettingsPageLinkText), culture);

    public static string AnalyticsNotEnabled_DialogContentTextLastPart() => DashboardsResources.Get(nameof (AnalyticsNotEnabled_DialogContentTextLastPart));

    public static string AnalyticsNotEnabled_DialogContentTextLastPart(CultureInfo culture) => DashboardsResources.Get(nameof (AnalyticsNotEnabled_DialogContentTextLastPart), culture);
  }
}
