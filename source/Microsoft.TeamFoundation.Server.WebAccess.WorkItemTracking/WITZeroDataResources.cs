// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.WITZeroDataResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 74AD14A4-225D-46D2-B154-945941A2D167
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking
{
  public static class WITZeroDataResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (WITZeroDataResources), typeof (WITZeroDataResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => WITZeroDataResources.s_resMgr;

    private static string Get(string resourceName) => WITZeroDataResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? WITZeroDataResources.Get(resourceName) : WITZeroDataResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) WITZeroDataResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? WITZeroDataResources.GetInt(resourceName) : (int) WITZeroDataResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) WITZeroDataResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? WITZeroDataResources.GetBool(resourceName) : (bool) WITZeroDataResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => WITZeroDataResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = WITZeroDataResources.Get(resourceName, culture);
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

    public static string Illustrations_GetBackToRecentWorkAltText() => WITZeroDataResources.Get(nameof (Illustrations_GetBackToRecentWorkAltText));

    public static string Illustrations_GetBackToRecentWorkAltText(CultureInfo culture) => WITZeroDataResources.Get(nameof (Illustrations_GetBackToRecentWorkAltText), culture);

    public static string Illustrations_KeepAnEyeOnImportantWorkAltText() => WITZeroDataResources.Get(nameof (Illustrations_KeepAnEyeOnImportantWorkAltText));

    public static string Illustrations_KeepAnEyeOnImportantWorkAltText(CultureInfo culture) => WITZeroDataResources.Get(nameof (Illustrations_KeepAnEyeOnImportantWorkAltText), culture);

    public static string Illustrations_MentionSomeoneAltText() => WITZeroDataResources.Get(nameof (Illustrations_MentionSomeoneAltText));

    public static string Illustrations_MentionSomeoneAltText(CultureInfo culture) => WITZeroDataResources.Get(nameof (Illustrations_MentionSomeoneAltText), culture);

    public static string Illustrations_SomethingWrongOnServerAltText() => WITZeroDataResources.Get(nameof (Illustrations_SomethingWrongOnServerAltText));

    public static string Illustrations_SomethingWrongOnServerAltText(CultureInfo culture) => WITZeroDataResources.Get(nameof (Illustrations_SomethingWrongOnServerAltText), culture);

    public static string ZeroData_WorkItems_LinkText() => WITZeroDataResources.Get(nameof (ZeroData_WorkItems_LinkText));

    public static string ZeroData_WorkItems_LinkText(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_WorkItems_LinkText), culture);

    public static string Illustrations_YourWorkInOnePlaceAltText() => WITZeroDataResources.Get(nameof (Illustrations_YourWorkInOnePlaceAltText));

    public static string Illustrations_YourWorkInOnePlaceAltText(CultureInfo culture) => WITZeroDataResources.Get(nameof (Illustrations_YourWorkInOnePlaceAltText), culture);

    public static string ZeroData_AssignedToMe_PrimaryMessage() => WITZeroDataResources.Get(nameof (ZeroData_AssignedToMe_PrimaryMessage));

    public static string ZeroData_AssignedToMe_PrimaryMessage(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_AssignedToMe_PrimaryMessage), culture);

    public static string ZeroData_AssignedToMe_SecondaryMessage() => WITZeroDataResources.Get(nameof (ZeroData_AssignedToMe_SecondaryMessage));

    public static string ZeroData_AssignedToMe_SecondaryMessage(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_AssignedToMe_SecondaryMessage), culture);

    public static string ZeroData_AssignedToMe_SecondaryMessageLinkUrl() => WITZeroDataResources.Get(nameof (ZeroData_AssignedToMe_SecondaryMessageLinkUrl));

    public static string ZeroData_AssignedToMe_SecondaryMessageLinkUrl(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_AssignedToMe_SecondaryMessageLinkUrl), culture);

    public static string ZeroData_Filter_WorkItems_ArtifactName() => WITZeroDataResources.Get(nameof (ZeroData_Filter_WorkItems_ArtifactName));

    public static string ZeroData_Filter_WorkItems_ArtifactName(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_Filter_WorkItems_ArtifactName), culture);

    public static string ZeroData_Following_NotConfigured_PrimaryMessage() => WITZeroDataResources.Get(nameof (ZeroData_Following_NotConfigured_PrimaryMessage));

    public static string ZeroData_Following_NotConfigured_PrimaryMessage(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_Following_NotConfigured_PrimaryMessage), culture);

    public static string ZeroData_Following_NotConfigured_SecondaryMessage() => WITZeroDataResources.Get(nameof (ZeroData_Following_NotConfigured_SecondaryMessage));

    public static string ZeroData_Following_NotConfigured_SecondaryMessage(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_Following_NotConfigured_SecondaryMessage), culture);

    public static string ZeroData_Following_NotConfigured_SecondaryMessageLinkText() => WITZeroDataResources.Get(nameof (ZeroData_Following_NotConfigured_SecondaryMessageLinkText));

    public static string ZeroData_Following_NotConfigured_SecondaryMessageLinkText(
      CultureInfo culture)
    {
      return WITZeroDataResources.Get(nameof (ZeroData_Following_NotConfigured_SecondaryMessageLinkText), culture);
    }

    public static string ZeroData_Following_NotConfigured_SecondaryMessageLinkUrl() => WITZeroDataResources.Get(nameof (ZeroData_Following_NotConfigured_SecondaryMessageLinkUrl));

    public static string ZeroData_Following_NotConfigured_SecondaryMessageLinkUrl(
      CultureInfo culture)
    {
      return WITZeroDataResources.Get(nameof (ZeroData_Following_NotConfigured_SecondaryMessageLinkUrl), culture);
    }

    public static string ZeroData_Following_PrimaryMessage() => WITZeroDataResources.Get(nameof (ZeroData_Following_PrimaryMessage));

    public static string ZeroData_Following_PrimaryMessage(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_Following_PrimaryMessage), culture);

    public static string ZeroData_Following_SecondaryMessageFormat(object arg0) => WITZeroDataResources.Format(nameof (ZeroData_Following_SecondaryMessageFormat), arg0);

    public static string ZeroData_Following_SecondaryMessageFormat(object arg0, CultureInfo culture) => WITZeroDataResources.Format(nameof (ZeroData_Following_SecondaryMessageFormat), culture, arg0);

    public static string ZeroData_Following_SecondaryMessageLinkText() => WITZeroDataResources.Get(nameof (ZeroData_Following_SecondaryMessageLinkText));

    public static string ZeroData_Following_SecondaryMessageLinkText(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_Following_SecondaryMessageLinkText), culture);

    public static string ZeroData_Following_SecondaryMessageLinkUrl() => WITZeroDataResources.Get(nameof (ZeroData_Following_SecondaryMessageLinkUrl));

    public static string ZeroData_Following_SecondaryMessageLinkUrl(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_Following_SecondaryMessageLinkUrl), culture);

    public static string ZeroData_GlobalSearch_LinkText() => WITZeroDataResources.Get(nameof (ZeroData_GlobalSearch_LinkText));

    public static string ZeroData_GlobalSearch_LinkText(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_GlobalSearch_LinkText), culture);

    public static string ZeroData_Mentioned_PrimaryMessage() => WITZeroDataResources.Get(nameof (ZeroData_Mentioned_PrimaryMessage));

    public static string ZeroData_Mentioned_PrimaryMessage(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_Mentioned_PrimaryMessage), culture);

    public static string ZeroData_Mentioned_SecondaryMessage() => WITZeroDataResources.Get(nameof (ZeroData_Mentioned_SecondaryMessage));

    public static string ZeroData_Mentioned_SecondaryMessage(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_Mentioned_SecondaryMessage), culture);

    public static string ZeroData_Mentioned_SecondaryMessageLinkText() => WITZeroDataResources.Get(nameof (ZeroData_Mentioned_SecondaryMessageLinkText));

    public static string ZeroData_Mentioned_SecondaryMessageLinkText(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_Mentioned_SecondaryMessageLinkText), culture);

    public static string ZeroData_Mentioned_SecondaryMessageLinkUrl() => WITZeroDataResources.Get(nameof (ZeroData_Mentioned_SecondaryMessageLinkUrl));

    public static string ZeroData_Mentioned_SecondaryMessageLinkUrl(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_Mentioned_SecondaryMessageLinkUrl), culture);

    public static string ZeroData_MyActivity_PrimaryMessage() => WITZeroDataResources.Get(nameof (ZeroData_MyActivity_PrimaryMessage));

    public static string ZeroData_MyActivity_PrimaryMessage(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_MyActivity_PrimaryMessage), culture);

    public static string ZeroData_MyActivity_SecondaryMessage() => WITZeroDataResources.Get(nameof (ZeroData_MyActivity_SecondaryMessage));

    public static string ZeroData_MyActivity_SecondaryMessage(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_MyActivity_SecondaryMessage), culture);

    public static string ZeroData_MyActivity_SecondaryMessageLinkUrl() => WITZeroDataResources.Get(nameof (ZeroData_MyActivity_SecondaryMessageLinkUrl));

    public static string ZeroData_MyActivity_SecondaryMessageLinkUrl(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_MyActivity_SecondaryMessageLinkUrl), culture);

    public static string ZeroData_RecentlyUpdated_PrimaryMessage() => WITZeroDataResources.Get(nameof (ZeroData_RecentlyUpdated_PrimaryMessage));

    public static string ZeroData_RecentlyUpdated_PrimaryMessage(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_RecentlyUpdated_PrimaryMessage), culture);

    public static string ZeroData_RecentlyUpdated_SecondaryMessage() => WITZeroDataResources.Get(nameof (ZeroData_RecentlyUpdated_SecondaryMessage));

    public static string ZeroData_RecentlyUpdated_SecondaryMessage(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_RecentlyUpdated_SecondaryMessage), culture);

    public static string ZeroData_RecentlyUpdated_SecondaryMessageLinkUrl() => WITZeroDataResources.Get(nameof (ZeroData_RecentlyUpdated_SecondaryMessageLinkUrl));

    public static string ZeroData_RecentlyUpdated_SecondaryMessageLinkUrl(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_RecentlyUpdated_SecondaryMessageLinkUrl), culture);

    public static string ZeroData_RecentlyCreated_PrimaryMessage() => WITZeroDataResources.Get(nameof (ZeroData_RecentlyCreated_PrimaryMessage));

    public static string ZeroData_RecentlyCreated_PrimaryMessage(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_RecentlyCreated_PrimaryMessage), culture);

    public static string ZeroData_RecentlyCreated_SecondaryMessage() => WITZeroDataResources.Get(nameof (ZeroData_RecentlyCreated_SecondaryMessage));

    public static string ZeroData_RecentlyCreated_SecondaryMessage(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_RecentlyCreated_SecondaryMessage), culture);

    public static string ZeroData_RecentlyCreated_SecondaryMessageLinkUrl() => WITZeroDataResources.Get(nameof (ZeroData_RecentlyCreated_SecondaryMessageLinkUrl));

    public static string ZeroData_RecentlyCreated_SecondaryMessageLinkUrl(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_RecentlyCreated_SecondaryMessageLinkUrl), culture);

    public static string ZeroData_ServerError_PrimaryText() => WITZeroDataResources.Get(nameof (ZeroData_ServerError_PrimaryText));

    public static string ZeroData_ServerError_PrimaryText(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_ServerError_PrimaryText), culture);

    public static string ZeroData_QueryResult_PrimaryText() => WITZeroDataResources.Get(nameof (ZeroData_QueryResult_PrimaryText));

    public static string ZeroData_QueryResult_PrimaryText(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_QueryResult_PrimaryText), culture);

    public static string ZeroData_Charts_NewChartPrimaryText() => WITZeroDataResources.Get(nameof (ZeroData_Charts_NewChartPrimaryText));

    public static string ZeroData_Charts_NewChartPrimaryText(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_Charts_NewChartPrimaryText), culture);

    public static string ZeroData_Charts_NewChartSecondaryText() => WITZeroDataResources.Get(nameof (ZeroData_Charts_NewChartSecondaryText));

    public static string ZeroData_Charts_NewChartSecondaryText(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_Charts_NewChartSecondaryText), culture);

    public static string ZeroData_Charts_UnsavedPrimaryText() => WITZeroDataResources.Get(nameof (ZeroData_Charts_UnsavedPrimaryText));

    public static string ZeroData_Charts_UnsavedPrimaryText(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_Charts_UnsavedPrimaryText), culture);

    public static string ZeroData_Charts_UnsavedSecondaryText() => WITZeroDataResources.Get(nameof (ZeroData_Charts_UnsavedSecondaryText));

    public static string ZeroData_Charts_UnsavedSecondaryText(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_Charts_UnsavedSecondaryText), culture);

    public static string ZeroData_Charts_LinkQueryPrimaryText() => WITZeroDataResources.Get(nameof (ZeroData_Charts_LinkQueryPrimaryText));

    public static string ZeroData_Charts_LinkQueryPrimaryText(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_Charts_LinkQueryPrimaryText), culture);

    public static string ZeroData_Charts_LinkQuerySecondaryText() => WITZeroDataResources.Get(nameof (ZeroData_Charts_LinkQuerySecondaryText));

    public static string ZeroData_Charts_LinkQuerySecondaryText(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_Charts_LinkQuerySecondaryText), culture);

    public static string ZeroData_RecentlyCompleted_PrimaryMessage() => WITZeroDataResources.Get(nameof (ZeroData_RecentlyCompleted_PrimaryMessage));

    public static string ZeroData_RecentlyCompleted_PrimaryMessage(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_RecentlyCompleted_PrimaryMessage), culture);

    public static string ZeroData_RecentlyCompleted_SecondaryMessage() => WITZeroDataResources.Get(nameof (ZeroData_RecentlyCompleted_SecondaryMessage));

    public static string ZeroData_RecentlyCompleted_SecondaryMessage(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_RecentlyCompleted_SecondaryMessage), culture);

    public static string ZeroData_RecentlyCompleted_SecondaryMessageLinkUrl() => WITZeroDataResources.Get(nameof (ZeroData_RecentlyCompleted_SecondaryMessageLinkUrl));

    public static string ZeroData_RecentlyCompleted_SecondaryMessageLinkUrl(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_RecentlyCompleted_SecondaryMessageLinkUrl), culture);

    public static string ZeroData_MyTeams_PrimaryMessage() => WITZeroDataResources.Get(nameof (ZeroData_MyTeams_PrimaryMessage));

    public static string ZeroData_MyTeams_PrimaryMessage(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_MyTeams_PrimaryMessage), culture);

    public static string ZeroData_MyTeams_SecondaryMessage() => WITZeroDataResources.Get(nameof (ZeroData_MyTeams_SecondaryMessage));

    public static string ZeroData_MyTeams_SecondaryMessage(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_MyTeams_SecondaryMessage), culture);

    public static string ZeroData_MyTeams_SecondaryMessageLinkUrl() => WITZeroDataResources.Get(nameof (ZeroData_MyTeams_SecondaryMessageLinkUrl));

    public static string ZeroData_MyTeams_SecondaryMessageLinkUrl(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_MyTeams_SecondaryMessageLinkUrl), culture);

    public static string ZeroData_MyTeams_NotConfigured_PrimaryMessage() => WITZeroDataResources.Get(nameof (ZeroData_MyTeams_NotConfigured_PrimaryMessage));

    public static string ZeroData_MyTeams_NotConfigured_PrimaryMessage(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_MyTeams_NotConfigured_PrimaryMessage), culture);

    public static string ZeroData_MyTeams_NotConfigured_SecondaryMessage() => WITZeroDataResources.Get(nameof (ZeroData_MyTeams_NotConfigured_SecondaryMessage));

    public static string ZeroData_MyTeams_NotConfigured_SecondaryMessage(CultureInfo culture) => WITZeroDataResources.Get(nameof (ZeroData_MyTeams_NotConfigured_SecondaryMessage), culture);
  }
}
