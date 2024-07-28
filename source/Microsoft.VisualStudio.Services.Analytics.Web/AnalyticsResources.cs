// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Web.AnalyticsResources
// Assembly: Microsoft.VisualStudio.Services.Analytics.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 455612C1-A616-4BB6-B9F5-E94C097DFD14
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Web.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.Analytics.Web
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

    public static string TitleDialogLabel() => AnalyticsResources.Get(nameof (TitleDialogLabel));

    public static string TitleDialogLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (TitleDialogLabel), culture);

    public static string DirectoryViewAllPivotName() => AnalyticsResources.Get(nameof (DirectoryViewAllPivotName));

    public static string DirectoryViewAllPivotName(CultureInfo culture) => AnalyticsResources.Get(nameof (DirectoryViewAllPivotName), culture);

    public static string DirectoryViewFavoritesPivotName() => AnalyticsResources.Get(nameof (DirectoryViewFavoritesPivotName));

    public static string DirectoryViewFavoritesPivotName(CultureInfo culture) => AnalyticsResources.Get(nameof (DirectoryViewFavoritesPivotName), culture);

    public static string DirectoryView_SearchWatermark() => AnalyticsResources.Get(nameof (DirectoryView_SearchWatermark));

    public static string DirectoryView_SearchWatermark(CultureInfo culture) => AnalyticsResources.Get(nameof (DirectoryView_SearchWatermark), culture);

    public static string DirectoryView_MyViews() => AnalyticsResources.Get(nameof (DirectoryView_MyViews));

    public static string DirectoryView_MyViews(CultureInfo culture) => AnalyticsResources.Get(nameof (DirectoryView_MyViews), culture);

    public static string DirectoryView_SharedViews() => AnalyticsResources.Get(nameof (DirectoryView_SharedViews));

    public static string DirectoryView_SharedViews(CultureInfo culture) => AnalyticsResources.Get(nameof (DirectoryView_SharedViews), culture);

    public static string DirectoryView_FirstTimeInfoPreviewBanner() => AnalyticsResources.Get(nameof (DirectoryView_FirstTimeInfoPreviewBanner));

    public static string DirectoryView_FirstTimeInfoPreviewBanner(CultureInfo culture) => AnalyticsResources.Get(nameof (DirectoryView_FirstTimeInfoPreviewBanner), culture);

    public static string DirectoryView_FirstTimeInfoPreviewBanner_Warn() => AnalyticsResources.Get(nameof (DirectoryView_FirstTimeInfoPreviewBanner_Warn));

    public static string DirectoryView_FirstTimeInfoPreviewBanner_Warn(CultureInfo culture) => AnalyticsResources.Get(nameof (DirectoryView_FirstTimeInfoPreviewBanner_Warn), culture);

    public static string DirectoryView_FirstTimeInfoPreviewBannerLink() => AnalyticsResources.Get(nameof (DirectoryView_FirstTimeInfoPreviewBannerLink));

    public static string DirectoryView_FirstTimeInfoPreviewBannerLink(CultureInfo culture) => AnalyticsResources.Get(nameof (DirectoryView_FirstTimeInfoPreviewBannerLink), culture);

    public static string AnalyticsViews_Title() => AnalyticsResources.Get(nameof (AnalyticsViews_Title));

    public static string AnalyticsViews_Title(CultureInfo culture) => AnalyticsResources.Get(nameof (AnalyticsViews_Title), culture);

    public static string Loading() => AnalyticsResources.Get(nameof (Loading));

    public static string Loading(CultureInfo culture) => AnalyticsResources.Get(nameof (Loading), culture);

    public static string NoResultsFound() => AnalyticsResources.Get(nameof (NoResultsFound));

    public static string NoResultsFound(CultureInfo culture) => AnalyticsResources.Get(nameof (NoResultsFound), culture);

    public static string DescriptionColumn() => AnalyticsResources.Get(nameof (DescriptionColumn));

    public static string DescriptionColumn(CultureInfo culture) => AnalyticsResources.Get(nameof (DescriptionColumn), culture);

    public static string LastModifiedByColumn() => AnalyticsResources.Get(nameof (LastModifiedByColumn));

    public static string LastModifiedByColumn(CultureInfo culture) => AnalyticsResources.Get(nameof (LastModifiedByColumn), culture);

    public static string NameColumn() => AnalyticsResources.Get(nameof (NameColumn));

    public static string NameColumn(CultureInfo culture) => AnalyticsResources.Get(nameof (NameColumn), culture);

    public static string AddWITTypeButtonText() => AnalyticsResources.Get(nameof (AddWITTypeButtonText));

    public static string AddWITTypeButtonText(CultureInfo culture) => AnalyticsResources.Get(nameof (AddWITTypeButtonText), culture);

    public static string FieldsNavLabel() => AnalyticsResources.Get(nameof (FieldsNavLabel));

    public static string FieldsNavLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (FieldsNavLabel), culture);

    public static string GeneralNavLabel() => AnalyticsResources.Get(nameof (GeneralNavLabel));

    public static string GeneralNavLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (GeneralNavLabel), culture);

    public static string HistoryNavLabel() => AnalyticsResources.Get(nameof (HistoryNavLabel));

    public static string HistoryNavLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (HistoryNavLabel), culture);

    public static string VerificationNavLabel() => AnalyticsResources.Get(nameof (VerificationNavLabel));

    public static string VerificationNavLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (VerificationNavLabel), culture);

    public static string ViewContextMenu_Edit() => AnalyticsResources.Get(nameof (ViewContextMenu_Edit));

    public static string ViewContextMenu_Edit(CultureInfo culture) => AnalyticsResources.Get(nameof (ViewContextMenu_Edit), culture);

    public static string ViewContextMenu_Copy() => AnalyticsResources.Get(nameof (ViewContextMenu_Copy));

    public static string ViewContextMenu_Copy(CultureInfo culture) => AnalyticsResources.Get(nameof (ViewContextMenu_Copy), culture);

    public static string ViewContextMenu_Delete() => AnalyticsResources.Get(nameof (ViewContextMenu_Delete));

    public static string ViewContextMenu_Delete(CultureInfo culture) => AnalyticsResources.Get(nameof (ViewContextMenu_Delete), culture);

    public static string ViewContextMenu_Security() => AnalyticsResources.Get(nameof (ViewContextMenu_Security));

    public static string ViewContextMenu_Security(CultureInfo culture) => AnalyticsResources.Get(nameof (ViewContextMenu_Security), culture);

    public static string WorkItemsNavLabel() => AnalyticsResources.Get(nameof (WorkItemsNavLabel));

    public static string WorkItemsNavLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (WorkItemsNavLabel), culture);

    public static string EditPanel_Header(object arg0) => AnalyticsResources.Format(nameof (EditPanel_Header), arg0);

    public static string EditPanel_Header(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (EditPanel_Header), culture, arg0);

    public static string ViewPanel_Edit() => AnalyticsResources.Get(nameof (ViewPanel_Edit));

    public static string ViewPanel_Edit(CultureInfo culture) => AnalyticsResources.Get(nameof (ViewPanel_Edit), culture);

    public static string ViewPanel_EditAriaLabel() => AnalyticsResources.Get(nameof (ViewPanel_EditAriaLabel));

    public static string ViewPanel_EditAriaLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (ViewPanel_EditAriaLabel), culture);

    public static string ViewPanel_ViewId() => AnalyticsResources.Get(nameof (ViewPanel_ViewId));

    public static string ViewPanel_ViewId(CultureInfo culture) => AnalyticsResources.Get(nameof (ViewPanel_ViewId), culture);

    public static string Directory_AddViewButton() => AnalyticsResources.Get(nameof (Directory_AddViewButton));

    public static string Directory_AddViewButton(CultureInfo culture) => AnalyticsResources.Get(nameof (Directory_AddViewButton), culture);

    public static string AriaDescriptionDetailPanelSaveButton() => AnalyticsResources.Get(nameof (AriaDescriptionDetailPanelSaveButton));

    public static string AriaDescriptionDetailPanelSaveButton(CultureInfo culture) => AnalyticsResources.Get(nameof (AriaDescriptionDetailPanelSaveButton), culture);

    public static string DetailPanelSaveButtonText() => AnalyticsResources.Get(nameof (DetailPanelSaveButtonText));

    public static string DetailPanelSaveButtonText(CultureInfo culture) => AnalyticsResources.Get(nameof (DetailPanelSaveButtonText), culture);

    public static string AriaDescriptionDetailPanelCancelButton() => AnalyticsResources.Get(nameof (AriaDescriptionDetailPanelCancelButton));

    public static string AriaDescriptionDetailPanelCancelButton(CultureInfo culture) => AnalyticsResources.Get(nameof (AriaDescriptionDetailPanelCancelButton), culture);

    public static string DetailPanelCancelButtonText() => AnalyticsResources.Get(nameof (DetailPanelCancelButtonText));

    public static string DetailPanelCancelButtonText(CultureInfo culture) => AnalyticsResources.Get(nameof (DetailPanelCancelButtonText), culture);

    public static string Overview_Header() => AnalyticsResources.Get(nameof (Overview_Header));

    public static string Overview_Header(CultureInfo culture) => AnalyticsResources.Get(nameof (Overview_Header), culture);

    public static string General_Name() => AnalyticsResources.Get(nameof (General_Name));

    public static string General_Name(CultureInfo culture) => AnalyticsResources.Get(nameof (General_Name), culture);

    public static string General_Name_Placeholder() => AnalyticsResources.Get(nameof (General_Name_Placeholder));

    public static string General_Name_Placeholder(CultureInfo culture) => AnalyticsResources.Get(nameof (General_Name_Placeholder), culture);

    public static string General_Description() => AnalyticsResources.Get(nameof (General_Description));

    public static string General_Description(CultureInfo culture) => AnalyticsResources.Get(nameof (General_Description), culture);

    public static string General_Description_Placeholder() => AnalyticsResources.Get(nameof (General_Description_Placeholder));

    public static string General_Description_Placeholder(CultureInfo culture) => AnalyticsResources.Get(nameof (General_Description_Placeholder), culture);

    public static string General_Description_Error_Too_Long() => AnalyticsResources.Get(nameof (General_Description_Error_Too_Long));

    public static string General_Description_Error_Too_Long(CultureInfo culture) => AnalyticsResources.Get(nameof (General_Description_Error_Too_Long), culture);

    public static string Sharing_PrivateView() => AnalyticsResources.Get(nameof (Sharing_PrivateView));

    public static string Sharing_PrivateView(CultureInfo culture) => AnalyticsResources.Get(nameof (Sharing_PrivateView), culture);

    public static string Sharing_PrivateView_Description() => AnalyticsResources.Get(nameof (Sharing_PrivateView_Description));

    public static string Sharing_PrivateView_Description(CultureInfo culture) => AnalyticsResources.Get(nameof (Sharing_PrivateView_Description), culture);

    public static string Sharing_SharedView() => AnalyticsResources.Get(nameof (Sharing_SharedView));

    public static string Sharing_SharedView(CultureInfo culture) => AnalyticsResources.Get(nameof (Sharing_SharedView), culture);

    public static string Sharing_SharedView_Description() => AnalyticsResources.Get(nameof (Sharing_SharedView_Description));

    public static string Sharing_SharedView_Description(CultureInfo culture) => AnalyticsResources.Get(nameof (Sharing_SharedView_Description), culture);

    public static string Sharing_Header() => AnalyticsResources.Get(nameof (Sharing_Header));

    public static string Sharing_Header(CultureInfo culture) => AnalyticsResources.Get(nameof (Sharing_Header), culture);

    public static string FieldOptions_Header() => AnalyticsResources.Get(nameof (FieldOptions_Header));

    public static string FieldOptions_Header(CultureInfo culture) => AnalyticsResources.Get(nameof (FieldOptions_Header), culture);

    public static string FieldOptions_CommonChoice() => AnalyticsResources.Get(nameof (FieldOptions_CommonChoice));

    public static string FieldOptions_CommonChoice(CultureInfo culture) => AnalyticsResources.Get(nameof (FieldOptions_CommonChoice), culture);

    public static string FieldOptions_CommonChoice_Description() => AnalyticsResources.Get(nameof (FieldOptions_CommonChoice_Description));

    public static string FieldOptions_CommonChoice_Description(CultureInfo culture) => AnalyticsResources.Get(nameof (FieldOptions_CommonChoice_Description), culture);

    public static string FieldOptions_CustomChoice() => AnalyticsResources.Get(nameof (FieldOptions_CustomChoice));

    public static string FieldOptions_CustomChoice(CultureInfo culture) => AnalyticsResources.Get(nameof (FieldOptions_CustomChoice), culture);

    public static string FieldOptions_CustomChoice_Description() => AnalyticsResources.Get(nameof (FieldOptions_CustomChoice_Description));

    public static string FieldOptions_CustomChoice_Description(CultureInfo culture) => AnalyticsResources.Get(nameof (FieldOptions_CustomChoice_Description), culture);

    public static string Fields_Header() => AnalyticsResources.Get(nameof (Fields_Header));

    public static string Fields_Header(CultureInfo culture) => AnalyticsResources.Get(nameof (Fields_Header), culture);

    public static string Day_Friday() => AnalyticsResources.Get(nameof (Day_Friday));

    public static string Day_Friday(CultureInfo culture) => AnalyticsResources.Get(nameof (Day_Friday), culture);

    public static string Day_Monday() => AnalyticsResources.Get(nameof (Day_Monday));

    public static string Day_Monday(CultureInfo culture) => AnalyticsResources.Get(nameof (Day_Monday), culture);

    public static string Day_Saturday() => AnalyticsResources.Get(nameof (Day_Saturday));

    public static string Day_Saturday(CultureInfo culture) => AnalyticsResources.Get(nameof (Day_Saturday), culture);

    public static string Day_Sunday() => AnalyticsResources.Get(nameof (Day_Sunday));

    public static string Day_Sunday(CultureInfo culture) => AnalyticsResources.Get(nameof (Day_Sunday), culture);

    public static string Day_Thursday() => AnalyticsResources.Get(nameof (Day_Thursday));

    public static string Day_Thursday(CultureInfo culture) => AnalyticsResources.Get(nameof (Day_Thursday), culture);

    public static string Day_Tuesday() => AnalyticsResources.Get(nameof (Day_Tuesday));

    public static string Day_Tuesday(CultureInfo culture) => AnalyticsResources.Get(nameof (Day_Tuesday), culture);

    public static string Day_Wednesday() => AnalyticsResources.Get(nameof (Day_Wednesday));

    public static string Day_Wednesday(CultureInfo culture) => AnalyticsResources.Get(nameof (Day_Wednesday), culture);

    public static string Granularity_Daily() => AnalyticsResources.Get(nameof (Granularity_Daily));

    public static string Granularity_Daily(CultureInfo culture) => AnalyticsResources.Get(nameof (Granularity_Daily), culture);

    public static string Granularity_Header() => AnalyticsResources.Get(nameof (Granularity_Header));

    public static string Granularity_Header(CultureInfo culture) => AnalyticsResources.Get(nameof (Granularity_Header), culture);

    public static string Granularity_Monthly() => AnalyticsResources.Get(nameof (Granularity_Monthly));

    public static string Granularity_Monthly(CultureInfo culture) => AnalyticsResources.Get(nameof (Granularity_Monthly), culture);

    public static string Granularity_Weekly() => AnalyticsResources.Get(nameof (Granularity_Weekly));

    public static string Granularity_Weekly(CultureInfo culture) => AnalyticsResources.Get(nameof (Granularity_Weekly), culture);

    public static string Granularity_Description() => AnalyticsResources.Get(nameof (Granularity_Description));

    public static string Granularity_Description(CultureInfo culture) => AnalyticsResources.Get(nameof (Granularity_Description), culture);

    public static string Summary_LastResults_Header() => AnalyticsResources.Get(nameof (Summary_LastResults_Header));

    public static string Summary_LastResults_Header(CultureInfo culture) => AnalyticsResources.Get(nameof (Summary_LastResults_Header), culture);

    public static string Summary_Teams_Header() => AnalyticsResources.Get(nameof (Summary_Teams_Header));

    public static string Summary_Teams_Header(CultureInfo culture) => AnalyticsResources.Get(nameof (Summary_Teams_Header), culture);

    public static string Summary_AreaPaths_Header() => AnalyticsResources.Get(nameof (Summary_AreaPaths_Header));

    public static string Summary_AreaPaths_Header(CultureInfo culture) => AnalyticsResources.Get(nameof (Summary_AreaPaths_Header), culture);

    public static string Summary_FilterCriteria_Header() => AnalyticsResources.Get(nameof (Summary_FilterCriteria_Header));

    public static string Summary_FilterCriteria_Header(CultureInfo culture) => AnalyticsResources.Get(nameof (Summary_FilterCriteria_Header), culture);

    public static string Summary_History_Header() => AnalyticsResources.Get(nameof (Summary_History_Header));

    public static string Summary_History_Header(CultureInfo culture) => AnalyticsResources.Get(nameof (Summary_History_Header), culture);

    public static string Summary_WorkItems_Header() => AnalyticsResources.Get(nameof (Summary_WorkItems_Header));

    public static string Summary_WorkItems_Header(CultureInfo culture) => AnalyticsResources.Get(nameof (Summary_WorkItems_Header), culture);

    public static string Summary_Fields_Header() => AnalyticsResources.Get(nameof (Summary_Fields_Header));

    public static string Summary_Fields_Header(CultureInfo culture) => AnalyticsResources.Get(nameof (Summary_Fields_Header), culture);

    public static string AllTeams() => AnalyticsResources.Get(nameof (AllTeams));

    public static string AllTeams(CultureInfo culture) => AnalyticsResources.Get(nameof (AllTeams), culture);

    public static string History_TooMuchDataWarning() => AnalyticsResources.Get(nameof (History_TooMuchDataWarning));

    public static string History_TooMuchDataWarning(CultureInfo culture) => AnalyticsResources.Get(nameof (History_TooMuchDataWarning), culture);

    public static string History_All() => AnalyticsResources.Get(nameof (History_All));

    public static string History_All(CultureInfo culture) => AnalyticsResources.Get(nameof (History_All), culture);

    public static string History_Date() => AnalyticsResources.Get(nameof (History_Date));

    public static string History_Date(CultureInfo culture) => AnalyticsResources.Get(nameof (History_Date), culture);

    public static string History_Header() => AnalyticsResources.Get(nameof (History_Header));

    public static string History_Header(CultureInfo culture) => AnalyticsResources.Get(nameof (History_Header), culture);

    public static string History_None() => AnalyticsResources.Get(nameof (History_None));

    public static string History_None(CultureInfo culture) => AnalyticsResources.Get(nameof (History_None), culture);

    public static string History_Rolling() => AnalyticsResources.Get(nameof (History_Rolling));

    public static string History_Rolling(CultureInfo culture) => AnalyticsResources.Get(nameof (History_Rolling), culture);

    public static string Rolling_Header() => AnalyticsResources.Get(nameof (Rolling_Header));

    public static string Rolling_Header(CultureInfo culture) => AnalyticsResources.Get(nameof (Rolling_Header), culture);

    public static string History_Section_Description() => AnalyticsResources.Get(nameof (History_Section_Description));

    public static string History_Section_Description(CultureInfo culture) => AnalyticsResources.Get(nameof (History_Section_Description), culture);

    public static string History_Date_Range_For_Summary_Template(object arg0, object arg1) => AnalyticsResources.Format(nameof (History_Date_Range_For_Summary_Template), arg0, arg1);

    public static string History_Date_Range_For_Summary_Template(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (History_Date_Range_For_Summary_Template), culture, arg0, arg1);
    }

    public static string History_Date_Range_To_Present_For_Summary_Template(object arg0) => AnalyticsResources.Format(nameof (History_Date_Range_To_Present_For_Summary_Template), arg0);

    public static string History_Date_Range_To_Present_For_Summary_Template(
      object arg0,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (History_Date_Range_To_Present_For_Summary_Template), culture, arg0);
    }

    public static string History_Rolling_For_Summary_Template(object arg0) => AnalyticsResources.Format(nameof (History_Rolling_For_Summary_Template), arg0);

    public static string History_Rolling_For_Summary_Template(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (History_Rolling_For_Summary_Template), culture, arg0);

    public static string History_IgnoreOldCompletedItemsForNoOrAllHistory() => AnalyticsResources.Get(nameof (History_IgnoreOldCompletedItemsForNoOrAllHistory));

    public static string History_IgnoreOldCompletedItemsForNoOrAllHistory(CultureInfo culture) => AnalyticsResources.Get(nameof (History_IgnoreOldCompletedItemsForNoOrAllHistory), culture);

    public static string History_IgnoreOldCompletedItemsForRolling() => AnalyticsResources.Get(nameof (History_IgnoreOldCompletedItemsForRolling));

    public static string History_IgnoreOldCompletedItemsForRolling(CultureInfo culture) => AnalyticsResources.Get(nameof (History_IgnoreOldCompletedItemsForRolling), culture);

    public static string History_IgnoreOldCompletedItemsForRange() => AnalyticsResources.Get(nameof (History_IgnoreOldCompletedItemsForRange));

    public static string History_IgnoreOldCompletedItemsForRange(CultureInfo culture) => AnalyticsResources.Get(nameof (History_IgnoreOldCompletedItemsForRange), culture);

    public static string AllPivot_ZeroData_PrimaryText() => AnalyticsResources.Get(nameof (AllPivot_ZeroData_PrimaryText));

    public static string AllPivot_ZeroData_PrimaryText(CultureInfo culture) => AnalyticsResources.Get(nameof (AllPivot_ZeroData_PrimaryText), culture);

    public static string AllPivot_ZeroData_SecondaryText() => AnalyticsResources.Get(nameof (AllPivot_ZeroData_SecondaryText));

    public static string AllPivot_ZeroData_SecondaryText(CultureInfo culture) => AnalyticsResources.Get(nameof (AllPivot_ZeroData_SecondaryText), culture);

    public static string FavoritePivot_ZeroData_ActionText() => AnalyticsResources.Get(nameof (FavoritePivot_ZeroData_ActionText));

    public static string FavoritePivot_ZeroData_ActionText(CultureInfo culture) => AnalyticsResources.Get(nameof (FavoritePivot_ZeroData_ActionText), culture);

    public static string FavoritePivot_ZeroData_PrimaryText() => AnalyticsResources.Get(nameof (FavoritePivot_ZeroData_PrimaryText));

    public static string FavoritePivot_ZeroData_PrimaryText(CultureInfo culture) => AnalyticsResources.Get(nameof (FavoritePivot_ZeroData_PrimaryText), culture);

    public static string FavoritePivot_ZeroData_SecondaryText(object arg0) => AnalyticsResources.Format(nameof (FavoritePivot_ZeroData_SecondaryText), arg0);

    public static string FavoritePivot_ZeroData_SecondaryText(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (FavoritePivot_ZeroData_SecondaryText), culture, arg0);

    public static string DateRangePicker_From() => AnalyticsResources.Get(nameof (DateRangePicker_From));

    public static string DateRangePicker_From(CultureInfo culture) => AnalyticsResources.Get(nameof (DateRangePicker_From), culture);

    public static string DateRangePicker_To() => AnalyticsResources.Get(nameof (DateRangePicker_To));

    public static string DateRangePicker_To(CultureInfo culture) => AnalyticsResources.Get(nameof (DateRangePicker_To), culture);

    public static string DateRangePicker_ToPresent() => AnalyticsResources.Get(nameof (DateRangePicker_ToPresent));

    public static string DateRangePicker_ToPresent(CultureInfo culture) => AnalyticsResources.Get(nameof (DateRangePicker_ToPresent), culture);

    public static string WorkItemTypesPickerDescription() => AnalyticsResources.Get(nameof (WorkItemTypesPickerDescription));

    public static string WorkItemTypesPickerDescription(CultureInfo culture) => AnalyticsResources.Get(nameof (WorkItemTypesPickerDescription), culture);

    public static string DirectoryView_DeleteDialogTitle() => AnalyticsResources.Get(nameof (DirectoryView_DeleteDialogTitle));

    public static string DirectoryView_DeleteDialogTitle(CultureInfo culture) => AnalyticsResources.Get(nameof (DirectoryView_DeleteDialogTitle), culture);

    public static string DirectoryView_DeleteDialogSubText(object arg0) => AnalyticsResources.Format(nameof (DirectoryView_DeleteDialogSubText), arg0);

    public static string DirectoryView_DeleteDialogSubText(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (DirectoryView_DeleteDialogSubText), culture, arg0);

    public static string DirectoryView_DeleteDialogPrimaryButtonText() => AnalyticsResources.Get(nameof (DirectoryView_DeleteDialogPrimaryButtonText));

    public static string DirectoryView_DeleteDialogPrimaryButtonText(CultureInfo culture) => AnalyticsResources.Get(nameof (DirectoryView_DeleteDialogPrimaryButtonText), culture);

    public static string DirectoryView_DeleteDialogSecondaryButtonText() => AnalyticsResources.Get(nameof (DirectoryView_DeleteDialogSecondaryButtonText));

    public static string DirectoryView_DeleteDialogSecondaryButtonText(CultureInfo culture) => AnalyticsResources.Get(nameof (DirectoryView_DeleteDialogSecondaryButtonText), culture);

    public static string FieldSelectionErrorMessage() => AnalyticsResources.Get(nameof (FieldSelectionErrorMessage));

    public static string FieldSelectionErrorMessage(CultureInfo culture) => AnalyticsResources.Get(nameof (FieldSelectionErrorMessage), culture);

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

    public static string NoLongerExists() => AnalyticsResources.Get(nameof (NoLongerExists));

    public static string NoLongerExists(CultureInfo culture) => AnalyticsResources.Get(nameof (NoLongerExists), culture);

    public static string MustProvideAName() => AnalyticsResources.Get(nameof (MustProvideAName));

    public static string MustProvideAName(CultureInfo culture) => AnalyticsResources.Get(nameof (MustProvideAName), culture);

    public static string MustChooseATeamErrorMessage() => AnalyticsResources.Get(nameof (MustChooseATeamErrorMessage));

    public static string MustChooseATeamErrorMessage(CultureInfo culture) => AnalyticsResources.Get(nameof (MustChooseATeamErrorMessage), culture);

    public static string WorkItems_TooMuchDataWarning() => AnalyticsResources.Get(nameof (WorkItems_TooMuchDataWarning));

    public static string WorkItems_TooMuchDataWarning(CultureInfo culture) => AnalyticsResources.Get(nameof (WorkItems_TooMuchDataWarning), culture);

    public static string General_NoSharedViewsPermissionsWarning() => AnalyticsResources.Get(nameof (General_NoSharedViewsPermissionsWarning));

    public static string General_NoSharedViewsPermissionsWarning(CultureInfo culture) => AnalyticsResources.Get(nameof (General_NoSharedViewsPermissionsWarning), culture);

    public static string WorkItems_TeamsOrAreaPaths_Title() => AnalyticsResources.Get(nameof (WorkItems_TeamsOrAreaPaths_Title));

    public static string WorkItems_TeamsOrAreaPaths_Title(CultureInfo culture) => AnalyticsResources.Get(nameof (WorkItems_TeamsOrAreaPaths_Title), culture);

    public static string WorkItems_TeamsOrAreaPaths_Description() => AnalyticsResources.Get(nameof (WorkItems_TeamsOrAreaPaths_Description));

    public static string WorkItems_TeamsOrAreaPaths_Description(CultureInfo culture) => AnalyticsResources.Get(nameof (WorkItems_TeamsOrAreaPaths_Description), culture);

    public static string WorkItems_TeamsOrAreaPaths_TeamsOption() => AnalyticsResources.Get(nameof (WorkItems_TeamsOrAreaPaths_TeamsOption));

    public static string WorkItems_TeamsOrAreaPaths_TeamsOption(CultureInfo culture) => AnalyticsResources.Get(nameof (WorkItems_TeamsOrAreaPaths_TeamsOption), culture);

    public static string WorkItems_TeamsOrAreaPaths_AreaPathOption() => AnalyticsResources.Get(nameof (WorkItems_TeamsOrAreaPaths_AreaPathOption));

    public static string WorkItems_TeamsOrAreaPaths_AreaPathOption(CultureInfo culture) => AnalyticsResources.Get(nameof (WorkItems_TeamsOrAreaPaths_AreaPathOption), culture);

    public static string VerificationFailedPrimaryText(object arg0) => AnalyticsResources.Format(nameof (VerificationFailedPrimaryText), arg0);

    public static string VerificationFailedPrimaryText(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (VerificationFailedPrimaryText), culture, arg0);

    public static string VerificationFailedTryAgainButtonAriaDescription() => AnalyticsResources.Get(nameof (VerificationFailedTryAgainButtonAriaDescription));

    public static string VerificationFailedTryAgainButtonAriaDescription(CultureInfo culture) => AnalyticsResources.Get(nameof (VerificationFailedTryAgainButtonAriaDescription), culture);

    public static string VerificationFailedTryAgainButtonText() => AnalyticsResources.Get(nameof (VerificationFailedTryAgainButtonText));

    public static string VerificationFailedTryAgainButtonText(CultureInfo culture) => AnalyticsResources.Get(nameof (VerificationFailedTryAgainButtonText), culture);

    public static string VerificationSucceededPrimaryText() => AnalyticsResources.Get(nameof (VerificationSucceededPrimaryText));

    public static string VerificationSucceededPrimaryText(CultureInfo culture) => AnalyticsResources.Get(nameof (VerificationSucceededPrimaryText), culture);

    public static string VerifiedAnalyticsViewValid() => AnalyticsResources.Get(nameof (VerifiedAnalyticsViewValid));

    public static string VerifiedAnalyticsViewValid(CultureInfo culture) => AnalyticsResources.Get(nameof (VerifiedAnalyticsViewValid), culture);

    public static string VerifyAnalyticsViewPrimaryText() => AnalyticsResources.Get(nameof (VerifyAnalyticsViewPrimaryText));

    public static string VerifyAnalyticsViewPrimaryText(CultureInfo culture) => AnalyticsResources.Get(nameof (VerifyAnalyticsViewPrimaryText), culture);

    public static string VerifyViewButtonAriaDescription() => AnalyticsResources.Get(nameof (VerifyViewButtonAriaDescription));

    public static string VerifyViewButtonAriaDescription(CultureInfo culture) => AnalyticsResources.Get(nameof (VerifyViewButtonAriaDescription), culture);

    public static string VerifyViewButtonText() => AnalyticsResources.Get(nameof (VerifyViewButtonText));

    public static string VerifyViewButtonText(CultureInfo culture) => AnalyticsResources.Get(nameof (VerifyViewButtonText), culture);

    public static string VerificationEstimatedLoadTime(object arg0) => AnalyticsResources.Format(nameof (VerificationEstimatedLoadTime), arg0);

    public static string VerificationEstimatedLoadTime(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (VerificationEstimatedLoadTime), culture, arg0);

    public static string VerificationTimeLessThanOneMinute() => AnalyticsResources.Get(nameof (VerificationTimeLessThanOneMinute));

    public static string VerificationTimeLessThanOneMinute(CultureInfo culture) => AnalyticsResources.Get(nameof (VerificationTimeLessThanOneMinute), culture);

    public static string VerificationTimeLessThanFiveMinutes() => AnalyticsResources.Get(nameof (VerificationTimeLessThanFiveMinutes));

    public static string VerificationTimeLessThanFiveMinutes(CultureInfo culture) => AnalyticsResources.Get(nameof (VerificationTimeLessThanFiveMinutes), culture);

    public static string VerificationTimeFiveMinutes() => AnalyticsResources.Get(nameof (VerificationTimeFiveMinutes));

    public static string VerificationTimeFiveMinutes(CultureInfo culture) => AnalyticsResources.Get(nameof (VerificationTimeFiveMinutes), culture);

    public static string VerificationTimeTenMinutes() => AnalyticsResources.Get(nameof (VerificationTimeTenMinutes));

    public static string VerificationTimeTenMinutes(CultureInfo culture) => AnalyticsResources.Get(nameof (VerificationTimeTenMinutes), culture);

    public static string VerificationTimeTwentyMinutes() => AnalyticsResources.Get(nameof (VerificationTimeTwentyMinutes));

    public static string VerificationTimeTwentyMinutes(CultureInfo culture) => AnalyticsResources.Get(nameof (VerificationTimeTwentyMinutes), culture);

    public static string VerificationTimeThirtyMinutes() => AnalyticsResources.Get(nameof (VerificationTimeThirtyMinutes));

    public static string VerificationTimeThirtyMinutes(CultureInfo culture) => AnalyticsResources.Get(nameof (VerificationTimeThirtyMinutes), culture);

    public static string VerificationTimeFortyFiveMinutes() => AnalyticsResources.Get(nameof (VerificationTimeFortyFiveMinutes));

    public static string VerificationTimeFortyFiveMinutes(CultureInfo culture) => AnalyticsResources.Get(nameof (VerificationTimeFortyFiveMinutes), culture);

    public static string VerificationTimeOneHour() => AnalyticsResources.Get(nameof (VerificationTimeOneHour));

    public static string VerificationTimeOneHour(CultureInfo culture) => AnalyticsResources.Get(nameof (VerificationTimeOneHour), culture);

    public static string VerificationTimeHourAndAHalf() => AnalyticsResources.Get(nameof (VerificationTimeHourAndAHalf));

    public static string VerificationTimeHourAndAHalf(CultureInfo culture) => AnalyticsResources.Get(nameof (VerificationTimeHourAndAHalf), culture);

    public static string VerificationTimeTwoHours() => AnalyticsResources.Get(nameof (VerificationTimeTwoHours));

    public static string VerificationTimeTwoHours(CultureInfo culture) => AnalyticsResources.Get(nameof (VerificationTimeTwoHours), culture);

    public static string VerificationTimeThreeHours() => AnalyticsResources.Get(nameof (VerificationTimeThreeHours));

    public static string VerificationTimeThreeHours(CultureInfo culture) => AnalyticsResources.Get(nameof (VerificationTimeThreeHours), culture);

    public static string VerificationTimeFourHours() => AnalyticsResources.Get(nameof (VerificationTimeFourHours));

    public static string VerificationTimeFourHours(CultureInfo culture) => AnalyticsResources.Get(nameof (VerificationTimeFourHours), culture);

    public static string VerificationTimeFivePlusHours() => AnalyticsResources.Get(nameof (VerificationTimeFivePlusHours));

    public static string VerificationTimeFivePlusHours(CultureInfo culture) => AnalyticsResources.Get(nameof (VerificationTimeFivePlusHours), culture);

    public static string VerificationEstimatedRows(object arg0) => AnalyticsResources.Format(nameof (VerificationEstimatedRows), arg0);

    public static string VerificationEstimatedRows(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (VerificationEstimatedRows), culture, arg0);

    public static string VerifyingSpinnerText() => AnalyticsResources.Get(nameof (VerifyingSpinnerText));

    public static string VerifyingSpinnerText(CultureInfo culture) => AnalyticsResources.Get(nameof (VerifyingSpinnerText), culture);

    public static string LastVerifiedPrimaryText(object arg0, object arg1) => AnalyticsResources.Format(nameof (LastVerifiedPrimaryText), arg0, arg1);

    public static string LastVerifiedPrimaryText(object arg0, object arg1, CultureInfo culture) => AnalyticsResources.Format(nameof (LastVerifiedPrimaryText), culture, arg0, arg1);

    public static string CancelVerificationButtonAriaLabel() => AnalyticsResources.Get(nameof (CancelVerificationButtonAriaLabel));

    public static string CancelVerificationButtonAriaLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (CancelVerificationButtonAriaLabel), culture);

    public static string CancelVerificationButtonText() => AnalyticsResources.Get(nameof (CancelVerificationButtonText));

    public static string CancelVerificationButtonText(CultureInfo culture) => AnalyticsResources.Get(nameof (CancelVerificationButtonText), culture);

    public static string EditDisabledWarning() => AnalyticsResources.Get(nameof (EditDisabledWarning));

    public static string EditDisabledWarning(CultureInfo culture) => AnalyticsResources.Get(nameof (EditDisabledWarning), culture);

    public static string SharedViewCreateDisabledWarning() => AnalyticsResources.Get(nameof (SharedViewCreateDisabledWarning));

    public static string SharedViewCreateDisabledWarning(CultureInfo culture) => AnalyticsResources.Get(nameof (SharedViewCreateDisabledWarning), culture);

    public static string VerificationDisabled() => AnalyticsResources.Get(nameof (VerificationDisabled));

    public static string VerificationDisabled(CultureInfo culture) => AnalyticsResources.Get(nameof (VerificationDisabled), culture);

    public static string GenericServerError() => AnalyticsResources.Get(nameof (GenericServerError));

    public static string GenericServerError(CultureInfo culture) => AnalyticsResources.Get(nameof (GenericServerError), culture);

    public static string LastModifiedByRow_LastAddedFriendlyText(object arg0, object arg1) => AnalyticsResources.Format(nameof (LastModifiedByRow_LastAddedFriendlyText), arg0, arg1);

    public static string LastModifiedByRow_LastAddedFriendlyText(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (LastModifiedByRow_LastAddedFriendlyText), culture, arg0, arg1);
    }

    public static string LastModifiedByRow_LastAddedLocaleDateText(object arg0, object arg1) => AnalyticsResources.Format(nameof (LastModifiedByRow_LastAddedLocaleDateText), arg0, arg1);

    public static string LastModifiedByRow_LastAddedLocaleDateText(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (LastModifiedByRow_LastAddedLocaleDateText), culture, arg0, arg1);
    }

    public static string LastModifiedByRow_LastUpdatedFriendlyText(object arg0, object arg1) => AnalyticsResources.Format(nameof (LastModifiedByRow_LastUpdatedFriendlyText), arg0, arg1);

    public static string LastModifiedByRow_LastUpdatedFriendlyText(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (LastModifiedByRow_LastUpdatedFriendlyText), culture, arg0, arg1);
    }

    public static string LastModifiedByRow_LastUpdatedLocaleDateText(object arg0, object arg1) => AnalyticsResources.Format(nameof (LastModifiedByRow_LastUpdatedLocaleDateText), arg0, arg1);

    public static string LastModifiedByRow_LastUpdatedLocaleDateText(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (LastModifiedByRow_LastUpdatedLocaleDateText), culture, arg0, arg1);
    }

    public static string AxFaultIn_ZeroData_AxViewsDocLink() => AnalyticsResources.Get(nameof (AxFaultIn_ZeroData_AxViewsDocLink));

    public static string AxFaultIn_ZeroData_AxViewsDocLink(CultureInfo culture) => AnalyticsResources.Get(nameof (AxFaultIn_ZeroData_AxViewsDocLink), culture);

    public static string AxFaultIn_ZeroData_PrimaryText() => AnalyticsResources.Get(nameof (AxFaultIn_ZeroData_PrimaryText));

    public static string AxFaultIn_ZeroData_PrimaryText(CultureInfo culture) => AnalyticsResources.Get(nameof (AxFaultIn_ZeroData_PrimaryText), culture);

    public static string AxFaultIn_ZeroData_SecondaryText() => AnalyticsResources.Get(nameof (AxFaultIn_ZeroData_SecondaryText));

    public static string AxFaultIn_ZeroData_SecondaryText(CultureInfo culture) => AnalyticsResources.Get(nameof (AxFaultIn_ZeroData_SecondaryText), culture);

    public static string QuoteFormat(object arg0) => AnalyticsResources.Format(nameof (QuoteFormat), arg0);

    public static string QuoteFormat(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (QuoteFormat), culture, arg0);

    public static string CommonFields_Header() => AnalyticsResources.Get(nameof (CommonFields_Header));

    public static string CommonFields_Header(CultureInfo culture) => AnalyticsResources.Get(nameof (CommonFields_Header), culture);

    public static string PrivateView_TooltipDescription() => AnalyticsResources.Get(nameof (PrivateView_TooltipDescription));

    public static string PrivateView_TooltipDescription(CultureInfo culture) => AnalyticsResources.Get(nameof (PrivateView_TooltipDescription), culture);

    public static string PrivateView_TooltipHeader() => AnalyticsResources.Get(nameof (PrivateView_TooltipHeader));

    public static string PrivateView_TooltipHeader(CultureInfo culture) => AnalyticsResources.Get(nameof (PrivateView_TooltipHeader), culture);

    public static string SharedView_TooltipDescription() => AnalyticsResources.Get(nameof (SharedView_TooltipDescription));

    public static string SharedView_TooltipDescription(CultureInfo culture) => AnalyticsResources.Get(nameof (SharedView_TooltipDescription), culture);

    public static string SharedView_TooltipHeader() => AnalyticsResources.Get(nameof (SharedView_TooltipHeader));

    public static string SharedView_TooltipHeader(CultureInfo culture) => AnalyticsResources.Get(nameof (SharedView_TooltipHeader), culture);

    public static string GenericErrorMessage(object arg0) => AnalyticsResources.Format(nameof (GenericErrorMessage), arg0);

    public static string GenericErrorMessage(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (GenericErrorMessage), culture, arg0);

    public static string ExpectedAnIntegerValue() => AnalyticsResources.Get(nameof (ExpectedAnIntegerValue));

    public static string ExpectedAnIntegerValue(CultureInfo culture) => AnalyticsResources.Get(nameof (ExpectedAnIntegerValue), culture);

    public static string ExpectedANumberValue() => AnalyticsResources.Get(nameof (ExpectedANumberValue));

    public static string ExpectedANumberValue(CultureInfo culture) => AnalyticsResources.Get(nameof (ExpectedANumberValue), culture);

    public static string Panel_NewViewHeader() => AnalyticsResources.Get(nameof (Panel_NewViewHeader));

    public static string Panel_NewViewHeader(CultureInfo culture) => AnalyticsResources.Get(nameof (Panel_NewViewHeader), culture);

    public static string MetadataCard_AllBacklogsAndWorkItems() => AnalyticsResources.Get(nameof (MetadataCard_AllBacklogsAndWorkItems));

    public static string MetadataCard_AllBacklogsAndWorkItems(CultureInfo culture) => AnalyticsResources.Get(nameof (MetadataCard_AllBacklogsAndWorkItems), culture);

    public static string AriaDescriptionDetailPanelContinueButton() => AnalyticsResources.Get(nameof (AriaDescriptionDetailPanelContinueButton));

    public static string AriaDescriptionDetailPanelContinueButton(CultureInfo culture) => AnalyticsResources.Get(nameof (AriaDescriptionDetailPanelContinueButton), culture);

    public static string DetailPanelContinueButtonText() => AnalyticsResources.Get(nameof (DetailPanelContinueButtonText));

    public static string DetailPanelContinueButtonText(CultureInfo culture) => AnalyticsResources.Get(nameof (DetailPanelContinueButtonText), culture);

    public static string GeneralSection_SectionInfoLinkText() => AnalyticsResources.Get(nameof (GeneralSection_SectionInfoLinkText));

    public static string GeneralSection_SectionInfoLinkText(CultureInfo culture) => AnalyticsResources.Get(nameof (GeneralSection_SectionInfoLinkText), culture);

    public static string GeneralSection_SectionInfoText() => AnalyticsResources.Get(nameof (GeneralSection_SectionInfoText));

    public static string GeneralSection_SectionInfoText(CultureInfo culture) => AnalyticsResources.Get(nameof (GeneralSection_SectionInfoText), culture);

    public static string VerificationSection_LearnMoreLink() => AnalyticsResources.Get(nameof (VerificationSection_LearnMoreLink));

    public static string VerificationSection_LearnMoreLink(CultureInfo culture) => AnalyticsResources.Get(nameof (VerificationSection_LearnMoreLink), culture);

    public static string HistorySection_GranularityInfoLinkAriaDescription() => AnalyticsResources.Get(nameof (HistorySection_GranularityInfoLinkAriaDescription));

    public static string HistorySection_GranularityInfoLinkAriaDescription(CultureInfo culture) => AnalyticsResources.Get(nameof (HistorySection_GranularityInfoLinkAriaDescription), culture);

    public static string HistorySection_HistoryInfoLinkAriaDescription() => AnalyticsResources.Get(nameof (HistorySection_HistoryInfoLinkAriaDescription));

    public static string HistorySection_HistoryInfoLinkAriaDescription(CultureInfo culture) => AnalyticsResources.Get(nameof (HistorySection_HistoryInfoLinkAriaDescription), culture);

    public static string SharingSection_InfoLinkAriaDescription() => AnalyticsResources.Get(nameof (SharingSection_InfoLinkAriaDescription));

    public static string SharingSection_InfoLinkAriaDescription(CultureInfo culture) => AnalyticsResources.Get(nameof (SharingSection_InfoLinkAriaDescription), culture);

    public static string HistorySection_InvalidRollingPeriod() => AnalyticsResources.Get(nameof (HistorySection_InvalidRollingPeriod));

    public static string HistorySection_InvalidRollingPeriod(CultureInfo culture) => AnalyticsResources.Get(nameof (HistorySection_InvalidRollingPeriod), culture);

    public static string HistorySection_StartDateLaterThanEndDate() => AnalyticsResources.Get(nameof (HistorySection_StartDateLaterThanEndDate));

    public static string HistorySection_StartDateLaterThanEndDate(CultureInfo culture) => AnalyticsResources.Get(nameof (HistorySection_StartDateLaterThanEndDate), culture);

    public static string DeleteDialog_CancelDeleteAriaDescription() => AnalyticsResources.Get(nameof (DeleteDialog_CancelDeleteAriaDescription));

    public static string DeleteDialog_CancelDeleteAriaDescription(CultureInfo culture) => AnalyticsResources.Get(nameof (DeleteDialog_CancelDeleteAriaDescription), culture);

    public static string DeleteDialog_DeleteViewAriaDescription() => AnalyticsResources.Get(nameof (DeleteDialog_DeleteViewAriaDescription));

    public static string DeleteDialog_DeleteViewAriaDescription(CultureInfo culture) => AnalyticsResources.Get(nameof (DeleteDialog_DeleteViewAriaDescription), culture);

    public static string DetailPanel_CloseButtonAriaLabel() => AnalyticsResources.Get(nameof (DetailPanel_CloseButtonAriaLabel));

    public static string DetailPanel_CloseButtonAriaLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (DetailPanel_CloseButtonAriaLabel), culture);

    public static string VerificationFailureAnnouncement() => AnalyticsResources.Get(nameof (VerificationFailureAnnouncement));

    public static string VerificationFailureAnnouncement(CultureInfo culture) => AnalyticsResources.Get(nameof (VerificationFailureAnnouncement), culture);

    public static string VerificationSucceededAnnouncement(object arg0) => AnalyticsResources.Format(nameof (VerificationSucceededAnnouncement), arg0);

    public static string VerificationSucceededAnnouncement(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (VerificationSucceededAnnouncement), culture, arg0);

    public static string DirectoryViewGroupHeader_Collapse() => AnalyticsResources.Get(nameof (DirectoryViewGroupHeader_Collapse));

    public static string DirectoryViewGroupHeader_Collapse(CultureInfo culture) => AnalyticsResources.Get(nameof (DirectoryViewGroupHeader_Collapse), culture);

    public static string DirectoryViewGroupHeader_Expand() => AnalyticsResources.Get(nameof (DirectoryViewGroupHeader_Expand));

    public static string DirectoryViewGroupHeader_Expand(CultureInfo culture) => AnalyticsResources.Get(nameof (DirectoryViewGroupHeader_Expand), culture);

    public static string DirectoryView_GroupHeaderName(object arg0, object arg1) => AnalyticsResources.Format(nameof (DirectoryView_GroupHeaderName), arg0, arg1);

    public static string DirectoryView_GroupHeaderName(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (DirectoryView_GroupHeaderName), culture, arg0, arg1);
    }

    public static string DayOfTheWeekAriaLabel() => AnalyticsResources.Get(nameof (DayOfTheWeekAriaLabel));

    public static string DayOfTheWeekAriaLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (DayOfTheWeekAriaLabel), culture);

    public static string DismissMessageBarAriaLabel() => AnalyticsResources.Get(nameof (DismissMessageBarAriaLabel));

    public static string DismissMessageBarAriaLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (DismissMessageBarAriaLabel), culture);

    public static string DirectoryView_FilterResultsMessage(object arg0) => AnalyticsResources.Format(nameof (DirectoryView_FilterResultsMessage), arg0);

    public static string DirectoryView_FilterResultsMessage(object arg0, CultureInfo culture) => AnalyticsResources.Format(nameof (DirectoryView_FilterResultsMessage), culture, arg0);

    public static string FieldOptions_AriaLabel() => AnalyticsResources.Get(nameof (FieldOptions_AriaLabel));

    public static string FieldOptions_AriaLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (FieldOptions_AriaLabel), culture);

    public static string StartDate() => AnalyticsResources.Get(nameof (StartDate));

    public static string StartDate(CultureInfo culture) => AnalyticsResources.Get(nameof (StartDate), culture);

    public static string VerifyingAriaLabel() => AnalyticsResources.Get(nameof (VerifyingAriaLabel));

    public static string VerifyingAriaLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (VerifyingAriaLabel), culture);

    public static string AnalyticsViews_PreviewTitle() => AnalyticsResources.Get(nameof (AnalyticsViews_PreviewTitle));

    public static string AnalyticsViews_PreviewTitle(CultureInfo culture) => AnalyticsResources.Get(nameof (AnalyticsViews_PreviewTitle), culture);

    public static string DirectoryView_FirstTimeInfoBannerMessageFormat(object arg0) => AnalyticsResources.Format(nameof (DirectoryView_FirstTimeInfoBannerMessageFormat), arg0);

    public static string DirectoryView_FirstTimeInfoBannerMessageFormat(
      object arg0,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (DirectoryView_FirstTimeInfoBannerMessageFormat), culture, arg0);
    }

    public static string DirectoryView_FirstTimeInfoBannerMessageLinkText() => AnalyticsResources.Get(nameof (DirectoryView_FirstTimeInfoBannerMessageLinkText));

    public static string DirectoryView_FirstTimeInfoBannerMessageLinkText(CultureInfo culture) => AnalyticsResources.Get(nameof (DirectoryView_FirstTimeInfoBannerMessageLinkText), culture);

    public static string DirectoryView_HubIsMoving() => AnalyticsResources.Get(nameof (DirectoryView_HubIsMoving));

    public static string DirectoryView_HubIsMoving(CultureInfo culture) => AnalyticsResources.Get(nameof (DirectoryView_HubIsMoving), culture);

    public static string DirectoryView_HubIsMovingInfoBannerMessageFormat(object arg0, object arg1) => AnalyticsResources.Format(nameof (DirectoryView_HubIsMovingInfoBannerMessageFormat), arg0, arg1);

    public static string DirectoryView_HubIsMovingInfoBannerMessageFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return AnalyticsResources.Format(nameof (DirectoryView_HubIsMovingInfoBannerMessageFormat), culture, arg0, arg1);
    }

    public static string DirectoryView_FirstTimeInfoBannerMessageLinkLabel() => AnalyticsResources.Get(nameof (DirectoryView_FirstTimeInfoBannerMessageLinkLabel));

    public static string DirectoryView_FirstTimeInfoBannerMessageLinkLabel(CultureInfo culture) => AnalyticsResources.Get(nameof (DirectoryView_FirstTimeInfoBannerMessageLinkLabel), culture);
  }
}
