// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal static class ServerResources
  {
    private static ResourceManager s_resMgr = new ResourceManager("Resources", typeof (ServerResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => ServerResources.s_resMgr;

    private static string Get(string resourceName) => ServerResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? ServerResources.Get(resourceName) : ServerResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) ServerResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? ServerResources.GetInt(resourceName) : (int) ServerResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) ServerResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? ServerResources.GetBool(resourceName) : (bool) ServerResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => ServerResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = ServerResources.Get(resourceName, culture);
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

    public static string ErrorSerializedExtensionPredicateTooLong() => ServerResources.Get(nameof (ErrorSerializedExtensionPredicateTooLong));

    public static string ErrorSerializedExtensionPredicateTooLong(CultureInfo culture) => ServerResources.Get(nameof (ErrorSerializedExtensionPredicateTooLong), culture);

    public static string InvalidFieldId(object arg0) => ServerResources.Format(nameof (InvalidFieldId), arg0);

    public static string InvalidFieldId(object arg0, CultureInfo culture) => ServerResources.Format(nameof (InvalidFieldId), culture, arg0);

    public static string InvalidFieldName(object arg0) => ServerResources.Format(nameof (InvalidFieldName), arg0);

    public static string InvalidFieldName(object arg0, CultureInfo culture) => ServerResources.Format(nameof (InvalidFieldName), culture, arg0);

    public static string InvalidNodeGuidException(object arg0) => ServerResources.Format(nameof (InvalidNodeGuidException), arg0);

    public static string InvalidNodeGuidException(object arg0, CultureInfo culture) => ServerResources.Format(nameof (InvalidNodeGuidException), culture, arg0);

    public static string InvalidNodeIdException(object arg0) => ServerResources.Format(nameof (InvalidNodeIdException), arg0);

    public static string InvalidNodeIdException(object arg0, CultureInfo culture) => ServerResources.Format(nameof (InvalidNodeIdException), culture, arg0);

    public static string InvalidNodeNameException(object arg0) => ServerResources.Format(nameof (InvalidNodeNameException), arg0);

    public static string InvalidNodeNameException(object arg0, CultureInfo culture) => ServerResources.Format(nameof (InvalidNodeNameException), culture, arg0);

    public static string InvalidWorkItemQueryTimeout() => ServerResources.Get(nameof (InvalidWorkItemQueryTimeout));

    public static string InvalidWorkItemQueryTimeout(CultureInfo culture) => ServerResources.Get(nameof (InvalidWorkItemQueryTimeout), culture);

    public static string Tags_AddedAndRemovedContainSameTag() => ServerResources.Get(nameof (Tags_AddedAndRemovedContainSameTag));

    public static string Tags_AddedAndRemovedContainSameTag(CultureInfo culture) => ServerResources.Get(nameof (Tags_AddedAndRemovedContainSameTag), culture);

    public static string Tags_CannotHaveRemovedTagsForNewWorkItem() => ServerResources.Get(nameof (Tags_CannotHaveRemovedTagsForNewWorkItem));

    public static string Tags_CannotHaveRemovedTagsForNewWorkItem(CultureInfo culture) => ServerResources.Get(nameof (Tags_CannotHaveRemovedTagsForNewWorkItem), culture);

    public static string Tags_DuplicateTagSequence(object arg0) => ServerResources.Format(nameof (Tags_DuplicateTagSequence), arg0);

    public static string Tags_DuplicateTagSequence(object arg0, CultureInfo culture) => ServerResources.Format(nameof (Tags_DuplicateTagSequence), culture, arg0);

    public static string Tags_InvalidScope(object arg0) => ServerResources.Format(nameof (Tags_InvalidScope), arg0);

    public static string Tags_InvalidScope(object arg0, CultureInfo culture) => ServerResources.Format(nameof (Tags_InvalidScope), culture, arg0);

    public static string Tags_InvalidTagName(object arg0) => ServerResources.Format(nameof (Tags_InvalidTagName), arg0);

    public static string Tags_InvalidTagName(object arg0, CultureInfo culture) => ServerResources.Format(nameof (Tags_InvalidTagName), culture, arg0);

    public static string Tags_NonExistingTag(object arg0) => ServerResources.Format(nameof (Tags_NonExistingTag), arg0);

    public static string Tags_NonExistingTag(object arg0, CultureInfo culture) => ServerResources.Format(nameof (Tags_NonExistingTag), culture, arg0);

    public static string UpdateDuplicateTempIdsInUpdateXmlException() => ServerResources.Get(nameof (UpdateDuplicateTempIdsInUpdateXmlException));

    public static string UpdateDuplicateTempIdsInUpdateXmlException(CultureInfo culture) => ServerResources.Get(nameof (UpdateDuplicateTempIdsInUpdateXmlException), culture);

    public static string UpdateWorkItemMultipleTimes(object arg0) => ServerResources.Format(nameof (UpdateWorkItemMultipleTimes), arg0);

    public static string UpdateWorkItemMultipleTimes(object arg0, CultureInfo culture) => ServerResources.Format(nameof (UpdateWorkItemMultipleTimes), culture, arg0);

    public static string QueryNotFound(object arg0) => ServerResources.Format(nameof (QueryNotFound), arg0);

    public static string QueryNotFound(object arg0, CultureInfo culture) => ServerResources.Format(nameof (QueryNotFound), culture, arg0);

    public static string WorkItemNotFoundOrAccessDenied(object arg0) => ServerResources.Format(nameof (WorkItemNotFoundOrAccessDenied), arg0);

    public static string WorkItemNotFoundOrAccessDenied(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemNotFoundOrAccessDenied), culture, arg0);

    public static string CannotQueryOnTags() => ServerResources.Get(nameof (CannotQueryOnTags));

    public static string CannotQueryOnTags(CultureInfo culture) => ServerResources.Get(nameof (CannotQueryOnTags), culture);

    public static string InvalidLinkQueryInvalidType(object arg0) => ServerResources.Format(nameof (InvalidLinkQueryInvalidType), arg0);

    public static string InvalidLinkQueryInvalidType(object arg0, CultureInfo culture) => ServerResources.Format(nameof (InvalidLinkQueryInvalidType), culture, arg0);

    public static string InvalidLinkQueryRecursionID(object arg0) => ServerResources.Format(nameof (InvalidLinkQueryRecursionID), arg0);

    public static string InvalidLinkQueryRecursionID(object arg0, CultureInfo culture) => ServerResources.Format(nameof (InvalidLinkQueryRecursionID), culture, arg0);

    public static string InvalidLinkQuerySortOrder(object arg0) => ServerResources.Format(nameof (InvalidLinkQuerySortOrder), arg0);

    public static string InvalidLinkQuerySortOrder(object arg0, CultureInfo culture) => ServerResources.Format(nameof (InvalidLinkQuerySortOrder), culture, arg0);

    public static string InvalidLinkSubQuery(object arg0) => ServerResources.Format(nameof (InvalidLinkSubQuery), arg0);

    public static string InvalidLinkSubQuery(object arg0, CultureInfo culture) => ServerResources.Format(nameof (InvalidLinkSubQuery), culture, arg0);

    public static string QueryDepthTooLargeException(object arg0) => ServerResources.Format(nameof (QueryDepthTooLargeException), arg0);

    public static string QueryDepthTooLargeException(object arg0, CultureInfo culture) => ServerResources.Format(nameof (QueryDepthTooLargeException), culture, arg0);

    public static string QueryDuplicateSortOrderColumnException() => ServerResources.Get(nameof (QueryDuplicateSortOrderColumnException));

    public static string QueryDuplicateSortOrderColumnException(CultureInfo culture) => ServerResources.Get(nameof (QueryDuplicateSortOrderColumnException), culture);

    public static string QueryExpandConstantFlagNotValidWithOperatorException() => ServerResources.Get(nameof (QueryExpandConstantFlagNotValidWithOperatorException));

    public static string QueryExpandConstantFlagNotValidWithOperatorException(CultureInfo culture) => ServerResources.Get(nameof (QueryExpandConstantFlagNotValidWithOperatorException), culture);

    public static string QueryExpressionOperatorNotSupportedException() => ServerResources.Get(nameof (QueryExpressionOperatorNotSupportedException));

    public static string QueryExpressionOperatorNotSupportedException(CultureInfo culture) => ServerResources.Get(nameof (QueryExpressionOperatorNotSupportedException), culture);

    public static string QueryGroupOperatorNotSupportedException() => ServerResources.Get(nameof (QueryGroupOperatorNotSupportedException));

    public static string QueryGroupOperatorNotSupportedException(CultureInfo culture) => ServerResources.Get(nameof (QueryGroupOperatorNotSupportedException), culture);

    public static string QueryIDsInvalidColumnName() => ServerResources.Get(nameof (QueryIDsInvalidColumnName));

    public static string QueryIDsInvalidColumnName(CultureInfo culture) => ServerResources.Get(nameof (QueryIDsInvalidColumnName), culture);

    public static string QueryInvalidAsOfDate() => ServerResources.Get(nameof (QueryInvalidAsOfDate));

    public static string QueryInvalidAsOfDate(CultureInfo culture) => ServerResources.Get(nameof (QueryInvalidAsOfDate), culture);

    public static string QueryInvalidDateValueException() => ServerResources.Get(nameof (QueryInvalidDateValueException));

    public static string QueryInvalidDateValueException(CultureInfo culture) => ServerResources.Get(nameof (QueryInvalidDateValueException), culture);

    public static string QueryInvalidDoubleValueException() => ServerResources.Get(nameof (QueryInvalidDoubleValueException));

    public static string QueryInvalidDoubleValueException(CultureInfo culture) => ServerResources.Get(nameof (QueryInvalidDoubleValueException), culture);

    public static string QueryInvalidEverContainsException() => ServerResources.Get(nameof (QueryInvalidEverContainsException));

    public static string QueryInvalidEverContainsException(CultureInfo culture) => ServerResources.Get(nameof (QueryInvalidEverContainsException), culture);

    public static string QueryInvalidExpandConstantException() => ServerResources.Get(nameof (QueryInvalidExpandConstantException));

    public static string QueryInvalidExpandConstantException(CultureInfo culture) => ServerResources.Get(nameof (QueryInvalidExpandConstantException), culture);

    public static string QueryInvalidExpandFlagException() => ServerResources.Get(nameof (QueryInvalidExpandFlagException));

    public static string QueryInvalidExpandFlagException(CultureInfo culture) => ServerResources.Get(nameof (QueryInvalidExpandFlagException), culture);

    public static string QueryInvalidExpressionOperatorException() => ServerResources.Get(nameof (QueryInvalidExpressionOperatorException));

    public static string QueryInvalidExpressionOperatorException(CultureInfo culture) => ServerResources.Get(nameof (QueryInvalidExpressionOperatorException), culture);

    public static string QueryInvalidFieldTypeException() => ServerResources.Get(nameof (QueryInvalidFieldTypeException));

    public static string QueryInvalidFieldTypeException(CultureInfo culture) => ServerResources.Get(nameof (QueryInvalidFieldTypeException), culture);

    public static string QueryInvalidLongTextException() => ServerResources.Get(nameof (QueryInvalidLongTextException));

    public static string QueryInvalidLongTextException(CultureInfo culture) => ServerResources.Get(nameof (QueryInvalidLongTextException), culture);

    public static string QueryInvalidLongTextOperator() => ServerResources.Get(nameof (QueryInvalidLongTextOperator));

    public static string QueryInvalidLongTextOperator(CultureInfo culture) => ServerResources.Get(nameof (QueryInvalidLongTextOperator), culture);

    public static string QueryInvalidNumberOfValuesInExpressionException() => ServerResources.Get(nameof (QueryInvalidNumberOfValuesInExpressionException));

    public static string QueryInvalidNumberOfValuesInExpressionException(CultureInfo culture) => ServerResources.Get(nameof (QueryInvalidNumberOfValuesInExpressionException), culture);

    public static string QueryInvalidNumberValueException() => ServerResources.Get(nameof (QueryInvalidNumberValueException));

    public static string QueryInvalidNumberValueException(CultureInfo culture) => ServerResources.Get(nameof (QueryInvalidNumberValueException), culture);

    public static string QueryInvalidValueLength(object arg0, object arg1) => ServerResources.Format(nameof (QueryInvalidValueLength), arg0, arg1);

    public static string QueryInvalidValueLength(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (QueryInvalidValueLength), culture, arg0, arg1);

    public static string QueryInvalidValueType() => ServerResources.Get(nameof (QueryInvalidValueType));

    public static string QueryInvalidValueType(CultureInfo culture) => ServerResources.Get(nameof (QueryInvalidValueType), culture);

    public static string QueryInvalidValueTypeForContainsOperator() => ServerResources.Get(nameof (QueryInvalidValueTypeForContainsOperator));

    public static string QueryInvalidValueTypeForContainsOperator(CultureInfo culture) => ServerResources.Get(nameof (QueryInvalidValueTypeForContainsOperator), culture);

    public static string QueryMissingColumnNameForExpressionException() => ServerResources.Get(nameof (QueryMissingColumnNameForExpressionException));

    public static string QueryMissingColumnNameForExpressionException(CultureInfo culture) => ServerResources.Get(nameof (QueryMissingColumnNameForExpressionException), culture);

    public static string QueryMissingGroupsOrExpressionsException() => ServerResources.Get(nameof (QueryMissingGroupsOrExpressionsException));

    public static string QueryMissingGroupsOrExpressionsException(CultureInfo culture) => ServerResources.Get(nameof (QueryMissingGroupsOrExpressionsException), culture);

    public static string QueryMissingValueForContainsOperator() => ServerResources.Get(nameof (QueryMissingValueForContainsOperator));

    public static string QueryMissingValueForContainsOperator(CultureInfo culture) => ServerResources.Get(nameof (QueryMissingValueForContainsOperator), culture);

    public static string UpdateTooManySqlParameters() => ServerResources.Get(nameof (UpdateTooManySqlParameters));

    public static string UpdateTooManySqlParameters(CultureInfo culture) => ServerResources.Get(nameof (UpdateTooManySqlParameters), culture);

    public static string MaxLongTextSizeExceeded(object arg0, object arg1) => ServerResources.Format(nameof (MaxLongTextSizeExceeded), arg0, arg1);

    public static string MaxLongTextSizeExceeded(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (MaxLongTextSizeExceeded), culture, arg0, arg1);

    public static string QueryExpressionInvalidChildNode() => ServerResources.Get(nameof (QueryExpressionInvalidChildNode));

    public static string QueryExpressionInvalidChildNode(CultureInfo culture) => ServerResources.Get(nameof (QueryExpressionInvalidChildNode), culture);

    public static string QueryExpressionInvalidNullField() => ServerResources.Get(nameof (QueryExpressionInvalidNullField));

    public static string QueryExpressionInvalidNullField(CultureInfo culture) => ServerResources.Get(nameof (QueryExpressionInvalidNullField), culture);

    public static string QueryExpressionInvalidOptimizationLongText() => ServerResources.Get(nameof (QueryExpressionInvalidOptimizationLongText));

    public static string QueryExpressionInvalidOptimizationLongText(CultureInfo culture) => ServerResources.Get(nameof (QueryExpressionInvalidOptimizationLongText), culture);

    public static string QueryExpressionInvalidPredicateArrayEmpty() => ServerResources.Get(nameof (QueryExpressionInvalidPredicateArrayEmpty));

    public static string QueryExpressionInvalidPredicateArrayEmpty(CultureInfo culture) => ServerResources.Get(nameof (QueryExpressionInvalidPredicateArrayEmpty), culture);

    public static string QueryExpressionInvalidPredicateArrayNested() => ServerResources.Get(nameof (QueryExpressionInvalidPredicateArrayNested));

    public static string QueryExpressionInvalidPredicateArrayNested(CultureInfo culture) => ServerResources.Get(nameof (QueryExpressionInvalidPredicateArrayNested), culture);

    public static string QueryExpressionInvalidPredicateArrayOperator() => ServerResources.Get(nameof (QueryExpressionInvalidPredicateArrayOperator));

    public static string QueryExpressionInvalidPredicateArrayOperator(CultureInfo culture) => ServerResources.Get(nameof (QueryExpressionInvalidPredicateArrayOperator), culture);

    public static string QueryExpressionInvalidPredicateArrayTypeMismatch() => ServerResources.Get(nameof (QueryExpressionInvalidPredicateArrayTypeMismatch));

    public static string QueryExpressionInvalidPredicateArrayTypeMismatch(CultureInfo culture) => ServerResources.Get(nameof (QueryExpressionInvalidPredicateArrayTypeMismatch), culture);

    public static string QueryExpressionInvalidPredicateEverContains() => ServerResources.Get(nameof (QueryExpressionInvalidPredicateEverContains));

    public static string QueryExpressionInvalidPredicateEverContains(CultureInfo culture) => ServerResources.Get(nameof (QueryExpressionInvalidPredicateEverContains), culture);

    public static string QueryExpressionInvalidPredicateExpandFlag(object arg0) => ServerResources.Format(nameof (QueryExpressionInvalidPredicateExpandFlag), arg0);

    public static string QueryExpressionInvalidPredicateExpandFlag(object arg0, CultureInfo culture) => ServerResources.Format(nameof (QueryExpressionInvalidPredicateExpandFlag), culture, arg0);

    public static string QueryExpressionInvalidPredicateLinkType() => ServerResources.Get(nameof (QueryExpressionInvalidPredicateLinkType));

    public static string QueryExpressionInvalidPredicateLinkType(CultureInfo culture) => ServerResources.Get(nameof (QueryExpressionInvalidPredicateLinkType), culture);

    public static string QueryExpressionInvalidPredicateLongText() => ServerResources.Get(nameof (QueryExpressionInvalidPredicateLongText));

    public static string QueryExpressionInvalidPredicateLongText(CultureInfo culture) => ServerResources.Get(nameof (QueryExpressionInvalidPredicateLongText), culture);

    public static string QueryExpressionInvalidSortFieldDupe(object arg0) => ServerResources.Format(nameof (QueryExpressionInvalidSortFieldDupe), arg0);

    public static string QueryExpressionInvalidSortFieldDupe(object arg0, CultureInfo culture) => ServerResources.Format(nameof (QueryExpressionInvalidSortFieldDupe), culture, arg0);

    public static string QueryExpressionInvalidSortFieldLinkType() => ServerResources.Get(nameof (QueryExpressionInvalidSortFieldLinkType));

    public static string QueryExpressionInvalidSortFieldLinkType(CultureInfo culture) => ServerResources.Get(nameof (QueryExpressionInvalidSortFieldLinkType), culture);

    public static string QueryExpressionInvalidSortFieldTableAlias() => ServerResources.Get(nameof (QueryExpressionInvalidSortFieldTableAlias));

    public static string QueryExpressionInvalidSortFieldTableAlias(CultureInfo culture) => ServerResources.Get(nameof (QueryExpressionInvalidSortFieldTableAlias), culture);

    public static string QueryExpressionInvalidPredicateNullValue() => ServerResources.Get(nameof (QueryExpressionInvalidPredicateNullValue));

    public static string QueryExpressionInvalidPredicateNullValue(CultureInfo culture) => ServerResources.Get(nameof (QueryExpressionInvalidPredicateNullValue), culture);

    public static string TooManyNewTagsAdded(object arg0, object arg1) => ServerResources.Format(nameof (TooManyNewTagsAdded), arg0, arg1);

    public static string TooManyNewTagsAdded(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (TooManyNewTagsAdded), culture, arg0, arg1);

    public static string QueryAccessDenied(object arg0, object arg1) => ServerResources.Format(nameof (QueryAccessDenied), arg0, arg1);

    public static string QueryAccessDenied(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (QueryAccessDenied), culture, arg0, arg1);

    public static string RulesNotFound(object arg0, object arg1) => ServerResources.Format(nameof (RulesNotFound), arg0, arg1);

    public static string RulesNotFound(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (RulesNotFound), culture, arg0, arg1);

    public static string LinkTargetWorkItemNotSaved(object arg0) => ServerResources.Format(nameof (LinkTargetWorkItemNotSaved), arg0);

    public static string LinkTargetWorkItemNotSaved(object arg0, CultureInfo culture) => ServerResources.Format(nameof (LinkTargetWorkItemNotSaved), culture, arg0);

    public static string RuleEvaluationFailed(object arg0) => ServerResources.Format(nameof (RuleEvaluationFailed), arg0);

    public static string RuleEvaluationFailed(object arg0, CultureInfo culture) => ServerResources.Format(nameof (RuleEvaluationFailed), culture, arg0);

    public static string RuleError(object arg0, object arg1) => ServerResources.Format(nameof (RuleError), arg0, arg1);

    public static string RuleError(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (RuleError), culture, arg0, arg1);

    public static string WholeBulkFailed() => ServerResources.Get(nameof (WholeBulkFailed));

    public static string WholeBulkFailed(CultureInfo culture) => ServerResources.Get(nameof (WholeBulkFailed), culture);

    public static string AdminPermissionRequired() => ServerResources.Get(nameof (AdminPermissionRequired));

    public static string AdminPermissionRequired(CultureInfo culture) => ServerResources.Get(nameof (AdminPermissionRequired), culture);

    public static string ErrorNoAreaWriteCommentAccess() => ServerResources.Get(nameof (ErrorNoAreaWriteCommentAccess));

    public static string ErrorNoAreaWriteCommentAccess(CultureInfo culture) => ServerResources.Get(nameof (ErrorNoAreaWriteCommentAccess), culture);

    public static string InvalidLinkUpdate() => ServerResources.Get(nameof (InvalidLinkUpdate));

    public static string InvalidLinkUpdate(CultureInfo culture) => ServerResources.Get(nameof (InvalidLinkUpdate), culture);

    public static string WorkItemNotFound(object arg0) => ServerResources.Format(nameof (WorkItemNotFound), arg0);

    public static string WorkItemNotFound(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemNotFound), culture, arg0);

    public static string FieldInvalidDateTime(object arg0) => ServerResources.Format(nameof (FieldInvalidDateTime), arg0);

    public static string FieldInvalidDateTime(object arg0, CultureInfo culture) => ServerResources.Format(nameof (FieldInvalidDateTime), culture, arg0);

    public static string FieldValueTooLong(object arg0) => ServerResources.Format(nameof (FieldValueTooLong), arg0);

    public static string FieldValueTooLong(object arg0, CultureInfo culture) => ServerResources.Format(nameof (FieldValueTooLong), culture, arg0);

    public static string FieldValueInvalidCharacters(object arg0) => ServerResources.Format(nameof (FieldValueInvalidCharacters), arg0);

    public static string FieldValueInvalidCharacters(object arg0, CultureInfo culture) => ServerResources.Format(nameof (FieldValueInvalidCharacters), culture, arg0);

    public static string InvalidFieldStatus(object arg0, object arg1) => ServerResources.Format(nameof (InvalidFieldStatus), arg0, arg1);

    public static string InvalidFieldStatus(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (InvalidFieldStatus), culture, arg0, arg1);

    public static string QueryTooComplex() => ServerResources.Get(nameof (QueryTooComplex));

    public static string QueryTooComplex(CultureInfo culture) => ServerResources.Get(nameof (QueryTooComplex), culture);

    public static string ResourceLinkTypeUnspecified(object arg0) => ServerResources.Format(nameof (ResourceLinkTypeUnspecified), arg0);

    public static string ResourceLinkTypeUnspecified(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ResourceLinkTypeUnspecified), culture, arg0);

    public static string WorkItemAreaPathInvalid(object arg0) => ServerResources.Format(nameof (WorkItemAreaPathInvalid), arg0);

    public static string WorkItemAreaPathInvalid(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemAreaPathInvalid), culture, arg0);

    public static string WorkItemFieldInvalidTreeId(object arg0, object arg1) => ServerResources.Format(nameof (WorkItemFieldInvalidTreeId), arg0, arg1);

    public static string WorkItemFieldInvalidTreeId(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (WorkItemFieldInvalidTreeId), culture, arg0, arg1);

    public static string WorkItemFieldInvalidTreeName(object arg0, object arg1) => ServerResources.Format(nameof (WorkItemFieldInvalidTreeName), arg0, arg1);

    public static string WorkItemFieldInvalidTreeName(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemFieldInvalidTreeName), culture, arg0, arg1);
    }

    public static string QueryExpressionInvalidColumnTypeComparison() => ServerResources.Get(nameof (QueryExpressionInvalidColumnTypeComparison));

    public static string QueryExpressionInvalidColumnTypeComparison(CultureInfo culture) => ServerResources.Get(nameof (QueryExpressionInvalidColumnTypeComparison), culture);

    public static string QueryExpressionInvalidPredicateExpandFlagWithIdentityField() => ServerResources.Get(nameof (QueryExpressionInvalidPredicateExpandFlagWithIdentityField));

    public static string QueryExpressionInvalidPredicateExpandFlagWithIdentityField(
      CultureInfo culture)
    {
      return ServerResources.Get(nameof (QueryExpressionInvalidPredicateExpandFlagWithIdentityField), culture);
    }

    public static string QueryExpressionInvalidPredicateIdentityFieldOperator(object arg0) => ServerResources.Format(nameof (QueryExpressionInvalidPredicateIdentityFieldOperator), arg0);

    public static string QueryExpressionInvalidPredicateIdentityFieldOperator(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (QueryExpressionInvalidPredicateIdentityFieldOperator), culture, arg0);
    }

    public static string QueryExpressionInvalidPredicateIdentityFieldValueType(object arg0) => ServerResources.Format(nameof (QueryExpressionInvalidPredicateIdentityFieldValueType), arg0);

    public static string QueryExpressionInvalidPredicateIdentityFieldValueType(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (QueryExpressionInvalidPredicateIdentityFieldValueType), culture, arg0);
    }

    public static string ProjectNotFound(object arg0) => ServerResources.Format(nameof (ProjectNotFound), arg0);

    public static string ProjectNotFound(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ProjectNotFound), culture, arg0);

    public static string IconNotFound(object arg0) => ServerResources.Format(nameof (IconNotFound), arg0);

    public static string IconNotFound(object arg0, CultureInfo culture) => ServerResources.Format(nameof (IconNotFound), culture, arg0);

    public static string ResourceLinkNotFound(object arg0) => ServerResources.Format(nameof (ResourceLinkNotFound), arg0);

    public static string ResourceLinkNotFound(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ResourceLinkNotFound), culture, arg0);

    public static string WorkItemTypeNotFound(object arg0, object arg1) => ServerResources.Format(nameof (WorkItemTypeNotFound), arg0, arg1);

    public static string WorkItemTypeNotFound(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (WorkItemTypeNotFound), culture, arg0, arg1);

    public static string GetConstantRecordInvalidGuidString() => ServerResources.Get(nameof (GetConstantRecordInvalidGuidString));

    public static string GetConstantRecordInvalidGuidString(CultureInfo culture) => ServerResources.Get(nameof (GetConstantRecordInvalidGuidString), culture);

    public static string WorkItemDestroyException() => ServerResources.Get(nameof (WorkItemDestroyException));

    public static string WorkItemDestroyException(CultureInfo culture) => ServerResources.Get(nameof (WorkItemDestroyException), culture);

    public static string WorkItemInvalidDestroyRequestException(object arg0) => ServerResources.Format(nameof (WorkItemInvalidDestroyRequestException), arg0);

    public static string WorkItemInvalidDestroyRequestException(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemInvalidDestroyRequestException), culture, arg0);

    public static string WorkItemTypeCategoryNotFound(object arg0, object arg1) => ServerResources.Format(nameof (WorkItemTypeCategoryNotFound), arg0, arg1);

    public static string WorkItemTypeCategoryNotFound(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemTypeCategoryNotFound), culture, arg0, arg1);
    }

    public static string ResourceLinkLengthInvalid(object arg0) => ServerResources.Format(nameof (ResourceLinkLengthInvalid), arg0);

    public static string ResourceLinkLengthInvalid(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ResourceLinkLengthInvalid), culture, arg0);

    public static string ResourceLinkTargetUnspecified(object arg0) => ServerResources.Format(nameof (ResourceLinkTargetUnspecified), arg0);

    public static string ResourceLinkTargetUnspecified(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ResourceLinkTargetUnspecified), culture, arg0);

    public static string WorkItemUnauthorizedAttachment(object arg0, object arg1) => ServerResources.Format(nameof (WorkItemUnauthorizedAttachment), arg0, arg1);

    public static string WorkItemUnauthorizedAttachment(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemUnauthorizedAttachment), culture, arg0, arg1);
    }

    public static string AsOfWorkItemQueriesNotSupported() => ServerResources.Get(nameof (AsOfWorkItemQueriesNotSupported));

    public static string AsOfWorkItemQueriesNotSupported(CultureInfo culture) => ServerResources.Get(nameof (AsOfWorkItemQueriesNotSupported), culture);

    public static string ParentQueriesNotSupported() => ServerResources.Get(nameof (ParentQueriesNotSupported));

    public static string ParentQueriesNotSupported(CultureInfo culture) => ServerResources.Get(nameof (ParentQueriesNotSupported), culture);

    public static string WorkItemUpdateBatchLimitExceeded(object arg0, object arg1) => ServerResources.Format(nameof (WorkItemUpdateBatchLimitExceeded), arg0, arg1);

    public static string WorkItemUpdateBatchLimitExceeded(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemUpdateBatchLimitExceeded), culture, arg0, arg1);
    }

    public static string WorkItemPageSizeExceeded(object arg0, object arg1) => ServerResources.Format(nameof (WorkItemPageSizeExceeded), arg0, arg1);

    public static string WorkItemPageSizeExceeded(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (WorkItemPageSizeExceeded), culture, arg0, arg1);

    public static string FieldAmbiguousIdentity(object arg0, object arg1, object arg2) => ServerResources.Format(nameof (FieldAmbiguousIdentity), arg0, arg1, arg2);

    public static string FieldAmbiguousIdentity(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (FieldAmbiguousIdentity), culture, arg0, arg1, arg2);
    }

    public static string FieldUnknownIdentity(object arg0, object arg1) => ServerResources.Format(nameof (FieldUnknownIdentity), arg0, arg1);

    public static string FieldUnknownIdentity(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (FieldUnknownIdentity), culture, arg0, arg1);

    public static string QueryTimeoutException(object arg0) => ServerResources.Format(nameof (QueryTimeoutException), arg0);

    public static string QueryTimeoutException(object arg0, CultureInfo culture) => ServerResources.Format(nameof (QueryTimeoutException), culture, arg0);

    public static string UndeletedQueryHasSiblingWithSameName() => ServerResources.Get(nameof (UndeletedQueryHasSiblingWithSameName));

    public static string UndeletedQueryHasSiblingWithSameName(CultureInfo culture) => ServerResources.Get(nameof (UndeletedQueryHasSiblingWithSameName), culture);

    public static string UndeletedQueryItemNotDeleted() => ServerResources.Get(nameof (UndeletedQueryItemNotDeleted));

    public static string UndeletedQueryItemNotDeleted(CultureInfo culture) => ServerResources.Get(nameof (UndeletedQueryItemNotDeleted), culture);

    public static string UndeletedQueryItemParentNotFound() => ServerResources.Get(nameof (UndeletedQueryItemParentNotFound));

    public static string UndeletedQueryItemParentNotFound(CultureInfo culture) => ServerResources.Get(nameof (UndeletedQueryItemParentNotFound), culture);

    public static string QueryResultSizeLimitExceeded(object arg0) => ServerResources.Format(nameof (QueryResultSizeLimitExceeded), arg0);

    public static string QueryResultSizeLimitExceeded(object arg0, CultureInfo culture) => ServerResources.Format(nameof (QueryResultSizeLimitExceeded), culture, arg0);

    public static string TrendQueryResultSizeLimitExceeded(object arg0) => ServerResources.Format(nameof (TrendQueryResultSizeLimitExceeded), arg0);

    public static string TrendQueryResultSizeLimitExceeded(object arg0, CultureInfo culture) => ServerResources.Format(nameof (TrendQueryResultSizeLimitExceeded), culture, arg0);

    public static string AdministratorAccessDenied() => ServerResources.Get(nameof (AdministratorAccessDenied));

    public static string AdministratorAccessDenied(CultureInfo culture) => ServerResources.Get(nameof (AdministratorAccessDenied), culture);

    public static string WorkItemTypeNameAlreadyExists() => ServerResources.Get(nameof (WorkItemTypeNameAlreadyExists));

    public static string WorkItemTypeNameAlreadyExists(CultureInfo culture) => ServerResources.Get(nameof (WorkItemTypeNameAlreadyExists), culture);

    public static string WorkItemTypeReferenceNameInvalidLength(object arg0) => ServerResources.Format(nameof (WorkItemTypeReferenceNameInvalidLength), arg0);

    public static string WorkItemTypeReferenceNameInvalidLength(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemTypeReferenceNameInvalidLength), culture, arg0);

    public static string UndeletedQuerysParentHasTooManyChildren() => ServerResources.Get(nameof (UndeletedQuerysParentHasTooManyChildren));

    public static string UndeletedQuerysParentHasTooManyChildren(CultureInfo culture) => ServerResources.Get(nameof (UndeletedQuerysParentHasTooManyChildren), culture);

    public static string TooManyQueryItemsRequestedException(object arg0) => ServerResources.Format(nameof (TooManyQueryItemsRequestedException), arg0);

    public static string TooManyQueryItemsRequestedException(object arg0, CultureInfo culture) => ServerResources.Format(nameof (TooManyQueryItemsRequestedException), culture, arg0);

    public static string SharedQueries() => ServerResources.Get(nameof (SharedQueries));

    public static string SharedQueries(CultureInfo culture) => ServerResources.Get(nameof (SharedQueries), culture);

    public static string MyQueries() => ServerResources.Get(nameof (MyQueries));

    public static string MyQueries(CultureInfo culture) => ServerResources.Get(nameof (MyQueries), culture);

    public static string WorkItemUpdateInvalidLinkEnds(object arg0, object arg1, object arg2) => ServerResources.Format(nameof (WorkItemUpdateInvalidLinkEnds), arg0, arg1, arg2);

    public static string WorkItemUpdateInvalidLinkEnds(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemUpdateInvalidLinkEnds), culture, arg0, arg1, arg2);
    }

    public static string InvalidListRuleError(object arg0, object arg1) => ServerResources.Format(nameof (InvalidListRuleError), arg0, arg1);

    public static string InvalidListRuleError(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (InvalidListRuleError), culture, arg0, arg1);

    public static string WorkItemQueriesOnly() => ServerResources.Get(nameof (WorkItemQueriesOnly));

    public static string WorkItemQueriesOnly(CultureInfo culture) => ServerResources.Get(nameof (WorkItemQueriesOnly), culture);

    public static string MissingContributionName(object arg0) => ServerResources.Format(nameof (MissingContributionName), arg0);

    public static string MissingContributionName(object arg0, CultureInfo culture) => ServerResources.Format(nameof (MissingContributionName), culture, arg0);

    public static string ClassificationNodeCannotAddDateToNonIteration(object arg0) => ServerResources.Format(nameof (ClassificationNodeCannotAddDateToNonIteration), arg0);

    public static string ClassificationNodeCannotAddDateToNonIteration(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ClassificationNodeCannotAddDateToNonIteration), culture, arg0);
    }

    public static string ClassificationNodeCannotCreateRootNodeAfterProjectCreation(object arg0) => ServerResources.Format(nameof (ClassificationNodeCannotCreateRootNodeAfterProjectCreation), arg0);

    public static string ClassificationNodeCannotCreateRootNodeAfterProjectCreation(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ClassificationNodeCannotCreateRootNodeAfterProjectCreation), culture, arg0);
    }

    public static string ClassificationNodeCannotChangeStructureType(object arg0, object arg1) => ServerResources.Format(nameof (ClassificationNodeCannotChangeStructureType), arg0, arg1);

    public static string ClassificationNodeCannotChangeStructureType(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ClassificationNodeCannotChangeStructureType), culture, arg0, arg1);
    }

    public static string ClassificationNodeCannotDeleteReclassifyNode(object arg0) => ServerResources.Format(nameof (ClassificationNodeCannotDeleteReclassifyNode), arg0);

    public static string ClassificationNodeCannotDeleteReclassifyNode(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ClassificationNodeCannotDeleteReclassifyNode), culture, arg0);
    }

    public static string ClassificationNodeCannotModifyRootNode(object arg0) => ServerResources.Format(nameof (ClassificationNodeCannotModifyRootNode), arg0);

    public static string ClassificationNodeCannotModifyRootNode(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ClassificationNodeCannotModifyRootNode), culture, arg0);

    public static string ClassificationNodeCircularNodeReference(object arg0) => ServerResources.Format(nameof (ClassificationNodeCircularNodeReference), arg0);

    public static string ClassificationNodeCircularNodeReference(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ClassificationNodeCircularNodeReference), culture, arg0);

    public static string ClassificationNodeDuplicateDeletedNode(object arg0) => ServerResources.Format(nameof (ClassificationNodeDuplicateDeletedNode), arg0);

    public static string ClassificationNodeDuplicateDeletedNode(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ClassificationNodeDuplicateDeletedNode), culture, arg0);

    public static string ClassificationNodeDuplicateIdentifier(object arg0) => ServerResources.Format(nameof (ClassificationNodeDuplicateIdentifier), arg0);

    public static string ClassificationNodeDuplicateIdentifier(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ClassificationNodeDuplicateIdentifier), culture, arg0);

    public static string ClassificationNodeDuplicateName(object arg0, object arg1) => ServerResources.Format(nameof (ClassificationNodeDuplicateName), arg0, arg1);

    public static string ClassificationNodeDuplicateName(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ClassificationNodeDuplicateName), culture, arg0, arg1);
    }

    public static string ClassificationNodeEmptyIdentifier() => ServerResources.Get(nameof (ClassificationNodeEmptyIdentifier));

    public static string ClassificationNodeEmptyIdentifier(CultureInfo culture) => ServerResources.Get(nameof (ClassificationNodeEmptyIdentifier), culture);

    public static string ClassificationNodeInvalidStructureType(object arg0) => ServerResources.Format(nameof (ClassificationNodeInvalidStructureType), arg0);

    public static string ClassificationNodeInvalidStructureType(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ClassificationNodeInvalidStructureType), culture, arg0);

    public static string ClassificationNodeMaximumDepthExceeded(object arg0) => ServerResources.Format(nameof (ClassificationNodeMaximumDepthExceeded), arg0);

    public static string ClassificationNodeMaximumDepthExceeded(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ClassificationNodeMaximumDepthExceeded), culture, arg0);

    public static string ClassificationNodeParentNodeDoesNotExist(object arg0) => ServerResources.Format(nameof (ClassificationNodeParentNodeDoesNotExist), arg0);

    public static string ClassificationNodeParentNodeDoesNotExist(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ClassificationNodeParentNodeDoesNotExist), culture, arg0);

    public static string ClassificationNodeReclassifiedToDifferentStructureType(
      object arg0,
      object arg1)
    {
      return ServerResources.Format(nameof (ClassificationNodeReclassifiedToDifferentStructureType), arg0, arg1);
    }

    public static string ClassificationNodeReclassifiedToDifferentStructureType(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ClassificationNodeReclassifiedToDifferentStructureType), culture, arg0, arg1);
    }

    public static string ClassificationNodeReclassifiedToRootNode(object arg0) => ServerResources.Format(nameof (ClassificationNodeReclassifiedToRootNode), arg0);

    public static string ClassificationNodeReclassifiedToRootNode(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ClassificationNodeReclassifiedToRootNode), culture, arg0);

    public static string ClassificationNodeReclassifiedToSubTree(object arg0, object arg1) => ServerResources.Format(nameof (ClassificationNodeReclassifiedToSubTree), arg0, arg1);

    public static string ClassificationNodeReclassifiedToSubTree(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ClassificationNodeReclassifiedToSubTree), culture, arg0, arg1);
    }

    public static string ClassificationNodeReclassificationNodeDoesNotExist(object arg0) => ServerResources.Format(nameof (ClassificationNodeReclassificationNodeDoesNotExist), arg0);

    public static string ClassificationNodeReclassificationNodeDoesNotExist(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ClassificationNodeReclassificationNodeDoesNotExist), culture, arg0);
    }

    public static string ClassificationNodeTooManyRootNode() => ServerResources.Get(nameof (ClassificationNodeTooManyRootNode));

    public static string ClassificationNodeTooManyRootNode(CultureInfo culture) => ServerResources.Get(nameof (ClassificationNodeTooManyRootNode), culture);

    public static string ClassificationNodeTooManyAreaPaths(object arg0) => ServerResources.Format(nameof (ClassificationNodeTooManyAreaPaths), arg0);

    public static string ClassificationNodeTooManyAreaPaths(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ClassificationNodeTooManyAreaPaths), culture, arg0);

    public static string ClassificationNodeTooManyIterationPaths(object arg0) => ServerResources.Format(nameof (ClassificationNodeTooManyIterationPaths), arg0);

    public static string ClassificationNodeTooManyIterationPaths(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ClassificationNodeTooManyIterationPaths), culture, arg0);

    public static string ClassificationNodeUnexpectedSqlError(object arg0) => ServerResources.Format(nameof (ClassificationNodeUnexpectedSqlError), arg0);

    public static string ClassificationNodeUnexpectedSqlError(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ClassificationNodeUnexpectedSqlError), culture, arg0);

    public static string QueryInGroupTooLarge(object arg0, object arg1) => ServerResources.Format(nameof (QueryInGroupTooLarge), arg0, arg1);

    public static string QueryInGroupTooLarge(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (QueryInGroupTooLarge), culture, arg0, arg1);

    public static string AadIdentityResolutionError() => ServerResources.Get(nameof (AadIdentityResolutionError));

    public static string AadIdentityResolutionError(CultureInfo culture) => ServerResources.Get(nameof (AadIdentityResolutionError), culture);

    public static string InvalidPath() => ServerResources.Get(nameof (InvalidPath));

    public static string InvalidPath(CultureInfo culture) => ServerResources.Get(nameof (InvalidPath), culture);

    public static string FieldUnknownPath(object arg0, object arg1) => ServerResources.Format(nameof (FieldUnknownPath), arg0, arg1);

    public static string FieldUnknownPath(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (FieldUnknownPath), culture, arg0, arg1);

    public static string QueryInvalidBooleanValueException() => ServerResources.Get(nameof (QueryInvalidBooleanValueException));

    public static string QueryInvalidBooleanValueException(CultureInfo culture) => ServerResources.Get(nameof (QueryInvalidBooleanValueException), culture);

    public static string NotInScopeAadIdentities(object arg0) => ServerResources.Format(nameof (NotInScopeAadIdentities), arg0);

    public static string NotInScopeAadIdentities(object arg0, CultureInfo culture) => ServerResources.Format(nameof (NotInScopeAadIdentities), culture, arg0);

    public static string FieldInvalidIntegerValue(object arg0) => ServerResources.Format(nameof (FieldInvalidIntegerValue), arg0);

    public static string FieldInvalidIntegerValue(object arg0, CultureInfo culture) => ServerResources.Format(nameof (FieldInvalidIntegerValue), culture, arg0);

    public static string UnknownProjectName() => ServerResources.Get(nameof (UnknownProjectName));

    public static string UnknownProjectName(CultureInfo culture) => ServerResources.Get(nameof (UnknownProjectName), culture);

    public static string QueryItemCacheParentNotFoundException(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServerResources.Format(nameof (QueryItemCacheParentNotFoundException), arg0, arg1, arg2);
    }

    public static string QueryItemCacheParentNotFoundException(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (QueryItemCacheParentNotFoundException), culture, arg0, arg1, arg2);
    }

    public static string QueryTooManyConcurrentUsers() => ServerResources.Get(nameof (QueryTooManyConcurrentUsers));

    public static string QueryTooManyConcurrentUsers(CultureInfo culture) => ServerResources.Get(nameof (QueryTooManyConcurrentUsers), culture);

    public static string QueryServerBusy() => ServerResources.Get(nameof (QueryServerBusy));

    public static string QueryServerBusy(CultureInfo culture) => ServerResources.Get(nameof (QueryServerBusy), culture);

    public static string ValidatorBannedRuleUsed(object arg0) => ServerResources.Format(nameof (ValidatorBannedRuleUsed), arg0);

    public static string ValidatorBannedRuleUsed(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ValidatorBannedRuleUsed), culture, arg0);

    public static string ValidatorBlockedRulesOnFieldThatLimitsRules(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorBlockedRulesOnFieldThatLimitsRules), arg0, arg1);

    public static string ValidatorBlockedRulesOnFieldThatLimitsRules(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorBlockedRulesOnFieldThatLimitsRules), culture, arg0, arg1);
    }

    public static string ValidatorCannotDefineGlobalLists(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorCannotDefineGlobalLists), arg0, arg1);

    public static string ValidatorCannotDefineGlobalLists(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorCannotDefineGlobalLists), culture, arg0, arg1);
    }

    public static string ValidatorCannotReferenceAGlobalList(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorCannotReferenceAGlobalList), arg0, arg1);

    public static string ValidatorCannotReferenceAGlobalList(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorCannotReferenceAGlobalList), culture, arg0, arg1);
    }

    public static string ValidatorCustomFieldInProtectedNamespace(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorCustomFieldInProtectedNamespace), arg0, arg1);

    public static string ValidatorCustomFieldInProtectedNamespace(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorCustomFieldInProtectedNamespace), culture, arg0, arg1);
    }

    public static string ValidatorFeatureInvalidMetastateMapping(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorFeatureInvalidMetastateMapping), arg0, arg1);

    public static string ValidatorFeatureInvalidMetastateMapping(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorFeatureInvalidMetastateMapping), culture, arg0, arg1);
    }

    public static string ValidatorFeatureMissingAllowedValuesOnField(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServerResources.Format(nameof (ValidatorFeatureMissingAllowedValuesOnField), arg0, arg1, arg2);
    }

    public static string ValidatorFeatureMissingAllowedValuesOnField(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorFeatureMissingAllowedValuesOnField), culture, arg0, arg1, arg2);
    }

    public static string ValidatorFeatureMissingField(object arg0, object arg1, object arg2) => ServerResources.Format(nameof (ValidatorFeatureMissingField), arg0, arg1, arg2);

    public static string ValidatorFeatureMissingField(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorFeatureMissingField), culture, arg0, arg1, arg2);
    }

    public static string ValidatorFeatureMissingHardcodedCategory(object arg0) => ServerResources.Format(nameof (ValidatorFeatureMissingHardcodedCategory), arg0);

    public static string ValidatorFeatureMissingHardcodedCategory(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ValidatorFeatureMissingHardcodedCategory), culture, arg0);

    public static string ValidatorFeatureMissingMappedStateInType(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServerResources.Format(nameof (ValidatorFeatureMissingMappedStateInType), arg0, arg1, arg2);
    }

    public static string ValidatorFeatureMissingMappedStateInType(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorFeatureMissingMappedStateInType), culture, arg0, arg1, arg2);
    }

    public static string ValidatorFeatureMissingMappingInConfig(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorFeatureMissingMappingInConfig), arg0, arg1);

    public static string ValidatorFeatureMissingMappingInConfig(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorFeatureMissingMappingInConfig), culture, arg0, arg1);
    }

    public static string ValidatorFeatureMissingTypeField(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ServerResources.Format(nameof (ValidatorFeatureMissingTypeField), arg0, arg1, arg2, arg3);
    }

    public static string ValidatorFeatureMissingTypeField(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorFeatureMissingTypeField), culture, arg0, arg1, arg2, arg3);
    }

    public static string ValidatorFeatureStateMappedToMultipleMetastates(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorFeatureStateMappedToMultipleMetastates), arg0, arg1);

    public static string ValidatorFeatureStateMappedToMultipleMetastates(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorFeatureStateMappedToMultipleMetastates), culture, arg0, arg1);
    }

    public static string ValidatorFeatureStateMissingInType(object arg0, object arg1, object arg2) => ServerResources.Format(nameof (ValidatorFeatureStateMissingInType), arg0, arg1, arg2);

    public static string ValidatorFeatureStateMissingInType(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorFeatureStateMissingInType), culture, arg0, arg1, arg2);
    }

    public static string ValidatorFeatureUniqueMappedStateInType(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ServerResources.Format(nameof (ValidatorFeatureUniqueMappedStateInType), arg0, arg1, arg2, arg3);
    }

    public static string ValidatorFeatureUniqueMappedStateInType(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorFeatureUniqueMappedStateInType), culture, arg0, arg1, arg2, arg3);
    }

    public static string ValidatorFeatureUnknownCategoryForFeature(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorFeatureUnknownCategoryForFeature), arg0, arg1);

    public static string ValidatorFeatureUnknownCategoryForFeature(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorFeatureUnknownCategoryForFeature), culture, arg0, arg1);
    }

    public static string ValidatorFeatureUnknownMetastate(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorFeatureUnknownMetastate), arg0, arg1);

    public static string ValidatorFeatureUnknownMetastate(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorFeatureUnknownMetastate), culture, arg0, arg1);
    }

    public static string ValidatorFieldDelete(object arg0) => ServerResources.Format(nameof (ValidatorFieldDelete), arg0);

    public static string ValidatorFieldDelete(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ValidatorFieldDelete), culture, arg0);

    public static string ValidatorImproperDefinitionOfProtectedField(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4)
    {
      return ServerResources.Format(nameof (ValidatorImproperDefinitionOfProtectedField), arg0, arg1, arg2, arg3, arg4);
    }

    public static string ValidatorImproperDefinitionOfProtectedField(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorImproperDefinitionOfProtectedField), culture, arg0, arg1, arg2, arg3, arg4);
    }

    public static string ValidatorInconsistentFieldDefinitionInTemplate(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorInconsistentFieldDefinitionInTemplate), arg0, arg1);

    public static string ValidatorInconsistentFieldDefinitionInTemplate(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorInconsistentFieldDefinitionInTemplate), culture, arg0, arg1);
    }

    public static string ValidatorInconsistentFieldDefinitionWithExistingTemplate(
      object arg0,
      object arg1)
    {
      return ServerResources.Format(nameof (ValidatorInconsistentFieldDefinitionWithExistingTemplate), arg0, arg1);
    }

    public static string ValidatorInconsistentFieldDefinitionWithExistingTemplate(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorInconsistentFieldDefinitionWithExistingTemplate), culture, arg0, arg1);
    }

    public static string ValidatorInconsistentFieldDefinitionWithFieldTable(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServerResources.Format(nameof (ValidatorInconsistentFieldDefinitionWithFieldTable), arg0, arg1, arg2);
    }

    public static string ValidatorInconsistentFieldDefinitionWithFieldTable(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorInconsistentFieldDefinitionWithFieldTable), culture, arg0, arg1, arg2);
    }

    public static string ValidatorLimitsTooManyCategories(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorLimitsTooManyCategories), arg0, arg1);

    public static string ValidatorLimitsTooManyCategories(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorLimitsTooManyCategories), culture, arg0, arg1);
    }

    public static string ValidatorLimitsTooManyCustomLinkTypes(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorLimitsTooManyCustomLinkTypes), arg0, arg1);

    public static string ValidatorLimitsTooManyCustomLinkTypes(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorLimitsTooManyCustomLinkTypes), culture, arg0, arg1);
    }

    public static string ValidatorLimitsTooManyFieldsInWIT(object arg0, object arg1, object arg2) => ServerResources.Format(nameof (ValidatorLimitsTooManyFieldsInWIT), arg0, arg1, arg2);

    public static string ValidatorLimitsTooManyFieldsInWIT(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorLimitsTooManyFieldsInWIT), culture, arg0, arg1, arg2);
    }

    public static string ValidatorLimitsTooManyFieldsTotal(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorLimitsTooManyFieldsTotal), arg0, arg1);

    public static string ValidatorLimitsTooManyFieldsTotal(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorLimitsTooManyFieldsTotal), culture, arg0, arg1);
    }

    public static string ValidatorLimitsTooManyGlobalLists(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorLimitsTooManyGlobalLists), arg0, arg1);

    public static string ValidatorLimitsTooManyGlobalLists(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorLimitsTooManyGlobalLists), culture, arg0, arg1);
    }

    public static string ValidatorLimitsTooManyItemsInAGlobalList(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServerResources.Format(nameof (ValidatorLimitsTooManyItemsInAGlobalList), arg0, arg1, arg2);
    }

    public static string ValidatorLimitsTooManyItemsInAGlobalList(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorLimitsTooManyItemsInAGlobalList), culture, arg0, arg1, arg2);
    }

    public static string ValidatorLimitsTooManyRulesOnWIT(object arg0, object arg1, object arg2) => ServerResources.Format(nameof (ValidatorLimitsTooManyRulesOnWIT), arg0, arg1, arg2);

    public static string ValidatorLimitsTooManyRulesOnWIT(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorLimitsTooManyRulesOnWIT), culture, arg0, arg1, arg2);
    }

    public static string ValidatorLimitsTooManyStatesInWIT(object arg0, object arg1, object arg2) => ServerResources.Format(nameof (ValidatorLimitsTooManyStatesInWIT), arg0, arg1, arg2);

    public static string ValidatorLimitsTooManyStatesInWIT(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorLimitsTooManyStatesInWIT), culture, arg0, arg1, arg2);
    }

    public static string ValidatorLimitsTooManySyncNameChangesFieldsInType(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServerResources.Format(nameof (ValidatorLimitsTooManySyncNameChangesFieldsInType), arg0, arg1, arg2);
    }

    public static string ValidatorLimitsTooManySyncNameChangesFieldsInType(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorLimitsTooManySyncNameChangesFieldsInType), culture, arg0, arg1, arg2);
    }

    public static string ValidatorLimitsTooManyValuesInValueList(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ServerResources.Format(nameof (ValidatorLimitsTooManyValuesInValueList), arg0, arg1, arg2, arg3);
    }

    public static string ValidatorLimitsTooManyValuesInValueList(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorLimitsTooManyValuesInValueList), culture, arg0, arg1, arg2, arg3);
    }

    public static string ValidatorLimitsTooManyWITs(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorLimitsTooManyWITs), arg0, arg1);

    public static string ValidatorLimitsTooManyWITs(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (ValidatorLimitsTooManyWITs), culture, arg0, arg1);

    public static string ValidatorMissingFeatureElementInProcessConfiguration(object arg0) => ServerResources.Format(nameof (ValidatorMissingFeatureElementInProcessConfiguration), arg0);

    public static string ValidatorMissingFeatureElementInProcessConfiguration(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorMissingFeatureElementInProcessConfiguration), culture, arg0);
    }

    public static string ValidatorMissingFileInPackage() => ServerResources.Get(nameof (ValidatorMissingFileInPackage));

    public static string ValidatorMissingFileInPackage(CultureInfo culture) => ServerResources.Get(nameof (ValidatorMissingFileInPackage), culture);

    public static string ValidatorMissingRefNameOnWorkItemType(object arg0) => ServerResources.Format(nameof (ValidatorMissingRefNameOnWorkItemType), arg0);

    public static string ValidatorMissingRefNameOnWorkItemType(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ValidatorMissingRefNameOnWorkItemType), culture, arg0);

    public static string ValidatorMissingTypeFieldDefinition(object arg0) => ServerResources.Format(nameof (ValidatorMissingTypeFieldDefinition), arg0);

    public static string ValidatorMissingTypeFieldDefinition(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ValidatorMissingTypeFieldDefinition), culture, arg0);

    public static string ValidatorMultipleCategoriesDocuments() => ServerResources.Get(nameof (ValidatorMultipleCategoriesDocuments));

    public static string ValidatorMultipleCategoriesDocuments(CultureInfo culture) => ServerResources.Get(nameof (ValidatorMultipleCategoriesDocuments), culture);

    public static string ValidatorMultipleProcessConfigurationDocuments() => ServerResources.Get(nameof (ValidatorMultipleProcessConfigurationDocuments));

    public static string ValidatorMultipleProcessConfigurationDocuments(CultureInfo culture) => ServerResources.Get(nameof (ValidatorMultipleProcessConfigurationDocuments), culture);

    public static string ValidatorMultipleUsesOfDisplayNameAcrossTemplates(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ServerResources.Format(nameof (ValidatorMultipleUsesOfDisplayNameAcrossTemplates), arg0, arg1, arg2, arg3);
    }

    public static string ValidatorMultipleUsesOfDisplayNameAcrossTemplates(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorMultipleUsesOfDisplayNameAcrossTemplates), culture, arg0, arg1, arg2, arg3);
    }

    public static string ValidatorMultipleUsesOfDisplayNameInFieldTable(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServerResources.Format(nameof (ValidatorMultipleUsesOfDisplayNameInFieldTable), arg0, arg1, arg2);
    }

    public static string ValidatorMultipleUsesOfDisplayNameInFieldTable(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorMultipleUsesOfDisplayNameInFieldTable), culture, arg0, arg1, arg2);
    }

    public static string ValidatorMultipleUsesOfDisplayNameInTemplate(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorMultipleUsesOfDisplayNameInTemplate), arg0, arg1);

    public static string ValidatorMultipleUsesOfDisplayNameInTemplate(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorMultipleUsesOfDisplayNameInTemplate), culture, arg0, arg1);
    }

    public static string ValidatorMultipleUsesOfWorkItemTypeName(object arg0) => ServerResources.Format(nameof (ValidatorMultipleUsesOfWorkItemTypeName), arg0);

    public static string ValidatorMultipleUsesOfWorkItemTypeName(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ValidatorMultipleUsesOfWorkItemTypeName), culture, arg0);

    public static string ValidatorMultipleUsesOfWorkItemTypeRefName(object arg0) => ServerResources.Format(nameof (ValidatorMultipleUsesOfWorkItemTypeRefName), arg0);

    public static string ValidatorMultipleUsesOfWorkItemTypeRefName(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorMultipleUsesOfWorkItemTypeRefName), culture, arg0);
    }

    public static string ValidatorNoCustomFormControls(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorNoCustomFormControls), arg0, arg1);

    public static string ValidatorNoCustomFormControls(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorNoCustomFormControls), culture, arg0, arg1);
    }

    public static string ValidatorNoCustomLinkTypes(object arg0) => ServerResources.Format(nameof (ValidatorNoCustomLinkTypes), arg0);

    public static string ValidatorNoCustomLinkTypes(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ValidatorNoCustomLinkTypes), culture, arg0);

    public static string ValidatorNoUsageOfForOrNotAttributes() => ServerResources.Get(nameof (ValidatorNoUsageOfForOrNotAttributes));

    public static string ValidatorNoUsageOfForOrNotAttributes(CultureInfo culture) => ServerResources.Get(nameof (ValidatorNoUsageOfForOrNotAttributes), culture);

    public static string ValidatorNoWorkItemTrackingGroupInProcessTemplate() => ServerResources.Get(nameof (ValidatorNoWorkItemTrackingGroupInProcessTemplate));

    public static string ValidatorNoWorkItemTrackingGroupInProcessTemplate(CultureInfo culture) => ServerResources.Get(nameof (ValidatorNoWorkItemTrackingGroupInProcessTemplate), culture);

    public static string ValidatorParsingXmlFails(object arg0) => ServerResources.Format(nameof (ValidatorParsingXmlFails), arg0);

    public static string ValidatorParsingXmlFails(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ValidatorParsingXmlFails), culture, arg0);

    public static string ValidatorPathToProcessConfigurationNotFound() => ServerResources.Get(nameof (ValidatorPathToProcessConfigurationNotFound));

    public static string ValidatorPathToProcessConfigurationNotFound(CultureInfo culture) => ServerResources.Get(nameof (ValidatorPathToProcessConfigurationNotFound), culture);

    public static string ValidatorPortfilioBacklogsMultipleChildren(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorPortfilioBacklogsMultipleChildren), arg0, arg1);

    public static string ValidatorPortfilioBacklogsMultipleChildren(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorPortfilioBacklogsMultipleChildren), culture, arg0, arg1);
    }

    public static string ValidatorPortfolioBacklogMissingParent(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorPortfolioBacklogMissingParent), arg0, arg1);

    public static string ValidatorPortfolioBacklogMissingParent(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorPortfolioBacklogMissingParent), culture, arg0, arg1);
    }

    public static string ValidatorPortfolioBacklogMultipleRoots(object arg0) => ServerResources.Format(nameof (ValidatorPortfolioBacklogMultipleRoots), arg0);

    public static string ValidatorPortfolioBacklogMultipleRoots(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ValidatorPortfolioBacklogMultipleRoots), culture, arg0);

    public static string ValidatorRenameOfFieldInExistingTemplates(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ServerResources.Format(nameof (ValidatorRenameOfFieldInExistingTemplates), arg0, arg1, arg2, arg3);
    }

    public static string ValidatorRenameOfFieldInExistingTemplates(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorRenameOfFieldInExistingTemplates), culture, arg0, arg1, arg2, arg3);
    }

    public static string ValidatorRenameOfFieldInExistingTemplatesError(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ServerResources.Format(nameof (ValidatorRenameOfFieldInExistingTemplatesError), arg0, arg1, arg2, arg3);
    }

    public static string ValidatorRenameOfFieldInExistingTemplatesError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorRenameOfFieldInExistingTemplatesError), culture, arg0, arg1, arg2, arg3);
    }

    public static string ValidatorRulesOnFieldThatProhibitsRules(object arg0) => ServerResources.Format(nameof (ValidatorRulesOnFieldThatProhibitsRules), arg0);

    public static string ValidatorRulesOnFieldThatProhibitsRules(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ValidatorRulesOnFieldThatProhibitsRules), culture, arg0);

    public static string ValidatorSchemaValidationFailure(object arg0) => ServerResources.Format(nameof (ValidatorSchemaValidationFailure), arg0);

    public static string ValidatorSchemaValidationFailure(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ValidatorSchemaValidationFailure), culture, arg0);

    public static string ValidatorTooManyPortfolioBacklogs() => ServerResources.Get(nameof (ValidatorTooManyPortfolioBacklogs));

    public static string ValidatorTooManyPortfolioBacklogs(CultureInfo culture) => ServerResources.Get(nameof (ValidatorTooManyPortfolioBacklogs), culture);

    public static string ValidatorUnknownWorkItemTypeInCategory(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorUnknownWorkItemTypeInCategory), arg0, arg1);

    public static string ValidatorUnknownWorkItemTypeInCategory(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorUnknownWorkItemTypeInCategory), culture, arg0, arg1);
    }

    public static string ValidatorUnusedCustomCategories(object arg0) => ServerResources.Format(nameof (ValidatorUnusedCustomCategories), arg0);

    public static string ValidatorUnusedCustomCategories(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ValidatorUnusedCustomCategories), culture, arg0);

    public static string ValidatorWorkItemTypeDelete(object arg0) => ServerResources.Format(nameof (ValidatorWorkItemTypeDelete), arg0);

    public static string ValidatorWorkItemTypeDelete(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ValidatorWorkItemTypeDelete), culture, arg0);

    public static string ValidatorWorkItemTypeRefNameInProtectedNamespace(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorWorkItemTypeRefNameInProtectedNamespace), arg0, arg1);

    public static string ValidatorWorkItemTypeRefNameInProtectedNamespace(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorWorkItemTypeRefNameInProtectedNamespace), culture, arg0, arg1);
    }

    public static string ValidatorWorkItemTypeDeleteWhileExistingWorkItems(object arg0) => ServerResources.Format(nameof (ValidatorWorkItemTypeDeleteWhileExistingWorkItems), arg0);

    public static string ValidatorWorkItemTypeDeleteWhileExistingWorkItems(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorWorkItemTypeDeleteWhileExistingWorkItems), culture, arg0);
    }

    public static string ValidatorWorkItemTypeRenameWhileExistingWorkItems(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorWorkItemTypeRenameWhileExistingWorkItems), arg0, arg1);

    public static string ValidatorWorkItemTypeRenameWhileExistingWorkItems(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorWorkItemTypeRenameWhileExistingWorkItems), culture, arg0, arg1);
    }

    public static string ValidatorWorkItemTypeRefNameInvalid(object arg0) => ServerResources.Format(nameof (ValidatorWorkItemTypeRefNameInvalid), arg0);

    public static string ValidatorWorkItemTypeRefNameInvalid(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ValidatorWorkItemTypeRefNameInvalid), culture, arg0);

    public static string ValidatorWorkItemTypeRename(object arg0, object arg1, object arg2) => ServerResources.Format(nameof (ValidatorWorkItemTypeRename), arg0, arg1, arg2);

    public static string ValidatorWorkItemTypeRename(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorWorkItemTypeRename), culture, arg0, arg1, arg2);
    }

    public static string ValidatorSystemPickListRedefined(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorSystemPickListRedefined), arg0, arg1);

    public static string ValidatorSystemPickListRedefined(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorSystemPickListRedefined), culture, arg0, arg1);
    }

    public static string ValidatorLinkTypeAttributeNotExist(object arg0) => ServerResources.Format(nameof (ValidatorLinkTypeAttributeNotExist), arg0);

    public static string ValidatorLinkTypeAttributeNotExist(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ValidatorLinkTypeAttributeNotExist), culture, arg0);

    public static string ValidatorLinkTypeDefinitionNotUnique(object arg0) => ServerResources.Format(nameof (ValidatorLinkTypeDefinitionNotUnique), arg0);

    public static string ValidatorLinkTypeDefinitionNotUnique(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ValidatorLinkTypeDefinitionNotUnique), culture, arg0);

    public static string ValidatorCustomLinkTypeInProtectedNamespace(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorCustomLinkTypeInProtectedNamespace), arg0, arg1);

    public static string ValidatorCustomLinkTypeInProtectedNamespace(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorCustomLinkTypeInProtectedNamespace), culture, arg0, arg1);
    }

    public static string ValidatorImproperDefinitionOfProtectedLinkType(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ServerResources.Format(nameof (ValidatorImproperDefinitionOfProtectedLinkType), arg0, arg1, arg2, arg3);
    }

    public static string ValidatorImproperDefinitionOfProtectedLinkType(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorImproperDefinitionOfProtectedLinkType), culture, arg0, arg1, arg2, arg3);
    }

    public static string ValidatorMultipleUsesOfLinkTypeDisplayName(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorMultipleUsesOfLinkTypeDisplayName), arg0, arg1);

    public static string ValidatorMultipleUsesOfLinkTypeDisplayName(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorMultipleUsesOfLinkTypeDisplayName), culture, arg0, arg1);
    }

    public static string WorkItemDateInFutureException() => ServerResources.Get(nameof (WorkItemDateInFutureException));

    public static string WorkItemDateInFutureException(CultureInfo culture) => ServerResources.Get(nameof (WorkItemDateInFutureException), culture);

    public static string WorkItemDatesNotIncreasingException() => ServerResources.Get(nameof (WorkItemDatesNotIncreasingException));

    public static string WorkItemDatesNotIncreasingException(CultureInfo culture) => ServerResources.Get(nameof (WorkItemDatesNotIncreasingException), culture);

    public static string MustSpecifyMatchingValue(object arg0, object arg1) => ServerResources.Format(nameof (MustSpecifyMatchingValue), arg0, arg1);

    public static string MustSpecifyMatchingValue(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (MustSpecifyMatchingValue), culture, arg0, arg1);

    public static string InvalidTreeNodeIdentifier() => ServerResources.Get(nameof (InvalidTreeNodeIdentifier));

    public static string InvalidTreeNodeIdentifier(CultureInfo culture) => ServerResources.Get(nameof (InvalidTreeNodeIdentifier), culture);

    public static string WorkItemFormOtherGroups() => ServerResources.Get(nameof (WorkItemFormOtherGroups));

    public static string WorkItemFormOtherGroups(CultureInfo culture) => ServerResources.Get(nameof (WorkItemFormOtherGroups), culture);

    public static string WorkItemFormLinksAndAttachments() => ServerResources.Get(nameof (WorkItemFormLinksAndAttachments));

    public static string WorkItemFormLinksAndAttachments(CultureInfo culture) => ServerResources.Get(nameof (WorkItemFormLinksAndAttachments), culture);

    public static string AllSpecifiedValuesMustMatch(object arg0, object arg1) => ServerResources.Format(nameof (AllSpecifiedValuesMustMatch), arg0, arg1);

    public static string AllSpecifiedValuesMustMatch(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (AllSpecifiedValuesMustMatch), culture, arg0, arg1);

    public static string FieldAlreadyExists(object arg0) => ServerResources.Format(nameof (FieldAlreadyExists), arg0);

    public static string FieldAlreadyExists(object arg0, CultureInfo culture) => ServerResources.Format(nameof (FieldAlreadyExists), culture, arg0);

    public static string InvalidResourceLinkType(object arg0) => ServerResources.Format(nameof (InvalidResourceLinkType), arg0);

    public static string InvalidResourceLinkType(object arg0, CultureInfo culture) => ServerResources.Format(nameof (InvalidResourceLinkType), culture, arg0);

    public static string InvalidResourceLinkTarget(object arg0) => ServerResources.Format(nameof (InvalidResourceLinkTarget), arg0);

    public static string InvalidResourceLinkTarget(object arg0, CultureInfo culture) => ServerResources.Format(nameof (InvalidResourceLinkTarget), culture, arg0);

    public static string InvalidResourceLinkTarget_ToolPart(object arg0) => ServerResources.Format(nameof (InvalidResourceLinkTarget_ToolPart), arg0);

    public static string InvalidResourceLinkTarget_ToolPart(object arg0, CultureInfo culture) => ServerResources.Format(nameof (InvalidResourceLinkTarget_ToolPart), culture, arg0);

    public static string FieldTypeInvalid() => ServerResources.Get(nameof (FieldTypeInvalid));

    public static string FieldTypeInvalid(CultureInfo culture) => ServerResources.Get(nameof (FieldTypeInvalid), culture);

    public static string ProcessCannotBeCustomized() => ServerResources.Get(nameof (ProcessCannotBeCustomized));

    public static string ProcessCannotBeCustomized(CultureInfo culture) => ServerResources.Get(nameof (ProcessCannotBeCustomized), culture);

    public static string WorkItemTrackingDescriptionInvalid(object arg0, object arg1) => ServerResources.Format(nameof (WorkItemTrackingDescriptionInvalid), arg0, arg1);

    public static string WorkItemTrackingDescriptionInvalid(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemTrackingDescriptionInvalid), culture, arg0, arg1);
    }

    public static string WorkItemTrackingNameInvalid(object arg0, object arg1, object arg2) => ServerResources.Format(nameof (WorkItemTrackingNameInvalid), arg0, arg1, arg2);

    public static string WorkItemTrackingNameInvalid(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemTrackingNameInvalid), culture, arg0, arg1, arg2);
    }

    public static string WorkItemTypeDoesNotExist(object arg0, object arg1) => ServerResources.Format(nameof (WorkItemTypeDoesNotExist), arg0, arg1);

    public static string WorkItemTypeDoesNotExist(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (WorkItemTypeDoesNotExist), culture, arg0, arg1);

    public static string WorkItemTypeFieldDoesNotExist(object arg0, object arg1) => ServerResources.Format(nameof (WorkItemTypeFieldDoesNotExist), arg0, arg1);

    public static string WorkItemTypeFieldDoesNotExist(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemTypeFieldDoesNotExist), culture, arg0, arg1);
    }

    public static string FieldLimitExceeded(object arg0) => ServerResources.Format(nameof (FieldLimitExceeded), arg0);

    public static string FieldLimitExceeded(object arg0, CultureInfo culture) => ServerResources.Format(nameof (FieldLimitExceeded), culture, arg0);

    public static string WorkItemTypeFieldLimitExceeded(object arg0) => ServerResources.Format(nameof (WorkItemTypeFieldLimitExceeded), arg0);

    public static string WorkItemTypeFieldLimitExceeded(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemTypeFieldLimitExceeded), culture, arg0);

    public static string WorkItemRuleInvalidValue(object arg0, object arg1) => ServerResources.Format(nameof (WorkItemRuleInvalidValue), arg0, arg1);

    public static string WorkItemRuleInvalidValue(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (WorkItemRuleInvalidValue), culture, arg0, arg1);

    public static string ParentWorkItemTypeDoesNotExist(object arg0, object arg1) => ServerResources.Format(nameof (ParentWorkItemTypeDoesNotExist), arg0, arg1);

    public static string ParentWorkItemTypeDoesNotExist(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ParentWorkItemTypeDoesNotExist), culture, arg0, arg1);
    }

    public static string FieldNameorRefNameAlreadyExists() => ServerResources.Get(nameof (FieldNameorRefNameAlreadyExists));

    public static string FieldNameorRefNameAlreadyExists(CultureInfo culture) => ServerResources.Get(nameof (FieldNameorRefNameAlreadyExists), culture);

    public static string WorkItemTypeRefNameDoesNotExist(object arg0) => ServerResources.Format(nameof (WorkItemTypeRefNameDoesNotExist), arg0);

    public static string WorkItemTypeRefNameDoesNotExist(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemTypeRefNameDoesNotExist), culture, arg0);

    public static string FormLayoutGroupDoesNotExistException(object arg0, object arg1) => ServerResources.Format(nameof (FormLayoutGroupDoesNotExistException), arg0, arg1);

    public static string FormLayoutGroupDoesNotExistException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (FormLayoutGroupDoesNotExistException), culture, arg0, arg1);
    }

    public static string ProcessWorkItemTypeFieldDoesNotExistException(object arg0, object arg1) => ServerResources.Format(nameof (ProcessWorkItemTypeFieldDoesNotExistException), arg0, arg1);

    public static string ProcessWorkItemTypeFieldDoesNotExistException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ProcessWorkItemTypeFieldDoesNotExistException), culture, arg0, arg1);
    }

    public static string InheritedWorkItemTypeAlreadyExists(object arg0, object arg1) => ServerResources.Format(nameof (InheritedWorkItemTypeAlreadyExists), arg0, arg1);

    public static string InheritedWorkItemTypeAlreadyExists(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (InheritedWorkItemTypeAlreadyExists), culture, arg0, arg1);
    }

    public static string FieldCouldNotBeFound(object arg0) => ServerResources.Format(nameof (FieldCouldNotBeFound), arg0);

    public static string FieldCouldNotBeFound(object arg0, CultureInfo culture) => ServerResources.Format(nameof (FieldCouldNotBeFound), culture, arg0);

    public static string FieldAlreadyExistsOnPage(object arg0, object arg1) => ServerResources.Format(nameof (FieldAlreadyExistsOnPage), arg0, arg1);

    public static string FieldAlreadyExistsOnPage(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (FieldAlreadyExistsOnPage), culture, arg0, arg1);

    public static string NewFormAreaPathLabel() => ServerResources.Get(nameof (NewFormAreaPathLabel));

    public static string NewFormAreaPathLabel(CultureInfo culture) => ServerResources.Get(nameof (NewFormAreaPathLabel), culture);

    public static string NewFormAssignedToLabel() => ServerResources.Get(nameof (NewFormAssignedToLabel));

    public static string NewFormAssignedToLabel(CultureInfo culture) => ServerResources.Get(nameof (NewFormAssignedToLabel), culture);

    public static string NewFormAssignedToWatermark() => ServerResources.Get(nameof (NewFormAssignedToWatermark));

    public static string NewFormAssignedToWatermark(CultureInfo culture) => ServerResources.Get(nameof (NewFormAssignedToWatermark), culture);

    public static string NewFormChangedByLabel() => ServerResources.Get(nameof (NewFormChangedByLabel));

    public static string NewFormChangedByLabel(CultureInfo culture) => ServerResources.Get(nameof (NewFormChangedByLabel), culture);

    public static string NewFormChangedDateLabel() => ServerResources.Get(nameof (NewFormChangedDateLabel));

    public static string NewFormChangedDateLabel(CultureInfo culture) => ServerResources.Get(nameof (NewFormChangedDateLabel), culture);

    public static string NewFormIterationLabel() => ServerResources.Get(nameof (NewFormIterationLabel));

    public static string NewFormIterationLabel(CultureInfo culture) => ServerResources.Get(nameof (NewFormIterationLabel), culture);

    public static string NewFormStateLabel() => ServerResources.Get(nameof (NewFormStateLabel));

    public static string NewFormStateLabel(CultureInfo culture) => ServerResources.Get(nameof (NewFormStateLabel), culture);

    public static string NewFormTitleWatermark() => ServerResources.Get(nameof (NewFormTitleWatermark));

    public static string NewFormTitleWatermark(CultureInfo culture) => ServerResources.Get(nameof (NewFormTitleWatermark), culture);

    public static string SystemDescriptionReadonly() => ServerResources.Get(nameof (SystemDescriptionReadonly));

    public static string SystemDescriptionReadonly(CultureInfo culture) => ServerResources.Get(nameof (SystemDescriptionReadonly), culture);

    public static string DevelopmentGroupTitle() => ServerResources.Get(nameof (DevelopmentGroupTitle));

    public static string DevelopmentGroupTitle(CultureInfo culture) => ServerResources.Get(nameof (DevelopmentGroupTitle), culture);

    public static string FormLayoutPageDoesNotExistException(object arg0, object arg1) => ServerResources.Format(nameof (FormLayoutPageDoesNotExistException), arg0, arg1);

    public static string FormLayoutPageDoesNotExistException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (FormLayoutPageDoesNotExistException), culture, arg0, arg1);
    }

    public static string FormLayoutSectionDoesNotExistException(object arg0, object arg1) => ServerResources.Format(nameof (FormLayoutSectionDoesNotExistException), arg0, arg1);

    public static string FormLayoutSectionDoesNotExistException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (FormLayoutSectionDoesNotExistException), culture, arg0, arg1);
    }

    public static string WorkItemTypeRuleDefaultStringValueInvalid(object arg0, object arg1) => ServerResources.Format(nameof (WorkItemTypeRuleDefaultStringValueInvalid), arg0, arg1);

    public static string WorkItemTypeRuleDefaultStringValueInvalid(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemTypeRuleDefaultStringValueInvalid), culture, arg0, arg1);
    }

    public static string WorkItemTypeCustomizationBlocked(object arg0) => ServerResources.Format(nameof (WorkItemTypeCustomizationBlocked), arg0);

    public static string WorkItemTypeCustomizationBlocked(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemTypeCustomizationBlocked), culture, arg0);

    public static string FormLayoutGroupHasChildrenException(object arg0, object arg1) => ServerResources.Format(nameof (FormLayoutGroupHasChildrenException), arg0, arg1);

    public static string FormLayoutGroupHasChildrenException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (FormLayoutGroupHasChildrenException), culture, arg0, arg1);
    }

    public static string WorkItemNonDeletableLinkException(object arg0, object arg1) => ServerResources.Format(nameof (WorkItemNonDeletableLinkException), arg0, arg1);

    public static string WorkItemNonDeletableLinkException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemNonDeletableLinkException), culture, arg0, arg1);
    }

    public static string WorkItemNonDeletableException(object arg0, object arg1) => ServerResources.Format(nameof (WorkItemNonDeletableException), arg0, arg1);

    public static string WorkItemNonDeletableException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemNonDeletableException), culture, arg0, arg1);
    }

    public static string UnauthorizedBypassRules() => ServerResources.Get(nameof (UnauthorizedBypassRules));

    public static string UnauthorizedBypassRules(CultureInfo culture) => ServerResources.Get(nameof (UnauthorizedBypassRules), culture);

    public static string FormLayoutControlDoesNotExistException(object arg0, object arg1) => ServerResources.Format(nameof (FormLayoutControlDoesNotExistException), arg0, arg1);

    public static string FormLayoutControlDoesNotExistException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (FormLayoutControlDoesNotExistException), culture, arg0, arg1);
    }

    public static string FormLayoutLabelExceededLimitException(object arg0, object arg1) => ServerResources.Format(nameof (FormLayoutLabelExceededLimitException), arg0, arg1);

    public static string FormLayoutLabelExceededLimitException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (FormLayoutLabelExceededLimitException), culture, arg0, arg1);
    }

    public static string FormLayoutLabelInvalidException(object arg0) => ServerResources.Format(nameof (FormLayoutLabelInvalidException), arg0);

    public static string FormLayoutLabelInvalidException(object arg0, CultureInfo culture) => ServerResources.Format(nameof (FormLayoutLabelInvalidException), culture, arg0);

    public static string FormLayoutSectionInvalidException(object arg0) => ServerResources.Format(nameof (FormLayoutSectionInvalidException), arg0);

    public static string FormLayoutSectionInvalidException(object arg0, CultureInfo culture) => ServerResources.Format(nameof (FormLayoutSectionInvalidException), culture, arg0);

    public static string WorkItemDeleteOrRestoreWithOtherUpdatesException(object arg0) => ServerResources.Format(nameof (WorkItemDeleteOrRestoreWithOtherUpdatesException), arg0);

    public static string WorkItemDeleteOrRestoreWithOtherUpdatesException(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemDeleteOrRestoreWithOtherUpdatesException), culture, arg0);
    }

    public static string WorkItemInvalidUpdateToIsDeletedFieldException(object arg0) => ServerResources.Format(nameof (WorkItemInvalidUpdateToIsDeletedFieldException), arg0);

    public static string WorkItemInvalidUpdateToIsDeletedFieldException(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemInvalidUpdateToIsDeletedFieldException), culture, arg0);
    }

    public static string WorkItemInvalidRestoreRequestException() => ServerResources.Get(nameof (WorkItemInvalidRestoreRequestException));

    public static string WorkItemInvalidRestoreRequestException(CultureInfo culture) => ServerResources.Get(nameof (WorkItemInvalidRestoreRequestException), culture);

    public static string WorkItemTargetProjectDoesNotExist(object arg0) => ServerResources.Format(nameof (WorkItemTargetProjectDoesNotExist), arg0);

    public static string WorkItemTargetProjectDoesNotExist(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemTargetProjectDoesNotExist), culture, arg0);

    public static string WorkItemTargetProjectWithIdDoesNotExist(object arg0) => ServerResources.Format(nameof (WorkItemTargetProjectWithIdDoesNotExist), arg0);

    public static string WorkItemTargetProjectWithIdDoesNotExist(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemTargetProjectWithIdDoesNotExist), culture, arg0);

    public static string WorkItemTargetTypeDoesNotExist(object arg0, object arg1) => ServerResources.Format(nameof (WorkItemTargetTypeDoesNotExist), arg0, arg1);

    public static string WorkItemTargetTypeDoesNotExist(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemTargetTypeDoesNotExist), culture, arg0, arg1);
    }

    public static string WorkItemTargetTypeNotSupported(object arg0) => ServerResources.Format(nameof (WorkItemTargetTypeNotSupported), arg0);

    public static string WorkItemTargetTypeNotSupported(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemTargetTypeNotSupported), culture, arg0);

    public static string WorkItemTypeNotChangeable(object arg0) => ServerResources.Format(nameof (WorkItemTypeNotChangeable), arg0);

    public static string WorkItemTypeNotChangeable(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemTypeNotChangeable), culture, arg0);

    public static string WorkItemUnauthorizedMove(object arg0) => ServerResources.Format(nameof (WorkItemUnauthorizedMove), arg0);

    public static string WorkItemUnauthorizedMove(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemUnauthorizedMove), culture, arg0);

    public static string WorkItemUnauthorizedDelete(object arg0) => ServerResources.Format(nameof (WorkItemUnauthorizedDelete), arg0);

    public static string WorkItemUnauthorizedDelete(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemUnauthorizedDelete), culture, arg0);

    public static string WorkItemUnauthorizedRestore(object arg0) => ServerResources.Format(nameof (WorkItemUnauthorizedRestore), arg0);

    public static string WorkItemUnauthorizedRestore(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemUnauthorizedRestore), culture, arg0);

    public static string WorkItemAreaPathDoesNotMatchTargetProject() => ServerResources.Get(nameof (WorkItemAreaPathDoesNotMatchTargetProject));

    public static string WorkItemAreaPathDoesNotMatchTargetProject(CultureInfo culture) => ServerResources.Get(nameof (WorkItemAreaPathDoesNotMatchTargetProject), culture);

    public static string WorkItemIterationPathDoesNotMatchTargetProject() => ServerResources.Get(nameof (WorkItemIterationPathDoesNotMatchTargetProject));

    public static string WorkItemIterationPathDoesNotMatchTargetProject(CultureInfo culture) => ServerResources.Get(nameof (WorkItemIterationPathDoesNotMatchTargetProject), culture);

    public static string WorkItemTargetAreaPathNotProvided() => ServerResources.Get(nameof (WorkItemTargetAreaPathNotProvided));

    public static string WorkItemTargetAreaPathNotProvided(CultureInfo culture) => ServerResources.Get(nameof (WorkItemTargetAreaPathNotProvided), culture);

    public static string WorkItemTargetIterationPathNotProvided() => ServerResources.Get(nameof (WorkItemTargetIterationPathNotProvided));

    public static string WorkItemTargetIterationPathNotProvided(CultureInfo culture) => ServerResources.Get(nameof (WorkItemTargetIterationPathNotProvided), culture);

    public static string WorkItemNotMovableLinkException(object arg0, object arg1) => ServerResources.Format(nameof (WorkItemNotMovableLinkException), arg0, arg1);

    public static string WorkItemNotMovableLinkException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemNotMovableLinkException), culture, arg0, arg1);
    }

    public static string WorkItemNotMovableException(object arg0, object arg1) => ServerResources.Format(nameof (WorkItemNotMovableException), arg0, arg1);

    public static string WorkItemNotMovableException(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (WorkItemNotMovableException), culture, arg0, arg1);

    public static string ValidatorUnsupportedIdentityFieldUsage(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorUnsupportedIdentityFieldUsage), arg0, arg1);

    public static string ValidatorUnsupportedIdentityFieldUsage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorUnsupportedIdentityFieldUsage), culture, arg0, arg1);
    }

    public static string FormLayoutControlsLimitExceededException(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServerResources.Format(nameof (FormLayoutControlsLimitExceededException), arg0, arg1, arg2);
    }

    public static string FormLayoutControlsLimitExceededException(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (FormLayoutControlsLimitExceededException), culture, arg0, arg1, arg2);
    }

    public static string FormLayoutGroupsLimitExceededException(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServerResources.Format(nameof (FormLayoutGroupsLimitExceededException), arg0, arg1, arg2);
    }

    public static string FormLayoutGroupsLimitExceededException(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (FormLayoutGroupsLimitExceededException), culture, arg0, arg1, arg2);
    }

    public static string PickListCountExceeded(object arg0) => ServerResources.Format(nameof (PickListCountExceeded), arg0);

    public static string PickListCountExceeded(object arg0, CultureInfo culture) => ServerResources.Format(nameof (PickListCountExceeded), culture, arg0);

    public static string PickListDoesNotExist(object arg0) => ServerResources.Format(nameof (PickListDoesNotExist), arg0);

    public static string PickListDoesNotExist(object arg0, CultureInfo culture) => ServerResources.Format(nameof (PickListDoesNotExist), culture, arg0);

    public static string PickListItemCountExceeded(object arg0, object arg1, object arg2) => ServerResources.Format(nameof (PickListItemCountExceeded), arg0, arg1, arg2);

    public static string PickListItemCountExceeded(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (PickListItemCountExceeded), culture, arg0, arg1, arg2);
    }

    public static string PickListItemTooLong(object arg0, object arg1) => ServerResources.Format(nameof (PickListItemTooLong), arg0, arg1);

    public static string PickListItemTooLong(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (PickListItemTooLong), culture, arg0, arg1);

    public static string PickListNameAlreadyInUse(object arg0) => ServerResources.Format(nameof (PickListNameAlreadyInUse), arg0);

    public static string PickListNameAlreadyInUse(object arg0, CultureInfo culture) => ServerResources.Format(nameof (PickListNameAlreadyInUse), culture, arg0);

    public static string FieldValue_Yes() => ServerResources.Get(nameof (FieldValue_Yes));

    public static string FieldValue_Yes(CultureInfo culture) => ServerResources.Get(nameof (FieldValue_Yes), culture);

    public static string PickListNotReferencedByField(object arg0) => ServerResources.Format(nameof (PickListNotReferencedByField), arg0);

    public static string PickListNotReferencedByField(object arg0, CultureInfo culture) => ServerResources.Format(nameof (PickListNotReferencedByField), culture, arg0);

    public static string WorkItemRestoreLinksUnrecoverableComment(object arg0) => ServerResources.Format(nameof (WorkItemRestoreLinksUnrecoverableComment), arg0);

    public static string WorkItemRestoreLinksUnrecoverableComment(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemRestoreLinksUnrecoverableComment), culture, arg0);

    public static string InvalidListItemType(object arg0, object arg1) => ServerResources.Format(nameof (InvalidListItemType), arg0, arg1);

    public static string InvalidListItemType(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (InvalidListItemType), culture, arg0, arg1);

    public static string InvalidPicklistTypeForField(object arg0, object arg1) => ServerResources.Format(nameof (InvalidPicklistTypeForField), arg0, arg1);

    public static string InvalidPicklistTypeForField(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (InvalidPicklistTypeForField), culture, arg0, arg1);

    public static string PicklistProcessAndFieldProcessDoNotMatch(object arg0, object arg1) => ServerResources.Format(nameof (PicklistProcessAndFieldProcessDoNotMatch), arg0, arg1);

    public static string PicklistProcessAndFieldProcessDoNotMatch(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (PicklistProcessAndFieldProcessDoNotMatch), culture, arg0, arg1);
    }

    public static string CannotDeletePicklistReferencedByField() => ServerResources.Get(nameof (CannotDeletePicklistReferencedByField));

    public static string CannotDeletePicklistReferencedByField(CultureInfo culture) => ServerResources.Get(nameof (CannotDeletePicklistReferencedByField), culture);

    public static string DuplicateListItem(object arg0) => ServerResources.Format(nameof (DuplicateListItem), arg0);

    public static string DuplicateListItem(object arg0, CultureInfo culture) => ServerResources.Format(nameof (DuplicateListItem), culture, arg0);

    public static string ProcessWorkItemTypeNameAlreadyInUse(object arg0) => ServerResources.Format(nameof (ProcessWorkItemTypeNameAlreadyInUse), arg0);

    public static string ProcessWorkItemTypeNameAlreadyInUse(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ProcessWorkItemTypeNameAlreadyInUse), culture, arg0);

    public static string ProcessWorkItemTypesLimitExceeded(object arg0) => ServerResources.Format(nameof (ProcessWorkItemTypesLimitExceeded), arg0);

    public static string ProcessWorkItemTypesLimitExceeded(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ProcessWorkItemTypesLimitExceeded), culture, arg0);

    public static string WorkItemTrackingInvalidColor(object arg0) => ServerResources.Format(nameof (WorkItemTrackingInvalidColor), arg0);

    public static string WorkItemTrackingInvalidColor(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemTrackingInvalidColor), culture, arg0);

    public static string WorkItemTrackingInvalidColorWithoutHash(object arg0) => ServerResources.Format(nameof (WorkItemTrackingInvalidColorWithoutHash), arg0);

    public static string WorkItemTrackingInvalidColorWithoutHash(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemTrackingInvalidColorWithoutHash), culture, arg0);

    public static string WorkItemTrackingInvalidIconId(object arg0) => ServerResources.Format(nameof (WorkItemTrackingInvalidIconId), arg0);

    public static string WorkItemTrackingInvalidIconId(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemTrackingInvalidIconId), culture, arg0);

    public static string WorkItemTrackingIconNotSupported() => ServerResources.Get(nameof (WorkItemTrackingIconNotSupported));

    public static string WorkItemTrackingIconNotSupported(CultureInfo culture) => ServerResources.Get(nameof (WorkItemTrackingIconNotSupported), culture);

    public static string FormLayoutHtmlGroupInvalidControlException() => ServerResources.Get(nameof (FormLayoutHtmlGroupInvalidControlException));

    public static string FormLayoutHtmlGroupInvalidControlException(CultureInfo culture) => ServerResources.Get(nameof (FormLayoutHtmlGroupInvalidControlException), culture);

    public static string FormLayoutAddHTMLControlToGroupException() => ServerResources.Get(nameof (FormLayoutAddHTMLControlToGroupException));

    public static string FormLayoutAddHTMLControlToGroupException(CultureInfo culture) => ServerResources.Get(nameof (FormLayoutAddHTMLControlToGroupException), culture);

    public static string FormLayoutSectionInvalidControlException() => ServerResources.Get(nameof (FormLayoutSectionInvalidControlException));

    public static string FormLayoutSectionInvalidControlException(CultureInfo culture) => ServerResources.Get(nameof (FormLayoutSectionInvalidControlException), culture);

    public static string LeadingNumbers() => ServerResources.Get(nameof (LeadingNumbers));

    public static string LeadingNumbers(CultureInfo culture) => ServerResources.Get(nameof (LeadingNumbers), culture);

    public static string PickListMetadataDoesNotExist() => ServerResources.Get(nameof (PickListMetadataDoesNotExist));

    public static string PickListMetadataDoesNotExist(CultureInfo culture) => ServerResources.Get(nameof (PickListMetadataDoesNotExist), culture);

    public static string StateActive() => ServerResources.Get(nameof (StateActive));

    public static string StateActive(CultureInfo culture) => ServerResources.Get(nameof (StateActive), culture);

    public static string StateClosed() => ServerResources.Get(nameof (StateClosed));

    public static string StateClosed(CultureInfo culture) => ServerResources.Get(nameof (StateClosed), culture);

    public static string StateDefaultReason(object arg0) => ServerResources.Format(nameof (StateDefaultReason), arg0);

    public static string StateDefaultReason(object arg0, CultureInfo culture) => ServerResources.Format(nameof (StateDefaultReason), culture, arg0);

    public static string WorkItemTypeRuleOnFieldNotSupport(object arg0) => ServerResources.Format(nameof (WorkItemTypeRuleOnFieldNotSupport), arg0);

    public static string WorkItemTypeRuleOnFieldNotSupport(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemTypeRuleOnFieldNotSupport), culture, arg0);

    public static string WorkItemStateCustomizationNotSupported() => ServerResources.Get(nameof (WorkItemStateCustomizationNotSupported));

    public static string WorkItemStateCustomizationNotSupported(CultureInfo culture) => ServerResources.Get(nameof (WorkItemStateCustomizationNotSupported), culture);

    public static string WorkItemStateDefinitionNotFound(object arg0) => ServerResources.Format(nameof (WorkItemStateDefinitionNotFound), arg0);

    public static string WorkItemStateDefinitionNotFound(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemStateDefinitionNotFound), culture, arg0);

    public static string WorkItemStateOrderInvalid(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ServerResources.Format(nameof (WorkItemStateOrderInvalid), arg0, arg1, arg2, arg3);
    }

    public static string WorkItemStateOrderInvalid(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemStateOrderInvalid), culture, arg0, arg1, arg2, arg3);
    }

    public static string WorkItemStateNameAlreadyInUse(object arg0) => ServerResources.Format(nameof (WorkItemStateNameAlreadyInUse), arg0);

    public static string WorkItemStateNameAlreadyInUse(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemStateNameAlreadyInUse), culture, arg0);

    public static string WorkItemTypeStateLimitExceeded(object arg0) => ServerResources.Format(nameof (WorkItemTypeStateLimitExceeded), arg0);

    public static string WorkItemTypeStateLimitExceeded(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemTypeStateLimitExceeded), culture, arg0);

    public static string WorkItemTypeTwoStateRestriction(object arg0, object arg1, object arg2) => ServerResources.Format(nameof (WorkItemTypeTwoStateRestriction), arg0, arg1, arg2);

    public static string WorkItemTypeTwoStateRestriction(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemTypeTwoStateRestriction), culture, arg0, arg1, arg2);
    }

    public static string WorkItemStateNameChangeNotAllowed() => ServerResources.Get(nameof (WorkItemStateNameChangeNotAllowed));

    public static string WorkItemStateNameChangeNotAllowed(CultureInfo culture) => ServerResources.Get(nameof (WorkItemStateNameChangeNotAllowed), culture);

    public static string WorkItemStateHideInvalid(object arg0) => ServerResources.Format(nameof (WorkItemStateHideInvalid), arg0);

    public static string WorkItemStateHideInvalid(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemStateHideInvalid), culture, arg0);

    public static string WorkItemStateDefinitionAlreadyExists(object arg0) => ServerResources.Format(nameof (WorkItemStateDefinitionAlreadyExists), arg0);

    public static string WorkItemStateDefinitionAlreadyExists(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemStateDefinitionAlreadyExists), culture, arg0);

    public static string WorkItemStateNoStateInCategory(object arg0) => ServerResources.Format(nameof (WorkItemStateNoStateInCategory), arg0);

    public static string WorkItemStateNoStateInCategory(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemStateNoStateInCategory), culture, arg0);

    public static string FormLayoutPageInvalidOperationException() => ServerResources.Get(nameof (FormLayoutPageInvalidOperationException));

    public static string FormLayoutPageInvalidOperationException(CultureInfo culture) => ServerResources.Get(nameof (FormLayoutPageInvalidOperationException), culture);

    public static string FormLayoutPagesLimitExceededException(object arg0, object arg1) => ServerResources.Format(nameof (FormLayoutPagesLimitExceededException), arg0, arg1);

    public static string FormLayoutPagesLimitExceededException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (FormLayoutPagesLimitExceededException), culture, arg0, arg1);
    }

    public static string BooleanFieldTypeMustBeRequired() => ServerResources.Get(nameof (BooleanFieldTypeMustBeRequired));

    public static string BooleanFieldTypeMustBeRequired(CultureInfo culture) => ServerResources.Get(nameof (BooleanFieldTypeMustBeRequired), culture);

    public static string BooleanFieldTypeMustHaveDefault() => ServerResources.Get(nameof (BooleanFieldTypeMustHaveDefault));

    public static string BooleanFieldTypeMustHaveDefault(CultureInfo culture) => ServerResources.Get(nameof (BooleanFieldTypeMustHaveDefault), culture);

    public static string ValidatorRequiredFieldsNotOnLayout(object arg0, object arg1) => ServerResources.Format(nameof (ValidatorRequiredFieldsNotOnLayout), arg0, arg1);

    public static string ValidatorRequiredFieldsNotOnLayout(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorRequiredFieldsNotOnLayout), culture, arg0, arg1);
    }

    public static string StateDefaultReasonOut(object arg0) => ServerResources.Format(nameof (StateDefaultReasonOut), arg0);

    public static string StateDefaultReasonOut(object arg0, CultureInfo culture) => ServerResources.Format(nameof (StateDefaultReasonOut), culture, arg0);

    public static string FieldNotCustomizableException(object arg0) => ServerResources.Format(nameof (FieldNotCustomizableException), arg0);

    public static string FieldNotCustomizableException(object arg0, CultureInfo culture) => ServerResources.Format(nameof (FieldNotCustomizableException), culture, arg0);

    public static string FormLayoutControlAlreadyExistInGroupException(object arg0, object arg1) => ServerResources.Format(nameof (FormLayoutControlAlreadyExistInGroupException), arg0, arg1);

    public static string FormLayoutControlAlreadyExistInGroupException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (FormLayoutControlAlreadyExistInGroupException), culture, arg0, arg1);
    }

    public static string FormLayoutGroupAlreadyExistsException(object arg0, object arg1) => ServerResources.Format(nameof (FormLayoutGroupAlreadyExistsException), arg0, arg1);

    public static string FormLayoutGroupAlreadyExistsException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (FormLayoutGroupAlreadyExistsException), culture, arg0, arg1);
    }

    public static string FormLayoutPageAlreadyExistsException(object arg0) => ServerResources.Format(nameof (FormLayoutPageAlreadyExistsException), arg0);

    public static string FormLayoutPageAlreadyExistsException(object arg0, CultureInfo culture) => ServerResources.Format(nameof (FormLayoutPageAlreadyExistsException), culture, arg0);

    public static string RecipientWorkItemAccessDenied() => ServerResources.Get(nameof (RecipientWorkItemAccessDenied));

    public static string RecipientWorkItemAccessDenied(CultureInfo culture) => ServerResources.Get(nameof (RecipientWorkItemAccessDenied), culture);

    public static string WorkItemCommentNotFound(object arg0, object arg1) => ServerResources.Format(nameof (WorkItemCommentNotFound), arg0, arg1);

    public static string WorkItemCommentNotFound(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (WorkItemCommentNotFound), culture, arg0, arg1);

    public static string WorkItemMaxCommentTextSizeExceeded(object arg0) => ServerResources.Format(nameof (WorkItemMaxCommentTextSizeExceeded), arg0);

    public static string WorkItemMaxCommentTextSizeExceeded(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemMaxCommentTextSizeExceeded), culture, arg0);

    public static string WorkItemCommentUnsupportedAccessMessage() => ServerResources.Get(nameof (WorkItemCommentUnsupportedAccessMessage));

    public static string WorkItemCommentUnsupportedAccessMessage(CultureInfo culture) => ServerResources.Get(nameof (WorkItemCommentUnsupportedAccessMessage), culture);

    public static string WorkItemStateBlockCompletedCategoryChanges() => ServerResources.Get(nameof (WorkItemStateBlockCompletedCategoryChanges));

    public static string WorkItemStateBlockCompletedCategoryChanges(CultureInfo culture) => ServerResources.Get(nameof (WorkItemStateBlockCompletedCategoryChanges), culture);

    public static string CreateStateDefinitionFailed() => ServerResources.Get(nameof (CreateStateDefinitionFailed));

    public static string CreateStateDefinitionFailed(CultureInfo culture) => ServerResources.Get(nameof (CreateStateDefinitionFailed), culture);

    public static string CSS_NODE_GENERIC_READ() => ServerResources.Get(nameof (CSS_NODE_GENERIC_READ));

    public static string CSS_NODE_GENERIC_READ(CultureInfo culture) => ServerResources.Get(nameof (CSS_NODE_GENERIC_READ), culture);

    public static string CSS_NODE_GENERIC_WRITE() => ServerResources.Get(nameof (CSS_NODE_GENERIC_WRITE));

    public static string CSS_NODE_GENERIC_WRITE(CultureInfo culture) => ServerResources.Get(nameof (CSS_NODE_GENERIC_WRITE), culture);

    public static string CSS_NODE_CREATE_CHILDREN() => ServerResources.Get(nameof (CSS_NODE_CREATE_CHILDREN));

    public static string CSS_NODE_CREATE_CHILDREN(CultureInfo culture) => ServerResources.Get(nameof (CSS_NODE_CREATE_CHILDREN), culture);

    public static string CSS_NODE_DELETE() => ServerResources.Get(nameof (CSS_NODE_DELETE));

    public static string CSS_NODE_DELETE(CultureInfo culture) => ServerResources.Get(nameof (CSS_NODE_DELETE), culture);

    public static string CSS_NODE_WORK_ITEM_WRITE() => ServerResources.Get(nameof (CSS_NODE_WORK_ITEM_WRITE));

    public static string CSS_NODE_WORK_ITEM_WRITE(CultureInfo culture) => ServerResources.Get(nameof (CSS_NODE_WORK_ITEM_WRITE), culture);

    public static string CSS_NODE_WORK_ITEM_READ() => ServerResources.Get(nameof (CSS_NODE_WORK_ITEM_READ));

    public static string CSS_NODE_WORK_ITEM_READ(CultureInfo culture) => ServerResources.Get(nameof (CSS_NODE_WORK_ITEM_READ), culture);

    public static string CSS_NODE_MANAGE_TEST_PLANS() => ServerResources.Get(nameof (CSS_NODE_MANAGE_TEST_PLANS));

    public static string CSS_NODE_MANAGE_TEST_PLANS(CultureInfo culture) => ServerResources.Get(nameof (CSS_NODE_MANAGE_TEST_PLANS), culture);

    public static string CSS_NODE_MANAGE_TEST_SUITES() => ServerResources.Get(nameof (CSS_NODE_MANAGE_TEST_SUITES));

    public static string CSS_NODE_MANAGE_TEST_SUITES(CultureInfo culture) => ServerResources.Get(nameof (CSS_NODE_MANAGE_TEST_SUITES), culture);

    public static string CSS_NODE_WORK_ITEM_SAVE_COMMENT() => ServerResources.Get(nameof (CSS_NODE_WORK_ITEM_SAVE_COMMENT));

    public static string CSS_NODE_WORK_ITEM_SAVE_COMMENT(CultureInfo culture) => ServerResources.Get(nameof (CSS_NODE_WORK_ITEM_SAVE_COMMENT), culture);

    public static string Completed() => ServerResources.Get(nameof (Completed));

    public static string Completed(CultureInfo culture) => ServerResources.Get(nameof (Completed), culture);

    public static string InProgress() => ServerResources.Get(nameof (InProgress));

    public static string InProgress(CultureInfo culture) => ServerResources.Get(nameof (InProgress), culture);

    public static string Proposed() => ServerResources.Get(nameof (Proposed));

    public static string Proposed(CultureInfo culture) => ServerResources.Get(nameof (Proposed), culture);

    public static string Resolved() => ServerResources.Get(nameof (Resolved));

    public static string Resolved(CultureInfo culture) => ServerResources.Get(nameof (Resolved), culture);

    public static string FormLayoutInfoNotAvailable(object arg0, object arg1) => ServerResources.Format(nameof (FormLayoutInfoNotAvailable), arg0, arg1);

    public static string FormLayoutInfoNotAvailable(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (FormLayoutInfoNotAvailable), culture, arg0, arg1);

    public static string NewFormReasonLabel() => ServerResources.Get(nameof (NewFormReasonLabel));

    public static string NewFormReasonLabel(CultureInfo culture) => ServerResources.Get(nameof (NewFormReasonLabel), culture);

    public static string WorkItemInlineImgPlaceHolderURL() => ServerResources.Get(nameof (WorkItemInlineImgPlaceHolderURL));

    public static string WorkItemInlineImgPlaceHolderURL(CultureInfo culture) => ServerResources.Get(nameof (WorkItemInlineImgPlaceHolderURL), culture);

    public static string FormLayoutStatusGroup() => ServerResources.Get(nameof (FormLayoutStatusGroup));

    public static string FormLayoutStatusGroup(CultureInfo culture) => ServerResources.Get(nameof (FormLayoutStatusGroup), culture);

    public static string WorkItemTrackingNothingToUpdate() => ServerResources.Get(nameof (WorkItemTrackingNothingToUpdate));

    public static string WorkItemTrackingNothingToUpdate(CultureInfo culture) => ServerResources.Get(nameof (WorkItemTrackingNothingToUpdate), culture);

    public static string StateNew() => ServerResources.Get(nameof (StateNew));

    public static string StateNew(CultureInfo culture) => ServerResources.Get(nameof (StateNew), culture);

    public static string ContributionId() => ServerResources.Get(nameof (ContributionId));

    public static string ContributionId(CultureInfo culture) => ServerResources.Get(nameof (ContributionId), culture);

    public static string Name() => ServerResources.Get(nameof (Name));

    public static string Name(CultureInfo culture) => ServerResources.Get(nameof (Name), culture);

    public static string Description() => ServerResources.Get(nameof (Description));

    public static string Description(CultureInfo culture) => ServerResources.Get(nameof (Description), culture);

    public static string PageContribution() => ServerResources.Get(nameof (PageContribution));

    public static string PageContribution(CultureInfo culture) => ServerResources.Get(nameof (PageContribution), culture);

    public static string GroupContribution() => ServerResources.Get(nameof (GroupContribution));

    public static string GroupContribution(CultureInfo culture) => ServerResources.Get(nameof (GroupContribution), culture);

    public static string ControlContribution() => ServerResources.Get(nameof (ControlContribution));

    public static string ControlContribution(CultureInfo culture) => ServerResources.Get(nameof (ControlContribution), culture);

    public static string ExtensionMoreInformation() => ServerResources.Get(nameof (ExtensionMoreInformation));

    public static string ExtensionMoreInformation(CultureInfo culture) => ServerResources.Get(nameof (ExtensionMoreInformation), culture);

    public static string Extension() => ServerResources.Get(nameof (Extension));

    public static string Extension(CultureInfo culture) => ServerResources.Get(nameof (Extension), culture);

    public static string WorkItemExtensions() => ServerResources.Get(nameof (WorkItemExtensions));

    public static string WorkItemExtensions(CultureInfo culture) => ServerResources.Get(nameof (WorkItemExtensions), culture);

    public static string Inputs() => ServerResources.Get(nameof (Inputs));

    public static string Inputs(CultureInfo culture) => ServerResources.Get(nameof (Inputs), culture);

    public static string IsRequired() => ServerResources.Get(nameof (IsRequired));

    public static string IsRequired(CultureInfo culture) => ServerResources.Get(nameof (IsRequired), culture);

    public static string Type() => ServerResources.Get(nameof (Type));

    public static string Type(CultureInfo culture) => ServerResources.Get(nameof (Type), culture);

    public static string ExtensionId() => ServerResources.Get(nameof (ExtensionId));

    public static string ExtensionId(CultureInfo culture) => ServerResources.Get(nameof (ExtensionId), culture);

    public static string CannotModifyFieldWithoutBypassingRules(object arg0) => ServerResources.Format(nameof (CannotModifyFieldWithoutBypassingRules), arg0);

    public static string CannotModifyFieldWithoutBypassingRules(object arg0, CultureInfo culture) => ServerResources.Format(nameof (CannotModifyFieldWithoutBypassingRules), culture, arg0);

    public static string WorkItemTemplateDescriptionTooLong(object arg0) => ServerResources.Format(nameof (WorkItemTemplateDescriptionTooLong), arg0);

    public static string WorkItemTemplateDescriptionTooLong(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemTemplateDescriptionTooLong), culture, arg0);

    public static string WorkItemTemplateDuplicateFieldRefName(object arg0) => ServerResources.Format(nameof (WorkItemTemplateDuplicateFieldRefName), arg0);

    public static string WorkItemTemplateDuplicateFieldRefName(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemTemplateDuplicateFieldRefName), culture, arg0);

    public static string WorkItemTemplateFieldRefNameContainsWhiteSpace(object arg0) => ServerResources.Format(nameof (WorkItemTemplateFieldRefNameContainsWhiteSpace), arg0);

    public static string WorkItemTemplateFieldRefNameContainsWhiteSpace(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemTemplateFieldRefNameContainsWhiteSpace), culture, arg0);
    }

    public static string WorkItemTemplateFieldRefNameTooLong(object arg0, object arg1) => ServerResources.Format(nameof (WorkItemTemplateFieldRefNameTooLong), arg0, arg1);

    public static string WorkItemTemplateFieldRefNameTooLong(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemTemplateFieldRefNameTooLong), culture, arg0, arg1);
    }

    public static string WorkItemTemplateLimitExceeded(object arg0) => ServerResources.Format(nameof (WorkItemTemplateLimitExceeded), arg0);

    public static string WorkItemTemplateLimitExceeded(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemTemplateLimitExceeded), culture, arg0);

    public static string WorkItemTemplateFieldsNotSpecified() => ServerResources.Get(nameof (WorkItemTemplateFieldsNotSpecified));

    public static string WorkItemTemplateFieldsNotSpecified(CultureInfo culture) => ServerResources.Get(nameof (WorkItemTemplateFieldsNotSpecified), culture);

    public static string WorkItemTemplateFieldValueTooLong(object arg0, object arg1) => ServerResources.Format(nameof (WorkItemTemplateFieldValueTooLong), arg0, arg1);

    public static string WorkItemTemplateFieldValueTooLong(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemTemplateFieldValueTooLong), culture, arg0, arg1);
    }

    public static string WorkItemTemplateNameNullOrEmpty() => ServerResources.Get(nameof (WorkItemTemplateNameNullOrEmpty));

    public static string WorkItemTemplateNameNullOrEmpty(CultureInfo culture) => ServerResources.Get(nameof (WorkItemTemplateNameNullOrEmpty), culture);

    public static string WorkItemTemplateNameTooLong(object arg0) => ServerResources.Format(nameof (WorkItemTemplateNameTooLong), arg0);

    public static string WorkItemTemplateNameTooLong(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemTemplateNameTooLong), culture, arg0);

    public static string WorkItemTemplateNotFound(object arg0) => ServerResources.Format(nameof (WorkItemTemplateNotFound), arg0);

    public static string WorkItemTemplateNotFound(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemTemplateNotFound), culture, arg0);

    public static string WorkItemTemplateTypeNameNullOrEmpty() => ServerResources.Get(nameof (WorkItemTemplateTypeNameNullOrEmpty));

    public static string WorkItemTemplateTypeNameNullOrEmpty(CultureInfo culture) => ServerResources.Get(nameof (WorkItemTemplateTypeNameNullOrEmpty), culture);

    public static string WorkItemTemplateWorkItemTypeNameTooLong(object arg0) => ServerResources.Format(nameof (WorkItemTemplateWorkItemTypeNameTooLong), arg0);

    public static string WorkItemTemplateWorkItemTypeNameTooLong(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemTemplateWorkItemTypeNameTooLong), culture, arg0);

    public static string BehaviorDoesNotExist(object arg0, object arg1) => ServerResources.Format(nameof (BehaviorDoesNotExist), arg0, arg1);

    public static string BehaviorDoesNotExist(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (BehaviorDoesNotExist), culture, arg0, arg1);

    public static string BehaviorNotReferencedByWorkItemType(object arg0, object arg1) => ServerResources.Format(nameof (BehaviorNotReferencedByWorkItemType), arg0, arg1);

    public static string BehaviorNotReferencedByWorkItemType(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (BehaviorNotReferencedByWorkItemType), culture, arg0, arg1);
    }

    public static string BehaviorReferenceAlreadyExists(object arg0, object arg1) => ServerResources.Format(nameof (BehaviorReferenceAlreadyExists), arg0, arg1);

    public static string BehaviorReferenceAlreadyExists(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (BehaviorReferenceAlreadyExists), culture, arg0, arg1);
    }

    public static string CannotReferenceAbstractBehavior(object arg0) => ServerResources.Format(nameof (CannotReferenceAbstractBehavior), arg0);

    public static string CannotReferenceAbstractBehavior(object arg0, CultureInfo culture) => ServerResources.Format(nameof (CannotReferenceAbstractBehavior), culture, arg0);

    public static string CannotReferenceBehaviorFromSystemWIT(object arg0) => ServerResources.Format(nameof (CannotReferenceBehaviorFromSystemWIT), arg0);

    public static string CannotReferenceBehaviorFromSystemWIT(object arg0, CultureInfo culture) => ServerResources.Format(nameof (CannotReferenceBehaviorFromSystemWIT), culture, arg0);

    public static string MultipleBehaviorReferencesNotAllowed(object arg0) => ServerResources.Format(nameof (MultipleBehaviorReferencesNotAllowed), arg0);

    public static string MultipleBehaviorReferencesNotAllowed(object arg0, CultureInfo culture) => ServerResources.Format(nameof (MultipleBehaviorReferencesNotAllowed), culture, arg0);

    public static string WorkItemTemplateIdNotNull() => ServerResources.Get(nameof (WorkItemTemplateIdNotNull));

    public static string WorkItemTemplateIdNotNull(CultureInfo culture) => ServerResources.Get(nameof (WorkItemTemplateIdNotNull), culture);

    public static string WorkItemTemplateUnknownUser() => ServerResources.Get(nameof (WorkItemTemplateUnknownUser));

    public static string WorkItemTemplateUnknownUser(CultureInfo culture) => ServerResources.Get(nameof (WorkItemTemplateUnknownUser), culture);

    public static string InvalidProjectMigrationDueToCustomTypes(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServerResources.Format(nameof (InvalidProjectMigrationDueToCustomTypes), arg0, arg1, arg2);
    }

    public static string InvalidProjectMigrationDueToCustomTypes(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (InvalidProjectMigrationDueToCustomTypes), culture, arg0, arg1, arg2);
    }

    public static string WorkItemTemplateUnknownPath() => ServerResources.Get(nameof (WorkItemTemplateUnknownPath));

    public static string WorkItemTemplateUnknownPath(CultureInfo culture) => ServerResources.Get(nameof (WorkItemTemplateUnknownPath), culture);

    public static string WorkItemTrackingCacheRefreshWaitTooLong(object arg0, object arg1) => ServerResources.Format(nameof (WorkItemTrackingCacheRefreshWaitTooLong), arg0, arg1);

    public static string WorkItemTrackingCacheRefreshWaitTooLong(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemTrackingCacheRefreshWaitTooLong), culture, arg0, arg1);
    }

    public static string ProcessWorkItemTypeInvalidPropertyUpdate(object arg0) => ServerResources.Format(nameof (ProcessWorkItemTypeInvalidPropertyUpdate), arg0);

    public static string ProcessWorkItemTypeInvalidPropertyUpdate(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ProcessWorkItemTypeInvalidPropertyUpdate), culture, arg0);

    public static string WorkItemTargetTypeIsDisabled(object arg0) => ServerResources.Format(nameof (WorkItemTargetTypeIsDisabled), arg0);

    public static string WorkItemTargetTypeIsDisabled(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemTargetTypeIsDisabled), culture, arg0);

    public static string WorkItemStringInvalidException() => ServerResources.Get(nameof (WorkItemStringInvalidException));

    public static string WorkItemStringInvalidException(CultureInfo culture) => ServerResources.Get(nameof (WorkItemStringInvalidException), culture);

    public static string ObjectDoesNotExistOrAccessIsDenied() => ServerResources.Get(nameof (ObjectDoesNotExistOrAccessIsDenied));

    public static string ObjectDoesNotExistOrAccessIsDenied(CultureInfo culture) => ServerResources.Get(nameof (ObjectDoesNotExistOrAccessIsDenied), culture);

    public static string WorkItemStateNotFound() => ServerResources.Get(nameof (WorkItemStateNotFound));

    public static string WorkItemStateNotFound(CultureInfo culture) => ServerResources.Get(nameof (WorkItemStateNotFound), culture);

    public static string StateCommitted() => ServerResources.Get(nameof (StateCommitted));

    public static string StateCommitted(CultureInfo culture) => ServerResources.Get(nameof (StateCommitted), culture);

    public static string StateDone() => ServerResources.Get(nameof (StateDone));

    public static string StateDone(CultureInfo culture) => ServerResources.Get(nameof (StateDone), culture);

    public static string StateProposed() => ServerResources.Get(nameof (StateProposed));

    public static string StateProposed(CultureInfo culture) => ServerResources.Get(nameof (StateProposed), culture);

    public static string ContributionControlCannotHaveControlTypeException() => ServerResources.Get(nameof (ContributionControlCannotHaveControlTypeException));

    public static string ContributionControlCannotHaveControlTypeException(CultureInfo culture) => ServerResources.Get(nameof (ContributionControlCannotHaveControlTypeException), culture);

    public static string FormLayoutControlInputsExceededException(object arg0, object arg1) => ServerResources.Format(nameof (FormLayoutControlInputsExceededException), arg0, arg1);

    public static string FormLayoutControlInputsExceededException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (FormLayoutControlInputsExceededException), culture, arg0, arg1);
    }

    public static string DefaultRuleInvalidAllowedValue(object arg0, object arg1, object arg2) => ServerResources.Format(nameof (DefaultRuleInvalidAllowedValue), arg0, arg1, arg2);

    public static string DefaultRuleInvalidAllowedValue(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (DefaultRuleInvalidAllowedValue), culture, arg0, arg1, arg2);
    }

    public static string GenericPickListNameAlreadyInUse() => ServerResources.Get(nameof (GenericPickListNameAlreadyInUse));

    public static string GenericPickListNameAlreadyInUse(CultureInfo culture) => ServerResources.Get(nameof (GenericPickListNameAlreadyInUse), culture);

    public static string DataType() => ServerResources.Get(nameof (DataType));

    public static string DataType(CultureInfo culture) => ServerResources.Get(nameof (DataType), culture);

    public static string FormLayoutAddContributedGroupException(object arg0) => ServerResources.Format(nameof (FormLayoutAddContributedGroupException), arg0);

    public static string FormLayoutAddContributedGroupException(object arg0, CultureInfo culture) => ServerResources.Format(nameof (FormLayoutAddContributedGroupException), culture, arg0);

    public static string FormLayoutAddContributedPageException(object arg0) => ServerResources.Format(nameof (FormLayoutAddContributedPageException), arg0);

    public static string FormLayoutAddContributedPageException(object arg0, CultureInfo culture) => ServerResources.Format(nameof (FormLayoutAddContributedPageException), culture, arg0);

    public static string FormLayoutAddControlToContributedGroupException() => ServerResources.Get(nameof (FormLayoutAddControlToContributedGroupException));

    public static string FormLayoutAddControlToContributedGroupException(CultureInfo culture) => ServerResources.Get(nameof (FormLayoutAddControlToContributedGroupException), culture);

    public static string FormLayoutAddGroupToContributedPageException(object arg0, object arg1) => ServerResources.Format(nameof (FormLayoutAddGroupToContributedPageException), arg0, arg1);

    public static string FormLayoutAddGroupToContributedPageException(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (FormLayoutAddGroupToContributedPageException), culture, arg0, arg1);
    }

    public static string FormLayoutRemoveContributedGroupException() => ServerResources.Get(nameof (FormLayoutRemoveContributedGroupException));

    public static string FormLayoutRemoveContributedGroupException(CultureInfo culture) => ServerResources.Get(nameof (FormLayoutRemoveContributedGroupException), culture);

    public static string FormLayoutRemoveContributedPageException(object arg0) => ServerResources.Format(nameof (FormLayoutRemoveContributedPageException), arg0);

    public static string FormLayoutRemoveContributedPageException(object arg0, CultureInfo culture) => ServerResources.Format(nameof (FormLayoutRemoveContributedPageException), culture, arg0);

    public static string CannotDisableWorkItemType(object arg0, object arg1) => ServerResources.Format(nameof (CannotDisableWorkItemType), arg0, arg1);

    public static string CannotDisableWorkItemType(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (CannotDisableWorkItemType), culture, arg0, arg1);

    public static string CannotDisableTestWorkItemTypeReason() => ServerResources.Get(nameof (CannotDisableTestWorkItemTypeReason));

    public static string CannotDisableTestWorkItemTypeReason(CultureInfo culture) => ServerResources.Get(nameof (CannotDisableTestWorkItemTypeReason), culture);

    public static string FieldType() => ServerResources.Get(nameof (FieldType));

    public static string FieldType(CultureInfo culture) => ServerResources.Get(nameof (FieldType), culture);

    public static string PicklistPermissionException() => ServerResources.Get(nameof (PicklistPermissionException));

    public static string PicklistPermissionException(CultureInfo culture) => ServerResources.Get(nameof (PicklistPermissionException), culture);

    public static string ControlContributionNotFoundException(object arg0) => ServerResources.Format(nameof (ControlContributionNotFoundException), arg0);

    public static string ControlContributionNotFoundException(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ControlContributionNotFoundException), culture, arg0);

    public static string ValidatorNoUsageOfForOrNotAttributesInCopyDefaultRules() => ServerResources.Get(nameof (ValidatorNoUsageOfForOrNotAttributesInCopyDefaultRules));

    public static string ValidatorNoUsageOfForOrNotAttributesInCopyDefaultRules(CultureInfo culture) => ServerResources.Get(nameof (ValidatorNoUsageOfForOrNotAttributesInCopyDefaultRules), culture);

    public static string BehaviorNameInUse() => ServerResources.Get(nameof (BehaviorNameInUse));

    public static string BehaviorNameInUse(CultureInfo culture) => ServerResources.Get(nameof (BehaviorNameInUse), culture);

    public static string PortfolioBehaviorLimitExceeded(object arg0) => ServerResources.Format(nameof (PortfolioBehaviorLimitExceeded), arg0);

    public static string PortfolioBehaviorLimitExceeded(object arg0, CultureInfo culture) => ServerResources.Format(nameof (PortfolioBehaviorLimitExceeded), culture, arg0);

    public static string ValidatorWarningUsingForNotAttributes() => ServerResources.Get(nameof (ValidatorWarningUsingForNotAttributes));

    public static string ValidatorWarningUsingForNotAttributes(CultureInfo culture) => ServerResources.Get(nameof (ValidatorWarningUsingForNotAttributes), culture);

    public static string ResolveEntitiesError(object arg0) => ServerResources.Format(nameof (ResolveEntitiesError), arg0);

    public static string ResolveEntitiesError(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ResolveEntitiesError), culture, arg0);

    public static string BehaviorCreationInvalidParent(object arg0, object arg1) => ServerResources.Format(nameof (BehaviorCreationInvalidParent), arg0, arg1);

    public static string BehaviorCreationInvalidParent(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (BehaviorCreationInvalidParent), culture, arg0, arg1);
    }

    public static string CacheMustBeCalledWithSystemDescriptor(object arg0) => ServerResources.Format(nameof (CacheMustBeCalledWithSystemDescriptor), arg0);

    public static string CacheMustBeCalledWithSystemDescriptor(object arg0, CultureInfo culture) => ServerResources.Format(nameof (CacheMustBeCalledWithSystemDescriptor), culture, arg0);

    public static string ComposedWorkItemTypeFieldIdIsNotPresent(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServerResources.Format(nameof (ComposedWorkItemTypeFieldIdIsNotPresent), arg0, arg1, arg2);
    }

    public static string ComposedWorkItemTypeFieldIdIsNotPresent(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ComposedWorkItemTypeFieldIdIsNotPresent), culture, arg0, arg1, arg2);
    }

    public static string BehaviorReferenceNotExists(object arg0, object arg1) => ServerResources.Format(nameof (BehaviorReferenceNotExists), arg0, arg1);

    public static string BehaviorReferenceNotExists(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (BehaviorReferenceNotExists), culture, arg0, arg1);

    public static string WorkItemLinksLimitExceeded(object arg0) => ServerResources.Format(nameof (WorkItemLinksLimitExceeded), arg0);

    public static string WorkItemLinksLimitExceeded(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemLinksLimitExceeded), culture, arg0);

    public static string WorkItemRemoteLinksLimitExceeded(object arg0) => ServerResources.Format(nameof (WorkItemRemoteLinksLimitExceeded), arg0);

    public static string WorkItemRemoteLinksLimitExceeded(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemRemoteLinksLimitExceeded), culture, arg0);

    public static string NotInScopeAadIdentity(object arg0, object arg1) => ServerResources.Format(nameof (NotInScopeAadIdentity), arg0, arg1);

    public static string NotInScopeAadIdentity(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (NotInScopeAadIdentity), culture, arg0, arg1);

    public static string DeleteFieldPermissionError(object arg0) => ServerResources.Format(nameof (DeleteFieldPermissionError), arg0);

    public static string DeleteFieldPermissionError(object arg0, CultureInfo culture) => ServerResources.Format(nameof (DeleteFieldPermissionError), culture, arg0);

    public static string ValueContainsInvalidHTML(object arg0) => ServerResources.Format(nameof (ValueContainsInvalidHTML), arg0);

    public static string ValueContainsInvalidHTML(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ValueContainsInvalidHTML), culture, arg0);

    public static string RulesNotFoundForWorkItemType(object arg0, object arg1) => ServerResources.Format(nameof (RulesNotFoundForWorkItemType), arg0, arg1);

    public static string RulesNotFoundForWorkItemType(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (RulesNotFoundForWorkItemType), culture, arg0, arg1);
    }

    public static string CannotDeleteOOBField(object arg0) => ServerResources.Format(nameof (CannotDeleteOOBField), arg0);

    public static string CannotDeleteOOBField(object arg0, CultureInfo culture) => ServerResources.Format(nameof (CannotDeleteOOBField), culture, arg0);

    public static string DefaultIdNotFound() => ServerResources.Get(nameof (DefaultIdNotFound));

    public static string DefaultIdNotFound(CultureInfo culture) => ServerResources.Get(nameof (DefaultIdNotFound), culture);

    public static string DefaultValueCannotBeGroup() => ServerResources.Get(nameof (DefaultValueCannotBeGroup));

    public static string DefaultValueCannotBeGroup(CultureInfo culture) => ServerResources.Get(nameof (DefaultValueCannotBeGroup), culture);

    public static string AddingRulesNotSupportedOnField(object arg0) => ServerResources.Format(nameof (AddingRulesNotSupportedOnField), arg0);

    public static string AddingRulesNotSupportedOnField(object arg0, CultureInfo culture) => ServerResources.Format(nameof (AddingRulesNotSupportedOnField), culture, arg0);

    public static string DefaultValueNotEditableOnField(object arg0) => ServerResources.Format(nameof (DefaultValueNotEditableOnField), arg0);

    public static string DefaultValueNotEditableOnField(object arg0, CultureInfo culture) => ServerResources.Format(nameof (DefaultValueNotEditableOnField), culture, arg0);

    public static string EditingOutOfBoxFieldIsNotSupported(object arg0) => ServerResources.Format(nameof (EditingOutOfBoxFieldIsNotSupported), arg0);

    public static string EditingOutOfBoxFieldIsNotSupported(object arg0, CultureInfo culture) => ServerResources.Format(nameof (EditingOutOfBoxFieldIsNotSupported), culture, arg0);

    public static string ReadOnlyUnsupportedForField(object arg0) => ServerResources.Format(nameof (ReadOnlyUnsupportedForField), arg0);

    public static string ReadOnlyUnsupportedForField(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ReadOnlyUnsupportedForField), culture, arg0);

    public static string RequiredNotEditableOnField(object arg0) => ServerResources.Format(nameof (RequiredNotEditableOnField), arg0);

    public static string RequiredNotEditableOnField(object arg0, CultureInfo culture) => ServerResources.Format(nameof (RequiredNotEditableOnField), culture, arg0);

    public static string QueryMaxWiqlTextLengthLimitExceeded(object arg0) => ServerResources.Format(nameof (QueryMaxWiqlTextLengthLimitExceeded), arg0);

    public static string QueryMaxWiqlTextLengthLimitExceeded(object arg0, CultureInfo culture) => ServerResources.Format(nameof (QueryMaxWiqlTextLengthLimitExceeded), culture, arg0);

    public static string OnlyRulesOrHelpTextsAllowed() => ServerResources.Get(nameof (OnlyRulesOrHelpTextsAllowed));

    public static string OnlyRulesOrHelpTextsAllowed(CultureInfo culture) => ServerResources.Get(nameof (OnlyRulesOrHelpTextsAllowed), culture);

    public static string WorkItemMaxAllowedAttachmentsLimitExceeded(object arg0) => ServerResources.Format(nameof (WorkItemMaxAllowedAttachmentsLimitExceeded), arg0);

    public static string WorkItemMaxAllowedAttachmentsLimitExceeded(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemMaxAllowedAttachmentsLimitExceeded), culture, arg0);
    }

    public static string TypeFilteringNotSupportedForDiscussions() => ServerResources.Get(nameof (TypeFilteringNotSupportedForDiscussions));

    public static string TypeFilteringNotSupportedForDiscussions(CultureInfo culture) => ServerResources.Get(nameof (TypeFilteringNotSupportedForDiscussions), culture);

    public static string CannotSetAllowGroupsOnOOBField(object arg0) => ServerResources.Format(nameof (CannotSetAllowGroupsOnOOBField), arg0);

    public static string CannotSetAllowGroupsOnOOBField(object arg0, CultureInfo culture) => ServerResources.Format(nameof (CannotSetAllowGroupsOnOOBField), culture, arg0);

    public static string LegacyProcessUpdateBlocked() => ServerResources.Get(nameof (LegacyProcessUpdateBlocked));

    public static string LegacyProcessUpdateBlocked(CultureInfo culture) => ServerResources.Get(nameof (LegacyProcessUpdateBlocked), culture);

    public static string WorkItemResourceLinkLocationUnsafeUrl(object arg0, object arg1) => ServerResources.Format(nameof (WorkItemResourceLinkLocationUnsafeUrl), arg0, arg1);

    public static string WorkItemResourceLinkLocationUnsafeUrl(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemResourceLinkLocationUnsafeUrl), culture, arg0, arg1);
    }

    public static string AllowedValuesNotEditableOnField(object arg0) => ServerResources.Format(nameof (AllowedValuesNotEditableOnField), arg0);

    public static string AllowedValuesNotEditableOnField(object arg0, CultureInfo culture) => ServerResources.Format(nameof (AllowedValuesNotEditableOnField), culture, arg0);

    public static string AllowExistingValueNotEditableOnField(object arg0) => ServerResources.Format(nameof (AllowExistingValueNotEditableOnField), arg0);

    public static string AllowExistingValueNotEditableOnField(object arg0, CultureInfo culture) => ServerResources.Format(nameof (AllowExistingValueNotEditableOnField), culture, arg0);

    public static string UnrecognizedResourceLink(object arg0, object arg1) => ServerResources.Format(nameof (UnrecognizedResourceLink), arg0, arg1);

    public static string UnrecognizedResourceLink(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (UnrecognizedResourceLink), culture, arg0, arg1);

    public static string DefaultRuleInvalidIdentityAllowedValue(object arg0, object arg1) => ServerResources.Format(nameof (DefaultRuleInvalidIdentityAllowedValue), arg0, arg1);

    public static string DefaultRuleInvalidIdentityAllowedValue(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (DefaultRuleInvalidIdentityAllowedValue), culture, arg0, arg1);
    }

    public static string IsEmptyDisabled() => ServerResources.Get(nameof (IsEmptyDisabled));

    public static string IsEmptyDisabled(CultureInfo culture) => ServerResources.Get(nameof (IsEmptyDisabled), culture);

    public static string Validation_ProcessProperty_KeyValueDelimeter_NotFound(object arg0) => ServerResources.Format(nameof (Validation_ProcessProperty_KeyValueDelimeter_NotFound), arg0);

    public static string Validation_ProcessProperty_KeyValueDelimeter_NotFound(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (Validation_ProcessProperty_KeyValueDelimeter_NotFound), culture, arg0);
    }

    public static string Validation_StateColors_StateOrColor_Invalid(object arg0) => ServerResources.Format(nameof (Validation_StateColors_StateOrColor_Invalid), arg0);

    public static string Validation_StateColors_StateOrColor_Invalid(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (Validation_StateColors_StateOrColor_Invalid), culture, arg0);
    }

    public static string Validation_StateColors_State_Duplicate(object arg0) => ServerResources.Format(nameof (Validation_StateColors_State_Duplicate), arg0);

    public static string Validation_StateColors_State_Duplicate(object arg0, CultureInfo culture) => ServerResources.Format(nameof (Validation_StateColors_State_Duplicate), culture, arg0);

    public static string Validation_WorkItemTypeIcons_Icon_Invalid(object arg0) => ServerResources.Format(nameof (Validation_WorkItemTypeIcons_Icon_Invalid), arg0);

    public static string Validation_WorkItemTypeIcons_Icon_Invalid(object arg0, CultureInfo culture) => ServerResources.Format(nameof (Validation_WorkItemTypeIcons_Icon_Invalid), culture, arg0);

    public static string Validation_WorkItemTypeIcons_Invalid_With_Message(object arg0, object arg1) => ServerResources.Format(nameof (Validation_WorkItemTypeIcons_Invalid_With_Message), arg0, arg1);

    public static string Validation_WorkItemTypeIcons_Invalid_With_Message(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (Validation_WorkItemTypeIcons_Invalid_With_Message), culture, arg0, arg1);
    }

    public static string Validation_WorkItemTypeIcons_Type_Duplicate(object arg0) => ServerResources.Format(nameof (Validation_WorkItemTypeIcons_Type_Duplicate), arg0);

    public static string Validation_WorkItemTypeIcons_Type_Duplicate(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (Validation_WorkItemTypeIcons_Type_Duplicate), culture, arg0);
    }

    public static string Validation_WorkItemTypeIcons_Type_Invalid(object arg0) => ServerResources.Format(nameof (Validation_WorkItemTypeIcons_Type_Invalid), arg0);

    public static string Validation_WorkItemTypeIcons_Type_Invalid(object arg0, CultureInfo culture) => ServerResources.Format(nameof (Validation_WorkItemTypeIcons_Type_Invalid), culture, arg0);

    public static string DeleteCollectionLevelFieldPermissionError(object arg0) => ServerResources.Format(nameof (DeleteCollectionLevelFieldPermissionError), arg0);

    public static string DeleteCollectionLevelFieldPermissionError(object arg0, CultureInfo culture) => ServerResources.Format(nameof (DeleteCollectionLevelFieldPermissionError), culture, arg0);

    public static string CannotConvertFieldTypeToPicklist(object arg0) => ServerResources.Format(nameof (CannotConvertFieldTypeToPicklist), arg0);

    public static string CannotConvertFieldTypeToPicklist(object arg0, CultureInfo culture) => ServerResources.Format(nameof (CannotConvertFieldTypeToPicklist), culture, arg0);

    public static string ValidatorInvalidNumberOfAgileCommonConfigurationDocuments() => ServerResources.Get(nameof (ValidatorInvalidNumberOfAgileCommonConfigurationDocuments));

    public static string ValidatorInvalidNumberOfAgileCommonConfigurationDocuments(
      CultureInfo culture)
    {
      return ServerResources.Get(nameof (ValidatorInvalidNumberOfAgileCommonConfigurationDocuments), culture);
    }

    public static string RuleValidationErrors_MultipleAdditional(object arg0, object arg1) => ServerResources.Format(nameof (RuleValidationErrors_MultipleAdditional), arg0, arg1);

    public static string RuleValidationErrors_MultipleAdditional(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (RuleValidationErrors_MultipleAdditional), culture, arg0, arg1);
    }

    public static string RuleValidationErrors_OneAdditional(object arg0) => ServerResources.Format(nameof (RuleValidationErrors_OneAdditional), arg0);

    public static string RuleValidationErrors_OneAdditional(object arg0, CultureInfo culture) => ServerResources.Format(nameof (RuleValidationErrors_OneAdditional), culture, arg0);

    public static string WorkItemHasNoValidTransitionXml(object arg0, object arg1, object arg2) => ServerResources.Format(nameof (WorkItemHasNoValidTransitionXml), arg0, arg1, arg2);

    public static string WorkItemHasNoValidTransitionXml(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemHasNoValidTransitionXml), culture, arg0, arg1, arg2);
    }

    public static string WorkItemNoAutomaticTransition(object arg0, object arg1, object arg2) => ServerResources.Format(nameof (WorkItemNoAutomaticTransition), arg0, arg1, arg2);

    public static string WorkItemNoAutomaticTransition(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemNoAutomaticTransition), culture, arg0, arg1, arg2);
    }

    public static string WorkItemRulePreventingTransition(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ServerResources.Format(nameof (WorkItemRulePreventingTransition), arg0, arg1, arg2, arg3);
    }

    public static string WorkItemRulePreventingTransition(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemRulePreventingTransition), culture, arg0, arg1, arg2, arg3);
    }

    public static string BehaviorReferenceNameInUse() => ServerResources.Get(nameof (BehaviorReferenceNameInUse));

    public static string BehaviorReferenceNameInUse(CultureInfo culture) => ServerResources.Get(nameof (BehaviorReferenceNameInUse), culture);

    public static string InvalidTeamFieldValue(object arg0, object arg1) => ServerResources.Format(nameof (InvalidTeamFieldValue), arg0, arg1);

    public static string InvalidTeamFieldValue(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (InvalidTeamFieldValue), culture, arg0, arg1);

    public static string ValidatorImproperDefinitionOfProtectedFieldOrFieldInDifferentLocale(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4)
    {
      return ServerResources.Format(nameof (ValidatorImproperDefinitionOfProtectedFieldOrFieldInDifferentLocale), arg0, arg1, arg2, arg3, arg4);
    }

    public static string ValidatorImproperDefinitionOfProtectedFieldOrFieldInDifferentLocale(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ValidatorImproperDefinitionOfProtectedFieldOrFieldInDifferentLocale), culture, arg0, arg1, arg2, arg3, arg4);
    }

    public static string UnauthorizedSuppressNotifications() => ServerResources.Get(nameof (UnauthorizedSuppressNotifications));

    public static string UnauthorizedSuppressNotifications(CultureInfo culture) => ServerResources.Get(nameof (UnauthorizedSuppressNotifications), culture);

    public static string WorkItemIdentityNotResovledById(object arg0) => ServerResources.Format(nameof (WorkItemIdentityNotResovledById), arg0);

    public static string WorkItemIdentityNotResovledById(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemIdentityNotResovledById), culture, arg0);

    public static string WorkItemUnauthorizedProjectAttachment(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServerResources.Format(nameof (WorkItemUnauthorizedProjectAttachment), arg0, arg1, arg2);
    }

    public static string WorkItemUnauthorizedProjectAttachment(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemUnauthorizedProjectAttachment), culture, arg0, arg1, arg2);
    }

    public static string CannotFindTokenForWorkItem(object arg0) => ServerResources.Format(nameof (CannotFindTokenForWorkItem), arg0);

    public static string CannotFindTokenForWorkItem(object arg0, CultureInfo culture) => ServerResources.Format(nameof (CannotFindTokenForWorkItem), culture, arg0);

    public static string WorkItemComponentFieldValueEmpty(object arg0, object arg1, object arg2) => ServerResources.Format(nameof (WorkItemComponentFieldValueEmpty), arg0, arg1, arg2);

    public static string WorkItemComponentFieldValueEmpty(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemComponentFieldValueEmpty), culture, arg0, arg1, arg2);
    }

    public static string WorkItemTypeCategoriesNotFound(object arg0) => ServerResources.Format(nameof (WorkItemTypeCategoriesNotFound), arg0);

    public static string WorkItemTypeCategoriesNotFound(object arg0, CultureInfo culture) => ServerResources.Format(nameof (WorkItemTypeCategoriesNotFound), culture, arg0);

    public static string ReadAllowedValuesNotAuthorizedError(object arg0) => ServerResources.Format(nameof (ReadAllowedValuesNotAuthorizedError), arg0);

    public static string ReadAllowedValuesNotAuthorizedError(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ReadAllowedValuesNotAuthorizedError), culture, arg0);

    public static string ReadDependentFieldsNotAuthorizedError(object arg0) => ServerResources.Format(nameof (ReadDependentFieldsNotAuthorizedError), arg0);

    public static string ReadDependentFieldsNotAuthorizedError(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ReadDependentFieldsNotAuthorizedError), culture, arg0);

    public static string ReadRulesPermissionErrorMessage(object arg0, object arg1) => ServerResources.Format(nameof (ReadRulesPermissionErrorMessage), arg0, arg1);

    public static string ReadRulesPermissionErrorMessage(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ReadRulesPermissionErrorMessage), culture, arg0, arg1);
    }

    public static string ProcessWorkItemTypeNotFound(object arg0) => ServerResources.Format(nameof (ProcessWorkItemTypeNotFound), arg0);

    public static string ProcessWorkItemTypeNotFound(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ProcessWorkItemTypeNotFound), culture, arg0);

    public static string ProjectWorkItemTypeNotFound(object arg0) => ServerResources.Format(nameof (ProjectWorkItemTypeNotFound), arg0);

    public static string ProjectWorkItemTypeNotFound(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ProjectWorkItemTypeNotFound), culture, arg0);

    public static string WorkItemUnauthorizedHistoricalDataAccessException() => ServerResources.Get(nameof (WorkItemUnauthorizedHistoricalDataAccessException));

    public static string WorkItemUnauthorizedHistoricalDataAccessException(CultureInfo culture) => ServerResources.Get(nameof (WorkItemUnauthorizedHistoricalDataAccessException), culture);

    public static string QueryNotFoundUnderProject(object arg0) => ServerResources.Format(nameof (QueryNotFoundUnderProject), arg0);

    public static string QueryNotFoundUnderProject(object arg0, CultureInfo culture) => ServerResources.Format(nameof (QueryNotFoundUnderProject), culture, arg0);

    public static string WorkItemTypeFieldUsedByProject() => ServerResources.Get(nameof (WorkItemTypeFieldUsedByProject));

    public static string WorkItemTypeFieldUsedByProject(CultureInfo culture) => ServerResources.Get(nameof (WorkItemTypeFieldUsedByProject), culture);

    public static string WorkItemRulesDoNotSupportEmbeddedImages() => ServerResources.Get(nameof (WorkItemRulesDoNotSupportEmbeddedImages));

    public static string WorkItemRulesDoNotSupportEmbeddedImages(CultureInfo culture) => ServerResources.Get(nameof (WorkItemRulesDoNotSupportEmbeddedImages), culture);

    public static string WorkItemTrackingMetadataError(object arg0, object arg1) => ServerResources.Format(nameof (WorkItemTrackingMetadataError), arg0, arg1);

    public static string WorkItemTrackingMetadataError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemTrackingMetadataError), culture, arg0, arg1);
    }

    public static string CannotResolveIdentityByDescriptor(object arg0) => ServerResources.Format(nameof (CannotResolveIdentityByDescriptor), arg0);

    public static string CannotResolveIdentityByDescriptor(object arg0, CultureInfo culture) => ServerResources.Format(nameof (CannotResolveIdentityByDescriptor), culture, arg0);

    public static string CannotResolveIdentityByTfId(object arg0) => ServerResources.Format(nameof (CannotResolveIdentityByTfId), arg0);

    public static string CannotResolveIdentityByTfId(object arg0, CultureInfo culture) => ServerResources.Format(nameof (CannotResolveIdentityByTfId), culture, arg0);

    public static string QueryExpressionInvalidPredicateIsEmpty(object arg0) => ServerResources.Format(nameof (QueryExpressionInvalidPredicateIsEmpty), arg0);

    public static string QueryExpressionInvalidPredicateIsEmpty(object arg0, CultureInfo culture) => ServerResources.Format(nameof (QueryExpressionInvalidPredicateIsEmpty), culture, arg0);

    public static string WorkitemTrackingSoapAccessBlocked() => ServerResources.Get(nameof (WorkitemTrackingSoapAccessBlocked));

    public static string WorkitemTrackingSoapAccessBlocked(CultureInfo culture) => ServerResources.Get(nameof (WorkitemTrackingSoapAccessBlocked), culture);

    public static string WorkItemTrackingPicklistConversionFailedBadValues(object arg0) => ServerResources.Format(nameof (WorkItemTrackingPicklistConversionFailedBadValues), arg0);

    public static string WorkItemTrackingPicklistConversionFailedBadValues(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (WorkItemTrackingPicklistConversionFailedBadValues), culture, arg0);
    }

    public static string SystemCategoryRefnameHasBeenChangedInXmlProcess(object arg0) => ServerResources.Format(nameof (SystemCategoryRefnameHasBeenChangedInXmlProcess), arg0);

    public static string SystemCategoryRefnameHasBeenChangedInXmlProcess(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (SystemCategoryRefnameHasBeenChangedInXmlProcess), culture, arg0);
    }

    public static string XmlToInheritedFieldDefinitionNotFoundExceptionMessage(object arg0) => ServerResources.Format(nameof (XmlToInheritedFieldDefinitionNotFoundExceptionMessage), arg0);

    public static string XmlToInheritedFieldDefinitionNotFoundExceptionMessage(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (XmlToInheritedFieldDefinitionNotFoundExceptionMessage), culture, arg0);
    }

    public static string DuplicatedWorkItemType(object arg0) => ServerResources.Format(nameof (DuplicatedWorkItemType), arg0);

    public static string DuplicatedWorkItemType(object arg0, CultureInfo culture) => ServerResources.Format(nameof (DuplicatedWorkItemType), culture, arg0);

    public static string Field() => ServerResources.Get(nameof (Field));

    public static string Field(CultureInfo culture) => ServerResources.Get(nameof (Field), culture);

    public static string WorkItemType() => ServerResources.Get(nameof (WorkItemType));

    public static string WorkItemType(CultureInfo culture) => ServerResources.Get(nameof (WorkItemType), culture);

    public static string ServiceEndpointNotFound(object arg0) => ServerResources.Format(nameof (ServiceEndpointNotFound), arg0);

    public static string ServiceEndpointNotFound(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ServiceEndpointNotFound), culture, arg0);

    public static string InvalidPayload() => ServerResources.Get(nameof (InvalidPayload));

    public static string InvalidPayload(CultureInfo culture) => ServerResources.Get(nameof (InvalidPayload), culture);

    public static string UnsupportedExternalEventSource() => ServerResources.Get(nameof (UnsupportedExternalEventSource));

    public static string UnsupportedExternalEventSource(CultureInfo culture) => ServerResources.Get(nameof (UnsupportedExternalEventSource), culture);

    public static string UnsupportedExternalEventHostType() => ServerResources.Get(nameof (UnsupportedExternalEventHostType));

    public static string UnsupportedExternalEventHostType(CultureInfo culture) => ServerResources.Get(nameof (UnsupportedExternalEventHostType), culture);

    public static string InvalidProviderKey(object arg0) => ServerResources.Format(nameof (InvalidProviderKey), arg0);

    public static string InvalidProviderKey(object arg0, CultureInfo culture) => ServerResources.Format(nameof (InvalidProviderKey), culture, arg0);

    public static string RepoNotConfiguredForGitHubEvents(object arg0) => ServerResources.Format(nameof (RepoNotConfiguredForGitHubEvents), arg0);

    public static string RepoNotConfiguredForGitHubEvents(object arg0, CultureInfo culture) => ServerResources.Format(nameof (RepoNotConfiguredForGitHubEvents), culture, arg0);

    public static string UnsupportedGitHubEventType(object arg0) => ServerResources.Format(nameof (UnsupportedGitHubEventType), arg0);

    public static string UnsupportedGitHubEventType(object arg0, CultureInfo culture) => ServerResources.Format(nameof (UnsupportedGitHubEventType), culture, arg0);

    public static string NoGitHubReposConfiguredForProject(object arg0) => ServerResources.Format(nameof (NoGitHubReposConfiguredForProject), arg0);

    public static string NoGitHubReposConfiguredForProject(object arg0, CultureInfo culture) => ServerResources.Format(nameof (NoGitHubReposConfiguredForProject), culture, arg0);

    public static string GitHubRepoNotConfiguredForProjectEvents(object arg0, object arg1) => ServerResources.Format(nameof (GitHubRepoNotConfiguredForProjectEvents), arg0, arg1);

    public static string GitHubRepoNotConfiguredForProjectEvents(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (GitHubRepoNotConfiguredForProjectEvents), culture, arg0, arg1);
    }

    public static string GitHubBoardsConnectionAlreadyExist(object arg0) => ServerResources.Format(nameof (GitHubBoardsConnectionAlreadyExist), arg0);

    public static string GitHubBoardsConnectionAlreadyExist(object arg0, CultureInfo culture) => ServerResources.Format(nameof (GitHubBoardsConnectionAlreadyExist), culture, arg0);

    public static string GitHubBoardsRepositoryAlreadyRegistered(object arg0) => ServerResources.Format(nameof (GitHubBoardsRepositoryAlreadyRegistered), arg0);

    public static string GitHubBoardsRepositoryAlreadyRegistered(object arg0, CultureInfo culture) => ServerResources.Format(nameof (GitHubBoardsRepositoryAlreadyRegistered), culture, arg0);

    public static string ProcessOutOfBoxFieldsNotFound(object arg0) => ServerResources.Format(nameof (ProcessOutOfBoxFieldsNotFound), arg0);

    public static string ProcessOutOfBoxFieldsNotFound(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ProcessOutOfBoxFieldsNotFound), culture, arg0);

    public static string GitHubExternalConnectionPermissionException() => ServerResources.Get(nameof (GitHubExternalConnectionPermissionException));

    public static string GitHubExternalConnectionPermissionException(CultureInfo culture) => ServerResources.Get(nameof (GitHubExternalConnectionPermissionException), culture);

    public static string GitHubBoardsConnectionDoesNotExist(object arg0) => ServerResources.Format(nameof (GitHubBoardsConnectionDoesNotExist), arg0);

    public static string GitHubBoardsConnectionDoesNotExist(object arg0, CultureInfo culture) => ServerResources.Format(nameof (GitHubBoardsConnectionDoesNotExist), culture, arg0);

    public static string FailedToDeprovisionRepo(object arg0) => ServerResources.Format(nameof (FailedToDeprovisionRepo), arg0);

    public static string FailedToDeprovisionRepo(object arg0, CultureInfo culture) => ServerResources.Format(nameof (FailedToDeprovisionRepo), culture, arg0);

    public static string UnsupportedGitHubItemType(object arg0) => ServerResources.Format(nameof (UnsupportedGitHubItemType), arg0);

    public static string UnsupportedGitHubItemType(object arg0, CultureInfo culture) => ServerResources.Format(nameof (UnsupportedGitHubItemType), culture, arg0);

    public static string GitHubBoardsMustHaveRepoInConnectionExceptionMessage() => ServerResources.Get(nameof (GitHubBoardsMustHaveRepoInConnectionExceptionMessage));

    public static string GitHubBoardsMustHaveRepoInConnectionExceptionMessage(CultureInfo culture) => ServerResources.Get(nameof (GitHubBoardsMustHaveRepoInConnectionExceptionMessage), culture);

    public static string GitHubConnectionNotFoundMessage() => ServerResources.Get(nameof (GitHubConnectionNotFoundMessage));

    public static string GitHubConnectionNotFoundMessage(CultureInfo culture) => ServerResources.Get(nameof (GitHubConnectionNotFoundMessage), culture);

    public static string GitHubEnterpriseVersionNotSupported(object arg0, object arg1) => ServerResources.Format(nameof (GitHubEnterpriseVersionNotSupported), arg0, arg1);

    public static string GitHubEnterpriseVersionNotSupported(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (GitHubEnterpriseVersionNotSupported), culture, arg0, arg1);
    }

    public static string GitHubEnterpriseVersionMayNotBeSupported(object arg0) => ServerResources.Format(nameof (GitHubEnterpriseVersionMayNotBeSupported), arg0);

    public static string GitHubEnterpriseVersionMayNotBeSupported(object arg0, CultureInfo culture) => ServerResources.Format(nameof (GitHubEnterpriseVersionMayNotBeSupported), culture, arg0);

    public static string CannotMigrateToDisabledProcess() => ServerResources.Get(nameof (CannotMigrateToDisabledProcess));

    public static string CannotMigrateToDisabledProcess(CultureInfo culture) => ServerResources.Get(nameof (CannotMigrateToDisabledProcess), culture);

    public static string InsufficientPermissionToChangeProcessOfProjectExceptionMessage(object arg0) => ServerResources.Format(nameof (InsufficientPermissionToChangeProcessOfProjectExceptionMessage), arg0);

    public static string InsufficientPermissionToChangeProcessOfProjectExceptionMessage(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (InsufficientPermissionToChangeProcessOfProjectExceptionMessage), culture, arg0);
    }

    public static string InvalidProjectProvidedInMigratingProjectList() => ServerResources.Get(nameof (InvalidProjectProvidedInMigratingProjectList));

    public static string InvalidProjectProvidedInMigratingProjectList(CultureInfo culture) => ServerResources.Get(nameof (InvalidProjectProvidedInMigratingProjectList), culture);

    public static string ProcessDefaultException() => ServerResources.Get(nameof (ProcessDefaultException));

    public static string ProcessDefaultException(CultureInfo culture) => ServerResources.Get(nameof (ProcessDefaultException), culture);

    public static string ProcessInvalidParent() => ServerResources.Get(nameof (ProcessInvalidParent));

    public static string ProcessInvalidParent(CultureInfo culture) => ServerResources.Get(nameof (ProcessInvalidParent), culture);

    public static string ProcessInvalidProjectMigration(object arg0, object arg1, object arg2) => ServerResources.Format(nameof (ProcessInvalidProjectMigration), arg0, arg1, arg2);

    public static string ProcessInvalidProjectMigration(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ProcessInvalidProjectMigration), culture, arg0, arg1, arg2);
    }

    public static string ProjectMigrateMissingWits(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ServerResources.Format(nameof (ProjectMigrateMissingWits), arg0, arg1, arg2, arg3);
    }

    public static string ProjectMigrateMissingWits(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ProjectMigrateMissingWits), culture, arg0, arg1, arg2, arg3);
    }

    public static string UnableToUploadMetadataResourceForProcess(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ServerResources.Format(nameof (UnableToUploadMetadataResourceForProcess), arg0, arg1, arg2, arg3);
    }

    public static string UnableToUploadMetadataResourceForProcess(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (UnableToUploadMetadataResourceForProcess), culture, arg0, arg1, arg2, arg3);
    }

    public static string GitHubBoardsExceededReposNumberLimit(object arg0) => ServerResources.Format(nameof (GitHubBoardsExceededReposNumberLimit), arg0);

    public static string GitHubBoardsExceededReposNumberLimit(object arg0, CultureInfo culture) => ServerResources.Format(nameof (GitHubBoardsExceededReposNumberLimit), culture, arg0);

    public static string ExternalConnectionFailedToPopulateMetadata() => ServerResources.Get(nameof (ExternalConnectionFailedToPopulateMetadata));

    public static string ExternalConnectionFailedToPopulateMetadata(CultureInfo culture) => ServerResources.Get(nameof (ExternalConnectionFailedToPopulateMetadata), culture);

    public static string RepoNotUniqueForGitHubEvent(object arg0, object arg1) => ServerResources.Format(nameof (RepoNotUniqueForGitHubEvent), arg0, arg1);

    public static string RepoNotUniqueForGitHubEvent(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (RepoNotUniqueForGitHubEvent), culture, arg0, arg1);

    public static string OOBBehaviorNamePluralNameMismatch(object arg0, object arg1, object arg2) => ServerResources.Format(nameof (OOBBehaviorNamePluralNameMismatch), arg0, arg1, arg2);

    public static string OOBBehaviorNamePluralNameMismatch(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (OOBBehaviorNamePluralNameMismatch), culture, arg0, arg1, arg2);
    }

    public static string ChangedByEmptyWithBypassRulesException() => ServerResources.Get(nameof (ChangedByEmptyWithBypassRulesException));

    public static string ChangedByEmptyWithBypassRulesException(CultureInfo culture) => ServerResources.Get(nameof (ChangedByEmptyWithBypassRulesException), culture);

    public static string MacroDoesNotAcceptParameters(object arg0) => ServerResources.Format(nameof (MacroDoesNotAcceptParameters), arg0);

    public static string MacroDoesNotAcceptParameters(object arg0, CultureInfo culture) => ServerResources.Format(nameof (MacroDoesNotAcceptParameters), culture, arg0);

    public static string IdentityEmailNotViewable() => ServerResources.Get(nameof (IdentityEmailNotViewable));

    public static string IdentityEmailNotViewable(CultureInfo culture) => ServerResources.Get(nameof (IdentityEmailNotViewable), culture);

    public static string InvalidConstantIdentity(object arg0) => ServerResources.Format(nameof (InvalidConstantIdentity), arg0);

    public static string InvalidConstantIdentity(object arg0, CultureInfo culture) => ServerResources.Format(nameof (InvalidConstantIdentity), culture, arg0);

    public static string WorkItemIdentityNotResovledByName() => ServerResources.Get(nameof (WorkItemIdentityNotResovledByName));

    public static string WorkItemIdentityNotResovledByName(CultureInfo culture) => ServerResources.Get(nameof (WorkItemIdentityNotResovledByName), culture);

    public static string StateDoing() => ServerResources.Get(nameof (StateDoing));

    public static string StateDoing(CultureInfo culture) => ServerResources.Get(nameof (StateDoing), culture);

    public static string StateToDo() => ServerResources.Get(nameof (StateToDo));

    public static string StateToDo(CultureInfo culture) => ServerResources.Get(nameof (StateToDo), culture);

    public static string GitHubAppCannotFetchInstallation(object arg0) => ServerResources.Format(nameof (GitHubAppCannotFetchInstallation), arg0);

    public static string GitHubAppCannotFetchInstallation(object arg0, CultureInfo culture) => ServerResources.Format(nameof (GitHubAppCannotFetchInstallation), culture, arg0);

    public static string GitHubInstallationIdNotFound() => ServerResources.Get(nameof (GitHubInstallationIdNotFound));

    public static string GitHubInstallationIdNotFound(CultureInfo culture) => ServerResources.Get(nameof (GitHubInstallationIdNotFound), culture);

    public static string QueryHasTooManyAADGroupsToExpand() => ServerResources.Get(nameof (QueryHasTooManyAADGroupsToExpand));

    public static string QueryHasTooManyAADGroupsToExpand(CultureInfo culture) => ServerResources.Get(nameof (QueryHasTooManyAADGroupsToExpand), culture);

    public static string QueryHasTooManyAADGroupsToExpand_DiscloseCountAndLimit(
      object arg0,
      object arg1)
    {
      return ServerResources.Format(nameof (QueryHasTooManyAADGroupsToExpand_DiscloseCountAndLimit), arg0, arg1);
    }

    public static string QueryHasTooManyAADGroupsToExpand_DiscloseCountAndLimit(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (QueryHasTooManyAADGroupsToExpand_DiscloseCountAndLimit), culture, arg0, arg1);
    }

    public static string SenderWorkItemAccessDenied() => ServerResources.Get(nameof (SenderWorkItemAccessDenied));

    public static string SenderWorkItemAccessDenied(CultureInfo culture) => ServerResources.Get(nameof (SenderWorkItemAccessDenied), culture);

    public static string ProcessUpdateBlockedSystemTemplate() => ServerResources.Get(nameof (ProcessUpdateBlockedSystemTemplate));

    public static string ProcessUpdateBlockedSystemTemplate(CultureInfo culture) => ServerResources.Get(nameof (ProcessUpdateBlockedSystemTemplate), culture);

    public static string ProcessUpdateBlockedSystemTemplateOnPrem() => ServerResources.Get(nameof (ProcessUpdateBlockedSystemTemplateOnPrem));

    public static string ProcessUpdateBlockedSystemTemplateOnPrem(CultureInfo culture) => ServerResources.Get(nameof (ProcessUpdateBlockedSystemTemplateOnPrem), culture);

    public static string ServiceEndpointAuthSchemeNotSupported() => ServerResources.Get(nameof (ServiceEndpointAuthSchemeNotSupported));

    public static string ServiceEndpointAuthSchemeNotSupported(CultureInfo culture) => ServerResources.Get(nameof (ServiceEndpointAuthSchemeNotSupported), culture);

    public static string GitHubBoardsRepositoriesAlreadyRegistered(object arg0) => ServerResources.Format(nameof (GitHubBoardsRepositoriesAlreadyRegistered), arg0);

    public static string GitHubBoardsRepositoriesAlreadyRegistered(object arg0, CultureInfo culture) => ServerResources.Format(nameof (GitHubBoardsRepositoriesAlreadyRegistered), culture, arg0);

    public static string GitHubBoardsCannotCreateConnection() => ServerResources.Get(nameof (GitHubBoardsCannotCreateConnection));

    public static string GitHubBoardsCannotCreateConnection(CultureInfo culture) => ServerResources.Get(nameof (GitHubBoardsCannotCreateConnection), culture);

    public static string CreateGitHubPullRequest_ApiError(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ServerResources.Format(nameof (CreateGitHubPullRequest_ApiError), arg0, arg1, arg2, arg3);
    }

    public static string CreateGitHubPullRequest_ApiError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (CreateGitHubPullRequest_ApiError), culture, arg0, arg1, arg2, arg3);
    }

    public static string CreateGitHubBranchRef_ApiError(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ServerResources.Format(nameof (CreateGitHubBranchRef_ApiError), arg0, arg1, arg2, arg3);
    }

    public static string CreateGitHubBranchRef_ApiError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (CreateGitHubBranchRef_ApiError), culture, arg0, arg1, arg2, arg3);
    }

    public static string CreateGitHubFileContent_ApiError(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ServerResources.Format(nameof (CreateGitHubFileContent_ApiError), arg0, arg1, arg2, arg3);
    }

    public static string CreateGitHubFileContent_ApiError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (CreateGitHubFileContent_ApiError), culture, arg0, arg1, arg2, arg3);
    }

    public static string GetGitHubReadmeFileContentData_ApiError(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServerResources.Format(nameof (GetGitHubReadmeFileContentData_ApiError), arg0, arg1, arg2);
    }

    public static string GetGitHubReadmeFileContentData_ApiError(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (GetGitHubReadmeFileContentData_ApiError), culture, arg0, arg1, arg2);
    }

    public static string GetGitHubReadmeFileContentData_FileNotFound(object arg0, object arg1) => ServerResources.Format(nameof (GetGitHubReadmeFileContentData_FileNotFound), arg0, arg1);

    public static string GetGitHubReadmeFileContentData_FileNotFound(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (GetGitHubReadmeFileContentData_FileNotFound), culture, arg0, arg1);
    }

    public static string UpdateGitHubFileContent_ApiError(
      object arg0,
      object arg1,
      object arg2,
      object arg3)
    {
      return ServerResources.Format(nameof (UpdateGitHubFileContent_ApiError), arg0, arg1, arg2, arg3);
    }

    public static string UpdateGitHubFileContent_ApiError(
      object arg0,
      object arg1,
      object arg2,
      object arg3,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (UpdateGitHubFileContent_ApiError), culture, arg0, arg1, arg2, arg3);
    }

    public static string GetDefaultGitHubRepositoryBranch_ApiError(object arg0, object arg1) => ServerResources.Format(nameof (GetDefaultGitHubRepositoryBranch_ApiError), arg0, arg1);

    public static string GetDefaultGitHubRepositoryBranch_ApiError(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (GetDefaultGitHubRepositoryBranch_ApiError), culture, arg0, arg1);
    }

    public static string GetGitHubBranchRef_ApiError(object arg0, object arg1, object arg2) => ServerResources.Format(nameof (GetGitHubBranchRef_ApiError), arg0, arg1, arg2);

    public static string GetGitHubBranchRef_ApiError(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (GetGitHubBranchRef_ApiError), culture, arg0, arg1, arg2);
    }

    public static string CurrentProjectNotFound() => ServerResources.Get(nameof (CurrentProjectNotFound));

    public static string CurrentProjectNotFound(CultureInfo culture) => ServerResources.Get(nameof (CurrentProjectNotFound), culture);

    public static string GitHubGetRepositories_ApiError(object arg0) => ServerResources.Format(nameof (GitHubGetRepositories_ApiError), arg0);

    public static string GitHubGetRepositories_ApiError(object arg0, CultureInfo culture) => ServerResources.Format(nameof (GitHubGetRepositories_ApiError), culture, arg0);

    public static string GitHubGetPullRequests_ApiError(object arg0) => ServerResources.Format(nameof (GitHubGetPullRequests_ApiError), arg0);

    public static string GitHubGetPullRequests_ApiError(object arg0, CultureInfo culture) => ServerResources.Format(nameof (GitHubGetPullRequests_ApiError), culture, arg0);

    public static string ExternalConnectionOrRepoHasError(object arg0) => ServerResources.Format(nameof (ExternalConnectionOrRepoHasError), arg0);

    public static string ExternalConnectionOrRepoHasError(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ExternalConnectionOrRepoHasError), culture, arg0);

    public static string ExternalConnectionIsInvalid() => ServerResources.Get(nameof (ExternalConnectionIsInvalid));

    public static string ExternalConnectionIsInvalid(CultureInfo culture) => ServerResources.Get(nameof (ExternalConnectionIsInvalid), culture);

    public static string OrgNodeIdInvalid(object arg0) => ServerResources.Format(nameof (OrgNodeIdInvalid), arg0);

    public static string OrgNodeIdInvalid(object arg0, CultureInfo culture) => ServerResources.Format(nameof (OrgNodeIdInvalid), culture, arg0);

    public static string MultipleUpdatesToSameField(object arg0, object arg1) => ServerResources.Format(nameof (MultipleUpdatesToSameField), arg0, arg1);

    public static string MultipleUpdatesToSameField(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (MultipleUpdatesToSameField), culture, arg0, arg1);

    public static string ExternalConnectionInvalidCredentials() => ServerResources.Get(nameof (ExternalConnectionInvalidCredentials));

    public static string ExternalConnectionInvalidCredentials(CultureInfo culture) => ServerResources.Get(nameof (ExternalConnectionInvalidCredentials), culture);

    public static string ExternalConnectionGitHubAppUninstalled() => ServerResources.Get(nameof (ExternalConnectionGitHubAppUninstalled));

    public static string ExternalConnectionGitHubAppUninstalled(CultureInfo culture) => ServerResources.Get(nameof (ExternalConnectionGitHubAppUninstalled), culture);

    public static string ExternalConnectionOAuthConfigurationDeleted() => ServerResources.Get(nameof (ExternalConnectionOAuthConfigurationDeleted));

    public static string ExternalConnectionOAuthConfigurationDeleted(CultureInfo culture) => ServerResources.Get(nameof (ExternalConnectionOAuthConfigurationDeleted), culture);

    public static string ExternalConnectionUnreachableRepositories(object arg0) => ServerResources.Format(nameof (ExternalConnectionUnreachableRepositories), arg0);

    public static string ExternalConnectionUnreachableRepositories(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ExternalConnectionUnreachableRepositories), culture, arg0);

    public static string ExternalConnectionRepositoryIdAlreadyExist(object arg0) => ServerResources.Format(nameof (ExternalConnectionRepositoryIdAlreadyExist), arg0);

    public static string ExternalConnectionRepositoryIdAlreadyExist(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (ExternalConnectionRepositoryIdAlreadyExist), culture, arg0);
    }

    public static string ExternalConnectionFailedToAddRepository() => ServerResources.Get(nameof (ExternalConnectionFailedToAddRepository));

    public static string ExternalConnectionFailedToAddRepository(CultureInfo culture) => ServerResources.Get(nameof (ExternalConnectionFailedToAddRepository), culture);

    public static string RepositoriesWithMappingToDifferentOrganization(object arg0) => ServerResources.Format(nameof (RepositoriesWithMappingToDifferentOrganization), arg0);

    public static string RepositoriesWithMappingToDifferentOrganization(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (RepositoriesWithMappingToDifferentOrganization), culture, arg0);
    }

    public static string PortfolioBehaviorSqlLimitExceeded() => ServerResources.Get(nameof (PortfolioBehaviorSqlLimitExceeded));

    public static string PortfolioBehaviorSqlLimitExceeded(CultureInfo culture) => ServerResources.Get(nameof (PortfolioBehaviorSqlLimitExceeded), culture);

    public static string validateNarrowBreakSpaceForConstants(object arg0) => ServerResources.Format(nameof (validateNarrowBreakSpaceForConstants), arg0);

    public static string validateNarrowBreakSpaceForConstants(object arg0, CultureInfo culture) => ServerResources.Format(nameof (validateNarrowBreakSpaceForConstants), culture, arg0);

    public static string ExternalConnectionNoRepositories() => ServerResources.Get(nameof (ExternalConnectionNoRepositories));

    public static string ExternalConnectionNoRepositories(CultureInfo culture) => ServerResources.Get(nameof (ExternalConnectionNoRepositories), culture);

    public static string AddRepoToInstallation_ApiError(object arg0) => ServerResources.Format(nameof (AddRepoToInstallation_ApiError), arg0);

    public static string AddRepoToInstallation_ApiError(object arg0, CultureInfo culture) => ServerResources.Format(nameof (AddRepoToInstallation_ApiError), culture, arg0);

    public static string GitHubMaximumRepoCountExceededError(object arg0) => ServerResources.Format(nameof (GitHubMaximumRepoCountExceededError), arg0);

    public static string GitHubMaximumRepoCountExceededError(object arg0, CultureInfo culture) => ServerResources.Format(nameof (GitHubMaximumRepoCountExceededError), culture, arg0);

    public static string MissingSeviceEndpointAuthorization() => ServerResources.Get(nameof (MissingSeviceEndpointAuthorization));

    public static string MissingSeviceEndpointAuthorization(CultureInfo culture) => ServerResources.Get(nameof (MissingSeviceEndpointAuthorization), culture);

    public static string InvalidAuthorizationScheme() => ServerResources.Get(nameof (InvalidAuthorizationScheme));

    public static string InvalidAuthorizationScheme(CultureInfo culture) => ServerResources.Get(nameof (InvalidAuthorizationScheme), culture);

    public static string DeploymentGroupLabel() => ServerResources.Get(nameof (DeploymentGroupLabel));

    public static string DeploymentGroupLabel(CultureInfo culture) => ServerResources.Get(nameof (DeploymentGroupLabel), culture);

    public static string GitHubEnterpriseVersionUnableToBeVerified() => ServerResources.Get(nameof (GitHubEnterpriseVersionUnableToBeVerified));

    public static string GitHubEnterpriseVersionUnableToBeVerified(CultureInfo culture) => ServerResources.Get(nameof (GitHubEnterpriseVersionUnableToBeVerified), culture);

    public static string FormLayoutInvalidSystemControlException(object arg0) => ServerResources.Format(nameof (FormLayoutInvalidSystemControlException), arg0);

    public static string FormLayoutInvalidSystemControlException(object arg0, CultureInfo culture) => ServerResources.Format(nameof (FormLayoutInvalidSystemControlException), culture, arg0);

    public static string FormLayoutSystemControlDoesNotExistException(object arg0) => ServerResources.Format(nameof (FormLayoutSystemControlDoesNotExistException), arg0);

    public static string FormLayoutSystemControlDoesNotExistException(
      object arg0,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (FormLayoutSystemControlDoesNotExistException), culture, arg0);
    }

    public static string FieldRefNameMustDifferFromName() => ServerResources.Get(nameof (FieldRefNameMustDifferFromName));

    public static string FieldRefNameMustDifferFromName(CultureInfo culture) => ServerResources.Get(nameof (FieldRefNameMustDifferFromName), culture);

    public static string FieldHasNotBeenDeleted() => ServerResources.Get(nameof (FieldHasNotBeenDeleted));

    public static string FieldHasNotBeenDeleted(CultureInfo culture) => ServerResources.Get(nameof (FieldHasNotBeenDeleted), culture);

    public static string ActiveWorkItemsExist(object arg0) => ServerResources.Format(nameof (ActiveWorkItemsExist), arg0);

    public static string ActiveWorkItemsExist(object arg0, CultureInfo culture) => ServerResources.Format(nameof (ActiveWorkItemsExist), culture, arg0);

    public static string InvalidCompletedCatagoryStateNamesForXMLProcess(object arg0, object arg1) => ServerResources.Format(nameof (InvalidCompletedCatagoryStateNamesForXMLProcess), arg0, arg1);

    public static string InvalidCompletedCatagoryStateNamesForXMLProcess(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (InvalidCompletedCatagoryStateNamesForXMLProcess), culture, arg0, arg1);
    }

    public static string CompletedCategoryNotHasOneState(object arg0, object arg1) => ServerResources.Format(nameof (CompletedCategoryNotHasOneState), arg0, arg1);

    public static string CompletedCategoryNotHasOneState(
      object arg0,
      object arg1,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (CompletedCategoryNotHasOneState), culture, arg0, arg1);
    }

    public static string FieldNotExistInWorkItemType(object arg0, object arg1) => ServerResources.Format(nameof (FieldNotExistInWorkItemType), arg0, arg1);

    public static string FieldNotExistInWorkItemType(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (FieldNotExistInWorkItemType), culture, arg0, arg1);

    public static string TooManyRevisionsForHistoryAPI(object arg0, object arg1, object arg2) => ServerResources.Format(nameof (TooManyRevisionsForHistoryAPI), arg0, arg1, arg2);

    public static string TooManyRevisionsForHistoryAPI(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (TooManyRevisionsForHistoryAPI), culture, arg0, arg1, arg2);
    }

    public static string NullOrEmptyParameter(object arg0) => ServerResources.Format(nameof (NullOrEmptyParameter), arg0);

    public static string NullOrEmptyParameter(object arg0, CultureInfo culture) => ServerResources.Format(nameof (NullOrEmptyParameter), culture, arg0);

    public static string Validation_ElementError(object arg0, object arg1) => ServerResources.Format(nameof (Validation_ElementError), arg0, arg1);

    public static string Validation_ElementError(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (Validation_ElementError), culture, arg0, arg1);

    public static string Validation_MissingAttributeValue(object arg0) => ServerResources.Format(nameof (Validation_MissingAttributeValue), arg0);

    public static string Validation_MissingAttributeValue(object arg0, CultureInfo culture) => ServerResources.Format(nameof (Validation_MissingAttributeValue), culture, arg0);

    public static string InvalidHistoryFieldValue(object arg0) => ServerResources.Format(nameof (InvalidHistoryFieldValue), arg0);

    public static string InvalidHistoryFieldValue(object arg0, CultureInfo culture) => ServerResources.Format(nameof (InvalidHistoryFieldValue), culture, arg0);

    public static string FieldAllowedValuesMismatch(object arg0, object arg1) => ServerResources.Format(nameof (FieldAllowedValuesMismatch), arg0, arg1);

    public static string FieldAllowedValuesMismatch(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (FieldAllowedValuesMismatch), culture, arg0, arg1);

    public static string FailedSetRecordValues(object arg0, object arg1) => ServerResources.Format(nameof (FailedSetRecordValues), arg0, arg1);

    public static string FailedSetRecordValues(object arg0, object arg1, CultureInfo culture) => ServerResources.Format(nameof (FailedSetRecordValues), culture, arg0, arg1);

    public static string InvitedUserEmailException(object arg0) => ServerResources.Format(nameof (InvitedUserEmailException), arg0);

    public static string InvitedUserEmailException(object arg0, CultureInfo culture) => ServerResources.Format(nameof (InvitedUserEmailException), culture, arg0);

    public static string TooManyRevisionsForWorkItemUpdateAPI(
      object arg0,
      object arg1,
      object arg2)
    {
      return ServerResources.Format(nameof (TooManyRevisionsForWorkItemUpdateAPI), arg0, arg1, arg2);
    }

    public static string TooManyRevisionsForWorkItemUpdateAPI(
      object arg0,
      object arg1,
      object arg2,
      CultureInfo culture)
    {
      return ServerResources.Format(nameof (TooManyRevisionsForWorkItemUpdateAPI), culture, arg0, arg1, arg2);
    }

    public static string QueryCrossProjectPermissionException(object arg0) => ServerResources.Format(nameof (QueryCrossProjectPermissionException), arg0);

    public static string QueryCrossProjectPermissionException(object arg0, CultureInfo culture) => ServerResources.Format(nameof (QueryCrossProjectPermissionException), culture, arg0);

    public static string WorkItemLockingNotAllowedOrDisabled() => ServerResources.Get(nameof (WorkItemLockingNotAllowedOrDisabled));

    public static string WorkItemLockingNotAllowedOrDisabled(CultureInfo culture) => ServerResources.Get(nameof (WorkItemLockingNotAllowedOrDisabled), culture);

    public static string GitHubBoardsBotLinkingErrorHeading() => ServerResources.Get(nameof (GitHubBoardsBotLinkingErrorHeading));

    public static string GitHubBoardsBotLinkingErrorHeading(CultureInfo culture) => ServerResources.Get(nameof (GitHubBoardsBotLinkingErrorHeading), culture);

    public static string GitHubBoardsBotLinkingErrorSuggestion() => ServerResources.Get(nameof (GitHubBoardsBotLinkingErrorSuggestion));

    public static string GitHubBoardsBotLinkingErrorSuggestion(CultureInfo culture) => ServerResources.Get(nameof (GitHubBoardsBotLinkingErrorSuggestion), culture);

    public static string GitHubBoardsBotLinkingSuccessHeading() => ServerResources.Get(nameof (GitHubBoardsBotLinkingSuccessHeading));

    public static string GitHubBoardsBotLinkingSuccessHeading(CultureInfo culture) => ServerResources.Get(nameof (GitHubBoardsBotLinkingSuccessHeading), culture);

    public static string GitHubGetCommits_ApiError(object arg0) => ServerResources.Format(nameof (GitHubGetCommits_ApiError), arg0);

    public static string GitHubGetCommits_ApiError(object arg0, CultureInfo culture) => ServerResources.Format(nameof (GitHubGetCommits_ApiError), culture, arg0);
  }
}
