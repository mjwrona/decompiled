// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.WITResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 74AD14A4-225D-46D2-B154-945941A2D167
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking
{
  public static class WITResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (WITResources), typeof (WITResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => WITResources.s_resMgr;

    private static string Get(string resourceName) => WITResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? WITResources.Get(resourceName) : WITResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) WITResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? WITResources.GetInt(resourceName) : (int) WITResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) WITResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? WITResources.GetBool(resourceName) : (bool) WITResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => WITResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = WITResources.Get(resourceName, culture);
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

    public static string AddAttachment() => WITResources.Get(nameof (AddAttachment));

    public static string AddAttachment(CultureInfo culture) => WITResources.Get(nameof (AddAttachment), culture);

    public static string AddAttachmentDialogFieldAttachment() => WITResources.Get(nameof (AddAttachmentDialogFieldAttachment));

    public static string AddAttachmentDialogFieldAttachment(CultureInfo culture) => WITResources.Get(nameof (AddAttachmentDialogFieldAttachment), culture);

    public static string DefaultingCurrentIterationTeam() => WITResources.Get(nameof (DefaultingCurrentIterationTeam));

    public static string DefaultingCurrentIterationTeam(CultureInfo culture) => WITResources.Get(nameof (DefaultingCurrentIterationTeam), culture);

    public static string AddAttachmentDialogFieldComment() => WITResources.Get(nameof (AddAttachmentDialogFieldComment));

    public static string AddAttachmentDialogFieldComment(CultureInfo culture) => WITResources.Get(nameof (AddAttachmentDialogFieldComment), culture);

    public static string AddAttachmentDialogTitle() => WITResources.Get(nameof (AddAttachmentDialogTitle));

    public static string AddAttachmentDialogTitle(CultureInfo culture) => WITResources.Get(nameof (AddAttachmentDialogTitle), culture);

    public static string Bug(object arg0) => WITResources.Format(nameof (Bug), arg0);

    public static string Bug(object arg0, CultureInfo culture) => WITResources.Format(nameof (Bug), culture, arg0);

    public static string Cancel() => WITResources.Get(nameof (Cancel));

    public static string Cancel(CultureInfo culture) => WITResources.Get(nameof (Cancel), culture);

    public static string Import() => WITResources.Get(nameof (Import));

    public static string Import(CultureInfo culture) => WITResources.Get(nameof (Import), culture);

    public static string ImportCSVErrorHeader() => WITResources.Get(nameof (ImportCSVErrorHeader));

    public static string ImportCSVErrorHeader(CultureInfo culture) => WITResources.Get(nameof (ImportCSVErrorHeader), culture);

    public static string InvalidCSVFileFormat() => WITResources.Get(nameof (InvalidCSVFileFormat));

    public static string InvalidCSVFileFormat(CultureInfo culture) => WITResources.Get(nameof (InvalidCSVFileFormat), culture);

    public static string ImportCSVPanelText() => WITResources.Get(nameof (ImportCSVPanelText));

    public static string ImportCSVPanelText(CultureInfo culture) => WITResources.Get(nameof (ImportCSVPanelText), culture);

    public static string ImportCSVPanelLoadingText() => WITResources.Get(nameof (ImportCSVPanelLoadingText));

    public static string ImportCSVPanelLoadingText(CultureInfo culture) => WITResources.Get(nameof (ImportCSVPanelLoadingText), culture);

    public static string ImportWorkItems() => WITResources.Get(nameof (ImportWorkItems));

    public static string ImportWorkItems(CultureInfo culture) => WITResources.Get(nameof (ImportWorkItems), culture);

    public static string ColumnOptions() => WITResources.Get(nameof (ColumnOptions));

    public static string ColumnOptions(CultureInfo culture) => WITResources.Get(nameof (ColumnOptions), culture);

    public static string CreateCopyOfWorkItem() => WITResources.Get(nameof (CreateCopyOfWorkItem));

    public static string CreateCopyOfWorkItem(CultureInfo culture) => WITResources.Get(nameof (CreateCopyOfWorkItem), culture);

    public static string DeleteAttachment() => WITResources.Get(nameof (DeleteAttachment));

    public static string DeleteAttachment(CultureInfo culture) => WITResources.Get(nameof (DeleteAttachment), culture);

    public static string EditQuery() => WITResources.Get(nameof (EditQuery));

    public static string EditQuery(CultureInfo culture) => WITResources.Get(nameof (EditQuery), culture);

    public static string EmailWorkItem() => WITResources.Get(nameof (EmailWorkItem));

    public static string EmailWorkItem(CultureInfo culture) => WITResources.Get(nameof (EmailWorkItem), culture);

    public static string InternalServerError() => WITResources.Get(nameof (InternalServerError));

    public static string InternalServerError(CultureInfo culture) => WITResources.Get(nameof (InternalServerError), culture);

    public static string InvalidErrorParameter() => WITResources.Get(nameof (InvalidErrorParameter));

    public static string InvalidErrorParameter(CultureInfo culture) => WITResources.Get(nameof (InvalidErrorParameter), culture);

    public static string LinksControlCommentColumnText() => WITResources.Get(nameof (LinksControlCommentColumnText));

    public static string LinksControlCommentColumnText(CultureInfo culture) => WITResources.Get(nameof (LinksControlCommentColumnText), culture);

    public static string LinksControlDescriptionColumnText() => WITResources.Get(nameof (LinksControlDescriptionColumnText));

    public static string LinksControlDescriptionColumnText(CultureInfo culture) => WITResources.Get(nameof (LinksControlDescriptionColumnText), culture);

    public static string LinksControlHyperlinkText() => WITResources.Get(nameof (LinksControlHyperlinkText));

    public static string LinksControlHyperlinkText(CultureInfo culture) => WITResources.Get(nameof (LinksControlHyperlinkText), culture);

    public static string LinksControlRelatedText() => WITResources.Get(nameof (LinksControlRelatedText));

    public static string LinksControlRelatedText(CultureInfo culture) => WITResources.Get(nameof (LinksControlRelatedText), culture);

    public static string LinksControlUnknownLinkTypeText() => WITResources.Get(nameof (LinksControlUnknownLinkTypeText));

    public static string LinksControlUnknownLinkTypeText(CultureInfo culture) => WITResources.Get(nameof (LinksControlUnknownLinkTypeText), culture);

    public static string LinkSelectedItemsToNewWorkItem() => WITResources.Get(nameof (LinkSelectedItemsToNewWorkItem));

    public static string LinkSelectedItemsToNewWorkItem(CultureInfo culture) => WITResources.Get(nameof (LinkSelectedItemsToNewWorkItem), culture);

    public static string LinkToExistingItem() => WITResources.Get(nameof (LinkToExistingItem));

    public static string LinkToExistingItem(CultureInfo culture) => WITResources.Get(nameof (LinkToExistingItem), culture);

    public static string LinkToExistingItemToolTip() => WITResources.Get(nameof (LinkToExistingItemToolTip));

    public static string LinkToExistingItemToolTip(CultureInfo culture) => WITResources.Get(nameof (LinkToExistingItemToolTip), culture);

    public static string OK() => WITResources.Get(nameof (OK));

    public static string OK(CultureInfo culture) => WITResources.Get(nameof (OK), culture);

    public static string AttachmentsTitle() => WITResources.Get(nameof (AttachmentsTitle));

    public static string AttachmentsTitle(CultureInfo culture) => WITResources.Get(nameof (AttachmentsTitle), culture);

    public static string AttachmentsGridViewIconAriaLabel() => WITResources.Get(nameof (AttachmentsGridViewIconAriaLabel));

    public static string AttachmentsGridViewIconAriaLabel(CultureInfo culture) => WITResources.Get(nameof (AttachmentsGridViewIconAriaLabel), culture);

    public static string AttachmentsThumbnailViewIconAriaLabel() => WITResources.Get(nameof (AttachmentsThumbnailViewIconAriaLabel));

    public static string AttachmentsThumbnailViewIconAriaLabel(CultureInfo culture) => WITResources.Get(nameof (AttachmentsThumbnailViewIconAriaLabel), culture);

    public static string Open() => WITResources.Get(nameof (Open));

    public static string Open(CultureInfo culture) => WITResources.Get(nameof (Open), culture);

    public static string OpenInNewTab() => WITResources.Get(nameof (OpenInNewTab));

    public static string OpenInNewTab(CultureInfo culture) => WITResources.Get(nameof (OpenInNewTab), culture);

    public static string Refresh() => WITResources.Get(nameof (Refresh));

    public static string Refresh(CultureInfo culture) => WITResources.Get(nameof (Refresh), culture);

    public static string RefreshTooltip() => WITResources.Get(nameof (RefreshTooltip));

    public static string RefreshTooltip(CultureInfo culture) => WITResources.Get(nameof (RefreshTooltip), culture);

    public static string Delete() => WITResources.Get(nameof (Delete));

    public static string Delete(CultureInfo culture) => WITResources.Get(nameof (Delete), culture);

    public static string Save() => WITResources.Get(nameof (Save));

    public static string Save(CultureInfo culture) => WITResources.Get(nameof (Save), culture);

    public static string SaveAll() => WITResources.Get(nameof (SaveAll));

    public static string SaveAll(CultureInfo culture) => WITResources.Get(nameof (SaveAll), culture);

    public static string SaveAndClose() => WITResources.Get(nameof (SaveAndClose));

    public static string SaveAndClose(CultureInfo culture) => WITResources.Get(nameof (SaveAndClose), culture);

    public static string SaveAttachment() => WITResources.Get(nameof (SaveAttachment));

    public static string SaveAttachment(CultureInfo culture) => WITResources.Get(nameof (SaveAttachment), culture);

    public static string TriageView() => WITResources.Get(nameof (TriageView));

    public static string TriageView(CultureInfo culture) => WITResources.Get(nameof (TriageView), culture);

    public static string LinkToolTypeBuild() => WITResources.Get(nameof (LinkToolTypeBuild));

    public static string LinkToolTypeBuild(CultureInfo culture) => WITResources.Get(nameof (LinkToolTypeBuild), culture);

    public static string LinkToolTypeCode() => WITResources.Get(nameof (LinkToolTypeCode));

    public static string LinkToolTypeCode(CultureInfo culture) => WITResources.Get(nameof (LinkToolTypeCode), culture);

    public static string LinkToolTypeRequirements() => WITResources.Get(nameof (LinkToolTypeRequirements));

    public static string LinkToolTypeRequirements(CultureInfo culture) => WITResources.Get(nameof (LinkToolTypeRequirements), culture);

    public static string LinkToolTypeTest() => WITResources.Get(nameof (LinkToolTypeTest));

    public static string LinkToolTypeTest(CultureInfo culture) => WITResources.Get(nameof (LinkToolTypeTest), culture);

    public static string LinkToolTypeWiki() => WITResources.Get(nameof (LinkToolTypeWiki));

    public static string LinkToolTypeWiki(CultureInfo culture) => WITResources.Get(nameof (LinkToolTypeWiki), culture);

    public static string LinkToolTypeWork() => WITResources.Get(nameof (LinkToolTypeWork));

    public static string LinkToolTypeWork(CultureInfo culture) => WITResources.Get(nameof (LinkToolTypeWork), culture);

    public static string LinkToolTypeRemoteWork() => WITResources.Get(nameof (LinkToolTypeRemoteWork));

    public static string LinkToolTypeRemoteWork(CultureInfo culture) => WITResources.Get(nameof (LinkToolTypeRemoteWork), culture);

    public static string LinksControlDuplicateStoryboard(object arg0) => WITResources.Format(nameof (LinksControlDuplicateStoryboard), arg0);

    public static string LinksControlDuplicateStoryboard(object arg0, CultureInfo culture) => WITResources.Format(nameof (LinksControlDuplicateStoryboard), culture, arg0);

    public static string LinksControlEnterStoryboardUrl() => WITResources.Get(nameof (LinksControlEnterStoryboardUrl));

    public static string LinksControlEnterStoryboardUrl(CultureInfo culture) => WITResources.Get(nameof (LinksControlEnterStoryboardUrl), culture);

    public static string LinkDialogCommentTitle() => WITResources.Get(nameof (LinkDialogCommentTitle));

    public static string LinkDialogCommentTitle(CultureInfo culture) => WITResources.Get(nameof (LinkDialogCommentTitle), culture);

    public static string LinkDialogHyperlinkAddressTitle() => WITResources.Get(nameof (LinkDialogHyperlinkAddressTitle));

    public static string LinkDialogHyperlinkAddressTitle(CultureInfo culture) => WITResources.Get(nameof (LinkDialogHyperlinkAddressTitle), culture);

    public static string RemoteLinkDialogAddressTitle() => WITResources.Get(nameof (RemoteLinkDialogAddressTitle));

    public static string RemoteLinkDialogAddressTitle(CultureInfo culture) => WITResources.Get(nameof (RemoteLinkDialogAddressTitle), culture);

    public static string RemoteLinkDialogAddressTitleWatermark() => WITResources.Get(nameof (RemoteLinkDialogAddressTitleWatermark));

    public static string RemoteLinkDialogAddressTitleWatermark(CultureInfo culture) => WITResources.Get(nameof (RemoteLinkDialogAddressTitleWatermark), culture);

    public static string LinksControlDuplicateRemoteLink() => WITResources.Get(nameof (LinksControlDuplicateRemoteLink));

    public static string LinksControlDuplicateRemoteLink(CultureInfo culture) => WITResources.Get(nameof (LinksControlDuplicateRemoteLink), culture);

    public static string LinksControlSameHostRemoteLink() => WITResources.Get(nameof (LinksControlSameHostRemoteLink));

    public static string LinksControlSameHostRemoteLink(CultureInfo culture) => WITResources.Get(nameof (LinksControlSameHostRemoteLink), culture);

    public static string LinksControlCircularRemoteLink() => WITResources.Get(nameof (LinksControlCircularRemoteLink));

    public static string LinksControlCircularRemoteLink(CultureInfo culture) => WITResources.Get(nameof (LinksControlCircularRemoteLink), culture);

    public static string RemoteLinkStatusPendingAdd() => WITResources.Get(nameof (RemoteLinkStatusPendingAdd));

    public static string RemoteLinkStatusPendingAdd(CultureInfo culture) => WITResources.Get(nameof (RemoteLinkStatusPendingAdd), culture);

    public static string RemoteLinkStatusSuccess() => WITResources.Get(nameof (RemoteLinkStatusSuccess));

    public static string RemoteLinkStatusSuccess(CultureInfo culture) => WITResources.Get(nameof (RemoteLinkStatusSuccess), culture);

    public static string RemoteLinkStatusFailed() => WITResources.Get(nameof (RemoteLinkStatusFailed));

    public static string RemoteLinkStatusFailed(CultureInfo culture) => WITResources.Get(nameof (RemoteLinkStatusFailed), culture);

    public static string RemoteLinkStatusPendingUpdate() => WITResources.Get(nameof (RemoteLinkStatusPendingUpdate));

    public static string RemoteLinkStatusPendingUpdate(CultureInfo culture) => WITResources.Get(nameof (RemoteLinkStatusPendingUpdate), culture);

    public static string RemoteLinkStatusPendingDelete() => WITResources.Get(nameof (RemoteLinkStatusPendingDelete));

    public static string RemoteLinkStatusPendingDelete(CultureInfo culture) => WITResources.Get(nameof (RemoteLinkStatusPendingDelete), culture);

    public static string LinkDialogStoryboardLinkAddressTitle() => WITResources.Get(nameof (LinkDialogStoryboardLinkAddressTitle));

    public static string LinkDialogStoryboardLinkAddressTitle(CultureInfo culture) => WITResources.Get(nameof (LinkDialogStoryboardLinkAddressTitle), culture);

    public static string LinkDialogWorkItemIdsTitle() => WITResources.Get(nameof (LinkDialogWorkItemIdsTitle));

    public static string LinkDialogWorkItemIdsTitle(CultureInfo culture) => WITResources.Get(nameof (LinkDialogWorkItemIdsTitle), culture);

    public static string LinksControlResultAttachmentText() => WITResources.Get(nameof (LinksControlResultAttachmentText));

    public static string LinksControlResultAttachmentText(CultureInfo culture) => WITResources.Get(nameof (LinksControlResultAttachmentText), culture);

    public static string LinksControlTestResultText() => WITResources.Get(nameof (LinksControlTestResultText));

    public static string LinksControlTestResultText(CultureInfo culture) => WITResources.Get(nameof (LinksControlTestResultText), culture);

    public static string LinksControlTestText() => WITResources.Get(nameof (LinksControlTestText));

    public static string LinksControlTestText(CultureInfo culture) => WITResources.Get(nameof (LinksControlTestText), culture);

    public static string LinksControlDuplicateHyperlink() => WITResources.Get(nameof (LinksControlDuplicateHyperlink));

    public static string LinksControlDuplicateHyperlink(CultureInfo culture) => WITResources.Get(nameof (LinksControlDuplicateHyperlink), culture);

    public static string LinkDialogDescription() => WITResources.Get(nameof (LinkDialogDescription));

    public static string LinkDialogDescription(CultureInfo culture) => WITResources.Get(nameof (LinkDialogDescription), culture);

    public static string LinkToExistingDialogFormNotFound(object arg0) => WITResources.Format(nameof (LinkToExistingDialogFormNotFound), arg0);

    public static string LinkToExistingDialogFormNotFound(object arg0, CultureInfo culture) => WITResources.Format(nameof (LinkToExistingDialogFormNotFound), culture, arg0);

    public static string LinkToExistingDialogLinkTypeTitle() => WITResources.Get(nameof (LinkToExistingDialogLinkTypeTitle));

    public static string LinkToExistingDialogLinkTypeTitle(CultureInfo culture) => WITResources.Get(nameof (LinkToExistingDialogLinkTypeTitle), culture);

    public static string LinkFormWorkItemNotFound() => WITResources.Get(nameof (LinkFormWorkItemNotFound));

    public static string LinkFormWorkItemNotFound(CultureInfo culture) => WITResources.Get(nameof (LinkFormWorkItemNotFound), culture);

    public static string LinksControlEnterUrl() => WITResources.Get(nameof (LinksControlEnterUrl));

    public static string LinksControlEnterUrl(CultureInfo culture) => WITResources.Get(nameof (LinksControlEnterUrl), culture);

    public static string LinkValidationFailed() => WITResources.Get(nameof (LinkValidationFailed));

    public static string LinkValidationFailed(CultureInfo culture) => WITResources.Get(nameof (LinkValidationFailed), culture);

    public static string LinkDialogErrorWorkItemTypesToAdd() => WITResources.Get(nameof (LinkDialogErrorWorkItemTypesToAdd));

    public static string LinkDialogErrorWorkItemTypesToAdd(CultureInfo culture) => WITResources.Get(nameof (LinkDialogErrorWorkItemTypesToAdd), culture);

    public static string LinkWorkItemsNoFavorites() => WITResources.Get(nameof (LinkWorkItemsNoFavorites));

    public static string LinkWorkItemsNoFavorites(CultureInfo culture) => WITResources.Get(nameof (LinkWorkItemsNoFavorites), culture);

    public static string StoryboardLinkInvalid(object arg0) => WITResources.Format(nameof (StoryboardLinkInvalid), arg0);

    public static string StoryboardLinkInvalid(object arg0, CultureInfo culture) => WITResources.Format(nameof (StoryboardLinkInvalid), culture, arg0);

    public static string NewLinkedWorkItemTitle() => WITResources.Get(nameof (NewLinkedWorkItemTitle));

    public static string NewLinkedWorkItemTitle(CultureInfo culture) => WITResources.Get(nameof (NewLinkedWorkItemTitle), culture);

    public static string NewLinkedWorkItemTypeTitle() => WITResources.Get(nameof (NewLinkedWorkItemTypeTitle));

    public static string NewLinkedWorkItemTypeTitle(CultureInfo culture) => WITResources.Get(nameof (NewLinkedWorkItemTypeTitle), culture);

    public static string LinksControlLinkTypeColumnText() => WITResources.Get(nameof (LinksControlLinkTypeColumnText));

    public static string LinksControlLinkTypeColumnText(CultureInfo culture) => WITResources.Get(nameof (LinksControlLinkTypeColumnText), culture);

    public static string DeleteQuery() => WITResources.Get(nameof (DeleteQuery));

    public static string DeleteQuery(CultureInfo culture) => WITResources.Get(nameof (DeleteQuery), culture);

    public static string NewQuery() => WITResources.Get(nameof (NewQuery));

    public static string NewQuery(CultureInfo culture) => WITResources.Get(nameof (NewQuery), culture);

    public static string NewQueryFolder() => WITResources.Get(nameof (NewQueryFolder));

    public static string NewQueryFolder(CultureInfo culture) => WITResources.Get(nameof (NewQueryFolder), culture);

    public static string RenameQuery() => WITResources.Get(nameof (RenameQuery));

    public static string RenameQuery(CultureInfo culture) => WITResources.Get(nameof (RenameQuery), culture);

    public static string AssignedToMeQuery() => WITResources.Get(nameof (AssignedToMeQuery));

    public static string AssignedToMeQuery(CultureInfo culture) => WITResources.Get(nameof (AssignedToMeQuery), culture);

    public static string QueryResultsGridLinkQueryStatusTextFormat(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return WITResources.Format(nameof (QueryResultsGridLinkQueryStatusTextFormat), arg0, arg1, arg2, arg3);
    }

    public static string QueryResultsGridLinkQueryStatusTextFormat(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (QueryResultsGridLinkQueryStatusTextFormat), culture, arg0, arg1, arg2, arg3);
    }

    public static string QueryResultsGridStatusTextFormat(object arg0, object arg1) => WITResources.Format(nameof (QueryResultsGridStatusTextFormat), arg0, arg1);

    public static string QueryResultsGridStatusTextFormat(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (QueryResultsGridStatusTextFormat), culture, arg0, arg1);
    }

    public static string QueryFilterInvalidMacro(object arg0) => WITResources.Format(nameof (QueryFilterInvalidMacro), arg0);

    public static string QueryFilterInvalidMacro(object arg0, CultureInfo culture) => WITResources.Format(nameof (QueryFilterInvalidMacro), culture, arg0);

    public static string AttachmentsGridCommentsColumn() => WITResources.Get(nameof (AttachmentsGridCommentsColumn));

    public static string AttachmentsGridCommentsColumn(CultureInfo culture) => WITResources.Get(nameof (AttachmentsGridCommentsColumn), culture);

    public static string AttachmentsGridDateAttachedColumn() => WITResources.Get(nameof (AttachmentsGridDateAttachedColumn));

    public static string AttachmentsGridDateAttachedColumn(CultureInfo culture) => WITResources.Get(nameof (AttachmentsGridDateAttachedColumn), culture);

    public static string AttachmentsGridNameColumn() => WITResources.Get(nameof (AttachmentsGridNameColumn));

    public static string AttachmentsGridNameColumn(CultureInfo culture) => WITResources.Get(nameof (AttachmentsGridNameColumn), culture);

    public static string AttachmentsGridSizeColumn() => WITResources.Get(nameof (AttachmentsGridSizeColumn));

    public static string AttachmentsGridSizeColumn(CultureInfo culture) => WITResources.Get(nameof (AttachmentsGridSizeColumn), culture);

    public static string ColumnOptionsAllProjects() => WITResources.Get(nameof (ColumnOptionsAllProjects));

    public static string ColumnOptionsAllProjects(CultureInfo culture) => WITResources.Get(nameof (ColumnOptionsAllProjects), culture);

    public static string ColumnOptionsAllWorkItemTypes() => WITResources.Get(nameof (ColumnOptionsAllWorkItemTypes));

    public static string ColumnOptionsAllWorkItemTypes(CultureInfo culture) => WITResources.Get(nameof (ColumnOptionsAllWorkItemTypes), culture);

    public static string ColumnOptionsTitle() => WITResources.Get(nameof (ColumnOptionsTitle));

    public static string ColumnOptionsTitle(CultureInfo culture) => WITResources.Get(nameof (ColumnOptionsTitle), culture);

    public static string ErrorSendingEmail() => WITResources.Get(nameof (ErrorSendingEmail));

    public static string ErrorSendingEmail(CultureInfo culture) => WITResources.Get(nameof (ErrorSendingEmail), culture);

    public static string UnableToFindQueryDefinition(object arg0) => WITResources.Format(nameof (UnableToFindQueryDefinition), arg0);

    public static string UnableToFindQueryDefinition(object arg0, CultureInfo culture) => WITResources.Format(nameof (UnableToFindQueryDefinition), culture, arg0);

    public static string UnableToCreateBugOfType(object arg0) => WITResources.Format(nameof (UnableToCreateBugOfType), arg0);

    public static string UnableToCreateBugOfType(object arg0, CultureInfo culture) => WITResources.Format(nameof (UnableToCreateBugOfType), culture, arg0);

    public static string ColumnOptionsInvalidWidth() => WITResources.Get(nameof (ColumnOptionsInvalidWidth));

    public static string ColumnOptionsInvalidWidth(CultureInfo culture) => WITResources.Get(nameof (ColumnOptionsInvalidWidth), culture);

    public static string EnterDetails() => WITResources.Get(nameof (EnterDetails));

    public static string EnterDetails(CultureInfo culture) => WITResources.Get(nameof (EnterDetails), culture);

    public static string ConfirmWorkItemRefresh() => WITResources.Get(nameof (ConfirmWorkItemRefresh));

    public static string ConfirmWorkItemRefresh(CultureInfo culture) => WITResources.Get(nameof (ConfirmWorkItemRefresh), culture);

    public static string ConfirmWorkItemRevert() => WITResources.Get(nameof (ConfirmWorkItemRevert));

    public static string ConfirmWorkItemRevert(CultureInfo culture) => WITResources.Get(nameof (ConfirmWorkItemRevert), culture);

    public static string WorkItemRevert() => WITResources.Get(nameof (WorkItemRevert));

    public static string WorkItemRevert(CultureInfo culture) => WITResources.Get(nameof (WorkItemRevert), culture);

    public static string AddAttachmentFileNotFoundError(object arg0) => WITResources.Format(nameof (AddAttachmentFileNotFoundError), arg0);

    public static string AddAttachmentFileNotFoundError(object arg0, CultureInfo culture) => WITResources.Format(nameof (AddAttachmentFileNotFoundError), culture, arg0);

    public static string AddAttachmentUnknownError() => WITResources.Get(nameof (AddAttachmentUnknownError));

    public static string AddAttachmentUnknownError(CultureInfo culture) => WITResources.Get(nameof (AddAttachmentUnknownError), culture);

    public static string CreateCopyOfWorkItemProject() => WITResources.Get(nameof (CreateCopyOfWorkItemProject));

    public static string CreateCopyOfWorkItemProject(CultureInfo culture) => WITResources.Get(nameof (CreateCopyOfWorkItemProject), culture);

    public static string CreateCopyOfWorkItemSourceFormat(object arg0, object arg1, object arg2) => WITResources.Format(nameof (CreateCopyOfWorkItemSourceFormat), arg0, arg1, arg2);

    public static string CreateCopyOfWorkItemSourceFormat(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (CreateCopyOfWorkItemSourceFormat), culture, arg0, arg1, arg2);
    }

    public static string CreateCopyOfWorkItemTitle() => WITResources.Get(nameof (CreateCopyOfWorkItemTitle));

    public static string CreateCopyOfWorkItemTitle(CultureInfo culture) => WITResources.Get(nameof (CreateCopyOfWorkItemTitle), culture);

    public static string CreateCopyOfWorkItemType() => WITResources.Get(nameof (CreateCopyOfWorkItemType));

    public static string CreateCopyOfWorkItemType(CultureInfo culture) => WITResources.Get(nameof (CreateCopyOfWorkItemType), culture);

    public static string CreateCopyCopiedFromNewWorkItem() => WITResources.Get(nameof (CreateCopyCopiedFromNewWorkItem));

    public static string CreateCopyCopiedFromNewWorkItem(CultureInfo culture) => WITResources.Get(nameof (CreateCopyCopiedFromNewWorkItem), culture);

    public static string CreateCopyCopiedFrom(object arg0) => WITResources.Format(nameof (CreateCopyCopiedFrom), arg0);

    public static string CreateCopyCopiedFrom(object arg0, CultureInfo culture) => WITResources.Format(nameof (CreateCopyCopiedFrom), culture, arg0);

    public static string UseWorkItemAsATemplate() => WITResources.Get(nameof (UseWorkItemAsATemplate));

    public static string UseWorkItemAsATemplate(CultureInfo culture) => WITResources.Get(nameof (UseWorkItemAsATemplate), culture);

    public static string TriageViewSaveErrorTitle() => WITResources.Get(nameof (TriageViewSaveErrorTitle));

    public static string TriageViewSaveErrorTitle(CultureInfo culture) => WITResources.Get(nameof (TriageViewSaveErrorTitle), culture);

    public static string SucessfullySavedNWisButFailedToSave(object arg0, object arg1) => WITResources.Format(nameof (SucessfullySavedNWisButFailedToSave), arg0, arg1);

    public static string SucessfullySavedNWisButFailedToSave(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (SucessfullySavedNWisButFailedToSave), culture, arg0, arg1);
    }

    public static string FollowingWisCouldNotBeSaved() => WITResources.Get(nameof (FollowingWisCouldNotBeSaved));

    public static string FollowingWisCouldNotBeSaved(CultureInfo culture) => WITResources.Get(nameof (FollowingWisCouldNotBeSaved), culture);

    public static string CorrectWisAndTryAgain() => WITResources.Get(nameof (CorrectWisAndTryAgain));

    public static string CorrectWisAndTryAgain(CultureInfo culture) => WITResources.Get(nameof (CorrectWisAndTryAgain), culture);

    public static string FailedToSaveNWorkItems(object arg0) => WITResources.Format(nameof (FailedToSaveNWorkItems), arg0);

    public static string FailedToSaveNWorkItems(object arg0, CultureInfo culture) => WITResources.Format(nameof (FailedToSaveNWorkItems), culture, arg0);

    public static string TriageViewWorkItemSaveError(object arg0) => WITResources.Format(nameof (TriageViewWorkItemSaveError), arg0);

    public static string TriageViewWorkItemSaveError(object arg0, CultureInfo culture) => WITResources.Format(nameof (TriageViewWorkItemSaveError), culture, arg0);

    public static string SaveResults() => WITResources.Get(nameof (SaveResults));

    public static string SaveResults(CultureInfo culture) => WITResources.Get(nameof (SaveResults), culture);

    public static string SaveResultsToolTip() => WITResources.Get(nameof (SaveResultsToolTip));

    public static string SaveResultsToolTip(CultureInfo culture) => WITResources.Get(nameof (SaveResultsToolTip), culture);

    public static string UnsavedWorkItemPrompt() => WITResources.Get(nameof (UnsavedWorkItemPrompt));

    public static string UnsavedWorkItemPrompt(CultureInfo culture) => WITResources.Get(nameof (UnsavedWorkItemPrompt), culture);

    public static string DialogCloseButtonText() => WITResources.Get(nameof (DialogCloseButtonText));

    public static string DialogCloseButtonText(CultureInfo culture) => WITResources.Get(nameof (DialogCloseButtonText), culture);

    public static string UnsavedWorkItemsQuery() => WITResources.Get(nameof (UnsavedWorkItemsQuery));

    public static string UnsavedWorkItemsQuery(CultureInfo culture) => WITResources.Get(nameof (UnsavedWorkItemsQuery), culture);

    public static string FollowedWorkItemsQuery() => WITResources.Get(nameof (FollowedWorkItemsQuery));

    public static string FollowedWorkItemsQuery(CultureInfo culture) => WITResources.Get(nameof (FollowedWorkItemsQuery), culture);

    public static string WorkItemFinderProject() => WITResources.Get(nameof (WorkItemFinderProject));

    public static string WorkItemFinderProject(CultureInfo culture) => WITResources.Get(nameof (WorkItemFinderProject), culture);

    public static string WorkItemFinderQuery() => WITResources.Get(nameof (WorkItemFinderQuery));

    public static string WorkItemFinderQuery(CultureInfo culture) => WITResources.Get(nameof (WorkItemFinderQuery), culture);

    public static string WorkItemFinderFavorites() => WITResources.Get(nameof (WorkItemFinderFavorites));

    public static string WorkItemFinderFavorites(CultureInfo culture) => WITResources.Get(nameof (WorkItemFinderFavorites), culture);

    public static string QueryEditor() => WITResources.Get(nameof (QueryEditor));

    public static string QueryEditor(CultureInfo culture) => WITResources.Get(nameof (QueryEditor), culture);

    public static string QueryEditorLinkTypeLabel() => WITResources.Get(nameof (QueryEditorLinkTypeLabel));

    public static string QueryEditorLinkTypeLabel(CultureInfo culture) => WITResources.Get(nameof (QueryEditorLinkTypeLabel), culture);

    public static string QueryEditorNewQueryNameFormat(object arg0) => WITResources.Format(nameof (QueryEditorNewQueryNameFormat), arg0);

    public static string QueryEditorNewQueryNameFormat(object arg0, CultureInfo culture) => WITResources.Format(nameof (QueryEditorNewQueryNameFormat), culture, arg0);

    public static string QueryEditorQueryModeDoesNotContain() => WITResources.Get(nameof (QueryEditorQueryModeDoesNotContain));

    public static string QueryEditorQueryModeDoesNotContain(CultureInfo culture) => WITResources.Get(nameof (QueryEditorQueryModeDoesNotContain), culture);

    public static string QueryEditorQueryModeLabel() => WITResources.Get(nameof (QueryEditorQueryModeLabel));

    public static string QueryEditorQueryModeLabel(CultureInfo culture) => WITResources.Get(nameof (QueryEditorQueryModeLabel), culture);

    public static string QueryEditorQueryModeMayHave() => WITResources.Get(nameof (QueryEditorQueryModeMayHave));

    public static string QueryEditorQueryModeMayHave(CultureInfo culture) => WITResources.Get(nameof (QueryEditorQueryModeMayHave), culture);

    public static string QueryEditorQueryModeMustHave() => WITResources.Get(nameof (QueryEditorQueryModeMustHave));

    public static string QueryEditorQueryModeMustHave(CultureInfo culture) => WITResources.Get(nameof (QueryEditorQueryModeMustHave), culture);

    public static string QueryEditorQueryTypeLabel() => WITResources.Get(nameof (QueryEditorQueryTypeLabel));

    public static string QueryEditorQueryTypeLabel(CultureInfo culture) => WITResources.Get(nameof (QueryEditorQueryTypeLabel), culture);

    public static string QueryEditorQueryTypeLink() => WITResources.Get(nameof (QueryEditorQueryTypeLink));

    public static string QueryEditorQueryTypeLink(CultureInfo culture) => WITResources.Get(nameof (QueryEditorQueryTypeLink), culture);

    public static string QueryEditorQueryTypeSimple() => WITResources.Get(nameof (QueryEditorQueryTypeSimple));

    public static string QueryEditorQueryTypeSimple(CultureInfo culture) => WITResources.Get(nameof (QueryEditorQueryTypeSimple), culture);

    public static string QueryEditorQueryTypeTree() => WITResources.Get(nameof (QueryEditorQueryTypeTree));

    public static string QueryEditorQueryTypeTree(CultureInfo culture) => WITResources.Get(nameof (QueryEditorQueryTypeTree), culture);

    public static string InvalidQuery() => WITResources.Get(nameof (InvalidQuery));

    public static string InvalidQuery(CultureInfo culture) => WITResources.Get(nameof (InvalidQuery), culture);

    public static string FlatQuery() => WITResources.Get(nameof (FlatQuery));

    public static string FlatQuery(CultureInfo culture) => WITResources.Get(nameof (FlatQuery), culture);

    public static string DirectLinksQuery() => WITResources.Get(nameof (DirectLinksQuery));

    public static string DirectLinksQuery(CultureInfo culture) => WITResources.Get(nameof (DirectLinksQuery), culture);

    public static string TreeLinksQuery() => WITResources.Get(nameof (TreeLinksQuery));

    public static string TreeLinksQuery(CultureInfo culture) => WITResources.Get(nameof (TreeLinksQuery), culture);

    public static string QueryEditorSourceFilterGroupingText() => WITResources.Get(nameof (QueryEditorSourceFilterGroupingText));

    public static string QueryEditorSourceFilterGroupingText(CultureInfo culture) => WITResources.Get(nameof (QueryEditorSourceFilterGroupingText), culture);

    public static string QueryEditorTargetFilterGroupingText() => WITResources.Get(nameof (QueryEditorTargetFilterGroupingText));

    public static string QueryEditorTargetFilterGroupingText(CultureInfo culture) => WITResources.Get(nameof (QueryEditorTargetFilterGroupingText), culture);

    public static string QueryEditorTreeTypeLabel() => WITResources.Get(nameof (QueryEditorTreeTypeLabel));

    public static string QueryEditorTreeTypeLabel(CultureInfo culture) => WITResources.Get(nameof (QueryEditorTreeTypeLabel), culture);

    public static string QueryFolder() => WITResources.Get(nameof (QueryFolder));

    public static string QueryFolder(CultureInfo culture) => WITResources.Get(nameof (QueryFolder), culture);

    public static string RevertQueryChanges() => WITResources.Get(nameof (RevertQueryChanges));

    public static string RevertQueryChanges(CultureInfo culture) => WITResources.Get(nameof (RevertQueryChanges), culture);

    public static string RunQuery() => WITResources.Get(nameof (RunQuery));

    public static string RunQuery(CultureInfo culture) => WITResources.Get(nameof (RunQuery), culture);

    public static string SaveQuery() => WITResources.Get(nameof (SaveQuery));

    public static string SaveQuery(CultureInfo culture) => WITResources.Get(nameof (SaveQuery), culture);

    public static string SearchResults() => WITResources.Get(nameof (SearchResults));

    public static string SearchResults(CultureInfo culture) => WITResources.Get(nameof (SearchResults), culture);

    public static string WorkItemsCreatedByMeQuery() => WITResources.Get(nameof (WorkItemsCreatedByMeQuery));

    public static string WorkItemsCreatedByMeQuery(CultureInfo culture) => WITResources.Get(nameof (WorkItemsCreatedByMeQuery), culture);

    public static string FieldStatusInvalidCharactersError(object arg0) => WITResources.Format(nameof (FieldStatusInvalidCharactersError), arg0);

    public static string FieldStatusInvalidCharactersError(object arg0, CultureInfo culture) => WITResources.Format(nameof (FieldStatusInvalidCharactersError), culture, arg0);

    public static string FieldStatusInvalidComputedFieldError(object arg0) => WITResources.Format(nameof (FieldStatusInvalidComputedFieldError), arg0);

    public static string FieldStatusInvalidComputedFieldError(object arg0, CultureInfo culture) => WITResources.Format(nameof (FieldStatusInvalidComputedFieldError), culture, arg0);

    public static string FieldStatusInvalidDateErrorError(object arg0) => WITResources.Format(nameof (FieldStatusInvalidDateErrorError), arg0);

    public static string FieldStatusInvalidDateErrorError(object arg0, CultureInfo culture) => WITResources.Format(nameof (FieldStatusInvalidDateErrorError), culture, arg0);

    public static string FieldStatusInvalidEmptyError(object arg0) => WITResources.Format(nameof (FieldStatusInvalidEmptyError), arg0);

    public static string FieldStatusInvalidEmptyError(object arg0, CultureInfo culture) => WITResources.Format(nameof (FieldStatusInvalidEmptyError), culture, arg0);

    public static string FieldStatusInvalidEmptyOrOldValueError(object arg0) => WITResources.Format(nameof (FieldStatusInvalidEmptyOrOldValueError), arg0);

    public static string FieldStatusInvalidEmptyOrOldValueError(object arg0, CultureInfo culture) => WITResources.Format(nameof (FieldStatusInvalidEmptyOrOldValueError), culture, arg0);

    public static string FieldStatusInvalidFormatError(object arg0) => WITResources.Format(nameof (FieldStatusInvalidFormatError), arg0);

    public static string FieldStatusInvalidFormatError(object arg0, CultureInfo culture) => WITResources.Format(nameof (FieldStatusInvalidFormatError), culture, arg0);

    public static string FieldStatusInvalidListValueError(object arg0, object arg1) => WITResources.Format(nameof (FieldStatusInvalidListValueError), arg0, arg1);

    public static string FieldStatusInvalidListValueError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (FieldStatusInvalidListValueError), culture, arg0, arg1);
    }

    public static string FieldStatusInvalidNotEmptyError(object arg0) => WITResources.Format(nameof (FieldStatusInvalidNotEmptyError), arg0);

    public static string FieldStatusInvalidNotEmptyError(object arg0, CultureInfo culture) => WITResources.Format(nameof (FieldStatusInvalidNotEmptyError), culture, arg0);

    public static string FieldStatusInvalidNotEmptyOrOldValueError(object arg0) => WITResources.Format(nameof (FieldStatusInvalidNotEmptyOrOldValueError), arg0);

    public static string FieldStatusInvalidNotEmptyOrOldValueError(object arg0, CultureInfo culture) => WITResources.Format(nameof (FieldStatusInvalidNotEmptyOrOldValueError), culture, arg0);

    public static string FieldStatusInvalidNotOldValueError(object arg0) => WITResources.Format(nameof (FieldStatusInvalidNotOldValueError), arg0);

    public static string FieldStatusInvalidNotOldValueError(object arg0, CultureInfo culture) => WITResources.Format(nameof (FieldStatusInvalidNotOldValueError), culture, arg0);

    public static string FieldStatusInvalidOldValueError(object arg0) => WITResources.Format(nameof (FieldStatusInvalidOldValueError), arg0);

    public static string FieldStatusInvalidOldValueError(object arg0, CultureInfo culture) => WITResources.Format(nameof (FieldStatusInvalidOldValueError), culture, arg0);

    public static string FieldStatusInvalidPathError(object arg0) => WITResources.Format(nameof (FieldStatusInvalidPathError), arg0);

    public static string FieldStatusInvalidPathError(object arg0, CultureInfo culture) => WITResources.Format(nameof (FieldStatusInvalidPathError), culture, arg0);

    public static string FieldStatusInvalidTooLongError(object arg0) => WITResources.Format(nameof (FieldStatusInvalidTooLongError), arg0);

    public static string FieldStatusInvalidTooLongError(object arg0, CultureInfo culture) => WITResources.Format(nameof (FieldStatusInvalidTooLongError), culture, arg0);

    public static string FieldStatusInvalidTypeError(object arg0) => WITResources.Format(nameof (FieldStatusInvalidTypeError), arg0);

    public static string FieldStatusInvalidTypeError(object arg0, CultureInfo culture) => WITResources.Format(nameof (FieldStatusInvalidTypeError), culture, arg0);

    public static string FieldStatusInvalidUnknownError(object arg0) => WITResources.Format(nameof (FieldStatusInvalidUnknownError), arg0);

    public static string FieldStatusInvalidUnknownError(object arg0, CultureInfo culture) => WITResources.Format(nameof (FieldStatusInvalidUnknownError), culture, arg0);

    public static string FieldStatusInvalidValueInOtherFieldError(object arg0) => WITResources.Format(nameof (FieldStatusInvalidValueInOtherFieldError), arg0);

    public static string FieldStatusInvalidValueInOtherFieldError(object arg0, CultureInfo culture) => WITResources.Format(nameof (FieldStatusInvalidValueInOtherFieldError), culture, arg0);

    public static string FieldStatusInvalidValueNotInOtherFieldError(object arg0) => WITResources.Format(nameof (FieldStatusInvalidValueNotInOtherFieldError), arg0);

    public static string FieldStatusInvalidValueNotInOtherFieldError(
      object arg0,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (FieldStatusInvalidValueNotInOtherFieldError), culture, arg0);
    }

    public static string InsertImageName() => WITResources.Get(nameof (InsertImageName));

    public static string InsertImageName(CultureInfo culture) => WITResources.Get(nameof (InsertImageName), culture);

    public static string InsertImageEditLabel() => WITResources.Get(nameof (InsertImageEditLabel));

    public static string InsertImageEditLabel(CultureInfo culture) => WITResources.Get(nameof (InsertImageEditLabel), culture);

    public static string InsertImageUploadingDialogTitle() => WITResources.Get(nameof (InsertImageUploadingDialogTitle));

    public static string InsertImageUploadingDialogTitle(CultureInfo culture) => WITResources.Get(nameof (InsertImageUploadingDialogTitle), culture);

    public static string InsertImageInvalidFileType(object arg0) => WITResources.Format(nameof (InsertImageInvalidFileType), arg0);

    public static string InsertImageInvalidFileType(object arg0, CultureInfo culture) => WITResources.Format(nameof (InsertImageInvalidFileType), culture, arg0);

    public static string ConfirmDeleteQuery(object arg0) => WITResources.Format(nameof (ConfirmDeleteQuery), arg0);

    public static string ConfirmDeleteQuery(object arg0, CultureInfo culture) => WITResources.Format(nameof (ConfirmDeleteQuery), culture, arg0);

    public static string FiltersForLinkedWorkItems() => WITResources.Get(nameof (FiltersForLinkedWorkItems));

    public static string FiltersForLinkedWorkItems(CultureInfo culture) => WITResources.Get(nameof (FiltersForLinkedWorkItems), culture);

    public static string NoData() => WITResources.Get(nameof (NoData));

    public static string NoData(CultureInfo culture) => WITResources.Get(nameof (NoData), culture);

    public static string RenameQueryFolderTitle() => WITResources.Get(nameof (RenameQueryFolderTitle));

    public static string RenameQueryFolderTitle(CultureInfo culture) => WITResources.Get(nameof (RenameQueryFolderTitle), culture);

    public static string RenameQueryTitle() => WITResources.Get(nameof (RenameQueryTitle));

    public static string RenameQueryTitle(CultureInfo culture) => WITResources.Get(nameof (RenameQueryTitle), culture);

    public static string ReturnLinksOfAnyType() => WITResources.Get(nameof (ReturnLinksOfAnyType));

    public static string ReturnLinksOfAnyType(CultureInfo culture) => WITResources.Get(nameof (ReturnLinksOfAnyType), culture);

    public static string ReturnSelectedLinkTypes() => WITResources.Get(nameof (ReturnSelectedLinkTypes));

    public static string ReturnSelectedLinkTypes(CultureInfo culture) => WITResources.Get(nameof (ReturnSelectedLinkTypes), culture);

    public static string NameLabel() => WITResources.Get(nameof (NameLabel));

    public static string NameLabel(CultureInfo culture) => WITResources.Get(nameof (NameLabel), culture);

    public static string LinksControlStoryboardText() => WITResources.Get(nameof (LinksControlStoryboardText));

    public static string LinksControlStoryboardText(CultureInfo culture) => WITResources.Get(nameof (LinksControlStoryboardText), culture);

    public static string WorkItemLogControlAddedComment() => WITResources.Get(nameof (WorkItemLogControlAddedComment));

    public static string WorkItemLogControlAddedComment(CultureInfo culture) => WITResources.Get(nameof (WorkItemLogControlAddedComment), culture);

    public static string WorkItemLogControlEditedComment() => WITResources.Get(nameof (WorkItemLogControlEditedComment));

    public static string WorkItemLogControlEditedComment(CultureInfo culture) => WITResources.Get(nameof (WorkItemLogControlEditedComment), culture);

    public static string WorkItemLogControlDeletedComment() => WITResources.Get(nameof (WorkItemLogControlDeletedComment));

    public static string WorkItemLogControlDeletedComment(CultureInfo culture) => WITResources.Get(nameof (WorkItemLogControlDeletedComment), culture);

    public static string WorkItemLogControlAttachmentAdded() => WITResources.Get(nameof (WorkItemLogControlAttachmentAdded));

    public static string WorkItemLogControlAttachmentAdded(CultureInfo culture) => WITResources.Get(nameof (WorkItemLogControlAttachmentAdded), culture);

    public static string WorkItemLogControlAttachmentDeleted() => WITResources.Get(nameof (WorkItemLogControlAttachmentDeleted));

    public static string WorkItemLogControlAttachmentDeleted(CultureInfo culture) => WITResources.Get(nameof (WorkItemLogControlAttachmentDeleted), culture);

    public static string WorkItemLogControlAttachmentsHeader() => WITResources.Get(nameof (WorkItemLogControlAttachmentsHeader));

    public static string WorkItemLogControlAttachmentsHeader(CultureInfo culture) => WITResources.Get(nameof (WorkItemLogControlAttachmentsHeader), culture);

    public static string WorkItemLogControlCreatedWorkItem(object arg0) => WITResources.Format(nameof (WorkItemLogControlCreatedWorkItem), arg0);

    public static string WorkItemLogControlCreatedWorkItem(object arg0, CultureInfo culture) => WITResources.Format(nameof (WorkItemLogControlCreatedWorkItem), culture, arg0);

    public static string WorkItemLogControlLinkAdded(object arg0) => WITResources.Format(nameof (WorkItemLogControlLinkAdded), arg0);

    public static string WorkItemLogControlLinkAdded(object arg0, CultureInfo culture) => WITResources.Format(nameof (WorkItemLogControlLinkAdded), culture, arg0);

    public static string WorkItemLogControlLinkDeleted(object arg0) => WITResources.Format(nameof (WorkItemLogControlLinkDeleted), arg0);

    public static string WorkItemLogControlLinkDeleted(object arg0, CultureInfo culture) => WITResources.Format(nameof (WorkItemLogControlLinkDeleted), culture, arg0);

    public static string WorkItemLogControlLinksHeader() => WITResources.Get(nameof (WorkItemLogControlLinksHeader));

    public static string WorkItemLogControlLinksHeader(CultureInfo culture) => WITResources.Get(nameof (WorkItemLogControlLinksHeader), culture);

    public static string WorkItemLogControlMadeChanges() => WITResources.Get(nameof (WorkItemLogControlMadeChanges));

    public static string WorkItemLogControlMadeChanges(CultureInfo culture) => WITResources.Get(nameof (WorkItemLogControlMadeChanges), culture);

    public static string WorkItemLogControlMadeOtherChanges() => WITResources.Get(nameof (WorkItemLogControlMadeOtherChanges));

    public static string WorkItemLogControlMadeOtherChanges(CultureInfo culture) => WITResources.Get(nameof (WorkItemLogControlMadeOtherChanges), culture);

    public static string WorkItemLogControlStateChange(object arg0, object arg1) => WITResources.Format(nameof (WorkItemLogControlStateChange), arg0, arg1);

    public static string WorkItemLogControlStateChange(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (WorkItemLogControlStateChange), culture, arg0, arg1);
    }

    public static string WorkItemLogControlUserViaUser(object arg0, object arg1) => WITResources.Format(nameof (WorkItemLogControlUserViaUser), arg0, arg1);

    public static string WorkItemLogControlUserViaUser(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (WorkItemLogControlUserViaUser), culture, arg0, arg1);
    }

    public static string FutureIterations() => WITResources.Get(nameof (FutureIterations));

    public static string FutureIterations(CultureInfo culture) => WITResources.Get(nameof (FutureIterations), culture);

    public static string QueryManageDialog_Validator_FolderInvalid() => WITResources.Get(nameof (QueryManageDialog_Validator_FolderInvalid));

    public static string QueryManageDialog_Validator_FolderInvalid(CultureInfo culture) => WITResources.Get(nameof (QueryManageDialog_Validator_FolderInvalid), culture);

    public static string QueryManageDialog_Validator_FolderSelfReferential() => WITResources.Get(nameof (QueryManageDialog_Validator_FolderSelfReferential));

    public static string QueryManageDialog_Validator_FolderSelfReferential(CultureInfo culture) => WITResources.Get(nameof (QueryManageDialog_Validator_FolderSelfReferential), culture);

    public static string SaveQueryAs() => WITResources.Get(nameof (SaveQueryAs));

    public static string SaveQueryAs(CultureInfo culture) => WITResources.Get(nameof (SaveQueryAs), culture);

    public static string SaveQueryAsDialogTitle() => WITResources.Get(nameof (SaveQueryAsDialogTitle));

    public static string SaveQueryAsDialogTitle(CultureInfo culture) => WITResources.Get(nameof (SaveQueryAsDialogTitle), culture);

    public static string SaveQueryAsTooltip() => WITResources.Get(nameof (SaveQueryAsTooltip));

    public static string SaveQueryAsTooltip(CultureInfo culture) => WITResources.Get(nameof (SaveQueryAsTooltip), culture);

    public static string InvalidQuerySyntax(object arg0, object arg1) => WITResources.Format(nameof (InvalidQuerySyntax), arg0, arg1);

    public static string InvalidQuerySyntax(object arg0, object arg1, CultureInfo culture) => WITResources.Format(nameof (InvalidQuerySyntax), culture, arg0, arg1);

    public static string ProjectDoesNotExist(object arg0) => WITResources.Format(nameof (ProjectDoesNotExist), arg0);

    public static string ProjectDoesNotExist(object arg0, CultureInfo culture) => WITResources.Format(nameof (ProjectDoesNotExist), culture, arg0);

    public static string QueryItemAlreadyExist(object arg0) => WITResources.Format(nameof (QueryItemAlreadyExist), arg0);

    public static string QueryItemAlreadyExist(object arg0, CultureInfo culture) => WITResources.Format(nameof (QueryItemAlreadyExist), culture, arg0);

    public static string QueryReferencesFieldThatDoesNotExist() => WITResources.Get(nameof (QueryReferencesFieldThatDoesNotExist));

    public static string QueryReferencesFieldThatDoesNotExist(CultureInfo culture) => WITResources.Get(nameof (QueryReferencesFieldThatDoesNotExist), culture);

    public static string WorkItemBulkSaveFailed() => WITResources.Get(nameof (WorkItemBulkSaveFailed));

    public static string WorkItemBulkSaveFailed(CultureInfo culture) => WITResources.Get(nameof (WorkItemBulkSaveFailed), culture);

    public static string WorkItemLinkTypeDoesNotExists(object arg0) => WITResources.Format(nameof (WorkItemLinkTypeDoesNotExists), arg0);

    public static string WorkItemLinkTypeDoesNotExists(object arg0, CultureInfo culture) => WITResources.Format(nameof (WorkItemLinkTypeDoesNotExists), culture, arg0);

    public static string WorkItemLinkTypeEndDoesNotExist(object arg0) => WITResources.Format(nameof (WorkItemLinkTypeEndDoesNotExist), arg0);

    public static string WorkItemLinkTypeEndDoesNotExist(object arg0, CultureInfo culture) => WITResources.Format(nameof (WorkItemLinkTypeEndDoesNotExist), culture, arg0);

    public static string LinksControlUnsafeUrl() => WITResources.Get(nameof (LinksControlUnsafeUrl));

    public static string LinksControlUnsafeUrl(CultureInfo culture) => WITResources.Get(nameof (LinksControlUnsafeUrl), culture);

    public static string QueryHierarchyItemAriaLabel(object arg0, object arg1, object arg2) => WITResources.Format(nameof (QueryHierarchyItemAriaLabel), arg0, arg1, arg2);

    public static string QueryHierarchyItemAriaLabel(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (QueryHierarchyItemAriaLabel), culture, arg0, arg1, arg2);
    }

    public static string Query() => WITResources.Get(nameof (Query));

    public static string Query(CultureInfo culture) => WITResources.Get(nameof (Query), culture);

    public static string Folder() => WITResources.Get(nameof (Folder));

    public static string Folder(CultureInfo culture) => WITResources.Get(nameof (Folder), culture);

    public static string QueryFilterErrorEmptyValue(object arg0) => WITResources.Format(nameof (QueryFilterErrorEmptyValue), arg0);

    public static string QueryFilterErrorEmptyValue(object arg0, CultureInfo culture) => WITResources.Format(nameof (QueryFilterErrorEmptyValue), culture, arg0);

    public static string QueryFilterErrorMissingOperator(object arg0) => WITResources.Format(nameof (QueryFilterErrorMissingOperator), arg0);

    public static string QueryFilterErrorMissingOperator(object arg0, CultureInfo culture) => WITResources.Format(nameof (QueryFilterErrorMissingOperator), culture, arg0);

    public static string QueryFilterErrorUnrecognizedOperator(object arg0) => WITResources.Format(nameof (QueryFilterErrorUnrecognizedOperator), arg0);

    public static string QueryFilterErrorUnrecognizedOperator(object arg0, CultureInfo culture) => WITResources.Format(nameof (QueryFilterErrorUnrecognizedOperator), culture, arg0);

    public static string CreateWorkitemIVURLSuccess() => WITResources.Get(nameof (CreateWorkitemIVURLSuccess));

    public static string CreateWorkitemIVURLSuccess(CultureInfo culture) => WITResources.Get(nameof (CreateWorkitemIVURLSuccess), culture);

    public static string NewLinkedWorkItem() => WITResources.Get(nameof (NewLinkedWorkItem));

    public static string NewLinkedWorkItem(CultureInfo culture) => WITResources.Get(nameof (NewLinkedWorkItem), culture);

    public static string StartStoryboarding() => WITResources.Get(nameof (StartStoryboarding));

    public static string StartStoryboarding(CultureInfo culture) => WITResources.Get(nameof (StartStoryboarding), culture);

    public static string BulkEditSelectedWorkItems() => WITResources.Get(nameof (BulkEditSelectedWorkItems));

    public static string BulkEditSelectedWorkItems(CultureInfo culture) => WITResources.Get(nameof (BulkEditSelectedWorkItems), culture);

    public static string BulkEditWorkItemsTitle() => WITResources.Get(nameof (BulkEditWorkItemsTitle));

    public static string BulkEditWorkItemsTitle(CultureInfo culture) => WITResources.Get(nameof (BulkEditWorkItemsTitle), culture);

    public static string BulkEditWorkItemsNotesForHistoryLabel() => WITResources.Get(nameof (BulkEditWorkItemsNotesForHistoryLabel));

    public static string BulkEditWorkItemsNotesForHistoryLabel(CultureInfo culture) => WITResources.Get(nameof (BulkEditWorkItemsNotesForHistoryLabel), culture);

    public static string AdhocQueryDefaultName() => WITResources.Get(nameof (AdhocQueryDefaultName));

    public static string AdhocQueryDefaultName(CultureInfo culture) => WITResources.Get(nameof (AdhocQueryDefaultName), culture);

    public static string QueryingStatusText() => WITResources.Get(nameof (QueryingStatusText));

    public static string QueryingStatusText(CultureInfo culture) => WITResources.Get(nameof (QueryingStatusText), culture);

    public static string Searching() => WITResources.Get(nameof (Searching));

    public static string Searching(CultureInfo culture) => WITResources.Get(nameof (Searching), culture);

    public static string QueryResultsNewQueryGridStatusText() => WITResources.Get(nameof (QueryResultsNewQueryGridStatusText));

    public static string QueryResultsNewQueryGridStatusText(CultureInfo culture) => WITResources.Get(nameof (QueryResultsNewQueryGridStatusText), culture);

    public static string QueryDirtyDocumentTitleFormat(object arg0) => WITResources.Format(nameof (QueryDirtyDocumentTitleFormat), arg0);

    public static string QueryDirtyDocumentTitleFormat(object arg0, CultureInfo culture) => WITResources.Format(nameof (QueryDirtyDocumentTitleFormat), culture, arg0);

    public static string QueryEditorQueryMatchTopLevelItems() => WITResources.Get(nameof (QueryEditorQueryMatchTopLevelItems));

    public static string QueryEditorQueryMatchTopLevelItems(CultureInfo culture) => WITResources.Get(nameof (QueryEditorQueryMatchTopLevelItems), culture);

    public static string QueryResultsGridStatusNoResultsText() => WITResources.Get(nameof (QueryResultsGridStatusNoResultsText));

    public static string QueryResultsGridStatusNoResultsText(CultureInfo culture) => WITResources.Get(nameof (QueryResultsGridStatusNoResultsText), culture);

    public static string NoResultsFound() => WITResources.Get(nameof (NoResultsFound));

    public static string NoResultsFound(CultureInfo culture) => WITResources.Get(nameof (NoResultsFound), culture);

    public static string NewMenuTitle() => WITResources.Get(nameof (NewMenuTitle));

    public static string NewMenuTitle(CultureInfo culture) => WITResources.Get(nameof (NewMenuTitle), culture);

    public static string QueryEditorQueryMatchLinkedItems() => WITResources.Get(nameof (QueryEditorQueryMatchLinkedItems));

    public static string QueryEditorQueryMatchLinkedItems(CultureInfo culture) => WITResources.Get(nameof (QueryEditorQueryMatchLinkedItems), culture);

    public static string ErrorCannotCreateWorkItemControl(object arg0) => WITResources.Format(nameof (ErrorCannotCreateWorkItemControl), arg0);

    public static string ErrorCannotCreateWorkItemControl(object arg0, CultureInfo culture) => WITResources.Format(nameof (ErrorCannotCreateWorkItemControl), culture, arg0);

    public static string ErrorCannotCreateLegacyExtension(object arg0) => WITResources.Format(nameof (ErrorCannotCreateLegacyExtension), arg0);

    public static string ErrorCannotCreateLegacyExtension(object arg0, CultureInfo culture) => WITResources.Format(nameof (ErrorCannotCreateLegacyExtension), culture, arg0);

    public static string WorkItemEditorCaption(object arg0, object arg1) => WITResources.Format(nameof (WorkItemEditorCaption), arg0, arg1);

    public static string WorkItemEditorCaption(object arg0, object arg1, CultureInfo culture) => WITResources.Format(nameof (WorkItemEditorCaption), culture, arg0, arg1);

    public static string WorkItemEditorCaptionNew(object arg0, object arg1) => WITResources.Format(nameof (WorkItemEditorCaptionNew), arg0, arg1);

    public static string WorkItemEditorCaptionNew(object arg0, object arg1, CultureInfo culture) => WITResources.Format(nameof (WorkItemEditorCaptionNew), culture, arg0, arg1);

    public static string WorkItemEditorDirtyCaption(object arg0) => WITResources.Format(nameof (WorkItemEditorDirtyCaption), arg0);

    public static string WorkItemEditorDirtyCaption(object arg0, CultureInfo culture) => WITResources.Format(nameof (WorkItemEditorDirtyCaption), culture, arg0);

    public static string WorkItemEditorWindowTitle(object arg0, object arg1) => WITResources.Format(nameof (WorkItemEditorWindowTitle), arg0, arg1);

    public static string WorkItemEditorWindowTitle(object arg0, object arg1, CultureInfo culture) => WITResources.Format(nameof (WorkItemEditorWindowTitle), culture, arg0, arg1);

    public static string RecentWorkItems() => WITResources.Get(nameof (RecentWorkItems));

    public static string RecentWorkItems(CultureInfo culture) => WITResources.Get(nameof (RecentWorkItems), culture);

    public static string FolderLabel() => WITResources.Get(nameof (FolderLabel));

    public static string FolderLabel(CultureInfo culture) => WITResources.Get(nameof (FolderLabel), culture);

    public static string DiscardWorkItem() => WITResources.Get(nameof (DiscardWorkItem));

    public static string DiscardWorkItem(CultureInfo culture) => WITResources.Get(nameof (DiscardWorkItem), culture);

    public static string LinkFormTestResultMessage() => WITResources.Get(nameof (LinkFormTestResultMessage));

    public static string LinkFormTestResultMessage(CultureInfo culture) => WITResources.Get(nameof (LinkFormTestResultMessage), culture);

    public static string LinkFormTestMessage() => WITResources.Get(nameof (LinkFormTestMessage));

    public static string LinkFormTestMessage(CultureInfo culture) => WITResources.Get(nameof (LinkFormTestMessage), culture);

    public static string NoMyFavoriteQueries() => WITResources.Get(nameof (NoMyFavoriteQueries));

    public static string NoMyFavoriteQueries(CultureInfo culture) => WITResources.Get(nameof (NoMyFavoriteQueries), culture);

    public static string NoTeamFavoriteQueries() => WITResources.Get(nameof (NoTeamFavoriteQueries));

    public static string NoTeamFavoriteQueries(CultureInfo culture) => WITResources.Get(nameof (NoTeamFavoriteQueries), culture);

    public static string LinkFormResultAttachmentMessage() => WITResources.Get(nameof (LinkFormResultAttachmentMessage));

    public static string LinkFormResultAttachmentMessage(CultureInfo culture) => WITResources.Get(nameof (LinkFormResultAttachmentMessage), culture);

    public static string ConfirmWorkItemDiscardNew() => WITResources.Get(nameof (ConfirmWorkItemDiscardNew));

    public static string ConfirmWorkItemDiscardNew(CultureInfo culture) => WITResources.Get(nameof (ConfirmWorkItemDiscardNew), culture);

    public static string MovingQueryLabel(object arg0) => WITResources.Format(nameof (MovingQueryLabel), arg0);

    public static string MovingQueryLabel(object arg0, CultureInfo culture) => WITResources.Format(nameof (MovingQueryLabel), culture, arg0);

    public static string DragQueryMoveFailed(object arg0, object arg1) => WITResources.Format(nameof (DragQueryMoveFailed), arg0, arg1);

    public static string DragQueryMoveFailed(object arg0, object arg1, CultureInfo culture) => WITResources.Format(nameof (DragQueryMoveFailed), culture, arg0, arg1);

    public static string DragFavoriteFailed(object arg0, object arg1) => WITResources.Format(nameof (DragFavoriteFailed), arg0, arg1);

    public static string DragFavoriteFailed(object arg0, object arg1, CultureInfo culture) => WITResources.Format(nameof (DragFavoriteFailed), culture, arg0, arg1);

    public static string QueryConfirmDragDropOperation(object arg0, object arg1) => WITResources.Format(nameof (QueryConfirmDragDropOperation), arg0, arg1);

    public static string QueryConfirmDragDropOperation(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (QueryConfirmDragDropOperation), culture, arg0, arg1);
    }

    public static string CopyNameTemplate(object arg0, object arg1) => WITResources.Format(nameof (CopyNameTemplate), arg0, arg1);

    public static string CopyNameTemplate(object arg0, object arg1, CultureInfo culture) => WITResources.Format(nameof (CopyNameTemplate), culture, arg0, arg1);

    public static string TriageNextWorkItemToolTip(object arg0) => WITResources.Format(nameof (TriageNextWorkItemToolTip), arg0);

    public static string TriageNextWorkItemToolTip(object arg0, CultureInfo culture) => WITResources.Format(nameof (TriageNextWorkItemToolTip), culture, arg0);

    public static string TriagePreviousWorkItemToolTip(object arg0) => WITResources.Format(nameof (TriagePreviousWorkItemToolTip), arg0);

    public static string TriagePreviousWorkItemToolTip(object arg0, CultureInfo culture) => WITResources.Format(nameof (TriagePreviousWorkItemToolTip), culture, arg0);

    public static string BackToQueryResults(object arg0) => WITResources.Format(nameof (BackToQueryResults), arg0);

    public static string BackToQueryResults(object arg0, CultureInfo culture) => WITResources.Format(nameof (BackToQueryResults), culture, arg0);

    public static string TriageSummary(object arg0, object arg1) => WITResources.Format(nameof (TriageSummary), arg0, arg1);

    public static string TriageSummary(object arg0, object arg1, CultureInfo culture) => WITResources.Format(nameof (TriageSummary), culture, arg0, arg1);

    public static string StateTransitionGraphViewAllChangesText() => WITResources.Get(nameof (StateTransitionGraphViewAllChangesText));

    public static string StateTransitionGraphViewAllChangesText(CultureInfo culture) => WITResources.Get(nameof (StateTransitionGraphViewAllChangesText), culture);

    public static string StateTransitionAriaLabel(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4)
    {
      return WITResources.Format(nameof (StateTransitionAriaLabel), arg0, arg1, arg2, arg3, arg4);
    }

    public static string StateTransitionAriaLabel(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (StateTransitionAriaLabel), culture, arg0, arg1, arg2, arg3, arg4);
    }

    public static string StateTransitionGraphViewAllChangesTitle() => WITResources.Get(nameof (StateTransitionGraphViewAllChangesTitle));

    public static string StateTransitionGraphViewAllChangesTitle(CultureInfo culture) => WITResources.Get(nameof (StateTransitionGraphViewAllChangesTitle), culture);

    public static string ErrorDateTimeValueOutOfRange() => WITResources.Get(nameof (ErrorDateTimeValueOutOfRange));

    public static string ErrorDateTimeValueOutOfRange(CultureInfo culture) => WITResources.Get(nameof (ErrorDateTimeValueOutOfRange), culture);

    public static string ErrorExpectingBooleanValue() => WITResources.Get(nameof (ErrorExpectingBooleanValue));

    public static string ErrorExpectingBooleanValue(CultureInfo culture) => WITResources.Get(nameof (ErrorExpectingBooleanValue), culture);

    public static string ErrorExpectingDateTime() => WITResources.Get(nameof (ErrorExpectingDateTime));

    public static string ErrorExpectingDateTime(CultureInfo culture) => WITResources.Get(nameof (ErrorExpectingDateTime), culture);

    public static string ErrorExpectingIntegerValue() => WITResources.Get(nameof (ErrorExpectingIntegerValue));

    public static string ErrorExpectingIntegerValue(CultureInfo culture) => WITResources.Get(nameof (ErrorExpectingIntegerValue), culture);

    public static string ErrorExpectingNumericValue() => WITResources.Get(nameof (ErrorExpectingNumericValue));

    public static string ErrorExpectingNumericValue(CultureInfo culture) => WITResources.Get(nameof (ErrorExpectingNumericValue), culture);

    public static string ErrorExpectingTeam() => WITResources.Get(nameof (ErrorExpectingTeam));

    public static string ErrorExpectingTeam(CultureInfo culture) => WITResources.Get(nameof (ErrorExpectingTeam), culture);

    public static string TagsLabelText() => WITResources.Get(nameof (TagsLabelText));

    public static string TagsLabelText(CultureInfo culture) => WITResources.Get(nameof (TagsLabelText), culture);

    public static string TagsDeleteButton_AriaLabel(object arg0) => WITResources.Format(nameof (TagsDeleteButton_AriaLabel), arg0);

    public static string TagsDeleteButton_AriaLabel(object arg0, CultureInfo culture) => WITResources.Format(nameof (TagsDeleteButton_AriaLabel), culture, arg0);

    public static string ColumnOptionsTooltip() => WITResources.Get(nameof (ColumnOptionsTooltip));

    public static string ColumnOptionsTooltip(CultureInfo culture) => WITResources.Get(nameof (ColumnOptionsTooltip), culture);

    public static string EmailQueryResult() => WITResources.Get(nameof (EmailQueryResult));

    public static string EmailQueryResult(CultureInfo culture) => WITResources.Get(nameof (EmailQueryResult), culture);

    public static string EmailSelectedWorkItems() => WITResources.Get(nameof (EmailSelectedWorkItems));

    public static string EmailSelectedWorkItems(CultureInfo culture) => WITResources.Get(nameof (EmailSelectedWorkItems), culture);

    public static string SendWorkItemsInEmailDialogTitle() => WITResources.Get(nameof (SendWorkItemsInEmailDialogTitle));

    public static string SendWorkItemsInEmailDialogTitle(CultureInfo culture) => WITResources.Get(nameof (SendWorkItemsInEmailDialogTitle), culture);

    public static string ErrorEmailUnsavedQuery(object arg0) => WITResources.Format(nameof (ErrorEmailUnsavedQuery), arg0);

    public static string ErrorEmailUnsavedQuery(object arg0, CultureInfo culture) => WITResources.Format(nameof (ErrorEmailUnsavedQuery), culture, arg0);

    public static string ErrorEmailEmptyQueryResults() => WITResources.Get(nameof (ErrorEmailEmptyQueryResults));

    public static string ErrorEmailEmptyQueryResults(CultureInfo culture) => WITResources.Get(nameof (ErrorEmailEmptyQueryResults), culture);

    public static string EmailWorkItemLimit(object arg0) => WITResources.Format(nameof (EmailWorkItemLimit), arg0);

    public static string EmailWorkItemLimit(object arg0, CultureInfo culture) => WITResources.Format(nameof (EmailWorkItemLimit), culture, arg0);

    public static string LinksControlChangesetText() => WITResources.Get(nameof (LinksControlChangesetText));

    public static string LinksControlChangesetText(CultureInfo culture) => WITResources.Get(nameof (LinksControlChangesetText), culture);

    public static string LinksControlCommitText() => WITResources.Get(nameof (LinksControlCommitText));

    public static string LinksControlCommitText(CultureInfo culture) => WITResources.Get(nameof (LinksControlCommitText), culture);

    public static string LinksControlVersionedItemText() => WITResources.Get(nameof (LinksControlVersionedItemText));

    public static string LinksControlVersionedItemText(CultureInfo culture) => WITResources.Get(nameof (LinksControlVersionedItemText), culture);

    public static string NewChartText() => WITResources.Get(nameof (NewChartText));

    public static string NewChartText(CultureInfo culture) => WITResources.Get(nameof (NewChartText), culture);

    public static string EmptyQueryChartList(object arg0) => WITResources.Format(nameof (EmptyQueryChartList), arg0);

    public static string EmptyQueryChartList(object arg0, CultureInfo culture) => WITResources.Format(nameof (EmptyQueryChartList), culture, arg0);

    public static string NoLicenseHostedEmptyQueryChartList() => WITResources.Get(nameof (NoLicenseHostedEmptyQueryChartList));

    public static string NoLicenseHostedEmptyQueryChartList(CultureInfo culture) => WITResources.Get(nameof (NoLicenseHostedEmptyQueryChartList), culture);

    public static string NoLicenseEmptyQueryChartList() => WITResources.Get(nameof (NoLicenseEmptyQueryChartList));

    public static string NoLicenseEmptyQueryChartList(CultureInfo culture) => WITResources.Get(nameof (NoLicenseEmptyQueryChartList), culture);

    public static string DeletedAreaPath() => WITResources.Get(nameof (DeletedAreaPath));

    public static string DeletedAreaPath(CultureInfo culture) => WITResources.Get(nameof (DeletedAreaPath), culture);

    public static string DeletedIterationPath() => WITResources.Get(nameof (DeletedIterationPath));

    public static string DeletedIterationPath(CultureInfo culture) => WITResources.Get(nameof (DeletedIterationPath), culture);

    public static string WorkItemChartsTreeQueryDisallowedMessage() => WITResources.Get(nameof (WorkItemChartsTreeQueryDisallowedMessage));

    public static string WorkItemChartsTreeQueryDisallowedMessage(CultureInfo culture) => WITResources.Get(nameof (WorkItemChartsTreeQueryDisallowedMessage), culture);

    public static string WorkItemChartsUnrecognizedQueryMessage() => WITResources.Get(nameof (WorkItemChartsUnrecognizedQueryMessage));

    public static string WorkItemChartsUnrecognizedQueryMessage(CultureInfo culture) => WITResources.Get(nameof (WorkItemChartsUnrecognizedQueryMessage), culture);

    public static string WorkItemChartsDisallowedAdhocQuery() => WITResources.Get(nameof (WorkItemChartsDisallowedAdhocQuery));

    public static string WorkItemChartsDisallowedAdhocQuery(CultureInfo culture) => WITResources.Get(nameof (WorkItemChartsDisallowedAdhocQuery), culture);

    public static string WorkItemChartsUnsavedQuery() => WITResources.Get(nameof (WorkItemChartsUnsavedQuery));

    public static string WorkItemChartsUnsavedQuery(CultureInfo culture) => WITResources.Get(nameof (WorkItemChartsUnsavedQuery), culture);

    public static string WorkItemChartsPageNotReady() => WITResources.Get(nameof (WorkItemChartsPageNotReady));

    public static string WorkItemChartsPageNotReady(CultureInfo culture) => WITResources.Get(nameof (WorkItemChartsPageNotReady), culture);

    public static string ErrorEmailUnsavedWorkItems() => WITResources.Get(nameof (ErrorEmailUnsavedWorkItems));

    public static string ErrorEmailUnsavedWorkItems(CultureInfo culture) => WITResources.Get(nameof (ErrorEmailUnsavedWorkItems), culture);

    public static string Backlog() => WITResources.Get(nameof (Backlog));

    public static string Backlog(CultureInfo culture) => WITResources.Get(nameof (Backlog), culture);

    public static string CopyQueryURL() => WITResources.Get(nameof (CopyQueryURL));

    public static string CopyQueryURL(CultureInfo culture) => WITResources.Get(nameof (CopyQueryURL), culture);

    public static string CopyQueryURLTitle() => WITResources.Get(nameof (CopyQueryURLTitle));

    public static string CopyQueryURLTitle(CultureInfo culture) => WITResources.Get(nameof (CopyQueryURLTitle), culture);

    public static string ShowMoreWorkItemsMessage(object arg0, object arg1) => WITResources.Format(nameof (ShowMoreWorkItemsMessage), arg0, arg1);

    public static string ShowMoreWorkItemsMessage(object arg0, object arg1, CultureInfo culture) => WITResources.Format(nameof (ShowMoreWorkItemsMessage), culture, arg0, arg1);

    public static string ShowMoreWorkItemsLinkMessage() => WITResources.Get(nameof (ShowMoreWorkItemsLinkMessage));

    public static string ShowMoreWorkItemsLinkMessage(CultureInfo culture) => WITResources.Get(nameof (ShowMoreWorkItemsLinkMessage), culture);

    public static string QueryTree_Loading() => WITResources.Get(nameof (QueryTree_Loading));

    public static string QueryTree_Loading(CultureInfo culture) => WITResources.Get(nameof (QueryTree_Loading), culture);

    public static string RichEditorMaximizeName(object arg0) => WITResources.Format(nameof (RichEditorMaximizeName), arg0);

    public static string RichEditorMaximizeName(object arg0, CultureInfo culture) => WITResources.Format(nameof (RichEditorMaximizeName), culture, arg0);

    public static string RichEditorRestoreName(object arg0) => WITResources.Format(nameof (RichEditorRestoreName), arg0);

    public static string RichEditorRestoreName(object arg0, CultureInfo culture) => WITResources.Format(nameof (RichEditorRestoreName), culture, arg0);

    public static string WorkItemIDListString() => WITResources.Get(nameof (WorkItemIDListString));

    public static string WorkItemIDListString(CultureInfo culture) => WITResources.Get(nameof (WorkItemIDListString), culture);

    public static string WorkItemFinder_AriaLabel_FindResultsGrid() => WITResources.Get(nameof (WorkItemFinder_AriaLabel_FindResultsGrid));

    public static string WorkItemFinder_AriaLabel_FindResultsGrid(CultureInfo culture) => WITResources.Get(nameof (WorkItemFinder_AriaLabel_FindResultsGrid), culture);

    public static string WorkItemFinderSelectQueryText() => WITResources.Get(nameof (WorkItemFinderSelectQueryText));

    public static string WorkItemFinderSelectQueryText(CultureInfo culture) => WITResources.Get(nameof (WorkItemFinderSelectQueryText), culture);

    public static string WorkItemFinderSelectFavoriteText() => WITResources.Get(nameof (WorkItemFinderSelectFavoriteText));

    public static string WorkItemFinderSelectFavoriteText(CultureInfo culture) => WITResources.Get(nameof (WorkItemFinderSelectFavoriteText), culture);

    public static string WorkItemFinderResultStatusString(object arg0) => WITResources.Format(nameof (WorkItemFinderResultStatusString), arg0);

    public static string WorkItemFinderResultStatusString(object arg0, CultureInfo culture) => WITResources.Format(nameof (WorkItemFinderResultStatusString), culture, arg0);

    public static string WorkItemFinderAllWorkItemTypes() => WITResources.Get(nameof (WorkItemFinderAllWorkItemTypes));

    public static string WorkItemFinderAllWorkItemTypes(CultureInfo culture) => WITResources.Get(nameof (WorkItemFinderAllWorkItemTypes), culture);

    public static string WorkItemFinderAnyProject() => WITResources.Get(nameof (WorkItemFinderAnyProject));

    public static string WorkItemFinderAnyProject(CultureInfo culture) => WITResources.Get(nameof (WorkItemFinderAnyProject), culture);

    public static string WorkItemFinderDialogTitle() => WITResources.Get(nameof (WorkItemFinderDialogTitle));

    public static string WorkItemFinderDialogTitle(CultureInfo culture) => WITResources.Get(nameof (WorkItemFinderDialogTitle), culture);

    public static string WorkItemFinderQueryInProgress() => WITResources.Get(nameof (WorkItemFinderQueryInProgress));

    public static string WorkItemFinderQueryInProgress(CultureInfo culture) => WITResources.Get(nameof (WorkItemFinderQueryInProgress), culture);

    public static string CopySelectedWorkItemsAsHtml() => WITResources.Get(nameof (CopySelectedWorkItemsAsHtml));

    public static string CopySelectedWorkItemsAsHtml(CultureInfo culture) => WITResources.Get(nameof (CopySelectedWorkItemsAsHtml), culture);

    public static string CopySelectedWorkItemsIdColumnTitle() => WITResources.Get(nameof (CopySelectedWorkItemsIdColumnTitle));

    public static string CopySelectedWorkItemsIdColumnTitle(CultureInfo culture) => WITResources.Get(nameof (CopySelectedWorkItemsIdColumnTitle), culture);

    public static string CopySelectedWorkitemsOpenAsQuery() => WITResources.Get(nameof (CopySelectedWorkitemsOpenAsQuery));

    public static string CopySelectedWorkitemsOpenAsQuery(CultureInfo culture) => WITResources.Get(nameof (CopySelectedWorkitemsOpenAsQuery), culture);

    public static string DeleteSelectedWorkItems() => WITResources.Get(nameof (DeleteSelectedWorkItems));

    public static string DeleteSelectedWorkItems(CultureInfo culture) => WITResources.Get(nameof (DeleteSelectedWorkItems), culture);

    public static string StatusMessage_FatalError(object arg0) => WITResources.Format(nameof (StatusMessage_FatalError), arg0);

    public static string StatusMessage_FatalError(object arg0, CultureInfo culture) => WITResources.Format(nameof (StatusMessage_FatalError), culture, arg0);

    public static string WorkItemLogControlStateGraphTitle() => WITResources.Get(nameof (WorkItemLogControlStateGraphTitle));

    public static string WorkItemLogControlStateGraphTitle(CultureInfo culture) => WITResources.Get(nameof (WorkItemLogControlStateGraphTitle), culture);

    public static string NoQueryFolderChildren() => WITResources.Get(nameof (NoQueryFolderChildren));

    public static string NoQueryFolderChildren(CultureInfo culture) => WITResources.Get(nameof (NoQueryFolderChildren), culture);

    public static string ChartEditor_Query_GroupingTooltip() => WITResources.Get(nameof (ChartEditor_Query_GroupingTooltip));

    public static string ChartEditor_Query_GroupingTooltip(CultureInfo culture) => WITResources.Get(nameof (ChartEditor_Query_GroupingTooltip), culture);

    public static string CopyQueryURLExpirationMessage(object arg0) => WITResources.Format(nameof (CopyQueryURLExpirationMessage), arg0);

    public static string CopyQueryURLExpirationMessage(object arg0, CultureInfo culture) => WITResources.Format(nameof (CopyQueryURLExpirationMessage), culture, arg0);

    public static string CopyQueryURLDataTooLargeException() => WITResources.Get(nameof (CopyQueryURLDataTooLargeException));

    public static string CopyQueryURLDataTooLargeException(CultureInfo culture) => WITResources.Get(nameof (CopyQueryURLDataTooLargeException), culture);

    public static string CopyQueryURLNotFoundException() => WITResources.Get(nameof (CopyQueryURLNotFoundException));

    public static string CopyQueryURLNotFoundException(CultureInfo culture) => WITResources.Get(nameof (CopyQueryURLNotFoundException), culture);

    public static string CopyQueryURLNotifyMessage() => WITResources.Get(nameof (CopyQueryURLNotifyMessage));

    public static string CopyQueryURLNotifyMessage(CultureInfo culture) => WITResources.Get(nameof (CopyQueryURLNotifyMessage), culture);

    public static string TextFilterNextItemsMessage(object arg0) => WITResources.Format(nameof (TextFilterNextItemsMessage), arg0);

    public static string TextFilterNextItemsMessage(object arg0, CultureInfo culture) => WITResources.Format(nameof (TextFilterNextItemsMessage), culture, arg0);

    public static string QueryAcrossProjects() => WITResources.Get(nameof (QueryAcrossProjects));

    public static string QueryAcrossProjects(CultureInfo culture) => WITResources.Get(nameof (QueryAcrossProjects), culture);

    public static string TextFilterResultCountMessage(object arg0, object arg1, object arg2) => WITResources.Format(nameof (TextFilterResultCountMessage), arg0, arg1, arg2);

    public static string TextFilterResultCountMessage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (TextFilterResultCountMessage), culture, arg0, arg1, arg2);
    }

    public static string EmailSelectedWorkItem() => WITResources.Get(nameof (EmailSelectedWorkItem));

    public static string EmailSelectedWorkItem(CultureInfo culture) => WITResources.Get(nameof (EmailSelectedWorkItem), culture);

    public static string EmailSelectedWorkItemToolTip() => WITResources.Get(nameof (EmailSelectedWorkItemToolTip));

    public static string EmailSelectedWorkItemToolTip(CultureInfo culture) => WITResources.Get(nameof (EmailSelectedWorkItemToolTip), culture);

    public static string TitleEmptyText() => WITResources.Get(nameof (TitleEmptyText));

    public static string TitleEmptyText(CultureInfo culture) => WITResources.Get(nameof (TitleEmptyText), culture);

    public static string AreaLabel() => WITResources.Get(nameof (AreaLabel));

    public static string AreaLabel(CultureInfo culture) => WITResources.Get(nameof (AreaLabel), culture);

    public static string IterationLabel() => WITResources.Get(nameof (IterationLabel));

    public static string IterationLabel(CultureInfo culture) => WITResources.Get(nameof (IterationLabel), culture);

    public static string AssignedToLabel() => WITResources.Get(nameof (AssignedToLabel));

    public static string AssignedToLabel(CultureInfo culture) => WITResources.Get(nameof (AssignedToLabel), culture);

    public static string StateLabel() => WITResources.Get(nameof (StateLabel));

    public static string StateLabel(CultureInfo culture) => WITResources.Get(nameof (StateLabel), culture);

    public static string LastUpdatedLabel() => WITResources.Get(nameof (LastUpdatedLabel));

    public static string LastUpdatedLabel(CultureInfo culture) => WITResources.Get(nameof (LastUpdatedLabel), culture);

    public static string LastUpdatedDateLabel() => WITResources.Get(nameof (LastUpdatedDateLabel));

    public static string LastUpdatedDateLabel(CultureInfo culture) => WITResources.Get(nameof (LastUpdatedDateLabel), culture);

    public static string UpdatedDateByMessage(object arg0, object arg1) => WITResources.Format(nameof (UpdatedDateByMessage), arg0, arg1);

    public static string UpdatedDateByMessage(object arg0, object arg1, CultureInfo culture) => WITResources.Format(nameof (UpdatedDateByMessage), culture, arg0, arg1);

    public static string UpdatedDateMessage(object arg0) => WITResources.Format(nameof (UpdatedDateMessage), arg0);

    public static string UpdatedDateMessage(object arg0, CultureInfo culture) => WITResources.Format(nameof (UpdatedDateMessage), culture, arg0);

    public static string AssignedToEmptyText() => WITResources.Get(nameof (AssignedToEmptyText));

    public static string AssignedToEmptyText(CultureInfo culture) => WITResources.Get(nameof (AssignedToEmptyText), culture);

    public static string StateEmptyText() => WITResources.Get(nameof (StateEmptyText));

    public static string StateEmptyText(CultureInfo culture) => WITResources.Get(nameof (StateEmptyText), culture);

    public static string AddTagText() => WITResources.Get(nameof (AddTagText));

    public static string AddTagText(CultureInfo culture) => WITResources.Get(nameof (AddTagText), culture);

    public static string AddTagPlusText() => WITResources.Get(nameof (AddTagPlusText));

    public static string AddTagPlusText(CultureInfo culture) => WITResources.Get(nameof (AddTagPlusText), culture);

    public static string Customize() => WITResources.Get(nameof (Customize));

    public static string Customize(CultureInfo culture) => WITResources.Get(nameof (Customize), culture);

    public static string AttachmentsNotSupported() => WITResources.Get(nameof (AttachmentsNotSupported));

    public static string AttachmentsNotSupported(CultureInfo culture) => WITResources.Get(nameof (AttachmentsNotSupported), culture);

    public static string InvalidArgumentExpectingArray() => WITResources.Get(nameof (InvalidArgumentExpectingArray));

    public static string InvalidArgumentExpectingArray(CultureInfo culture) => WITResources.Get(nameof (InvalidArgumentExpectingArray), culture);

    public static string NoActiveWorkItem() => WITResources.Get(nameof (NoActiveWorkItem));

    public static string NoActiveWorkItem(CultureInfo culture) => WITResources.Get(nameof (NoActiveWorkItem), culture);

    public static string InvalidRemoteWorkItemUrl() => WITResources.Get(nameof (InvalidRemoteWorkItemUrl));

    public static string InvalidRemoteWorkItemUrl(CultureInfo culture) => WITResources.Get(nameof (InvalidRemoteWorkItemUrl), culture);

    public static string InvalidWorkItemUrl(object arg0) => WITResources.Format(nameof (InvalidWorkItemUrl), arg0);

    public static string InvalidWorkItemUrl(object arg0, CultureInfo culture) => WITResources.Format(nameof (InvalidWorkItemUrl), culture, arg0);

    public static string CannotGetRegisteredInstance(object arg0) => WITResources.Format(nameof (CannotGetRegisteredInstance), arg0);

    public static string CannotGetRegisteredInstance(object arg0, CultureInfo culture) => WITResources.Format(nameof (CannotGetRegisteredInstance), culture, arg0);

    public static string ContributionTimedOut(object arg0) => WITResources.Format(nameof (ContributionTimedOut), arg0);

    public static string ContributionTimedOut(object arg0, CultureInfo culture) => WITResources.Format(nameof (ContributionTimedOut), culture, arg0);

    public static string MissingUri(object arg0) => WITResources.Format(nameof (MissingUri), arg0);

    public static string MissingUri(object arg0, CultureInfo culture) => WITResources.Format(nameof (MissingUri), culture, arg0);

    public static string LinksControlBranchText() => WITResources.Get(nameof (LinksControlBranchText));

    public static string LinksControlBranchText(CultureInfo culture) => WITResources.Get(nameof (LinksControlBranchText), culture);

    public static string LinksControlPullRequestText() => WITResources.Get(nameof (LinksControlPullRequestText));

    public static string LinksControlPullRequestText(CultureInfo culture) => WITResources.Get(nameof (LinksControlPullRequestText), culture);

    public static string LinksControlIssueText() => WITResources.Get(nameof (LinksControlIssueText));

    public static string LinksControlIssueText(CultureInfo culture) => WITResources.Get(nameof (LinksControlIssueText), culture);

    public static string LinksControlTagsText() => WITResources.Get(nameof (LinksControlTagsText));

    public static string LinksControlTagsText(CultureInfo culture) => WITResources.Get(nameof (LinksControlTagsText), culture);

    public static string LinksControlBuildText() => WITResources.Get(nameof (LinksControlBuildText));

    public static string LinksControlBuildText(CultureInfo culture) => WITResources.Get(nameof (LinksControlBuildText), culture);

    public static string LinksControlFoundInBuildText() => WITResources.Get(nameof (LinksControlFoundInBuildText));

    public static string LinksControlFoundInBuildText(CultureInfo culture) => WITResources.Get(nameof (LinksControlFoundInBuildText), culture);

    public static string LinksControlIntegratedInBuildText() => WITResources.Get(nameof (LinksControlIntegratedInBuildText));

    public static string LinksControlIntegratedInBuildText(CultureInfo culture) => WITResources.Get(nameof (LinksControlIntegratedInBuildText), culture);

    public static string KeyboardShortcutGroup_Work() => WITResources.Get(nameof (KeyboardShortcutGroup_Work));

    public static string KeyboardShortcutGroup_Work(CultureInfo culture) => WITResources.Get(nameof (KeyboardShortcutGroup_Work), culture);

    public static string KeyboardShortcutGroup_Queries() => WITResources.Get(nameof (KeyboardShortcutGroup_Queries));

    public static string KeyboardShortcutGroup_Queries(CultureInfo culture) => WITResources.Get(nameof (KeyboardShortcutGroup_Queries), culture);

    public static string KeyboardShortcutDescription_NewQuery() => WITResources.Get(nameof (KeyboardShortcutDescription_NewQuery));

    public static string KeyboardShortcutDescription_NewQuery(CultureInfo culture) => WITResources.Get(nameof (KeyboardShortcutDescription_NewQuery), culture);

    public static string KeyboardShortcutDescription_ReturnToQuery() => WITResources.Get(nameof (KeyboardShortcutDescription_ReturnToQuery));

    public static string KeyboardShortcutDescription_ReturnToQuery(CultureInfo culture) => WITResources.Get(nameof (KeyboardShortcutDescription_ReturnToQuery), culture);

    public static string KeyboardShortcutDescription_OpenWorkItems() => WITResources.Get(nameof (KeyboardShortcutDescription_OpenWorkItems));

    public static string KeyboardShortcutDescription_OpenWorkItems(CultureInfo culture) => WITResources.Get(nameof (KeyboardShortcutDescription_OpenWorkItems), culture);

    public static string KeyboardShortcutDescription_OpenBacklog() => WITResources.Get(nameof (KeyboardShortcutDescription_OpenBacklog));

    public static string KeyboardShortcutDescription_OpenBacklog(CultureInfo culture) => WITResources.Get(nameof (KeyboardShortcutDescription_OpenBacklog), culture);

    public static string KeyboardShortcutDescription_OpenCurrentIteration() => WITResources.Get(nameof (KeyboardShortcutDescription_OpenCurrentIteration));

    public static string KeyboardShortcutDescription_OpenCurrentIteration(CultureInfo culture) => WITResources.Get(nameof (KeyboardShortcutDescription_OpenCurrentIteration), culture);

    public static string KeyboardShortcutDescription_OpenTaskBoard() => WITResources.Get(nameof (KeyboardShortcutDescription_OpenTaskBoard));

    public static string KeyboardShortcutDescription_OpenTaskBoard(CultureInfo culture) => WITResources.Get(nameof (KeyboardShortcutDescription_OpenTaskBoard), culture);

    public static string KeyboardShortcutDescription_ToggleFullScreen() => WITResources.Get(nameof (KeyboardShortcutDescription_ToggleFullScreen));

    public static string KeyboardShortcutDescription_ToggleFullScreen(CultureInfo culture) => WITResources.Get(nameof (KeyboardShortcutDescription_ToggleFullScreen), culture);

    public static string KeyboardShortcutDescription_RefreshQuery() => WITResources.Get(nameof (KeyboardShortcutDescription_RefreshQuery));

    public static string KeyboardShortcutDescription_RefreshQuery(CultureInfo culture) => WITResources.Get(nameof (KeyboardShortcutDescription_RefreshQuery), culture);

    public static string KeyboardShortcutDescription_Next_Item() => WITResources.Get(nameof (KeyboardShortcutDescription_Next_Item));

    public static string KeyboardShortcutDescription_Next_Item(CultureInfo culture) => WITResources.Get(nameof (KeyboardShortcutDescription_Next_Item), culture);

    public static string KeyboardShortcutDescription_Previous_Item() => WITResources.Get(nameof (KeyboardShortcutDescription_Previous_Item));

    public static string KeyboardShortcutDescription_Previous_Item(CultureInfo culture) => WITResources.Get(nameof (KeyboardShortcutDescription_Previous_Item), culture);

    public static string KeyboardShortcutDescription_FilterResults() => WITResources.Get(nameof (KeyboardShortcutDescription_FilterResults));

    public static string KeyboardShortcutDescription_FilterResults(CultureInfo culture) => WITResources.Get(nameof (KeyboardShortcutDescription_FilterResults), culture);

    public static string WorkItemDiscussionAdornmentTooltipSingular() => WITResources.Get(nameof (WorkItemDiscussionAdornmentTooltipSingular));

    public static string WorkItemDiscussionAdornmentTooltipSingular(CultureInfo culture) => WITResources.Get(nameof (WorkItemDiscussionAdornmentTooltipSingular), culture);

    public static string WorkItemDiscussionAdornmentTooltip(object arg0) => WITResources.Format(nameof (WorkItemDiscussionAdornmentTooltip), arg0);

    public static string WorkItemDiscussionAdornmentTooltip(object arg0, CultureInfo culture) => WITResources.Format(nameof (WorkItemDiscussionAdornmentTooltip), culture, arg0);

    public static string GoToDiscussionShortcutWithControl() => WITResources.Get(nameof (GoToDiscussionShortcutWithControl));

    public static string GoToDiscussionShortcutWithControl(CultureInfo culture) => WITResources.Get(nameof (GoToDiscussionShortcutWithControl), culture);

    public static string GoToDiscussionShortcutWithCommand() => WITResources.Get(nameof (GoToDiscussionShortcutWithCommand));

    public static string GoToDiscussionShortcutWithCommand(CultureInfo culture) => WITResources.Get(nameof (GoToDiscussionShortcutWithCommand), culture);

    public static string WorkItemDiscussionAdornmentOverLimitNumberDisplay(object arg0) => WITResources.Format(nameof (WorkItemDiscussionAdornmentOverLimitNumberDisplay), arg0);

    public static string WorkItemDiscussionAdornmentOverLimitNumberDisplay(
      object arg0,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (WorkItemDiscussionAdornmentOverLimitNumberDisplay), culture, arg0);
    }

    public static string RecycleBin() => WITResources.Get(nameof (RecycleBin));

    public static string RecycleBin(CultureInfo culture) => WITResources.Get(nameof (RecycleBin), culture);

    public static string DeleteWorkItemDeleteButtonText() => WITResources.Get(nameof (DeleteWorkItemDeleteButtonText));

    public static string DeleteWorkItemDeleteButtonText(CultureInfo culture) => WITResources.Get(nameof (DeleteWorkItemDeleteButtonText), culture);

    public static string DeleteWorkItemDialogConfirmationTextWithoutCleanUpScheduled() => WITResources.Get(nameof (DeleteWorkItemDialogConfirmationTextWithoutCleanUpScheduled));

    public static string DeleteWorkItemDialogConfirmationTextWithoutCleanUpScheduled(
      CultureInfo culture)
    {
      return WITResources.Get(nameof (DeleteWorkItemDialogConfirmationTextWithoutCleanUpScheduled), culture);
    }

    public static string DeleteWorkItemDialogConfirmationTextWithCleanUpScheduled(object arg0) => WITResources.Format(nameof (DeleteWorkItemDialogConfirmationTextWithCleanUpScheduled), arg0);

    public static string DeleteWorkItemDialogConfirmationTextWithCleanUpScheduled(
      object arg0,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (DeleteWorkItemDialogConfirmationTextWithCleanUpScheduled), culture, arg0);
    }

    public static string DeleteWorkItemDialogTitle() => WITResources.Get(nameof (DeleteWorkItemDialogTitle));

    public static string DeleteWorkItemDialogTitle(CultureInfo culture) => WITResources.Get(nameof (DeleteWorkItemDialogTitle), culture);

    public static string DeleteWorkItemDialogRefreshWarningMessage() => WITResources.Get(nameof (DeleteWorkItemDialogRefreshWarningMessage));

    public static string DeleteWorkItemDialogRefreshWarningMessage(CultureInfo culture) => WITResources.Get(nameof (DeleteWorkItemDialogRefreshWarningMessage), culture);

    public static string FormGridSectionLabel(object arg0) => WITResources.Format(nameof (FormGridSectionLabel), arg0);

    public static string FormGridSectionLabel(object arg0, CultureInfo culture) => WITResources.Format(nameof (FormGridSectionLabel), culture, arg0);

    public static string WorkItemLogControlDeletedWorkItem() => WITResources.Get(nameof (WorkItemLogControlDeletedWorkItem));

    public static string WorkItemLogControlDeletedWorkItem(CultureInfo culture) => WITResources.Get(nameof (WorkItemLogControlDeletedWorkItem), culture);

    public static string WorkItemLogControlRestoredWorkItem() => WITResources.Get(nameof (WorkItemLogControlRestoredWorkItem));

    public static string WorkItemLogControlRestoredWorkItem(CultureInfo culture) => WITResources.Get(nameof (WorkItemLogControlRestoredWorkItem), culture);

    public static string DestroyWorkItemsTooltip() => WITResources.Get(nameof (DestroyWorkItemsTooltip));

    public static string DestroyWorkItemsTooltip(CultureInfo culture) => WITResources.Get(nameof (DestroyWorkItemsTooltip), culture);

    public static string DestroyWorkItemDialogTitle() => WITResources.Get(nameof (DestroyWorkItemDialogTitle));

    public static string DestroyWorkItemDialogTitle(CultureInfo culture) => WITResources.Get(nameof (DestroyWorkItemDialogTitle), culture);

    public static string DestroyWorkItemDeleteButtonText() => WITResources.Get(nameof (DestroyWorkItemDeleteButtonText));

    public static string DestroyWorkItemDeleteButtonText(CultureInfo culture) => WITResources.Get(nameof (DestroyWorkItemDeleteButtonText), culture);

    public static string DestroyWorkItemDialogConfirmationText() => WITResources.Get(nameof (DestroyWorkItemDialogConfirmationText));

    public static string DestroyWorkItemDialogConfirmationText(CultureInfo culture) => WITResources.Get(nameof (DestroyWorkItemDialogConfirmationText), culture);

    public static string RestoreWorkItemsTooltip() => WITResources.Get(nameof (RestoreWorkItemsTooltip));

    public static string RestoreWorkItemsTooltip(CultureInfo culture) => WITResources.Get(nameof (RestoreWorkItemsTooltip), culture);

    public static string RestoreWorkItemDialogTitle() => WITResources.Get(nameof (RestoreWorkItemDialogTitle));

    public static string RestoreWorkItemDialogTitle(CultureInfo culture) => WITResources.Get(nameof (RestoreWorkItemDialogTitle), culture);

    public static string RestoreWorkItemDeleteButtonText() => WITResources.Get(nameof (RestoreWorkItemDeleteButtonText));

    public static string RestoreWorkItemDeleteButtonText(CultureInfo culture) => WITResources.Get(nameof (RestoreWorkItemDeleteButtonText), culture);

    public static string RestoreWorkItemDialogConfirmationText() => WITResources.Get(nameof (RestoreWorkItemDialogConfirmationText));

    public static string RestoreWorkItemDialogConfirmationText(CultureInfo culture) => WITResources.Get(nameof (RestoreWorkItemDialogConfirmationText), culture);

    public static string FollowWorkItem() => WITResources.Get(nameof (FollowWorkItem));

    public static string FollowWorkItem(CultureInfo culture) => WITResources.Get(nameof (FollowWorkItem), culture);

    public static string FollowingWorkItem() => WITResources.Get(nameof (FollowingWorkItem));

    public static string FollowingWorkItem(CultureInfo culture) => WITResources.Get(nameof (FollowingWorkItem), culture);

    public static string UnfollowWorkItem() => WITResources.Get(nameof (UnfollowWorkItem));

    public static string UnfollowWorkItem(CultureInfo culture) => WITResources.Get(nameof (UnfollowWorkItem), culture);

    public static string CustomFollowWorkItem() => WITResources.Get(nameof (CustomFollowWorkItem));

    public static string CustomFollowWorkItem(CultureInfo culture) => WITResources.Get(nameof (CustomFollowWorkItem), culture);

    public static string CustomFollowDialogTitle() => WITResources.Get(nameof (CustomFollowDialogTitle));

    public static string CustomFollowDialogTitle(CultureInfo culture) => WITResources.Get(nameof (CustomFollowDialogTitle), culture);

    public static string CustomFollowDialogAllNotificationsRadio() => WITResources.Get(nameof (CustomFollowDialogAllNotificationsRadio));

    public static string CustomFollowDialogAllNotificationsRadio(CultureInfo culture) => WITResources.Get(nameof (CustomFollowDialogAllNotificationsRadio), culture);

    public static string CustomFollowDialogAllNotificationsRadioDescription() => WITResources.Get(nameof (CustomFollowDialogAllNotificationsRadioDescription));

    public static string CustomFollowDialogAllNotificationsRadioDescription(CultureInfo culture) => WITResources.Get(nameof (CustomFollowDialogAllNotificationsRadioDescription), culture);

    public static string CustomFollowDialogCustomNotificationRadio() => WITResources.Get(nameof (CustomFollowDialogCustomNotificationRadio));

    public static string CustomFollowDialogCustomNotificationRadio(CultureInfo culture) => WITResources.Get(nameof (CustomFollowDialogCustomNotificationRadio), culture);

    public static string CustomFollowDialogCustomNotificationRadioDescription() => WITResources.Get(nameof (CustomFollowDialogCustomNotificationRadioDescription));

    public static string CustomFollowDialogCustomNotificationRadioDescription(CultureInfo culture) => WITResources.Get(nameof (CustomFollowDialogCustomNotificationRadioDescription), culture);

    public static string CustomFollowDialogNoNotificationRadio() => WITResources.Get(nameof (CustomFollowDialogNoNotificationRadio));

    public static string CustomFollowDialogNoNotificationRadio(CultureInfo culture) => WITResources.Get(nameof (CustomFollowDialogNoNotificationRadio), culture);

    public static string CustomFollowDialogNoNotificationRadioDescription() => WITResources.Get(nameof (CustomFollowDialogNoNotificationRadioDescription));

    public static string CustomFollowDialogNoNotificationRadioDescription(CultureInfo culture) => WITResources.Get(nameof (CustomFollowDialogNoNotificationRadioDescription), culture);

    public static string CustomFollowDialogStateCheckbox() => WITResources.Get(nameof (CustomFollowDialogStateCheckbox));

    public static string CustomFollowDialogStateCheckbox(CultureInfo culture) => WITResources.Get(nameof (CustomFollowDialogStateCheckbox), culture);

    public static string CustomFollowDialogAssignedToCheckbox() => WITResources.Get(nameof (CustomFollowDialogAssignedToCheckbox));

    public static string CustomFollowDialogAssignedToCheckbox(CultureInfo culture) => WITResources.Get(nameof (CustomFollowDialogAssignedToCheckbox), culture);

    public static string CustomFollowDialogIterationPathCheckbox() => WITResources.Get(nameof (CustomFollowDialogIterationPathCheckbox));

    public static string CustomFollowDialogIterationPathCheckbox(CultureInfo culture) => WITResources.Get(nameof (CustomFollowDialogIterationPathCheckbox), culture);

    public static string WorkItemDiscussionAddComment() => WITResources.Get(nameof (WorkItemDiscussionAddComment));

    public static string WorkItemDiscussionAddComment(CultureInfo culture) => WITResources.Get(nameof (WorkItemDiscussionAddComment), culture);

    public static string WorkItemDiscussionLabel() => WITResources.Get(nameof (WorkItemDiscussionLabel));

    public static string WorkItemDiscussionLabel(CultureInfo culture) => WITResources.Get(nameof (WorkItemDiscussionLabel), culture);

    public static string BrowserRefreshRequired() => WITResources.Get(nameof (BrowserRefreshRequired));

    public static string BrowserRefreshRequired(CultureInfo culture) => WITResources.Get(nameof (BrowserRefreshRequired), culture);

    public static string AttachmentAsLinkType() => WITResources.Get(nameof (AttachmentAsLinkType));

    public static string AttachmentAsLinkType(CultureInfo culture) => WITResources.Get(nameof (AttachmentAsLinkType), culture);

    public static string WorkItemLogControlOtherLinkAdded() => WITResources.Get(nameof (WorkItemLogControlOtherLinkAdded));

    public static string WorkItemLogControlOtherLinkAdded(CultureInfo culture) => WITResources.Get(nameof (WorkItemLogControlOtherLinkAdded), culture);

    public static string WorkItemLogControlOtherLinkDeleted() => WITResources.Get(nameof (WorkItemLogControlOtherLinkDeleted));

    public static string WorkItemLogControlOtherLinkDeleted(CultureInfo culture) => WITResources.Get(nameof (WorkItemLogControlOtherLinkDeleted), culture);

    public static string WorkItemDestroyError(object arg0, object arg1) => WITResources.Format(nameof (WorkItemDestroyError), arg0, arg1);

    public static string WorkItemDestroyError(object arg0, object arg1, CultureInfo culture) => WITResources.Format(nameof (WorkItemDestroyError), culture, arg0, arg1);

    public static string WorkItemBulkDestroyError() => WITResources.Get(nameof (WorkItemBulkDestroyError));

    public static string WorkItemBulkDestroyError(CultureInfo culture) => WITResources.Get(nameof (WorkItemBulkDestroyError), culture);

    public static string WorkItemRestoreError(object arg0, object arg1) => WITResources.Format(nameof (WorkItemRestoreError), arg0, arg1);

    public static string WorkItemRestoreError(object arg0, object arg1, CultureInfo culture) => WITResources.Format(nameof (WorkItemRestoreError), culture, arg0, arg1);

    public static string WorkItemBulkRestoreError() => WITResources.Get(nameof (WorkItemBulkRestoreError));

    public static string WorkItemBulkRestoreError(CultureInfo culture) => WITResources.Get(nameof (WorkItemBulkRestoreError), culture);

    public static string WorkItemDeleteError(object arg0, object arg1) => WITResources.Format(nameof (WorkItemDeleteError), arg0, arg1);

    public static string WorkItemDeleteError(object arg0, object arg1, CultureInfo culture) => WITResources.Format(nameof (WorkItemDeleteError), culture, arg0, arg1);

    public static string WorkItemBulkDeleteError() => WITResources.Get(nameof (WorkItemBulkDeleteError));

    public static string WorkItemBulkDeleteError(CultureInfo culture) => WITResources.Get(nameof (WorkItemBulkDeleteError), culture);

    public static string WorkItemLogControlLinkRestored() => WITResources.Get(nameof (WorkItemLogControlLinkRestored));

    public static string WorkItemLogControlLinkRestored(CultureInfo culture) => WITResources.Get(nameof (WorkItemLogControlLinkRestored), culture);

    public static string WorkItemArtifactLastUpdatedLabel(object arg0) => WITResources.Format(nameof (WorkItemArtifactLastUpdatedLabel), arg0);

    public static string WorkItemArtifactLastUpdatedLabel(object arg0, CultureInfo culture) => WITResources.Format(nameof (WorkItemArtifactLastUpdatedLabel), culture, arg0);

    public static string WorkItemArtifactLastUpdatedByLabel(object arg0, object arg1) => WITResources.Format(nameof (WorkItemArtifactLastUpdatedByLabel), arg0, arg1);

    public static string WorkItemArtifactLastUpdatedByLabel(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (WorkItemArtifactLastUpdatedByLabel), culture, arg0, arg1);
    }

    public static string WorkItemCountDisplayStringPlural(object arg0) => WITResources.Format(nameof (WorkItemCountDisplayStringPlural), arg0);

    public static string WorkItemCountDisplayStringPlural(object arg0, CultureInfo culture) => WITResources.Format(nameof (WorkItemCountDisplayStringPlural), culture, arg0);

    public static string WorkItemCountDisplayStringSingular(object arg0) => WITResources.Format(nameof (WorkItemCountDisplayStringSingular), arg0);

    public static string WorkItemCountDisplayStringSingular(object arg0, CultureInfo culture) => WITResources.Format(nameof (WorkItemCountDisplayStringSingular), culture, arg0);

    public static string WorkItemIdDisplayStringSingular(object arg0) => WITResources.Format(nameof (WorkItemIdDisplayStringSingular), arg0);

    public static string WorkItemIdDisplayStringSingular(object arg0, CultureInfo culture) => WITResources.Format(nameof (WorkItemIdDisplayStringSingular), culture, arg0);

    public static string AssignedToPrefix() => WITResources.Get(nameof (AssignedToPrefix));

    public static string AssignedToPrefix(CultureInfo culture) => WITResources.Get(nameof (AssignedToPrefix), culture);

    public static string DeleteOrRestoreConfirmationMessage() => WITResources.Get(nameof (DeleteOrRestoreConfirmationMessage));

    public static string DeleteOrRestoreConfirmationMessage(CultureInfo culture) => WITResources.Get(nameof (DeleteOrRestoreConfirmationMessage), culture);

    public static string SelectedWorkItemsLabel() => WITResources.Get(nameof (SelectedWorkItemsLabel));

    public static string SelectedWorkItemsLabel(CultureInfo culture) => WITResources.Get(nameof (SelectedWorkItemsLabel), culture);

    public static string BulkEditTagsAdd() => WITResources.Get(nameof (BulkEditTagsAdd));

    public static string BulkEditTagsAdd(CultureInfo culture) => WITResources.Get(nameof (BulkEditTagsAdd), culture);

    public static string BulkEditTagsRemove() => WITResources.Get(nameof (BulkEditTagsRemove));

    public static string BulkEditTagsRemove(CultureInfo culture) => WITResources.Get(nameof (BulkEditTagsRemove), culture);

    public static string RecyclebinCleanupMessage(object arg0) => WITResources.Format(nameof (RecyclebinCleanupMessage), arg0);

    public static string RecyclebinCleanupMessage(object arg0, CultureInfo culture) => WITResources.Format(nameof (RecyclebinCleanupMessage), culture, arg0);

    public static string MoveWorkItemDiscussionLabel() => WITResources.Get(nameof (MoveWorkItemDiscussionLabel));

    public static string MoveWorkItemDiscussionLabel(CultureInfo culture) => WITResources.Get(nameof (MoveWorkItemDiscussionLabel), culture);

    public static string MoveWorkItemTitle() => WITResources.Get(nameof (MoveWorkItemTitle));

    public static string MoveWorkItemTitle(CultureInfo culture) => WITResources.Get(nameof (MoveWorkItemTitle), culture);

    public static string MoveWorkItem() => WITResources.Get(nameof (MoveWorkItem));

    public static string MoveWorkItem(CultureInfo culture) => WITResources.Get(nameof (MoveWorkItem), culture);

    public static string MoveWorkItemDialogTitle() => WITResources.Get(nameof (MoveWorkItemDialogTitle));

    public static string MoveWorkItemDialogTitle(CultureInfo culture) => WITResources.Get(nameof (MoveWorkItemDialogTitle), culture);

    public static string MoveWorkItemMessage() => WITResources.Get(nameof (MoveWorkItemMessage));

    public static string MoveWorkItemMessage(CultureInfo culture) => WITResources.Get(nameof (MoveWorkItemMessage), culture);

    public static string DeleteWorkItemDialogConfirmationTextLearnMoreLink() => WITResources.Get(nameof (DeleteWorkItemDialogConfirmationTextLearnMoreLink));

    public static string DeleteWorkItemDialogConfirmationTextLearnMoreLink(CultureInfo culture) => WITResources.Get(nameof (DeleteWorkItemDialogConfirmationTextLearnMoreLink), culture);

    public static string DeleteWorkItemDialogConfirmationText() => WITResources.Get(nameof (DeleteWorkItemDialogConfirmationText));

    public static string DeleteWorkItemDialogConfirmationText(CultureInfo culture) => WITResources.Get(nameof (DeleteWorkItemDialogConfirmationText), culture);

    public static string WorkItemDiscussionMentionLinkComment(
      object arg0,
      object arg1,
      object arg2)
    {
      return WITResources.Format(nameof (WorkItemDiscussionMentionLinkComment), arg0, arg1, arg2);
    }

    public static string WorkItemDiscussionMentionLinkComment(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (WorkItemDiscussionMentionLinkComment), culture, arg0, arg1, arg2);
    }

    public static string WorkItemHtmlFieldMentionLinkComment(object arg0, object arg1, object arg2) => WITResources.Format(nameof (WorkItemHtmlFieldMentionLinkComment), arg0, arg1, arg2);

    public static string WorkItemHtmlFieldMentionLinkComment(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (WorkItemHtmlFieldMentionLinkComment), culture, arg0, arg1, arg2);
    }

    public static string MoveWorkItemDestinationLabel() => WITResources.Get(nameof (MoveWorkItemDestinationLabel));

    public static string MoveWorkItemDestinationLabel(CultureInfo culture) => WITResources.Get(nameof (MoveWorkItemDestinationLabel), culture);

    public static string MoveWorkItemPermissionError() => WITResources.Get(nameof (MoveWorkItemPermissionError));

    public static string MoveWorkItemPermissionError(CultureInfo culture) => WITResources.Get(nameof (MoveWorkItemPermissionError), culture);

    public static string WorkItemDelete_NoPermission() => WITResources.Get(nameof (WorkItemDelete_NoPermission));

    public static string WorkItemDelete_NoPermission(CultureInfo culture) => WITResources.Get(nameof (WorkItemDelete_NoPermission), culture);

    public static string WorkItemLogControlProjectChange(object arg0, object arg1) => WITResources.Format(nameof (WorkItemLogControlProjectChange), arg0, arg1);

    public static string WorkItemLogControlProjectChange(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (WorkItemLogControlProjectChange), culture, arg0, arg1);
    }

    public static string MoveWorkItemProjectWatermark() => WITResources.Get(nameof (MoveWorkItemProjectWatermark));

    public static string MoveWorkItemProjectWatermark(CultureInfo culture) => WITResources.Get(nameof (MoveWorkItemProjectWatermark), culture);

    public static string ChangeTypeLabel() => WITResources.Get(nameof (ChangeTypeLabel));

    public static string ChangeTypeLabel(CultureInfo culture) => WITResources.Get(nameof (ChangeTypeLabel), culture);

    public static string ChangeTypeMessage() => WITResources.Get(nameof (ChangeTypeMessage));

    public static string ChangeTypeMessage(CultureInfo culture) => WITResources.Get(nameof (ChangeTypeMessage), culture);

    public static string MoveWorkItemTypeError(object arg0, object arg1) => WITResources.Format(nameof (MoveWorkItemTypeError), arg0, arg1);

    public static string MoveWorkItemTypeError(object arg0, object arg1, CultureInfo culture) => WITResources.Format(nameof (MoveWorkItemTypeError), culture, arg0, arg1);

    public static string ChangeType() => WITResources.Get(nameof (ChangeType));

    public static string ChangeType(CultureInfo culture) => WITResources.Get(nameof (ChangeType), culture);

    public static string ChangeTypeTooltipDisabled() => WITResources.Get(nameof (ChangeTypeTooltipDisabled));

    public static string ChangeTypeTooltipDisabled(CultureInfo culture) => WITResources.Get(nameof (ChangeTypeTooltipDisabled), culture);

    public static string ChangeTypeDialogTitle() => WITResources.Get(nameof (ChangeTypeDialogTitle));

    public static string ChangeTypeDialogTitle(CultureInfo culture) => WITResources.Get(nameof (ChangeTypeDialogTitle), culture);

    public static string WorkItemLogControlWorkItemTypeChange(object arg0, object arg1) => WITResources.Format(nameof (WorkItemLogControlWorkItemTypeChange), arg0, arg1);

    public static string WorkItemLogControlWorkItemTypeChange(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (WorkItemLogControlWorkItemTypeChange), culture, arg0, arg1);
    }

    public static string MoveWorkItemChangeTypeWatermark() => WITResources.Get(nameof (MoveWorkItemChangeTypeWatermark));

    public static string MoveWorkItemChangeTypeWatermark(CultureInfo culture) => WITResources.Get(nameof (MoveWorkItemChangeTypeWatermark), culture);

    public static string ProjectNameRequired() => WITResources.Get(nameof (ProjectNameRequired));

    public static string ProjectNameRequired(CultureInfo culture) => WITResources.Get(nameof (ProjectNameRequired), culture);

    public static string WorkItemTypeDoesNotExist(object arg0) => WITResources.Format(nameof (WorkItemTypeDoesNotExist), arg0);

    public static string WorkItemTypeDoesNotExist(object arg0, CultureInfo culture) => WITResources.Format(nameof (WorkItemTypeDoesNotExist), culture, arg0);

    public static string WorkItemTypeNameRequired() => WITResources.Get(nameof (WorkItemTypeNameRequired));

    public static string WorkItemTypeNameRequired(CultureInfo culture) => WITResources.Get(nameof (WorkItemTypeNameRequired), culture);

    public static string MoveWorkItemDialogCurrentProjLabel() => WITResources.Get(nameof (MoveWorkItemDialogCurrentProjLabel));

    public static string MoveWorkItemDialogCurrentProjLabel(CultureInfo culture) => WITResources.Get(nameof (MoveWorkItemDialogCurrentProjLabel), culture);

    public static string MoveTargetWorkItemBlockedCase(object arg0, object arg1) => WITResources.Format(nameof (MoveTargetWorkItemBlockedCase), arg0, arg1);

    public static string MoveTargetWorkItemBlockedCase(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (MoveTargetWorkItemBlockedCase), culture, arg0, arg1);
    }

    public static string MoveSourceWorkItemBlockedCase(object arg0, object arg1) => WITResources.Format(nameof (MoveSourceWorkItemBlockedCase), arg0, arg1);

    public static string MoveSourceWorkItemBlockedCase(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (MoveSourceWorkItemBlockedCase), culture, arg0, arg1);
    }

    public static string ChangeTypeBlockedCase(object arg0) => WITResources.Format(nameof (ChangeTypeBlockedCase), arg0);

    public static string ChangeTypeBlockedCase(object arg0, CultureInfo culture) => WITResources.Format(nameof (ChangeTypeBlockedCase), culture, arg0);

    public static string MoveWorkItemAreaPathLabel() => WITResources.Get(nameof (MoveWorkItemAreaPathLabel));

    public static string MoveWorkItemAreaPathLabel(CultureInfo culture) => WITResources.Get(nameof (MoveWorkItemAreaPathLabel), culture);

    public static string MoveWorkItemAreaPathWatermark() => WITResources.Get(nameof (MoveWorkItemAreaPathWatermark));

    public static string MoveWorkItemAreaPathWatermark(CultureInfo culture) => WITResources.Get(nameof (MoveWorkItemAreaPathWatermark), culture);

    public static string MoveWorkItemIterationPathLabel() => WITResources.Get(nameof (MoveWorkItemIterationPathLabel));

    public static string MoveWorkItemIterationPathLabel(CultureInfo culture) => WITResources.Get(nameof (MoveWorkItemIterationPathLabel), culture);

    public static string MoveWorkItemIterationPathWatermark() => WITResources.Get(nameof (MoveWorkItemIterationPathWatermark));

    public static string MoveWorkItemIterationPathWatermark(CultureInfo culture) => WITResources.Get(nameof (MoveWorkItemIterationPathWatermark), culture);

    public static string MoveWorkItemKeepType() => WITResources.Get(nameof (MoveWorkItemKeepType));

    public static string MoveWorkItemKeepType(CultureInfo culture) => WITResources.Get(nameof (MoveWorkItemKeepType), culture);

    public static string GenericHistoryRetrievalError() => WITResources.Get(nameof (GenericHistoryRetrievalError));

    public static string GenericHistoryRetrievalError(CultureInfo culture) => WITResources.Get(nameof (GenericHistoryRetrievalError), culture);

    public static string MoveWorkItemPathInvalid() => WITResources.Get(nameof (MoveWorkItemPathInvalid));

    public static string MoveWorkItemPathInvalid(CultureInfo culture) => WITResources.Get(nameof (MoveWorkItemPathInvalid), culture);

    public static string WorkItemMoveInProgress() => WITResources.Get(nameof (WorkItemMoveInProgress));

    public static string WorkItemMoveInProgress(CultureInfo culture) => WITResources.Get(nameof (WorkItemMoveInProgress), culture);

    public static string WorkItemTypeChangeInProgress() => WITResources.Get(nameof (WorkItemTypeChangeInProgress));

    public static string WorkItemTypeChangeInProgress(CultureInfo culture) => WITResources.Get(nameof (WorkItemTypeChangeInProgress), culture);

    public static string MoveWorkItemBulkTypeError(object arg0, object arg1) => WITResources.Format(nameof (MoveWorkItemBulkTypeError), arg0, arg1);

    public static string MoveWorkItemBulkTypeError(object arg0, object arg1, CultureInfo culture) => WITResources.Format(nameof (MoveWorkItemBulkTypeError), culture, arg0, arg1);

    public static string ChangeTypeDiscussionLabel() => WITResources.Get(nameof (ChangeTypeDiscussionLabel));

    public static string ChangeTypeDiscussionLabel(CultureInfo culture) => WITResources.Get(nameof (ChangeTypeDiscussionLabel), culture);

    public static string WorkItemMoveInProgressWithErrors() => WITResources.Get(nameof (WorkItemMoveInProgressWithErrors));

    public static string WorkItemMoveInProgressWithErrors(CultureInfo culture) => WITResources.Get(nameof (WorkItemMoveInProgressWithErrors), culture);

    public static string WorkItemTypeChangeInProgressWithErrors() => WITResources.Get(nameof (WorkItemTypeChangeInProgressWithErrors));

    public static string WorkItemTypeChangeInProgressWithErrors(CultureInfo culture) => WITResources.Get(nameof (WorkItemTypeChangeInProgressWithErrors), culture);

    public static string ReasonLabel() => WITResources.Get(nameof (ReasonLabel));

    public static string ReasonLabel(CultureInfo culture) => WITResources.Get(nameof (ReasonLabel), culture);

    public static string WorkItemCommentsRetrievalError() => WITResources.Get(nameof (WorkItemCommentsRetrievalError));

    public static string WorkItemCommentsRetrievalError(CultureInfo culture) => WITResources.Get(nameof (WorkItemCommentsRetrievalError), culture);

    public static string SaveAndCloseWorkItemText() => WITResources.Get(nameof (SaveAndCloseWorkItemText));

    public static string SaveAndCloseWorkItemText(CultureInfo culture) => WITResources.Get(nameof (SaveAndCloseWorkItemText), culture);

    public static string SuccessfulUnfollowFromEmail() => WITResources.Get(nameof (SuccessfulUnfollowFromEmail));

    public static string SuccessfulUnfollowFromEmail(CultureInfo culture) => WITResources.Get(nameof (SuccessfulUnfollowFromEmail), culture);

    public static string FailedUnfollowFromEmail() => WITResources.Get(nameof (FailedUnfollowFromEmail));

    public static string FailedUnfollowFromEmail(CultureInfo culture) => WITResources.Get(nameof (FailedUnfollowFromEmail), culture);

    public static string DevelopmentControlCreateBranchTitle() => WITResources.Get(nameof (DevelopmentControlCreateBranchTitle));

    public static string DevelopmentControlCreateBranchTitle(CultureInfo culture) => WITResources.Get(nameof (DevelopmentControlCreateBranchTitle), culture);

    public static string DevelopmentControlWorkHasntStartedMessage() => WITResources.Get(nameof (DevelopmentControlWorkHasntStartedMessage));

    public static string DevelopmentControlWorkHasntStartedMessage(CultureInfo culture) => WITResources.Get(nameof (DevelopmentControlWorkHasntStartedMessage), culture);

    public static string LinksControlAddLinkToExistingItem() => WITResources.Get(nameof (LinksControlAddLinkToExistingItem));

    public static string LinksControlAddLinkToExistingItem(CultureInfo culture) => WITResources.Get(nameof (LinksControlAddLinkToExistingItem), culture);

    public static string LinksControlCreateNewLink() => WITResources.Get(nameof (LinksControlCreateNewLink));

    public static string LinksControlCreateNewLink(CultureInfo culture) => WITResources.Get(nameof (LinksControlCreateNewLink), culture);

    public static string LinksControlAddLinkDisplayText() => WITResources.Get(nameof (LinksControlAddLinkDisplayText));

    public static string LinksControlAddLinkDisplayText(CultureInfo culture) => WITResources.Get(nameof (LinksControlAddLinkDisplayText), culture);

    public static string WorkItemTemplates_ContextMenuItem_Text() => WITResources.Get(nameof (WorkItemTemplates_ContextMenuItem_Text));

    public static string WorkItemTemplates_ContextMenuItem_Text(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplates_ContextMenuItem_Text), culture);

    public static string WorkItemTemplates_CaptureTemplateMenuItem_Text() => WITResources.Get(nameof (WorkItemTemplates_CaptureTemplateMenuItem_Text));

    public static string WorkItemTemplates_CaptureTemplateMenuItem_Text(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplates_CaptureTemplateMenuItem_Text), culture);

    public static string WorkItemTemplates_CaptureTemplateDialogTitle() => WITResources.Get(nameof (WorkItemTemplates_CaptureTemplateDialogTitle));

    public static string WorkItemTemplates_CaptureTemplateDialogTitle(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplates_CaptureTemplateDialogTitle), culture);

    public static string WorkItemTemplates_NoTemplatesDisabledMenuItemTitle() => WITResources.Get(nameof (WorkItemTemplates_NoTemplatesDisabledMenuItemTitle));

    public static string WorkItemTemplates_NoTemplatesDisabledMenuItemTitle(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplates_NoTemplatesDisabledMenuItemTitle), culture);

    public static string WorkItemTemplates_ManageMenuItem_Text() => WITResources.Get(nameof (WorkItemTemplates_ManageMenuItem_Text));

    public static string WorkItemTemplates_ManageMenuItem_Text(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplates_ManageMenuItem_Text), culture);

    public static string WorkItemTemplateDialog_AddComment() => WITResources.Get(nameof (WorkItemTemplateDialog_AddComment));

    public static string WorkItemTemplateDialog_AddComment(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplateDialog_AddComment), culture);

    public static string WorkItemTemplateDialog_Name() => WITResources.Get(nameof (WorkItemTemplateDialog_Name));

    public static string WorkItemTemplateDialog_Name(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplateDialog_Name), culture);

    public static string WorkItemTemplateDialog_Description() => WITResources.Get(nameof (WorkItemTemplateDialog_Description));

    public static string WorkItemTemplateDialog_Description(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplateDialog_Description), culture);

    public static string WorkItemTemplateDialog_RemoveUnmodified() => WITResources.Get(nameof (WorkItemTemplateDialog_RemoveUnmodified));

    public static string WorkItemTemplateDialog_RemoveUnmodified(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplateDialog_RemoveUnmodified), culture);

    public static string WorkItemTemplateDialog_ConfirmClosePrompt() => WITResources.Get(nameof (WorkItemTemplateDialog_ConfirmClosePrompt));

    public static string WorkItemTemplateDialog_ConfirmClosePrompt(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplateDialog_ConfirmClosePrompt), culture);

    public static string MultiFieldAddNewField() => WITResources.Get(nameof (MultiFieldAddNewField));

    public static string MultiFieldAddNewField(CultureInfo culture) => WITResources.Get(nameof (MultiFieldAddNewField), culture);

    public static string MultiFieldInsertNewFieldTooltip() => WITResources.Get(nameof (MultiFieldInsertNewFieldTooltip));

    public static string MultiFieldInsertNewFieldTooltip(CultureInfo culture) => WITResources.Get(nameof (MultiFieldInsertNewFieldTooltip), culture);

    public static string MultiFieldRemoveFieldTooltip() => WITResources.Get(nameof (MultiFieldRemoveFieldTooltip));

    public static string MultiFieldRemoveFieldTooltip(CultureInfo culture) => WITResources.Get(nameof (MultiFieldRemoveFieldTooltip), culture);

    public static string ContributionNotFound() => WITResources.Get(nameof (ContributionNotFound));

    public static string ContributionNotFound(CultureInfo culture) => WITResources.Get(nameof (ContributionNotFound), culture);

    public static string LinksControlAddNewItemDisabledTooltip() => WITResources.Get(nameof (LinksControlAddNewItemDisabledTooltip));

    public static string LinksControlAddNewItemDisabledTooltip(CultureInfo culture) => WITResources.Get(nameof (LinksControlAddNewItemDisabledTooltip), culture);

    public static string LinksControlAddLinkDisabledTooltip() => WITResources.Get(nameof (LinksControlAddLinkDisabledTooltip));

    public static string LinksControlAddLinkDisabledTooltip(CultureInfo culture) => WITResources.Get(nameof (LinksControlAddLinkDisabledTooltip), culture);

    public static string NewFeatureViewBoardLinkTitle() => WITResources.Get(nameof (NewFeatureViewBoardLinkTitle));

    public static string NewFeatureViewBoardLinkTitle(CultureInfo culture) => WITResources.Get(nameof (NewFeatureViewBoardLinkTitle), culture);

    public static string NewFeatureCustomizeProcessLinkTitle() => WITResources.Get(nameof (NewFeatureCustomizeProcessLinkTitle));

    public static string NewFeatureCustomizeProcessLinkTitle(CultureInfo culture) => WITResources.Get(nameof (NewFeatureCustomizeProcessLinkTitle), culture);

    public static string VisualizeYourWorkText() => WITResources.Get(nameof (VisualizeYourWorkText));

    public static string VisualizeYourWorkText(CultureInfo culture) => WITResources.Get(nameof (VisualizeYourWorkText), culture);

    public static string TrackChangesText() => WITResources.Get(nameof (TrackChangesText));

    public static string TrackChangesText(CultureInfo culture) => WITResources.Get(nameof (TrackChangesText), culture);

    public static string CustomizeWorkItemsText() => WITResources.Get(nameof (CustomizeWorkItemsText));

    public static string CustomizeWorkItemsText(CultureInfo culture) => WITResources.Get(nameof (CustomizeWorkItemsText), culture);

    public static string CustomizeWorkItemsNewFeatureTitle() => WITResources.Get(nameof (CustomizeWorkItemsNewFeatureTitle));

    public static string CustomizeWorkItemsNewFeatureTitle(CultureInfo culture) => WITResources.Get(nameof (CustomizeWorkItemsNewFeatureTitle), culture);

    public static string NewFeatureLearnMoreLinkTitle() => WITResources.Get(nameof (NewFeatureLearnMoreLinkTitle));

    public static string NewFeatureLearnMoreLinkTitle(CultureInfo culture) => WITResources.Get(nameof (NewFeatureLearnMoreLinkTitle), culture);

    public static string VisualizeYourWorkNewFeatureTitle() => WITResources.Get(nameof (VisualizeYourWorkNewFeatureTitle));

    public static string VisualizeYourWorkNewFeatureTitle(CultureInfo culture) => WITResources.Get(nameof (VisualizeYourWorkNewFeatureTitle), culture);

    public static string TrackChangesNewFeatureTitle() => WITResources.Get(nameof (TrackChangesNewFeatureTitle));

    public static string TrackChangesNewFeatureTitle(CultureInfo culture) => WITResources.Get(nameof (TrackChangesNewFeatureTitle), culture);

    public static string FilesUploadedExceedMaxMb(object arg0, object arg1) => WITResources.Format(nameof (FilesUploadedExceedMaxMb), arg0, arg1);

    public static string FilesUploadedExceedMaxMb(object arg0, object arg1, CultureInfo culture) => WITResources.Format(nameof (FilesUploadedExceedMaxMb), culture, arg0, arg1);

    public static string DeleteAttachmentsDialogConfirmationText() => WITResources.Get(nameof (DeleteAttachmentsDialogConfirmationText));

    public static string DeleteAttachmentsDialogConfirmationText(CultureInfo culture) => WITResources.Get(nameof (DeleteAttachmentsDialogConfirmationText), culture);

    public static string DeleteAttachmentsDialogTitle() => WITResources.Get(nameof (DeleteAttachmentsDialogTitle));

    public static string DeleteAttachmentsDialogTitle(CultureInfo culture) => WITResources.Get(nameof (DeleteAttachmentsDialogTitle), culture);

    public static string AttachmentsOversizedDialogTitle() => WITResources.Get(nameof (AttachmentsOversizedDialogTitle));

    public static string AttachmentsOversizedDialogTitle(CultureInfo culture) => WITResources.Get(nameof (AttachmentsOversizedDialogTitle), culture);

    public static string DeleteAttachments() => WITResources.Get(nameof (DeleteAttachments));

    public static string DeleteAttachments(CultureInfo culture) => WITResources.Get(nameof (DeleteAttachments), culture);

    public static string DownloadAttachment() => WITResources.Get(nameof (DownloadAttachment));

    public static string DownloadAttachment(CultureInfo culture) => WITResources.Get(nameof (DownloadAttachment), culture);

    public static string DownloadAttachments() => WITResources.Get(nameof (DownloadAttachments));

    public static string DownloadAttachments(CultureInfo culture) => WITResources.Get(nameof (DownloadAttachments), culture);

    public static string EditComment() => WITResources.Get(nameof (EditComment));

    public static string EditComment(CultureInfo culture) => WITResources.Get(nameof (EditComment), culture);

    public static string EditComments() => WITResources.Get(nameof (EditComments));

    public static string EditComments(CultureInfo culture) => WITResources.Get(nameof (EditComments), culture);

    public static string EditCommentDialogLabel() => WITResources.Get(nameof (EditCommentDialogLabel));

    public static string EditCommentDialogLabel(CultureInfo culture) => WITResources.Get(nameof (EditCommentDialogLabel), culture);

    public static string FileThrottleDialogTitle() => WITResources.Get(nameof (FileThrottleDialogTitle));

    public static string FileThrottleDialogTitle(CultureInfo culture) => WITResources.Get(nameof (FileThrottleDialogTitle), culture);

    public static string FileThrottleMessage(object arg0) => WITResources.Format(nameof (FileThrottleMessage), arg0);

    public static string FileThrottleMessage(object arg0, CultureInfo culture) => WITResources.Format(nameof (FileThrottleMessage), culture, arg0);

    public static string HistoryControlAgoOlder() => WITResources.Get(nameof (HistoryControlAgoOlder));

    public static string HistoryControlAgoOlder(CultureInfo culture) => WITResources.Get(nameof (HistoryControlAgoOlder), culture);

    public static string HistoryControlAgoLastThirtyDays() => WITResources.Get(nameof (HistoryControlAgoLastThirtyDays));

    public static string HistoryControlAgoLastThirtyDays(CultureInfo culture) => WITResources.Get(nameof (HistoryControlAgoLastThirtyDays), culture);

    public static string HistoryControlAgoLastSevenDays() => WITResources.Get(nameof (HistoryControlAgoLastSevenDays));

    public static string HistoryControlAgoLastSevenDays(CultureInfo culture) => WITResources.Get(nameof (HistoryControlAgoLastSevenDays), culture);

    public static string HistoryControlAgoToday() => WITResources.Get(nameof (HistoryControlAgoToday));

    public static string HistoryControlAgoToday(CultureInfo culture) => WITResources.Get(nameof (HistoryControlAgoToday), culture);

    public static string HistoryControlAgoYesterday() => WITResources.Get(nameof (HistoryControlAgoYesterday));

    public static string HistoryControlAgoYesterday(CultureInfo culture) => WITResources.Get(nameof (HistoryControlAgoYesterday), culture);

    public static string HistoryControlLinksTitle() => WITResources.Get(nameof (HistoryControlLinksTitle));

    public static string HistoryControlLinksTitle(CultureInfo culture) => WITResources.Get(nameof (HistoryControlLinksTitle), culture);

    public static string HistoryControlAssignmentAdornmentText(object arg0) => WITResources.Format(nameof (HistoryControlAssignmentAdornmentText), arg0);

    public static string HistoryControlAssignmentAdornmentText(object arg0, CultureInfo culture) => WITResources.Format(nameof (HistoryControlAssignmentAdornmentText), culture, arg0);

    public static string HistoryControlAssignmentDefaultAdornmentText() => WITResources.Get(nameof (HistoryControlAssignmentDefaultAdornmentText));

    public static string HistoryControlAssignmentDefaultAdornmentText(CultureInfo culture) => WITResources.Get(nameof (HistoryControlAssignmentDefaultAdornmentText), culture);

    public static string HistoryControlStateAdornmentText(object arg0) => WITResources.Format(nameof (HistoryControlStateAdornmentText), arg0);

    public static string HistoryControlStateAdornmentText(object arg0, CultureInfo culture) => WITResources.Format(nameof (HistoryControlStateAdornmentText), culture, arg0);

    public static string HistoryControlCommentAddedAdornmentText() => WITResources.Get(nameof (HistoryControlCommentAddedAdornmentText));

    public static string HistoryControlCommentAddedAdornmentText(CultureInfo culture) => WITResources.Get(nameof (HistoryControlCommentAddedAdornmentText), culture);

    public static string HistoryControlWorkItemDeletedAdornmentText() => WITResources.Get(nameof (HistoryControlWorkItemDeletedAdornmentText));

    public static string HistoryControlWorkItemDeletedAdornmentText(CultureInfo culture) => WITResources.Get(nameof (HistoryControlWorkItemDeletedAdornmentText), culture);

    public static string HistoryControlWorkItemTagAddedAdornmentText() => WITResources.Get(nameof (HistoryControlWorkItemTagAddedAdornmentText));

    public static string HistoryControlWorkItemTagAddedAdornmentText(CultureInfo culture) => WITResources.Get(nameof (HistoryControlWorkItemTagAddedAdornmentText), culture);

    public static string HistoryControlWorkItemTagRemovedAdornmentText() => WITResources.Get(nameof (HistoryControlWorkItemTagRemovedAdornmentText));

    public static string HistoryControlWorkItemTagRemovedAdornmentText(CultureInfo culture) => WITResources.Get(nameof (HistoryControlWorkItemTagRemovedAdornmentText), culture);

    public static string HistoryControlWorkItemRestoredAdornmentText() => WITResources.Get(nameof (HistoryControlWorkItemRestoredAdornmentText));

    public static string HistoryControlWorkItemRestoredAdornmentText(CultureInfo culture) => WITResources.Get(nameof (HistoryControlWorkItemRestoredAdornmentText), culture);

    public static string HistoryControlProjectAdornmentText(object arg0) => WITResources.Format(nameof (HistoryControlProjectAdornmentText), arg0);

    public static string HistoryControlProjectAdornmentText(object arg0, CultureInfo culture) => WITResources.Format(nameof (HistoryControlProjectAdornmentText), culture, arg0);

    public static string HistoryControlSingleAttachmentAdornmentText(object arg0) => WITResources.Format(nameof (HistoryControlSingleAttachmentAdornmentText), arg0);

    public static string HistoryControlSingleAttachmentAdornmentText(
      object arg0,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (HistoryControlSingleAttachmentAdornmentText), culture, arg0);
    }

    public static string HistoryControlMultipleAttachmentsAdornmentText() => WITResources.Get(nameof (HistoryControlMultipleAttachmentsAdornmentText));

    public static string HistoryControlMultipleAttachmentsAdornmentText(CultureInfo culture) => WITResources.Get(nameof (HistoryControlMultipleAttachmentsAdornmentText), culture);

    public static string HistoryControlSingleLinkAdornmentText(object arg0) => WITResources.Format(nameof (HistoryControlSingleLinkAdornmentText), arg0);

    public static string HistoryControlSingleLinkAdornmentText(object arg0, CultureInfo culture) => WITResources.Format(nameof (HistoryControlSingleLinkAdornmentText), culture, arg0);

    public static string HistoryControlNewValueText() => WITResources.Get(nameof (HistoryControlNewValueText));

    public static string HistoryControlNewValueText(CultureInfo culture) => WITResources.Get(nameof (HistoryControlNewValueText), culture);

    public static string HistoryControlOldValueText() => WITResources.Get(nameof (HistoryControlOldValueText));

    public static string HistoryControlOldValueText(CultureInfo culture) => WITResources.Get(nameof (HistoryControlOldValueText), culture);

    public static string HistoryControlAttachmentAddedText() => WITResources.Get(nameof (HistoryControlAttachmentAddedText));

    public static string HistoryControlAttachmentAddedText(CultureInfo culture) => WITResources.Get(nameof (HistoryControlAttachmentAddedText), culture);

    public static string HistoryControlAttachmentDeletedText() => WITResources.Get(nameof (HistoryControlAttachmentDeletedText));

    public static string HistoryControlAttachmentDeletedText(CultureInfo culture) => WITResources.Get(nameof (HistoryControlAttachmentDeletedText), culture);

    public static string HistoryControlLinkAddedText() => WITResources.Get(nameof (HistoryControlLinkAddedText));

    public static string HistoryControlLinkAddedText(CultureInfo culture) => WITResources.Get(nameof (HistoryControlLinkAddedText), culture);

    public static string HistoryControlLinkDeletedText() => WITResources.Get(nameof (HistoryControlLinkDeletedText));

    public static string HistoryControlLinkDeletedText(CultureInfo culture) => WITResources.Get(nameof (HistoryControlLinkDeletedText), culture);

    public static string WorkItemLinkText() => WITResources.Get(nameof (WorkItemLinkText));

    public static string WorkItemLinkText(CultureInfo culture) => WITResources.Get(nameof (WorkItemLinkText), culture);

    public static string ExternalLinkText() => WITResources.Get(nameof (ExternalLinkText));

    public static string ExternalLinkText(CultureInfo culture) => WITResources.Get(nameof (ExternalLinkText), culture);

    public static string HyperLinkText() => WITResources.Get(nameof (HyperLinkText));

    public static string HyperLinkText(CultureInfo culture) => WITResources.Get(nameof (HyperLinkText), culture);

    public static string LinkText() => WITResources.Get(nameof (LinkText));

    public static string LinkText(CultureInfo culture) => WITResources.Get(nameof (LinkText), culture);

    public static string HistoryControlMultipleLinksAdornmentText() => WITResources.Get(nameof (HistoryControlMultipleLinksAdornmentText));

    public static string HistoryControlMultipleLinksAdornmentText(CultureInfo culture) => WITResources.Get(nameof (HistoryControlMultipleLinksAdornmentText), culture);

    public static string BulkUnfollowWorkItems() => WITResources.Get(nameof (BulkUnfollowWorkItems));

    public static string BulkUnfollowWorkItems(CultureInfo culture) => WITResources.Get(nameof (BulkUnfollowWorkItems), culture);

    public static string BulkUnfollowsFailed() => WITResources.Get(nameof (BulkUnfollowsFailed));

    public static string BulkUnfollowsFailed(CultureInfo culture) => WITResources.Get(nameof (BulkUnfollowsFailed), culture);

    public static string AttachmentUploadFailDialogTitle() => WITResources.Get(nameof (AttachmentUploadFailDialogTitle));

    public static string AttachmentUploadFailDialogTitle(CultureInfo culture) => WITResources.Get(nameof (AttachmentUploadFailDialogTitle), culture);

    public static string AttachmentUploadFailMessage(object arg0) => WITResources.Format(nameof (AttachmentUploadFailMessage), arg0);

    public static string AttachmentUploadFailMessage(object arg0, CultureInfo culture) => WITResources.Format(nameof (AttachmentUploadFailMessage), culture, arg0);

    public static string AttachmentUploadFailPermissionsMessage() => WITResources.Get(nameof (AttachmentUploadFailPermissionsMessage));

    public static string AttachmentUploadFailPermissionsMessage(CultureInfo culture) => WITResources.Get(nameof (AttachmentUploadFailPermissionsMessage), culture);

    public static string AttachmentUploadCancelledMessage() => WITResources.Get(nameof (AttachmentUploadCancelledMessage));

    public static string AttachmentUploadCancelledMessage(CultureInfo culture) => WITResources.Get(nameof (AttachmentUploadCancelledMessage), culture);

    public static string AttachmentDropMessageHeader() => WITResources.Get(nameof (AttachmentDropMessageHeader));

    public static string AttachmentDropMessageHeader(CultureInfo culture) => WITResources.Get(nameof (AttachmentDropMessageHeader), culture);

    public static string AttachmentDropMessage() => WITResources.Get(nameof (AttachmentDropMessage));

    public static string AttachmentDropMessage(CultureInfo culture) => WITResources.Get(nameof (AttachmentDropMessage), culture);

    public static string AttachmentUploadProgressNotification() => WITResources.Get(nameof (AttachmentUploadProgressNotification));

    public static string AttachmentUploadProgressNotification(CultureInfo culture) => WITResources.Get(nameof (AttachmentUploadProgressNotification), culture);

    public static string AttachmentUploadSuccessNotification() => WITResources.Get(nameof (AttachmentUploadSuccessNotification));

    public static string AttachmentUploadSuccessNotification(CultureInfo culture) => WITResources.Get(nameof (AttachmentUploadSuccessNotification), culture);

    public static string DialogClose() => WITResources.Get(nameof (DialogClose));

    public static string DialogClose(CultureInfo culture) => WITResources.Get(nameof (DialogClose), culture);

    public static string DialogSave() => WITResources.Get(nameof (DialogSave));

    public static string DialogSave(CultureInfo culture) => WITResources.Get(nameof (DialogSave), culture);

    public static string WorkItemTemplate_FailedToSave() => WITResources.Get(nameof (WorkItemTemplate_FailedToSave));

    public static string WorkItemTemplate_FailedToSave(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplate_FailedToSave), culture);

    public static string WorkItemTemplate_FailedToLoad() => WITResources.Get(nameof (WorkItemTemplate_FailedToLoad));

    public static string WorkItemTemplate_FailedToLoad(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplate_FailedToLoad), culture);

    public static string WorkItemTemplateDialog_CopyLink() => WITResources.Get(nameof (WorkItemTemplateDialog_CopyLink));

    public static string WorkItemTemplateDialog_CopyLink(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplateDialog_CopyLink), culture);

    public static string WorkItemTemplateDoesNotExist() => WITResources.Get(nameof (WorkItemTemplateDoesNotExist));

    public static string WorkItemTemplateDoesNotExist(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplateDoesNotExist), culture);

    public static string WorkItemTemplateNotApplied() => WITResources.Get(nameof (WorkItemTemplateNotApplied));

    public static string WorkItemTemplateNotApplied(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplateNotApplied), culture);

    public static string HistoryControlViaText() => WITResources.Get(nameof (HistoryControlViaText));

    public static string HistoryControlViaText(CultureInfo culture) => WITResources.Get(nameof (HistoryControlViaText), culture);

    public static string HistoryControlViaTextEnd() => WITResources.Get(nameof (HistoryControlViaTextEnd));

    public static string HistoryControlViaTextEnd(CultureInfo culture) => WITResources.Get(nameof (HistoryControlViaTextEnd), culture);

    public static string WorkItemTemplateDialog_CopyLinkDisabledMessage() => WITResources.Get(nameof (WorkItemTemplateDialog_CopyLinkDisabledMessage));

    public static string WorkItemTemplateDialog_CopyLinkDisabledMessage(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplateDialog_CopyLinkDisabledMessage), culture);

    public static string WorkItemTemplateDialog_CopyLinkEnabledMessage() => WITResources.Get(nameof (WorkItemTemplateDialog_CopyLinkEnabledMessage));

    public static string WorkItemTemplateDialog_CopyLinkEnabledMessage(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplateDialog_CopyLinkEnabledMessage), culture);

    public static string DeleteSingleAttachmentDialogConfirmationText(object arg0) => WITResources.Format(nameof (DeleteSingleAttachmentDialogConfirmationText), arg0);

    public static string DeleteSingleAttachmentDialogConfirmationText(
      object arg0,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (DeleteSingleAttachmentDialogConfirmationText), culture, arg0);
    }

    public static string WorkItemLogControlMadeChange(object arg0) => WITResources.Format(nameof (WorkItemLogControlMadeChange), arg0);

    public static string WorkItemLogControlMadeChange(object arg0, CultureInfo culture) => WITResources.Format(nameof (WorkItemLogControlMadeChange), culture, arg0);

    public static string WorkItemLogControlMadeChangeWithValue(object arg0, object arg1) => WITResources.Format(nameof (WorkItemLogControlMadeChangeWithValue), arg0, arg1);

    public static string WorkItemLogControlMadeChangeWithValue(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (WorkItemLogControlMadeChangeWithValue), culture, arg0, arg1);
    }

    public static string HistoryControlSummaryAssigned() => WITResources.Get(nameof (HistoryControlSummaryAssigned));

    public static string HistoryControlSummaryAssigned(CultureInfo culture) => WITResources.Get(nameof (HistoryControlSummaryAssigned), culture);

    public static string BulkEdit_ApplyTemplateButton() => WITResources.Get(nameof (BulkEdit_ApplyTemplateButton));

    public static string BulkEdit_ApplyTemplateButton(CultureInfo culture) => WITResources.Get(nameof (BulkEdit_ApplyTemplateButton), culture);

    public static string BulkEdit_ApplyTemplateMessage(object arg0) => WITResources.Format(nameof (BulkEdit_ApplyTemplateMessage), arg0);

    public static string BulkEdit_ApplyTemplateMessage(object arg0, CultureInfo culture) => WITResources.Format(nameof (BulkEdit_ApplyTemplateMessage), culture, arg0);

    public static string BulkEdit_ApplyTemplateTitle() => WITResources.Get(nameof (BulkEdit_ApplyTemplateTitle));

    public static string BulkEdit_ApplyTemplateTitle(CultureInfo culture) => WITResources.Get(nameof (BulkEdit_ApplyTemplateTitle), culture);

    public static string WorkItemTemplates_UserDoesNotHaveTeamPermission() => WITResources.Get(nameof (WorkItemTemplates_UserDoesNotHaveTeamPermission));

    public static string WorkItemTemplates_UserDoesNotHaveTeamPermission(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplates_UserDoesNotHaveTeamPermission), culture);

    public static string Unknown() => WITResources.Get(nameof (Unknown));

    public static string Unknown(CultureInfo culture) => WITResources.Get(nameof (Unknown), culture);

    public static string WorkItemDeleteError_ClickHere() => WITResources.Get(nameof (WorkItemDeleteError_ClickHere));

    public static string WorkItemDeleteError_ClickHere(CultureInfo culture) => WITResources.Get(nameof (WorkItemDeleteError_ClickHere), culture);

    public static string WorkItemDeleteError_MoreInfo() => WITResources.Get(nameof (WorkItemDeleteError_MoreInfo));

    public static string WorkItemDeleteError_MoreInfo(CultureInfo culture) => WITResources.Get(nameof (WorkItemDeleteError_MoreInfo), culture);

    public static string WorkItemTemplates_LegacyCopyMenuItem_Text() => WITResources.Get(nameof (WorkItemTemplates_LegacyCopyMenuItem_Text));

    public static string WorkItemTemplates_LegacyCopyMenuItem_Text(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplates_LegacyCopyMenuItem_Text), culture);

    public static string WorkItemTemplates_LegacyCopyMenuItem_Title() => WITResources.Get(nameof (WorkItemTemplates_LegacyCopyMenuItem_Title));

    public static string WorkItemTemplates_LegacyCopyMenuItem_Title(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplates_LegacyCopyMenuItem_Title), culture);

    public static string WorkItemTemplates_CaptureTemplateMenuItem_TooManyItemsTooltip() => WITResources.Get(nameof (WorkItemTemplates_CaptureTemplateMenuItem_TooManyItemsTooltip));

    public static string WorkItemTemplates_CaptureTemplateMenuItem_TooManyItemsTooltip(
      CultureInfo culture)
    {
      return WITResources.Get(nameof (WorkItemTemplates_CaptureTemplateMenuItem_TooManyItemsTooltip), culture);
    }

    public static string CreateCopyCopiedWithAllLinksFrom(object arg0) => WITResources.Format(nameof (CreateCopyCopiedWithAllLinksFrom), arg0);

    public static string CreateCopyCopiedWithAllLinksFrom(object arg0, CultureInfo culture) => WITResources.Format(nameof (CreateCopyCopiedWithAllLinksFrom), culture, arg0);

    public static string CreateCopyOfWorkItem_IncludeLinks() => WITResources.Get(nameof (CreateCopyOfWorkItem_IncludeLinks));

    public static string CreateCopyOfWorkItem_IncludeLinks(CultureInfo culture) => WITResources.Get(nameof (CreateCopyOfWorkItem_IncludeLinks), culture);

    public static string PreviewAttachment() => WITResources.Get(nameof (PreviewAttachment));

    public static string PreviewAttachment(CultureInfo culture) => WITResources.Get(nameof (PreviewAttachment), culture);

    public static string PreviewUnhandled() => WITResources.Get(nameof (PreviewUnhandled));

    public static string PreviewUnhandled(CultureInfo culture) => WITResources.Get(nameof (PreviewUnhandled), culture);

    public static string LinkDialogTypeNotValid() => WITResources.Get(nameof (LinkDialogTypeNotValid));

    public static string LinkDialogTypeNotValid(CultureInfo culture) => WITResources.Get(nameof (LinkDialogTypeNotValid), culture);

    public static string AttachmentsGrid_PlaceholderPrefixText() => WITResources.Get(nameof (AttachmentsGrid_PlaceholderPrefixText));

    public static string AttachmentsGrid_PlaceholderPrefixText(CultureInfo culture) => WITResources.Get(nameof (AttachmentsGrid_PlaceholderPrefixText), culture);

    public static string EmailResolveIdentityFailed() => WITResources.Get(nameof (EmailResolveIdentityFailed));

    public static string EmailResolveIdentityFailed(CultureInfo culture) => WITResources.Get(nameof (EmailResolveIdentityFailed), culture);

    public static string TestCaseDeleteConfirmationDialogText(
      object arg0,
      object arg1,
      object arg2)
    {
      return WITResources.Format(nameof (TestCaseDeleteConfirmationDialogText), arg0, arg1, arg2);
    }

    public static string TestCaseDeleteConfirmationDialogText(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (TestCaseDeleteConfirmationDialogText), culture, arg0, arg1, arg2);
    }

    public static string TestPlanDeleteConfirmationDialogText(
      object arg0,
      object arg1,
      object arg2)
    {
      return WITResources.Format(nameof (TestPlanDeleteConfirmationDialogText), arg0, arg1, arg2);
    }

    public static string TestPlanDeleteConfirmationDialogText(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (TestPlanDeleteConfirmationDialogText), culture, arg0, arg1, arg2);
    }

    public static string TestSuiteDeleteConfirmationDialogText(
      object arg0,
      object arg1,
      object arg2)
    {
      return WITResources.Format(nameof (TestSuiteDeleteConfirmationDialogText), arg0, arg1, arg2);
    }

    public static string TestSuiteDeleteConfirmationDialogText(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (TestSuiteDeleteConfirmationDialogText), culture, arg0, arg1, arg2);
    }

    public static string SharedParameterDeleteConfirmationDialogText(
      object arg0,
      object arg1,
      object arg2)
    {
      return WITResources.Format(nameof (SharedParameterDeleteConfirmationDialogText), arg0, arg1, arg2);
    }

    public static string SharedParameterDeleteConfirmationDialogText(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (SharedParameterDeleteConfirmationDialogText), culture, arg0, arg1, arg2);
    }

    public static string SharedStepDeleteConfirmationDialogText(
      object arg0,
      object arg1,
      object arg2)
    {
      return WITResources.Format(nameof (SharedStepDeleteConfirmationDialogText), arg0, arg1, arg2);
    }

    public static string SharedStepDeleteConfirmationDialogText(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (SharedStepDeleteConfirmationDialogText), culture, arg0, arg1, arg2);
    }

    public static string TestWorkItemDeleteButtonText() => WITResources.Get(nameof (TestWorkItemDeleteButtonText));

    public static string TestWorkItemDeleteButtonText(CultureInfo culture) => WITResources.Get(nameof (TestWorkItemDeleteButtonText), culture);

    public static string TestWorkItemDeleteDialogTitle() => WITResources.Get(nameof (TestWorkItemDeleteDialogTitle));

    public static string TestWorkItemDeleteDialogTitle(CultureInfo culture) => WITResources.Get(nameof (TestWorkItemDeleteDialogTitle), culture);

    public static string FetchingImplications() => WITResources.Get(nameof (FetchingImplications));

    public static string FetchingImplications(CultureInfo culture) => WITResources.Get(nameof (FetchingImplications), culture);

    public static string TestWorkItemDeleteDialogInputLabel(object arg0) => WITResources.Format(nameof (TestWorkItemDeleteDialogInputLabel), arg0);

    public static string TestWorkItemDeleteDialogInputLabel(object arg0, CultureInfo culture) => WITResources.Format(nameof (TestWorkItemDeleteDialogInputLabel), culture, arg0);

    public static string InvalidTestWorkItem() => WITResources.Get(nameof (InvalidTestWorkItem));

    public static string InvalidTestWorkItem(CultureInfo culture) => WITResources.Get(nameof (InvalidTestWorkItem), culture);

    public static string IdColonText() => WITResources.Get(nameof (IdColonText));

    public static string IdColonText(CultureInfo culture) => WITResources.Get(nameof (IdColonText), culture);

    public static string TestSuites() => WITResources.Get(nameof (TestSuites));

    public static string TestSuites(CultureInfo culture) => WITResources.Get(nameof (TestSuites), culture);

    public static string Tests() => WITResources.Get(nameof (Tests));

    public static string Tests(CultureInfo culture) => WITResources.Get(nameof (Tests), culture);

    public static string TestResults() => WITResources.Get(nameof (TestResults));

    public static string TestResults(CultureInfo culture) => WITResources.Get(nameof (TestResults), culture);

    public static string TestWorkItemAffectedItems() => WITResources.Get(nameof (TestWorkItemAffectedItems));

    public static string TestWorkItemAffectedItems(CultureInfo culture) => WITResources.Get(nameof (TestWorkItemAffectedItems), culture);

    public static string TestWorkItemDeletedItems() => WITResources.Get(nameof (TestWorkItemDeletedItems));

    public static string TestWorkItemDeletedItems(CultureInfo culture) => WITResources.Get(nameof (TestWorkItemDeletedItems), culture);

    public static string ChildTestSuites() => WITResources.Get(nameof (ChildTestSuites));

    public static string ChildTestSuites(CultureInfo culture) => WITResources.Get(nameof (ChildTestSuites), culture);

    public static string TestWorkItemDeletePermissionError() => WITResources.Get(nameof (TestWorkItemDeletePermissionError));

    public static string TestWorkItemDeletePermissionError(CultureInfo culture) => WITResources.Get(nameof (TestWorkItemDeletePermissionError), culture);

    public static string ErrorExtensionService_CannotRefreshWorkItem() => WITResources.Get(nameof (ErrorExtensionService_CannotRefreshWorkItem));

    public static string ErrorExtensionService_CannotRefreshWorkItem(CultureInfo culture) => WITResources.Get(nameof (ErrorExtensionService_CannotRefreshWorkItem), culture);

    public static string ErrorExtensionService_CannotRevertWorkItem() => WITResources.Get(nameof (ErrorExtensionService_CannotRevertWorkItem));

    public static string ErrorExtensionService_CannotRevertWorkItem(CultureInfo culture) => WITResources.Get(nameof (ErrorExtensionService_CannotRevertWorkItem), culture);

    public static string WorkItemFormIsReadonly() => WITResources.Get(nameof (WorkItemFormIsReadonly));

    public static string WorkItemFormIsReadonly(CultureInfo culture) => WITResources.Get(nameof (WorkItemFormIsReadonly), culture);

    public static string ScopedIdentityTooltip(object arg0) => WITResources.Format(nameof (ScopedIdentityTooltip), arg0);

    public static string ScopedIdentityTooltip(object arg0, CultureInfo culture) => WITResources.Format(nameof (ScopedIdentityTooltip), culture, arg0);

    public static string ScopedIdentityTooltipWithSuffix(object arg0) => WITResources.Format(nameof (ScopedIdentityTooltipWithSuffix), arg0);

    public static string ScopedIdentityTooltipWithSuffix(object arg0, CultureInfo culture) => WITResources.Format(nameof (ScopedIdentityTooltipWithSuffix), culture, arg0);

    public static string Heading_SuggestedValues() => WITResources.Get(nameof (Heading_SuggestedValues));

    public static string Heading_SuggestedValues(CultureInfo culture) => WITResources.Get(nameof (Heading_SuggestedValues), culture);

    public static string Picker_FilterWatermark() => WITResources.Get(nameof (Picker_FilterWatermark));

    public static string Picker_FilterWatermark(CultureInfo culture) => WITResources.Get(nameof (Picker_FilterWatermark), culture);

    public static string TagEdit_FilterWatermark() => WITResources.Get(nameof (TagEdit_FilterWatermark));

    public static string TagEdit_FilterWatermark(CultureInfo culture) => WITResources.Get(nameof (TagEdit_FilterWatermark), culture);

    public static string TagEdit_Separator() => WITResources.Get(nameof (TagEdit_Separator));

    public static string TagEdit_Separator(CultureInfo culture) => WITResources.Get(nameof (TagEdit_Separator), culture);

    public static string TagEdit_ZeroData() => WITResources.Get(nameof (TagEdit_ZeroData));

    public static string TagEdit_ZeroData(CultureInfo culture) => WITResources.Get(nameof (TagEdit_ZeroData), culture);

    public static string Queries() => WITResources.Get(nameof (Queries));

    public static string Queries(CultureInfo culture) => WITResources.Get(nameof (Queries), culture);

    public static string QueryColumnLastModifiedBy() => WITResources.Get(nameof (QueryColumnLastModifiedBy));

    public static string QueryColumnLastModifiedBy(CultureInfo culture) => WITResources.Get(nameof (QueryColumnLastModifiedBy), culture);

    public static string QueryColumnFolder() => WITResources.Get(nameof (QueryColumnFolder));

    public static string QueryColumnFolder(CultureInfo culture) => WITResources.Get(nameof (QueryColumnFolder), culture);

    public static string QueryColumnTitle() => WITResources.Get(nameof (QueryColumnTitle));

    public static string QueryColumnTitle(CultureInfo culture) => WITResources.Get(nameof (QueryColumnTitle), culture);

    public static string Updated() => WITResources.Get(nameof (Updated));

    public static string Updated(CultureInfo culture) => WITResources.Get(nameof (Updated), culture);

    public static string DeleteQueryFolder() => WITResources.Get(nameof (DeleteQueryFolder));

    public static string DeleteQueryFolder(CultureInfo culture) => WITResources.Get(nameof (DeleteQueryFolder), culture);

    public static string RenameQueryFolder() => WITResources.Get(nameof (RenameQueryFolder));

    public static string RenameQueryFolder(CultureInfo culture) => WITResources.Get(nameof (RenameQueryFolder), culture);

    public static string MobileDiscussionZeroData() => WITResources.Get(nameof (MobileDiscussionZeroData));

    public static string MobileDiscussionZeroData(CultureInfo culture) => WITResources.Get(nameof (MobileDiscussionZeroData), culture);

    public static string MobileDiscussionUnsaved() => WITResources.Get(nameof (MobileDiscussionUnsaved));

    public static string MobileDiscussionUnsaved(CultureInfo culture) => WITResources.Get(nameof (MobileDiscussionUnsaved), culture);

    public static string MobileDiscussionAddComment() => WITResources.Get(nameof (MobileDiscussionAddComment));

    public static string MobileDiscussionAddComment(CultureInfo culture) => WITResources.Get(nameof (MobileDiscussionAddComment), culture);

    public static string MobileDiscussionLoading() => WITResources.Get(nameof (MobileDiscussionLoading));

    public static string MobileDiscussionLoading(CultureInfo culture) => WITResources.Get(nameof (MobileDiscussionLoading), culture);

    public static string MobileDiscussionMessageSend() => WITResources.Get(nameof (MobileDiscussionMessageSend));

    public static string MobileDiscussionMessageSend(CultureInfo culture) => WITResources.Get(nameof (MobileDiscussionMessageSend), culture);

    public static string MobileDiscussionViewNumberOfComments(object arg0) => WITResources.Format(nameof (MobileDiscussionViewNumberOfComments), arg0);

    public static string MobileDiscussionViewNumberOfComments(object arg0, CultureInfo culture) => WITResources.Format(nameof (MobileDiscussionViewNumberOfComments), culture, arg0);

    public static string FavoriteQueryFailed(object arg0) => WITResources.Format(nameof (FavoriteQueryFailed), arg0);

    public static string FavoriteQueryFailed(object arg0, CultureInfo culture) => WITResources.Format(nameof (FavoriteQueryFailed), culture, arg0);

    public static string UnfavoriteQueryFailed(object arg0) => WITResources.Format(nameof (UnfavoriteQueryFailed), arg0);

    public static string UnfavoriteQueryFailed(object arg0, CultureInfo culture) => WITResources.Format(nameof (UnfavoriteQueryFailed), culture, arg0);

    public static string MaxQuerySearchResultMessage(object arg0, object arg1) => WITResources.Format(nameof (MaxQuerySearchResultMessage), arg0, arg1);

    public static string MaxQuerySearchResultMessage(object arg0, object arg1, CultureInfo culture) => WITResources.Format(nameof (MaxQuerySearchResultMessage), culture, arg0, arg1);

    public static string AttachmentsZeroData() => WITResources.Get(nameof (AttachmentsZeroData));

    public static string AttachmentsZeroData(CultureInfo culture) => WITResources.Get(nameof (AttachmentsZeroData), culture);

    public static string LinksControlZeroData() => WITResources.Get(nameof (LinksControlZeroData));

    public static string LinksControlZeroData(CultureInfo culture) => WITResources.Get(nameof (LinksControlZeroData), culture);

    public static string NameLabel_Watermark() => WITResources.Get(nameof (NameLabel_Watermark));

    public static string NameLabel_Watermark(CultureInfo culture) => WITResources.Get(nameof (NameLabel_Watermark), culture);

    public static string NewFolder() => WITResources.Get(nameof (NewFolder));

    public static string NewFolder(CultureInfo culture) => WITResources.Get(nameof (NewFolder), culture);

    public static string WorkItemPaneFilter_Bottom() => WITResources.Get(nameof (WorkItemPaneFilter_Bottom));

    public static string WorkItemPaneFilter_Bottom(CultureInfo culture) => WITResources.Get(nameof (WorkItemPaneFilter_Bottom), culture);

    public static string WorkItemPaneFilter_Off() => WITResources.Get(nameof (WorkItemPaneFilter_Off));

    public static string WorkItemPaneFilter_Off(CultureInfo culture) => WITResources.Get(nameof (WorkItemPaneFilter_Off), culture);

    public static string WorkItemPaneFilter_Right() => WITResources.Get(nameof (WorkItemPaneFilter_Right));

    public static string WorkItemPaneFilter_Right(CultureInfo culture) => WITResources.Get(nameof (WorkItemPaneFilter_Right), culture);

    public static string WorkItemPaneFilter_BottomAriaLabel() => WITResources.Get(nameof (WorkItemPaneFilter_BottomAriaLabel));

    public static string WorkItemPaneFilter_BottomAriaLabel(CultureInfo culture) => WITResources.Get(nameof (WorkItemPaneFilter_BottomAriaLabel), culture);

    public static string WorkItemPaneFilter_OffAriaLabel() => WITResources.Get(nameof (WorkItemPaneFilter_OffAriaLabel));

    public static string WorkItemPaneFilter_OffAriaLabel(CultureInfo culture) => WITResources.Get(nameof (WorkItemPaneFilter_OffAriaLabel), culture);

    public static string WorkItemPaneFilter_RightAriaLabel() => WITResources.Get(nameof (WorkItemPaneFilter_RightAriaLabel));

    public static string WorkItemPaneFilter_RightAriaLabel(CultureInfo culture) => WITResources.Get(nameof (WorkItemPaneFilter_RightAriaLabel), culture);

    public static string WorkItemPaneFilter_FilterWorkItems() => WITResources.Get(nameof (WorkItemPaneFilter_FilterWorkItems));

    public static string WorkItemPaneFilter_FilterWorkItems(CultureInfo culture) => WITResources.Get(nameof (WorkItemPaneFilter_FilterWorkItems), culture);

    public static string WorkItemsChartsTabTitle() => WITResources.Get(nameof (WorkItemsChartsTabTitle));

    public static string WorkItemsChartsTabTitle(CultureInfo culture) => WITResources.Get(nameof (WorkItemsChartsTabTitle), culture);

    public static string WorkItemsHubEditorTabTitle() => WITResources.Get(nameof (WorkItemsHubEditorTabTitle));

    public static string WorkItemsHubEditorTabTitle(CultureInfo culture) => WITResources.Get(nameof (WorkItemsHubEditorTabTitle), culture);

    public static string WorkItemsHubResultsTabTitle() => WITResources.Get(nameof (WorkItemsHubResultsTabTitle));

    public static string WorkItemsHubResultsTabTitle(CultureInfo culture) => WITResources.Get(nameof (WorkItemsHubResultsTabTitle), culture);

    public static string WorkItemFinderFindButtonText() => WITResources.Get(nameof (WorkItemFinderFindButtonText));

    public static string WorkItemFinderFindButtonText(CultureInfo culture) => WITResources.Get(nameof (WorkItemFinderFindButtonText), culture);

    public static string WorkItemsHubTruncatedCommentCount(object arg0) => WITResources.Format(nameof (WorkItemsHubTruncatedCommentCount), arg0);

    public static string WorkItemsHubTruncatedCommentCount(object arg0, CultureInfo culture) => WITResources.Format(nameof (WorkItemsHubTruncatedCommentCount), culture, arg0);

    public static string WorkItemsHubComments() => WITResources.Get(nameof (WorkItemsHubComments));

    public static string WorkItemsHubComments(CultureInfo culture) => WITResources.Get(nameof (WorkItemsHubComments), culture);

    public static string WorkItemFinderIds() => WITResources.Get(nameof (WorkItemFinderIds));

    public static string WorkItemFinderIds(CultureInfo culture) => WITResources.Get(nameof (WorkItemFinderIds), culture);

    public static string WorkItemFinderTitleContains() => WITResources.Get(nameof (WorkItemFinderTitleContains));

    public static string WorkItemFinderTitleContains(CultureInfo culture) => WITResources.Get(nameof (WorkItemFinderTitleContains), culture);

    public static string WorkItemFinderTypes() => WITResources.Get(nameof (WorkItemFinderTypes));

    public static string WorkItemFinderTypes(CultureInfo culture) => WITResources.Get(nameof (WorkItemFinderTypes), culture);

    public static string WorkItemMethodSelectionText() => WITResources.Get(nameof (WorkItemMethodSelectionText));

    public static string WorkItemMethodSelectionText(CultureInfo culture) => WITResources.Get(nameof (WorkItemMethodSelectionText), culture);

    public static string QueryMyFavoriteLabel() => WITResources.Get(nameof (QueryMyFavoriteLabel));

    public static string QueryMyFavoriteLabel(CultureInfo culture) => WITResources.Get(nameof (QueryMyFavoriteLabel), culture);

    public static string QueryEditorRoleTitle() => WITResources.Get(nameof (QueryEditorRoleTitle));

    public static string QueryEditorRoleTitle(CultureInfo culture) => WITResources.Get(nameof (QueryEditorRoleTitle), culture);

    public static string QueryResultsGridRoleTitle() => WITResources.Get(nameof (QueryResultsGridRoleTitle));

    public static string QueryResultsGridRoleTitle(CultureInfo culture) => WITResources.Get(nameof (QueryResultsGridRoleTitle), culture);

    public static string WorkItemDetailsRoleTitle() => WITResources.Get(nameof (WorkItemDetailsRoleTitle));

    public static string WorkItemDetailsRoleTitle(CultureInfo culture) => WITResources.Get(nameof (WorkItemDetailsRoleTitle), culture);

    public static string BulkEditDialogFieldsRoleTitle() => WITResources.Get(nameof (BulkEditDialogFieldsRoleTitle));

    public static string BulkEditDialogFieldsRoleTitle(CultureInfo culture) => WITResources.Get(nameof (BulkEditDialogFieldsRoleTitle), culture);

    public static string BulkEditDialogHistoryRoleTitle() => WITResources.Get(nameof (BulkEditDialogHistoryRoleTitle));

    public static string BulkEditDialogHistoryRoleTitle(CultureInfo culture) => WITResources.Get(nameof (BulkEditDialogHistoryRoleTitle), culture);

    public static string EmptyMyFavoriteGroupTextPrefix() => WITResources.Get(nameof (EmptyMyFavoriteGroupTextPrefix));

    public static string EmptyMyFavoriteGroupTextPrefix(CultureInfo culture) => WITResources.Get(nameof (EmptyMyFavoriteGroupTextPrefix), culture);

    public static string EmptyTeamFavoriteGroupTextPrefix() => WITResources.Get(nameof (EmptyTeamFavoriteGroupTextPrefix));

    public static string EmptyTeamFavoriteGroupTextPrefix(CultureInfo culture) => WITResources.Get(nameof (EmptyTeamFavoriteGroupTextPrefix), culture);

    public static string EmptyMyFavoriteGroupTextSuffix() => WITResources.Get(nameof (EmptyMyFavoriteGroupTextSuffix));

    public static string EmptyMyFavoriteGroupTextSuffix(CultureInfo culture) => WITResources.Get(nameof (EmptyMyFavoriteGroupTextSuffix), culture);

    public static string EmptyTeamFavoriteGroupTextLink() => WITResources.Get(nameof (EmptyTeamFavoriteGroupTextLink));

    public static string EmptyTeamFavoriteGroupTextLink(CultureInfo culture) => WITResources.Get(nameof (EmptyTeamFavoriteGroupTextLink), culture);

    public static string EmptyTeamFavoriteGroupTextSuffix() => WITResources.Get(nameof (EmptyTeamFavoriteGroupTextSuffix));

    public static string EmptyTeamFavoriteGroupTextSuffix(CultureInfo culture) => WITResources.Get(nameof (EmptyTeamFavoriteGroupTextSuffix), culture);

    public static string UntitledQuery() => WITResources.Get(nameof (UntitledQuery));

    public static string UntitledQuery(CultureInfo culture) => WITResources.Get(nameof (UntitledQuery), culture);

    public static string MyFavoritesGroupName() => WITResources.Get(nameof (MyFavoritesGroupName));

    public static string MyFavoritesGroupName(CultureInfo culture) => WITResources.Get(nameof (MyFavoritesGroupName), culture);

    public static string LastVisitedGroupName() => WITResources.Get(nameof (LastVisitedGroupName));

    public static string LastVisitedGroupName(CultureInfo culture) => WITResources.Get(nameof (LastVisitedGroupName), culture);

    public static string LoadingTeamFavorites() => WITResources.Get(nameof (LoadingTeamFavorites));

    public static string LoadingTeamFavorites(CultureInfo culture) => WITResources.Get(nameof (LoadingTeamFavorites), culture);

    public static string WorkItemTemplateDialog_SavingAnnouncerEnd() => WITResources.Get(nameof (WorkItemTemplateDialog_SavingAnnouncerEnd));

    public static string WorkItemTemplateDialog_SavingAnnouncerEnd(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplateDialog_SavingAnnouncerEnd), culture);

    public static string WorkItemTemplateDialog_SavingAnnouncerError() => WITResources.Get(nameof (WorkItemTemplateDialog_SavingAnnouncerError));

    public static string WorkItemTemplateDialog_SavingAnnouncerError(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplateDialog_SavingAnnouncerError), culture);

    public static string WorkItemTemplateDialog_SavingAnnouncerStart() => WITResources.Get(nameof (WorkItemTemplateDialog_SavingAnnouncerStart));

    public static string WorkItemTemplateDialog_SavingAnnouncerStart(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplateDialog_SavingAnnouncerStart), culture);

    public static string LinkingDialog_LoadingEnd() => WITResources.Get(nameof (LinkingDialog_LoadingEnd));

    public static string LinkingDialog_LoadingEnd(CultureInfo culture) => WITResources.Get(nameof (LinkingDialog_LoadingEnd), culture);

    public static string LinkingDialog_LoadingStart() => WITResources.Get(nameof (LinkingDialog_LoadingStart));

    public static string LinkingDialog_LoadingStart(CultureInfo culture) => WITResources.Get(nameof (LinkingDialog_LoadingStart), culture);

    public static string MoreSaveOptions() => WITResources.Get(nameof (MoreSaveOptions));

    public static string MoreSaveOptions(CultureInfo culture) => WITResources.Get(nameof (MoreSaveOptions), culture);

    public static string SaveHotkeyWithControl() => WITResources.Get(nameof (SaveHotkeyWithControl));

    public static string SaveHotkeyWithControl(CultureInfo culture) => WITResources.Get(nameof (SaveHotkeyWithControl), culture);

    public static string SaveHotkeyWithCommand() => WITResources.Get(nameof (SaveHotkeyWithCommand));

    public static string SaveHotkeyWithCommand(CultureInfo culture) => WITResources.Get(nameof (SaveHotkeyWithCommand), culture);

    public static string SaveAndCloseHotkeyWithControl() => WITResources.Get(nameof (SaveAndCloseHotkeyWithControl));

    public static string SaveAndCloseHotkeyWithControl(CultureInfo culture) => WITResources.Get(nameof (SaveAndCloseHotkeyWithControl), culture);

    public static string SaveAndCloseHotkeyWithCommand() => WITResources.Get(nameof (SaveAndCloseHotkeyWithCommand));

    public static string SaveAndCloseHotkeyWithCommand(CultureInfo culture) => WITResources.Get(nameof (SaveAndCloseHotkeyWithCommand), culture);

    public static string CopyToClipboard() => WITResources.Get(nameof (CopyToClipboard));

    public static string CopyToClipboard(CultureInfo culture) => WITResources.Get(nameof (CopyToClipboard), culture);

    public static string QueryFolderDoesNotExist(object arg0) => WITResources.Format(nameof (QueryFolderDoesNotExist), arg0);

    public static string QueryFolderDoesNotExist(object arg0, CultureInfo culture) => WITResources.Format(nameof (QueryFolderDoesNotExist), culture, arg0);

    public static string PrivacyStatement() => WITResources.Get(nameof (PrivacyStatement));

    public static string PrivacyStatement(CultureInfo culture) => WITResources.Get(nameof (PrivacyStatement), culture);

    public static string ItemsPlural(object arg0) => WITResources.Format(nameof (ItemsPlural), arg0);

    public static string ItemsPlural(object arg0, CultureInfo culture) => WITResources.Format(nameof (ItemsPlural), culture, arg0);

    public static string LinkTopologyDependencyImageAltText() => WITResources.Get(nameof (LinkTopologyDependencyImageAltText));

    public static string LinkTopologyDependencyImageAltText(CultureInfo culture) => WITResources.Get(nameof (LinkTopologyDependencyImageAltText), culture);

    public static string LinkTopologyDirectedNetworkImageAltText() => WITResources.Get(nameof (LinkTopologyDirectedNetworkImageAltText));

    public static string LinkTopologyDirectedNetworkImageAltText(CultureInfo culture) => WITResources.Get(nameof (LinkTopologyDirectedNetworkImageAltText), culture);

    public static string LinkTopologyNetworkImageAltText() => WITResources.Get(nameof (LinkTopologyNetworkImageAltText));

    public static string LinkTopologyNetworkImageAltText(CultureInfo culture) => WITResources.Get(nameof (LinkTopologyNetworkImageAltText), culture);

    public static string LinkTopologyTreeReverseImageAltText() => WITResources.Get(nameof (LinkTopologyTreeReverseImageAltText));

    public static string LinkTopologyTreeReverseImageAltText(CultureInfo culture) => WITResources.Get(nameof (LinkTopologyTreeReverseImageAltText), culture);

    public static string LinkTopologyTreeForwardImageAltText() => WITResources.Get(nameof (LinkTopologyTreeForwardImageAltText));

    public static string LinkTopologyTreeForwardImageAltText(CultureInfo culture) => WITResources.Get(nameof (LinkTopologyTreeForwardImageAltText), culture);

    public static string QueryTeamFavoriteLabel(object arg0) => WITResources.Format(nameof (QueryTeamFavoriteLabel), arg0);

    public static string QueryTeamFavoriteLabel(object arg0, CultureInfo culture) => WITResources.Format(nameof (QueryTeamFavoriteLabel), culture, arg0);

    public static string Picker_EmptyInput() => WITResources.Get(nameof (Picker_EmptyInput));

    public static string Picker_EmptyInput(CultureInfo culture) => WITResources.Get(nameof (Picker_EmptyInput), culture);

    public static string WorkItemSaveLabel() => WITResources.Get(nameof (WorkItemSaveLabel));

    public static string WorkItemSaveLabel(CultureInfo culture) => WITResources.Get(nameof (WorkItemSaveLabel), culture);

    public static string ReadOnlyFieldIconLabel(object arg0) => WITResources.Format(nameof (ReadOnlyFieldIconLabel), arg0);

    public static string ReadOnlyFieldIconLabel(object arg0, CultureInfo culture) => WITResources.Format(nameof (ReadOnlyFieldIconLabel), culture, arg0);

    public static string WorkItemChangedReproDescriptionMessage(
      object arg0,
      object arg1,
      object arg2)
    {
      return WITResources.Format(nameof (WorkItemChangedReproDescriptionMessage), arg0, arg1, arg2);
    }

    public static string WorkItemChangedReproDescriptionMessage(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (WorkItemChangedReproDescriptionMessage), culture, arg0, arg1, arg2);
    }

    public static string AssignedToField() => WITResources.Get(nameof (AssignedToField));

    public static string AssignedToField(CultureInfo culture) => WITResources.Get(nameof (AssignedToField), culture);

    public static string IdField() => WITResources.Get(nameof (IdField));

    public static string IdField(CultureInfo culture) => WITResources.Get(nameof (IdField), culture);

    public static string TitleField() => WITResources.Get(nameof (TitleField));

    public static string TitleField(CultureInfo culture) => WITResources.Get(nameof (TitleField), culture);

    public static string StateField() => WITResources.Get(nameof (StateField));

    public static string StateField(CultureInfo culture) => WITResources.Get(nameof (StateField), culture);

    public static string ReasonField() => WITResources.Get(nameof (ReasonField));

    public static string ReasonField(CultureInfo culture) => WITResources.Get(nameof (ReasonField), culture);

    public static string BooleanFieldFalseValue() => WITResources.Get(nameof (BooleanFieldFalseValue));

    public static string BooleanFieldFalseValue(CultureInfo culture) => WITResources.Get(nameof (BooleanFieldFalseValue), culture);

    public static string BooleanFieldTrueValue() => WITResources.Get(nameof (BooleanFieldTrueValue));

    public static string BooleanFieldTrueValue(CultureInfo culture) => WITResources.Get(nameof (BooleanFieldTrueValue), culture);

    public static string AttachmentLoading() => WITResources.Get(nameof (AttachmentLoading));

    public static string AttachmentLoading(CultureInfo culture) => WITResources.Get(nameof (AttachmentLoading), culture);

    public static string AttachmentUploadingEnd() => WITResources.Get(nameof (AttachmentUploadingEnd));

    public static string AttachmentUploadingEnd(CultureInfo culture) => WITResources.Get(nameof (AttachmentUploadingEnd), culture);

    public static string AttachmentUploadingError() => WITResources.Get(nameof (AttachmentUploadingError));

    public static string AttachmentUploadingError(CultureInfo culture) => WITResources.Get(nameof (AttachmentUploadingError), culture);

    public static string AttachmentUploadingStart() => WITResources.Get(nameof (AttachmentUploadingStart));

    public static string AttachmentUploadingStart(CultureInfo culture) => WITResources.Get(nameof (AttachmentUploadingStart), culture);

    public static string TagAriaLabel(object arg0, object arg1, object arg2) => WITResources.Format(nameof (TagAriaLabel), arg0, arg1, arg2);

    public static string TagAriaLabel(object arg0, object arg1, object arg2, CultureInfo culture) => WITResources.Format(nameof (TagAriaLabel), culture, arg0, arg1, arg2);

    public static string TagCountAriaLabel(object arg0, object arg1) => WITResources.Format(nameof (TagCountAriaLabel), arg0, arg1);

    public static string TagCountAriaLabel(object arg0, object arg1, CultureInfo culture) => WITResources.Format(nameof (TagCountAriaLabel), culture, arg0, arg1);

    public static string EditTagsAriaLabel(object arg0) => WITResources.Format(nameof (EditTagsAriaLabel), arg0);

    public static string EditTagsAriaLabel(object arg0, CultureInfo culture) => WITResources.Format(nameof (EditTagsAriaLabel), culture, arg0);

    public static string WorkItemDiscussionPreviewAriaLabel(object arg0) => WITResources.Format(nameof (WorkItemDiscussionPreviewAriaLabel), arg0);

    public static string WorkItemDiscussionPreviewAriaLabel(object arg0, CultureInfo culture) => WITResources.Format(nameof (WorkItemDiscussionPreviewAriaLabel), culture, arg0);

    public static string WorkItemZeroDataDiscussionAriaLabel() => WITResources.Get(nameof (WorkItemZeroDataDiscussionAriaLabel));

    public static string WorkItemZeroDataDiscussionAriaLabel(CultureInfo culture) => WITResources.Get(nameof (WorkItemZeroDataDiscussionAriaLabel), culture);

    public static string WorkItemUnsavedDiscussionAriaLabel() => WITResources.Get(nameof (WorkItemUnsavedDiscussionAriaLabel));

    public static string WorkItemUnsavedDiscussionAriaLabel(CultureInfo culture) => WITResources.Get(nameof (WorkItemUnsavedDiscussionAriaLabel), culture);

    public static string WorkItemDiscussionCommentAriaLabel(object arg0, object arg1) => WITResources.Format(nameof (WorkItemDiscussionCommentAriaLabel), arg0, arg1);

    public static string WorkItemDiscussionCommentAriaLabel(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (WorkItemDiscussionCommentAriaLabel), culture, arg0, arg1);
    }

    public static string WorkItemFinderWorkItemTypes() => WITResources.Get(nameof (WorkItemFinderWorkItemTypes));

    public static string WorkItemFinderWorkItemTypes(CultureInfo culture) => WITResources.Get(nameof (WorkItemFinderWorkItemTypes), culture);

    public static string FilterByKeyword() => WITResources.Get(nameof (FilterByKeyword));

    public static string FilterByKeyword(CultureInfo culture) => WITResources.Get(nameof (FilterByKeyword), culture);

    public static string FilterByTypes() => WITResources.Get(nameof (FilterByTypes));

    public static string FilterByTypes(CultureInfo culture) => WITResources.Get(nameof (FilterByTypes), culture);

    public static string FilterByStates() => WITResources.Get(nameof (FilterByStates));

    public static string FilterByStates(CultureInfo culture) => WITResources.Get(nameof (FilterByStates), culture);

    public static string FilterByAssignedTo() => WITResources.Get(nameof (FilterByAssignedTo));

    public static string FilterByAssignedTo(CultureInfo culture) => WITResources.Get(nameof (FilterByAssignedTo), culture);

    public static string FilterByTags() => WITResources.Get(nameof (FilterByTags));

    public static string FilterByTags(CultureInfo culture) => WITResources.Get(nameof (FilterByTags), culture);

    public static string FilterByWorkItemStatus() => WITResources.Get(nameof (FilterByWorkItemStatus));

    public static string FilterByWorkItemStatus(CultureInfo culture) => WITResources.Get(nameof (FilterByWorkItemStatus), culture);

    public static string FilterStatusFieldName() => WITResources.Get(nameof (FilterStatusFieldName));

    public static string FilterStatusFieldName(CultureInfo culture) => WITResources.Get(nameof (FilterStatusFieldName), culture);

    public static string FilterInvalidWorkItems() => WITResources.Get(nameof (FilterInvalidWorkItems));

    public static string FilterInvalidWorkItems(CultureInfo culture) => WITResources.Get(nameof (FilterInvalidWorkItems), culture);

    public static string FilterValidWorkItems() => WITResources.Get(nameof (FilterValidWorkItems));

    public static string FilterValidWorkItems(CultureInfo culture) => WITResources.Get(nameof (FilterValidWorkItems), culture);

    public static string FilterNoStatuses() => WITResources.Get(nameof (FilterNoStatuses));

    public static string FilterNoStatuses(CultureInfo culture) => WITResources.Get(nameof (FilterNoStatuses), culture);

    public static string FilterNoTypes() => WITResources.Get(nameof (FilterNoTypes));

    public static string FilterNoTypes(CultureInfo culture) => WITResources.Get(nameof (FilterNoTypes), culture);

    public static string FilterNoStates() => WITResources.Get(nameof (FilterNoStates));

    public static string FilterNoStates(CultureInfo culture) => WITResources.Get(nameof (FilterNoStates), culture);

    public static string FilterNoAssignedTo() => WITResources.Get(nameof (FilterNoAssignedTo));

    public static string FilterNoAssignedTo(CultureInfo culture) => WITResources.Get(nameof (FilterNoAssignedTo), culture);

    public static string FilterNoTags() => WITResources.Get(nameof (FilterNoTags));

    public static string FilterNoTags(CultureInfo culture) => WITResources.Get(nameof (FilterNoTags), culture);

    public static string DateTimePicker_GoToToday() => WITResources.Get(nameof (DateTimePicker_GoToToday));

    public static string DateTimePicker_GoToToday(CultureInfo culture) => WITResources.Get(nameof (DateTimePicker_GoToToday), culture);

    public static string DateTimePicker_AMPMDownAriaLabel() => WITResources.Get(nameof (DateTimePicker_AMPMDownAriaLabel));

    public static string DateTimePicker_AMPMDownAriaLabel(CultureInfo culture) => WITResources.Get(nameof (DateTimePicker_AMPMDownAriaLabel), culture);

    public static string DateTimePicker_AMPMUpAriaLabel() => WITResources.Get(nameof (DateTimePicker_AMPMUpAriaLabel));

    public static string DateTimePicker_AMPMUpAriaLabel(CultureInfo culture) => WITResources.Get(nameof (DateTimePicker_AMPMUpAriaLabel), culture);

    public static string DateTimePicker_HourDownAriaLabel() => WITResources.Get(nameof (DateTimePicker_HourDownAriaLabel));

    public static string DateTimePicker_HourDownAriaLabel(CultureInfo culture) => WITResources.Get(nameof (DateTimePicker_HourDownAriaLabel), culture);

    public static string DateTimePicker_HourUpAriaLabel() => WITResources.Get(nameof (DateTimePicker_HourUpAriaLabel));

    public static string DateTimePicker_HourUpAriaLabel(CultureInfo culture) => WITResources.Get(nameof (DateTimePicker_HourUpAriaLabel), culture);

    public static string DateTimePicker_MinuteDownAriaLabel() => WITResources.Get(nameof (DateTimePicker_MinuteDownAriaLabel));

    public static string DateTimePicker_MinuteDownAriaLabel(CultureInfo culture) => WITResources.Get(nameof (DateTimePicker_MinuteDownAriaLabel), culture);

    public static string DateTimePicker_MinuteUpAriaLabel() => WITResources.Get(nameof (DateTimePicker_MinuteUpAriaLabel));

    public static string DateTimePicker_MinuteUpAriaLabel(CultureInfo culture) => WITResources.Get(nameof (DateTimePicker_MinuteUpAriaLabel), culture);

    public static string DateTimePicker_SelectDateTime() => WITResources.Get(nameof (DateTimePicker_SelectDateTime));

    public static string DateTimePicker_SelectDateTime(CultureInfo culture) => WITResources.Get(nameof (DateTimePicker_SelectDateTime), culture);

    public static string DateTimePicker_ClearDate() => WITResources.Get(nameof (DateTimePicker_ClearDate));

    public static string DateTimePicker_ClearDate(CultureInfo culture) => WITResources.Get(nameof (DateTimePicker_ClearDate), culture);

    public static string HistoryControlChangesHeader() => WITResources.Get(nameof (HistoryControlChangesHeader));

    public static string HistoryControlChangesHeader(CultureInfo culture) => WITResources.Get(nameof (HistoryControlChangesHeader), culture);

    public static string MobileDiscussionViewNumberOfCommentsSingle() => WITResources.Get(nameof (MobileDiscussionViewNumberOfCommentsSingle));

    public static string MobileDiscussionViewNumberOfCommentsSingle(CultureInfo culture) => WITResources.Get(nameof (MobileDiscussionViewNumberOfCommentsSingle), culture);

    public static string FieldValueComboAriaLabel(object arg0) => WITResources.Format(nameof (FieldValueComboAriaLabel), arg0);

    public static string FieldValueComboAriaLabel(object arg0, CultureInfo culture) => WITResources.Format(nameof (FieldValueComboAriaLabel), culture, arg0);

    public static string FieldValueIdentityPickerAriaLabel(object arg0) => WITResources.Format(nameof (FieldValueIdentityPickerAriaLabel), arg0);

    public static string FieldValueIdentityPickerAriaLabel(object arg0, CultureInfo culture) => WITResources.Format(nameof (FieldValueIdentityPickerAriaLabel), culture, arg0);

    public static string HistoryControlCommentAriaLabel() => WITResources.Get(nameof (HistoryControlCommentAriaLabel));

    public static string HistoryControlCommentAriaLabel(CultureInfo culture) => WITResources.Get(nameof (HistoryControlCommentAriaLabel), culture);

    public static string CopyWorkItemTitle() => WITResources.Get(nameof (CopyWorkItemTitle));

    public static string CopyWorkItemTitle(CultureInfo culture) => WITResources.Get(nameof (CopyWorkItemTitle), culture);

    public static string GroupErrorAriaLabel(object arg0) => WITResources.Format(nameof (GroupErrorAriaLabel), arg0);

    public static string GroupErrorAriaLabel(object arg0, CultureInfo culture) => WITResources.Format(nameof (GroupErrorAriaLabel), culture, arg0);

    public static string FieldErrorAriaLabel(object arg0) => WITResources.Format(nameof (FieldErrorAriaLabel), arg0);

    public static string FieldErrorAriaLabel(object arg0, CultureInfo culture) => WITResources.Format(nameof (FieldErrorAriaLabel), culture, arg0);

    public static string WorkItemFieldLabelTitleFormat(object arg0) => WITResources.Format(nameof (WorkItemFieldLabelTitleFormat), arg0);

    public static string WorkItemFieldLabelTitleFormat(object arg0, CultureInfo culture) => WITResources.Format(nameof (WorkItemFieldLabelTitleFormat), culture, arg0);

    public static string QueryMoveOrRename_QueryItemAlreadyExists() => WITResources.Get(nameof (QueryMoveOrRename_QueryItemAlreadyExists));

    public static string QueryMoveOrRename_QueryItemAlreadyExists(CultureInfo culture) => WITResources.Get(nameof (QueryMoveOrRename_QueryItemAlreadyExists), culture);

    public static string QueriesView_PivotTitle() => WITResources.Get(nameof (QueriesView_PivotTitle));

    public static string QueriesView_PivotTitle(CultureInfo culture) => WITResources.Get(nameof (QueriesView_PivotTitle), culture);

    public static string QueryFolderPicker_ErrorMessage_FolderDoesNotExist() => WITResources.Get(nameof (QueryFolderPicker_ErrorMessage_FolderDoesNotExist));

    public static string QueryFolderPicker_ErrorMessage_FolderDoesNotExist(CultureInfo culture) => WITResources.Get(nameof (QueryFolderPicker_ErrorMessage_FolderDoesNotExist), culture);

    public static string QuerySaveDialog_ErrorMessage_NameInvalid() => WITResources.Get(nameof (QuerySaveDialog_ErrorMessage_NameInvalid));

    public static string QuerySaveDialog_ErrorMessage_NameInvalid(CultureInfo culture) => WITResources.Get(nameof (QuerySaveDialog_ErrorMessage_NameInvalid), culture);

    public static string QuerySaveDialog_ErrorMessage_NameAlreadyExists() => WITResources.Get(nameof (QuerySaveDialog_ErrorMessage_NameAlreadyExists));

    public static string QuerySaveDialog_ErrorMessage_NameAlreadyExists(CultureInfo culture) => WITResources.Get(nameof (QuerySaveDialog_ErrorMessage_NameAlreadyExists), culture);

    public static string QueryFolderPicker_ErrorMessage_NameExceededLimit(object arg0, object arg1) => WITResources.Format(nameof (QueryFolderPicker_ErrorMessage_NameExceededLimit), arg0, arg1);

    public static string QueryFolderPicker_ErrorMessage_NameExceededLimit(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (QueryFolderPicker_ErrorMessage_NameExceededLimit), culture, arg0, arg1);
    }

    public static string QuerySaveDialog_ErrorMessage_NameRequired() => WITResources.Get(nameof (QuerySaveDialog_ErrorMessage_NameRequired));

    public static string QuerySaveDialog_ErrorMessage_NameRequired(CultureInfo culture) => WITResources.Get(nameof (QuerySaveDialog_ErrorMessage_NameRequired), culture);

    public static string QueryFolderPicker_ErrorMessage_FolderRequired() => WITResources.Get(nameof (QueryFolderPicker_ErrorMessage_FolderRequired));

    public static string QueryFolderPicker_ErrorMessage_FolderRequired(CultureInfo culture) => WITResources.Get(nameof (QueryFolderPicker_ErrorMessage_FolderRequired), culture);

    public static string LinkWorkItemTypeNotValid() => WITResources.Get(nameof (LinkWorkItemTypeNotValid));

    public static string LinkWorkItemTypeNotValid(CultureInfo culture) => WITResources.Get(nameof (LinkWorkItemTypeNotValid), culture);

    public static string DeleteWorkItemsToSeeMore() => WITResources.Get(nameof (DeleteWorkItemsToSeeMore));

    public static string DeleteWorkItemsToSeeMore(CultureInfo culture) => WITResources.Get(nameof (DeleteWorkItemsToSeeMore), culture);

    public static string Confirm() => WITResources.Get(nameof (Confirm));

    public static string Confirm(CultureInfo culture) => WITResources.Get(nameof (Confirm), culture);

    public static string MobileGoBackAriaLabel() => WITResources.Get(nameof (MobileGoBackAriaLabel));

    public static string MobileGoBackAriaLabel(CultureInfo culture) => WITResources.Get(nameof (MobileGoBackAriaLabel), culture);

    public static string WorkItemNotSaveable() => WITResources.Get(nameof (WorkItemNotSaveable));

    public static string WorkItemNotSaveable(CultureInfo culture) => WITResources.Get(nameof (WorkItemNotSaveable), culture);

    public static string ContributionBlockingWITSave(object arg0) => WITResources.Format(nameof (ContributionBlockingWITSave), arg0);

    public static string ContributionBlockingWITSave(object arg0, CultureInfo culture) => WITResources.Format(nameof (ContributionBlockingWITSave), culture, arg0);

    public static string DateTimePicker_NextMonthAriaLabel() => WITResources.Get(nameof (DateTimePicker_NextMonthAriaLabel));

    public static string DateTimePicker_NextMonthAriaLabel(CultureInfo culture) => WITResources.Get(nameof (DateTimePicker_NextMonthAriaLabel), culture);

    public static string DateTimePicker_PreviousMonthAriaLabel() => WITResources.Get(nameof (DateTimePicker_PreviousMonthAriaLabel));

    public static string DateTimePicker_PreviousMonthAriaLabel(CultureInfo culture) => WITResources.Get(nameof (DateTimePicker_PreviousMonthAriaLabel), culture);

    public static string DiscussionViewNumberOfComments(object arg0) => WITResources.Format(nameof (DiscussionViewNumberOfComments), arg0);

    public static string DiscussionViewNumberOfComments(object arg0, CultureInfo culture) => WITResources.Format(nameof (DiscussionViewNumberOfComments), culture, arg0);

    public static string DiscussionViewNumberOfCommentsSingle() => WITResources.Get(nameof (DiscussionViewNumberOfCommentsSingle));

    public static string DiscussionViewNumberOfCommentsSingle(CultureInfo culture) => WITResources.Get(nameof (DiscussionViewNumberOfCommentsSingle), culture);

    public static string TabErrorAriaLabel(object arg0) => WITResources.Format(nameof (TabErrorAriaLabel), arg0);

    public static string TabErrorAriaLabel(object arg0, CultureInfo culture) => WITResources.Format(nameof (TabErrorAriaLabel), culture, arg0);

    public static string WorkItemControlReadOnly(object arg0) => WITResources.Format(nameof (WorkItemControlReadOnly), arg0);

    public static string WorkItemControlReadOnly(object arg0, CultureInfo culture) => WITResources.Format(nameof (WorkItemControlReadOnly), culture, arg0);

    public static string HistoryControlTreeAriaLabel() => WITResources.Get(nameof (HistoryControlTreeAriaLabel));

    public static string HistoryControlTreeAriaLabel(CultureInfo culture) => WITResources.Get(nameof (HistoryControlTreeAriaLabel), culture);

    public static string Wiql_MacroCurrentIteration() => WITResources.Get(nameof (Wiql_MacroCurrentIteration));

    public static string Wiql_MacroCurrentIteration(CultureInfo culture) => WITResources.Get(nameof (Wiql_MacroCurrentIteration), culture);

    public static string Wiql_MacroMe() => WITResources.Get(nameof (Wiql_MacroMe));

    public static string Wiql_MacroMe(CultureInfo culture) => WITResources.Get(nameof (Wiql_MacroMe), culture);

    public static string QueriesView_AllPivotName() => WITResources.Get(nameof (QueriesView_AllPivotName));

    public static string QueriesView_AllPivotName(CultureInfo culture) => WITResources.Get(nameof (QueriesView_AllPivotName), culture);

    public static string QueriesView_FolderPivotName() => WITResources.Get(nameof (QueriesView_FolderPivotName));

    public static string QueriesView_FolderPivotName(CultureInfo culture) => WITResources.Get(nameof (QueriesView_FolderPivotName), culture);

    public static string QueriesView_FavoritesPivotName() => WITResources.Get(nameof (QueriesView_FavoritesPivotName));

    public static string QueriesView_FavoritesPivotName(CultureInfo culture) => WITResources.Get(nameof (QueriesView_FavoritesPivotName), culture);

    public static string FilterByAreaPath() => WITResources.Get(nameof (FilterByAreaPath));

    public static string FilterByAreaPath(CultureInfo culture) => WITResources.Get(nameof (FilterByAreaPath), culture);

    public static string BackToQuery() => WITResources.Get(nameof (BackToQuery));

    public static string BackToQuery(CultureInfo culture) => WITResources.Get(nameof (BackToQuery), culture);

    public static string SaveItems() => WITResources.Get(nameof (SaveItems));

    public static string SaveItems(CultureInfo culture) => WITResources.Get(nameof (SaveItems), culture);

    public static string EmailQuery() => WITResources.Get(nameof (EmailQuery));

    public static string EmailQuery(CultureInfo culture) => WITResources.Get(nameof (EmailQuery), culture);

    public static string ExportToCSV() => WITResources.Get(nameof (ExportToCSV));

    public static string ExportToCSV(CultureInfo culture) => WITResources.Get(nameof (ExportToCSV), culture);

    public static string ExportToCSVError() => WITResources.Get(nameof (ExportToCSVError));

    public static string ExportToCSVError(CultureInfo culture) => WITResources.Get(nameof (ExportToCSVError), culture);

    public static string DirectLinksFlattenedMessage() => WITResources.Get(nameof (DirectLinksFlattenedMessage));

    public static string DirectLinksFlattenedMessage(CultureInfo culture) => WITResources.Get(nameof (DirectLinksFlattenedMessage), culture);

    public static string WarningExportingDirectLinksTitle() => WITResources.Get(nameof (WarningExportingDirectLinksTitle));

    public static string WarningExportingDirectLinksTitle(CultureInfo culture) => WITResources.Get(nameof (WarningExportingDirectLinksTitle), culture);

    public static string LoadingQueryResults() => WITResources.Get(nameof (LoadingQueryResults));

    public static string LoadingQueryResults(CultureInfo culture) => WITResources.Get(nameof (LoadingQueryResults), culture);

    public static string LoadingWorkItems() => WITResources.Get(nameof (LoadingWorkItems));

    public static string LoadingWorkItems(CultureInfo culture) => WITResources.Get(nameof (LoadingWorkItems), culture);

    public static string LoadingWorkItemsLoading() => WITResources.Get(nameof (LoadingWorkItemsLoading));

    public static string LoadingWorkItemsLoading(CultureInfo culture) => WITResources.Get(nameof (LoadingWorkItemsLoading), culture);

    public static string SavingWorkItemsLoading() => WITResources.Get(nameof (SavingWorkItemsLoading));

    public static string SavingWorkItemsLoading(CultureInfo culture) => WITResources.Get(nameof (SavingWorkItemsLoading), culture);

    public static string ExportingWorkItemsLoading() => WITResources.Get(nameof (ExportingWorkItemsLoading));

    public static string ExportingWorkItemsLoading(CultureInfo culture) => WITResources.Get(nameof (ExportingWorkItemsLoading), culture);

    public static string SavingWorkItems() => WITResources.Get(nameof (SavingWorkItems));

    public static string SavingWorkItems(CultureInfo culture) => WITResources.Get(nameof (SavingWorkItems), culture);

    public static string ExportingWorkItems() => WITResources.Get(nameof (ExportingWorkItems));

    public static string ExportingWorkItems(CultureInfo culture) => WITResources.Get(nameof (ExportingWorkItems), culture);

    public static string KeyboardShortcutDescription_AssignToMe() => WITResources.Get(nameof (KeyboardShortcutDescription_AssignToMe));

    public static string KeyboardShortcutDescription_AssignToMe(CultureInfo culture) => WITResources.Get(nameof (KeyboardShortcutDescription_AssignToMe), culture);

    public static string KeyboardShortcutDescription_AttachmentsTab() => WITResources.Get(nameof (KeyboardShortcutDescription_AttachmentsTab));

    public static string KeyboardShortcutDescription_AttachmentsTab(CultureInfo culture) => WITResources.Get(nameof (KeyboardShortcutDescription_AttachmentsTab), culture);

    public static string KeyboardShortcutDescription_CopyWorkItemTitle() => WITResources.Get(nameof (KeyboardShortcutDescription_CopyWorkItemTitle));

    public static string KeyboardShortcutDescription_CopyWorkItemTitle(CultureInfo culture) => WITResources.Get(nameof (KeyboardShortcutDescription_CopyWorkItemTitle), culture);

    public static string KeyboardShortcutDescription_Discussion() => WITResources.Get(nameof (KeyboardShortcutDescription_Discussion));

    public static string KeyboardShortcutDescription_Discussion(CultureInfo culture) => WITResources.Get(nameof (KeyboardShortcutDescription_Discussion), culture);

    public static string KeyboardShortcutDescription_HistoryTab() => WITResources.Get(nameof (KeyboardShortcutDescription_HistoryTab));

    public static string KeyboardShortcutDescription_HistoryTab(CultureInfo culture) => WITResources.Get(nameof (KeyboardShortcutDescription_HistoryTab), culture);

    public static string KeyboardShortcutDescription_LeftTab() => WITResources.Get(nameof (KeyboardShortcutDescription_LeftTab));

    public static string KeyboardShortcutDescription_LeftTab(CultureInfo culture) => WITResources.Get(nameof (KeyboardShortcutDescription_LeftTab), culture);

    public static string KeyboardShortcutDescription_LinksTab() => WITResources.Get(nameof (KeyboardShortcutDescription_LinksTab));

    public static string KeyboardShortcutDescription_LinksTab(CultureInfo culture) => WITResources.Get(nameof (KeyboardShortcutDescription_LinksTab), culture);

    public static string KeyboardShortcutDescription_RightTab() => WITResources.Get(nameof (KeyboardShortcutDescription_RightTab));

    public static string KeyboardShortcutDescription_RightTab(CultureInfo culture) => WITResources.Get(nameof (KeyboardShortcutDescription_RightTab), culture);

    public static string KeyboardShortcutDescription_Save() => WITResources.Get(nameof (KeyboardShortcutDescription_Save));

    public static string KeyboardShortcutDescription_Save(CultureInfo culture) => WITResources.Get(nameof (KeyboardShortcutDescription_Save), culture);

    public static string KeyboardShortcutDescription_SaveAndClose() => WITResources.Get(nameof (KeyboardShortcutDescription_SaveAndClose));

    public static string KeyboardShortcutDescription_SaveAndClose(CultureInfo culture) => WITResources.Get(nameof (KeyboardShortcutDescription_SaveAndClose), culture);

    public static string FullscreenEnterIconText() => WITResources.Get(nameof (FullscreenEnterIconText));

    public static string FullscreenEnterIconText(CultureInfo culture) => WITResources.Get(nameof (FullscreenEnterIconText), culture);

    public static string FullscreenExitIconText() => WITResources.Get(nameof (FullscreenExitIconText));

    public static string FullscreenExitIconText(CultureInfo culture) => WITResources.Get(nameof (FullscreenExitIconText), culture);

    public static string KeyboardShortcutGroup_WorkItemForm() => WITResources.Get(nameof (KeyboardShortcutGroup_WorkItemForm));

    public static string KeyboardShortcutGroup_WorkItemForm(CultureInfo culture) => WITResources.Get(nameof (KeyboardShortcutGroup_WorkItemForm), culture);

    public static string TabHeaderTooltip(object arg0) => WITResources.Format(nameof (TabHeaderTooltip), arg0);

    public static string TabHeaderTooltip(object arg0, CultureInfo culture) => WITResources.Format(nameof (TabHeaderTooltip), culture, arg0);

    public static string WorkItemLogControlHistoryHeader() => WITResources.Get(nameof (WorkItemLogControlHistoryHeader));

    public static string WorkItemLogControlHistoryHeader(CultureInfo culture) => WITResources.Get(nameof (WorkItemLogControlHistoryHeader), culture);

    public static string RefreshCharts() => WITResources.Get(nameof (RefreshCharts));

    public static string RefreshCharts(CultureInfo culture) => WITResources.Get(nameof (RefreshCharts), culture);

    public static string RunQuery_PromptUnsavedWorkItemChanges_MessageTitle() => WITResources.Get(nameof (RunQuery_PromptUnsavedWorkItemChanges_MessageTitle));

    public static string RunQuery_PromptUnsavedWorkItemChanges_MessageTitle(CultureInfo culture) => WITResources.Get(nameof (RunQuery_PromptUnsavedWorkItemChanges_MessageTitle), culture);

    public static string RunQuery_PromptUnsavedWorkItemChanges_ProceedButtonText() => WITResources.Get(nameof (RunQuery_PromptUnsavedWorkItemChanges_ProceedButtonText));

    public static string RunQuery_PromptUnsavedWorkItemChanges_ProceedButtonText(CultureInfo culture) => WITResources.Get(nameof (RunQuery_PromptUnsavedWorkItemChanges_ProceedButtonText), culture);

    public static string RunQuery_PromptUnsavedWorkItemChanges_RejectButtonText() => WITResources.Get(nameof (RunQuery_PromptUnsavedWorkItemChanges_RejectButtonText));

    public static string RunQuery_PromptUnsavedWorkItemChanges_RejectButtonText(CultureInfo culture) => WITResources.Get(nameof (RunQuery_PromptUnsavedWorkItemChanges_RejectButtonText), culture);

    public static string RunQuery_PromptUnsavedWorkItemChanges_MessageContentText() => WITResources.Get(nameof (RunQuery_PromptUnsavedWorkItemChanges_MessageContentText));

    public static string RunQuery_PromptUnsavedWorkItemChanges_MessageContentText(
      CultureInfo culture)
    {
      return WITResources.Get(nameof (RunQuery_PromptUnsavedWorkItemChanges_MessageContentText), culture);
    }

    public static string FormGroupCollapseAriaLabel(object arg0) => WITResources.Format(nameof (FormGroupCollapseAriaLabel), arg0);

    public static string FormGroupCollapseAriaLabel(object arg0, CultureInfo culture) => WITResources.Format(nameof (FormGroupCollapseAriaLabel), culture, arg0);

    public static string FormGroupExpandAriaLabel(object arg0) => WITResources.Format(nameof (FormGroupExpandAriaLabel), arg0);

    public static string FormGroupExpandAriaLabel(object arg0, CultureInfo culture) => WITResources.Format(nameof (FormGroupExpandAriaLabel), culture, arg0);

    public static string QueryResultsGridLinkQueryStatusSecondaryTextFormat(
      object arg0,
      object arg1,
      object arg2)
    {
      return WITResources.Format(nameof (QueryResultsGridLinkQueryStatusSecondaryTextFormat), arg0, arg1, arg2);
    }

    public static string QueryResultsGridLinkQueryStatusSecondaryTextFormat(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (QueryResultsGridLinkQueryStatusSecondaryTextFormat), culture, arg0, arg1, arg2);
    }

    public static string QueryResultsGridNumSelectedSecondaryTextFormat(object arg0) => WITResources.Format(nameof (QueryResultsGridNumSelectedSecondaryTextFormat), arg0);

    public static string QueryResultsGridNumSelectedSecondaryTextFormat(
      object arg0,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (QueryResultsGridNumSelectedSecondaryTextFormat), culture, arg0);
    }

    public static string ColumnOptionsPanelText() => WITResources.Get(nameof (ColumnOptionsPanelText));

    public static string ColumnOptionsPanelText(CultureInfo culture) => WITResources.Get(nameof (ColumnOptionsPanelText), culture);

    public static string ColumnOptionsPanelKeyboardShortCutWithControl() => WITResources.Get(nameof (ColumnOptionsPanelKeyboardShortCutWithControl));

    public static string ColumnOptionsPanelKeyboardShortCutWithControl(CultureInfo culture) => WITResources.Get(nameof (ColumnOptionsPanelKeyboardShortCutWithControl), culture);

    public static string ColumnOptionsPanelKeyboardShortCutWithCommand() => WITResources.Get(nameof (ColumnOptionsPanelKeyboardShortCutWithCommand));

    public static string ColumnOptionsPanelKeyboardShortCutWithCommand(CultureInfo culture) => WITResources.Get(nameof (ColumnOptionsPanelKeyboardShortCutWithCommand), culture);

    public static string AddAColumn() => WITResources.Get(nameof (AddAColumn));

    public static string AddAColumn(CultureInfo culture) => WITResources.Get(nameof (AddAColumn), culture);

    public static string AddARollupColumn() => WITResources.Get(nameof (AddARollupColumn));

    public static string AddARollupColumn(CultureInfo culture) => WITResources.Get(nameof (AddARollupColumn), culture);

    public static string AddARollupColumnFromQuicklist() => WITResources.Get(nameof (AddARollupColumnFromQuicklist));

    public static string AddARollupColumnFromQuicklist(CultureInfo culture) => WITResources.Get(nameof (AddARollupColumnFromQuicklist), culture);

    public static string ConfigureCustomRollupColumn() => WITResources.Get(nameof (ConfigureCustomRollupColumn));

    public static string ConfigureCustomRollupColumn(CultureInfo culture) => WITResources.Get(nameof (ConfigureCustomRollupColumn), culture);

    public static string MissingRollupCalculationError() => WITResources.Get(nameof (MissingRollupCalculationError));

    public static string MissingRollupCalculationError(CultureInfo culture) => WITResources.Get(nameof (MissingRollupCalculationError), culture);

    public static string RollupProgressBarOptionsHeadingText() => WITResources.Get(nameof (RollupProgressBarOptionsHeadingText));

    public static string RollupProgressBarOptionsHeadingText(CultureInfo culture) => WITResources.Get(nameof (RollupProgressBarOptionsHeadingText), culture);

    public static string RollupTotalNumberOptionsHeadingText() => WITResources.Get(nameof (RollupTotalNumberOptionsHeadingText));

    public static string RollupTotalNumberOptionsHeadingText(CultureInfo culture) => WITResources.Get(nameof (RollupTotalNumberOptionsHeadingText), culture);

    public static string AdditionalDisplayFieldFocusZoneLabel(object arg0) => WITResources.Format(nameof (AdditionalDisplayFieldFocusZoneLabel), arg0);

    public static string AdditionalDisplayFieldFocusZoneLabel(object arg0, CultureInfo culture) => WITResources.Format(nameof (AdditionalDisplayFieldFocusZoneLabel), culture, arg0);

    public static string RemoveColumn() => WITResources.Get(nameof (RemoveColumn));

    public static string RemoveColumn(CultureInfo culture) => WITResources.Get(nameof (RemoveColumn), culture);

    public static string FieldCannotBeEmpty() => WITResources.Get(nameof (FieldCannotBeEmpty));

    public static string FieldCannotBeEmpty(CultureInfo culture) => WITResources.Get(nameof (FieldCannotBeEmpty), culture);

    public static string FieldCannotBeEdited(object arg0) => WITResources.Format(nameof (FieldCannotBeEdited), arg0);

    public static string FieldCannotBeEdited(object arg0, CultureInfo culture) => WITResources.Format(nameof (FieldCannotBeEdited), culture, arg0);

    public static string FieldCannotBeEditedNorRemoved(object arg0) => WITResources.Format(nameof (FieldCannotBeEditedNorRemoved), arg0);

    public static string FieldCannotBeEditedNorRemoved(object arg0, CultureInfo culture) => WITResources.Format(nameof (FieldCannotBeEditedNorRemoved), culture, arg0);

    public static string FieldDoesNotExist() => WITResources.Get(nameof (FieldDoesNotExist));

    public static string FieldDoesNotExist(CultureInfo culture) => WITResources.Get(nameof (FieldDoesNotExist), culture);

    public static string FieldAlreadyAdded() => WITResources.Get(nameof (FieldAlreadyAdded));

    public static string FieldAlreadyAdded(CultureInfo culture) => WITResources.Get(nameof (FieldAlreadyAdded), culture);

    public static string FieldCannotBeRemoved() => WITResources.Get(nameof (FieldCannotBeRemoved));

    public static string FieldCannotBeRemoved(CultureInfo culture) => WITResources.Get(nameof (FieldCannotBeRemoved), culture);

    public static string PickAColumn() => WITResources.Get(nameof (PickAColumn));

    public static string PickAColumn(CultureInfo culture) => WITResources.Get(nameof (PickAColumn), culture);

    public static string CommandBarTitle_CopyWorkItems() => WITResources.Get(nameof (CommandBarTitle_CopyWorkItems));

    public static string CommandBarTitle_CopyWorkItems(CultureInfo culture) => WITResources.Get(nameof (CommandBarTitle_CopyWorkItems), culture);

    public static string CommandBarTitle_DeleteWorkItems() => WITResources.Get(nameof (CommandBarTitle_DeleteWorkItems));

    public static string CommandBarTitle_DeleteWorkItems(CultureInfo culture) => WITResources.Get(nameof (CommandBarTitle_DeleteWorkItems), culture);

    public static string ColumnOptionsColumns() => WITResources.Get(nameof (ColumnOptionsColumns));

    public static string ColumnOptionsColumns(CultureInfo culture) => WITResources.Get(nameof (ColumnOptionsColumns), culture);

    public static string ColumnOptionsSorting() => WITResources.Get(nameof (ColumnOptionsSorting));

    public static string ColumnOptionsSorting(CultureInfo culture) => WITResources.Get(nameof (ColumnOptionsSorting), culture);

    public static string ColumnOptionsAtLeastOneColumnIsRequired() => WITResources.Get(nameof (ColumnOptionsAtLeastOneColumnIsRequired));

    public static string ColumnOptionsAtLeastOneColumnIsRequired(CultureInfo culture) => WITResources.Get(nameof (ColumnOptionsAtLeastOneColumnIsRequired), culture);

    public static string ColumnOptionsSortAscending() => WITResources.Get(nameof (ColumnOptionsSortAscending));

    public static string ColumnOptionsSortAscending(CultureInfo culture) => WITResources.Get(nameof (ColumnOptionsSortAscending), culture);

    public static string ColumnOptionsSortDescending() => WITResources.Get(nameof (ColumnOptionsSortDescending));

    public static string ColumnOptionsSortDescending(CultureInfo culture) => WITResources.Get(nameof (ColumnOptionsSortDescending), culture);

    public static string Items() => WITResources.Get(nameof (Items));

    public static string Items(CultureInfo culture) => WITResources.Get(nameof (Items), culture);

    public static string WorkItemDiscussionAddWithPRComment() => WITResources.Get(nameof (WorkItemDiscussionAddWithPRComment));

    public static string WorkItemDiscussionAddWithPRComment(CultureInfo culture) => WITResources.Get(nameof (WorkItemDiscussionAddWithPRComment), culture);

    public static string ClosePanel() => WITResources.Get(nameof (ClosePanel));

    public static string ClosePanel(CultureInfo culture) => WITResources.Get(nameof (ClosePanel), culture);

    public static string AdditionalFieldEmptyFocusZoneLabel() => WITResources.Get(nameof (AdditionalFieldEmptyFocusZoneLabel));

    public static string AdditionalFieldEmptyFocusZoneLabel(CultureInfo culture) => WITResources.Get(nameof (AdditionalFieldEmptyFocusZoneLabel), culture);

    public static string AdditionalSortFieldFocusZoneLabel(object arg0, object arg1) => WITResources.Format(nameof (AdditionalSortFieldFocusZoneLabel), arg0, arg1);

    public static string AdditionalSortFieldFocusZoneLabel(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (AdditionalSortFieldFocusZoneLabel), culture, arg0, arg1);
    }

    public static string LinksControlWikiPageText() => WITResources.Get(nameof (LinksControlWikiPageText));

    public static string LinksControlWikiPageText(CultureInfo culture) => WITResources.Get(nameof (LinksControlWikiPageText), culture);

    public static string WorkItemFinderTitleContainsAndType() => WITResources.Get(nameof (WorkItemFinderTitleContainsAndType));

    public static string WorkItemFinderTitleContainsAndType(CultureInfo culture) => WITResources.Get(nameof (WorkItemFinderTitleContainsAndType), culture);

    public static string EditQuery_PromptQueryRevert_MessageContentText() => WITResources.Get(nameof (EditQuery_PromptQueryRevert_MessageContentText));

    public static string EditQuery_PromptQueryRevert_MessageContentText(CultureInfo culture) => WITResources.Get(nameof (EditQuery_PromptQueryRevert_MessageContentText), culture);

    public static string EditQuery_PromptQueryRevert_MessageTitle() => WITResources.Get(nameof (EditQuery_PromptQueryRevert_MessageTitle));

    public static string EditQuery_PromptQueryRevert_MessageTitle(CultureInfo culture) => WITResources.Get(nameof (EditQuery_PromptQueryRevert_MessageTitle), culture);

    public static string WorkItemNotFoundClientException(object arg0) => WITResources.Format(nameof (WorkItemNotFoundClientException), arg0);

    public static string WorkItemNotFoundClientException(object arg0, CultureInfo culture) => WITResources.Format(nameof (WorkItemNotFoundClientException), culture, arg0);

    public static string RefreshHotkey() => WITResources.Get(nameof (RefreshHotkey));

    public static string RefreshHotkey(CultureInfo culture) => WITResources.Get(nameof (RefreshHotkey), culture);

    public static string NewQueryHotkey() => WITResources.Get(nameof (NewQueryHotkey));

    public static string NewQueryHotkey(CultureInfo culture) => WITResources.Get(nameof (NewQueryHotkey), culture);

    public static string WorkItemIsReadOnlyError() => WITResources.Get(nameof (WorkItemIsReadOnlyError));

    public static string WorkItemIsReadOnlyError(CultureInfo culture) => WITResources.Get(nameof (WorkItemIsReadOnlyError), culture);

    public static string NoCommentsText() => WITResources.Get(nameof (NoCommentsText));

    public static string NoCommentsText(CultureInfo culture) => WITResources.Get(nameof (NoCommentsText), culture);

    public static string QueryFavoriteItemAriaLabel(object arg0) => WITResources.Format(nameof (QueryFavoriteItemAriaLabel), arg0);

    public static string QueryFavoriteItemAriaLabel(object arg0, CultureInfo culture) => WITResources.Format(nameof (QueryFavoriteItemAriaLabel), culture, arg0);

    public static string QuerySaveDialog_FolderCombo_ExpandButtonAriaLabel() => WITResources.Get(nameof (QuerySaveDialog_FolderCombo_ExpandButtonAriaLabel));

    public static string QuerySaveDialog_FolderCombo_ExpandButtonAriaLabel(CultureInfo culture) => WITResources.Get(nameof (QuerySaveDialog_FolderCombo_ExpandButtonAriaLabel), culture);

    public static string FilterNoArea() => WITResources.Get(nameof (FilterNoArea));

    public static string FilterNoArea(CultureInfo culture) => WITResources.Get(nameof (FilterNoArea), culture);

    public static string NoValueForMultiLineControl(object arg0) => WITResources.Format(nameof (NoValueForMultiLineControl), arg0);

    public static string NoValueForMultiLineControl(object arg0, CultureInfo culture) => WITResources.Format(nameof (NoValueForMultiLineControl), culture, arg0);

    public static string HtmlFieldPlaceholder(object arg0) => WITResources.Format(nameof (HtmlFieldPlaceholder), arg0);

    public static string HtmlFieldPlaceholder(object arg0, CultureInfo culture) => WITResources.Format(nameof (HtmlFieldPlaceholder), culture, arg0);

    public static string WorkItemDiscussionAddCommentHelpText() => WITResources.Get(nameof (WorkItemDiscussionAddCommentHelpText));

    public static string WorkItemDiscussionAddCommentHelpText(CultureInfo culture) => WITResources.Get(nameof (WorkItemDiscussionAddCommentHelpText), culture);

    public static string WorkItemDiscussionAddWithPRCommentHelpText() => WITResources.Get(nameof (WorkItemDiscussionAddWithPRCommentHelpText));

    public static string WorkItemDiscussionAddWithPRCommentHelpText(CultureInfo culture) => WITResources.Get(nameof (WorkItemDiscussionAddWithPRCommentHelpText), culture);

    public static string HistoryControlTooltipAdditionalTextMessage() => WITResources.Get(nameof (HistoryControlTooltipAdditionalTextMessage));

    public static string HistoryControlTooltipAdditionalTextMessage(CultureInfo culture) => WITResources.Get(nameof (HistoryControlTooltipAdditionalTextMessage), culture);

    public static string ReadonlyWorkItemSaveButtonTooltip() => WITResources.Get(nameof (ReadonlyWorkItemSaveButtonTooltip));

    public static string ReadonlyWorkItemSaveButtonTooltip(CultureInfo culture) => WITResources.Get(nameof (ReadonlyWorkItemSaveButtonTooltip), culture);

    public static string LoadingWorkItem() => WITResources.Get(nameof (LoadingWorkItem));

    public static string LoadingWorkItem(CultureInfo culture) => WITResources.Get(nameof (LoadingWorkItem), culture);

    public static string LinkDialogWorkItemPickerInputWatermark() => WITResources.Get(nameof (LinkDialogWorkItemPickerInputWatermark));

    public static string LinkDialogWorkItemPickerInputWatermark(CultureInfo culture) => WITResources.Get(nameof (LinkDialogWorkItemPickerInputWatermark), culture);

    public static string LinkDialog_CircularWorkItemRelationshipError(
      object arg0,
      object arg1,
      object arg2)
    {
      return WITResources.Format(nameof (LinkDialog_CircularWorkItemRelationshipError), arg0, arg1, arg2);
    }

    public static string LinkDialog_CircularWorkItemRelationshipError(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (LinkDialog_CircularWorkItemRelationshipError), culture, arg0, arg1, arg2);
    }

    public static string LinkDialog_LinkChildWorkItemWhichHasAParentError(object arg0, object arg1) => WITResources.Format(nameof (LinkDialog_LinkChildWorkItemWhichHasAParentError), arg0, arg1);

    public static string LinkDialog_LinkChildWorkItemWhichHasAParentError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (LinkDialog_LinkChildWorkItemWhichHasAParentError), culture, arg0, arg1);
    }

    public static string LinkDialog_LinkDuplicateWorkItemsError(object arg0, object arg1) => WITResources.Format(nameof (LinkDialog_LinkDuplicateWorkItemsError), arg0, arg1);

    public static string LinkDialog_LinkDuplicateWorkItemsError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (LinkDialog_LinkDuplicateWorkItemsError), culture, arg0, arg1);
    }

    public static string LinkDialog_QueuedLinkDuplicateWorkItemsError(object arg0) => WITResources.Format(nameof (LinkDialog_QueuedLinkDuplicateWorkItemsError), arg0);

    public static string LinkDialog_QueuedLinkDuplicateWorkItemsError(
      object arg0,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (LinkDialog_QueuedLinkDuplicateWorkItemsError), culture, arg0);
    }

    public static string LinkDialog_NewWorkItem_SameTypeHierarchyError(object arg0, object arg1) => WITResources.Format(nameof (LinkDialog_NewWorkItem_SameTypeHierarchyError), arg0, arg1);

    public static string LinkDialog_NewWorkItem_SameTypeHierarchyError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (LinkDialog_NewWorkItem_SameTypeHierarchyError), culture, arg0, arg1);
    }

    public static string LinkDialog_ExistingWorkItem_SameTypeHierarchyError(
      object arg0,
      object arg1)
    {
      return WITResources.Format(nameof (LinkDialog_ExistingWorkItem_SameTypeHierarchyError), arg0, arg1);
    }

    public static string LinkDialog_ExistingWorkItem_SameTypeHierarchyError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (LinkDialog_ExistingWorkItem_SameTypeHierarchyError), culture, arg0, arg1);
    }

    public static string LinkDialog_SameTypeHierarchyError_LearnMore() => WITResources.Get(nameof (LinkDialog_SameTypeHierarchyError_LearnMore));

    public static string LinkDialog_SameTypeHierarchyError_LearnMore(CultureInfo culture) => WITResources.Get(nameof (LinkDialog_SameTypeHierarchyError_LearnMore), culture);

    public static string LinkDialog_LinkMultipleWorkItemsAsParentsError() => WITResources.Get(nameof (LinkDialog_LinkMultipleWorkItemsAsParentsError));

    public static string LinkDialog_LinkMultipleWorkItemsAsParentsError(CultureInfo culture) => WITResources.Get(nameof (LinkDialog_LinkMultipleWorkItemsAsParentsError), culture);

    public static string LinkDialog_LinkSelfWorkItemError(object arg0) => WITResources.Format(nameof (LinkDialog_LinkSelfWorkItemError), arg0);

    public static string LinkDialog_LinkSelfWorkItemError(object arg0, CultureInfo culture) => WITResources.Format(nameof (LinkDialog_LinkSelfWorkItemError), culture, arg0);

    public static string LinkDialog_LinkUnsupportWorkItemTypeError(object arg0, object arg1) => WITResources.Format(nameof (LinkDialog_LinkUnsupportWorkItemTypeError), arg0, arg1);

    public static string LinkDialog_LinkUnsupportWorkItemTypeError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (LinkDialog_LinkUnsupportWorkItemTypeError), culture, arg0, arg1);
    }

    public static string LinkDialog_WorkItemNotFoundError(object arg0) => WITResources.Format(nameof (LinkDialog_WorkItemNotFoundError), arg0);

    public static string LinkDialog_WorkItemNotFoundError(object arg0, CultureInfo culture) => WITResources.Format(nameof (LinkDialog_WorkItemNotFoundError), culture, arg0);

    public static string BeginGetWorkItemArgumentNull() => WITResources.Get(nameof (BeginGetWorkItemArgumentNull));

    public static string BeginGetWorkItemArgumentNull(CultureInfo culture) => WITResources.Get(nameof (BeginGetWorkItemArgumentNull), culture);

    public static string WorkItemIdNaN() => WITResources.Get(nameof (WorkItemIdNaN));

    public static string WorkItemIdNaN(CultureInfo culture) => WITResources.Get(nameof (WorkItemIdNaN), culture);

    public static string WorkItemIdOutOfRange() => WITResources.Get(nameof (WorkItemIdOutOfRange));

    public static string WorkItemIdOutOfRange(CultureInfo culture) => WITResources.Get(nameof (WorkItemIdOutOfRange), culture);

    public static string WorkItemIdUndefined() => WITResources.Get(nameof (WorkItemIdUndefined));

    public static string WorkItemIdUndefined(CultureInfo culture) => WITResources.Get(nameof (WorkItemIdUndefined), culture);

    public static string WorkItemTemplateDialog_TeamName() => WITResources.Get(nameof (WorkItemTemplateDialog_TeamName));

    public static string WorkItemTemplateDialog_TeamName(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplateDialog_TeamName), culture);

    public static string WorkItemTemplateDialog_NoMemberTeamsError() => WITResources.Get(nameof (WorkItemTemplateDialog_NoMemberTeamsError));

    public static string WorkItemTemplateDialog_NoMemberTeamsError(CultureInfo culture) => WITResources.Get(nameof (WorkItemTemplateDialog_NoMemberTeamsError), culture);

    public static string CopyWorkItemToPublicProjectMessage() => WITResources.Get(nameof (CopyWorkItemToPublicProjectMessage));

    public static string CopyWorkItemToPublicProjectMessage(CultureInfo culture) => WITResources.Get(nameof (CopyWorkItemToPublicProjectMessage), culture);

    public static string MoveWorkItemToPublicProjectMessage() => WITResources.Get(nameof (MoveWorkItemToPublicProjectMessage));

    public static string MoveWorkItemToPublicProjectMessage(CultureInfo culture) => WITResources.Get(nameof (MoveWorkItemToPublicProjectMessage), culture);

    public static string WorkItemMoveCopyPublicVisibilityLearnMoreLinkText() => WITResources.Get(nameof (WorkItemMoveCopyPublicVisibilityLearnMoreLinkText));

    public static string WorkItemMoveCopyPublicVisibilityLearnMoreLinkText(CultureInfo culture) => WITResources.Get(nameof (WorkItemMoveCopyPublicVisibilityLearnMoreLinkText), culture);

    public static string WorkItemPublicVisibilityLearnMoreLink() => WITResources.Get(nameof (WorkItemPublicVisibilityLearnMoreLink));

    public static string WorkItemPublicVisibilityLearnMoreLink(CultureInfo culture) => WITResources.Get(nameof (WorkItemPublicVisibilityLearnMoreLink), culture);

    public static string EmailWorkItems_NoSelection() => WITResources.Get(nameof (EmailWorkItems_NoSelection));

    public static string EmailWorkItems_NoSelection(CultureInfo culture) => WITResources.Get(nameof (EmailWorkItems_NoSelection), culture);

    public static string BrowseAllQueries() => WITResources.Get(nameof (BrowseAllQueries));

    public static string BrowseAllQueries(CultureInfo culture) => WITResources.Get(nameof (BrowseAllQueries), culture);

    public static string QueriesHub_NewFolderCreated_LinkText() => WITResources.Get(nameof (QueriesHub_NewFolderCreated_LinkText));

    public static string QueriesHub_NewFolderCreated_LinkText(CultureInfo culture) => WITResources.Get(nameof (QueriesHub_NewFolderCreated_LinkText), culture);

    public static string QueriesHub_NewFolderCreated_MessageText(object arg0) => WITResources.Format(nameof (QueriesHub_NewFolderCreated_MessageText), arg0);

    public static string QueriesHub_NewFolderCreated_MessageText(object arg0, CultureInfo culture) => WITResources.Format(nameof (QueriesHub_NewFolderCreated_MessageText), culture, arg0);

    public static string NoSuggestionsFound() => WITResources.Get(nameof (NoSuggestionsFound));

    public static string NoSuggestionsFound(CultureInfo culture) => WITResources.Get(nameof (NoSuggestionsFound), culture);

    public static string LinksControlGitHubCommitText() => WITResources.Get(nameof (LinksControlGitHubCommitText));

    public static string LinksControlGitHubCommitText(CultureInfo culture) => WITResources.Get(nameof (LinksControlGitHubCommitText), culture);

    public static string LinksControlGitHubPullRequestText() => WITResources.Get(nameof (LinksControlGitHubPullRequestText));

    public static string LinksControlGitHubPullRequestText(CultureInfo culture) => WITResources.Get(nameof (LinksControlGitHubPullRequestText), culture);

    public static string LinksControlGitHubIssueText() => WITResources.Get(nameof (LinksControlGitHubIssueText));

    public static string LinksControlGitHubIssueText(CultureInfo culture) => WITResources.Get(nameof (LinksControlGitHubIssueText), culture);

    public static string LinkToolTypeGitHub() => WITResources.Get(nameof (LinkToolTypeGitHub));

    public static string LinkToolTypeGitHub(CultureInfo culture) => WITResources.Get(nameof (LinkToolTypeGitHub), culture);

    public static string LinksControlGitHubHelpText_about() => WITResources.Get(nameof (LinksControlGitHubHelpText_about));

    public static string LinksControlGitHubHelpText_about(CultureInfo culture) => WITResources.Get(nameof (LinksControlGitHubHelpText_about), culture);

    public static string GitHubInputNotValid(object arg0) => WITResources.Format(nameof (GitHubInputNotValid), arg0);

    public static string GitHubInputNotValid(object arg0, CultureInfo culture) => WITResources.Format(nameof (GitHubInputNotValid), culture, arg0);

    public static string GithubPRLinkDialogAddressTitle() => WITResources.Get(nameof (GithubPRLinkDialogAddressTitle));

    public static string GithubPRLinkDialogAddressTitle(CultureInfo culture) => WITResources.Get(nameof (GithubPRLinkDialogAddressTitle), culture);

    public static string GithubPRLinkDialogAddressTitleWatermark() => WITResources.Get(nameof (GithubPRLinkDialogAddressTitleWatermark));

    public static string GithubPRLinkDialogAddressTitleWatermark(CultureInfo culture) => WITResources.Get(nameof (GithubPRLinkDialogAddressTitleWatermark), culture);

    public static string GithubCommitLinkDialogAddressTitle() => WITResources.Get(nameof (GithubCommitLinkDialogAddressTitle));

    public static string GithubCommitLinkDialogAddressTitle(CultureInfo culture) => WITResources.Get(nameof (GithubCommitLinkDialogAddressTitle), culture);

    public static string GithubCommitLinkDialogAddressTitleWatermark() => WITResources.Get(nameof (GithubCommitLinkDialogAddressTitleWatermark));

    public static string GithubCommitLinkDialogAddressTitleWatermark(CultureInfo culture) => WITResources.Get(nameof (GithubCommitLinkDialogAddressTitleWatermark), culture);

    public static string GithubIssueLinkDialogAddressTitle() => WITResources.Get(nameof (GithubIssueLinkDialogAddressTitle));

    public static string GithubIssueLinkDialogAddressTitle(CultureInfo culture) => WITResources.Get(nameof (GithubIssueLinkDialogAddressTitle), culture);

    public static string GithubIssueLinkDialogAddressTitleWatermark() => WITResources.Get(nameof (GithubIssueLinkDialogAddressTitleWatermark));

    public static string GithubIssueLinkDialogAddressTitleWatermark(CultureInfo culture) => WITResources.Get(nameof (GithubIssueLinkDialogAddressTitleWatermark), culture);

    public static string CannotResolveExternalLinkResource(object arg0) => WITResources.Format(nameof (CannotResolveExternalLinkResource), arg0);

    public static string CannotResolveExternalLinkResource(object arg0, CultureInfo culture) => WITResources.Format(nameof (CannotResolveExternalLinkResource), culture, arg0);

    public static string WorkItemArtifactLinkGitHubCannotRender() => WITResources.Get(nameof (WorkItemArtifactLinkGitHubCannotRender));

    public static string WorkItemArtifactLinkGitHubCannotRender(CultureInfo culture) => WITResources.Get(nameof (WorkItemArtifactLinkGitHubCannotRender), culture);

    public static string GitHubLinkAlreadyExists(object arg0) => WITResources.Format(nameof (GitHubLinkAlreadyExists), arg0);

    public static string GitHubLinkAlreadyExists(object arg0, CultureInfo culture) => WITResources.Format(nameof (GitHubLinkAlreadyExists), culture, arg0);

    public static string Update() => WITResources.Get(nameof (Update));

    public static string Update(CultureInfo culture) => WITResources.Get(nameof (Update), culture);

    public static string ConfirmDeleteComment() => WITResources.Get(nameof (ConfirmDeleteComment));

    public static string ConfirmDeleteComment(CultureInfo culture) => WITResources.Get(nameof (ConfirmDeleteComment), culture);

    public static string Edit() => WITResources.Get(nameof (Edit));

    public static string Edit(CultureInfo culture) => WITResources.Get(nameof (Edit), culture);

    public static string Edited() => WITResources.Get(nameof (Edited));

    public static string Edited(CultureInfo culture) => WITResources.Get(nameof (Edited), culture);

    public static string CommentedFormat(object arg0) => WITResources.Format(nameof (CommentedFormat), arg0);

    public static string CommentedFormat(object arg0, CultureInfo culture) => WITResources.Format(nameof (CommentedFormat), culture, arg0);

    public static string CommentEditedFormat(object arg0) => WITResources.Format(nameof (CommentEditedFormat), arg0);

    public static string CommentEditedFormat(object arg0, CultureInfo culture) => WITResources.Format(nameof (CommentEditedFormat), culture, arg0);

    public static string HistoryOutdatedWithUnsavedChangesWarning() => WITResources.Get(nameof (HistoryOutdatedWithUnsavedChangesWarning));

    public static string HistoryOutdatedWithUnsavedChangesWarning(CultureInfo culture) => WITResources.Get(nameof (HistoryOutdatedWithUnsavedChangesWarning), culture);

    public static string HistoryOutdatedFromUnsavedChangesWarning() => WITResources.Get(nameof (HistoryOutdatedFromUnsavedChangesWarning));

    public static string HistoryOutdatedFromUnsavedChangesWarning(CultureInfo culture) => WITResources.Get(nameof (HistoryOutdatedFromUnsavedChangesWarning), culture);

    public static string ConfirmDiscardCommentDraft() => WITResources.Get(nameof (ConfirmDiscardCommentDraft));

    public static string ConfirmDiscardCommentDraft(CultureInfo culture) => WITResources.Get(nameof (ConfirmDiscardCommentDraft), culture);

    public static string CommentEditedTag() => WITResources.Get(nameof (CommentEditedTag));

    public static string CommentEditedTag(CultureInfo culture) => WITResources.Get(nameof (CommentEditedTag), culture);

    public static string CommentEditedTooltip(object arg0) => WITResources.Format(nameof (CommentEditedTooltip), arg0);

    public static string CommentEditedTooltip(object arg0, CultureInfo culture) => WITResources.Format(nameof (CommentEditedTooltip), culture, arg0);

    public static string CommentedJustNow() => WITResources.Get(nameof (CommentedJustNow));

    public static string CommentedJustNow(CultureInfo culture) => WITResources.Get(nameof (CommentedJustNow), culture);

    public static string GitHubPromoteZeroDataText1() => WITResources.Get(nameof (GitHubPromoteZeroDataText1));

    public static string GitHubPromoteZeroDataText1(CultureInfo culture) => WITResources.Get(nameof (GitHubPromoteZeroDataText1), culture);

    public static string GitHubPromoteZeroDataText2() => WITResources.Get(nameof (GitHubPromoteZeroDataText2));

    public static string GitHubPromoteZeroDataText2(CultureInfo culture) => WITResources.Get(nameof (GitHubPromoteZeroDataText2), culture);

    public static string AzureReposZeroDataText1() => WITResources.Get(nameof (AzureReposZeroDataText1));

    public static string AzureReposZeroDataText1(CultureInfo culture) => WITResources.Get(nameof (AzureReposZeroDataText1), culture);

    public static string AzureReposZeroDataText2() => WITResources.Get(nameof (AzureReposZeroDataText2));

    public static string AzureReposZeroDataText2(CultureInfo culture) => WITResources.Get(nameof (AzureReposZeroDataText2), culture);

    public static string AzureReposZeroDataText3() => WITResources.Get(nameof (AzureReposZeroDataText3));

    public static string AzureReposZeroDataText3(CultureInfo culture) => WITResources.Get(nameof (AzureReposZeroDataText3), culture);

    public static string CreateaBranchText() => WITResources.Get(nameof (CreateaBranchText));

    public static string CreateaBranchText(CultureInfo culture) => WITResources.Get(nameof (CreateaBranchText), culture);

    public static string LinksControlZeroDataBranchText() => WITResources.Get(nameof (LinksControlZeroDataBranchText));

    public static string LinksControlZeroDataBranchText(CultureInfo culture) => WITResources.Get(nameof (LinksControlZeroDataBranchText), culture);

    public static string GitHubZeroDataText1() => WITResources.Get(nameof (GitHubZeroDataText1));

    public static string GitHubZeroDataText1(CultureInfo culture) => WITResources.Get(nameof (GitHubZeroDataText1), culture);

    public static string GitHubZeroDataText2() => WITResources.Get(nameof (GitHubZeroDataText2));

    public static string GitHubZeroDataText2(CultureInfo culture) => WITResources.Get(nameof (GitHubZeroDataText2), culture);

    public static string GitHubZeroDataText3() => WITResources.Get(nameof (GitHubZeroDataText3));

    public static string GitHubZeroDataText3(CultureInfo culture) => WITResources.Get(nameof (GitHubZeroDataText3), culture);

    public static string GitHubZeroDataCommitLinkTypeText() => WITResources.Get(nameof (GitHubZeroDataCommitLinkTypeText));

    public static string GitHubZeroDataCommitLinkTypeText(CultureInfo culture) => WITResources.Get(nameof (GitHubZeroDataCommitLinkTypeText), culture);

    public static string GitHubZeroDataPullRequestTypeText() => WITResources.Get(nameof (GitHubZeroDataPullRequestTypeText));

    public static string GitHubZeroDataPullRequestTypeText(CultureInfo culture) => WITResources.Get(nameof (GitHubZeroDataPullRequestTypeText), culture);

    public static string UnsavedCommentPrompt() => WITResources.Get(nameof (UnsavedCommentPrompt));

    public static string UnsavedCommentPrompt(CultureInfo culture) => WITResources.Get(nameof (UnsavedCommentPrompt), culture);

    public static string UnsavedComment() => WITResources.Get(nameof (UnsavedComment));

    public static string UnsavedComment(CultureInfo culture) => WITResources.Get(nameof (UnsavedComment), culture);

    public static string WithUnsavedComment(object arg0) => WITResources.Format(nameof (WithUnsavedComment), arg0);

    public static string WithUnsavedComment(object arg0, CultureInfo culture) => WITResources.Format(nameof (WithUnsavedComment), culture, arg0);

    public static string AddYourReaction() => WITResources.Get(nameof (AddYourReaction));

    public static string AddYourReaction(CultureInfo culture) => WITResources.Get(nameof (AddYourReaction), culture);

    public static string ReactionTypeEmojiConfoundedFace() => WITResources.Get(nameof (ReactionTypeEmojiConfoundedFace));

    public static string ReactionTypeEmojiConfoundedFace(CultureInfo culture) => WITResources.Get(nameof (ReactionTypeEmojiConfoundedFace), culture);

    public static string ReactionTypeEmojiPartyPopper() => WITResources.Get(nameof (ReactionTypeEmojiPartyPopper));

    public static string ReactionTypeEmojiPartyPopper(CultureInfo culture) => WITResources.Get(nameof (ReactionTypeEmojiPartyPopper), culture);

    public static string ReactionTypeEmojiRedHeart() => WITResources.Get(nameof (ReactionTypeEmojiRedHeart));

    public static string ReactionTypeEmojiRedHeart(CultureInfo culture) => WITResources.Get(nameof (ReactionTypeEmojiRedHeart), culture);

    public static string ReactionTypeEmojiSmilingFace() => WITResources.Get(nameof (ReactionTypeEmojiSmilingFace));

    public static string ReactionTypeEmojiSmilingFace(CultureInfo culture) => WITResources.Get(nameof (ReactionTypeEmojiSmilingFace), culture);

    public static string ReactionTypeEmojiThumbsDown() => WITResources.Get(nameof (ReactionTypeEmojiThumbsDown));

    public static string ReactionTypeEmojiThumbsDown(CultureInfo culture) => WITResources.Get(nameof (ReactionTypeEmojiThumbsDown), culture);

    public static string ReactionTypeEmojiThumbsUp() => WITResources.Get(nameof (ReactionTypeEmojiThumbsUp));

    public static string ReactionTypeEmojiThumbsUp(CultureInfo culture) => WITResources.Get(nameof (ReactionTypeEmojiThumbsUp), culture);

    public static string ShowMoreComments() => WITResources.Get(nameof (ShowMoreComments));

    public static string ShowMoreComments(CultureInfo culture) => WITResources.Get(nameof (ShowMoreComments), culture);

    public static string HistoryControlCommentDeletedAdornmentText() => WITResources.Get(nameof (HistoryControlCommentDeletedAdornmentText));

    public static string HistoryControlCommentDeletedAdornmentText(CultureInfo culture) => WITResources.Get(nameof (HistoryControlCommentDeletedAdornmentText), culture);

    public static string HistoryControlCommentEditedAdornmentText() => WITResources.Get(nameof (HistoryControlCommentEditedAdornmentText));

    public static string HistoryControlCommentEditedAdornmentText(CultureInfo culture) => WITResources.Get(nameof (HistoryControlCommentEditedAdornmentText), culture);

    public static string QuoteReply() => WITResources.Get(nameof (QuoteReply));

    public static string QuoteReply(CultureInfo culture) => WITResources.Get(nameof (QuoteReply), culture);

    public static string ExportingCsvProgressButtonText() => WITResources.Get(nameof (ExportingCsvProgressButtonText));

    public static string ExportingCsvProgressButtonText(CultureInfo culture) => WITResources.Get(nameof (ExportingCsvProgressButtonText), culture);

    public static string ExportToCsvNoQueryResultsMessage() => WITResources.Get(nameof (ExportToCsvNoQueryResultsMessage));

    public static string ExportToCsvNoQueryResultsMessage(CultureInfo culture) => WITResources.Get(nameof (ExportToCsvNoQueryResultsMessage), culture);

    public static string ExportToCsvQueryResultTooBig(object arg0) => WITResources.Format(nameof (ExportToCsvQueryResultTooBig), arg0);

    public static string ExportToCsvQueryResultTooBig(object arg0, CultureInfo culture) => WITResources.Format(nameof (ExportToCsvQueryResultTooBig), culture, arg0);

    public static string ImporCsvEmptyDatasetError() => WITResources.Get(nameof (ImporCsvEmptyDatasetError));

    public static string ImporCsvEmptyDatasetError(CultureInfo culture) => WITResources.Get(nameof (ImporCsvEmptyDatasetError), culture);

    public static string ImporCsvExtraFieldsFoundError(object arg0, object arg1) => WITResources.Format(nameof (ImporCsvExtraFieldsFoundError), arg0, arg1);

    public static string ImporCsvExtraFieldsFoundError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (ImporCsvExtraFieldsFoundError), culture, arg0, arg1);
    }

    public static string ImporCsvDoubleQuoteTerminatedEarlyError(object arg0, object arg1) => WITResources.Format(nameof (ImporCsvDoubleQuoteTerminatedEarlyError), arg0, arg1);

    public static string ImporCsvDoubleQuoteTerminatedEarlyError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (ImporCsvDoubleQuoteTerminatedEarlyError), culture, arg0, arg1);
    }

    public static string ImporCsvDoubleQuoteNotEnclosedError(object arg0, object arg1) => WITResources.Format(nameof (ImporCsvDoubleQuoteNotEnclosedError), arg0, arg1);

    public static string ImporCsvDoubleQuoteNotEnclosedError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (ImporCsvDoubleQuoteNotEnclosedError), culture, arg0, arg1);
    }

    public static string ImporCsvNoTerminatingDoubleQuotesError(object arg0, object arg1) => WITResources.Format(nameof (ImporCsvNoTerminatingDoubleQuotesError), arg0, arg1);

    public static string ImporCsvNoTerminatingDoubleQuotesError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (ImporCsvNoTerminatingDoubleQuotesError), culture, arg0, arg1);
    }

    public static string ImporCsvMaxRowsExceededError(object arg0) => WITResources.Format(nameof (ImporCsvMaxRowsExceededError), arg0);

    public static string ImporCsvMaxRowsExceededError(object arg0, CultureInfo culture) => WITResources.Format(nameof (ImporCsvMaxRowsExceededError), culture, arg0);

    public static string ImporCsvEmptyFieldValuesError(object arg0, object arg1) => WITResources.Format(nameof (ImporCsvEmptyFieldValuesError), arg0, arg1);

    public static string ImporCsvEmptyFieldValuesError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (ImporCsvEmptyFieldValuesError), culture, arg0, arg1);
    }

    public static string ImporCsvExcessFieldValuesError(object arg0, object arg1) => WITResources.Format(nameof (ImporCsvExcessFieldValuesError), arg0, arg1);

    public static string ImporCsvExcessFieldValuesError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (ImporCsvExcessFieldValuesError), culture, arg0, arg1);
    }

    public static string ImporCsvIncorrectTreeStructureError(object arg0, object arg1) => WITResources.Format(nameof (ImporCsvIncorrectTreeStructureError), arg0, arg1);

    public static string ImporCsvIncorrectTreeStructureError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (ImporCsvIncorrectTreeStructureError), culture, arg0, arg1);
    }

    public static string ImporCsvNonIntegerWorkItemIdError(object arg0, object arg1, object arg2) => WITResources.Format(nameof (ImporCsvNonIntegerWorkItemIdError), arg0, arg1, arg2);

    public static string ImporCsvNonIntegerWorkItemIdError(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (ImporCsvNonIntegerWorkItemIdError), culture, arg0, arg1, arg2);
    }

    public static string ImporCsvInvalidWorkItemIdError(object arg0, object arg1, object arg2) => WITResources.Format(nameof (ImporCsvInvalidWorkItemIdError), arg0, arg1, arg2);

    public static string ImporCsvInvalidWorkItemIdError(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (ImporCsvInvalidWorkItemIdError), culture, arg0, arg1, arg2);
    }

    public static string ImporCsvInvalidFieldValueError(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return WITResources.Format(nameof (ImporCsvInvalidFieldValueError), arg0, arg1, arg2, arg3);
    }

    public static string ImporCsvInvalidFieldValueError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (ImporCsvInvalidFieldValueError), culture, arg0, arg1, arg2, arg3);
    }

    public static string ImporCsvDuplicateWorkItemIdColumnError(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return WITResources.Format(nameof (ImporCsvDuplicateWorkItemIdColumnError), arg0, arg1, arg2, arg3);
    }

    public static string ImporCsvDuplicateWorkItemIdColumnError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (ImporCsvDuplicateWorkItemIdColumnError), culture, arg0, arg1, arg2, arg3);
    }

    public static string ImporCsvErrorsFound(object arg0) => WITResources.Format(nameof (ImporCsvErrorsFound), arg0);

    public static string ImporCsvErrorsFound(object arg0, CultureInfo culture) => WITResources.Format(nameof (ImporCsvErrorsFound), culture, arg0);

    public static string ImporCsvReadonlyFieldModifiedError(object arg0) => WITResources.Format(nameof (ImporCsvReadonlyFieldModifiedError), arg0);

    public static string ImporCsvReadonlyFieldModifiedError(object arg0, CultureInfo culture) => WITResources.Format(nameof (ImporCsvReadonlyFieldModifiedError), culture, arg0);

    public static string AddReaction(object arg0) => WITResources.Format(nameof (AddReaction), arg0);

    public static string AddReaction(object arg0, CultureInfo culture) => WITResources.Format(nameof (AddReaction), culture, arg0);

    public static string ReactionAdditionalUsersCount(object arg0) => WITResources.Format(nameof (ReactionAdditionalUsersCount), arg0);

    public static string ReactionAdditionalUsersCount(object arg0, CultureInfo culture) => WITResources.Format(nameof (ReactionAdditionalUsersCount), culture, arg0);

    public static string ReactionUserListHeaderPlural(object arg0) => WITResources.Format(nameof (ReactionUserListHeaderPlural), arg0);

    public static string ReactionUserListHeaderPlural(object arg0, CultureInfo culture) => WITResources.Format(nameof (ReactionUserListHeaderPlural), culture, arg0);

    public static string ReactionUserListHeaderSingular(object arg0) => WITResources.Format(nameof (ReactionUserListHeaderSingular), arg0);

    public static string ReactionUserListHeaderSingular(object arg0, CultureInfo culture) => WITResources.Format(nameof (ReactionUserListHeaderSingular), culture, arg0);

    public static string RemoveReaction(object arg0) => WITResources.Format(nameof (RemoveReaction), arg0);

    public static string RemoveReaction(object arg0, CultureInfo culture) => WITResources.Format(nameof (RemoveReaction), culture, arg0);

    public static string DiscardChanges() => WITResources.Get(nameof (DiscardChanges));

    public static string DiscardChanges(CultureInfo culture) => WITResources.Get(nameof (DiscardChanges), culture);

    public static string SaveChanges() => WITResources.Get(nameof (SaveChanges));

    public static string SaveChanges(CultureInfo culture) => WITResources.Get(nameof (SaveChanges), culture);

    public static string MVCActionToDpIdMappingNotFoundError(object arg0) => WITResources.Format(nameof (MVCActionToDpIdMappingNotFoundError), arg0);

    public static string MVCActionToDpIdMappingNotFoundError(object arg0, CultureInfo culture) => WITResources.Format(nameof (MVCActionToDpIdMappingNotFoundError), culture, arg0);

    public static string WorkItemFinderResetButtonText() => WITResources.Get(nameof (WorkItemFinderResetButtonText));

    public static string WorkItemFinderResetButtonText(CultureInfo culture) => WITResources.Get(nameof (WorkItemFinderResetButtonText), culture);

    public static string CreateCopyCopiedWithAllAttachmentsFrom(object arg0) => WITResources.Format(nameof (CreateCopyCopiedWithAllAttachmentsFrom), arg0);

    public static string CreateCopyCopiedWithAllAttachmentsFrom(object arg0, CultureInfo culture) => WITResources.Format(nameof (CreateCopyCopiedWithAllAttachmentsFrom), culture, arg0);

    public static string CreateCopyCopiedWithAllLinksAndAttachmentsFrom(object arg0) => WITResources.Format(nameof (CreateCopyCopiedWithAllLinksAndAttachmentsFrom), arg0);

    public static string CreateCopyCopiedWithAllLinksAndAttachmentsFrom(
      object arg0,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (CreateCopyCopiedWithAllLinksAndAttachmentsFrom), culture, arg0);
    }

    public static string CreateCopyOfWorkItem_IncludeAttachments() => WITResources.Get(nameof (CreateCopyOfWorkItem_IncludeAttachments));

    public static string CreateCopyOfWorkItem_IncludeAttachments(CultureInfo culture) => WITResources.Get(nameof (CreateCopyOfWorkItem_IncludeAttachments), culture);

    public static string CreateCopyOfWorkItem_IncludeChildren() => WITResources.Get(nameof (CreateCopyOfWorkItem_IncludeChildren));

    public static string CreateCopyOfWorkItem_IncludeChildren(CultureInfo culture) => WITResources.Get(nameof (CreateCopyOfWorkItem_IncludeChildren), culture);

    public static string CreateCopyOfWorkItem_CopiedWorkItemSuffix() => WITResources.Get(nameof (CreateCopyOfWorkItem_CopiedWorkItemSuffix));

    public static string CreateCopyOfWorkItem_CopiedWorkItemSuffix(CultureInfo culture) => WITResources.Get(nameof (CreateCopyOfWorkItem_CopiedWorkItemSuffix), culture);

    public static string CreateCopyOfWorkItem_Failed() => WITResources.Get(nameof (CreateCopyOfWorkItem_Failed));

    public static string CreateCopyOfWorkItem_Failed(CultureInfo culture) => WITResources.Get(nameof (CreateCopyOfWorkItem_Failed), culture);

    public static string TooManyChildItemsToCopy(object arg0) => WITResources.Format(nameof (TooManyChildItemsToCopy), arg0);

    public static string TooManyChildItemsToCopy(object arg0, CultureInfo culture) => WITResources.Format(nameof (TooManyChildItemsToCopy), culture, arg0);

    public static string ErrorCopyingChildItem(object arg0, object arg1) => WITResources.Format(nameof (ErrorCopyingChildItem), arg0, arg1);

    public static string ErrorCopyingChildItem(object arg0, object arg1, CultureInfo culture) => WITResources.Format(nameof (ErrorCopyingChildItem), culture, arg0, arg1);

    public static string UnsavedWorkItemActionsMessage() => WITResources.Get(nameof (UnsavedWorkItemActionsMessage));

    public static string UnsavedWorkItemActionsMessage(CultureInfo culture) => WITResources.Get(nameof (UnsavedWorkItemActionsMessage), culture);

    public static string ProgressPercentage(object arg0) => WITResources.Format(nameof (ProgressPercentage), arg0);

    public static string ProgressPercentage(object arg0, CultureInfo culture) => WITResources.Format(nameof (ProgressPercentage), culture, arg0);

    public static string ProgressBarTooltip(object arg0, object arg1, object arg2) => WITResources.Format(nameof (ProgressBarTooltip), arg0, arg1, arg2);

    public static string ProgressBarTooltip(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (ProgressBarTooltip), culture, arg0, arg1, arg2);
    }

    public static string RollupTotalByTooltip(object arg0, object arg1) => WITResources.Format(nameof (RollupTotalByTooltip), arg0, arg1);

    public static string RollupTotalByTooltip(object arg0, object arg1, CultureInfo culture) => WITResources.Format(nameof (RollupTotalByTooltip), culture, arg0, arg1);

    public static string CannotFindRollupData() => WITResources.Get(nameof (CannotFindRollupData));

    public static string CannotFindRollupData(CultureInfo culture) => WITResources.Get(nameof (CannotFindRollupData), culture);

    public static string GenericRollupError() => WITResources.Get(nameof (GenericRollupError));

    public static string GenericRollupError(CultureInfo culture) => WITResources.Get(nameof (GenericRollupError), culture);

    public static string WorkItemsText() => WITResources.Get(nameof (WorkItemsText));

    public static string WorkItemsText(CultureInfo culture) => WITResources.Get(nameof (WorkItemsText), culture);

    public static string InformationIcon(object arg0) => WITResources.Format(nameof (InformationIcon), arg0);

    public static string InformationIcon(object arg0, CultureInfo culture) => WITResources.Format(nameof (InformationIcon), culture, arg0);

    public static string RollupColumnInfo() => WITResources.Get(nameof (RollupColumnInfo));

    public static string RollupColumnInfo(CultureInfo culture) => WITResources.Get(nameof (RollupColumnInfo), culture);

    public static string RollupColumn_LearnMore() => WITResources.Get(nameof (RollupColumn_LearnMore));

    public static string RollupColumn_LearnMore(CultureInfo culture) => WITResources.Get(nameof (RollupColumn_LearnMore), culture);

    public static string RollupDataQualityException() => WITResources.Get(nameof (RollupDataQualityException));

    public static string RollupDataQualityException(CultureInfo culture) => WITResources.Get(nameof (RollupDataQualityException), culture);

    public static string RollupColumn_TotalBy_AllDescendants() => WITResources.Get(nameof (RollupColumn_TotalBy_AllDescendants));

    public static string RollupColumn_TotalBy_AllDescendants(CultureInfo culture) => WITResources.Get(nameof (RollupColumn_TotalBy_AllDescendants), culture);

    public static string RollupColumn_TotalBy_WorkItemType(object arg0) => WITResources.Format(nameof (RollupColumn_TotalBy_WorkItemType), arg0);

    public static string RollupColumn_TotalBy_WorkItemType(object arg0, CultureInfo culture) => WITResources.Format(nameof (RollupColumn_TotalBy_WorkItemType), culture, arg0);

    public static string RollupColumn_TotalBy_Backlog(object arg0) => WITResources.Format(nameof (RollupColumn_TotalBy_Backlog), arg0);

    public static string RollupColumn_TotalBy_Backlog(object arg0, CultureInfo culture) => WITResources.Format(nameof (RollupColumn_TotalBy_Backlog), culture, arg0);

    public static string RollupColumn_TotalBy_FieldName(object arg0) => WITResources.Format(nameof (RollupColumn_TotalBy_FieldName), arg0);

    public static string RollupColumn_TotalBy_FieldName(object arg0, CultureInfo culture) => WITResources.Format(nameof (RollupColumn_TotalBy_FieldName), culture, arg0);

    public static string RollupColumn_TotalBy_Filter_Field(object arg0, object arg1) => WITResources.Format(nameof (RollupColumn_TotalBy_Filter_Field), arg0, arg1);

    public static string RollupColumn_TotalBy_Filter_Field(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (RollupColumn_TotalBy_Filter_Field), culture, arg0, arg1);
    }

    public static string RollupColumn_ProgressBy_AllCompletedDescendants() => WITResources.Get(nameof (RollupColumn_ProgressBy_AllCompletedDescendants));

    public static string RollupColumn_ProgressBy_AllCompletedDescendants(CultureInfo culture) => WITResources.Get(nameof (RollupColumn_ProgressBy_AllCompletedDescendants), culture);

    public static string RollupColumn_ProgressBy_WorkItemType(object arg0) => WITResources.Format(nameof (RollupColumn_ProgressBy_WorkItemType), arg0);

    public static string RollupColumn_ProgressBy_WorkItemType(object arg0, CultureInfo culture) => WITResources.Format(nameof (RollupColumn_ProgressBy_WorkItemType), culture, arg0);

    public static string RollupColumn_ProgressBy_Backlog(object arg0) => WITResources.Format(nameof (RollupColumn_ProgressBy_Backlog), arg0);

    public static string RollupColumn_ProgressBy_Backlog(object arg0, CultureInfo culture) => WITResources.Format(nameof (RollupColumn_ProgressBy_Backlog), culture, arg0);

    public static string RollupColumn_ProgressBy_EffortField(object arg0) => WITResources.Format(nameof (RollupColumn_ProgressBy_EffortField), arg0);

    public static string RollupColumn_ProgressBy_EffortField(object arg0, CultureInfo culture) => WITResources.Format(nameof (RollupColumn_ProgressBy_EffortField), culture, arg0);

    public static string RollupColumn_ProgressBy_Filter_Field(object arg0, object arg1) => WITResources.Format(nameof (RollupColumn_ProgressBy_Filter_Field), arg0, arg1);

    public static string RollupColumn_ProgressBy_Filter_Field(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return WITResources.Format(nameof (RollupColumn_ProgressBy_Filter_Field), culture, arg0, arg1);
    }

    public static string Rollup_EffortFieldName() => WITResources.Get(nameof (Rollup_EffortFieldName));

    public static string Rollup_EffortFieldName(CultureInfo culture) => WITResources.Get(nameof (Rollup_EffortFieldName), culture);

    public static string LiveReloadWorkItemDeleteNotificationMessage() => WITResources.Get(nameof (LiveReloadWorkItemDeleteNotificationMessage));

    public static string LiveReloadWorkItemDeleteNotificationMessage(CultureInfo culture) => WITResources.Get(nameof (LiveReloadWorkItemDeleteNotificationMessage), culture);

    public static string WorkItemProjectChangedNotificationMessage() => WITResources.Get(nameof (WorkItemProjectChangedNotificationMessage));

    public static string WorkItemProjectChangedNotificationMessage(CultureInfo culture) => WITResources.Get(nameof (WorkItemProjectChangedNotificationMessage), culture);

    public static string WorkItemTypeChangedNotificationMessage() => WITResources.Get(nameof (WorkItemTypeChangedNotificationMessage));

    public static string WorkItemTypeChangedNotificationMessage(CultureInfo culture) => WITResources.Get(nameof (WorkItemTypeChangedNotificationMessage), culture);

    public static string ImportFromCsv() => WITResources.Get(nameof (ImportFromCsv));

    public static string ImportFromCsv(CultureInfo culture) => WITResources.Get(nameof (ImportFromCsv), culture);

    public static string ImportEmptyDataSet() => WITResources.Get(nameof (ImportEmptyDataSet));

    public static string ImportEmptyDataSet(CultureInfo culture) => WITResources.Get(nameof (ImportEmptyDataSet), culture);

    public static string ImportEmptyColumnHeaders(object arg0) => WITResources.Format(nameof (ImportEmptyColumnHeaders), arg0);

    public static string ImportEmptyColumnHeaders(object arg0, CultureInfo culture) => WITResources.Format(nameof (ImportEmptyColumnHeaders), culture, arg0);

    public static string ImportEmptyColumn(object arg0) => WITResources.Format(nameof (ImportEmptyColumn), arg0);

    public static string ImportEmptyColumn(object arg0, CultureInfo culture) => WITResources.Format(nameof (ImportEmptyColumn), culture, arg0);

    public static string ImportInvalidColumnHeaders(object arg0) => WITResources.Format(nameof (ImportInvalidColumnHeaders), arg0);

    public static string ImportInvalidColumnHeaders(object arg0, CultureInfo culture) => WITResources.Format(nameof (ImportInvalidColumnHeaders), culture, arg0);

    public static string ImportMissingRequiredHeaders(object arg0) => WITResources.Format(nameof (ImportMissingRequiredHeaders), arg0);

    public static string ImportMissingRequiredHeaders(object arg0, CultureInfo culture) => WITResources.Format(nameof (ImportMissingRequiredHeaders), culture, arg0);

    public static string ImportDuplicateNonRequiredHeaders(object arg0) => WITResources.Format(nameof (ImportDuplicateNonRequiredHeaders), arg0);

    public static string ImportDuplicateNonRequiredHeaders(object arg0, CultureInfo culture) => WITResources.Format(nameof (ImportDuplicateNonRequiredHeaders), culture, arg0);

    public static string ImportDuplicateRequiredHeaders(object arg0) => WITResources.Format(nameof (ImportDuplicateRequiredHeaders), arg0);

    public static string ImportDuplicateRequiredHeaders(object arg0, CultureInfo culture) => WITResources.Format(nameof (ImportDuplicateRequiredHeaders), culture, arg0);

    public static string ImportNonConsecutiveTreeHeader(object arg0) => WITResources.Format(nameof (ImportNonConsecutiveTreeHeader), arg0);

    public static string ImportNonConsecutiveTreeHeader(object arg0, CultureInfo culture) => WITResources.Format(nameof (ImportNonConsecutiveTreeHeader), culture, arg0);

    public static string ImportErrorMessageTitle() => WITResources.Get(nameof (ImportErrorMessageTitle));

    public static string ImportErrorMessageTitle(CultureInfo culture) => WITResources.Get(nameof (ImportErrorMessageTitle), culture);

    public static string ProgressBarLoading() => WITResources.Get(nameof (ProgressBarLoading));

    public static string ProgressBarLoading(CultureInfo culture) => WITResources.Get(nameof (ProgressBarLoading), culture);

    public static string CustomRollupPanelTitle() => WITResources.Get(nameof (CustomRollupPanelTitle));

    public static string CustomRollupPanelTitle(CultureInfo culture) => WITResources.Get(nameof (CustomRollupPanelTitle), culture);

    public static string CustomRollupTypeOptionLabel() => WITResources.Get(nameof (CustomRollupTypeOptionLabel));

    public static string CustomRollupTypeOptionLabel(CultureInfo culture) => WITResources.Get(nameof (CustomRollupTypeOptionLabel), culture);

    public static string CustomRollupProgressOptionTitle() => WITResources.Get(nameof (CustomRollupProgressOptionTitle));

    public static string CustomRollupProgressOptionTitle(CultureInfo culture) => WITResources.Get(nameof (CustomRollupProgressOptionTitle), culture);

    public static string CustomRollupProgressOptionSubtitle() => WITResources.Get(nameof (CustomRollupProgressOptionSubtitle));

    public static string CustomRollupProgressOptionSubtitle(CultureInfo culture) => WITResources.Get(nameof (CustomRollupProgressOptionSubtitle), culture);

    public static string CustomRollupTotalOptionTitle() => WITResources.Get(nameof (CustomRollupTotalOptionTitle));

    public static string CustomRollupTotalOptionTitle(CultureInfo culture) => WITResources.Get(nameof (CustomRollupTotalOptionTitle), culture);

    public static string CustomRollupTotalOptionSubtitle() => WITResources.Get(nameof (CustomRollupTotalOptionSubtitle));

    public static string CustomRollupTotalOptionSubtitle(CultureInfo culture) => WITResources.Get(nameof (CustomRollupTotalOptionSubtitle), culture);

    public static string Backlog_DisplayName(object arg0) => WITResources.Format(nameof (Backlog_DisplayName), arg0);

    public static string Backlog_DisplayName(object arg0, CultureInfo culture) => WITResources.Format(nameof (Backlog_DisplayName), culture, arg0);

    public static string WorkItemTypePicker_Label() => WITResources.Get(nameof (WorkItemTypePicker_Label));

    public static string WorkItemTypePicker_Label(CultureInfo culture) => WITResources.Get(nameof (WorkItemTypePicker_Label), culture);

    public static string AggregationTypePickerLabel() => WITResources.Get(nameof (AggregationTypePickerLabel));

    public static string AggregationTypePickerLabel(CultureInfo culture) => WITResources.Get(nameof (AggregationTypePickerLabel), culture);

    public static string Count() => WITResources.Get(nameof (Count));

    public static string Count(CultureInfo culture) => WITResources.Get(nameof (Count), culture);

    public static string Sum() => WITResources.Get(nameof (Sum));

    public static string Sum(CultureInfo culture) => WITResources.Get(nameof (Sum), culture);

    public static string Of() => WITResources.Get(nameof (Of));

    public static string Of(CultureInfo culture) => WITResources.Get(nameof (Of), culture);

    public static string WorkItems() => WITResources.Get(nameof (WorkItems));

    public static string WorkItems(CultureInfo culture) => WITResources.Get(nameof (WorkItems), culture);

    public static string AggregationModeLabel() => WITResources.Get(nameof (AggregationModeLabel));

    public static string AggregationModeLabel(CultureInfo culture) => WITResources.Get(nameof (AggregationModeLabel), culture);

    public static string AggregationFieldLabel() => WITResources.Get(nameof (AggregationFieldLabel));

    public static string AggregationFieldLabel(CultureInfo culture) => WITResources.Get(nameof (AggregationFieldLabel), culture);

    public static string RollupColumnFieldRowAriaLabel(object arg0) => WITResources.Format(nameof (RollupColumnFieldRowAriaLabel), arg0);

    public static string RollupColumnFieldRowAriaLabel(object arg0, CultureInfo culture) => WITResources.Format(nameof (RollupColumnFieldRowAriaLabel), culture, arg0);

    public static string QueryBreadcrumbAriaLabel(object arg0) => WITResources.Format(nameof (QueryBreadcrumbAriaLabel), arg0);

    public static string QueryBreadcrumbAriaLabel(object arg0, CultureInfo culture) => WITResources.Format(nameof (QueryBreadcrumbAriaLabel), culture, arg0);

    public static string AddParentText1() => WITResources.Get(nameof (AddParentText1));

    public static string AddParentText1(CultureInfo culture) => WITResources.Get(nameof (AddParentText1), culture);

    public static string AddParentText2() => WITResources.Get(nameof (AddParentText2));

    public static string AddParentText2(CultureInfo culture) => WITResources.Get(nameof (AddParentText2), culture);

    public static string ZeroDataImageAltText() => WITResources.Get(nameof (ZeroDataImageAltText));

    public static string ZeroDataImageAltText(CultureInfo culture) => WITResources.Get(nameof (ZeroDataImageAltText), culture);

    public static string AriaLabelForActions() => WITResources.Get(nameof (AriaLabelForActions));

    public static string AriaLabelForActions(CultureInfo culture) => WITResources.Get(nameof (AriaLabelForActions), culture);

    public static string ChooseFile() => WITResources.Get(nameof (ChooseFile));

    public static string ChooseFile(CultureInfo culture) => WITResources.Get(nameof (ChooseFile), culture);
  }
}
