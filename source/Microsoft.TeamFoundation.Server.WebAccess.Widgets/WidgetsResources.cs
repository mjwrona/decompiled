// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Widgets.WidgetsResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Widgets, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DD4C24BB-2646-4C82-B0E8-494FC53AC01D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Widgets.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Server.WebAccess.Widgets
{
  internal static class WidgetsResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (WidgetsResources), typeof (WidgetsResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => WidgetsResources.s_resMgr;

    private static string Get(string resourceName) => WidgetsResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? WidgetsResources.Get(resourceName) : WidgetsResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) WidgetsResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? WidgetsResources.GetInt(resourceName) : (int) WidgetsResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) WidgetsResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? WidgetsResources.GetBool(resourceName) : (bool) WidgetsResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => WidgetsResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = WidgetsResources.Get(resourceName, culture);
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

    public static string BuildChart_ToolTipTextBuildName(object arg0) => WidgetsResources.Format(nameof (BuildChart_ToolTipTextBuildName), arg0);

    public static string BuildChart_ToolTipTextBuildName(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (BuildChart_ToolTipTextBuildName), culture, arg0);

    public static string BuildChart_DurationFormatDays(object arg0) => WidgetsResources.Format(nameof (BuildChart_DurationFormatDays), arg0);

    public static string BuildChart_DurationFormatDays(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (BuildChart_DurationFormatDays), culture, arg0);

    public static string BuildChart_DurationFormatHours(object arg0) => WidgetsResources.Format(nameof (BuildChart_DurationFormatHours), arg0);

    public static string BuildChart_DurationFormatHours(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (BuildChart_DurationFormatHours), culture, arg0);

    public static string BuildChart_DurationFormatMinutes(object arg0) => WidgetsResources.Format(nameof (BuildChart_DurationFormatMinutes), arg0);

    public static string BuildChart_DurationFormatMinutes(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (BuildChart_DurationFormatMinutes), culture, arg0);

    public static string BuildChart_DurationFormatSeconds(object arg0) => WidgetsResources.Format(nameof (BuildChart_DurationFormatSeconds), arg0);

    public static string BuildChart_DurationFormatSeconds(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (BuildChart_DurationFormatSeconds), culture, arg0);

    public static string BuildChart_LoadingBuildsFailed() => WidgetsResources.Get(nameof (BuildChart_LoadingBuildsFailed));

    public static string BuildChart_LoadingBuildsFailed(CultureInfo culture) => WidgetsResources.Get(nameof (BuildChart_LoadingBuildsFailed), culture);

    public static string BuildChart_NoBuildsFound() => WidgetsResources.Get(nameof (BuildChart_NoBuildsFound));

    public static string BuildChart_NoBuildsFound(CultureInfo culture) => WidgetsResources.Get(nameof (BuildChart_NoBuildsFound), culture);

    public static string CodeScalar_FooterText() => WidgetsResources.Get(nameof (CodeScalar_FooterText));

    public static string CodeScalar_FooterText(CultureInfo culture) => WidgetsResources.Get(nameof (CodeScalar_FooterText), culture);

    public static string CodeScalar_FooterText_ChangeSet_SevenDays() => WidgetsResources.Get(nameof (CodeScalar_FooterText_ChangeSet_SevenDays));

    public static string CodeScalar_FooterText_ChangeSet_SevenDays(CultureInfo culture) => WidgetsResources.Get(nameof (CodeScalar_FooterText_ChangeSet_SevenDays), culture);

    public static string CodeScalar_FooterText_Commit_SevenDays() => WidgetsResources.Get(nameof (CodeScalar_FooterText_Commit_SevenDays));

    public static string CodeScalar_FooterText_Commit_SevenDays(CultureInfo culture) => WidgetsResources.Get(nameof (CodeScalar_FooterText_Commit_SevenDays), culture);

    public static string ConfigurationTeamPicker_InitialSelectionNotFound() => WidgetsResources.Get(nameof (ConfigurationTeamPicker_InitialSelectionNotFound));

    public static string ConfigurationTeamPicker_InitialSelectionNotFound(CultureInfo culture) => WidgetsResources.Get(nameof (ConfigurationTeamPicker_InitialSelectionNotFound), culture);

    public static string ConfigurationTeamPicker_InvalidTeamSelected() => WidgetsResources.Get(nameof (ConfigurationTeamPicker_InvalidTeamSelected));

    public static string ConfigurationTeamPicker_InvalidTeamSelected(CultureInfo culture) => WidgetsResources.Get(nameof (ConfigurationTeamPicker_InvalidTeamSelected), culture);

    public static string ConfigurationTeamPicker_Placeholder() => WidgetsResources.Get(nameof (ConfigurationTeamPicker_Placeholder));

    public static string ConfigurationTeamPicker_Placeholder(CultureInfo culture) => WidgetsResources.Get(nameof (ConfigurationTeamPicker_Placeholder), culture);

    public static string HowToLinks_BlurbTfs() => WidgetsResources.Get(nameof (HowToLinks_BlurbTfs));

    public static string HowToLinks_BlurbTfs(CultureInfo culture) => WidgetsResources.Get(nameof (HowToLinks_BlurbTfs), culture);

    public static string HowToLinks_BlurbVso() => WidgetsResources.Get(nameof (HowToLinks_BlurbVso));

    public static string HowToLinks_BlurbVso(CultureInfo culture) => WidgetsResources.Get(nameof (HowToLinks_BlurbVso), culture);

    public static string HowToLinks_BlurbAzureDevOps() => WidgetsResources.Get(nameof (HowToLinks_BlurbAzureDevOps));

    public static string HowToLinks_BlurbAzureDevOps(CultureInfo culture) => WidgetsResources.Get(nameof (HowToLinks_BlurbAzureDevOps), culture);

    public static string HowToLinks_BlurbNonMember() => WidgetsResources.Get(nameof (HowToLinks_BlurbNonMember));

    public static string HowToLinks_BlurbNonMember(CultureInfo culture) => WidgetsResources.Get(nameof (HowToLinks_BlurbNonMember), culture);

    public static string HowToLinks_BuildSubtitle() => WidgetsResources.Get(nameof (HowToLinks_BuildSubtitle));

    public static string HowToLinks_BuildSubtitle(CultureInfo culture) => WidgetsResources.Get(nameof (HowToLinks_BuildSubtitle), culture);

    public static string HowToLinks_BuildTitle() => WidgetsResources.Get(nameof (HowToLinks_BuildTitle));

    public static string HowToLinks_BuildTitle(CultureInfo culture) => WidgetsResources.Get(nameof (HowToLinks_BuildTitle), culture);

    public static string HowToLinks_CodeSubtitle() => WidgetsResources.Get(nameof (HowToLinks_CodeSubtitle));

    public static string HowToLinks_CodeSubtitle(CultureInfo culture) => WidgetsResources.Get(nameof (HowToLinks_CodeSubtitle), culture);

    public static string HowToLinks_CodeTitle() => WidgetsResources.Get(nameof (HowToLinks_CodeTitle));

    public static string HowToLinks_CodeTitle(CultureInfo culture) => WidgetsResources.Get(nameof (HowToLinks_CodeTitle), culture);

    public static string HowToLinks_ChartSubtitle() => WidgetsResources.Get(nameof (HowToLinks_ChartSubtitle));

    public static string HowToLinks_ChartSubtitle(CultureInfo culture) => WidgetsResources.Get(nameof (HowToLinks_ChartSubtitle), culture);

    public static string HowToLinks_ChartTitle() => WidgetsResources.Get(nameof (HowToLinks_ChartTitle));

    public static string HowToLinks_ChartTitle(CultureInfo culture) => WidgetsResources.Get(nameof (HowToLinks_ChartTitle), culture);

    public static string HowToLinks_Title() => WidgetsResources.Get(nameof (HowToLinks_Title));

    public static string HowToLinks_Title(CultureInfo culture) => WidgetsResources.Get(nameof (HowToLinks_Title), culture);

    public static string HowToLinks_WorkSubtitle() => WidgetsResources.Get(nameof (HowToLinks_WorkSubtitle));

    public static string HowToLinks_WorkSubtitle(CultureInfo culture) => WidgetsResources.Get(nameof (HowToLinks_WorkSubtitle), culture);

    public static string HowToLinks_WorkTitle() => WidgetsResources.Get(nameof (HowToLinks_WorkTitle));

    public static string HowToLinks_WorkTitle(CultureInfo culture) => WidgetsResources.Get(nameof (HowToLinks_WorkTitle), culture);

    public static string InvalidConfiguration() => WidgetsResources.Get(nameof (InvalidConfiguration));

    public static string InvalidConfiguration(CultureInfo culture) => WidgetsResources.Get(nameof (InvalidConfiguration), culture);

    public static string InvalidConfigurationReconfigure() => WidgetsResources.Get(nameof (InvalidConfigurationReconfigure));

    public static string InvalidConfigurationReconfigure(CultureInfo culture) => WidgetsResources.Get(nameof (InvalidConfigurationReconfigure), culture);

    public static string Markdown_DefaultMarkdown() => WidgetsResources.Get(nameof (Markdown_DefaultMarkdown));

    public static string Markdown_DefaultMarkdown(CultureInfo culture) => WidgetsResources.Get(nameof (Markdown_DefaultMarkdown), culture);

    public static string Markdown_LearnLinkText() => WidgetsResources.Get(nameof (Markdown_LearnLinkText));

    public static string Markdown_LearnLinkText(CultureInfo culture) => WidgetsResources.Get(nameof (Markdown_LearnLinkText), culture);

    public static string Markdown_LearnLinkUrl() => WidgetsResources.Get(nameof (Markdown_LearnLinkUrl));

    public static string Markdown_LearnLinkUrl(CultureInfo culture) => WidgetsResources.Get(nameof (Markdown_LearnLinkUrl), culture);

    public static string Markdown_LengthValidationMessage() => WidgetsResources.Get(nameof (Markdown_LengthValidationMessage));

    public static string Markdown_LengthValidationMessage(CultureInfo culture) => WidgetsResources.Get(nameof (Markdown_LengthValidationMessage), culture);

    public static string NewWorkItem_CreateButtonText() => WidgetsResources.Get(nameof (NewWorkItem_CreateButtonText));

    public static string NewWorkItem_CreateButtonText(CultureInfo culture) => WidgetsResources.Get(nameof (NewWorkItem_CreateButtonText), culture);

    public static string NewWorkItem_EnterTitle() => WidgetsResources.Get(nameof (NewWorkItem_EnterTitle));

    public static string NewWorkItem_EnterTitle(CultureInfo culture) => WidgetsResources.Get(nameof (NewWorkItem_EnterTitle), culture);

    public static string NewWorkItem_WidgetTitle() => WidgetsResources.Get(nameof (NewWorkItem_WidgetTitle));

    public static string NewWorkItem_WidgetTitle(CultureInfo culture) => WidgetsResources.Get(nameof (NewWorkItem_WidgetTitle), culture);

    public static string OtherLinksWidget_TeamProjectPortal() => WidgetsResources.Get(nameof (OtherLinksWidget_TeamProjectPortal));

    public static string OtherLinksWidget_TeamProjectPortal(CultureInfo culture) => WidgetsResources.Get(nameof (OtherLinksWidget_TeamProjectPortal), culture);

    public static string OtherLinksWidget_Title() => WidgetsResources.Get(nameof (OtherLinksWidget_Title));

    public static string OtherLinksWidget_Title(CultureInfo culture) => WidgetsResources.Get(nameof (OtherLinksWidget_Title), culture);

    public static string OtherLinks_ConfigureAreasTitle() => WidgetsResources.Get(nameof (OtherLinks_ConfigureAreasTitle));

    public static string OtherLinks_ConfigureAreasTitle(CultureInfo culture) => WidgetsResources.Get(nameof (OtherLinks_ConfigureAreasTitle), culture);

    public static string OtherLinks_ConfigureIterationTitle() => WidgetsResources.Get(nameof (OtherLinks_ConfigureIterationTitle));

    public static string OtherLinks_ConfigureIterationTitle(CultureInfo culture) => WidgetsResources.Get(nameof (OtherLinks_ConfigureIterationTitle), culture);

    public static string QueryScalar_BackgroundColorError() => WidgetsResources.Get(nameof (QueryScalar_BackgroundColorError));

    public static string QueryScalar_BackgroundColorError(CultureInfo culture) => WidgetsResources.Get(nameof (QueryScalar_BackgroundColorError), culture);

    public static string QueryScalar_BackgroundColorLabel() => WidgetsResources.Get(nameof (QueryScalar_BackgroundColorLabel));

    public static string QueryScalar_BackgroundColorLabel(CultureInfo culture) => WidgetsResources.Get(nameof (QueryScalar_BackgroundColorLabel), culture);

    public static string QueryScalar_ConditionalFormattingBackgroundColorText() => WidgetsResources.Get(nameof (QueryScalar_ConditionalFormattingBackgroundColorText));

    public static string QueryScalar_ConditionalFormattingBackgroundColorText(CultureInfo culture) => WidgetsResources.Get(nameof (QueryScalar_ConditionalFormattingBackgroundColorText), culture);

    public static string QueryScalar_ConfigNoQuerySelected() => WidgetsResources.Get(nameof (QueryScalar_ConfigNoQuerySelected));

    public static string QueryScalar_ConfigNoQuerySelected(CultureInfo culture) => WidgetsResources.Get(nameof (QueryScalar_ConfigNoQuerySelected), culture);

    public static string QueryScalar_ConfigTeamFieldLabel() => WidgetsResources.Get(nameof (QueryScalar_ConfigTeamFieldLabel));

    public static string QueryScalar_ConfigTeamFieldLabel(CultureInfo culture) => WidgetsResources.Get(nameof (QueryScalar_ConfigTeamFieldLabel), culture);

    public static string QueryScalar_FooterText() => WidgetsResources.Get(nameof (QueryScalar_FooterText));

    public static string QueryScalar_FooterText(CultureInfo culture) => WidgetsResources.Get(nameof (QueryScalar_FooterText), culture);

    public static string QueryScalar_OperatorError() => WidgetsResources.Get(nameof (QueryScalar_OperatorError));

    public static string QueryScalar_OperatorError(CultureInfo culture) => WidgetsResources.Get(nameof (QueryScalar_OperatorError), culture);

    public static string QueryScalar_QueryCaption() => WidgetsResources.Get(nameof (QueryScalar_QueryCaption));

    public static string QueryScalar_QueryCaption(CultureInfo culture) => WidgetsResources.Get(nameof (QueryScalar_QueryCaption), culture);

    public static string QueryScalar_ThresholdError() => WidgetsResources.Get(nameof (QueryScalar_ThresholdError));

    public static string QueryScalar_ThresholdError(CultureInfo culture) => WidgetsResources.Get(nameof (QueryScalar_ThresholdError), culture);

    public static string QueryScalar_Watermark() => WidgetsResources.Get(nameof (QueryScalar_Watermark));

    public static string QueryScalar_Watermark(CultureInfo culture) => WidgetsResources.Get(nameof (QueryScalar_Watermark), culture);

    public static string SprintBurndownWidget_DialogTitle(object arg0) => WidgetsResources.Format(nameof (SprintBurndownWidget_DialogTitle), arg0);

    public static string SprintBurndownWidget_DialogTitle(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (SprintBurndownWidget_DialogTitle), culture, arg0);

    public static string SprintBurndownWidget_NoIterationsSetForTeamMessage() => WidgetsResources.Get(nameof (SprintBurndownWidget_NoIterationsSetForTeamMessage));

    public static string SprintBurndownWidget_NoIterationsSetForTeamMessage(CultureInfo culture) => WidgetsResources.Get(nameof (SprintBurndownWidget_NoIterationsSetForTeamMessage), culture);

    public static string SprintBurndown_GenericTitle() => WidgetsResources.Get(nameof (SprintBurndown_GenericTitle));

    public static string SprintBurndown_GenericTitle(CultureInfo culture) => WidgetsResources.Get(nameof (SprintBurndown_GenericTitle), culture);

    public static string SprintBurndown_SetupIterationsLink() => WidgetsResources.Get(nameof (SprintBurndown_SetupIterationsLink));

    public static string SprintBurndown_SetupIterationsLink(CultureInfo culture) => WidgetsResources.Get(nameof (SprintBurndown_SetupIterationsLink), culture);

    public static string SprintBurndown_SetupIterationsMessage() => WidgetsResources.Get(nameof (SprintBurndown_SetupIterationsMessage));

    public static string SprintBurndown_SetupIterationsMessage(CultureInfo culture) => WidgetsResources.Get(nameof (SprintBurndown_SetupIterationsMessage), culture);

    public static string SprintCapacity_GenericTitleFormat(object arg0) => WidgetsResources.Format(nameof (SprintCapacity_GenericTitleFormat), arg0);

    public static string SprintCapacity_GenericTitleFormat(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (SprintCapacity_GenericTitleFormat), culture, arg0);

    public static string SprintCapacity_NoCapacityMessage() => WidgetsResources.Get(nameof (SprintCapacity_NoCapacityMessage));

    public static string SprintCapacity_NoCapacityMessage(CultureInfo culture) => WidgetsResources.Get(nameof (SprintCapacity_NoCapacityMessage), culture);

    public static string SprintCapacity_ProgressControl_DefaultProgressText(
      object arg0,
      object arg1)
    {
      return WidgetsResources.Format(nameof (SprintCapacity_ProgressControl_DefaultProgressText), arg0, arg1);
    }

    public static string SprintCapacity_ProgressControl_DefaultProgressText(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (SprintCapacity_ProgressControl_DefaultProgressText), culture, arg0, arg1);
    }

    public static string SprintCapacity_ProgressControl_NoCurrentValue(object arg0) => WidgetsResources.Format(nameof (SprintCapacity_ProgressControl_NoCurrentValue), arg0);

    public static string SprintCapacity_ProgressControl_NoCurrentValue(
      object arg0,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (SprintCapacity_ProgressControl_NoCurrentValue), culture, arg0);
    }

    public static string SprintCapacity_ProgressControl_SummaryProgressText(object arg0) => WidgetsResources.Format(nameof (SprintCapacity_ProgressControl_SummaryProgressText), arg0);

    public static string SprintCapacity_ProgressControl_SummaryProgressText(
      object arg0,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (SprintCapacity_ProgressControl_SummaryProgressText), culture, arg0);
    }

    public static string SprintCapacity_ProgressControl_SummaryProgressToolTip(
      object arg0,
      object arg1)
    {
      return WidgetsResources.Format(nameof (SprintCapacity_ProgressControl_SummaryProgressToolTip), arg0, arg1);
    }

    public static string SprintCapacity_ProgressControl_SummaryProgressToolTip(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (SprintCapacity_ProgressControl_SummaryProgressToolTip), culture, arg0, arg1);
    }

    public static string SprintCapacity_ScheduleWorkLink() => WidgetsResources.Get(nameof (SprintCapacity_ScheduleWorkLink));

    public static string SprintCapacity_ScheduleWorkLink(CultureInfo culture) => WidgetsResources.Get(nameof (SprintCapacity_ScheduleWorkLink), culture);

    public static string SprintCapacity_SetupIterationLink() => WidgetsResources.Get(nameof (SprintCapacity_SetupIterationLink));

    public static string SprintCapacity_SetupIterationLink(CultureInfo culture) => WidgetsResources.Get(nameof (SprintCapacity_SetupIterationLink), culture);

    public static string SprintCapacity_SetupIterationMessage() => WidgetsResources.Get(nameof (SprintCapacity_SetupIterationMessage));

    public static string SprintCapacity_SetupIterationMessage(CultureInfo culture) => WidgetsResources.Get(nameof (SprintCapacity_SetupIterationMessage), culture);

    public static string SprintCapacity_StoriesInProgress(object arg0) => WidgetsResources.Format(nameof (SprintCapacity_StoriesInProgress), arg0);

    public static string SprintCapacity_StoriesInProgress(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (SprintCapacity_StoriesInProgress), culture, arg0);

    public static string SprintCapacity_StoriesNotStarted(object arg0) => WidgetsResources.Format(nameof (SprintCapacity_StoriesNotStarted), arg0);

    public static string SprintCapacity_StoriesNotStarted(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (SprintCapacity_StoriesNotStarted), culture, arg0);

    public static string SprintCapacity_StoriesWithOneStatusLabel(object arg0, object arg1) => WidgetsResources.Format(nameof (SprintCapacity_StoriesWithOneStatusLabel), arg0, arg1);

    public static string SprintCapacity_StoriesWithOneStatusLabel(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (SprintCapacity_StoriesWithOneStatusLabel), culture, arg0, arg1);
    }

    public static string SprintCapacity_StoriesWithTwoStatusesLabel(
      object arg0,
      object arg1,
      object arg2)
    {
      return WidgetsResources.Format(nameof (SprintCapacity_StoriesWithTwoStatusesLabel), arg0, arg1, arg2);
    }

    public static string SprintCapacity_StoriesWithTwoStatusesLabel(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (SprintCapacity_StoriesWithTwoStatusesLabel), culture, arg0, arg1, arg2);
    }

    public static string SprintOverview_BugsVRequirements() => WidgetsResources.Get(nameof (SprintOverview_BugsVRequirements));

    public static string SprintOverview_BugsVRequirements(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverview_BugsVRequirements), culture);

    public static string SprintOverview_GenericTitle() => WidgetsResources.Get(nameof (SprintOverview_GenericTitle));

    public static string SprintOverview_GenericTitle(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverview_GenericTitle), culture);

    public static string SprintOverview_NA() => WidgetsResources.Get(nameof (SprintOverview_NA));

    public static string SprintOverview_NA(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverview_NA), culture);

    public static string SprintOverview_SetupIterationsLink() => WidgetsResources.Get(nameof (SprintOverview_SetupIterationsLink));

    public static string SprintOverview_SetupIterationsLink(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverview_SetupIterationsLink), culture);

    public static string SprintOverview_SetupIterationsMessage() => WidgetsResources.Get(nameof (SprintOverview_SetupIterationsMessage));

    public static string SprintOverview_SetupIterationsMessage(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverview_SetupIterationsMessage), culture);

    public static string SprintOverview_TotalStoryCountPctCompleted(object arg0) => WidgetsResources.Format(nameof (SprintOverview_TotalStoryCountPctCompleted), arg0);

    public static string SprintOverview_TotalStoryCountPctCompleted(
      object arg0,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (SprintOverview_TotalStoryCountPctCompleted), culture, arg0);
    }

    public static string SprintOverview_WorkDayRemaining() => WidgetsResources.Get(nameof (SprintOverview_WorkDayRemaining));

    public static string SprintOverview_WorkDayRemaining(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverview_WorkDayRemaining), culture);

    public static string SprintOverview_WorkDaysRemaining() => WidgetsResources.Get(nameof (SprintOverview_WorkDaysRemaining));

    public static string SprintOverview_WorkDaysRemaining(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverview_WorkDaysRemaining), culture);

    public static string TeamMembersWidget_ErrorLoadingMembers() => WidgetsResources.Get(nameof (TeamMembersWidget_ErrorLoadingMembers));

    public static string TeamMembersWidget_ErrorLoadingMembers(CultureInfo culture) => WidgetsResources.Get(nameof (TeamMembersWidget_ErrorLoadingMembers), culture);

    public static string TeamMembersWidget_ErrorLoadingTeamId() => WidgetsResources.Get(nameof (TeamMembersWidget_ErrorLoadingTeamId));

    public static string TeamMembersWidget_ErrorLoadingTeamId(CultureInfo culture) => WidgetsResources.Get(nameof (TeamMembersWidget_ErrorLoadingTeamId), culture);

    public static string TeamMembersWidget_ShowingFormat(object arg0, object arg1) => WidgetsResources.Format(nameof (TeamMembersWidget_ShowingFormat), arg0, arg1);

    public static string TeamMembersWidget_ShowingFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (TeamMembersWidget_ShowingFormat), culture, arg0, arg1);
    }

    public static string TeamMembersWidget_Title() => WidgetsResources.Get(nameof (TeamMembersWidget_Title));

    public static string TeamMembersWidget_Title(CultureInfo culture) => WidgetsResources.Get(nameof (TeamMembersWidget_Title), culture);

    public static string TeamRoomWidget_Title() => WidgetsResources.Get(nameof (TeamRoomWidget_Title));

    public static string TeamRoomWidget_Title(CultureInfo culture) => WidgetsResources.Get(nameof (TeamRoomWidget_Title), culture);

    public static string TeamRoomWidget_UsersInRoomFormat(object arg0) => WidgetsResources.Format(nameof (TeamRoomWidget_UsersInRoomFormat), arg0);

    public static string TeamRoomWidget_UsersInRoomFormat(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (TeamRoomWidget_UsersInRoomFormat), culture, arg0);

    public static string VSLinksWidget_Title() => WidgetsResources.Get(nameof (VSLinksWidget_Title));

    public static string VSLinksWidget_Title(CultureInfo culture) => WidgetsResources.Get(nameof (VSLinksWidget_Title), culture);

    public static string VSLinksWidget_VSDownloadLink() => WidgetsResources.Get(nameof (VSLinksWidget_VSDownloadLink));

    public static string VSLinksWidget_VSDownloadLink(CultureInfo culture) => WidgetsResources.Get(nameof (VSLinksWidget_VSDownloadLink), culture);

    public static string VSLinksWidget_VSDownloadLink_SubTitle() => WidgetsResources.Get(nameof (VSLinksWidget_VSDownloadLink_SubTitle));

    public static string VSLinksWidget_VSDownloadLink_SubTitle(CultureInfo culture) => WidgetsResources.Get(nameof (VSLinksWidget_VSDownloadLink_SubTitle), culture);

    public static string VSLinksWidget_VSDownloadLink_Title() => WidgetsResources.Get(nameof (VSLinksWidget_VSDownloadLink_Title));

    public static string VSLinksWidget_VSDownloadLink_Title(CultureInfo culture) => WidgetsResources.Get(nameof (VSLinksWidget_VSDownloadLink_Title), culture);

    public static string VSLinksWidget_VSOpenLink_SubTitle() => WidgetsResources.Get(nameof (VSLinksWidget_VSOpenLink_SubTitle));

    public static string VSLinksWidget_VSOpenLink_SubTitle(CultureInfo culture) => WidgetsResources.Get(nameof (VSLinksWidget_VSOpenLink_SubTitle), culture);

    public static string VSLinksWidget_VSOpenLink_Title() => WidgetsResources.Get(nameof (VSLinksWidget_VSOpenLink_Title));

    public static string VSLinksWidget_VSOpenLink_Title(CultureInfo culture) => WidgetsResources.Get(nameof (VSLinksWidget_VSOpenLink_Title), culture);

    public static string VSLinksWidget_VSOpenLink_Tooltip() => WidgetsResources.Get(nameof (VSLinksWidget_VSOpenLink_Tooltip));

    public static string VSLinksWidget_VSOpenLink_Tooltip(CultureInfo culture) => WidgetsResources.Get(nameof (VSLinksWidget_VSOpenLink_Tooltip), culture);

    public static string Widget_InternalError() => WidgetsResources.Get(nameof (Widget_InternalError));

    public static string Widget_InternalError(CultureInfo culture) => WidgetsResources.Get(nameof (Widget_InternalError), culture);

    public static string WorkLinks_TeamWorkBacklogTitle() => WidgetsResources.Get(nameof (WorkLinks_TeamWorkBacklogTitle));

    public static string WorkLinks_TeamWorkBacklogTitle(CultureInfo culture) => WidgetsResources.Get(nameof (WorkLinks_TeamWorkBacklogTitle), culture);

    public static string WorkLinks_TeamWorkBoardTitle() => WidgetsResources.Get(nameof (WorkLinks_TeamWorkBoardTitle));

    public static string WorkLinks_TeamWorkBoardTitle(CultureInfo culture) => WidgetsResources.Get(nameof (WorkLinks_TeamWorkBoardTitle), culture);

    public static string WorkLinks_TeamWorkQueriesTitle() => WidgetsResources.Get(nameof (WorkLinks_TeamWorkQueriesTitle));

    public static string WorkLinks_TeamWorkQueriesTitle(CultureInfo culture) => WidgetsResources.Get(nameof (WorkLinks_TeamWorkQueriesTitle), culture);

    public static string WorkLinks_TeamWorkTaskboardTitle() => WidgetsResources.Get(nameof (WorkLinks_TeamWorkTaskboardTitle));

    public static string WorkLinks_TeamWorkTaskboardTitle(CultureInfo culture) => WidgetsResources.Get(nameof (WorkLinks_TeamWorkTaskboardTitle), culture);

    public static string WorkLinks_TeamWorkTitle() => WidgetsResources.Get(nameof (WorkLinks_TeamWorkTitle));

    public static string WorkLinks_TeamWorkTitle(CultureInfo culture) => WidgetsResources.Get(nameof (WorkLinks_TeamWorkTitle), culture);

    public static string CodeScalar_ConfigurationBranchLabel() => WidgetsResources.Get(nameof (CodeScalar_ConfigurationBranchLabel));

    public static string CodeScalar_ConfigurationBranchLabel(CultureInfo culture) => WidgetsResources.Get(nameof (CodeScalar_ConfigurationBranchLabel), culture);

    public static string CodeScalar_ConfigurationNoBranchSelected() => WidgetsResources.Get(nameof (CodeScalar_ConfigurationNoBranchSelected));

    public static string CodeScalar_ConfigurationNoBranchSelected(CultureInfo culture) => WidgetsResources.Get(nameof (CodeScalar_ConfigurationNoBranchSelected), culture);

    public static string CodeScalar_ConfigurationNoPathSelected() => WidgetsResources.Get(nameof (CodeScalar_ConfigurationNoPathSelected));

    public static string CodeScalar_ConfigurationNoPathSelected(CultureInfo culture) => WidgetsResources.Get(nameof (CodeScalar_ConfigurationNoPathSelected), culture);

    public static string CodeScalar_ConfigurationNoRepositorySelected() => WidgetsResources.Get(nameof (CodeScalar_ConfigurationNoRepositorySelected));

    public static string CodeScalar_ConfigurationNoRepositorySelected(CultureInfo culture) => WidgetsResources.Get(nameof (CodeScalar_ConfigurationNoRepositorySelected), culture);

    public static string CodeScalar_ConfigurationPathLabel() => WidgetsResources.Get(nameof (CodeScalar_ConfigurationPathLabel));

    public static string CodeScalar_ConfigurationPathLabel(CultureInfo culture) => WidgetsResources.Get(nameof (CodeScalar_ConfigurationPathLabel), culture);

    public static string CodeScalar_ConfigurationRepositoryLabel() => WidgetsResources.Get(nameof (CodeScalar_ConfigurationRepositoryLabel));

    public static string CodeScalar_ConfigurationRepositoryLabel(CultureInfo culture) => WidgetsResources.Get(nameof (CodeScalar_ConfigurationRepositoryLabel), culture);

    public static string CodeScalar_ConfigurationSelectAPath() => WidgetsResources.Get(nameof (CodeScalar_ConfigurationSelectAPath));

    public static string CodeScalar_ConfigurationSelectAPath(CultureInfo culture) => WidgetsResources.Get(nameof (CodeScalar_ConfigurationSelectAPath), culture);

    public static string CodeScalar_ConfigurationSelectRepositoryAndBranch() => WidgetsResources.Get(nameof (CodeScalar_ConfigurationSelectRepositoryAndBranch));

    public static string CodeScalar_ConfigurationSelectRepositoryAndBranch(CultureInfo culture) => WidgetsResources.Get(nameof (CodeScalar_ConfigurationSelectRepositoryAndBranch), culture);

    public static string CodeScalar_DefaultWidgetName() => WidgetsResources.Get(nameof (CodeScalar_DefaultWidgetName));

    public static string CodeScalar_DefaultWidgetName(CultureInfo culture) => WidgetsResources.Get(nameof (CodeScalar_DefaultWidgetName), culture);

    public static string QueryScalar_DefaultWidgetName() => WidgetsResources.Get(nameof (QueryScalar_DefaultWidgetName));

    public static string QueryScalar_DefaultWidgetName(CultureInfo culture) => WidgetsResources.Get(nameof (QueryScalar_DefaultWidgetName), culture);

    public static string SprintOverview_Completed() => WidgetsResources.Get(nameof (SprintOverview_Completed));

    public static string SprintOverview_Completed(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverview_Completed), culture);

    public static string SprintOverview_InProgress() => WidgetsResources.Get(nameof (SprintOverview_InProgress));

    public static string SprintOverview_InProgress(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverview_InProgress), culture);

    public static string SprintOverview_NotStarted() => WidgetsResources.Get(nameof (SprintOverview_NotStarted));

    public static string SprintOverview_NotStarted(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverview_NotStarted), culture);

    public static string SprintOverview_TotalStoryCountNACompleted() => WidgetsResources.Get(nameof (SprintOverview_TotalStoryCountNACompleted));

    public static string SprintOverview_TotalStoryCountNACompleted(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverview_TotalStoryCountNACompleted), culture);

    public static string BuildChartConfiguration_ErrorNoDefinitionSelected() => WidgetsResources.Get(nameof (BuildChartConfiguration_ErrorNoDefinitionSelected));

    public static string BuildChartConfiguration_ErrorNoDefinitionSelected(CultureInfo culture) => WidgetsResources.Get(nameof (BuildChartConfiguration_ErrorNoDefinitionSelected), culture);

    public static string BuildChartConfiguration_Label() => WidgetsResources.Get(nameof (BuildChartConfiguration_Label));

    public static string BuildChartConfiguration_Label(CultureInfo culture) => WidgetsResources.Get(nameof (BuildChartConfiguration_Label), culture);

    public static string BuildChartConfiguration_Watermark() => WidgetsResources.Get(nameof (BuildChartConfiguration_Watermark));

    public static string BuildChartConfiguration_Watermark(CultureInfo culture) => WidgetsResources.Get(nameof (BuildChartConfiguration_Watermark), culture);

    public static string BuildChart_ToolTipTextResult(object arg0) => WidgetsResources.Format(nameof (BuildChart_ToolTipTextResult), arg0);

    public static string BuildChart_ToolTipTextResult(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (BuildChart_ToolTipTextResult), culture, arg0);

    public static string BuildChart_ToolTipTextRunTime(object arg0) => WidgetsResources.Format(nameof (BuildChart_ToolTipTextRunTime), arg0);

    public static string BuildChart_ToolTipTextRunTime(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (BuildChart_ToolTipTextRunTime), culture, arg0);

    public static string BuildConfiguration_DefaultWidgetName() => WidgetsResources.Get(nameof (BuildConfiguration_DefaultWidgetName));

    public static string BuildConfiguration_DefaultWidgetName(CultureInfo culture) => WidgetsResources.Get(nameof (BuildConfiguration_DefaultWidgetName), culture);

    public static string BuildHistogram_DefaultWidgetName() => WidgetsResources.Get(nameof (BuildHistogram_DefaultWidgetName));

    public static string BuildHistogram_DefaultWidgetName(CultureInfo culture) => WidgetsResources.Get(nameof (BuildHistogram_DefaultWidgetName), culture);

    public static string BuildHistogramConfiguration_BranchSectionHeader() => WidgetsResources.Get(nameof (BuildHistogramConfiguration_BranchSectionHeader));

    public static string BuildHistogramConfiguration_BranchSectionHeader(CultureInfo culture) => WidgetsResources.Get(nameof (BuildHistogramConfiguration_BranchSectionHeader), culture);

    public static string BuildHistogramConfiguration_AllBranchesLabel() => WidgetsResources.Get(nameof (BuildHistogramConfiguration_AllBranchesLabel));

    public static string BuildHistogramConfiguration_AllBranchesLabel(CultureInfo culture) => WidgetsResources.Get(nameof (BuildHistogramConfiguration_AllBranchesLabel), culture);

    public static string Markdown_APICallFailed() => WidgetsResources.Get(nameof (Markdown_APICallFailed));

    public static string Markdown_APICallFailed(CultureInfo culture) => WidgetsResources.Get(nameof (Markdown_APICallFailed), culture);

    public static string Markdown_ViewSourceFile() => WidgetsResources.Get(nameof (Markdown_ViewSourceFile));

    public static string Markdown_ViewSourceFile(CultureInfo culture) => WidgetsResources.Get(nameof (Markdown_ViewSourceFile), culture);

    public static string Markdown_InsufficientPermission() => WidgetsResources.Get(nameof (Markdown_InsufficientPermission));

    public static string Markdown_InsufficientPermission(CultureInfo culture) => WidgetsResources.Get(nameof (Markdown_InsufficientPermission), culture);

    public static string Markdown_CustomMarkdownHeader() => WidgetsResources.Get(nameof (Markdown_CustomMarkdownHeader));

    public static string Markdown_CustomMarkdownHeader(CultureInfo culture) => WidgetsResources.Get(nameof (Markdown_CustomMarkdownHeader), culture);

    public static string Markdown_SelectMarkdown() => WidgetsResources.Get(nameof (Markdown_SelectMarkdown));

    public static string Markdown_SelectMarkdown(CultureInfo culture) => WidgetsResources.Get(nameof (Markdown_SelectMarkdown), culture);

    public static string CumulativeFlowDiagram_BoardLabel() => WidgetsResources.Get(nameof (CumulativeFlowDiagram_BoardLabel));

    public static string CumulativeFlowDiagram_BoardLabel(CultureInfo culture) => WidgetsResources.Get(nameof (CumulativeFlowDiagram_BoardLabel), culture);

    public static string CumulativeFlowDiagram_ChartColorLabel() => WidgetsResources.Get(nameof (CumulativeFlowDiagram_ChartColorLabel));

    public static string CumulativeFlowDiagram_ChartColorLabel(CultureInfo culture) => WidgetsResources.Get(nameof (CumulativeFlowDiagram_ChartColorLabel), culture);

    public static string SwimlanePicker_DefaultSwimlaneDisplayNameAddition() => WidgetsResources.Get(nameof (SwimlanePicker_DefaultSwimlaneDisplayNameAddition));

    public static string SwimlanePicker_DefaultSwimlaneDisplayNameAddition(CultureInfo culture) => WidgetsResources.Get(nameof (SwimlanePicker_DefaultSwimlaneDisplayNameAddition), culture);

    public static string RollingPeriod_DaysPrompt() => WidgetsResources.Get(nameof (RollingPeriod_DaysPrompt));

    public static string RollingPeriod_DaysPrompt(CultureInfo culture) => WidgetsResources.Get(nameof (RollingPeriod_DaysPrompt), culture);

    public static string SwimlanePicker_SwimlaneLabel() => WidgetsResources.Get(nameof (SwimlanePicker_SwimlaneLabel));

    public static string SwimlanePicker_SwimlaneLabel(CultureInfo culture) => WidgetsResources.Get(nameof (SwimlanePicker_SwimlaneLabel), culture);

    public static string CumulativeFlowDiagram_TeamLabel() => WidgetsResources.Get(nameof (CumulativeFlowDiagram_TeamLabel));

    public static string CumulativeFlowDiagram_TeamLabel(CultureInfo culture) => WidgetsResources.Get(nameof (CumulativeFlowDiagram_TeamLabel), culture);

    public static string SwimlanePicker_AllOptionName() => WidgetsResources.Get(nameof (SwimlanePicker_AllOptionName));

    public static string SwimlanePicker_AllOptionName(CultureInfo culture) => WidgetsResources.Get(nameof (SwimlanePicker_AllOptionName), culture);

    public static string CumulativeFlowDiagram_DefaultWidgetName() => WidgetsResources.Get(nameof (CumulativeFlowDiagram_DefaultWidgetName));

    public static string CumulativeFlowDiagram_DefaultWidgetName(CultureInfo culture) => WidgetsResources.Get(nameof (CumulativeFlowDiagram_DefaultWidgetName), culture);

    public static string WitChart_ConfigurationQueryNotFound() => WidgetsResources.Get(nameof (WitChart_ConfigurationQueryNotFound));

    public static string WitChart_ConfigurationQueryNotFound(CultureInfo culture) => WidgetsResources.Get(nameof (WitChart_ConfigurationQueryNotFound), culture);

    public static string ChartWidget_ChartType() => WidgetsResources.Get(nameof (ChartWidget_ChartType));

    public static string ChartWidget_ChartType(CultureInfo culture) => WidgetsResources.Get(nameof (ChartWidget_ChartType), culture);

    public static string ChartRemainingItems() => WidgetsResources.Get(nameof (ChartRemainingItems));

    public static string ChartRemainingItems(CultureInfo culture) => WidgetsResources.Get(nameof (ChartRemainingItems), culture);

    public static string ChartDataNullValueLabel() => WidgetsResources.Get(nameof (ChartDataNullValueLabel));

    public static string ChartDataNullValueLabel(CultureInfo culture) => WidgetsResources.Get(nameof (ChartDataNullValueLabel), culture);

    public static string WitChart_ConfigurationCannotGetProject() => WidgetsResources.Get(nameof (WitChart_ConfigurationCannotGetProject));

    public static string WitChart_ConfigurationCannotGetProject(CultureInfo culture) => WidgetsResources.Get(nameof (WitChart_ConfigurationCannotGetProject), culture);

    public static string ChartConfiguration_GenericDropdownError() => WidgetsResources.Get(nameof (ChartConfiguration_GenericDropdownError));

    public static string ChartConfiguration_GenericDropdownError(CultureInfo culture) => WidgetsResources.Get(nameof (ChartConfiguration_GenericDropdownError), culture);

    public static string WitChart_HierarchicalQueriesNotSupportedError() => WidgetsResources.Get(nameof (WitChart_HierarchicalQueriesNotSupportedError));

    public static string WitChart_HierarchicalQueriesNotSupportedError(CultureInfo culture) => WidgetsResources.Get(nameof (WitChart_HierarchicalQueriesNotSupportedError), culture);

    public static string BaseChart_InvalidConfiguration(object arg0) => WidgetsResources.Format(nameof (BaseChart_InvalidConfiguration), arg0);

    public static string BaseChart_InvalidConfiguration(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (BaseChart_InvalidConfiguration), culture, arg0);

    public static string BaseChart_InvalidResultsFormat() => WidgetsResources.Get(nameof (BaseChart_InvalidResultsFormat));

    public static string BaseChart_InvalidResultsFormat(CultureInfo culture) => WidgetsResources.Get(nameof (BaseChart_InvalidResultsFormat), culture);

    public static string ChartWidget_QuerySelectionTooltip() => WidgetsResources.Get(nameof (ChartWidget_QuerySelectionTooltip));

    public static string ChartWidget_QuerySelectionTooltip(CultureInfo culture) => WidgetsResources.Get(nameof (ChartWidget_QuerySelectionTooltip), culture);

    public static string ChartPaletteName_Blue() => WidgetsResources.Get(nameof (ChartPaletteName_Blue));

    public static string ChartPaletteName_Blue(CultureInfo culture) => WidgetsResources.Get(nameof (ChartPaletteName_Blue), culture);

    public static string ChartPaletteName_Green() => WidgetsResources.Get(nameof (ChartPaletteName_Green));

    public static string ChartPaletteName_Green(CultureInfo culture) => WidgetsResources.Get(nameof (ChartPaletteName_Green), culture);

    public static string ChartPaletteName_Purple() => WidgetsResources.Get(nameof (ChartPaletteName_Purple));

    public static string ChartPaletteName_Purple(CultureInfo culture) => WidgetsResources.Get(nameof (ChartPaletteName_Purple), culture);

    public static string ChartPaletteName_Red() => WidgetsResources.Get(nameof (ChartPaletteName_Red));

    public static string ChartPaletteName_Red(CultureInfo culture) => WidgetsResources.Get(nameof (ChartPaletteName_Red), culture);

    public static string ChartPaletteName_Orange() => WidgetsResources.Get(nameof (ChartPaletteName_Orange));

    public static string ChartPaletteName_Orange(CultureInfo culture) => WidgetsResources.Get(nameof (ChartPaletteName_Orange), culture);

    public static string Markdown_VC_SelectFileErrorNoFileSelected() => WidgetsResources.Get(nameof (Markdown_VC_SelectFileErrorNoFileSelected));

    public static string Markdown_VC_SelectFileErrorNoFileSelected(CultureInfo culture) => WidgetsResources.Get(nameof (Markdown_VC_SelectFileErrorNoFileSelected), culture);

    public static string ChartPaletteName_Yellow() => WidgetsResources.Get(nameof (ChartPaletteName_Yellow));

    public static string ChartPaletteName_Yellow(CultureInfo culture) => WidgetsResources.Get(nameof (ChartPaletteName_Yellow), culture);

    public static string WitChart_DefaultWidgetName() => WidgetsResources.Get(nameof (WitChart_DefaultWidgetName));

    public static string WitChart_DefaultWidgetName(CultureInfo culture) => WidgetsResources.Get(nameof (WitChart_DefaultWidgetName), culture);

    public static string WitChart_TitleTemplate_QueryName_GroupBy(object arg0, object arg1) => WidgetsResources.Format(nameof (WitChart_TitleTemplate_QueryName_GroupBy), arg0, arg1);

    public static string WitChart_TitleTemplate_QueryName_GroupBy(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (WitChart_TitleTemplate_QueryName_GroupBy), culture, arg0, arg1);
    }

    public static string ErrorQuery(object arg0) => WidgetsResources.Format(nameof (ErrorQuery), arg0);

    public static string ErrorQuery(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (ErrorQuery), culture, arg0);

    public static string WitChart_ConfigLazyUpgradeWarning() => WidgetsResources.Get(nameof (WitChart_ConfigLazyUpgradeWarning));

    public static string WitChart_ConfigLazyUpgradeWarning(CultureInfo culture) => WidgetsResources.Get(nameof (WitChart_ConfigLazyUpgradeWarning), culture);

    public static string WitQueryChart_DataSourceNeeded() => WidgetsResources.Get(nameof (WitQueryChart_DataSourceNeeded));

    public static string WitQueryChart_DataSourceNeeded(CultureInfo culture) => WidgetsResources.Get(nameof (WitQueryChart_DataSourceNeeded), culture);

    public static string WitChart_WorkItemsPluralName() => WidgetsResources.Get(nameof (WitChart_WorkItemsPluralName));

    public static string WitChart_WorkItemsPluralName(CultureInfo culture) => WidgetsResources.Get(nameof (WitChart_WorkItemsPluralName), culture);

    public static string NoRepoSelectedText() => WidgetsResources.Get(nameof (NoRepoSelectedText));

    public static string NoRepoSelectedText(CultureInfo culture) => WidgetsResources.Get(nameof (NoRepoSelectedText), culture);

    public static string PullRequestWidgetConfigurationInvalidTeamSelected() => WidgetsResources.Get(nameof (PullRequestWidgetConfigurationInvalidTeamSelected));

    public static string PullRequestWidgetConfigurationInvalidTeamSelected(CultureInfo culture) => WidgetsResources.Get(nameof (PullRequestWidgetConfigurationInvalidTeamSelected), culture);

    public static string PullRequestWidgetConfigurationNoRepoSelected() => WidgetsResources.Get(nameof (PullRequestWidgetConfigurationNoRepoSelected));

    public static string PullRequestWidgetConfigurationNoRepoSelected(CultureInfo culture) => WidgetsResources.Get(nameof (PullRequestWidgetConfigurationNoRepoSelected), culture);

    public static string PullRequestWidgetConfigurationNoTeamSelected() => WidgetsResources.Get(nameof (PullRequestWidgetConfigurationNoTeamSelected));

    public static string PullRequestWidgetConfigurationNoTeamSelected(CultureInfo culture) => WidgetsResources.Get(nameof (PullRequestWidgetConfigurationNoTeamSelected), culture);

    public static string PullRequestWidgetConfigurationNoViewSelected() => WidgetsResources.Get(nameof (PullRequestWidgetConfigurationNoViewSelected));

    public static string PullRequestWidgetConfigurationNoViewSelected(CultureInfo culture) => WidgetsResources.Get(nameof (PullRequestWidgetConfigurationNoViewSelected), culture);

    public static string PullRequestWidgetConfigurationRepoSectionHeader() => WidgetsResources.Get(nameof (PullRequestWidgetConfigurationRepoSectionHeader));

    public static string PullRequestWidgetConfigurationRepoSectionHeader(CultureInfo culture) => WidgetsResources.Get(nameof (PullRequestWidgetConfigurationRepoSectionHeader), culture);

    public static string PullRequestWidgetConfigurationTeamNotFound() => WidgetsResources.Get(nameof (PullRequestWidgetConfigurationTeamNotFound));

    public static string PullRequestWidgetConfigurationTeamNotFound(CultureInfo culture) => WidgetsResources.Get(nameof (PullRequestWidgetConfigurationTeamNotFound), culture);

    public static string PullRequestWidgetConfigurationTeamSectionHeader() => WidgetsResources.Get(nameof (PullRequestWidgetConfigurationTeamSectionHeader));

    public static string PullRequestWidgetConfigurationTeamSectionHeader(CultureInfo culture) => WidgetsResources.Get(nameof (PullRequestWidgetConfigurationTeamSectionHeader), culture);

    public static string PullRequestWidgetConfigurationViewSectionHeader() => WidgetsResources.Get(nameof (PullRequestWidgetConfigurationViewSectionHeader));

    public static string PullRequestWidgetConfigurationViewSectionHeader(CultureInfo culture) => WidgetsResources.Get(nameof (PullRequestWidgetConfigurationViewSectionHeader), culture);

    public static string PullRequestWidgetCreatedDateFormat(object arg0) => WidgetsResources.Format(nameof (PullRequestWidgetCreatedDateFormat), arg0);

    public static string PullRequestWidgetCreatedDateFormat(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (PullRequestWidgetCreatedDateFormat), culture, arg0);

    public static string PullRequestWidgetDetailsFormat(object arg0, object arg1, object arg2) => WidgetsResources.Format(nameof (PullRequestWidgetDetailsFormat), arg0, arg1, arg2);

    public static string PullRequestWidgetDetailsFormat(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (PullRequestWidgetDetailsFormat), culture, arg0, arg1, arg2);
    }

    public static string PullRequestWidgetNoResultsEmptyMessage() => WidgetsResources.Get(nameof (PullRequestWidgetNoResultsEmptyMessage));

    public static string PullRequestWidgetNoResultsEmptyMessage(CultureInfo culture) => WidgetsResources.Get(nameof (PullRequestWidgetNoResultsEmptyMessage), culture);

    public static string PullRequestWidgetTitle() => WidgetsResources.Get(nameof (PullRequestWidgetTitle));

    public static string PullRequestWidgetTitle(CultureInfo culture) => WidgetsResources.Get(nameof (PullRequestWidgetTitle), culture);

    public static string PullRequestWidgetTitleCountFormat(object arg0) => WidgetsResources.Format(nameof (PullRequestWidgetTitleCountFormat), arg0);

    public static string PullRequestWidgetTitleCountFormat(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (PullRequestWidgetTitleCountFormat), culture, arg0);

    public static string PullRequestWidgetTitleFormat(object arg0) => WidgetsResources.Format(nameof (PullRequestWidgetTitleFormat), arg0);

    public static string PullRequestWidgetTitleFormat(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (PullRequestWidgetTitleFormat), culture, arg0);

    public static string PullRequestWidgetViewAllAnchorText() => WidgetsResources.Get(nameof (PullRequestWidgetViewAllAnchorText));

    public static string PullRequestWidgetViewAllAnchorText(CultureInfo culture) => WidgetsResources.Get(nameof (PullRequestWidgetViewAllAnchorText), culture);

    public static string PullRequestWidgetViewFilterTitle() => WidgetsResources.Get(nameof (PullRequestWidgetViewFilterTitle));

    public static string PullRequestWidgetViewFilterTitle(CultureInfo culture) => WidgetsResources.Get(nameof (PullRequestWidgetViewFilterTitle), culture);

    public static string PullRequestWidgetViewAssignedToSpecificTeam(object arg0) => WidgetsResources.Format(nameof (PullRequestWidgetViewAssignedToSpecificTeam), arg0);

    public static string PullRequestWidgetViewAssignedToSpecificTeam(
      object arg0,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (PullRequestWidgetViewAssignedToSpecificTeam), culture, arg0);
    }

    public static string PullRequestWidgetNoResultsErrorSymbol() => WidgetsResources.Get(nameof (PullRequestWidgetNoResultsErrorSymbol));

    public static string PullRequestWidgetNoResultsErrorSymbol(CultureInfo culture) => WidgetsResources.Get(nameof (PullRequestWidgetNoResultsErrorSymbol), culture);

    public static string TeamMembers_InviteAFriend() => WidgetsResources.Get(nameof (TeamMembers_InviteAFriend));

    public static string TeamMembers_InviteAFriend(CultureInfo culture) => WidgetsResources.Get(nameof (TeamMembers_InviteAFriend), culture);

    public static string TeamMembers_InviteAFriend_Message_Caption_1() => WidgetsResources.Get(nameof (TeamMembers_InviteAFriend_Message_Caption_1));

    public static string TeamMembers_InviteAFriend_Message_Caption_1(CultureInfo culture) => WidgetsResources.Get(nameof (TeamMembers_InviteAFriend_Message_Caption_1), culture);

    public static string TeamMembers_InviteAFriend_Message_Caption_2() => WidgetsResources.Get(nameof (TeamMembers_InviteAFriend_Message_Caption_2));

    public static string TeamMembers_InviteAFriend_Message_Caption_2(CultureInfo culture) => WidgetsResources.Get(nameof (TeamMembers_InviteAFriend_Message_Caption_2), culture);

    public static string TeamMembers_InviteAFriend_Message_Caption_3() => WidgetsResources.Get(nameof (TeamMembers_InviteAFriend_Message_Caption_3));

    public static string TeamMembers_InviteAFriend_Message_Caption_3(CultureInfo culture) => WidgetsResources.Get(nameof (TeamMembers_InviteAFriend_Message_Caption_3), culture);

    public static string TeamMembers_ViewAll() => WidgetsResources.Get(nameof (TeamMembers_ViewAll));

    public static string TeamMembers_ViewAll(CultureInfo culture) => WidgetsResources.Get(nameof (TeamMembers_ViewAll), culture);

    public static string TeamMembers_ManageTeamMembers() => WidgetsResources.Get(nameof (TeamMembers_ManageTeamMembers));

    public static string TeamMembers_ManageTeamMembers(CultureInfo culture) => WidgetsResources.Get(nameof (TeamMembers_ManageTeamMembers), culture);

    public static string QueryScalar_AddRule() => WidgetsResources.Get(nameof (QueryScalar_AddRule));

    public static string QueryScalar_AddRule(CultureInfo culture) => WidgetsResources.Get(nameof (QueryScalar_AddRule), culture);

    public static string QueryScalar_CompareText() => WidgetsResources.Get(nameof (QueryScalar_CompareText));

    public static string QueryScalar_CompareText(CultureInfo culture) => WidgetsResources.Get(nameof (QueryScalar_CompareText), culture);

    public static string QueryScalar_ConditionalColorLabel() => WidgetsResources.Get(nameof (QueryScalar_ConditionalColorLabel));

    public static string QueryScalar_ConditionalColorLabel(CultureInfo culture) => WidgetsResources.Get(nameof (QueryScalar_ConditionalColorLabel), culture);

    public static string QueryScalar_RuleLimitReached() => WidgetsResources.Get(nameof (QueryScalar_RuleLimitReached));

    public static string QueryScalar_RuleLimitReached(CultureInfo culture) => WidgetsResources.Get(nameof (QueryScalar_RuleLimitReached), culture);

    public static string QueryScalar_ConditionalColorLabelTooltip() => WidgetsResources.Get(nameof (QueryScalar_ConditionalColorLabelTooltip));

    public static string QueryScalar_ConditionalColorLabelTooltip(CultureInfo culture) => WidgetsResources.Get(nameof (QueryScalar_ConditionalColorLabelTooltip), culture);

    public static string IterationPicker_NoIterationSelectedError() => WidgetsResources.Get(nameof (IterationPicker_NoIterationSelectedError));

    public static string IterationPicker_NoIterationSelectedError(CultureInfo culture) => WidgetsResources.Get(nameof (IterationPicker_NoIterationSelectedError), culture);

    public static string IterationPicker_SelectedIterationDoesNotHaveDatesErrorFormat(object arg0) => WidgetsResources.Format(nameof (IterationPicker_SelectedIterationDoesNotHaveDatesErrorFormat), arg0);

    public static string IterationPicker_SelectedIterationDoesNotHaveDatesErrorFormat(
      object arg0,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (IterationPicker_SelectedIterationDoesNotHaveDatesErrorFormat), culture, arg0);
    }

    public static string IterationPicker_SelectedIterationNotFoundError() => WidgetsResources.Get(nameof (IterationPicker_SelectedIterationNotFoundError));

    public static string IterationPicker_SelectedIterationNotFoundError(CultureInfo culture) => WidgetsResources.Get(nameof (IterationPicker_SelectedIterationNotFoundError), culture);

    public static string BacklogPicker_NoBacklogSelectedError() => WidgetsResources.Get(nameof (BacklogPicker_NoBacklogSelectedError));

    public static string BacklogPicker_NoBacklogSelectedError(CultureInfo culture) => WidgetsResources.Get(nameof (BacklogPicker_NoBacklogSelectedError), culture);

    public static string BacklogPicker_SelectedBacklogNotFoundError() => WidgetsResources.Get(nameof (BacklogPicker_SelectedBacklogNotFoundError));

    public static string BacklogPicker_SelectedBacklogNotFoundError(CultureInfo culture) => WidgetsResources.Get(nameof (BacklogPicker_SelectedBacklogNotFoundError), culture);

    public static string CumulativeFlowDiagram_PreviouslySelectedBoardNotFoundError() => WidgetsResources.Get(nameof (CumulativeFlowDiagram_PreviouslySelectedBoardNotFoundError));

    public static string CumulativeFlowDiagram_PreviouslySelectedBoardNotFoundError(
      CultureInfo culture)
    {
      return WidgetsResources.Get(nameof (CumulativeFlowDiagram_PreviouslySelectedBoardNotFoundError), culture);
    }

    public static string SwimlanePicker_PreviouslySelectedSwimlaneNotFoundError() => WidgetsResources.Get(nameof (SwimlanePicker_PreviouslySelectedSwimlaneNotFoundError));

    public static string SwimlanePicker_PreviouslySelectedSwimlaneNotFoundError(CultureInfo culture) => WidgetsResources.Get(nameof (SwimlanePicker_PreviouslySelectedSwimlaneNotFoundError), culture);

    public static string CumulativeFlowDiagram_PreviouslySelectedTeamNotFoundError() => WidgetsResources.Get(nameof (CumulativeFlowDiagram_PreviouslySelectedTeamNotFoundError));

    public static string CumulativeFlowDiagram_PreviouslySelectedTeamNotFoundError(
      CultureInfo culture)
    {
      return WidgetsResources.Get(nameof (CumulativeFlowDiagram_PreviouslySelectedTeamNotFoundError), culture);
    }

    public static string CumulativeFlowDiagram_IncludeFirstColumnLabel() => WidgetsResources.Get(nameof (CumulativeFlowDiagram_IncludeFirstColumnLabel));

    public static string CumulativeFlowDiagram_IncludeFirstColumnLabel(CultureInfo culture) => WidgetsResources.Get(nameof (CumulativeFlowDiagram_IncludeFirstColumnLabel), culture);

    public static string RollingPeriod_MaxAllowedDaysErrorFormat(object arg0) => WidgetsResources.Format(nameof (RollingPeriod_MaxAllowedDaysErrorFormat), arg0);

    public static string RollingPeriod_MaxAllowedDaysErrorFormat(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (RollingPeriod_MaxAllowedDaysErrorFormat), culture, arg0);

    public static string SwimlanePicker_NoSwimlaneSelectedError() => WidgetsResources.Get(nameof (SwimlanePicker_NoSwimlaneSelectedError));

    public static string SwimlanePicker_NoSwimlaneSelectedError(CultureInfo culture) => WidgetsResources.Get(nameof (SwimlanePicker_NoSwimlaneSelectedError), culture);

    public static string SwimlanePicker_SelectedSwimlaneNotFoundError() => WidgetsResources.Get(nameof (SwimlanePicker_SelectedSwimlaneNotFoundError));

    public static string SwimlanePicker_SelectedSwimlaneNotFoundError(CultureInfo culture) => WidgetsResources.Get(nameof (SwimlanePicker_SelectedSwimlaneNotFoundError), culture);

    public static string TeamPicker_NoTeamSelectedError() => WidgetsResources.Get(nameof (TeamPicker_NoTeamSelectedError));

    public static string TeamPicker_NoTeamSelectedError(CultureInfo culture) => WidgetsResources.Get(nameof (TeamPicker_NoTeamSelectedError), culture);

    public static string TeamPicker_SelectedTeamNotFoundError() => WidgetsResources.Get(nameof (TeamPicker_SelectedTeamNotFoundError));

    public static string TeamPicker_SelectedTeamNotFoundError(CultureInfo culture) => WidgetsResources.Get(nameof (TeamPicker_SelectedTeamNotFoundError), culture);

    public static string NewWorkItem_DefaultTypeError() => WidgetsResources.Get(nameof (NewWorkItem_DefaultTypeError));

    public static string NewWorkItem_DefaultTypeError(CultureInfo culture) => WidgetsResources.Get(nameof (NewWorkItem_DefaultTypeError), culture);

    public static string NewWorkItem_DefaultTypeText() => WidgetsResources.Get(nameof (NewWorkItem_DefaultTypeText));

    public static string NewWorkItem_DefaultTypeText(CultureInfo culture) => WidgetsResources.Get(nameof (NewWorkItem_DefaultTypeText), culture);

    public static string WitChart_SelectAQueryMessage() => WidgetsResources.Get(nameof (WitChart_SelectAQueryMessage));

    public static string WitChart_SelectAQueryMessage(CultureInfo culture) => WidgetsResources.Get(nameof (WitChart_SelectAQueryMessage), culture);

    public static string CumulativeFlowDiagram_LearnLink() => WidgetsResources.Get(nameof (CumulativeFlowDiagram_LearnLink));

    public static string CumulativeFlowDiagram_LearnLink(CultureInfo culture) => WidgetsResources.Get(nameof (CumulativeFlowDiagram_LearnLink), culture);

    public static string SwimlanePicker_SwimlaneBadge() => WidgetsResources.Get(nameof (SwimlanePicker_SwimlaneBadge));

    public static string SwimlanePicker_SwimlaneBadge(CultureInfo culture) => WidgetsResources.Get(nameof (SwimlanePicker_SwimlaneBadge), culture);

    public static string TimePeriodSubtitleFormat_Plural(object arg0) => WidgetsResources.Format(nameof (TimePeriodSubtitleFormat_Plural), arg0);

    public static string TimePeriodSubtitleFormat_Plural(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (TimePeriodSubtitleFormat_Plural), culture, arg0);

    public static string RollingPeriod_DateHintFormat(object arg0) => WidgetsResources.Format(nameof (RollingPeriod_DateHintFormat), arg0);

    public static string RollingPeriod_DateHintFormat(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (RollingPeriod_DateHintFormat), culture, arg0);

    public static string IframeConfiguration_Url() => WidgetsResources.Get(nameof (IframeConfiguration_Url));

    public static string IframeConfiguration_Url(CultureInfo culture) => WidgetsResources.Get(nameof (IframeConfiguration_Url), culture);

    public static string IframeConfiguration_NoUrlError() => WidgetsResources.Get(nameof (IframeConfiguration_NoUrlError));

    public static string IframeConfiguration_NoUrlError(CultureInfo culture) => WidgetsResources.Get(nameof (IframeConfiguration_NoUrlError), culture);

    public static string IframeConfiguration_UrlNoProtocolError() => WidgetsResources.Get(nameof (IframeConfiguration_UrlNoProtocolError));

    public static string IframeConfiguration_UrlNoProtocolError(CultureInfo culture) => WidgetsResources.Get(nameof (IframeConfiguration_UrlNoProtocolError), culture);

    public static string IframeConfiguration_UrlWatermark() => WidgetsResources.Get(nameof (IframeConfiguration_UrlWatermark));

    public static string IframeConfiguration_UrlWatermark(CultureInfo culture) => WidgetsResources.Get(nameof (IframeConfiguration_UrlWatermark), culture);

    public static string IframeConfiguration_UrlDisclaimer(object arg0) => WidgetsResources.Format(nameof (IframeConfiguration_UrlDisclaimer), arg0);

    public static string IframeConfiguration_UrlDisclaimer(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (IframeConfiguration_UrlDisclaimer), culture, arg0);

    public static string CumulativeFlowDiagram_DeletedColumnNameSuffix() => WidgetsResources.Get(nameof (CumulativeFlowDiagram_DeletedColumnNameSuffix));

    public static string CumulativeFlowDiagram_DeletedColumnNameSuffix(CultureInfo culture) => WidgetsResources.Get(nameof (CumulativeFlowDiagram_DeletedColumnNameSuffix), culture);

    public static string TimePeriodSubtitleFormat_Singular() => WidgetsResources.Get(nameof (TimePeriodSubtitleFormat_Singular));

    public static string TimePeriodSubtitleFormat_Singular(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriodSubtitleFormat_Singular), culture);

    public static string WidgetNotificationConfigureTitle() => WidgetsResources.Get(nameof (WidgetNotificationConfigureTitle));

    public static string WidgetNotificationConfigureTitle(CultureInfo culture) => WidgetsResources.Get(nameof (WidgetNotificationConfigureTitle), culture);

    public static string WidgetNotificationConfigure_ScreenReaderSupportText(object arg0) => WidgetsResources.Format(nameof (WidgetNotificationConfigure_ScreenReaderSupportText), arg0);

    public static string WidgetNotificationConfigure_ScreenReaderSupportText(
      object arg0,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (WidgetNotificationConfigure_ScreenReaderSupportText), culture, arg0);
    }

    public static string WidgetNotificationConfigure_ScreenReaderSupportText_Admin(object arg0) => WidgetsResources.Format(nameof (WidgetNotificationConfigure_ScreenReaderSupportText_Admin), arg0);

    public static string WidgetNotificationConfigure_ScreenReaderSupportText_Admin(
      object arg0,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (WidgetNotificationConfigure_ScreenReaderSupportText_Admin), culture, arg0);
    }

    public static string AssignedToMeWidget_DefaultTitle(object arg0) => WidgetsResources.Format(nameof (AssignedToMeWidget_DefaultTitle), arg0);

    public static string AssignedToMeWidget_DefaultTitle(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (AssignedToMeWidget_DefaultTitle), culture, arg0);

    public static string CountFilter_ExcessItemsSuffix() => WidgetsResources.Get(nameof (CountFilter_ExcessItemsSuffix));

    public static string CountFilter_ExcessItemsSuffix(CultureInfo culture) => WidgetsResources.Get(nameof (CountFilter_ExcessItemsSuffix), culture);

    public static string CountFilterList_Other() => WidgetsResources.Get(nameof (CountFilterList_Other));

    public static string CountFilterList_Other(CultureInfo culture) => WidgetsResources.Get(nameof (CountFilterList_Other), culture);

    public static string AssignedToMeWidget_NoWorkText(object arg0) => WidgetsResources.Format(nameof (AssignedToMeWidget_NoWorkText), arg0);

    public static string AssignedToMeWidget_NoWorkText(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (AssignedToMeWidget_NoWorkText), culture, arg0);

    public static string AssignedToMeWidget_TooMuchWorkText() => WidgetsResources.Get(nameof (AssignedToMeWidget_TooMuchWorkText));

    public static string AssignedToMeWidget_TooMuchWorkText(CultureInfo culture) => WidgetsResources.Get(nameof (AssignedToMeWidget_TooMuchWorkText), culture);

    public static string AssignedToMeWidget_ViewAllText() => WidgetsResources.Get(nameof (AssignedToMeWidget_ViewAllText));

    public static string AssignedToMeWidget_ViewAllText(CultureInfo culture) => WidgetsResources.Get(nameof (AssignedToMeWidget_ViewAllText), culture);

    public static string AssignedToMeWidget_TempQueryName() => WidgetsResources.Get(nameof (AssignedToMeWidget_TempQueryName));

    public static string AssignedToMeWidget_TempQueryName(CultureInfo culture) => WidgetsResources.Get(nameof (AssignedToMeWidget_TempQueryName), culture);

    public static string NewWorkItem_Title_AriaLabel() => WidgetsResources.Get(nameof (NewWorkItem_Title_AriaLabel));

    public static string NewWorkItem_Title_AriaLabel(CultureInfo culture) => WidgetsResources.Get(nameof (NewWorkItem_Title_AriaLabel), culture);

    public static string NewWorkItem_Type_AriaLabel() => WidgetsResources.Get(nameof (NewWorkItem_Type_AriaLabel));

    public static string NewWorkItem_Type_AriaLabel(CultureInfo culture) => WidgetsResources.Get(nameof (NewWorkItem_Type_AriaLabel), culture);

    public static string PullRequest_ViewAllAriaLabel_Format(object arg0) => WidgetsResources.Format(nameof (PullRequest_ViewAllAriaLabel_Format), arg0);

    public static string PullRequest_ViewAllAriaLabel_Format(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (PullRequest_ViewAllAriaLabel_Format), culture, arg0);

    public static string PullRequest_Filter_AssignedToTeam() => WidgetsResources.Get(nameof (PullRequest_Filter_AssignedToTeam));

    public static string PullRequest_Filter_AssignedToTeam(CultureInfo culture) => WidgetsResources.Get(nameof (PullRequest_Filter_AssignedToTeam), culture);

    public static string TeamMember_ManageAriaLabel() => WidgetsResources.Get(nameof (TeamMember_ManageAriaLabel));

    public static string TeamMember_ManageAriaLabel(CultureInfo culture) => WidgetsResources.Get(nameof (TeamMember_ManageAriaLabel), culture);

    public static string TeamMember_ViewAllAriaLabel() => WidgetsResources.Get(nameof (TeamMember_ViewAllAriaLabel));

    public static string TeamMember_ViewAllAriaLabel(CultureInfo culture) => WidgetsResources.Get(nameof (TeamMember_ViewAllAriaLabel), culture);

    public static string SprintOverview_CountOfWorkItems() => WidgetsResources.Get(nameof (SprintOverview_CountOfWorkItems));

    public static string SprintOverview_CountOfWorkItems(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverview_CountOfWorkItems), culture);

    public static string SprintOverview_DaysCompleted() => WidgetsResources.Get(nameof (SprintOverview_DaysCompleted));

    public static string SprintOverview_DaysCompleted(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverview_DaysCompleted), culture);

    public static string SprintOverview_DaysRemaining() => WidgetsResources.Get(nameof (SprintOverview_DaysRemaining));

    public static string SprintOverview_DaysRemaining(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverview_DaysRemaining), culture);

    public static string SprintOverview_DayCompleted() => WidgetsResources.Get(nameof (SprintOverview_DayCompleted));

    public static string SprintOverview_DayCompleted(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverview_DayCompleted), culture);

    public static string SprintOverview_DayRemaining() => WidgetsResources.Get(nameof (SprintOverview_DayRemaining));

    public static string SprintOverview_DayRemaining(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverview_DayRemaining), culture);

    public static string SprintOverview_WorkRemaining() => WidgetsResources.Get(nameof (SprintOverview_WorkRemaining));

    public static string SprintOverview_WorkRemaining(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverview_WorkRemaining), culture);

    public static string SprintOverview_WorksRemaining() => WidgetsResources.Get(nameof (SprintOverview_WorksRemaining));

    public static string SprintOverview_WorksRemaining(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverview_WorksRemaining), culture);

    public static string SprintOverview_MissingTeams() => WidgetsResources.Get(nameof (SprintOverview_MissingTeams));

    public static string SprintOverview_MissingTeams(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverview_MissingTeams), culture);

    public static string SprintOverview_NoDayRemaining() => WidgetsResources.Get(nameof (SprintOverview_NoDayRemaining));

    public static string SprintOverview_NoDayRemaining(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverview_NoDayRemaining), culture);

    public static string SprintOverview_WorkNotStarted() => WidgetsResources.Get(nameof (SprintOverview_WorkNotStarted));

    public static string SprintOverview_WorkNotStarted(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverview_WorkNotStarted), culture);

    public static string SprintOverviewConfiguration_CountOfWorkItems() => WidgetsResources.Get(nameof (SprintOverviewConfiguration_CountOfWorkItems));

    public static string SprintOverviewConfiguration_CountOfWorkItems(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverviewConfiguration_CountOfWorkItems), culture);

    public static string SprintOverviewConfiguration_InvalidTeam() => WidgetsResources.Get(nameof (SprintOverviewConfiguration_InvalidTeam));

    public static string SprintOverviewConfiguration_InvalidTeam(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverviewConfiguration_InvalidTeam), culture);

    public static string SprintOverviewConfiguration_InvalidUnit() => WidgetsResources.Get(nameof (SprintOverviewConfiguration_InvalidUnit));

    public static string SprintOverviewConfiguration_InvalidUnit(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverviewConfiguration_InvalidUnit), culture);

    public static string SprintOverviewConfiguration_ShowNonWorkingDays() => WidgetsResources.Get(nameof (SprintOverviewConfiguration_ShowNonWorkingDays));

    public static string SprintOverviewConfiguration_ShowNonWorkingDays(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverviewConfiguration_ShowNonWorkingDays), culture);

    public static string SprintOverviewConfiguration_Team() => WidgetsResources.Get(nameof (SprintOverviewConfiguration_Team));

    public static string SprintOverviewConfiguration_Team(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverviewConfiguration_Team), culture);

    public static string SprintOverviewConfiguration_Values() => WidgetsResources.Get(nameof (SprintOverviewConfiguration_Values));

    public static string SprintOverviewConfiguration_Values(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverviewConfiguration_Values), culture);

    public static string SprintOverviewConfiguration_WorkingDays() => WidgetsResources.Get(nameof (SprintOverviewConfiguration_WorkingDays));

    public static string SprintOverviewConfiguration_WorkingDays(CultureInfo culture) => WidgetsResources.Get(nameof (SprintOverviewConfiguration_WorkingDays), culture);

    public static string QueryScalar_ConditionalDeleteColorRuleAriaLabel() => WidgetsResources.Get(nameof (QueryScalar_ConditionalDeleteColorRuleAriaLabel));

    public static string QueryScalar_ConditionalDeleteColorRuleAriaLabel(CultureInfo culture) => WidgetsResources.Get(nameof (QueryScalar_ConditionalDeleteColorRuleAriaLabel), culture);

    public static string CumulativeFlowDiagram_TitleWithoutSwimlaneFormat(object arg0, object arg1) => WidgetsResources.Format(nameof (CumulativeFlowDiagram_TitleWithoutSwimlaneFormat), arg0, arg1);

    public static string CumulativeFlowDiagram_TitleWithoutSwimlaneFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (CumulativeFlowDiagram_TitleWithoutSwimlaneFormat), culture, arg0, arg1);
    }

    public static string CumulativeFlowDiagram_TitleWithSwimlaneFormat(
      object arg0,
      object arg1,
      object arg2)
    {
      return WidgetsResources.Format(nameof (CumulativeFlowDiagram_TitleWithSwimlaneFormat), arg0, arg1, arg2);
    }

    public static string CumulativeFlowDiagram_TitleWithSwimlaneFormat(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (CumulativeFlowDiagram_TitleWithSwimlaneFormat), culture, arg0, arg1, arg2);
    }

    public static string TimePeriodSubtitleFormat_StartDate(object arg0) => WidgetsResources.Format(nameof (TimePeriodSubtitleFormat_StartDate), arg0);

    public static string TimePeriodSubtitleFormat_StartDate(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (TimePeriodSubtitleFormat_StartDate), culture, arg0);

    public static string CumulativeFlowDiagram_TimePeriodLabel() => WidgetsResources.Get(nameof (CumulativeFlowDiagram_TimePeriodLabel));

    public static string CumulativeFlowDiagram_TimePeriodLabel(CultureInfo culture) => WidgetsResources.Get(nameof (CumulativeFlowDiagram_TimePeriodLabel), culture);

    public static string RollingPeriod_InvalidInputError() => WidgetsResources.Get(nameof (RollingPeriod_InvalidInputError));

    public static string RollingPeriod_InvalidInputError(CultureInfo culture) => WidgetsResources.Get(nameof (RollingPeriod_InvalidInputError), culture);

    public static string StartDatePicker_MaxAllowedDaysErrorFormat(object arg0) => WidgetsResources.Format(nameof (StartDatePicker_MaxAllowedDaysErrorFormat), arg0);

    public static string StartDatePicker_MaxAllowedDaysErrorFormat(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (StartDatePicker_MaxAllowedDaysErrorFormat), culture, arg0);

    public static string StartDatePicker_MustBePastDateError() => WidgetsResources.Get(nameof (StartDatePicker_MustBePastDateError));

    public static string StartDatePicker_MustBePastDateError(CultureInfo culture) => WidgetsResources.Get(nameof (StartDatePicker_MustBePastDateError), culture);

    public static string StartDatePicker_NoDateSelectedError() => WidgetsResources.Get(nameof (StartDatePicker_NoDateSelectedError));

    public static string StartDatePicker_NoDateSelectedError(CultureInfo culture) => WidgetsResources.Get(nameof (StartDatePicker_NoDateSelectedError), culture);

    public static string StartDatePicker_StartDate() => WidgetsResources.Get(nameof (StartDatePicker_StartDate));

    public static string StartDatePicker_StartDate(CultureInfo culture) => WidgetsResources.Get(nameof (StartDatePicker_StartDate), culture);

    public static string BacklogPicker_NoBacklogsToChooseFromError() => WidgetsResources.Get(nameof (BacklogPicker_NoBacklogsToChooseFromError));

    public static string BacklogPicker_NoBacklogsToChooseFromError(CultureInfo culture) => WidgetsResources.Get(nameof (BacklogPicker_NoBacklogsToChooseFromError), culture);

    public static string BacklogPicker_Watermark() => WidgetsResources.Get(nameof (BacklogPicker_Watermark));

    public static string BacklogPicker_Watermark(CultureInfo culture) => WidgetsResources.Get(nameof (BacklogPicker_Watermark), culture);

    public static string IterationPicker_Watermark() => WidgetsResources.Get(nameof (IterationPicker_Watermark));

    public static string IterationPicker_Watermark(CultureInfo culture) => WidgetsResources.Get(nameof (IterationPicker_Watermark), culture);

    public static string SwimlanePicker_Watermark() => WidgetsResources.Get(nameof (SwimlanePicker_Watermark));

    public static string SwimlanePicker_Watermark(CultureInfo culture) => WidgetsResources.Get(nameof (SwimlanePicker_Watermark), culture);

    public static string TeamPicker_Watermark() => WidgetsResources.Get(nameof (TeamPicker_Watermark));

    public static string TeamPicker_Watermark(CultureInfo culture) => WidgetsResources.Get(nameof (TeamPicker_Watermark), culture);

    public static string CumulativeFlowDiagram_ColumnOptionsLabel() => WidgetsResources.Get(nameof (CumulativeFlowDiagram_ColumnOptionsLabel));

    public static string CumulativeFlowDiagram_ColumnOptionsLabel(CultureInfo culture) => WidgetsResources.Get(nameof (CumulativeFlowDiagram_ColumnOptionsLabel), culture);

    public static string TeamDataService_FailedToFindAnyBoardsError() => WidgetsResources.Get(nameof (TeamDataService_FailedToFindAnyBoardsError));

    public static string TeamDataService_FailedToFindAnyBoardsError(CultureInfo culture) => WidgetsResources.Get(nameof (TeamDataService_FailedToFindAnyBoardsError), culture);

    public static string WitChart_ConfigurationErrorLoadingQueries() => WidgetsResources.Get(nameof (WitChart_ConfigurationErrorLoadingQueries));

    public static string WitChart_ConfigurationErrorLoadingQueries(CultureInfo culture) => WidgetsResources.Get(nameof (WitChart_ConfigurationErrorLoadingQueries), culture);

    public static string KanbanTimeControl_AverageDay() => WidgetsResources.Get(nameof (KanbanTimeControl_AverageDay));

    public static string KanbanTimeControl_AverageDay(CultureInfo culture) => WidgetsResources.Get(nameof (KanbanTimeControl_AverageDay), culture);

    public static string KanbanTime_PreviouslySelectedTeamNotFoundError() => WidgetsResources.Get(nameof (KanbanTime_PreviouslySelectedTeamNotFoundError));

    public static string KanbanTime_PreviouslySelectedTeamNotFoundError(CultureInfo culture) => WidgetsResources.Get(nameof (KanbanTime_PreviouslySelectedTeamNotFoundError), culture);

    public static string KanbanTime_PreviouslySelectedWitTypeNotFoundError() => WidgetsResources.Get(nameof (KanbanTime_PreviouslySelectedWitTypeNotFoundError));

    public static string KanbanTime_PreviouslySelectedWitTypeNotFoundError(CultureInfo culture) => WidgetsResources.Get(nameof (KanbanTime_PreviouslySelectedWitTypeNotFoundError), culture);

    public static string WorkItemTypePicker_NoWorkItemTypeSelectedError() => WidgetsResources.Get(nameof (WorkItemTypePicker_NoWorkItemTypeSelectedError));

    public static string WorkItemTypePicker_NoWorkItemTypeSelectedError(CultureInfo culture) => WidgetsResources.Get(nameof (WorkItemTypePicker_NoWorkItemTypeSelectedError), culture);

    public static string WorkItemTypePicker_NoWorkItemTypesToChooseFromError() => WidgetsResources.Get(nameof (WorkItemTypePicker_NoWorkItemTypesToChooseFromError));

    public static string WorkItemTypePicker_NoWorkItemTypesToChooseFromError(CultureInfo culture) => WidgetsResources.Get(nameof (WorkItemTypePicker_NoWorkItemTypesToChooseFromError), culture);

    public static string WorkItemTypePicker_SelectedWorkItemTypeNotFoundError() => WidgetsResources.Get(nameof (WorkItemTypePicker_SelectedWorkItemTypeNotFoundError));

    public static string WorkItemTypePicker_SelectedWorkItemTypeNotFoundError(CultureInfo culture) => WidgetsResources.Get(nameof (WorkItemTypePicker_SelectedWorkItemTypeNotFoundError), culture);

    public static string WorkItemTypePicker_Watermark() => WidgetsResources.Get(nameof (WorkItemTypePicker_Watermark));

    public static string WorkItemTypePicker_Watermark(CultureInfo culture) => WidgetsResources.Get(nameof (WorkItemTypePicker_Watermark), culture);

    public static string KanbanTime_TeamLabel() => WidgetsResources.Get(nameof (KanbanTime_TeamLabel));

    public static string KanbanTime_TeamLabel(CultureInfo culture) => WidgetsResources.Get(nameof (KanbanTime_TeamLabel), culture);

    public static string LeadTime_TitleFormat(object arg0) => WidgetsResources.Format(nameof (LeadTime_TitleFormat), arg0);

    public static string LeadTime_TitleFormat(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (LeadTime_TitleFormat), culture, arg0);

    public static string RollingPeriod_MinAllowedDaysErrorFormat(object arg0) => WidgetsResources.Format(nameof (RollingPeriod_MinAllowedDaysErrorFormat), arg0);

    public static string RollingPeriod_MinAllowedDaysErrorFormat(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (RollingPeriod_MinAllowedDaysErrorFormat), culture, arg0);

    public static string KanbanTime_CycleTime_SubTitle() => WidgetsResources.Get(nameof (KanbanTime_CycleTime_SubTitle));

    public static string KanbanTime_CycleTime_SubTitle(CultureInfo culture) => WidgetsResources.Get(nameof (KanbanTime_CycleTime_SubTitle), culture);

    public static string KanbanTime_LeadTime_SubTitle() => WidgetsResources.Get(nameof (KanbanTime_LeadTime_SubTitle));

    public static string KanbanTime_LeadTime_SubTitle(CultureInfo culture) => WidgetsResources.Get(nameof (KanbanTime_LeadTime_SubTitle), culture);

    public static string KanbanTime_TooManyWorkItemTooltip() => WidgetsResources.Get(nameof (KanbanTime_TooManyWorkItemTooltip));

    public static string KanbanTime_TooManyWorkItemTooltip(CultureInfo culture) => WidgetsResources.Get(nameof (KanbanTime_TooManyWorkItemTooltip), culture);

    public static string CycleTime_DefaultTitle() => WidgetsResources.Get(nameof (CycleTime_DefaultTitle));

    public static string CycleTime_DefaultTitle(CultureInfo culture) => WidgetsResources.Get(nameof (CycleTime_DefaultTitle), culture);

    public static string CycleTime_TitleFormat(object arg0) => WidgetsResources.Format(nameof (CycleTime_TitleFormat), arg0);

    public static string CycleTime_TitleFormat(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (CycleTime_TitleFormat), culture, arg0);

    public static string KanbanTime_CycleTime_TempQuery() => WidgetsResources.Get(nameof (KanbanTime_CycleTime_TempQuery));

    public static string KanbanTime_CycleTime_TempQuery(CultureInfo culture) => WidgetsResources.Get(nameof (KanbanTime_CycleTime_TempQuery), culture);

    public static string KanbanTime_LeadTime_TempQuery() => WidgetsResources.Get(nameof (KanbanTime_LeadTime_TempQuery));

    public static string KanbanTime_LeadTime_TempQuery(CultureInfo culture) => WidgetsResources.Get(nameof (KanbanTime_LeadTime_TempQuery), culture);

    public static string KanbanTime_WorkItemTypeLabel() => WidgetsResources.Get(nameof (KanbanTime_WorkItemTypeLabel));

    public static string KanbanTime_WorkItemTypeLabel(CultureInfo culture) => WidgetsResources.Get(nameof (KanbanTime_WorkItemTypeLabel), culture);

    public static string LeadTime_DefaultTitle() => WidgetsResources.Get(nameof (LeadTime_DefaultTitle));

    public static string LeadTime_DefaultTitle(CultureInfo culture) => WidgetsResources.Get(nameof (LeadTime_DefaultTitle), culture);

    public static string LeadTime_PreviouslySelectedBacklogNotFoundError() => WidgetsResources.Get(nameof (LeadTime_PreviouslySelectedBacklogNotFoundError));

    public static string LeadTime_PreviouslySelectedBacklogNotFoundError(CultureInfo culture) => WidgetsResources.Get(nameof (LeadTime_PreviouslySelectedBacklogNotFoundError), culture);

    public static string Analytics_FirstUseMessage() => WidgetsResources.Get(nameof (Analytics_FirstUseMessage));

    public static string Analytics_FirstUseMessage(CultureInfo culture) => WidgetsResources.Get(nameof (Analytics_FirstUseMessage), culture);

    public static string Analytics_FirstUseSubMessage() => WidgetsResources.Get(nameof (Analytics_FirstUseSubMessage));

    public static string Analytics_FirstUseSubMessage(CultureInfo culture) => WidgetsResources.Get(nameof (Analytics_FirstUseSubMessage), culture);

    public static string ItemListControl_TooltipTextFormat(object arg0, object arg1) => WidgetsResources.Format(nameof (ItemListControl_TooltipTextFormat), arg0, arg1);

    public static string ItemListControl_TooltipTextFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (ItemListControl_TooltipTextFormat), culture, arg0, arg1);
    }

    public static string CumulativeFlowDiagram_emptyAltTextDescription() => WidgetsResources.Get(nameof (CumulativeFlowDiagram_emptyAltTextDescription));

    public static string CumulativeFlowDiagram_emptyAltTextDescription(CultureInfo culture) => WidgetsResources.Get(nameof (CumulativeFlowDiagram_emptyAltTextDescription), culture);

    public static string BaseChart_emptyAltTextDescription() => WidgetsResources.Get(nameof (BaseChart_emptyAltTextDescription));

    public static string BaseChart_emptyAltTextDescription(CultureInfo culture) => WidgetsResources.Get(nameof (BaseChart_emptyAltTextDescription), culture);

    public static string KanbanTime_TooManyWorkItemsAriaLabel() => WidgetsResources.Get(nameof (KanbanTime_TooManyWorkItemsAriaLabel));

    public static string KanbanTime_TooManyWorkItemsAriaLabel(CultureInfo culture) => WidgetsResources.Get(nameof (KanbanTime_TooManyWorkItemsAriaLabel), culture);

    public static string QueryScalar_OperatorComboLabel() => WidgetsResources.Get(nameof (QueryScalar_OperatorComboLabel));

    public static string QueryScalar_OperatorComboLabel(CultureInfo culture) => WidgetsResources.Get(nameof (QueryScalar_OperatorComboLabel), culture);

    public static string QueryScalar_OperatorLabel() => WidgetsResources.Get(nameof (QueryScalar_OperatorLabel));

    public static string QueryScalar_OperatorLabel(CultureInfo culture) => WidgetsResources.Get(nameof (QueryScalar_OperatorLabel), culture);

    public static string QueryScalar_RuleColorLabel() => WidgetsResources.Get(nameof (QueryScalar_RuleColorLabel));

    public static string QueryScalar_RuleColorLabel(CultureInfo culture) => WidgetsResources.Get(nameof (QueryScalar_RuleColorLabel), culture);

    public static string QueryScalar_RuleLabel() => WidgetsResources.Get(nameof (QueryScalar_RuleLabel));

    public static string QueryScalar_RuleLabel(CultureInfo culture) => WidgetsResources.Get(nameof (QueryScalar_RuleLabel), culture);

    public static string QueryScalar_RuleThresholdLabel() => WidgetsResources.Get(nameof (QueryScalar_RuleThresholdLabel));

    public static string QueryScalar_RuleThresholdLabel(CultureInfo culture) => WidgetsResources.Get(nameof (QueryScalar_RuleThresholdLabel), culture);

    public static string ItemListControl_FilterAriaDescriptionTextFormat(object arg0) => WidgetsResources.Format(nameof (ItemListControl_FilterAriaDescriptionTextFormat), arg0);

    public static string ItemListControl_FilterAriaDescriptionTextFormat(
      object arg0,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (ItemListControl_FilterAriaDescriptionTextFormat), culture, arg0);
    }

    public static string VelocityChart_CompletedLate_StateName() => WidgetsResources.Get(nameof (VelocityChart_CompletedLate_StateName));

    public static string VelocityChart_CompletedLate_StateName(CultureInfo culture) => WidgetsResources.Get(nameof (VelocityChart_CompletedLate_StateName), culture);

    public static string VelocityChart_Completed_StateName() => WidgetsResources.Get(nameof (VelocityChart_Completed_StateName));

    public static string VelocityChart_Completed_StateName(CultureInfo culture) => WidgetsResources.Get(nameof (VelocityChart_Completed_StateName), culture);

    public static string VelocityChart_Incomplete_StateName() => WidgetsResources.Get(nameof (VelocityChart_Incomplete_StateName));

    public static string VelocityChart_Incomplete_StateName(CultureInfo culture) => WidgetsResources.Get(nameof (VelocityChart_Incomplete_StateName), culture);

    public static string VelocityChart_Planned_StateName() => WidgetsResources.Get(nameof (VelocityChart_Planned_StateName));

    public static string VelocityChart_Planned_StateName(CultureInfo culture) => WidgetsResources.Get(nameof (VelocityChart_Planned_StateName), culture);

    public static string KanbanTime_DefaultSettings_FailedToFindAnyBacklogsError() => WidgetsResources.Get(nameof (KanbanTime_DefaultSettings_FailedToFindAnyBacklogsError));

    public static string KanbanTime_DefaultSettings_FailedToFindAnyBacklogsError(CultureInfo culture) => WidgetsResources.Get(nameof (KanbanTime_DefaultSettings_FailedToFindAnyBacklogsError), culture);

    public static string VelocityWidget_IterationsSubtitle(object arg0) => WidgetsResources.Format(nameof (VelocityWidget_IterationsSubtitle), arg0);

    public static string VelocityWidget_IterationsSubtitle(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (VelocityWidget_IterationsSubtitle), culture, arg0);

    public static string VelocityWidget_NoIterations() => WidgetsResources.Get(nameof (VelocityWidget_NoIterations));

    public static string VelocityWidget_NoIterations(CultureInfo culture) => WidgetsResources.Get(nameof (VelocityWidget_NoIterations), culture);

    public static string VelocityWidget_SingleIterationSubtitle() => WidgetsResources.Get(nameof (VelocityWidget_SingleIterationSubtitle));

    public static string VelocityWidget_SingleIterationSubtitle(CultureInfo culture) => WidgetsResources.Get(nameof (VelocityWidget_SingleIterationSubtitle), culture);

    public static string Velocity_DefaultWidgetName() => WidgetsResources.Get(nameof (Velocity_DefaultWidgetName));

    public static string Velocity_DefaultWidgetName(CultureInfo culture) => WidgetsResources.Get(nameof (Velocity_DefaultWidgetName), culture);

    public static string VelocityWidget_AverageVelocity() => WidgetsResources.Get(nameof (VelocityWidget_AverageVelocity));

    public static string VelocityWidget_AverageVelocity(CultureInfo culture) => WidgetsResources.Get(nameof (VelocityWidget_AverageVelocity), culture);

    public static string VelocityWidget_CountOfWorkItems() => WidgetsResources.Get(nameof (VelocityWidget_CountOfWorkItems));

    public static string VelocityWidget_CountOfWorkItems(CultureInfo culture) => WidgetsResources.Get(nameof (VelocityWidget_CountOfWorkItems), culture);

    public static string VelocityConfig_VelocityLabel() => WidgetsResources.Get(nameof (VelocityConfig_VelocityLabel));

    public static string VelocityConfig_VelocityLabel(CultureInfo culture) => WidgetsResources.Get(nameof (VelocityConfig_VelocityLabel), culture);

    public static string VelocityConfig_NumberIterationsLabel() => WidgetsResources.Get(nameof (VelocityConfig_NumberIterationsLabel));

    public static string VelocityConfig_NumberIterationsLabel(CultureInfo culture) => WidgetsResources.Get(nameof (VelocityConfig_NumberIterationsLabel), culture);

    public static string VelocityConfig_AdvancedFeaturesLabel() => WidgetsResources.Get(nameof (VelocityConfig_AdvancedFeaturesLabel));

    public static string VelocityConfig_AdvancedFeaturesLabel(CultureInfo culture) => WidgetsResources.Get(nameof (VelocityConfig_AdvancedFeaturesLabel), culture);

    public static string VelocityConfig_DisplayPlannedWorkHeader() => WidgetsResources.Get(nameof (VelocityConfig_DisplayPlannedWorkHeader));

    public static string VelocityConfig_DisplayPlannedWorkHeader(CultureInfo culture) => WidgetsResources.Get(nameof (VelocityConfig_DisplayPlannedWorkHeader), culture);

    public static string VelocityConfig_PlannedWorkBodyText() => WidgetsResources.Get(nameof (VelocityConfig_PlannedWorkBodyText));

    public static string VelocityConfig_PlannedWorkBodyText(CultureInfo culture) => WidgetsResources.Get(nameof (VelocityConfig_PlannedWorkBodyText), culture);

    public static string VelocityConfig_DisplayCompletedLateWorkHeader() => WidgetsResources.Get(nameof (VelocityConfig_DisplayCompletedLateWorkHeader));

    public static string VelocityConfig_DisplayCompletedLateWorkHeader(CultureInfo culture) => WidgetsResources.Get(nameof (VelocityConfig_DisplayCompletedLateWorkHeader), culture);

    public static string VelocityConfig_LateWorkBodyText() => WidgetsResources.Get(nameof (VelocityConfig_LateWorkBodyText));

    public static string VelocityConfig_LateWorkBodyText(CultureInfo culture) => WidgetsResources.Get(nameof (VelocityConfig_LateWorkBodyText), culture);

    public static string AnalyticsWidgetsCommonConfig_TeamLabel() => WidgetsResources.Get(nameof (AnalyticsWidgetsCommonConfig_TeamLabel));

    public static string AnalyticsWidgetsCommonConfig_TeamLabel(CultureInfo culture) => WidgetsResources.Get(nameof (AnalyticsWidgetsCommonConfig_TeamLabel), culture);

    public static string AnalyticsWidgetsCommonConfig_WorkItemsLabel() => WidgetsResources.Get(nameof (AnalyticsWidgetsCommonConfig_WorkItemsLabel));

    public static string AnalyticsWidgetsCommonConfig_WorkItemsLabel(CultureInfo culture) => WidgetsResources.Get(nameof (AnalyticsWidgetsCommonConfig_WorkItemsLabel), culture);

    public static string AnalyticsWidgetsCommonConfig_EnableAdvancedFeaturesMessage() => WidgetsResources.Get(nameof (AnalyticsWidgetsCommonConfig_EnableAdvancedFeaturesMessage));

    public static string AnalyticsWidgetsCommonConfig_EnableAdvancedFeaturesMessage(
      CultureInfo culture)
    {
      return WidgetsResources.Get(nameof (AnalyticsWidgetsCommonConfig_EnableAdvancedFeaturesMessage), culture);
    }

    public static string MessageCard_SetIterationDates() => WidgetsResources.Get(nameof (MessageCard_SetIterationDates));

    public static string MessageCard_SetIterationDates(CultureInfo culture) => WidgetsResources.Get(nameof (MessageCard_SetIterationDates), culture);

    public static string VelocityWidget_SetIterationDatesAriaLabel() => WidgetsResources.Get(nameof (VelocityWidget_SetIterationDatesAriaLabel));

    public static string VelocityWidget_SetIterationDatesAriaLabel(CultureInfo culture) => WidgetsResources.Get(nameof (VelocityWidget_SetIterationDatesAriaLabel), culture);

    public static string VelocityWidget_SetIterationDatesLink() => WidgetsResources.Get(nameof (VelocityWidget_SetIterationDatesLink));

    public static string VelocityWidget_SetIterationDatesLink(CultureInfo culture) => WidgetsResources.Get(nameof (VelocityWidget_SetIterationDatesLink), culture);

    public static string MessageCard_AxFaultInMessage() => WidgetsResources.Get(nameof (MessageCard_AxFaultInMessage));

    public static string MessageCard_AxFaultInMessage(CultureInfo culture) => WidgetsResources.Get(nameof (MessageCard_AxFaultInMessage), culture);

    public static string VelocityWidget_LearnMore() => WidgetsResources.Get(nameof (VelocityWidget_LearnMore));

    public static string VelocityWidget_LearnMore(CultureInfo culture) => WidgetsResources.Get(nameof (VelocityWidget_LearnMore), culture);

    public static string VelocityWidget_Loading() => WidgetsResources.Get(nameof (VelocityWidget_Loading));

    public static string VelocityWidget_Loading(CultureInfo culture) => WidgetsResources.Get(nameof (VelocityWidget_Loading), culture);

    public static string MessageCard_Unconfigured() => WidgetsResources.Get(nameof (MessageCard_Unconfigured));

    public static string MessageCard_Unconfigured(CultureInfo culture) => WidgetsResources.Get(nameof (MessageCard_Unconfigured), culture);

    public static string VelocityWidget_IterationsErrorFormat(object arg0) => WidgetsResources.Format(nameof (VelocityWidget_IterationsErrorFormat), arg0);

    public static string VelocityWidget_IterationsErrorFormat(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (VelocityWidget_IterationsErrorFormat), culture, arg0);

    public static string VelocityWidget_MetastateErrorFormat(object arg0) => WidgetsResources.Format(nameof (VelocityWidget_MetastateErrorFormat), arg0);

    public static string VelocityWidget_MetastateErrorFormat(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (VelocityWidget_MetastateErrorFormat), culture, arg0);

    public static string ModernWidget_WorkItemTypeErrorFormat(object arg0) => WidgetsResources.Format(nameof (ModernWidget_WorkItemTypeErrorFormat), arg0);

    public static string ModernWidget_WorkItemTypeErrorFormat(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (ModernWidget_WorkItemTypeErrorFormat), culture, arg0);

    public static string ModernWidget_PluralBacklogNameErrorFormat(object arg0) => WidgetsResources.Format(nameof (ModernWidget_PluralBacklogNameErrorFormat), arg0);

    public static string ModernWidget_PluralBacklogNameErrorFormat(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (ModernWidget_PluralBacklogNameErrorFormat), culture, arg0);

    public static string VelocityWidget_WidgetErrorFormat() => WidgetsResources.Get(nameof (VelocityWidget_WidgetErrorFormat));

    public static string VelocityWidget_WidgetErrorFormat(CultureInfo culture) => WidgetsResources.Get(nameof (VelocityWidget_WidgetErrorFormat), culture);

    public static string VelocityWidget_ErrorDetails() => WidgetsResources.Get(nameof (VelocityWidget_ErrorDetails));

    public static string VelocityWidget_ErrorDetails(CultureInfo culture) => WidgetsResources.Get(nameof (VelocityWidget_ErrorDetails), culture);

    public static string VelocityWidget_ErrorDetailsAriaLabel() => WidgetsResources.Get(nameof (VelocityWidget_ErrorDetailsAriaLabel));

    public static string VelocityWidget_ErrorDetailsAriaLabel(CultureInfo culture) => WidgetsResources.Get(nameof (VelocityWidget_ErrorDetailsAriaLabel), culture);

    public static string VelocityWidget_WidgetError() => WidgetsResources.Get(nameof (VelocityWidget_WidgetError));

    public static string VelocityWidget_WidgetError(CultureInfo culture) => WidgetsResources.Get(nameof (VelocityWidget_WidgetError), culture);

    public static string ModernWidget_TeamSettingsErrorFormat(object arg0) => WidgetsResources.Format(nameof (ModernWidget_TeamSettingsErrorFormat), arg0);

    public static string ModernWidget_TeamSettingsErrorFormat(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (ModernWidget_TeamSettingsErrorFormat), culture, arg0);

    public static string VelocityWidget_LearnMoreLink() => WidgetsResources.Get(nameof (VelocityWidget_LearnMoreLink));

    public static string VelocityWidget_LearnMoreLink(CultureInfo culture) => WidgetsResources.Get(nameof (VelocityWidget_LearnMoreLink), culture);

    public static string AggregationConfigurationControl_Count() => WidgetsResources.Get(nameof (AggregationConfigurationControl_Count));

    public static string AggregationConfigurationControl_Count(CultureInfo culture) => WidgetsResources.Get(nameof (AggregationConfigurationControl_Count), culture);

    public static string AggregationConfigurationControl_Of_Label() => WidgetsResources.Get(nameof (AggregationConfigurationControl_Of_Label));

    public static string AggregationConfigurationControl_Of_Label(CultureInfo culture) => WidgetsResources.Get(nameof (AggregationConfigurationControl_Of_Label), culture);

    public static string AggregationConfigurationControl_Sum() => WidgetsResources.Get(nameof (AggregationConfigurationControl_Sum));

    public static string AggregationConfigurationControl_Sum(CultureInfo culture) => WidgetsResources.Get(nameof (AggregationConfigurationControl_Sum), culture);

    public static string AggregationConfigurationControl_WorkItems() => WidgetsResources.Get(nameof (AggregationConfigurationControl_WorkItems));

    public static string AggregationConfigurationControl_WorkItems(CultureInfo culture) => WidgetsResources.Get(nameof (AggregationConfigurationControl_WorkItems), culture);

    public static string VelocityConfig_NameFormat(object arg0) => WidgetsResources.Format(nameof (VelocityConfig_NameFormat), arg0);

    public static string VelocityConfig_NameFormat(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (VelocityConfig_NameFormat), culture, arg0);

    public static string NumericInput_InvalidInputError() => WidgetsResources.Get(nameof (NumericInput_InvalidInputError));

    public static string NumericInput_InvalidInputError(CultureInfo culture) => WidgetsResources.Get(nameof (NumericInput_InvalidInputError), culture);

    public static string NumericInput_MaxAllowableIterationsErrorFormat(object arg0) => WidgetsResources.Format(nameof (NumericInput_MaxAllowableIterationsErrorFormat), arg0);

    public static string NumericInput_MaxAllowableIterationsErrorFormat(
      object arg0,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (NumericInput_MaxAllowableIterationsErrorFormat), culture, arg0);
    }

    public static string NumericInput_MinAllowableIterationsErrorFormat(object arg0) => WidgetsResources.Format(nameof (NumericInput_MinAllowableIterationsErrorFormat), arg0);

    public static string NumericInput_MinAllowableIterationsErrorFormat(
      object arg0,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (NumericInput_MinAllowableIterationsErrorFormat), culture, arg0);
    }

    public static string AggregationConfigurationControl_FieldNotFoundError() => WidgetsResources.Get(nameof (AggregationConfigurationControl_FieldNotFoundError));

    public static string AggregationConfigurationControl_FieldNotFoundError(CultureInfo culture) => WidgetsResources.Get(nameof (AggregationConfigurationControl_FieldNotFoundError), culture);

    public static string AggregationConfigurationControl_ModeNotFoundError() => WidgetsResources.Get(nameof (AggregationConfigurationControl_ModeNotFoundError));

    public static string AggregationConfigurationControl_ModeNotFoundError(CultureInfo culture) => WidgetsResources.Get(nameof (AggregationConfigurationControl_ModeNotFoundError), culture);

    public static string Burndown_DefaultWidgetName() => WidgetsResources.Get(nameof (Burndown_DefaultWidgetName));

    public static string Burndown_DefaultWidgetName(CultureInfo culture) => WidgetsResources.Get(nameof (Burndown_DefaultWidgetName), culture);

    public static string BurndownConfig_Name() => WidgetsResources.Get(nameof (BurndownConfig_Name));

    public static string BurndownConfig_Name(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownConfig_Name), culture);

    public static string BurnupConfig_Name() => WidgetsResources.Get(nameof (BurnupConfig_Name));

    public static string BurnupConfig_Name(CultureInfo culture) => WidgetsResources.Get(nameof (BurnupConfig_Name), culture);

    public static string MonteCarloConfig_Name() => WidgetsResources.Get(nameof (MonteCarloConfig_Name));

    public static string MonteCarloConfig_Name(CultureInfo culture) => WidgetsResources.Get(nameof (MonteCarloConfig_Name), culture);

    public static string BurnConfig_IncludeBugsLabelFormat(object arg0) => WidgetsResources.Format(nameof (BurnConfig_IncludeBugsLabelFormat), arg0);

    public static string BurnConfig_IncludeBugsLabelFormat(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (BurnConfig_IncludeBugsLabelFormat), culture, arg0);

    public static string BurndownWidget_StackByHeader() => WidgetsResources.Get(nameof (BurndownWidget_StackByHeader));

    public static string BurndownWidget_StackByHeader(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownWidget_StackByHeader), culture);

    public static string BurndownWidget_StackByLabel() => WidgetsResources.Get(nameof (BurndownWidget_StackByLabel));

    public static string BurndownWidget_StackByLabel(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownWidget_StackByLabel), culture);

    public static string BurnupWidget_StackByLabel() => WidgetsResources.Get(nameof (BurnupWidget_StackByLabel));

    public static string BurnupWidget_StackByLabel(CultureInfo culture) => WidgetsResources.Get(nameof (BurnupWidget_StackByLabel), culture);

    public static string ProjectPicker_Watermark() => WidgetsResources.Get(nameof (ProjectPicker_Watermark));

    public static string ProjectPicker_Watermark(CultureInfo culture) => WidgetsResources.Get(nameof (ProjectPicker_Watermark), culture);

    public static string ModePickerAriaLabel() => WidgetsResources.Get(nameof (ModePickerAriaLabel));

    public static string ModePickerAriaLabel(CultureInfo culture) => WidgetsResources.Get(nameof (ModePickerAriaLabel), culture);

    public static string FieldPickerAriaLabel() => WidgetsResources.Get(nameof (FieldPickerAriaLabel));

    public static string FieldPickerAriaLabel(CultureInfo culture) => WidgetsResources.Get(nameof (FieldPickerAriaLabel), culture);

    public static string BurndownWidget_TeamsHeader() => WidgetsResources.Get(nameof (BurndownWidget_TeamsHeader));

    public static string BurndownWidget_TeamsHeader(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownWidget_TeamsHeader), culture);

    public static string ProjectPicker_NoProjectSelectedError() => WidgetsResources.Get(nameof (ProjectPicker_NoProjectSelectedError));

    public static string ProjectPicker_NoProjectSelectedError(CultureInfo culture) => WidgetsResources.Get(nameof (ProjectPicker_NoProjectSelectedError), culture);

    public static string ProjectPicker_SelectedProjectNotFoundError() => WidgetsResources.Get(nameof (ProjectPicker_SelectedProjectNotFoundError));

    public static string ProjectPicker_SelectedProjectNotFoundError(CultureInfo culture) => WidgetsResources.Get(nameof (ProjectPicker_SelectedProjectNotFoundError), culture);

    public static string ProjectTeamPickerAriaLabel() => WidgetsResources.Get(nameof (ProjectTeamPickerAriaLabel));

    public static string ProjectTeamPickerAriaLabel(CultureInfo culture) => WidgetsResources.Get(nameof (ProjectTeamPickerAriaLabel), culture);

    public static string ProjectTeamPicker_RemoveRowButtonAriaLabel() => WidgetsResources.Get(nameof (ProjectTeamPicker_RemoveRowButtonAriaLabel));

    public static string ProjectTeamPicker_RemoveRowButtonAriaLabel(CultureInfo culture) => WidgetsResources.Get(nameof (ProjectTeamPicker_RemoveRowButtonAriaLabel), culture);

    public static string BurndownWidget_TeamHeaderTooltip() => WidgetsResources.Get(nameof (BurndownWidget_TeamHeaderTooltip));

    public static string BurndownWidget_TeamHeaderTooltip(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownWidget_TeamHeaderTooltip), culture);

    public static string ProjectTeamPickerList_MaxTeamsAdded() => WidgetsResources.Get(nameof (ProjectTeamPickerList_MaxTeamsAdded));

    public static string ProjectTeamPickerList_MaxTeamsAdded(CultureInfo culture) => WidgetsResources.Get(nameof (ProjectTeamPickerList_MaxTeamsAdded), culture);

    public static string ProjectTeamPickerList_AddTeam() => WidgetsResources.Get(nameof (ProjectTeamPickerList_AddTeam));

    public static string ProjectTeamPickerList_AddTeam(CultureInfo culture) => WidgetsResources.Get(nameof (ProjectTeamPickerList_AddTeam), culture);

    public static string BurndownWidget_RemainingEffortSeriesName() => WidgetsResources.Get(nameof (BurndownWidget_RemainingEffortSeriesName));

    public static string BurndownWidget_RemainingEffortSeriesName(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownWidget_RemainingEffortSeriesName), culture);

    public static string BurndownWidget_CompletedEffortSeriesName() => WidgetsResources.Get(nameof (BurndownWidget_CompletedEffortSeriesName));

    public static string BurndownWidget_CompletedEffortSeriesName(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownWidget_CompletedEffortSeriesName), culture);

    public static string BurndownWidget_BurndownTrendlineSeriesName() => WidgetsResources.Get(nameof (BurndownWidget_BurndownTrendlineSeriesName));

    public static string BurndownWidget_BurndownTrendlineSeriesName(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownWidget_BurndownTrendlineSeriesName), culture);

    public static string BurnupWidget_BurnupTrendlineSeriesName() => WidgetsResources.Get(nameof (BurnupWidget_BurnupTrendlineSeriesName));

    public static string BurnupWidget_BurnupTrendlineSeriesName(CultureInfo culture) => WidgetsResources.Get(nameof (BurnupWidget_BurnupTrendlineSeriesName), culture);

    public static string BurndownWidget_TotalScopeTrendlineSeriesName() => WidgetsResources.Get(nameof (BurndownWidget_TotalScopeTrendlineSeriesName));

    public static string BurndownWidget_TotalScopeTrendlineSeriesName(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownWidget_TotalScopeTrendlineSeriesName), culture);

    public static string BurndownWidget_HeroDescription() => WidgetsResources.Get(nameof (BurndownWidget_HeroDescription));

    public static string BurndownWidget_HeroDescription(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownWidget_HeroDescription), culture);

    public static string BurndownWidget_TotalScopeIncreaseMetricName() => WidgetsResources.Get(nameof (BurndownWidget_TotalScopeIncreaseMetricName));

    public static string BurndownWidget_TotalScopeIncreaseMetricName(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownWidget_TotalScopeIncreaseMetricName), culture);

    public static string BurndownWidget_CompletedMetricName() => WidgetsResources.Get(nameof (BurndownWidget_CompletedMetricName));

    public static string BurndownWidget_CompletedMetricName(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownWidget_CompletedMetricName), culture);

    public static string BurndownWidget_AverageBurndownMetricName() => WidgetsResources.Get(nameof (BurndownWidget_AverageBurndownMetricName));

    public static string BurndownWidget_AverageBurndownMetricName(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownWidget_AverageBurndownMetricName), culture);

    public static string BurndownWidget_AverageBurnupMetricName() => WidgetsResources.Get(nameof (BurndownWidget_AverageBurnupMetricName));

    public static string BurndownWidget_AverageBurnupMetricName(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownWidget_AverageBurnupMetricName), culture);

    public static string BurndownWidget_ItemsNotEstimatedMetricName() => WidgetsResources.Get(nameof (BurndownWidget_ItemsNotEstimatedMetricName));

    public static string BurndownWidget_ItemsNotEstimatedMetricName(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownWidget_ItemsNotEstimatedMetricName), culture);

    public static string BurndownWidget_ProjectCompletionForecastFormat(object arg0) => WidgetsResources.Format(nameof (BurndownWidget_ProjectCompletionForecastFormat), arg0);

    public static string BurndownWidget_ProjectCompletionForecastFormat(
      object arg0,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (BurndownWidget_ProjectCompletionForecastFormat), culture, arg0);
    }

    public static string BurndownWidget_ProjectCompletionForecastOutsideChartFormat(
      object arg0,
      object arg1,
      object arg2)
    {
      return WidgetsResources.Format(nameof (BurndownWidget_ProjectCompletionForecastOutsideChartFormat), arg0, arg1, arg2);
    }

    public static string BurndownWidget_ProjectCompletionForecastOutsideChartFormat(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (BurndownWidget_ProjectCompletionForecastOutsideChartFormat), culture, arg0, arg1, arg2);
    }

    public static string TimePeriod_DaySingular() => WidgetsResources.Get(nameof (TimePeriod_DaySingular));

    public static string TimePeriod_DaySingular(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_DaySingular), culture);

    public static string TimePeriod_DayPlural() => WidgetsResources.Get(nameof (TimePeriod_DayPlural));

    public static string TimePeriod_DayPlural(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_DayPlural), culture);

    public static string TimePeriod_WeekSingular() => WidgetsResources.Get(nameof (TimePeriod_WeekSingular));

    public static string TimePeriod_WeekSingular(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_WeekSingular), culture);

    public static string TimePeriod_WeekPlural() => WidgetsResources.Get(nameof (TimePeriod_WeekPlural));

    public static string TimePeriod_WeekPlural(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_WeekPlural), culture);

    public static string TimePeriod_MonthSingular() => WidgetsResources.Get(nameof (TimePeriod_MonthSingular));

    public static string TimePeriod_MonthSingular(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_MonthSingular), culture);

    public static string TimePeriod_MonthPlural() => WidgetsResources.Get(nameof (TimePeriod_MonthPlural));

    public static string TimePeriod_MonthPlural(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_MonthPlural), culture);

    public static string TimePeriod_IterationSingular() => WidgetsResources.Get(nameof (TimePeriod_IterationSingular));

    public static string TimePeriod_IterationSingular(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_IterationSingular), culture);

    public static string TimePeriod_IterationPlural() => WidgetsResources.Get(nameof (TimePeriod_IterationPlural));

    public static string TimePeriod_IterationPlural(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_IterationPlural), culture);

    public static string BurndownWidget_AdvancedFeaturesHeader() => WidgetsResources.Get(nameof (BurndownWidget_AdvancedFeaturesHeader));

    public static string BurndownWidget_AdvancedFeaturesHeader(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownWidget_AdvancedFeaturesHeader), culture);

    public static string MonteCarloWidget_AdvancedFeaturesShowStatisticalProbabilities() => WidgetsResources.Get(nameof (MonteCarloWidget_AdvancedFeaturesShowStatisticalProbabilities));

    public static string MonteCarloWidget_AdvancedFeaturesShowStatisticalProbabilities(
      CultureInfo culture)
    {
      return WidgetsResources.Get(nameof (MonteCarloWidget_AdvancedFeaturesShowStatisticalProbabilities), culture);
    }

    public static string MonteCarloWidget_AdvancedFeaturesUseDurationsCheckboxLabel() => WidgetsResources.Get(nameof (MonteCarloWidget_AdvancedFeaturesUseDurationsCheckboxLabel));

    public static string MonteCarloWidget_AdvancedFeaturesUseDurationsCheckboxLabel(
      CultureInfo culture)
    {
      return WidgetsResources.Get(nameof (MonteCarloWidget_AdvancedFeaturesUseDurationsCheckboxLabel), culture);
    }

    public static string MonteCarloWidget_yAxis2Label() => WidgetsResources.Get(nameof (MonteCarloWidget_yAxis2Label));

    public static string MonteCarloWidget_yAxis2Label(CultureInfo culture) => WidgetsResources.Get(nameof (MonteCarloWidget_yAxis2Label), culture);

    public static string MonteCarloWidget_SimulationLabel() => WidgetsResources.Get(nameof (MonteCarloWidget_SimulationLabel));

    public static string MonteCarloWidget_SimulationLabel(CultureInfo culture) => WidgetsResources.Get(nameof (MonteCarloWidget_SimulationLabel), culture);

    public static string MonteCarloWidget_CompleteDateLabel() => WidgetsResources.Get(nameof (MonteCarloWidget_CompleteDateLabel));

    public static string MonteCarloWidget_CompleteDateLabel(CultureInfo culture) => WidgetsResources.Get(nameof (MonteCarloWidget_CompleteDateLabel), culture);

    public static string MonteCarloWidget_CompleteDurationLabel() => WidgetsResources.Get(nameof (MonteCarloWidget_CompleteDurationLabel));

    public static string MonteCarloWidget_CompleteDurationLabel(CultureInfo culture) => WidgetsResources.Get(nameof (MonteCarloWidget_CompleteDurationLabel), culture);

    public static string MonteCarloWidget_ProjectionConfidenceLevelLabel(object arg0) => WidgetsResources.Format(nameof (MonteCarloWidget_ProjectionConfidenceLevelLabel), arg0);

    public static string MonteCarloWidget_ProjectionConfidenceLevelLabel(
      object arg0,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (MonteCarloWidget_ProjectionConfidenceLevelLabel), culture, arg0);
    }

    public static string MonteCarloWidget_NumberOfWorkItemsLabel() => WidgetsResources.Get(nameof (MonteCarloWidget_NumberOfWorkItemsLabel));

    public static string MonteCarloWidget_NumberOfWorkItemsLabel(CultureInfo culture) => WidgetsResources.Get(nameof (MonteCarloWidget_NumberOfWorkItemsLabel), culture);

    public static string MonteCarloWidget_NumberOfWorkItemsDescriptionLabel() => WidgetsResources.Get(nameof (MonteCarloWidget_NumberOfWorkItemsDescriptionLabel));

    public static string MonteCarloWidget_NumberOfWorkItemsDescriptionLabel(CultureInfo culture) => WidgetsResources.Get(nameof (MonteCarloWidget_NumberOfWorkItemsDescriptionLabel), culture);

    public static string MonteCarloWidget_NumberOfWorkItemsNumbersOnlyErrorMessage() => WidgetsResources.Get(nameof (MonteCarloWidget_NumberOfWorkItemsNumbersOnlyErrorMessage));

    public static string MonteCarloWidget_NumberOfWorkItemsNumbersOnlyErrorMessage(
      CultureInfo culture)
    {
      return WidgetsResources.Get(nameof (MonteCarloWidget_NumberOfWorkItemsNumbersOnlyErrorMessage), culture);
    }

    public static string MonteCarloWidget_NumberOfWorkItemsNumberTooHighErrorMessage(object arg0) => WidgetsResources.Format(nameof (MonteCarloWidget_NumberOfWorkItemsNumberTooHighErrorMessage), arg0);

    public static string MonteCarloWidget_NumberOfWorkItemsNumberTooHighErrorMessage(
      object arg0,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (MonteCarloWidget_NumberOfWorkItemsNumberTooHighErrorMessage), culture, arg0);
    }

    public static string BurndownWidget_AdvancedFeaturesBurndownTrendlineLabel() => WidgetsResources.Get(nameof (BurndownWidget_AdvancedFeaturesBurndownTrendlineLabel));

    public static string BurndownWidget_AdvancedFeaturesBurndownTrendlineLabel(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownWidget_AdvancedFeaturesBurndownTrendlineLabel), culture);

    public static string BurnupWidget_AdvancedFeaturesBurnupTrendlineLabel() => WidgetsResources.Get(nameof (BurnupWidget_AdvancedFeaturesBurnupTrendlineLabel));

    public static string BurnupWidget_AdvancedFeaturesBurnupTrendlineLabel(CultureInfo culture) => WidgetsResources.Get(nameof (BurnupWidget_AdvancedFeaturesBurnupTrendlineLabel), culture);

    public static string BurndownWidget_AdvancedFeaturesScopeTrendlineLabel() => WidgetsResources.Get(nameof (BurndownWidget_AdvancedFeaturesScopeTrendlineLabel));

    public static string BurndownWidget_AdvancedFeaturesScopeTrendlineLabel(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownWidget_AdvancedFeaturesScopeTrendlineLabel), culture);

    public static string BurndownWidget_AdvancedFeaturesCompletedWorkLabel() => WidgetsResources.Get(nameof (BurndownWidget_AdvancedFeaturesCompletedWorkLabel));

    public static string BurndownWidget_AdvancedFeaturesCompletedWorkLabel(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownWidget_AdvancedFeaturesCompletedWorkLabel), culture);

    public static string BurnupWidget_AdvancedFeaturesRemainingWorkLabel() => WidgetsResources.Get(nameof (BurnupWidget_AdvancedFeaturesRemainingWorkLabel));

    public static string BurnupWidget_AdvancedFeaturesRemainingWorkLabel(CultureInfo culture) => WidgetsResources.Get(nameof (BurnupWidget_AdvancedFeaturesRemainingWorkLabel), culture);

    public static string BurndownConfig_FieldCriteriaHeader() => WidgetsResources.Get(nameof (BurndownConfig_FieldCriteriaHeader));

    public static string BurndownConfig_FieldCriteriaHeader(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownConfig_FieldCriteriaHeader), culture);

    public static string BurndownConfig_FieldCriteriaTooltip() => WidgetsResources.Get(nameof (BurndownConfig_FieldCriteriaTooltip));

    public static string BurndownConfig_FieldCriteriaTooltip(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownConfig_FieldCriteriaTooltip), culture);

    public static string BurndownConfig_FieldCriteriaMaxFiltersAdded() => WidgetsResources.Get(nameof (BurndownConfig_FieldCriteriaMaxFiltersAdded));

    public static string BurndownConfig_FieldCriteriaMaxFiltersAdded(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownConfig_FieldCriteriaMaxFiltersAdded), culture);

    public static string BurndownConfig_FieldFilterAddCriteria() => WidgetsResources.Get(nameof (BurndownConfig_FieldFilterAddCriteria));

    public static string BurndownConfig_FieldFilterAddCriteria(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownConfig_FieldFilterAddCriteria), culture);

    public static string FieldFilterList_RemoveRowButtonAriaLabel() => WidgetsResources.Get(nameof (FieldFilterList_RemoveRowButtonAriaLabel));

    public static string FieldFilterList_RemoveRowButtonAriaLabel(CultureInfo culture) => WidgetsResources.Get(nameof (FieldFilterList_RemoveRowButtonAriaLabel), culture);

    public static string BurndownWidget_TimePeriodHeader() => WidgetsResources.Get(nameof (BurndownWidget_TimePeriodHeader));

    public static string BurndownWidget_TimePeriodHeader(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownWidget_TimePeriodHeader), culture);

    public static string BurndownWidget_LearnMore() => WidgetsResources.Get(nameof (BurndownWidget_LearnMore));

    public static string BurndownWidget_LearnMore(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownWidget_LearnMore), culture);

    public static string BurndownWidget_LearnMoreLink() => WidgetsResources.Get(nameof (BurndownWidget_LearnMoreLink));

    public static string BurndownWidget_LearnMoreLink(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownWidget_LearnMoreLink), culture);

    public static string BurnupWidget_LearnMoreLink() => WidgetsResources.Get(nameof (BurnupWidget_LearnMoreLink));

    public static string BurnupWidget_LearnMoreLink(CultureInfo culture) => WidgetsResources.Get(nameof (BurnupWidget_LearnMoreLink), culture);

    public static string TimePeriod_EndDateLabel() => WidgetsResources.Get(nameof (TimePeriod_EndDateLabel));

    public static string TimePeriod_EndDateLabel(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_EndDateLabel), culture);

    public static string TimePeriod_PlotByLabel() => WidgetsResources.Get(nameof (TimePeriod_PlotByLabel));

    public static string TimePeriod_PlotByLabel(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_PlotByLabel), culture);

    public static string TimePeriod_PlotBurndownByLabel() => WidgetsResources.Get(nameof (TimePeriod_PlotBurndownByLabel));

    public static string TimePeriod_PlotBurndownByLabel(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_PlotBurndownByLabel), culture);

    public static string TimePeriod_PlotBurnupByLabel() => WidgetsResources.Get(nameof (TimePeriod_PlotBurnupByLabel));

    public static string TimePeriod_PlotBurnupByLabel(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_PlotBurnupByLabel), culture);

    public static string TimePeriod_PlotByDateValue() => WidgetsResources.Get(nameof (TimePeriod_PlotByDateValue));

    public static string TimePeriod_PlotByDateValue(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_PlotByDateValue), culture);

    public static string TimePeriod_PlotByIterationValue() => WidgetsResources.Get(nameof (TimePeriod_PlotByIterationValue));

    public static string TimePeriod_PlotByIterationValue(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_PlotByIterationValue), culture);

    public static string TimePeriod_PlotIntervalLabel() => WidgetsResources.Get(nameof (TimePeriod_PlotIntervalLabel));

    public static string TimePeriod_PlotIntervalLabel(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_PlotIntervalLabel), culture);

    public static string TimePeriod_LastDayOfWeekLabel() => WidgetsResources.Get(nameof (TimePeriod_LastDayOfWeekLabel));

    public static string TimePeriod_LastDayOfWeekLabel(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_LastDayOfWeekLabel), culture);

    public static string TimePeriod_PlotUnitsDaysValue() => WidgetsResources.Get(nameof (TimePeriod_PlotUnitsDaysValue));

    public static string TimePeriod_PlotUnitsDaysValue(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_PlotUnitsDaysValue), culture);

    public static string TimePeriod_PlotUnitsWeeksValue() => WidgetsResources.Get(nameof (TimePeriod_PlotUnitsWeeksValue));

    public static string TimePeriod_PlotUnitsWeeksValue(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_PlotUnitsWeeksValue), culture);

    public static string TimePeriod_PlotUnitsMonthsValue() => WidgetsResources.Get(nameof (TimePeriod_PlotUnitsMonthsValue));

    public static string TimePeriod_PlotUnitsMonthsValue(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_PlotUnitsMonthsValue), culture);

    public static string TimePeriod_LastDayOfWeekMonday() => WidgetsResources.Get(nameof (TimePeriod_LastDayOfWeekMonday));

    public static string TimePeriod_LastDayOfWeekMonday(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_LastDayOfWeekMonday), culture);

    public static string TimePeriod_LastDayOfWeekTuesday() => WidgetsResources.Get(nameof (TimePeriod_LastDayOfWeekTuesday));

    public static string TimePeriod_LastDayOfWeekTuesday(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_LastDayOfWeekTuesday), culture);

    public static string TimePeriod_LastDayOfWeekWednesday() => WidgetsResources.Get(nameof (TimePeriod_LastDayOfWeekWednesday));

    public static string TimePeriod_LastDayOfWeekWednesday(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_LastDayOfWeekWednesday), culture);

    public static string TimePeriod_LastDayOfWeekThursday() => WidgetsResources.Get(nameof (TimePeriod_LastDayOfWeekThursday));

    public static string TimePeriod_LastDayOfWeekThursday(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_LastDayOfWeekThursday), culture);

    public static string TimePeriod_LastDayOfWeekFriday() => WidgetsResources.Get(nameof (TimePeriod_LastDayOfWeekFriday));

    public static string TimePeriod_LastDayOfWeekFriday(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_LastDayOfWeekFriday), culture);

    public static string TimePeriod_LastDayOfWeekSaturday() => WidgetsResources.Get(nameof (TimePeriod_LastDayOfWeekSaturday));

    public static string TimePeriod_LastDayOfWeekSaturday(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_LastDayOfWeekSaturday), culture);

    public static string TimePeriod_LastDayOfWeekSunday() => WidgetsResources.Get(nameof (TimePeriod_LastDayOfWeekSunday));

    public static string TimePeriod_LastDayOfWeekSunday(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_LastDayOfWeekSunday), culture);

    public static string TimePeriod_OverFiftyPlotPointsFormat(object arg0, object arg1) => WidgetsResources.Format(nameof (TimePeriod_OverFiftyPlotPointsFormat), arg0, arg1);

    public static string TimePeriod_OverFiftyPlotPointsFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (TimePeriod_OverFiftyPlotPointsFormat), culture, arg0, arg1);
    }

    public static string TimePeriod_OverFiftyPlotPointsDayWarning() => WidgetsResources.Get(nameof (TimePeriod_OverFiftyPlotPointsDayWarning));

    public static string TimePeriod_OverFiftyPlotPointsDayWarning(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_OverFiftyPlotPointsDayWarning), culture);

    public static string TimePeriod_OverFiftyPlotPointsWeekWarning() => WidgetsResources.Get(nameof (TimePeriod_OverFiftyPlotPointsWeekWarning));

    public static string TimePeriod_OverFiftyPlotPointsWeekWarning(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_OverFiftyPlotPointsWeekWarning), culture);

    public static string TimePeriod_OverFiftyPlotPointsMonthWarning() => WidgetsResources.Get(nameof (TimePeriod_OverFiftyPlotPointsMonthWarning));

    public static string TimePeriod_OverFiftyPlotPointsMonthWarning(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_OverFiftyPlotPointsMonthWarning), culture);

    public static string BurndownConfig_BurndownLabel() => WidgetsResources.Get(nameof (BurndownConfig_BurndownLabel));

    public static string BurndownConfig_BurndownLabel(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownConfig_BurndownLabel), culture);

    public static string BurndownConfig_FieldFilterNameLabel() => WidgetsResources.Get(nameof (BurndownConfig_FieldFilterNameLabel));

    public static string BurndownConfig_FieldFilterNameLabel(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownConfig_FieldFilterNameLabel), culture);

    public static string BurndownConfig_FieldFilterQueryValueLabel() => WidgetsResources.Get(nameof (BurndownConfig_FieldFilterQueryValueLabel));

    public static string BurndownConfig_FieldFilterQueryValueLabel(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownConfig_FieldFilterQueryValueLabel), culture);

    public static string BurndownConfig_FieldQueryOperatorLabel() => WidgetsResources.Get(nameof (BurndownConfig_FieldQueryOperatorLabel));

    public static string BurndownConfig_FieldQueryOperatorLabel(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownConfig_FieldQueryOperatorLabel), culture);

    public static string BurndownConfig_BlockedBugsCheckboxMessage() => WidgetsResources.Get(nameof (BurndownConfig_BlockedBugsCheckboxMessage));

    public static string BurndownConfig_BlockedBugsCheckboxMessage(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownConfig_BlockedBugsCheckboxMessage), culture);

    public static string BurndownConfig_BlockedBugsLearnMore() => WidgetsResources.Get(nameof (BurndownConfig_BlockedBugsLearnMore));

    public static string BurndownConfig_BlockedBugsLearnMore(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownConfig_BlockedBugsLearnMore), culture);

    public static string FieldCriteriaHelper_ContainsText() => WidgetsResources.Get(nameof (FieldCriteriaHelper_ContainsText));

    public static string FieldCriteriaHelper_ContainsText(CultureInfo culture) => WidgetsResources.Get(nameof (FieldCriteriaHelper_ContainsText), culture);

    public static string FieldCriteriaHelper_DoesNotContainText() => WidgetsResources.Get(nameof (FieldCriteriaHelper_DoesNotContainText));

    public static string FieldCriteriaHelper_DoesNotContainText(CultureInfo culture) => WidgetsResources.Get(nameof (FieldCriteriaHelper_DoesNotContainText), culture);

    public static string AnalyticsFieldsFilterControl_ErrorOnRowFormat(object arg0, object arg1) => WidgetsResources.Format(nameof (AnalyticsFieldsFilterControl_ErrorOnRowFormat), arg0, arg1);

    public static string AnalyticsFieldsFilterControl_ErrorOnRowFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (AnalyticsFieldsFilterControl_ErrorOnRowFormat), culture, arg0, arg1);
    }

    public static string AnalyticsFieldsFilterControl_FieldUnsupported() => WidgetsResources.Get(nameof (AnalyticsFieldsFilterControl_FieldUnsupported));

    public static string AnalyticsFieldsFilterControl_FieldUnsupported(CultureInfo culture) => WidgetsResources.Get(nameof (AnalyticsFieldsFilterControl_FieldUnsupported), culture);

    public static string AnalyticsFieldsFilterControl_NoSupportedOperator() => WidgetsResources.Get(nameof (AnalyticsFieldsFilterControl_NoSupportedOperator));

    public static string AnalyticsFieldsFilterControl_NoSupportedOperator(CultureInfo culture) => WidgetsResources.Get(nameof (AnalyticsFieldsFilterControl_NoSupportedOperator), culture);

    public static string AnalyticsFieldsFilterControl_ErrorOnValidation() => WidgetsResources.Get(nameof (AnalyticsFieldsFilterControl_ErrorOnValidation));

    public static string AnalyticsFieldsFilterControl_ErrorOnValidation(CultureInfo culture) => WidgetsResources.Get(nameof (AnalyticsFieldsFilterControl_ErrorOnValidation), culture);

    public static string AnalyticsFieldsFilterControl_EmptyFieldError() => WidgetsResources.Get(nameof (AnalyticsFieldsFilterControl_EmptyFieldError));

    public static string AnalyticsFieldsFilterControl_EmptyFieldError(CultureInfo culture) => WidgetsResources.Get(nameof (AnalyticsFieldsFilterControl_EmptyFieldError), culture);

    public static string AnalyticsFieldsFilterControl_UnsupportedMacroError() => WidgetsResources.Get(nameof (AnalyticsFieldsFilterControl_UnsupportedMacroError));

    public static string AnalyticsFieldsFilterControl_UnsupportedMacroError(CultureInfo culture) => WidgetsResources.Get(nameof (AnalyticsFieldsFilterControl_UnsupportedMacroError), culture);

    public static string FieldFilterPicker_EmptyFieldError() => WidgetsResources.Get(nameof (FieldFilterPicker_EmptyFieldError));

    public static string FieldFilterPicker_EmptyFieldError(CultureInfo culture) => WidgetsResources.Get(nameof (FieldFilterPicker_EmptyFieldError), culture);

    public static string EndDate_AfterStartDateError() => WidgetsResources.Get(nameof (EndDate_AfterStartDateError));

    public static string EndDate_AfterStartDateError(CultureInfo culture) => WidgetsResources.Get(nameof (EndDate_AfterStartDateError), culture);

    public static string DatePicker_NeedMinDayPlusOneFormat(object arg0) => WidgetsResources.Format(nameof (DatePicker_NeedMinDayPlusOneFormat), arg0);

    public static string DatePicker_NeedMinDayPlusOneFormat(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (DatePicker_NeedMinDayPlusOneFormat), culture, arg0);

    public static string DatePicker_MaxDaysAllowed(object arg0) => WidgetsResources.Format(nameof (DatePicker_MaxDaysAllowed), arg0);

    public static string DatePicker_MaxDaysAllowed(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (DatePicker_MaxDaysAllowed), culture, arg0);

    public static string DatePicker_MaxWeeksAllowed(object arg0) => WidgetsResources.Format(nameof (DatePicker_MaxWeeksAllowed), arg0);

    public static string DatePicker_MaxWeeksAllowed(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (DatePicker_MaxWeeksAllowed), culture, arg0);

    public static string DatePicker_MaxMonthsAllowed(object arg0) => WidgetsResources.Format(nameof (DatePicker_MaxMonthsAllowed), arg0);

    public static string DatePicker_MaxMonthsAllowed(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (DatePicker_MaxMonthsAllowed), culture, arg0);

    public static string StartDatePicker_IterationModeError() => WidgetsResources.Get(nameof (StartDatePicker_IterationModeError));

    public static string StartDatePicker_IterationModeError(CultureInfo culture) => WidgetsResources.Get(nameof (StartDatePicker_IterationModeError), culture);

    public static string TimePeriod_IterationsHeader() => WidgetsResources.Get(nameof (TimePeriod_IterationsHeader));

    public static string TimePeriod_IterationsHeader(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_IterationsHeader), culture);

    public static string TimePeriod_IterationsAddLabel() => WidgetsResources.Get(nameof (TimePeriod_IterationsAddLabel));

    public static string TimePeriod_IterationsAddLabel(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_IterationsAddLabel), culture);

    public static string TimePeriod_IterationsRemoveLabel() => WidgetsResources.Get(nameof (TimePeriod_IterationsRemoveLabel));

    public static string TimePeriod_IterationsRemoveLabel(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_IterationsRemoveLabel), culture);

    public static string TimePeriod_MaxIterationsAdded() => WidgetsResources.Get(nameof (TimePeriod_MaxIterationsAdded));

    public static string TimePeriod_MaxIterationsAdded(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_MaxIterationsAdded), culture);

    public static string TimePeriod_IterationsEmptyOrUnrecognized() => WidgetsResources.Get(nameof (TimePeriod_IterationsEmptyOrUnrecognized));

    public static string TimePeriod_IterationsEmptyOrUnrecognized(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_IterationsEmptyOrUnrecognized), culture);

    public static string TimePeriod_DuplicateIteration() => WidgetsResources.Get(nameof (TimePeriod_DuplicateIteration));

    public static string TimePeriod_DuplicateIteration(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_DuplicateIteration), culture);

    public static string TimePeriod_SelectOneIteration() => WidgetsResources.Get(nameof (TimePeriod_SelectOneIteration));

    public static string TimePeriod_SelectOneIteration(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_SelectOneIteration), culture);

    public static string IterationsPicker_NoIterationsMessage() => WidgetsResources.Get(nameof (IterationsPicker_NoIterationsMessage));

    public static string IterationsPicker_NoIterationsMessage(CultureInfo culture) => WidgetsResources.Get(nameof (IterationsPicker_NoIterationsMessage), culture);

    public static string TimePeriod_CurrentIterationNotSupported() => WidgetsResources.Get(nameof (TimePeriod_CurrentIterationNotSupported));

    public static string TimePeriod_CurrentIterationNotSupported(CultureInfo culture) => WidgetsResources.Get(nameof (TimePeriod_CurrentIterationNotSupported), culture);

    public static string BurndownConfiguration_FieldCriteriaValidationFailed() => WidgetsResources.Get(nameof (BurndownConfiguration_FieldCriteriaValidationFailed));

    public static string BurndownConfiguration_FieldCriteriaValidationFailed(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownConfiguration_FieldCriteriaValidationFailed), culture);

    public static string BurndownConfiguration_IterationValidationFailed() => WidgetsResources.Get(nameof (BurndownConfiguration_IterationValidationFailed));

    public static string BurndownConfiguration_IterationValidationFailed(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownConfiguration_IterationValidationFailed), culture);

    public static string BurnXConfiguration_WidgetTitlePlaceholder() => WidgetsResources.Get(nameof (BurnXConfiguration_WidgetTitlePlaceholder));

    public static string BurnXConfiguration_WidgetTitlePlaceholder(CultureInfo culture) => WidgetsResources.Get(nameof (BurnXConfiguration_WidgetTitlePlaceholder), culture);

    public static string BurnupConfig_AggregationBurnupLabel() => WidgetsResources.Get(nameof (BurnupConfig_AggregationBurnupLabel));

    public static string BurnupConfig_AggregationBurnupLabel(CultureInfo culture) => WidgetsResources.Get(nameof (BurnupConfig_AggregationBurnupLabel), culture);

    public static string BurndownConfig_FieldFilterInvalidField() => WidgetsResources.Get(nameof (BurndownConfig_FieldFilterInvalidField));

    public static string BurndownConfig_FieldFilterInvalidField(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownConfig_FieldFilterInvalidField), culture);

    public static string BurndownConfig_ProjectTeamPickerRowSelectionError() => WidgetsResources.Get(nameof (BurndownConfig_ProjectTeamPickerRowSelectionError));

    public static string BurndownConfig_ProjectTeamPickerRowSelectionError(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownConfig_ProjectTeamPickerRowSelectionError), culture);

    public static string BurndownConfig_AnalyticsFilterFieldPlaceholderText() => WidgetsResources.Get(nameof (BurndownConfig_AnalyticsFilterFieldPlaceholderText));

    public static string BurndownConfig_AnalyticsFilterFieldPlaceholderText(CultureInfo culture) => WidgetsResources.Get(nameof (BurndownConfig_AnalyticsFilterFieldPlaceholderText), culture);

    public static string PlotByControl_NoEndDate() => WidgetsResources.Get(nameof (PlotByControl_NoEndDate));

    public static string PlotByControl_NoEndDate(CultureInfo culture) => WidgetsResources.Get(nameof (PlotByControl_NoEndDate), culture);

    public static string NetworkError_Status0() => WidgetsResources.Get(nameof (NetworkError_Status0));

    public static string NetworkError_Status0(CultureInfo culture) => WidgetsResources.Get(nameof (NetworkError_Status0), culture);

    public static string WorkItemTypePicker_BacklogLabel() => WidgetsResources.Get(nameof (WorkItemTypePicker_BacklogLabel));

    public static string WorkItemTypePicker_BacklogLabel(CultureInfo culture) => WidgetsResources.Get(nameof (WorkItemTypePicker_BacklogLabel), culture);

    public static string CumulativeFlowDiagram_PlaceholderBoardUnsupportedFormat(object arg0) => WidgetsResources.Format(nameof (CumulativeFlowDiagram_PlaceholderBoardUnsupportedFormat), arg0);

    public static string CumulativeFlowDiagram_PlaceholderBoardUnsupportedFormat(
      object arg0,
      CultureInfo culture)
    {
      return WidgetsResources.Format(nameof (CumulativeFlowDiagram_PlaceholderBoardUnsupportedFormat), culture, arg0);
    }

    public static string AnalyticsTrendsConfig_Name() => WidgetsResources.Get(nameof (AnalyticsTrendsConfig_Name));

    public static string AnalyticsTrendsConfig_Name(CultureInfo culture) => WidgetsResources.Get(nameof (AnalyticsTrendsConfig_Name), culture);

    public static string KanbanTime_WorkItemTypeNeeded() => WidgetsResources.Get(nameof (KanbanTime_WorkItemTypeNeeded));

    public static string KanbanTime_WorkItemTypeNeeded(CultureInfo culture) => WidgetsResources.Get(nameof (KanbanTime_WorkItemTypeNeeded), culture);

    public static string CumulativeFlowDiagram_BoardNeeded() => WidgetsResources.Get(nameof (CumulativeFlowDiagram_BoardNeeded));

    public static string CumulativeFlowDiagram_BoardNeeded(CultureInfo culture) => WidgetsResources.Get(nameof (CumulativeFlowDiagram_BoardNeeded), culture);

    public static string UnconfiguredImage_AltText() => WidgetsResources.Get(nameof (UnconfiguredImage_AltText));

    public static string UnconfiguredImage_AltText(CultureInfo culture) => WidgetsResources.Get(nameof (UnconfiguredImage_AltText), culture);

    public static string NoAssignedItems_AltText() => WidgetsResources.Get(nameof (NoAssignedItems_AltText));

    public static string NoAssignedItems_AltText(CultureInfo culture) => WidgetsResources.Get(nameof (NoAssignedItems_AltText), culture);

    public static string Config_InvalidQueryValues() => WidgetsResources.Get(nameof (Config_InvalidQueryValues));

    public static string Config_InvalidQueryValues(CultureInfo culture) => WidgetsResources.Get(nameof (Config_InvalidQueryValues), culture);

    public static string DeletedBoardColumnName(object arg0) => WidgetsResources.Format(nameof (DeletedBoardColumnName), arg0);

    public static string DeletedBoardColumnName(object arg0, CultureInfo culture) => WidgetsResources.Format(nameof (DeletedBoardColumnName), culture, arg0);

    public static string CumulativeFlowDiagram_ColumnLabel() => WidgetsResources.Get(nameof (CumulativeFlowDiagram_ColumnLabel));

    public static string CumulativeFlowDiagram_ColumnLabel(CultureInfo culture) => WidgetsResources.Get(nameof (CumulativeFlowDiagram_ColumnLabel), culture);

    public static string SprintCapacity_TitleFormat(object arg0, object arg1) => WidgetsResources.Format(nameof (SprintCapacity_TitleFormat), arg0, arg1);

    public static string SprintCapacity_TitleFormat(object arg0, object arg1, CultureInfo culture) => WidgetsResources.Format(nameof (SprintCapacity_TitleFormat), culture, arg0, arg1);

    public static string QueryScalar_QueryCaptionRequired() => WidgetsResources.Get(nameof (QueryScalar_QueryCaptionRequired));

    public static string QueryScalar_QueryCaptionRequired(CultureInfo culture) => WidgetsResources.Get(nameof (QueryScalar_QueryCaptionRequired), culture);

    public static string ShowResolvedItemsAsCompletedLabel() => WidgetsResources.Get(nameof (ShowResolvedItemsAsCompletedLabel));

    public static string ShowResolvedItemsAsCompletedLabel(CultureInfo culture) => WidgetsResources.Get(nameof (ShowResolvedItemsAsCompletedLabel), culture);

    public static string ShowResolvedItemsAsCompletedToolTipText() => WidgetsResources.Get(nameof (ShowResolvedItemsAsCompletedToolTipText));

    public static string ShowResolvedItemsAsCompletedToolTipText(CultureInfo culture) => WidgetsResources.Get(nameof (ShowResolvedItemsAsCompletedToolTipText), culture);
  }
}
