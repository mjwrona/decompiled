// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalResourceStrings
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal static class DalResourceStrings
  {
    private static ResourceManager s_resMgr = new ResourceManager("ResourceStrings", typeof (DalResourceStrings).GetTypeInfo().Assembly);
    public const string ArgumentConflictException = "ArgumentConflictException";
    public const string BadAdminDataTypeCannotChangeAfterFieldHasBeenUsed = "BadAdminDataTypeCannotChangeAfterFieldHasBeenUsed";
    public const string DataTypeInvalid = "DataTypeInvalid";
    public const string DeleteFieldFieldDoesNotExist = "DeleteFieldFieldDoesNotExist";
    public const string DeleteFieldFieldInUse = "DeleteFieldFieldInUse";
    public const string FullTextError = "FullTextError";
    public const string GetPageInvalidFieldName = "GetPageInvalidFieldName";
    public const string IAuthorizationServiceInitializeError = "IAuthorizationServiceInitializeError";
    public const string InsertActionMissingFromConstantId = "InsertActionMissingFromConstantId";
    public const string InsertActionMissingToConstantId = "InsertActionMissingToConstantId";
    public const string InsertActionMissingWorkItemTypeId = "InsertActionMissingWorkItemTypeId";
    public const string InsertActionNameTooLong = "InsertActionNameTooLong";
    public const string InsertConstantNameTooLong = "InsertConstantNameTooLong";
    public const string InsertConstantSetMissingConsantId = "InsertConstantSetMissingConsantId";
    public const string InsertFieldInvalidFieldName = "InsertFieldInvalidFieldName";
    public const string InsertFieldInvalidReferenceName = "InsertFieldInvalidReferenceName";
    public const string InsertFieldUsageMissingFieldId = "InsertFieldUsageMissingFieldId";
    public const string InsertTreePropertyNameTooLong = "InsertTreePropertyNameTooLong";
    public const string InsertUpdateFieldInvalidReportingFormula = "InsertUpdateFieldInvalidReportingFormula";
    public const string InsertUpdateFieldInvalidReportingType = "InsertUpdateFieldInvalidReportingType";
    public const string InsertWorkItemTypeMissingNameConsantId = "InsertWorkItemTypeMissingNameConsantId";
    public const string InsertWorkItemTypeUsageMissingFieldID = "InsertWorkItemTypeUsageMissingFieldID";
    public const string InsertWorkItemTypeUsageMissingWorkItemTypeID = "InsertWorkItemTypeUsageMissingWorkItemTypeID";
    public const string InvalidBuildNameException = "InvalidBuildNameException";
    public const string InvalidFieldName = "InvalidFieldName";
    public const string InvalidFieldForOperation = "InvalidFieldForOperation";
    public const string InvalidMetadataTablesRequestedException = "InvalidMetadataTablesRequestedException";
    public const string InvalidNodeIdException = "InvalidNodeIdException";
    public const string InvalidNodeGuidException = "InvalidNodeGuidException";
    public const string InvalidProjectException = "InvalidProjectException";
    public const string InvalidProjectURIException = "InvalidProjectURIException";
    public const string InvalidRowVersionsProvidedException = "InvalidRowVersionsProvidedException";
    public const string InvalidURIException = "InvalidURIException";
    public const string InvalidUserException = "InvalidUserException";
    public const string InvalidWorkIdsRequestedException = "InvalidWorkIdsRequestedException";
    public const string InvalidWorkItemIdException = "InvalidWorkItemIdException";
    public const string InvalidWorkItemIdsException = "InvalidWorkItemIdsException";
    public const string InvalidBatchReadParamException = "InvalidBatchReadParamException";
    public const string MissingAttributeInXmlException = "MissingAttributeInXmlException";
    public const string ErrorNoPermissionCacheOutofdate = "ErrorNoPermissionCacheOutofdate";
    public const string ErrorMetadataCacheOutofdate = "ErrorMetadataCacheOutofdate";
    public const string ErrorFieldNameInUse = "ErrorFieldNameInUse";
    public const string ErrorFieldReferenceNameInUse = "ErrorFieldReferenceNameInUse";
    public const string NestedActionsCannotRemoveLinkException = "NestedActionsCannotRemoveLinkException";
    public const string NestedActionsCannotUpdateLinkException = "NestedActionsCannotUpdateLinkException";
    public const string NestedActionsCantRemoveFileException = "NestedActionsCantRemoveFileException";
    public const string NestedActionsCantRemoveRelationException = "NestedActionsCantRemoveRelationException";
    public const string NestedActionsCantUpdateRelationException = "NestedActionsCantUpdateRelationException";
    public const string NestedActionsDenyDependencyMissingWorkItemIdException = "NestedActionsDenyDependencyMissingWorkItemIdException";
    public const string NullUpdateElementException = "NullUpdateElementException";
    public const string QueryExpandConstantFlagNotValidWithOperatorException = "QueryExpandConstantFlagNotValidWithOperatorException";
    public const string QueryExpressionOperatorNotSupportedException = "QueryExpressionOperatorNotSupportedException";
    public const string QueryGroupOperatorNotSupportedException = "QueryGroupOperatorNotSupportedException";
    public const string QueryIDsInvalidColumnName = "QueryIDsInvalidColumnName";
    public const string QueryInvalidAsOfDate = "QueryInvalidAsOfDate";
    public const string QueryInvalidDateValueException = "QueryInvalidDateValueException";
    public const string QueryInvalidDoubleValueException = "QueryInvalidDoubleValueException";
    public const string QueryInvalidEverContainsException = "QueryInvalidEverContainsException";
    public const string QueryInvalidExcludeLowerException = "QueryInvalidExcludeLowerException";
    public const string QueryInvalidExcludeUpperException = "QueryInvalidExcludeUpperException";
    public const string QueryInvalidExcludeUpperLowerException = "QueryInvalidExcludeUpperLowerException";
    public const string QueryInvalidExpandConstantException = "QueryInvalidExpandConstantException";
    public const string QueryInvalidExpandFlagException = "QueryInvalidExpandFlagException";
    public const string QueryInvalidExpressionOperatorException = "QueryInvalidExpressionOperatorException";
    public const string QueryInvalidFieldTypeException = "QueryInvalidFieldTypeException";
    public const string QueryInvalidLongTextException = "QueryInvalidLongTextException";
    public const string QueryInvalidLongTextOperator = "QueryInvalidLongTextOperator";
    public const string QueryInvalidNumberOfValuesInExpressionException = "QueryInvalidNumberOfValuesInExpressionException";
    public const string QueryInvalidNumberValueException = "QueryInvalidNumberValueException";
    public const string QueryInvalidSortOrderColumnException = "QueryInvalidSortOrderColumnException";
    public const string QueryInvalidTempIdException = "QueryInvalidTempIdException";
    public const string QueryInvalidValueType = "QueryInvalidValueType";
    public const string QueryInvalidValueTypeForContainsOperator = "QueryInvalidValueTypeForContainsOperator";
    public const string QueryInvalidValueLength = "QueryInvalidValueLength";
    public const string QueryMissingColumnNameForExpressionException = "QueryMissingColumnNameForExpressionException";
    public const string QueryMissingGroupsOrExpressionsException = "QueryMissingGroupsOrExpressionsException";
    public const string QueryParameterOutOfRange = "QueryParameterOutOfRange";
    public const string UnexpectedReturnedDataSetException = "UnexpectedReturnedDataSetException";
    public const string UnexpectedSqlError = "UnexpectedSqlError";
    public const string UpdateActionNothingToUpdate = "UpdateActionNothingToUpdate";
    public const string UpdateActionNotSupportedException = "UpdateActionNotSupportedException";
    public const string UpdateDependencyInvalidIssueIdException = "UpdateDependencyInvalidIssueIdException";
    public const string UpdateDuplicateTempIdsInUpdateXmlException = "UpdateDuplicateTempIdsInUpdateXmlException";
    public const string UpdateInsertFileFailedToFindFile = "UpdateInsertFileFailedToFindFile";
    public const string UpdateInsertFileInvalidCreationDate = "UpdateInsertFileInvalidCreationDate";
    public const string UpdateInsertFileInvalidFileNameGuidException = "UpdateInsertFileInvalidFileNameGuidException";
    public const string UpdateInsertFileTooManyAttachments = "UpdateInsertFileTooManyAttachments";
    public const string UpdateInvalidAttributeIntegerException = "UpdateInvalidAttributeIntegerException";
    public const string UpdateInvalidBooleanAttributeException = "UpdateInvalidBooleanAttributeException";
    public const string UpdateInvalidColumnName = "UpdateInvalidColumnName";
    public const string UpdateInvalidGuidAttributeException = "UpdateInvalidGuidAttributeException";
    public const string UpdateInvalidValueIntegerException = "UpdateInvalidValueIntegerException";
    public const string UpdateMissingColumnValueException = "UpdateMissingColumnValueException";
    public const string UpdateMissingRequiredIdException = "UpdateMissingRequiredIdException";
    public const string UpdateNoActionsInUpdateXmlException = "UpdateNoActionsInUpdateXmlException";
    public const string UpdateRemoveFileIssueIdException = "UpdateRemoveFileIssueIdException";
    public const string UpdateRemoveResourceLinkIdException = "UpdateRemoveResourceLinkIdException";
    public const string UpdateResourceLinkIdException = "UpdateResourceLinkIdException";
    public const string UpdateTooManySqlParameters = "UpdateTooManySqlParameters";
    public const string UpdateWorkItemTypeNothingToUpdate = "UpdateWorkItemTypeNothingToUpdate";
    public const string UpdateWorkItemTypeUsageNothingToUpdate = "UpdateWorkItemTypeUsageNothingToUpdate";
    public const string WorkIdsRequestedOverLimitException = "WorkIdsRequestedOverLimitException";
    public const string FireWorkItemEventFailed = "FireWorkItemEventFailed";
    public const string FireWorkItemEventFailedToQueueEvent = "FireWorkItemEventFailedToQueueEvent";
    public const string InvalidMessage = "InvalidMessage";
    public const string ThreadPoolQueueUserWorkItemFailed = "ThreadPoolQueueUserWorkItemFailed";
    public const string ReportCountersNotInstalled = "ReportCountersNotInstalled";
    public const string ParameterNotNullOrEmpty = "ParameterNotNullOrEmpty";
    public const string ReadXmlNotImplemented = "ReadXmlNotImplemented";
    public const string AccessDeniedOrDoesNotExist = "AccessDeniedOrDoesNotExist";
    public const string UpdateWorkItemMultipleTimes = "UpdateWorkItemMultipleTimes";
    public const string UpdateWorkItemTooManyWorkitems = "UpdateWorkItemTooManyWorkitems";
    public const string MissingElementInAction = "MissingElementInAction";
    public const string RepeatedElementInAction = "RepeatedElementInAction";
    public const string DuplicateTableName = "DuplicateTableName";
    public const string TableAlreadyInCollection = "TableAlreadyInCollection";
    public const string TableNotFound = "TableNotFound";
    public const string UnrecognisedTypeToSerialize = "UnrecognisedTypeToSerialize";
    public const string QueryMissingValueForContainsOperator = "QueryMissingValueForContainsOperator";
    public const string CancelledByUser = "CancelledByUser";
    public const string LinkTypeChangesetArtifactName = "LinkTypeChangesetArtifactName";
    public const string LinkTypeChangeset = "LinkTypeChangeset";
    public const string LinkTypeSourceCodeArtifactName = "LinkTypeSourceCodeArtifactName";
    public const string LinkTypeSourceCode = "LinkTypeSourceCode";
    public const string LinkTypeTestResultArtifactName = "LinkTypeTestResultArtifactName";
    public const string LinkTypeTestResult = "LinkTypeTestResult";
    public const string InvalidWorkItemId = "InvalidWorkItemId";
    public const string ProjectIdNotPositive = "ProjectIdNotPositive";
    public const string DuplicateColumnSetOnWorkItem = "DuplicateColumnSetOnWorkItem";
    public const string DuplicateColumnSetOnNewWorkItem = "DuplicateColumnSetOnNewWorkItem";
    public const string MaxLongTextSizeExceeded = "MaxLongTextSizeExceeded";
    public const string InsertWorkItemMissingAreadID = "InsertWorkItemMissingAreadID";
    public const string InsertConstantWithBackslash = "InsertConstantWithBackslash";
    public const string InsertLinkTypeInvalidReferenceName = "InsertLinkTypeInvalidReferenceName";
    public const string InsertLinkTypeInvalidFriendlyName = "InsertLinkTypeInvalidFriendlyName";
    public const string InsertLinkCannotUseDisabledType = "InsertLinkCannotUseDisabledType";
    public const string InvalidLinkQuerySortOrder = "InvalidLinkQuerySortOrder";
    public const string InvalidLinkQueryRecursionID = "InvalidLinkQueryRecursionID";
    public const string InvalidLinkQueryInvalidType = "InvalidLinkQueryInvalidType";
    public const string InvalidLinkSubQuery = "InvalidLinkSubQuery";
    public const string UserInputLoop = "UserInputLoop";
    public const string SharedQueries = "SharedQueries";
    public const string PrivateQueries = "PrivateQueries";
    public const string InsertWorkItemTypeCategoryInvalidFriendlyName = "InsertWorkItemTypeCategoryInvalidFriendlyName";
    public const string InsertWorkItemTypeCategoryInvalidReferenceName = "InsertWorkItemTypeCategoryInvalidReferenceName";
    public const string InsertWorkItemTypeCategoryMemberMissingCategory = "InsertWorkItemTypeCategoryMemberMissingCategory";
    public const string InsertWorkItemTypeCategoryMemberMissingWorkItemType = "InsertWorkItemTypeCategoryMemberMissingWorkItemType";
    public const string InsertWorkItemTypeCategoryMissingWorkItemType = "InsertWorkItemTypeCategoryMissingWorkItemType";
    public const string WorkItemTypeCategoryTypeNotInProject = "WorkItemTypeCategoryTypeNotInProject";
    public const string ErrorDuplicateCatRefName = "ErrorDuplicateCatRefName";
    public const string ErrorDuplicateCatName = "ErrorDuplicateCatName";
    public const string WorkItemTypeCategoryCannotDeleteDefaultType = "WorkItemTypeCategoryCannotDeleteDefaultType";
    public const string ErrorInvalidQueryId = "ErrorInvalidQueryId";
    public const string QueryHierarchyNameCannotBeEmpty = "QueryHierarchyNameCannotBeEmpty";
    public const string QueryHierarchyQueryTextCannotBeEmpty = "QueryHierarchyQueryTextCannotBeEmpty";
    public const string QueryHierarchyItemDoesNotExist = "QueryHierarchyItemDoesNotExist";
    public const string QueryHierarchyInvalidPermissionsSet = "QueryHierarchyInvalidPermissionsSet";
    public const string QueryHierarchyIrrevocablePermissionsRemoved = "QueryHierarchyIrrevocablePermissionsRemoved";
    public const string QueryHierarchyCannotSetAllowAndDeny = "QueryHierarchyCannotSetAllowAndDeny";
    public const string QueryHierarchyMustAllowAdditionalPermissions = "QueryHierarchyMustAllowAdditionalPermissions";
    public const string QueryHierarchyMustDenyAdditionalPermissions = "QueryHierarchyMustDenyAdditionalPermissions";
    public const string QueryHierarchyMustAllowAllPermissions = "QueryHierarchyMustAllowAllPermissions";
    public const string QueryHierarchyMustDenyAllPermissions = "QueryHierarchyMustDenyAllPermissions";
    public const string QueryHierarchyItemAccessException = "QueryHierarchyItemAccessException";
    public const string QueryHierarchyChildrenAccessException = "QueryHierarchyChildrenAccessException";
    public const string STORED_QUERY_Permission_Read = "STORED_QUERY_Permission_Read";
    public const string STORED_QUERY_Permission_Contribute = "STORED_QUERY_Permission_Contribute";
    public const string STORED_QUERY_Permission_Delete = "STORED_QUERY_Permission_Delete";
    public const string STORED_QUERY_Permission_ManagePermissions = "STORED_QUERY_Permission_ManagePermissions";
    public const string STORED_QUERY_Permission_FullControl = "STORED_QUERY_Permission_FullControl";
    public const string STORED_QUERY_PermissionDescription_Read = "STORED_QUERY_PermissionDescription_Read";
    public const string STORED_QUERY_PermissionDescription_Contribute = "STORED_QUERY_PermissionDescription_Contribute";
    public const string STORED_QUERY_PermissionDescription_Delete = "STORED_QUERY_PermissionDescription_Delete";
    public const string STORED_QUERY_PermissionDescription_ManagePermissions = "STORED_QUERY_PermissionDescription_ManagePermissions";
    public const string STORED_QUERY_PermissionDescription_FullControl = "STORED_QUERY_PermissionDescription_FullControl";
    public const string STORED_QUERY_FOLDER_Permission_Read = "STORED_QUERY_FOLDER_Permission_Read";
    public const string STORED_QUERY_FOLDER_Permission_Contribute = "STORED_QUERY_FOLDER_Permission_Contribute";
    public const string STORED_QUERY_FOLDER_Permission_Delete = "STORED_QUERY_FOLDER_Permission_Delete";
    public const string STORED_QUERY_FOLDER_Permission_ManagePermissions = "STORED_QUERY_FOLDER_Permission_ManagePermissions";
    public const string STORED_QUERY_FOLDER_Permission_FullControl = "STORED_QUERY_FOLDER_Permission_FullControl";
    public const string STORED_QUERY_FOLDER_PermissionDescription_Read = "STORED_QUERY_FOLDER_PermissionDescription_Read";
    public const string STORED_QUERY_FOLDER_PermissionDescription_Contribute = "STORED_QUERY_FOLDER_PermissionDescription_Contribute";
    public const string STORED_QUERY_FOLDER_PermissionDescription_Delete = "STORED_QUERY_FOLDER_PermissionDescription_Delete";
    public const string STORED_QUERY_FOLDER_PermissionDescription_ManagePermissions = "STORED_QUERY_FOLDER_PermissionDescription_ManagePermissions";
    public const string STORED_QUERY_FOLDER_PermissionDescription_FullControl = "STORED_QUERY_FOLDER_PermissionDescription_FullControl";
    public const string CannotImportFromOlderClient = "CannotImportFromOlderClient";
    public const string CannotEditLinkFromOlderClient = "CannotEditLinkFromOlderClient";
    public const string CannotEditLink = "CannotEditLink";
    public const string QueryDuplicateSortOrderColumnException = "QueryDuplicateSortOrderColumnException";
    public const string UpdateInvalidComputedColumnName = "UpdateInvalidComputedColumnName";
    public const string FileUploadUserNotAuthorized = "FileUploadUserNotAuthorized";
    public const string UpdateWorkItemFailedToGetSafeHtmlString = "UpdateWorkItemFailedToGetSafeHtmlString";
    public const string ErrorAddConstantStringNotSupportedByCollation = "ErrorAddConstantStringNotSupportedByCollation";
    public const string InvalidWorkItemQueryTimeout = "InvalidWorkItemQueryTimeout";
    public const string AttachmentsDestoyedComment = "AttachmentsDestoyedComment";
    public const string ManageLinkTypesDenied = "ManageLinkTypesDenied";
    public const string UpdateTempIdNotDefined = "UpdateTempIdNotDefined";
    public const string InvalidFieldId = "InvalidFieldId";
    public const string UnresolvableQueryPermissionIdentity = "UnresolvableQueryPermissionIdentity";
    public const string CannotQueryOnTags = "CannotQueryOnTags";
    public const string AddedTags = "AddedTags";
    public const string RemovedTags = "RemovedTags";
    public const string DeletedPostFix = "DeletedPostFix";
    public const string CannotQueryFolder = "CannotQueryFolder";
    public const string QueryHasNoProject = "QueryHasNoProject";
    public const string IncorrectQueryType = "IncorrectQueryType";
    public const string QueryMustBeFlat = "QueryMustBeFlat";
    public const string UnexpectedWorkItemFieldValue = "UnexpectedWorkItemFieldValue";
    public const string UnexpectedWorkItemUpdateResult = "UnexpectedWorkItemUpdateResult";
    public const string UnknownWorkItemField = "UnknownWorkItemField";
    public const string Tags_InvalidTagName = "Tags_InvalidTagName";
    public const string Tags_NonExistingTag = "Tags_NonExistingTag";
    public const string Tags_InvalidScope = "Tags_InvalidScope";
    public const string Tags_DuplicateTagSequence = "Tags_DuplicateTagSequence";
    public const string Tags_AddedAndRemovedContainSameTag = "Tags_AddedAndRemovedContainSameTag";
    public const string Tags_CannotHaveRemovedTagsForNewWorkItem = "Tags_CannotHaveRemovedTagsForNewWorkItem";
    public const string TooManyNewTagsAdded = "TooManyNewTagsAdded";
    public const string QueryInvalidBooleanValueException = "QueryInvalidBooleanValueException";
    public const string QueryInvalidGuidValueException = "QueryInvalidGuidValueException";
    public const string InvalidRange = "InvalidRange";
    public const string WrongSeriesForTrendData = "WrongSeriesForTrendData";
    public const string WorkItemsPluralName = "WorkItemsPluralName";
    public const string MismatchedUserForAssignedToMeGroupKey = "MismatchedUserForAssignedToMeGroupKey";
    public const string MissingUserForAssignedToMeGroupKey = "MissingUserForAssignedToMeGroupKey";
    public const string QueryChartIdMismatch = "QueryChartIdMismatch";
    public const string TestCaseDataFieldCannotBeModified = "TestCaseDataFieldCannotBeModified";
    public const string SharedStepSaveException = "SharedStepSaveException";
    public const string ErrorConversionFailed = "ErrorConversionFailed";
    public const string TooManyQueryItemUnderParent = "TooManyQueryItemUnderParent";
    public const string TooManyQueryItemsReturned = "TooManyQueryItemsReturned";
    public const string ChartingFlatQueriesOnly = "ChartingFlatQueriesOnly";
    public const string ChartingRequiresWorkItemResponse = "ChartingRequiresWorkItemResponse";
    public const string ErrorMultipleQueriesTasks = "ErrorMultipleQueriesTasks";
    public const string ErrorMissingQueryName = "ErrorMissingQueryName";
    public const string ErrorMissingQueryText = "ErrorMissingQueryText";
    public const string ErrorQueryFileDoesntExist = "ErrorQueryFileDoesntExist";
    public const string ErrorQueryFileMalformed = "ErrorQueryFileMalformed";
    public const string ErrorWiqlInvalidFieldName = "ErrorWiqlInvalidFieldName";
    public const string ErrorInvalidQueryPermissionName = "ErrorInvalidQueryPermissionName";
    public const string ErrorNoneQueryPermission = "ErrorNoneQueryPermission";
    public const string ErrorInvalidQueryPermission = "ErrorInvalidQueryPermission";
    public const string ErrorProjectGroupNotDefined = "ErrorProjectGroupNotDefined";
    public const string ErrorMissingIdentity = "ErrorMissingIdentity";
    public const string ErrorTaskNotSupported = "ErrorTaskNotSupported";
    public const string ErrorQueryReferencesInvalidPath = "ErrorQueryReferencesInvalidPath";
    public const string GetMetadataServerBusy = "GetMetadataServerBusy";
    public const string GetMetadataTooManyConcurrentUsers = "GetMetadataTooManyConcurrentUsers";
    public const string CannotUpdateCurrentIterationQueryInVS = "CannotUpdateCurrentIterationQueryInVS";
    public const string CurrentIteration_TeamRequired = "CurrentIteration_TeamRequired";

    public static ResourceManager Manager => DalResourceStrings.s_resMgr;

    public static string Get(string resourceName) => DalResourceStrings.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    public static string Get(string resourceName, CultureInfo culture) => culture == null ? DalResourceStrings.Get(resourceName) : DalResourceStrings.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) DalResourceStrings.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? DalResourceStrings.GetInt(resourceName) : (int) DalResourceStrings.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) DalResourceStrings.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? DalResourceStrings.GetBool(resourceName) : (bool) DalResourceStrings.s_resMgr.GetObject(resourceName, culture);

    public static string Format(string resourceName, params object[] args) => DalResourceStrings.Format(resourceName, CultureInfo.CurrentUICulture, args);

    public static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = DalResourceStrings.Get(resourceName, culture);
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
  }
}
