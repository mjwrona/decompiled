// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web
{
  internal static class ResourceStrings
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (ResourceStrings), typeof (ResourceStrings).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ResourceStrings.s_resMgr;

    private static string Get(string resourceName) => ResourceStrings.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ResourceStrings.Get(resourceName) : ResourceStrings.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ResourceStrings.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ResourceStrings.GetInt(resourceName) : (int) ResourceStrings.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ResourceStrings.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ResourceStrings.GetBool(resourceName) : (bool) ResourceStrings.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ResourceStrings.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ResourceStrings.Get(resourceName, culture);
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

    public static string AsOfParameterConflict() => ResourceStrings.Get(nameof (AsOfParameterConflict));

    public static string AsOfParameterConflict(CultureInfo culture) => ResourceStrings.Get(nameof (AsOfParameterConflict), culture);

    public static string AttachmentContentInvalid() => ResourceStrings.Get(nameof (AttachmentContentInvalid));

    public static string AttachmentContentInvalid(CultureInfo culture) => ResourceStrings.Get(nameof (AttachmentContentInvalid), culture);

    public static string ClassificationNodeUrlCreationFailed() => ResourceStrings.Get(nameof (ClassificationNodeUrlCreationFailed));

    public static string ClassificationNodeUrlCreationFailed(CultureInfo culture) => ResourceStrings.Get(nameof (ClassificationNodeUrlCreationFailed), culture);

    public static string ExpandParameterConflict() => ResourceStrings.Get(nameof (ExpandParameterConflict));

    public static string ExpandParameterConflict(CultureInfo culture) => ResourceStrings.Get(nameof (ExpandParameterConflict), culture);

    public static string FieldUrlCreationFailed() => ResourceStrings.Get(nameof (FieldUrlCreationFailed));

    public static string FieldUrlCreationFailed(CultureInfo culture) => ResourceStrings.Get(nameof (FieldUrlCreationFailed), culture);

    public static string FoldersCannotBeQueried() => ResourceStrings.Get(nameof (FoldersCannotBeQueried));

    public static string FoldersCannotBeQueried(CultureInfo culture) => ResourceStrings.Get(nameof (FoldersCannotBeQueried), culture);

    public static string IdMismatch(object arg0, object arg1) => ResourceStrings.Format(nameof (IdMismatch), arg0, arg1);

    public static string IdMismatch(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (IdMismatch), culture, arg0, arg1);

    public static string IdsOutOfRange(object arg0) => ResourceStrings.Format(nameof (IdsOutOfRange), arg0);

    public static string IdsOutOfRange(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (IdsOutOfRange), culture, arg0);

    public static string InvalidOrMissingResourceId() => ResourceStrings.Get(nameof (InvalidOrMissingResourceId));

    public static string InvalidOrMissingResourceId(CultureInfo culture) => ResourceStrings.Get(nameof (InvalidOrMissingResourceId), culture);

    public static string InvalidRelationType(object arg0) => ResourceStrings.Format(nameof (InvalidRelationType), arg0);

    public static string InvalidRelationType(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (InvalidRelationType), culture, arg0);

    public static string InvalidRelationUrl() => ResourceStrings.Get(nameof (InvalidRelationUrl));

    public static string InvalidRelationUrl(CultureInfo culture) => ResourceStrings.Get(nameof (InvalidRelationUrl), culture);

    public static string MissingFieldOnQuery() => ResourceStrings.Get(nameof (MissingFieldOnQuery));

    public static string MissingFieldOnQuery(CultureInfo culture) => ResourceStrings.Get(nameof (MissingFieldOnQuery), culture);

    public static string MissingPatchDocument() => ResourceStrings.Get(nameof (MissingPatchDocument));

    public static string MissingPatchDocument(CultureInfo culture) => ResourceStrings.Get(nameof (MissingPatchDocument), culture);

    public static string MissingQueryParameter(object arg0) => ResourceStrings.Format(nameof (MissingQueryParameter), arg0);

    public static string MissingQueryParameter(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (MissingQueryParameter), culture, arg0);

    public static string MissingQueryParameters(object arg0, object arg1) => ResourceStrings.Format(nameof (MissingQueryParameters), arg0, arg1);

    public static string MissingQueryParameters(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (MissingQueryParameters), culture, arg0, arg1);

    public static string MissingWiqlParameter() => ResourceStrings.Get(nameof (MissingWiqlParameter));

    public static string MissingWiqlParameter(CultureInfo culture) => ResourceStrings.Get(nameof (MissingWiqlParameter), culture);

    public static string NullOrEmptyAttachment() => ResourceStrings.Get(nameof (NullOrEmptyAttachment));

    public static string NullOrEmptyAttachment(CultureInfo culture) => ResourceStrings.Get(nameof (NullOrEmptyAttachment), culture);

    public static string NullOrEmptyParameter(object arg0) => ResourceStrings.Format(nameof (NullOrEmptyParameter), arg0);

    public static string NullOrEmptyParameter(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (NullOrEmptyParameter), culture, arg0);

    public static string NullQueryParameter() => ResourceStrings.Get(nameof (NullQueryParameter));

    public static string NullQueryParameter(CultureInfo culture) => ResourceStrings.Get(nameof (NullQueryParameter), culture);

    public static string ProjectNotFound() => ResourceStrings.Get(nameof (ProjectNotFound));

    public static string ProjectNotFound(CultureInfo culture) => ResourceStrings.Get(nameof (ProjectNotFound), culture);

    public static string QueryInvalidParent(object arg0) => ResourceStrings.Format(nameof (QueryInvalidParent), arg0);

    public static string QueryInvalidParent(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (QueryInvalidParent), culture, arg0);

    public static string QueryParameterOutOfRange(object arg0) => ResourceStrings.Format(nameof (QueryParameterOutOfRange), arg0);

    public static string QueryParameterOutOfRange(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (QueryParameterOutOfRange), culture, arg0);

    public static string QueryParameterOutOfRangeWithRangeValues(
      object arg0,
      object arg1,
      object arg2)
    {
      return ResourceStrings.Format(nameof (QueryParameterOutOfRangeWithRangeValues), arg0, arg1, arg2);
    }

    public static string QueryParameterOutOfRangeWithRangeValues(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (QueryParameterOutOfRangeWithRangeValues), culture, arg0, arg1, arg2);
    }

    public static string QueryUpdateFolderConflict() => ResourceStrings.Get(nameof (QueryUpdateFolderConflict));

    public static string QueryUpdateFolderConflict(CultureInfo culture) => ResourceStrings.Get(nameof (QueryUpdateFolderConflict), culture);

    public static string RevisionNotFound(object arg0, object arg1) => ResourceStrings.Format(nameof (RevisionNotFound), arg0, arg1);

    public static string RevisionNotFound(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (RevisionNotFound), culture, arg0, arg1);

    public static string UpdateNotFound() => ResourceStrings.Get(nameof (UpdateNotFound));

    public static string UpdateNotFound(CultureInfo culture) => ResourceStrings.Get(nameof (UpdateNotFound), culture);

    public static string WorkItemAttachmentUrlCreationFailed() => ResourceStrings.Get(nameof (WorkItemAttachmentUrlCreationFailed));

    public static string WorkItemAttachmentUrlCreationFailed(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemAttachmentUrlCreationFailed), culture);

    public static string WorkItemPatchDoesNotSupportEmptyPath() => ResourceStrings.Get(nameof (WorkItemPatchDoesNotSupportEmptyPath));

    public static string WorkItemPatchDoesNotSupportEmptyPath(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemPatchDoesNotSupportEmptyPath), culture);

    public static string WorkItemPatchDoesNotSupportPatchingTopLevelProperties(object arg0) => ResourceStrings.Format(nameof (WorkItemPatchDoesNotSupportPatchingTopLevelProperties), arg0);

    public static string WorkItemPatchDoesNotSupportPatchingTopLevelProperties(
      object arg0,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (WorkItemPatchDoesNotSupportPatchingTopLevelProperties), culture, arg0);
    }

    public static string WorkItemRevisionUrlCreationFailed() => ResourceStrings.Get(nameof (WorkItemRevisionUrlCreationFailed));

    public static string WorkItemRevisionUrlCreationFailed(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemRevisionUrlCreationFailed), culture);

    public static string WorkItemUpdatesUrlCreationFailed() => ResourceStrings.Get(nameof (WorkItemUpdatesUrlCreationFailed));

    public static string WorkItemUpdatesUrlCreationFailed(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemUpdatesUrlCreationFailed), culture);

    public static string WorkItemUrlCreationFailed() => ResourceStrings.Get(nameof (WorkItemUrlCreationFailed));

    public static string WorkItemUrlCreationFailed(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemUrlCreationFailed), culture);

    public static string WorkItemIconUrlCreationFailed() => ResourceStrings.Get(nameof (WorkItemIconUrlCreationFailed));

    public static string WorkItemIconUrlCreationFailed(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemIconUrlCreationFailed), culture);

    public static string ReportingWorkItemRevisionsUrlCreationFailed() => ResourceStrings.Get(nameof (ReportingWorkItemRevisionsUrlCreationFailed));

    public static string ReportingWorkItemRevisionsUrlCreationFailed(CultureInfo culture) => ResourceStrings.Get(nameof (ReportingWorkItemRevisionsUrlCreationFailed), culture);

    public static string ReportingWorkItemLinksUrlCreationFailed() => ResourceStrings.Get(nameof (ReportingWorkItemLinksUrlCreationFailed));

    public static string ReportingWorkItemLinksUrlCreationFailed(CultureInfo culture) => ResourceStrings.Get(nameof (ReportingWorkItemLinksUrlCreationFailed), culture);

    public static string InvalidWiql() => ResourceStrings.Get(nameof (InvalidWiql));

    public static string InvalidWiql(CultureInfo culture) => ResourceStrings.Get(nameof (InvalidWiql), culture);

    public static string UnknownVariableType() => ResourceStrings.Get(nameof (UnknownVariableType));

    public static string UnknownVariableType(CultureInfo culture) => ResourceStrings.Get(nameof (UnknownVariableType), culture);

    public static string WorkItemNotFound(object arg0) => ResourceStrings.Format(nameof (WorkItemNotFound), arg0);

    public static string WorkItemNotFound(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (WorkItemNotFound), culture, arg0);

    public static string FieldMismatch(object arg0) => ResourceStrings.Format(nameof (FieldMismatch), arg0);

    public static string FieldMismatch(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (FieldMismatch), culture, arg0);

    public static string RulesFromParseError() => ResourceStrings.Get(nameof (RulesFromParseError));

    public static string RulesFromParseError(CultureInfo culture) => ResourceStrings.Get(nameof (RulesFromParseError), culture);

    public static string RulesFromNotSupported() => ResourceStrings.Get(nameof (RulesFromNotSupported));

    public static string RulesFromNotSupported(CultureInfo culture) => ResourceStrings.Get(nameof (RulesFromNotSupported), culture);

    public static string BatchRequiresAtLeastOneOperation() => ResourceStrings.Get(nameof (BatchRequiresAtLeastOneOperation));

    public static string BatchRequiresAtLeastOneOperation(CultureInfo culture) => ResourceStrings.Get(nameof (BatchRequiresAtLeastOneOperation), culture);

    public static string UnsupportedBatchOperation() => ResourceStrings.Get(nameof (UnsupportedBatchOperation));

    public static string UnsupportedBatchOperation(CultureInfo culture) => ResourceStrings.Get(nameof (UnsupportedBatchOperation), culture);

    public static string WorkItemUpdateBatchFailed() => ResourceStrings.Get(nameof (WorkItemUpdateBatchFailed));

    public static string WorkItemUpdateBatchFailed(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemUpdateBatchFailed), culture);

    public static string WorkItemTrackingTypeTemplateNotFound() => ResourceStrings.Get(nameof (WorkItemTrackingTypeTemplateNotFound));

    public static string WorkItemTrackingTypeTemplateNotFound(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemTrackingTypeTemplateNotFound), culture);

    public static string WorkItemNotFoundAtTime(object arg0, object arg1) => ResourceStrings.Format(nameof (WorkItemNotFoundAtTime), arg0, arg1);

    public static string WorkItemNotFoundAtTime(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (WorkItemNotFoundAtTime), culture, arg0, arg1);

    public static string ReclassificationIdRequired() => ResourceStrings.Get(nameof (ReclassificationIdRequired));

    public static string ReclassificationIdRequired(CultureInfo culture) => ResourceStrings.Get(nameof (ReclassificationIdRequired), culture);

    public static string InvalidAsOfParameter() => ResourceStrings.Get(nameof (InvalidAsOfParameter));

    public static string InvalidAsOfParameter(CultureInfo culture) => ResourceStrings.Get(nameof (InvalidAsOfParameter), culture);

    public static string UnsupportedAttachmentUploadType(object arg0, object arg1) => ResourceStrings.Format(nameof (UnsupportedAttachmentUploadType), arg0, arg1);

    public static string UnsupportedAttachmentUploadType(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (UnsupportedAttachmentUploadType), culture, arg0, arg1);
    }

    public static string IncorrectContentRangeForAttachmentUpload() => ResourceStrings.Get(nameof (IncorrectContentRangeForAttachmentUpload));

    public static string IncorrectContentRangeForAttachmentUpload(CultureInfo culture) => ResourceStrings.Get(nameof (IncorrectContentRangeForAttachmentUpload), culture);

    public static string UnsupportedContentTypeForAttachmentUpload() => ResourceStrings.Get(nameof (UnsupportedContentTypeForAttachmentUpload));

    public static string UnsupportedContentTypeForAttachmentUpload(CultureInfo culture) => ResourceStrings.Get(nameof (UnsupportedContentTypeForAttachmentUpload), culture);

    public static string IncorrectAttachmentContentLength() => ResourceStrings.Get(nameof (IncorrectAttachmentContentLength));

    public static string IncorrectAttachmentContentLength(CultureInfo culture) => ResourceStrings.Get(nameof (IncorrectAttachmentContentLength), culture);

    public static string InvalidRelationResourceSize() => ResourceStrings.Get(nameof (InvalidRelationResourceSize));

    public static string InvalidRelationResourceSize(CultureInfo culture) => ResourceStrings.Get(nameof (InvalidRelationResourceSize), culture);

    public static string WorkItemRuleNotSupported(object arg0) => ResourceStrings.Format(nameof (WorkItemRuleNotSupported), arg0);

    public static string WorkItemRuleNotSupported(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (WorkItemRuleNotSupported), culture, arg0);

    public static string MissingFieldParameter(object arg0) => ResourceStrings.Format(nameof (MissingFieldParameter), arg0);

    public static string MissingFieldParameter(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (MissingFieldParameter), culture, arg0);

    public static string MissingWorkItemTypeParameter(object arg0) => ResourceStrings.Format(nameof (MissingWorkItemTypeParameter), arg0);

    public static string MissingWorkItemTypeParameter(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (MissingWorkItemTypeParameter), culture, arg0);

    public static string NullFieldObject() => ResourceStrings.Get(nameof (NullFieldObject));

    public static string NullFieldObject(CultureInfo culture) => ResourceStrings.Get(nameof (NullFieldObject), culture);

    public static string NullWorkItemTypeObject() => ResourceStrings.Get(nameof (NullWorkItemTypeObject));

    public static string NullWorkItemTypeObject(CultureInfo culture) => ResourceStrings.Get(nameof (NullWorkItemTypeObject), culture);

    public static string ProcessImportExportController_ErrorsLink() => ResourceStrings.Get(nameof (ProcessImportExportController_ErrorsLink));

    public static string ProcessImportExportController_ErrorsLink(CultureInfo culture) => ResourceStrings.Get(nameof (ProcessImportExportController_ErrorsLink), culture);

    public static string ProcessImportExportController_Status_FailedProject(
      object arg0,
      object arg1)
    {
      return ResourceStrings.Format(nameof (ProcessImportExportController_Status_FailedProject), arg0, arg1);
    }

    public static string ProcessImportExportController_Status_FailedProject(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (ProcessImportExportController_Status_FailedProject), culture, arg0, arg1);
    }

    public static string InvalidPickListType() => ResourceStrings.Get(nameof (InvalidPickListType));

    public static string InvalidPickListType(CultureInfo culture) => ResourceStrings.Get(nameof (InvalidPickListType), culture);

    public static string QueryGetBatchFailed() => ResourceStrings.Get(nameof (QueryGetBatchFailed));

    public static string QueryGetBatchFailed(CultureInfo culture) => ResourceStrings.Get(nameof (QueryGetBatchFailed), culture);

    public static string InvalidQueryId(object arg0) => ResourceStrings.Format(nameof (InvalidQueryId), arg0);

    public static string InvalidQueryId(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (InvalidQueryId), culture, arg0);

    public static string InvalidPicklistId() => ResourceStrings.Get(nameof (InvalidPicklistId));

    public static string InvalidPicklistId(CultureInfo culture) => ResourceStrings.Get(nameof (InvalidPicklistId), culture);

    public static string ProcessNotFound(object arg0) => ResourceStrings.Format(nameof (ProcessNotFound), arg0);

    public static string ProcessNotFound(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (ProcessNotFound), culture, arg0);

    public static string WorkItemStateCategoryInvalid(object arg0) => ResourceStrings.Format(nameof (WorkItemStateCategoryInvalid), arg0);

    public static string WorkItemStateCategoryInvalid(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (WorkItemStateCategoryInvalid), culture, arg0);

    public static string InconsistentParametersForBatchRequest(object arg0) => ResourceStrings.Format(nameof (InconsistentParametersForBatchRequest), arg0);

    public static string InconsistentParametersForBatchRequest(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (InconsistentParametersForBatchRequest), culture, arg0);

    public static string InvalidGetWorkItemTypeExpandValue(object arg0) => ResourceStrings.Format(nameof (InvalidGetWorkItemTypeExpandValue), arg0);

    public static string InvalidGetWorkItemTypeExpandValue(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (InvalidGetWorkItemTypeExpandValue), culture, arg0);

    public static string UnhideStateUsingDeleteState() => ResourceStrings.Get(nameof (UnhideStateUsingDeleteState));

    public static string UnhideStateUsingDeleteState(CultureInfo culture) => ResourceStrings.Get(nameof (UnhideStateUsingDeleteState), culture);

    public static string FieldTypeDisabled(object arg0) => ResourceStrings.Format(nameof (FieldTypeDisabled), arg0);

    public static string FieldTypeDisabled(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (FieldTypeDisabled), culture, arg0);

    public static string Templates_ArgumentNull(object arg0) => ResourceStrings.Format(nameof (Templates_ArgumentNull), arg0);

    public static string Templates_ArgumentNull(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (Templates_ArgumentNull), culture, arg0);

    public static string Templates_InvalidOperation(object arg0) => ResourceStrings.Format(nameof (Templates_InvalidOperation), arg0);

    public static string Templates_InvalidOperation(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (Templates_InvalidOperation), culture, arg0);

    public static string Templates_InvalidPath(object arg0) => ResourceStrings.Format(nameof (Templates_InvalidPath), arg0);

    public static string Templates_InvalidPath(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (Templates_InvalidPath), culture, arg0);

    public static string Templates_MissingPath() => ResourceStrings.Get(nameof (Templates_MissingPath));

    public static string Templates_MissingPath(CultureInfo culture) => ResourceStrings.Get(nameof (Templates_MissingPath), culture);

    public static string HistoryNotUpdatedForRevision(object arg0, object arg1) => ResourceStrings.Format(nameof (HistoryNotUpdatedForRevision), arg0, arg1);

    public static string HistoryNotUpdatedForRevision(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (HistoryNotUpdatedForRevision), culture, arg0, arg1);
    }

    public static string WorkItemRevisionNotFound(object arg0, object arg1) => ResourceStrings.Format(nameof (WorkItemRevisionNotFound), arg0, arg1);

    public static string WorkItemRevisionNotFound(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (WorkItemRevisionNotFound), culture, arg0, arg1);

    public static string NullOrEmptyRule() => ResourceStrings.Get(nameof (NullOrEmptyRule));

    public static string NullOrEmptyRule(CultureInfo culture) => ResourceStrings.Get(nameof (NullOrEmptyRule), culture);

    public static string NullOrEmptyRuleValue() => ResourceStrings.Get(nameof (NullOrEmptyRuleValue));

    public static string NullOrEmptyRuleValue(CultureInfo culture) => ResourceStrings.Get(nameof (NullOrEmptyRuleValue), culture);

    public static string InvalidBehaviorPropertyName(object arg0) => ResourceStrings.Format(nameof (InvalidBehaviorPropertyName), arg0);

    public static string InvalidBehaviorPropertyName(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (InvalidBehaviorPropertyName), culture, arg0);

    public static string InvalidIsDefaultValue() => ResourceStrings.Get(nameof (InvalidIsDefaultValue));

    public static string InvalidIsDefaultValue(CultureInfo culture) => ResourceStrings.Get(nameof (InvalidIsDefaultValue), culture);

    public static string WorkItemRuleActionInvalid(object arg0) => ResourceStrings.Format(nameof (WorkItemRuleActionInvalid), arg0);

    public static string WorkItemRuleActionInvalid(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (WorkItemRuleActionInvalid), culture, arg0);

    public static string WorkItemRuleConditionInvalid(object arg0) => ResourceStrings.Format(nameof (WorkItemRuleConditionInvalid), arg0);

    public static string WorkItemRuleConditionInvalid(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (WorkItemRuleConditionInvalid), culture, arg0);

    public static string DefaultIdNotFound() => ResourceStrings.Get(nameof (DefaultIdNotFound));

    public static string DefaultIdNotFound(CultureInfo culture) => ResourceStrings.Get(nameof (DefaultIdNotFound), culture);

    public static string DefaultValueCannotBeGroup() => ResourceStrings.Get(nameof (DefaultValueCannotBeGroup));

    public static string DefaultValueCannotBeGroup(CultureInfo culture) => ResourceStrings.Get(nameof (DefaultValueCannotBeGroup), culture);

    public static string ClassificationNodeRequred() => ResourceStrings.Get(nameof (ClassificationNodeRequred));

    public static string ClassificationNodeRequred(CultureInfo culture) => ResourceStrings.Get(nameof (ClassificationNodeRequred), culture);

    public static string ActionTypeRequiresConditions(object arg0) => ResourceStrings.Format(nameof (ActionTypeRequiresConditions), arg0);

    public static string ActionTypeRequiresConditions(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (ActionTypeRequiresConditions), culture, arg0);

    public static string ConditionTypeRequiresOtherCondition(object arg0) => ResourceStrings.Format(nameof (ConditionTypeRequiresOtherCondition), arg0);

    public static string ConditionTypeRequiresOtherCondition(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (ConditionTypeRequiresOtherCondition), culture, arg0);

    public static string DuplicateValueChangeRules(object arg0, object arg1) => ResourceStrings.Format(nameof (DuplicateValueChangeRules), arg0, arg1);

    public static string DuplicateValueChangeRules(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (DuplicateValueChangeRules), culture, arg0, arg1);

    public static string DuplicateRuleAction(object arg0, object arg1) => ResourceStrings.Format(nameof (DuplicateRuleAction), arg0, arg1);

    public static string DuplicateRuleAction(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (DuplicateRuleAction), culture, arg0, arg1);

    public static string FieldNotAllowedInConditions(object arg0) => ResourceStrings.Format(nameof (FieldNotAllowedInConditions), arg0);

    public static string FieldNotAllowedInConditions(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (FieldNotAllowedInConditions), culture, arg0);

    public static string FieldRefNameMismatch(object arg0, object arg1) => ResourceStrings.Format(nameof (FieldRefNameMismatch), arg0, arg1);

    public static string FieldRefNameMismatch(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (FieldRefNameMismatch), culture, arg0, arg1);

    public static string MissingRequiredProperty(object arg0) => ResourceStrings.Format(nameof (MissingRequiredProperty), arg0);

    public static string MissingRequiredProperty(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (MissingRequiredProperty), culture, arg0);

    public static string MissingRequiredPropertyFor(object arg0, object arg1) => ResourceStrings.Format(nameof (MissingRequiredPropertyFor), arg0, arg1);

    public static string MissingRequiredPropertyFor(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (MissingRequiredPropertyFor), culture, arg0, arg1);

    public static string RuleActionDisallowsOtherActions(object arg0) => ResourceStrings.Format(nameof (RuleActionDisallowsOtherActions), arg0);

    public static string RuleActionDisallowsOtherActions(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (RuleActionDisallowsOtherActions), culture, arg0);

    public static string RuleActionConflictsWithAction(object arg0, object arg1) => ResourceStrings.Format(nameof (RuleActionConflictsWithAction), arg0, arg1);

    public static string RuleActionConflictsWithAction(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (RuleActionConflictsWithAction), culture, arg0, arg1);
    }

    public static string RulesTooManyConditions(object arg0) => ResourceStrings.Format(nameof (RulesTooManyConditions), arg0);

    public static string RulesTooManyConditions(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (RulesTooManyConditions), culture, arg0);

    public static string RuleStringContainsInvalidChars() => ResourceStrings.Get(nameof (RuleStringContainsInvalidChars));

    public static string RuleStringContainsInvalidChars(CultureInfo culture) => ResourceStrings.Get(nameof (RuleStringContainsInvalidChars), culture);

    public static string RuleStringTooLong(object arg0, object arg1, object arg2) => ResourceStrings.Format(nameof (RuleStringTooLong), arg0, arg1, arg2);

    public static string RuleStringTooLong(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (RuleStringTooLong), culture, arg0, arg1, arg2);
    }

    public static string ThreeConditionsRequirePattern(object arg0, object arg1) => ResourceStrings.Format(nameof (ThreeConditionsRequirePattern), arg0, arg1);

    public static string ThreeConditionsRequirePattern(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (ThreeConditionsRequirePattern), culture, arg0, arg1);
    }

    public static string TwoConditionsRequireEitherPattern(object arg0, object arg1, object arg2) => ResourceStrings.Format(nameof (TwoConditionsRequireEitherPattern), arg0, arg1, arg2);

    public static string TwoConditionsRequireEitherPattern(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (TwoConditionsRequireEitherPattern), culture, arg0, arg1, arg2);
    }

    public static string TwoConditionsRequirePattern(object arg0, object arg1) => ResourceStrings.Format(nameof (TwoConditionsRequirePattern), arg0, arg1);

    public static string TwoConditionsRequirePattern(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (TwoConditionsRequirePattern), culture, arg0, arg1);

    public static string DisallowValueUsedIncorrectly() => ResourceStrings.Get(nameof (DisallowValueUsedIncorrectly));

    public static string DisallowValueUsedIncorrectly(CultureInfo culture) => ResourceStrings.Get(nameof (DisallowValueUsedIncorrectly), culture);

    public static string UnrecognizedPropertyValue(object arg0, object arg1) => ResourceStrings.Format(nameof (UnrecognizedPropertyValue), arg0, arg1);

    public static string UnrecognizedPropertyValue(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (UnrecognizedPropertyValue), culture, arg0, arg1);

    public static string WorkItemTooManyRules(object arg0) => ResourceStrings.Format(nameof (WorkItemTooManyRules), arg0);

    public static string WorkItemTooManyRules(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (WorkItemTooManyRules), culture, arg0);

    public static string UnknownFieldReferenceName(object arg0, object arg1) => ResourceStrings.Format(nameof (UnknownFieldReferenceName), arg0, arg1);

    public static string UnknownFieldReferenceName(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (UnknownFieldReferenceName), culture, arg0, arg1);

    public static string QueryArtifactLinksExceedingLimit(object arg0) => ResourceStrings.Format(nameof (QueryArtifactLinksExceedingLimit), arg0);

    public static string QueryArtifactLinksExceedingLimit(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (QueryArtifactLinksExceedingLimit), culture, arg0);

    public static string CannotDeleteSystemRule() => ResourceStrings.Get(nameof (CannotDeleteSystemRule));

    public static string CannotDeleteSystemRule(CultureInfo culture) => ResourceStrings.Get(nameof (CannotDeleteSystemRule), culture);

    public static string CannotAddDuplicateRule(object arg0) => ResourceStrings.Format(nameof (CannotAddDuplicateRule), arg0);

    public static string CannotAddDuplicateRule(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (CannotAddDuplicateRule), culture, arg0);

    public static string CannotUpdateSystemRule() => ResourceStrings.Get(nameof (CannotUpdateSystemRule));

    public static string CannotUpdateSystemRule(CultureInfo culture) => ResourceStrings.Get(nameof (CannotUpdateSystemRule), culture);

    public static string RuleIdMismatchInRequestBody(object arg0, object arg1) => ResourceStrings.Format(nameof (RuleIdMismatchInRequestBody), arg0, arg1);

    public static string RuleIdMismatchInRequestBody(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (RuleIdMismatchInRequestBody), culture, arg0, arg1);

    public static string CannotAddDuplicateRuleWithName(object arg0) => ResourceStrings.Format(nameof (CannotAddDuplicateRuleWithName), arg0);

    public static string CannotAddDuplicateRuleWithName(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (CannotAddDuplicateRuleWithName), culture, arg0);

    public static string GetCdnUrlFailed(object arg0) => ResourceStrings.Format(nameof (GetCdnUrlFailed), arg0);

    public static string GetCdnUrlFailed(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (GetCdnUrlFailed), culture, arg0);

    public static string ProcessTemplateIdIsNotValid() => ResourceStrings.Get(nameof (ProcessTemplateIdIsNotValid));

    public static string ProcessTemplateIdIsNotValid(CultureInfo culture) => ResourceStrings.Get(nameof (ProcessTemplateIdIsNotValid), culture);

    public static string ProcessTemplateNameConflict(object arg0) => ResourceStrings.Format(nameof (ProcessTemplateNameConflict), arg0);

    public static string ProcessTemplateNameConflict(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (ProcessTemplateNameConflict), culture, arg0);

    public static string ProcessTemplateNameIsNotValid() => ResourceStrings.Get(nameof (ProcessTemplateNameIsNotValid));

    public static string ProcessTemplateNameIsNotValid(CultureInfo culture) => ResourceStrings.Get(nameof (ProcessTemplateNameIsNotValid), culture);

    public static string DuplicateRuleFriendlyName(object arg0) => ResourceStrings.Format(nameof (DuplicateRuleFriendlyName), arg0);

    public static string DuplicateRuleFriendlyName(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (DuplicateRuleFriendlyName), culture, arg0);

    public static string WorkItemPatchDocument_InvalidPath(object arg0) => ResourceStrings.Format(nameof (WorkItemPatchDocument_InvalidPath), arg0);

    public static string WorkItemPatchDocument_InvalidPath(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (WorkItemPatchDocument_InvalidPath), culture, arg0);

    public static string WorkItemPatchDocument_TestFailed(object arg0, object arg1, object arg2) => ResourceStrings.Format(nameof (WorkItemPatchDocument_TestFailed), arg0, arg1, arg2);

    public static string WorkItemPatchDocument_TestFailed(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (WorkItemPatchDocument_TestFailed), culture, arg0, arg1, arg2);
    }

    public static string WorkItemPatchDocument_TestNotSupportedForRelations() => ResourceStrings.Get(nameof (WorkItemPatchDocument_TestNotSupportedForRelations));

    public static string WorkItemPatchDocument_TestNotSupportedForRelations(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemPatchDocument_TestNotSupportedForRelations), culture);

    public static string WorkItemPatchDocument_IndexOutOfRange(object arg0) => ResourceStrings.Format(nameof (WorkItemPatchDocument_IndexOutOfRange), arg0);

    public static string WorkItemPatchDocument_IndexOutOfRange(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (WorkItemPatchDocument_IndexOutOfRange), culture, arg0);

    public static string WorkItemPatchDocument_OperationNotSuppported(object arg0) => ResourceStrings.Format(nameof (WorkItemPatchDocument_OperationNotSuppported), arg0);

    public static string WorkItemPatchDocument_OperationNotSuppported(
      object arg0,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (WorkItemPatchDocument_OperationNotSuppported), culture, arg0);
    }

    public static string WorkItemPatchDocument_CannotChangeRelationType() => ResourceStrings.Get(nameof (WorkItemPatchDocument_CannotChangeRelationType));

    public static string WorkItemPatchDocument_CannotChangeRelationType(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemPatchDocument_CannotChangeRelationType), culture);

    public static string WorkItemPatchDocument_DuplicateWorkItemId(object arg0) => ResourceStrings.Format(nameof (WorkItemPatchDocument_DuplicateWorkItemId), arg0);

    public static string WorkItemPatchDocument_DuplicateWorkItemId(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (WorkItemPatchDocument_DuplicateWorkItemId), culture, arg0);

    public static string RequestBodyMustBeIsPicklistTrue() => ResourceStrings.Get(nameof (RequestBodyMustBeIsPicklistTrue));

    public static string RequestBodyMustBeIsPicklistTrue(CultureInfo culture) => ResourceStrings.Get(nameof (RequestBodyMustBeIsPicklistTrue), culture);

    public static string RuleName() => ResourceStrings.Get(nameof (RuleName));

    public static string RuleName(CultureInfo culture) => ResourceStrings.Get(nameof (RuleName), culture);

    public static string RuleActionNotAllowedOverSystemRule() => ResourceStrings.Get(nameof (RuleActionNotAllowedOverSystemRule));

    public static string RuleActionNotAllowedOverSystemRule(CultureInfo culture) => ResourceStrings.Get(nameof (RuleActionNotAllowedOverSystemRule), culture);

    public static string FieldNotAllowedInActions(object arg0) => ResourceStrings.Format(nameof (FieldNotAllowedInActions), arg0);

    public static string FieldNotAllowedInActions(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (FieldNotAllowedInActions), culture, arg0);

    public static string ConditionsOnDateTimeValue() => ResourceStrings.Get(nameof (ConditionsOnDateTimeValue));

    public static string ConditionsOnDateTimeValue(CultureInfo culture) => ResourceStrings.Get(nameof (ConditionsOnDateTimeValue), culture);

    public static string TargetFieldNotCompatibleWithFieldType(object arg0, object arg1) => ResourceStrings.Format(nameof (TargetFieldNotCompatibleWithFieldType), arg0, arg1);

    public static string TargetFieldNotCompatibleWithFieldType(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (TargetFieldNotCompatibleWithFieldType), culture, arg0, arg1);
    }

    public static string WorkItemStateTransitionInvalidAction(object arg0) => ResourceStrings.Format(nameof (WorkItemStateTransitionInvalidAction), arg0);

    public static string WorkItemStateTransitionInvalidAction(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (WorkItemStateTransitionInvalidAction), culture, arg0);

    public static string FeatureNotSupportedForInheritedProcessMessage() => ResourceStrings.Get(nameof (FeatureNotSupportedForInheritedProcessMessage));

    public static string FeatureNotSupportedForInheritedProcessMessage(CultureInfo culture) => ResourceStrings.Get(nameof (FeatureNotSupportedForInheritedProcessMessage), culture);

    public static string AssignedToMe() => ResourceStrings.Get(nameof (AssignedToMe));

    public static string AssignedToMe(CultureInfo culture) => ResourceStrings.Get(nameof (AssignedToMe), culture);

    public static string Following() => ResourceStrings.Get(nameof (Following));

    public static string Following(CultureInfo culture) => ResourceStrings.Get(nameof (Following), culture);

    public static string Mentioned() => ResourceStrings.Get(nameof (Mentioned));

    public static string Mentioned(CultureInfo culture) => ResourceStrings.Get(nameof (Mentioned), culture);

    public static string MyActivity() => ResourceStrings.Get(nameof (MyActivity));

    public static string MyActivity(CultureInfo culture) => ResourceStrings.Get(nameof (MyActivity), culture);

    public static string WorkItemsHubUrlCreationFailed() => ResourceStrings.Get(nameof (WorkItemsHubUrlCreationFailed));

    public static string WorkItemsHubUrlCreationFailed(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemsHubUrlCreationFailed), culture);

    public static string PredefinedQueriesUrlCreationFailed() => ResourceStrings.Get(nameof (PredefinedQueriesUrlCreationFailed));

    public static string PredefinedQueriesUrlCreationFailed(CultureInfo culture) => ResourceStrings.Get(nameof (PredefinedQueriesUrlCreationFailed), culture);

    public static string EnterpriseWithUrlNotFound(object arg0) => ResourceStrings.Format(nameof (EnterpriseWithUrlNotFound), arg0);

    public static string EnterpriseWithUrlNotFound(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (EnterpriseWithUrlNotFound), culture, arg0);

    public static string WorkItemUrlNotWellFormed(object arg0) => ResourceStrings.Format(nameof (WorkItemUrlNotWellFormed), arg0);

    public static string WorkItemUrlNotWellFormed(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (WorkItemUrlNotWellFormed), culture, arg0);

    public static string QueriesBatchLimitExceeded(object arg0, object arg1) => ResourceStrings.Format(nameof (QueriesBatchLimitExceeded), arg0, arg1);

    public static string QueriesBatchLimitExceeded(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (QueriesBatchLimitExceeded), culture, arg0, arg1);

    public static string RemoteLinkFeatureDisabled() => ResourceStrings.Get(nameof (RemoteLinkFeatureDisabled));

    public static string RemoteLinkFeatureDisabled(CultureInfo culture) => ResourceStrings.Get(nameof (RemoteLinkFeatureDisabled), culture);

    public static string WorkItemPatchDoesNotSupportReplacingAttachedFile() => ResourceStrings.Get(nameof (WorkItemPatchDoesNotSupportReplacingAttachedFile));

    public static string WorkItemPatchDoesNotSupportReplacingAttachedFile(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemPatchDoesNotSupportReplacingAttachedFile), culture);

    public static string LocationServiceException() => ResourceStrings.Get(nameof (LocationServiceException));

    public static string LocationServiceException(CultureInfo culture) => ResourceStrings.Get(nameof (LocationServiceException), culture);

    public static string InvalidEventSource(object arg0) => ResourceStrings.Format(nameof (InvalidEventSource), arg0);

    public static string InvalidEventSource(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (InvalidEventSource), culture, arg0);

    public static string WorkItemForNotRuleActionInvalid(object arg0) => ResourceStrings.Format(nameof (WorkItemForNotRuleActionInvalid), arg0);

    public static string WorkItemForNotRuleActionInvalid(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (WorkItemForNotRuleActionInvalid), culture, arg0);

    public static string WorkItemForNotRuleConditionInvalid(object arg0) => ResourceStrings.Format(nameof (WorkItemForNotRuleConditionInvalid), arg0);

    public static string WorkItemForNotRuleConditionInvalid(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (WorkItemForNotRuleConditionInvalid), culture, arg0);

    public static string WorkItemHideFieldRuleNotSupported() => ResourceStrings.Get(nameof (WorkItemHideFieldRuleNotSupported));

    public static string WorkItemHideFieldRuleNotSupported(CultureInfo culture) => ResourceStrings.Get(nameof (WorkItemHideFieldRuleNotSupported), culture);

    public static string WorkItemInvalidHideFieldRuleCondition(object arg0) => ResourceStrings.Format(nameof (WorkItemInvalidHideFieldRuleCondition), arg0);

    public static string WorkItemInvalidHideFieldRuleCondition(object arg0, CultureInfo culture) => ResourceStrings.Format(nameof (WorkItemInvalidHideFieldRuleCondition), culture, arg0);

    public static string WorkItemInvalidHideFieldRuleMultipleActions(object arg0) => ResourceStrings.Format(nameof (WorkItemInvalidHideFieldRuleMultipleActions), arg0);

    public static string WorkItemInvalidHideFieldRuleMultipleActions(
      object arg0,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (WorkItemInvalidHideFieldRuleMultipleActions), culture, arg0);
    }

    public static string RestoreWorkItemUnsupportedPayload() => ResourceStrings.Get(nameof (RestoreWorkItemUnsupportedPayload));

    public static string RestoreWorkItemUnsupportedPayload(CultureInfo culture) => ResourceStrings.Get(nameof (RestoreWorkItemUnsupportedPayload), culture);

    public static string MigrationAcrossOOBTypesNotSupported() => ResourceStrings.Get(nameof (MigrationAcrossOOBTypesNotSupported));

    public static string MigrationAcrossOOBTypesNotSupported(CultureInfo culture) => ResourceStrings.Get(nameof (MigrationAcrossOOBTypesNotSupported), culture);

    public static string ProjectUsesSameProcess() => ResourceStrings.Get(nameof (ProjectUsesSameProcess));

    public static string ProjectUsesSameProcess(CultureInfo culture) => ResourceStrings.Get(nameof (ProjectUsesSameProcess), culture);

    public static string XMLMigrationNotSupported() => ResourceStrings.Get(nameof (XMLMigrationNotSupported));

    public static string XMLMigrationNotSupported(CultureInfo culture) => ResourceStrings.Get(nameof (XMLMigrationNotSupported), culture);

    public static string NameAndPathInconsistent(object arg0, object arg1, object arg2) => ResourceStrings.Format(nameof (NameAndPathInconsistent), arg0, arg1, arg2);

    public static string NameAndPathInconsistent(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ResourceStrings.Format(nameof (NameAndPathInconsistent), culture, arg0, arg1, arg2);
    }

    public static string NullRuleActionListItem() => ResourceStrings.Get(nameof (NullRuleActionListItem));

    public static string NullRuleActionListItem(CultureInfo culture) => ResourceStrings.Get(nameof (NullRuleActionListItem), culture);

    public static string ParameterExceededThreshold(object arg0, object arg1) => ResourceStrings.Format(nameof (ParameterExceededThreshold), arg0, arg1);

    public static string ParameterExceededThreshold(object arg0, object arg1, CultureInfo culture) => ResourceStrings.Format(nameof (ParameterExceededThreshold), culture, arg0, arg1);

    public static string UpdateWorkItemUnsupportedPayload() => ResourceStrings.Get(nameof (UpdateWorkItemUnsupportedPayload));

    public static string UpdateWorkItemUnsupportedPayload(CultureInfo culture) => ResourceStrings.Get(nameof (UpdateWorkItemUnsupportedPayload), culture);

    public static string ExternalConnectionNoReadPermissions() => ResourceStrings.Get(nameof (ExternalConnectionNoReadPermissions));

    public static string ExternalConnectionNoReadPermissions(CultureInfo culture) => ResourceStrings.Get(nameof (ExternalConnectionNoReadPermissions), culture);

    public static string ExternalConnectionNoWritePermissions() => ResourceStrings.Get(nameof (ExternalConnectionNoWritePermissions));

    public static string ExternalConnectionNoWritePermissions(CultureInfo culture) => ResourceStrings.Get(nameof (ExternalConnectionNoWritePermissions), culture);
  }
}
