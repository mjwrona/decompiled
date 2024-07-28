// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Analytics.WebPlatform.AnalyticsResources
// Assembly: Microsoft.VisualStudio.Analytics.WebPlatform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DA6B9D87-1232-44CA-8EC9-599418A96267
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Analytics.WebPlatform.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Analytics.WebPlatform
{
  internal static class AnalyticsResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (AnalyticsResources), typeof (AnalyticsResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => AnalyticsResources.s_resMgr;

    private static string Get(string resourceName) => AnalyticsResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? AnalyticsResources.Get(resourceName) : AnalyticsResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) AnalyticsResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? AnalyticsResources.GetInt(resourceName) : (int) AnalyticsResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) AnalyticsResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? AnalyticsResources.GetBool(resourceName) : (bool) AnalyticsResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => AnalyticsResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = AnalyticsResources.Get(resourceName, culture);
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

    public static string FieldCriteriaFilterDescription() => AnalyticsResources.Get(nameof (FieldCriteriaFilterDescription));

    public static string FieldCriteriaFilterDescription(CultureInfo culture) => AnalyticsResources.Get(nameof (FieldCriteriaFilterDescription), culture);

    public static string FieldCriteriaFilterTitle() => AnalyticsResources.Get(nameof (FieldCriteriaFilterTitle));

    public static string FieldCriteriaFilterTitle(CultureInfo culture) => AnalyticsResources.Get(nameof (FieldCriteriaFilterTitle), culture);

    public static string FieldCriteriaFilterAddButtonLabel() => AnalyticsResources.Get(nameof (FieldCriteriaFilterAddButtonLabel));

    public static string FieldCriteriaFilterAddButtonLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (FieldCriteriaFilterAddButtonLabel), culture);

    public static string ProjectTeamPickerAllTeams() => AnalyticsResources.Get(nameof (ProjectTeamPickerAllTeams));

    public static string ProjectTeamPickerAllTeams(CultureInfo culture) => AnalyticsResources.Get(nameof (ProjectTeamPickerAllTeams), culture);

    public static string ProjectTeamPickerTitle() => AnalyticsResources.Get(nameof (ProjectTeamPickerTitle));

    public static string ProjectTeamPickerTitle(CultureInfo culture) => AnalyticsResources.Get(nameof (ProjectTeamPickerTitle), culture);

    public static string ProjectTeamPickerAddButtonLabel() => AnalyticsResources.Get(nameof (ProjectTeamPickerAddButtonLabel));

    public static string ProjectTeamPickerAddButtonLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (ProjectTeamPickerAddButtonLabel), culture);

    public static string NoLongerExists() => AnalyticsResources.Get(nameof (NoLongerExists));

    public static string NoLongerExists(CultureInfo culture) => AnalyticsResources.Get(nameof (NoLongerExists), culture);

    public static string BacklogsLabel() => AnalyticsResources.Get(nameof (BacklogsLabel));

    public static string BacklogsLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (BacklogsLabel), culture);

    public static string WorkItemsLabel() => AnalyticsResources.Get(nameof (WorkItemsLabel));

    public static string WorkItemsLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (WorkItemsLabel), culture);

    public static string ProjectTitle() => AnalyticsResources.Get(nameof (ProjectTitle));

    public static string ProjectTitle(CultureInfo culture) => AnalyticsResources.Get(nameof (ProjectTitle), culture);

    public static string TeamTitle() => AnalyticsResources.Get(nameof (TeamTitle));

    public static string TeamTitle(CultureInfo culture) => AnalyticsResources.Get(nameof (TeamTitle), culture);

    public static string AreaPathTitle() => AnalyticsResources.Get(nameof (AreaPathTitle));

    public static string AreaPathTitle(CultureInfo culture) => AnalyticsResources.Get(nameof (AreaPathTitle), culture);

    public static string OperatorTitle() => AnalyticsResources.Get(nameof (OperatorTitle));

    public static string OperatorTitle(CultureInfo culture) => AnalyticsResources.Get(nameof (OperatorTitle), culture);

    public static string ProjectAreaPickerTitle() => AnalyticsResources.Get(nameof (ProjectAreaPickerTitle));

    public static string ProjectAreaPickerTitle(CultureInfo culture) => AnalyticsResources.Get(nameof (ProjectAreaPickerTitle), culture);

    public static string ProjectAreaPickerAddButtonLabel() => AnalyticsResources.Get(nameof (ProjectAreaPickerAddButtonLabel));

    public static string ProjectAreaPickerAddButtonLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (ProjectAreaPickerAddButtonLabel), culture);

    public static string Operation_Eq() => AnalyticsResources.Get(nameof (Operation_Eq));

    public static string Operation_Eq(CultureInfo culture) => AnalyticsResources.Get(nameof (Operation_Eq), culture);

    public static string Operation_Ne() => AnalyticsResources.Get(nameof (Operation_Ne));

    public static string Operation_Ne(CultureInfo culture) => AnalyticsResources.Get(nameof (Operation_Ne), culture);

    public static string Operation_Ge() => AnalyticsResources.Get(nameof (Operation_Ge));

    public static string Operation_Ge(CultureInfo culture) => AnalyticsResources.Get(nameof (Operation_Ge), culture);

    public static string Operation_Le() => AnalyticsResources.Get(nameof (Operation_Le));

    public static string Operation_Le(CultureInfo culture) => AnalyticsResources.Get(nameof (Operation_Le), culture);

    public static string Operation_Gt() => AnalyticsResources.Get(nameof (Operation_Gt));

    public static string Operation_Gt(CultureInfo culture) => AnalyticsResources.Get(nameof (Operation_Gt), culture);

    public static string Operation_Lt() => AnalyticsResources.Get(nameof (Operation_Lt));

    public static string Operation_Lt(CultureInfo culture) => AnalyticsResources.Get(nameof (Operation_Lt), culture);

    public static string Operation_Contains() => AnalyticsResources.Get(nameof (Operation_Contains));

    public static string Operation_Contains(CultureInfo culture) => AnalyticsResources.Get(nameof (Operation_Contains), culture);

    public static string Operation_Notcontains() => AnalyticsResources.Get(nameof (Operation_Notcontains));

    public static string Operation_Notcontains(CultureInfo culture) => AnalyticsResources.Get(nameof (Operation_Notcontains), culture);

    public static string Operation_Under() => AnalyticsResources.Get(nameof (Operation_Under));

    public static string Operation_Under(CultureInfo culture) => AnalyticsResources.Get(nameof (Operation_Under), culture);

    public static string Operation_Notunder() => AnalyticsResources.Get(nameof (Operation_Notunder));

    public static string Operation_Notunder(CultureInfo culture) => AnalyticsResources.Get(nameof (Operation_Notunder), culture);

    public static string FieldValuesOperator_Or() => AnalyticsResources.Get(nameof (FieldValuesOperator_Or));

    public static string FieldValuesOperator_Or(CultureInfo culture) => AnalyticsResources.Get(nameof (FieldValuesOperator_Or), culture);

    public static string FieldValuesOperator_And() => AnalyticsResources.Get(nameof (FieldValuesOperator_And));

    public static string FieldValuesOperator_And(CultureInfo culture) => AnalyticsResources.Get(nameof (FieldValuesOperator_And), culture);

    public static string Exception_PublicUserOnlyAllowsSingleProjectQuery() => AnalyticsResources.Get(nameof (Exception_PublicUserOnlyAllowsSingleProjectQuery));

    public static string Exception_PublicUserOnlyAllowsSingleProjectQuery(CultureInfo culture) => AnalyticsResources.Get(nameof (Exception_PublicUserOnlyAllowsSingleProjectQuery), culture);

    public static string FieldValueTrueLabel() => AnalyticsResources.Get(nameof (FieldValueTrueLabel));

    public static string FieldValueTrueLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (FieldValueTrueLabel), culture);

    public static string FieldValueFalseLabel() => AnalyticsResources.Get(nameof (FieldValueFalseLabel));

    public static string FieldValueFalseLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (FieldValueFalseLabel), culture);

    public static string FieldCriteriaFilter_RowAriaLabel() => AnalyticsResources.Get(nameof (FieldCriteriaFilter_RowAriaLabel));

    public static string FieldCriteriaFilter_RowAriaLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (FieldCriteriaFilter_RowAriaLabel), culture);

    public static string InfoIconButton_TooltipText() => AnalyticsResources.Get(nameof (InfoIconButton_TooltipText));

    public static string InfoIconButton_TooltipText(CultureInfo culture) => AnalyticsResources.Get(nameof (InfoIconButton_TooltipText), culture);

    public static string ProjectAreaPicker_RowAriaLabel() => AnalyticsResources.Get(nameof (ProjectAreaPicker_RowAriaLabel));

    public static string ProjectAreaPicker_RowAriaLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (ProjectAreaPicker_RowAriaLabel), culture);

    public static string ProjectTeamPicker_RowAriaLabel() => AnalyticsResources.Get(nameof (ProjectTeamPicker_RowAriaLabel));

    public static string ProjectTeamPicker_RowAriaLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (ProjectTeamPicker_RowAriaLabel), culture);

    public static string Backlog_DisplayName(object arg0) => AnalyticsResources.Format(nameof (Backlog_DisplayName), arg0);

    public static string Backlog_DisplayName(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (Backlog_DisplayName), culture, arg0);

    public static string WorkItemTypesPickerTitle() => AnalyticsResources.Get(nameof (WorkItemTypesPickerTitle));

    public static string WorkItemTypesPickerTitle(CultureInfo culture) => AnalyticsResources.Get(nameof (WorkItemTypesPickerTitle), culture);

    public static string WorkItemTypesPickerAddButtonLabel() => AnalyticsResources.Get(nameof (WorkItemTypesPickerAddButtonLabel));

    public static string WorkItemTypesPickerAddButtonLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (WorkItemTypesPickerAddButtonLabel), culture);

    public static string FieldCriteriaComboAriaLabel() => AnalyticsResources.Get(nameof (FieldCriteriaComboAriaLabel));

    public static string FieldCriteriaComboAriaLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (FieldCriteriaComboAriaLabel), culture);

    public static string OperatorComboAriaLabel() => AnalyticsResources.Get(nameof (OperatorComboAriaLabel));

    public static string OperatorComboAriaLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (OperatorComboAriaLabel), culture);

    public static string ValueComboAriaLabel() => AnalyticsResources.Get(nameof (ValueComboAriaLabel));

    public static string ValueComboAriaLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (ValueComboAriaLabel), culture);

    public static string AreaPathDoesntExist() => AnalyticsResources.Get(nameof (AreaPathDoesntExist));

    public static string AreaPathDoesntExist(CultureInfo culture) => AnalyticsResources.Get(nameof (AreaPathDoesntExist), culture);

    public static string BacklogDoesNotExist() => AnalyticsResources.Get(nameof (BacklogDoesNotExist));

    public static string BacklogDoesNotExist(CultureInfo culture) => AnalyticsResources.Get(nameof (BacklogDoesNotExist), culture);

    public static string WorkItemTypeDoesNotExist() => AnalyticsResources.Get(nameof (WorkItemTypeDoesNotExist));

    public static string WorkItemTypeDoesNotExist(CultureInfo culture) => AnalyticsResources.Get(nameof (WorkItemTypeDoesNotExist), culture);

    public static string ProjectMissingErrorMessage() => AnalyticsResources.Get(nameof (ProjectMissingErrorMessage));

    public static string ProjectMissingErrorMessage(CultureInfo culture) => AnalyticsResources.Get(nameof (ProjectMissingErrorMessage), culture);

    public static string TeamMissingErrorMessage() => AnalyticsResources.Get(nameof (TeamMissingErrorMessage));

    public static string TeamMissingErrorMessage(CultureInfo culture) => AnalyticsResources.Get(nameof (TeamMissingErrorMessage), culture);

    public static string FieldCriteriaFieldErrorMessage() => AnalyticsResources.Get(nameof (FieldCriteriaFieldErrorMessage));

    public static string FieldCriteriaFieldErrorMessage(CultureInfo culture) => AnalyticsResources.Get(nameof (FieldCriteriaFieldErrorMessage), culture);

    public static string FieldCriteriaNotAnAllowedValue() => AnalyticsResources.Get(nameof (FieldCriteriaNotAnAllowedValue));

    public static string FieldCriteriaNotAnAllowedValue(CultureInfo culture) => AnalyticsResources.Get(nameof (FieldCriteriaNotAnAllowedValue), culture);

    public static string MustChooseATeamErrorMessage() => AnalyticsResources.Get(nameof (MustChooseATeamErrorMessage));

    public static string MustChooseATeamErrorMessage(CultureInfo culture) => AnalyticsResources.Get(nameof (MustChooseATeamErrorMessage), culture);

    public static string ExpectedAnIntegerValue() => AnalyticsResources.Get(nameof (ExpectedAnIntegerValue));

    public static string ExpectedAnIntegerValue(CultureInfo culture) => AnalyticsResources.Get(nameof (ExpectedAnIntegerValue), culture);

    public static string ExpectedANumberValue() => AnalyticsResources.Get(nameof (ExpectedANumberValue));

    public static string ExpectedANumberValue(CultureInfo culture) => AnalyticsResources.Get(nameof (ExpectedANumberValue), culture);

    public static string ConnectionErrorText() => AnalyticsResources.Get(nameof (ConnectionErrorText));

    public static string ConnectionErrorText(CultureInfo culture) => AnalyticsResources.Get(nameof (ConnectionErrorText), culture);

    public static string AnalyticsTrendsConfig_LoadProjectsFailed() => AnalyticsResources.Get(nameof (AnalyticsTrendsConfig_LoadProjectsFailed));

    public static string AnalyticsTrendsConfig_LoadProjectsFailed(CultureInfo culture) => AnalyticsResources.Get(nameof (AnalyticsTrendsConfig_LoadProjectsFailed), culture);

    public static string AnalyticsTrendsConfig_LoadTeamsFailed() => AnalyticsResources.Get(nameof (AnalyticsTrendsConfig_LoadTeamsFailed));

    public static string AnalyticsTrendsConfig_LoadTeamsFailed(CultureInfo culture) => AnalyticsResources.Get(nameof (AnalyticsTrendsConfig_LoadTeamsFailed), culture);

    public static string AnalyticsTrendsConfig_LoadWorkItemTypesFailed() => AnalyticsResources.Get(nameof (AnalyticsTrendsConfig_LoadWorkItemTypesFailed));

    public static string AnalyticsTrendsConfig_LoadWorkItemTypesFailed(CultureInfo culture) => AnalyticsResources.Get(nameof (AnalyticsTrendsConfig_LoadWorkItemTypesFailed), culture);

    public static string AnalyticsTrendsConfig_LoadBacklogConfigurationsFailed() => AnalyticsResources.Get(nameof (AnalyticsTrendsConfig_LoadBacklogConfigurationsFailed));

    public static string AnalyticsTrendsConfig_LoadBacklogConfigurationsFailed(CultureInfo culture) => AnalyticsResources.Get(nameof (AnalyticsTrendsConfig_LoadBacklogConfigurationsFailed), culture);
  }
}
