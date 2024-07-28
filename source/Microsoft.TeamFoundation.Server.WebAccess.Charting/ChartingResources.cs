// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Charting.ChartingResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Charting, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17E26871-FBED-4ABC-AC4C-33E090B65836
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Charting.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Server.WebAccess.Charting
{
  internal static class ChartingResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ChartingResources), typeof (ChartingResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ChartingResources.s_resMgr;

    private static string Get(string resourceName) => ChartingResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ChartingResources.Get(resourceName) : ChartingResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ChartingResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ChartingResources.GetInt(resourceName) : (int) ChartingResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ChartingResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ChartingResources.GetBool(resourceName) : (bool) ChartingResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ChartingResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ChartingResources.Get(resourceName, culture);
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

    public static string ChartConfiguration_ColorForSeries(object arg0) => ChartingResources.Format(nameof (ChartConfiguration_ColorForSeries), arg0);

    public static string ChartConfiguration_ColorForSeries(object arg0, CultureInfo culture) => ChartingResources.Format(nameof (ChartConfiguration_ColorForSeries), culture, arg0);

    public static string ChartConfiguration_ColorForSingleSeries() => ChartingResources.Get(nameof (ChartConfiguration_ColorForSingleSeries));

    public static string ChartConfiguration_ColorForSingleSeries(CultureInfo culture) => ChartingResources.Get(nameof (ChartConfiguration_ColorForSingleSeries), culture);

    public static string ColorManager_Label() => ChartingResources.Get(nameof (ColorManager_Label));

    public static string ColorManager_Label(CultureInfo culture) => ChartingResources.Get(nameof (ColorManager_Label), culture);

    public static string ColorManager_LabelTooltip() => ChartingResources.Get(nameof (ColorManager_LabelTooltip));

    public static string ColorManager_LabelTooltip(CultureInfo culture) => ChartingResources.Get(nameof (ColorManager_LabelTooltip), culture);

    public static string ColorManager_Control_ClearCustomColorsLabel() => ChartingResources.Get(nameof (ColorManager_Control_ClearCustomColorsLabel));

    public static string ColorManager_Control_ClearCustomColorsLabel(CultureInfo culture) => ChartingResources.Get(nameof (ColorManager_Control_ClearCustomColorsLabel), culture);

    public static string ChartConfigurationWatermark_Columns() => ChartingResources.Get(nameof (ChartConfigurationWatermark_Columns));

    public static string ChartConfigurationWatermark_Columns(CultureInfo culture) => ChartingResources.Get(nameof (ChartConfigurationWatermark_Columns), culture);

    public static string ChartConfigurationWatermark_GroupBy() => ChartingResources.Get(nameof (ChartConfigurationWatermark_GroupBy));

    public static string ChartConfigurationWatermark_GroupBy(CultureInfo culture) => ChartingResources.Get(nameof (ChartConfigurationWatermark_GroupBy), culture);

    public static string ChartConfigurationWatermark_Rows() => ChartingResources.Get(nameof (ChartConfigurationWatermark_Rows));

    public static string ChartConfigurationWatermark_Rows(CultureInfo culture) => ChartingResources.Get(nameof (ChartConfigurationWatermark_Rows), culture);

    public static string ChartConfigurationWatermark_StackBy() => ChartingResources.Get(nameof (ChartConfigurationWatermark_StackBy));

    public static string ChartConfigurationWatermark_StackBy(CultureInfo culture) => ChartingResources.Get(nameof (ChartConfigurationWatermark_StackBy), culture);

    public static string ChartConfigurationWatermark_YAxis() => ChartingResources.Get(nameof (ChartConfigurationWatermark_YAxis));

    public static string ChartConfigurationWatermark_YAxis(CultureInfo culture) => ChartingResources.Get(nameof (ChartConfigurationWatermark_YAxis), culture);

    public static string ChartConfiguration_ChartTypeTooltipFormat(object arg0, object arg1) => ChartingResources.Format(nameof (ChartConfiguration_ChartTypeTooltipFormat), arg0, arg1);

    public static string ChartConfiguration_ChartTypeTooltipFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ChartingResources.Format(nameof (ChartConfiguration_ChartTypeTooltipFormat), culture, arg0, arg1);
    }

    public static string ChartConfiguration_GenericDropdownError() => ChartingResources.Get(nameof (ChartConfiguration_GenericDropdownError));

    public static string ChartConfiguration_GenericDropdownError(CultureInfo culture) => ChartingResources.Get(nameof (ChartConfiguration_GenericDropdownError), culture);

    public static string ChartConfiguration_MoreInfoLabelFormat(object arg0) => ChartingResources.Format(nameof (ChartConfiguration_MoreInfoLabelFormat), arg0);

    public static string ChartConfiguration_MoreInfoLabelFormat(object arg0, CultureInfo culture) => ChartingResources.Format(nameof (ChartConfiguration_MoreInfoLabelFormat), culture, arg0);

    public static string ChartEditor_ChartTypeLabel() => ChartingResources.Get(nameof (ChartEditor_ChartTypeLabel));

    public static string ChartEditor_ChartTypeLabel(CultureInfo culture) => ChartingResources.Get(nameof (ChartEditor_ChartTypeLabel), culture);

    public static string ChartEditor_NameLabel() => ChartingResources.Get(nameof (ChartEditor_NameLabel));

    public static string ChartEditor_NameLabel(CultureInfo culture) => ChartingResources.Get(nameof (ChartEditor_NameLabel), culture);

    public static string Config_ErrorAsterix() => ChartingResources.Get(nameof (Config_ErrorAsterix));

    public static string Config_ErrorAsterix(CultureInfo culture) => ChartingResources.Get(nameof (Config_ErrorAsterix), culture);

    public static string CycleTimePluralDaysFormat(object arg0) => ChartingResources.Format(nameof (CycleTimePluralDaysFormat), arg0);

    public static string CycleTimePluralDaysFormat(object arg0, CultureInfo culture) => ChartingResources.Format(nameof (CycleTimePluralDaysFormat), culture, arg0);

    public static string CycleTimeSingleDay() => ChartingResources.Get(nameof (CycleTimeSingleDay));

    public static string CycleTimeSingleDay(CultureInfo culture) => ChartingResources.Get(nameof (CycleTimeSingleDay), culture);

    public static string EmptyChart_AltTextFormat(object arg0) => ChartingResources.Format(nameof (EmptyChart_AltTextFormat), arg0);

    public static string EmptyChart_AltTextFormat(object arg0, CultureInfo culture) => ChartingResources.Format(nameof (EmptyChart_AltTextFormat), culture, arg0);

    public static string EnterChartTitle() => ChartingResources.Get(nameof (EnterChartTitle));

    public static string EnterChartTitle(CultureInfo culture) => ChartingResources.Get(nameof (EnterChartTitle), culture);

    public static string KanbanTime_CycleTime_emptyAltTextDescription() => ChartingResources.Get(nameof (KanbanTime_CycleTime_emptyAltTextDescription));

    public static string KanbanTime_CycleTime_emptyAltTextDescription(CultureInfo culture) => ChartingResources.Get(nameof (KanbanTime_CycleTime_emptyAltTextDescription), culture);

    public static string KanbanTime_LeadTime_emptyAltTextDescription() => ChartingResources.Get(nameof (KanbanTime_LeadTime_emptyAltTextDescription));

    public static string KanbanTime_LeadTime_emptyAltTextDescription(CultureInfo culture) => ChartingResources.Get(nameof (KanbanTime_LeadTime_emptyAltTextDescription), culture);

    public static string LeadTimePluralDaysFormat(object arg0) => ChartingResources.Format(nameof (LeadTimePluralDaysFormat), arg0);

    public static string LeadTimePluralDaysFormat(object arg0, CultureInfo culture) => ChartingResources.Format(nameof (LeadTimePluralDaysFormat), culture, arg0);

    public static string LeadTimeSingleDay() => ChartingResources.Get(nameof (LeadTimeSingleDay));

    public static string LeadTimeSingleDay(CultureInfo culture) => ChartingResources.Get(nameof (LeadTimeSingleDay), culture);

    public static string MoreItemFormat(object arg0) => ChartingResources.Format(nameof (MoreItemFormat), arg0);

    public static string MoreItemFormat(object arg0, CultureInfo culture) => ChartingResources.Format(nameof (MoreItemFormat), culture, arg0);

    public static string ClearButtonLabel() => ChartingResources.Get(nameof (ClearButtonLabel));

    public static string ClearButtonLabel(CultureInfo culture) => ChartingResources.Get(nameof (ClearButtonLabel), culture);

    public static string ChartConfiguration_TagsLabel() => ChartingResources.Get(nameof (ChartConfiguration_TagsLabel));

    public static string ChartConfiguration_TagsLabel(CultureInfo culture) => ChartingResources.Get(nameof (ChartConfiguration_TagsLabel), culture);

    public static string TagsPickerControlTooltipText() => ChartingResources.Get(nameof (TagsPickerControlTooltipText));

    public static string TagsPickerControlTooltipText(CultureInfo culture) => ChartingResources.Get(nameof (TagsPickerControlTooltipText), culture);

    public static string TagsPickerControlLabel() => ChartingResources.Get(nameof (TagsPickerControlLabel));

    public static string TagsPickerControlLabel(CultureInfo culture) => ChartingResources.Get(nameof (TagsPickerControlLabel), culture);

    public static string ChartConfiguration_AllTagsPlaceholder() => ChartingResources.Get(nameof (ChartConfiguration_AllTagsPlaceholder));

    public static string ChartConfiguration_AllTagsPlaceholder(CultureInfo culture) => ChartingResources.Get(nameof (ChartConfiguration_AllTagsPlaceholder), culture);

    public static string ChartConfiguration_AllTagsRadioButtonText() => ChartingResources.Get(nameof (ChartConfiguration_AllTagsRadioButtonText));

    public static string ChartConfiguration_AllTagsRadioButtonText(CultureInfo culture) => ChartingResources.Get(nameof (ChartConfiguration_AllTagsRadioButtonText), culture);

    public static string ChartConfiguration_SelectedTagsRadioButtonText() => ChartingResources.Get(nameof (ChartConfiguration_SelectedTagsRadioButtonText));

    public static string ChartConfiguration_SelectedTagsRadioButtonText(CultureInfo culture) => ChartingResources.Get(nameof (ChartConfiguration_SelectedTagsRadioButtonText), culture);

    public static string ChartConfiguration_TagsControlSuggestionText() => ChartingResources.Get(nameof (ChartConfiguration_TagsControlSuggestionText));

    public static string ChartConfiguration_TagsControlSuggestionText(CultureInfo culture) => ChartingResources.Get(nameof (ChartConfiguration_TagsControlSuggestionText), culture);

    public static string ChartConfiguration_TagsControlLoadingText() => ChartingResources.Get(nameof (ChartConfiguration_TagsControlLoadingText));

    public static string ChartConfiguration_TagsControlLoadingText(CultureInfo culture) => ChartingResources.Get(nameof (ChartConfiguration_TagsControlLoadingText), culture);
  }
}
