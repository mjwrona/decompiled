// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.WITServerResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 74AD14A4-225D-46D2-B154-945941A2D167
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking
{
  public static class WITServerResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (WITServerResources), typeof (WITServerResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => WITServerResources.s_resMgr;

    private static string Get(string resourceName) => WITServerResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? WITServerResources.Get(resourceName) : WITServerResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) WITServerResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? WITServerResources.GetInt(resourceName) : (int) WITServerResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) WITServerResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? WITServerResources.GetBool(resourceName) : (bool) WITServerResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => WITServerResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = WITServerResources.Get(resourceName, culture);
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

    public static string NewWorkItem() => WITServerResources.Get(nameof (NewWorkItem));

    public static string NewWorkItem(CultureInfo culture) => WITServerResources.Get(nameof (NewWorkItem), culture);

    public static string QueryEditorInvalidOperatorForSpecialValue(object arg0, object arg1) => WITServerResources.Format(nameof (QueryEditorInvalidOperatorForSpecialValue), arg0, arg1);

    public static string QueryEditorInvalidOperatorForSpecialValue(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITServerResources.Format(nameof (QueryEditorInvalidOperatorForSpecialValue), culture, arg0, arg1);
    }

    public static string ColumnOptionsAvailableColumns() => WITServerResources.Get(nameof (ColumnOptionsAvailableColumns));

    public static string ColumnOptionsAvailableColumns(CultureInfo culture) => WITServerResources.Get(nameof (ColumnOptionsAvailableColumns), culture);

    public static string ColumnOptionsSelectedColumns() => WITServerResources.Get(nameof (ColumnOptionsSelectedColumns));

    public static string ColumnOptionsSelectedColumns(CultureInfo culture) => WITServerResources.Get(nameof (ColumnOptionsSelectedColumns), culture);

    public static string WorkItemPaneFilter_Off() => WITServerResources.Get(nameof (WorkItemPaneFilter_Off));

    public static string WorkItemPaneFilter_Off(CultureInfo culture) => WITServerResources.Get(nameof (WorkItemPaneFilter_Off), culture);

    public static string WorkItemPaneFilter_Bottom() => WITServerResources.Get(nameof (WorkItemPaneFilter_Bottom));

    public static string WorkItemPaneFilter_Bottom(CultureInfo culture) => WITServerResources.Get(nameof (WorkItemPaneFilter_Bottom), culture);

    public static string WorkItemPaneFilter_Right() => WITServerResources.Get(nameof (WorkItemPaneFilter_Right));

    public static string WorkItemPaneFilter_Right(CultureInfo culture) => WITServerResources.Get(nameof (WorkItemPaneFilter_Right), culture);

    public static string WorkItemsHubEditorTabTitle() => WITServerResources.Get(nameof (WorkItemsHubEditorTabTitle));

    public static string WorkItemsHubEditorTabTitle(CultureInfo culture) => WITServerResources.Get(nameof (WorkItemsHubEditorTabTitle), culture);

    public static string WorkItemsHubResultsTabTitle() => WITServerResources.Get(nameof (WorkItemsHubResultsTabTitle));

    public static string WorkItemsHubResultsTabTitle(CultureInfo culture) => WITServerResources.Get(nameof (WorkItemsHubResultsTabTitle), culture);

    public static string ResultListDefaultIdColumnWidth() => WITServerResources.Get(nameof (ResultListDefaultIdColumnWidth));

    public static string ResultListDefaultIdColumnWidth(CultureInfo culture) => WITServerResources.Get(nameof (ResultListDefaultIdColumnWidth), culture);

    public static string ResultListDefaultStringColumnWidth() => WITServerResources.Get(nameof (ResultListDefaultStringColumnWidth));

    public static string ResultListDefaultStringColumnWidth(CultureInfo culture) => WITServerResources.Get(nameof (ResultListDefaultStringColumnWidth), culture);

    public static string ResultListDefaultTitleColumnWidth() => WITServerResources.Get(nameof (ResultListDefaultTitleColumnWidth));

    public static string ResultListDefaultTitleColumnWidth(CultureInfo culture) => WITServerResources.Get(nameof (ResultListDefaultTitleColumnWidth), culture);

    public static string NotConfiguredMessage(object arg0) => WITServerResources.Format(nameof (NotConfiguredMessage), arg0);

    public static string NotConfiguredMessage(object arg0, CultureInfo culture) => WITServerResources.Format(nameof (NotConfiguredMessage), culture, arg0);

    public static string InvalidQueryNamePathUrl() => WITServerResources.Get(nameof (InvalidQueryNamePathUrl));

    public static string InvalidQueryNamePathUrl(CultureInfo culture) => WITServerResources.Get(nameof (InvalidQueryNamePathUrl), culture);

    public static string InvalidWorkItemCompatUrl() => WITServerResources.Get(nameof (InvalidWorkItemCompatUrl));

    public static string InvalidWorkItemCompatUrl(CultureInfo culture) => WITServerResources.Get(nameof (InvalidWorkItemCompatUrl), culture);

    public static string UnableToDetermineWorkItemType() => WITServerResources.Get(nameof (UnableToDetermineWorkItemType));

    public static string UnableToDetermineWorkItemType(CultureInfo culture) => WITServerResources.Get(nameof (UnableToDetermineWorkItemType), culture);

    public static string UnableToDetermineTeamProjectFromCompatUrl() => WITServerResources.Get(nameof (UnableToDetermineTeamProjectFromCompatUrl));

    public static string UnableToDetermineTeamProjectFromCompatUrl(CultureInfo culture) => WITServerResources.Get(nameof (UnableToDetermineTeamProjectFromCompatUrl), culture);

    public static string ColumnOptionsAddColumn() => WITServerResources.Get(nameof (ColumnOptionsAddColumn));

    public static string ColumnOptionsAddColumn(CultureInfo culture) => WITServerResources.Get(nameof (ColumnOptionsAddColumn), culture);

    public static string ColumnOptionsMoveColumnDown() => WITServerResources.Get(nameof (ColumnOptionsMoveColumnDown));

    public static string ColumnOptionsMoveColumnDown(CultureInfo culture) => WITServerResources.Get(nameof (ColumnOptionsMoveColumnDown), culture);

    public static string ColumnOptionsMoveColumnUp() => WITServerResources.Get(nameof (ColumnOptionsMoveColumnUp));

    public static string ColumnOptionsMoveColumnUp(CultureInfo culture) => WITServerResources.Get(nameof (ColumnOptionsMoveColumnUp), culture);

    public static string ColumnOptionsRemoveColumn() => WITServerResources.Get(nameof (ColumnOptionsRemoveColumn));

    public static string ColumnOptionsRemoveColumn(CultureInfo culture) => WITServerResources.Get(nameof (ColumnOptionsRemoveColumn), culture);

    public static string ColumnOptionsSortAscending() => WITServerResources.Get(nameof (ColumnOptionsSortAscending));

    public static string ColumnOptionsSortAscending(CultureInfo culture) => WITServerResources.Get(nameof (ColumnOptionsSortAscending), culture);

    public static string ColumnOptionsSortDescending() => WITServerResources.Get(nameof (ColumnOptionsSortDescending));

    public static string ColumnOptionsSortDescending(CultureInfo culture) => WITServerResources.Get(nameof (ColumnOptionsSortDescending), culture);

    public static string InvalidFields(object arg0) => WITServerResources.Format(nameof (InvalidFields), arg0);

    public static string InvalidFields(object arg0, CultureInfo culture) => WITServerResources.Format(nameof (InvalidFields), culture, arg0);

    public static string WorkItemsChartsTabTitle() => WITServerResources.Get(nameof (WorkItemsChartsTabTitle));

    public static string WorkItemsChartsTabTitle(CultureInfo culture) => WITServerResources.Get(nameof (WorkItemsChartsTabTitle), culture);

    public static string ErrorIdsAndRevsLengthMustMatch() => WITServerResources.Get(nameof (ErrorIdsAndRevsLengthMustMatch));

    public static string ErrorIdsAndRevsLengthMustMatch(CultureInfo culture) => WITServerResources.Get(nameof (ErrorIdsAndRevsLengthMustMatch), culture);

    public static string QueryIdNotAdhocQuery() => WITServerResources.Get(nameof (QueryIdNotAdhocQuery));

    public static string QueryIdNotAdhocQuery(CultureInfo culture) => WITServerResources.Get(nameof (QueryIdNotAdhocQuery), culture);

    public static string InvalidLinkType(object arg0) => WITServerResources.Format(nameof (InvalidLinkType), arg0);

    public static string InvalidLinkType(object arg0, CultureInfo culture) => WITServerResources.Format(nameof (InvalidLinkType), culture, arg0);

    public static string MustBeAdminToModifyProcess() => WITServerResources.Get(nameof (MustBeAdminToModifyProcess));

    public static string MustBeAdminToModifyProcess(CultureInfo culture) => WITServerResources.Get(nameof (MustBeAdminToModifyProcess), culture);

    public static string MustCreateProcess() => WITServerResources.Get(nameof (MustCreateProcess));

    public static string MustCreateProcess(CultureInfo culture) => WITServerResources.Get(nameof (MustCreateProcess), culture);

    public static string CopiedToClipboard() => WITServerResources.Get(nameof (CopiedToClipboard));

    public static string CopiedToClipboard(CultureInfo culture) => WITServerResources.Get(nameof (CopiedToClipboard), culture);

    public static string UnfollowWorkItem() => WITServerResources.Get(nameof (UnfollowWorkItem));

    public static string UnfollowWorkItem(CultureInfo culture) => WITServerResources.Get(nameof (UnfollowWorkItem), culture);

    public static string WorkItemTypeExtensionsDataProviderExtensionIdsNotPassedError() => WITServerResources.Get(nameof (WorkItemTypeExtensionsDataProviderExtensionIdsNotPassedError));

    public static string WorkItemTypeExtensionsDataProviderExtensionIdsNotPassedError(
      CultureInfo culture)
    {
      return WITServerResources.Get(nameof (WorkItemTypeExtensionsDataProviderExtensionIdsNotPassedError), culture);
    }

    public static string UpdateWorkItemsDataProviderUpdatePackageNotPassedError() => WITServerResources.Get(nameof (UpdateWorkItemsDataProviderUpdatePackageNotPassedError));

    public static string UpdateWorkItemsDataProviderUpdatePackageNotPassedError(CultureInfo culture) => WITServerResources.Get(nameof (UpdateWorkItemsDataProviderUpdatePackageNotPassedError), culture);

    public static string UpdateWorkItemsDataProviderWrongTypeCastError() => WITServerResources.Get(nameof (UpdateWorkItemsDataProviderWrongTypeCastError));

    public static string UpdateWorkItemsDataProviderWrongTypeCastError(CultureInfo culture) => WITServerResources.Get(nameof (UpdateWorkItemsDataProviderWrongTypeCastError), culture);

    public static string WorkItemFinderSelectAllButtonText() => WITServerResources.Get(nameof (WorkItemFinderSelectAllButtonText));

    public static string WorkItemFinderSelectAllButtonText(CultureInfo culture) => WITServerResources.Get(nameof (WorkItemFinderSelectAllButtonText), culture);

    public static string WorkItemFinderUnselectAllButtonText() => WITServerResources.Get(nameof (WorkItemFinderUnselectAllButtonText));

    public static string WorkItemFinderUnselectAllButtonText(CultureInfo culture) => WITServerResources.Get(nameof (WorkItemFinderUnselectAllButtonText), culture);

    public static string WorkItemFinderResetButtonText() => WITServerResources.Get(nameof (WorkItemFinderResetButtonText));

    public static string WorkItemFinderResetButtonText(CultureInfo culture) => WITServerResources.Get(nameof (WorkItemFinderResetButtonText), culture);
  }
}
